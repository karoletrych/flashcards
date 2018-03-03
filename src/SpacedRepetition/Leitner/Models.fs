namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open SQLite
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider
open SQLiteNetExtensions.Attributes
open Flashcards.Services.DataAccess.Database
open System.Threading.Tasks

module Models =

    [<Literal>]
    let CurrentDeckName = "CurrentDeck"
    [<Literal>]
    let RetiredDeckName = "RetiredDeck"

    let deckTitles = [
        CurrentDeckName;
        RetiredDeckName;
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
    let inline startAsPlainTask (work : Async<unit>) = Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)

    type Deck () = 
        [<PrimaryKey>]
        [<AutoIncrement>]
        member val Id = 0 with get, set

        [<Indexed>]
        member val DeckTitle = CurrentDeckName with get, set

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
                let insertIntoDeck (flashcard : Flashcard) = async{
                        let! decks =
                            (deckRepository.FindMatching(fun d -> d.DeckTitle = CurrentDeckName)) 
                            |> Async.AwaitTask
                        let deck = decks |> Seq.exactlyOne
                        deck.Cards.Add(flashcard)
                        do! deckRepository.Update(deck) |> Async.AwaitTask
                    }
                async{
                    do! tableCreator.CreateTable<CardDeck>() 
                        |> Async.AwaitTask 
                        |> Async.Ignore
                    do! tableCreator.CreateTable<Deck>() 
                        |> Async.AwaitTask 
                        |> Async.Ignore

                    do! deckRepository.UpdateAll(
                            deckTitles 
                            |> List.map (fun title -> Deck(DeckTitle = title)))
                            |> Async.AwaitTask
                    flashcardRepository.ObjectInserted.Add(fun f -> (insertIntoDeck f) |> Async.StartImmediate)
                } 
                |> startAsPlainTask

