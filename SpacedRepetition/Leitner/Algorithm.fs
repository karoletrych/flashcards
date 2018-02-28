namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open System.Linq
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider
open Flashcards.SpacedRepetition.Leitner.Models
open System.Threading.Tasks

module Algorithm =
    let progressDecks =
        Map.toList 
        >> List.choose (fun (d,cards) -> 
            match d with 
            | ProgressDeck pd -> Some (pd, cards) 
            | _ -> None)
        >> Map.ofList

    let cardsInSession sessionNumber (allDecks : Map<DeckTitle, List<int>>) = 
        allDecks
        |> progressDecks
        |> Map.filter (fun k _ -> k |> List.contains sessionNumber)
        |> Map.toList
        |> List.collect (fun deck -> snd deck |> Seq.toList)
        
    // https://en.wikipedia.org/wiki/Leitner_system
    let rearrangeCards 
        results 
        sessionNumber 
        (decks : Map<DeckTitle, List<CardId>>) = 
            let deckBeginningWithSession session = 
                decks
                |> progressDecks 
                |> Map.pick (fun d c -> 
                    match d |> List.head = session with
                    | true -> Some c
                    | false -> None)
            results
            |> Seq.iter (fun struct (card, known, deckTitle) ->
                let deck = decks.[deckTitle]
                match deckTitle with
                // If a learner is successful at a card from Deck Current,
                // it gets transferred into the progress deck that begins with 
                // that session's number.
                | CurrentDeck when known ->
                    deck.Remove(card) |> ignore
                    (deckBeginningWithSession sessionNumber).Add(card)
                // If a learner has difficulty with a card during a subsequent review, 
                // the card is returned to Deck Current; 
                | ProgressDeck _ when not known ->
                    deck.Remove(card) |> ignore
                    decks.[CurrentDeck].Add(card)
                // When a learner is successful at a card during a session that matches 
                // the last number on the deck that card goes into Deck Retired
                | ProgressDeck deckTitle when known && List.last deckTitle = sessionNumber ->
                    deck.Remove(card) |> ignore
                    decks.[RetiredDeck].Add(card)
                | _ -> ())
            decks

    type IProperties = 
        abstract member Get : string -> obj
        abstract member Set : string -> obj -> unit
        abstract member ContainsKey : string -> bool

    type LeitnerRepetition(cardDeckRepository : IRepository<CardDeck>, 
                           flashcardRepository : IRepository<Flashcard>,
                           properties : IProperties) =
        member this.allDecks () = 
            let deckCurrent = {
                Title = CurrentDeck;
                Cards = 
                    (cardDeckRepository.FindMatching(fun cd -> cd.DeckTitle = DeckTitleEnum.CurrentDeck))
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                    |> Seq.map (fun cd -> cd.CardId)
                    |> List                             
            }

            let deckRetired = {
                Title = RetiredDeck; 
                Cards = 
                    cardDeckRepository.FindMatching(fun cd -> cd.DeckTitle = DeckTitleEnum.RetiredDeck)
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                    |> Seq.map (fun cd -> cd.CardId)
                    |> List
            }
                
            let progressDecks = 
                progressDeckTitles
                |> List.map ProgressDeck
                |> List.map (fun progressDeck -> 
                    let enum = Models.toDeckTitleEnum progressDeck
                    (progressDeck, 
                        cardDeckRepository.FindMatching(fun cd -> cd.DeckTitle = enum)
                        |> Async.AwaitTask
                        |> Async.RunSynchronously
                        |> Seq.map (fun cd -> cd.CardId)
                        |> List
                ))
                |> Map.ofList
            let allDecks = 
                progressDecks 
                |> Map.add CurrentDeck deckCurrent.Cards
                |> Map.add RetiredDeck deckRetired.Cards
            allDecks
        member this.sessionNumber () = 
            if not (properties.ContainsKey "LeitnerSessionNumber")
            then 
                (properties.Set "LeitnerSessionNumber" 0)
                0
            else 
                let number = (properties.Get "LeitnerSessionNumber") :?> int
                number
        member this.incrementSessionNumber () = 
            let sessionNumber = this.sessionNumber ()
            if sessionNumber = 9
            then
                properties.Set "LeitnerSessionNumber" 0
            else
                properties.Set "LeitnerSessionNumber" (sessionNumber + 1)
        interface ISpacedRepetition with 
            member this.ChooseFlashcards () = 
                let flashcards = 
                    let allDecks = this.allDecks()
                    allDecks
                    |> cardsInSession (this.sessionNumber()) 
                    |> List.append (allDecks.[CurrentDeck] |> Seq.toList)
                    |> List.map (fun id -> 
                            flashcardRepository.FindMatching(fun f -> f.Id = id) 
                            |> Async.AwaitTask 
                            |> Async.RunSynchronously
                            |> Seq.exactlyOne)
                flashcards 
                |> List.toSeq 
                |> Task.FromResult
            member this.RearrangeFlashcards results =
                let carddecks = 
                    cardDeckRepository.FindAll()
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                
                let results = 
                    results
                    |> Seq.map (fun struct (c, r) -> struct (c.Id, r, carddecks |> Seq.find(fun cd -> cd.CardId = c.Id) |> fun cd -> toDeckTitle cd.DeckTitle))

                let newDecks = rearrangeCards results (this.sessionNumber())  (this.allDecks())
                newDecks
                |> Map.toSeq
                |> Seq.iter (fun (d, cards) -> 
                    cards.ForEach(fun c->
                        (cardDeckRepository.Update(CardDeck(CardId = c, DeckTitle = toDeckTitleEnum d)) 
                        |> Async.AwaitTask 
                        |> Async.RunSynchronously 
                        |> ignore)
                        )
                    )
                this.incrementSessionNumber ()
