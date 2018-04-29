namespace Flashcards.Domain.SpacedRepetition.Leitner
open Flashcards.Domain.Services.DataAccess
open Flashcards.Domain.Models
open Flashcards.Domain.SpacedRepetition.Interface
open Flashcards.Domain.SpacedRepetition.Leitner.Models
open Flashcards.Infrastructure.Settings
open System.Threading.Tasks
open System.Linq

module Algorithm =

    let toInt c = 
        c |> System.Char.GetNumericValue |> int
    let isDigit (i : int) (c : char) = 
        System.Char.IsDigit(c) 
        && c|> toInt = i 
    
    type MoveOperation = {
        Flashcard : Flashcard;
        SourceDeck : string;
        DestinationDeck : string;
    }

    // https://en.wikipedia.org/wiki/Leitner_system
    let rearrangeCards 
        repetitionResults 
        sessionNumber =
            let deckBeginningWithSession session = 
                deckTitles
                |> Seq.pick (fun d -> 
                    match d with
                    | digits when digits.[0] |> isDigit sessionNumber ->
                        Some d
                    | _ -> 
                        None)
            repetitionResults
            |> Seq.choose (fun (card, known, deck : Deck) ->
                match deck.DeckTitle with
                // If a learner is successful at a card from Deck Current,
                // it gets transferred into the progress deck that begins with 
                // that session's number.
                | CurrentDeckTitle when known ->
                    Some {Flashcard = card; SourceDeck = deck.DeckTitle; DestinationDeck = deckBeginningWithSession sessionNumber}
                // If a learner has difficulty with a card during a subsequent review, 
                // the card is returned to Deck Current; 
                | _ when not known ->
                    Some {Flashcard = card; SourceDeck = deck.DeckTitle; DestinationDeck = CurrentDeckTitle}
                // When a learner is successful at a card during a session that matches 
                // the last number on the deck that card goes into Deck Retired
                | deckTitle when known && (deckTitle |> Seq.last |> toInt) = sessionNumber ->
                    Some {Flashcard = card; SourceDeck = deck.DeckTitle; DestinationDeck = RetiredDeckTitle}
                | _ -> None)
            
    type SessionNumberSetting() =
        inherit Setting<int>() with
            override this.Key with get () = "SessionNumber"
            override this.DefaultValue with get () = 0

    type RepetitionDoneTodaySetting() =
        inherit Setting<bool>() with
            override this.Key with get () = "RepetitionDoneToday"
            override this.DefaultValue with get () = false

    type StreakDaysSetting() =
        inherit Setting<int>() with
            override this.Key with get () = "StreakDaysSetting"
            override this.DefaultValue with get () = 0

    type RepetitionSession(
                            repetitionDoneTodaySetting : ISetting<bool>,
                            sessionNumberSetting : ISetting<int>,
                            streakDaysSetting : ISetting<int>) = 
        interface IRepetitionSession with
            member this.Increment() =
                if not repetitionDoneTodaySetting.Value
                    then streakDaysSetting.Value <- 0

                repetitionDoneTodaySetting.Value <- false

                let sessionNumber = sessionNumberSetting.Value
                if sessionNumber < 9
                then 
                    sessionNumberSetting.Value <- sessionNumber + 1;
                else 
                    sessionNumberSetting.Value <- 0;
            override this.Value with get () = sessionNumberSetting.Value

     
    type LeitnerRepetition(
                            deckRepository : IRepository<Deck>,
                            cardDeckRepository : IRepository<CardDeck>,
                            sessionNumberSetting : ISetting<int>,
                            repetitionDoneTodaySetting : ISetting<bool>,
                            streakDaysSetting : ISetting<int>) =
        member this.allDecks () = 
            deckRepository.GetAllWithChildren(null, true)
            |> Async.AwaitTask
            |> Async.RunSynchronously
       
        interface ISpacedRepetition with 
            member this.CurrentRepetitionFlashcards () =
                if repetitionDoneTodaySetting.Value 
                then Task.FromResult Seq.empty
                else
                    let sessionNumber = sessionNumberSetting.Value
                
                    let decks = this.allDecks()
                    decks
                    |> Seq.filter (fun (deck : Deck) -> 
                                            (deck.DeckTitle
                                            |> Seq.map toInt 
                                            |> Seq.contains sessionNumber || deck.DeckTitle = CurrentDeckTitle))
                    |> Seq.collect (fun deck -> deck.Cards)
                    |> Task.FromResult
                
            member this.SubmitRepetitionResults (results) =
                async {
                    let decks = this.allDecks()
                    let cardsWithDecks = 
                        let parentDeck (card : Flashcard) = 
                            decks 
                            |> Seq.find(fun deck -> 
                                deck.Cards 
                                |> Seq.exists (fun c -> c.Id = card.Id))
                        results
                        |> Seq.map (fun result -> 
                                        (result.Flashcard, result.IsKnown, parentDeck result.Flashcard))
                    let decksMoveOperations = 
                        rearrangeCards 
                            cardsWithDecks
                            sessionNumberSetting.Value
                        |> Seq.toList

                    let decksMap = 
                        deckRepository.GetAllWithChildren(null, false) 
                        |> Async.AwaitTask 
                        |> Async.RunSynchronously
                        |> Seq.map (fun d -> (d.DeckTitle, d.Id))
                        |> Map.ofSeq

                    decksMoveOperations
                    |> Seq.iter (fun m ->
                        let sourceId = decksMap.[m.SourceDeck]
                        let destinationId = decksMap.[m.DestinationDeck]
                        let toRemove = cardDeckRepository.Single(fun cd -> cd.CardId = m.Flashcard.Id && cd.DeckId = sourceId) |> Async.AwaitTask |> Async.RunSynchronously
                        cardDeckRepository.Delete(toRemove) |> sync
                        cardDeckRepository.Insert(CardDeck(CardId = m.Flashcard.Id, DeckId = destinationId)) |> sync
                        )

                    repetitionDoneTodaySetting.Value <- true
                    streakDaysSetting.Value <- streakDaysSetting.Value + 1
                }
                |> Async.StartAsTask
                |> (fun t -> t :> Task)


            member this.LearnedFlashcards 
                with get () =
                    let cards = 
                        deckRepository.GetAllWithChildren((fun deck -> deck.DeckTitle = RetiredDeckTitle),  true)
                        |> Async.AwaitTask
                        |> Async.RunSynchronously
                        |> Seq.exactlyOne
                        |> (fun deck -> deck.Cards)
                    cards.AsEnumerable()
                    
                
                