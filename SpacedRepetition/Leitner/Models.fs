namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open SQLite
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider
open SQLiteNetExtensions.Attributes
open Flashcards.Services.DataAccess.Database

module Models =
    let currentDeckName = "CurrentDeck"
    let retiredDeckName = "RetiredDeck"

    let deckTitles = [
        currentDeckName;
        retiredDeckName;
        "0259";
        "1360";
        "2471";
        "3582";
        "4693";
        "5704";
        "6815";
        "7926";
        "8037";
        "9148";
    ]    

    type Deck () = 
        [<PrimaryKey>]
        [<AutoIncrement>]
        member val Id = 0 with get, set

        [<Indexed>]
        member val DeckTitle = currentDeckName with get, set

        [<ManyToMany(typeof<CardDeck>, CascadeOperations = CascadeOperation.All)>]
        member val Cards = List<Flashcard>() with get, set 
        
    and CardDeck () = 
        [<ForeignKey(typeof<Flashcard>)>]
        member val CardId = 0 with get, set

        [<ForeignKey(typeof<Deck>)>]
        member val DeckId = 0 with get, set

        [<AutoIncrement>]
        [<PrimaryKey>]
        member val CardDeckId = 0 with get, set


     type LeitnerInitializer
         (flashcardRepository : IRepository<Flashcard>,
          deckRepository : IRepository<Deck>,
           tableCreator : ITableCreator) =
        interface ISpacedRepetitionInitializer with
            member this.Initialize() =
                let insertIntoDeck (flashcard : Flashcard) =
                    let deck =
                        deckRepository.FindMatching(fun d -> d.DeckTitle = currentDeckName)
                        |> Async.AwaitTask
                        |> Async.RunSynchronously
                        |> Seq.exactlyOne
                    deck.Cards.Add(flashcard)
                    deckRepository.Update(deck) |> Async.AwaitTask |> Async.RunSynchronously |> ignore
                
                tableCreator.CreateTable<CardDeck>() |> ignore
                tableCreator.CreateTable<Deck>() |> ignore
                
                deckTitles 
                |> List.iter (fun title ->
                    deckRepository.Insert(Deck(DeckTitle = title))
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                    |> ignore)
                flashcardRepository.ObjectInserted.Add(fun f -> insertIntoDeck f)
