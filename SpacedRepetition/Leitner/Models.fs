﻿namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open SQLite
open Flashcards.Services.DataAccess
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider
open SQLiteNetExtensions.Attributes
open Flashcards.Services.DataAccess.Database


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
    let inline sync (x : System.Threading.Tasks.Task) = x |> Async.AwaitTask |> Async.RunSynchronously
    let inline syncT x = x |> Async.AwaitTask |> Async.RunSynchronously
    

    type Deck () = 
        [<PrimaryKey>]
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
                let insertIntoDeck (flashcard : Flashcard) = 
                    let decks =
                        deckRepository.FindMatching(fun d -> d.DeckTitle = "CurrentDeck") 
                        |> syncT
                    let deck = decks |> Seq.exactlyOne
                    deck.Cards.Add(flashcard)
                    deckRepository.Update(deck) |> sync

                tableCreator.CreateTable<CardDeck>() |> sync
                tableCreator.CreateTable<Deck>() |> sync
                if deckRepository.FindAll() |> syncT |> Seq.isEmpty then
                    deckRepository.UpdateAll(deckTitles 
                                                |> List.mapi (fun id title -> 
                                                            Deck(DeckTitle = title, Cards = List<Flashcard>(), Id = id)))
                                                |> sync
                flashcardRepository.ObjectInserted.Add(fun f -> insertIntoDeck f)
