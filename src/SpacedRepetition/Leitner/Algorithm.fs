namespace Flashcards.SpacedRepetition.Leitner
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider
open Flashcards.SpacedRepetition.Leitner.Models
open System.Threading.Tasks

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
            |> Seq.iter (fun struct (card, known, deck : Deck) ->
                match deck.DeckTitle with
                // If a learner is successful at a card from Deck Current,
                // it gets transferred into the progress deck that begins with 
                // that session's number.
                | CurrentDeckName when known ->
                    deck.Cards.Remove(card) |> ignore
                    (deckBeginningWithSession sessionNumber).Cards.Add(card)
                // If a learner has difficulty with a card during a subsequent review, 
                // the card is returned to Deck Current; 
                | _ when not known ->
                    deck.Cards.Remove(card) |> ignore
                    (decks |> findDeck CurrentDeckName).Cards.Add(card)
                // When a learner is successful at a card during a session that matches 
                // the last number on the deck that card goes into Deck Retired
                | deckTitle when known && (deckTitle |> Seq.last |> toInt) = sessionNumber ->
                    deck.Cards.Remove(card) |> ignore
                    (decks |> findDeck RetiredDeckName).Cards.Add(card)
                | _ -> ())
            decks
            
    type IProperties = 
        abstract member Get : string -> obj
        abstract member Set : string -> obj -> unit
        abstract member ContainsKey : string -> bool

    type LeitnerRepetition(deckRepository : IRepository<Deck>, 
                           properties : IProperties) =
        let sessionNumberKey = "LeitnerSessionNumber"
        member this.allDecks () = 
            deckRepository.FindAll()
            |> Async.AwaitTask
            |> Async.RunSynchronously
                
        member this.sessionNumber () = 
            if not (properties.ContainsKey sessionNumberKey)
            then 
                (properties.Set sessionNumberKey 0)
                0
            else 
                let number = (properties.Get sessionNumberKey) :?> int
                number
        member this.incrementSessionNumber () = 
            let sessionNumber = this.sessionNumber ()
            if sessionNumber = 9
            then
                properties.Set sessionNumberKey 0
            else
                properties.Set sessionNumberKey (sessionNumber + 1)
        interface ISpacedRepetition with 
            member this.ChooseFlashcards () = 
                let cardsToAsk sessionNumber decks = 
                    decks
                    |> Seq.filter (fun (deck : Deck) -> 
                        (deck.DeckTitle |> Seq.map toInt |> Seq.contains sessionNumber 
                        || deck.DeckTitle = CurrentDeckName))
                    |> Seq.collect (fun deck -> deck.Cards)
                
                let decks = this.allDecks()
                decks
                |> cardsToAsk (this.sessionNumber())
                |> Task.FromResult
                
            member this.RearrangeFlashcards results =
                let decks = this.allDecks()
                
                let deckWithCard (card : Flashcard) = 
                    decks 
                    |> Seq.find(fun deck -> 
                        deck.Cards 
                        |> Seq.exists (fun c -> c.Id = card.Id))

                let cardsWithDecks = 
                    results
                    |> Seq.map (fun struct (card, known) -> 
                                    struct (card, known, deckWithCard card))
                let newDecks = 
                    rearrangeCards 
                        cardsWithDecks
                        (this.sessionNumber())
                        (decks)

                deckRepository.UpdateAll(newDecks)
                |> Async.AwaitTask
                |> Async.RunSynchronously

                this.incrementSessionNumber ()
                