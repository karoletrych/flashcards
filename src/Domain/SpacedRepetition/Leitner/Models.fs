namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open SQLite
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Interface
open SQLiteNetExtensions.Attributes
open Flashcards.Services.DataAccess.Database


module Models =
    [<Literal>]
    let CurrentDeckTitle = "CurrentDeck"
    [<Literal>]
    let RetiredDeckTitle = "RetiredDeck"

    let deckTitles = [
        CurrentDeckTitle;
        RetiredDeckTitle;
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
    let inline sync (x : System.Threading.Tasks.Task) = x |> Async.AwaitTask |> Async.RunSynchronously
    let inline syncT x = x |> Async.AwaitTask |> Async.RunSynchronously
    

    type Deck () = 
        [<PrimaryKey>]
        member val Id = 0 with get, set

        [<Indexed>]
        member val DeckTitle = CurrentDeckTitle with get, set

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
         (notifier : INotifyObjectInserted<Flashcard>,
          deckRepository : IRepository<Deck>,
          cardDeckRepository : IRepository<CardDeck>,
          tableCreatorFunc : ITableCreator) =
        let deckCurrentId = 
            lazy(
                let decks =
                    (deckRepository.GetAllWithChildren((fun d -> d.DeckTitle = CurrentDeckTitle), false)) 
                    |> Async.AwaitTask 
                    |> Async.RunSynchronously
                decks |> Seq.exactlyOne |> (fun d -> d.Id)
            )

        interface ISpacedRepetitionInitializer with
            member this.Initialize() =
                let insertIntoDeck (flashcard : Flashcard) = 
                    cardDeckRepository.Insert(CardDeck(CardId = flashcard.Id, DeckId = deckCurrentId.Value))                        
                    |> Async.AwaitTask
                    |> Async.RunSynchronously

                tableCreatorFunc.CreateTable<CardDeck>() |> sync
                tableCreatorFunc.CreateTable<Deck>() |> sync
                if deckRepository.GetAllWithChildren(null, false) |> syncT |> Seq.isEmpty then
                    deckRepository.InsertOrReplaceAllWithChildren(deckTitles 
                                             |> List.mapi (fun id title -> 
                                                        Deck(DeckTitle = title, Cards = List<Flashcard>(), Id = id)))
                                             |> sync
                notifier.ObjectInserted.Add(fun f -> insertIntoDeck f)
