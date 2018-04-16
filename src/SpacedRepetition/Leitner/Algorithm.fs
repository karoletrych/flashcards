namespace Flashcards.SpacedRepetition.Leitner
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Interface
open Flashcards.SpacedRepetition.Leitner.Models
open Flashcards.Settings
open System.Threading.Tasks
open System.Linq

module Algorithm =

    let toInt c = 
        c |> System.Char.GetNumericValue |> int
    let isDigit (i : int) (c : char) = 
        System.Char.IsDigit(c) 
        && c|> toInt = i 

    // https://en.wikipedia.org/wiki/Leitner_system
    let rearrangeCards 
        results 
        sessionNumber 
        (decks : Deck seq)= 
            let deckBeginningWithSession session = 
                decks
                |> Seq.pick (fun d -> 
                    match d.DeckTitle with
                    | digits when digits.[0] |> isDigit sessionNumber ->
                        Some d
                    | _ -> 
                        None)
            let findDeck deckName (decks : Deck seq) : Deck = 
                decks
                |> Seq.find (fun d -> d.DeckTitle = deckName)
            results
            |> Seq.iter (fun (card, known, deck : Deck) ->
                match deck.DeckTitle with
                // If a learner is successful at a card from Deck Current,
                // it gets transferred into the progress deck that begins with 
                // that session's number.
                | CurrentDeckTitle when known ->
                    deck.Cards.Remove(card) |> ignore
                    (deckBeginningWithSession sessionNumber).Cards.Add(card)
                // If a learner has difficulty with a card during a subsequent review, 
                // the card is returned to Deck Current; 
                | _ when not known ->
                    deck.Cards.Remove(card) |> ignore
                    (decks |> findDeck CurrentDeckTitle).Cards.Add(card)
                // When a learner is successful at a card during a session that matches 
                // the last number on the deck that card goes into Deck Retired
                | deckTitle when known && (deckTitle |> Seq.last |> toInt) = sessionNumber ->
                    deck.Cards.Remove(card) |> ignore
                    (decks |> findDeck RetiredDeckTitle).Cards.Add(card)
                | _ -> ())
            decks
            
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

    type SessionNumber(
                        repetitionDoneTodaySetting : ISetting<bool>,
                        sessionNumberSetting : ISetting<int>,
                        streakDaysSetting : ISetting<int>) = 
        interface ISessionNumber with
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

     
    type LeitnerRepetition(
                            deckRepository : IRepository<Deck>,
                            sessionNumberSetting : ISetting<int>,
                            repetitionDoneTodaySetting : ISetting<bool>,
                            streakDaysSetting : ISetting<int>) =
        member this.allDecks () = 
            deckRepository.FindAll()
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
                let newDecks = 
                    rearrangeCards 
                        cardsWithDecks
                        sessionNumberSetting.Value
                        decks

                deckRepository.InsertOrReplaceAll(newDecks)
                |> sync

                repetitionDoneTodaySetting.Value <- true
                streakDaysSetting.Value <- streakDaysSetting.Value + 1

            member this.LearnedFlashcards 
                with get () =
                    let cards = 
                        deckRepository.FindWhere(fun deck -> deck.DeckTitle = RetiredDeckTitle)
                        |> Async.AwaitTask
                        |> Async.RunSynchronously
                        |> Seq.exactlyOne
                        |> (fun deck -> deck.Cards)
                    cards.AsEnumerable()
                    
                
                