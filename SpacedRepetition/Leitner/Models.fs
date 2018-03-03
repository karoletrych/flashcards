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
                    async{
                    let! decks =
                        deckRepository.FindMatching(fun d -> d.DeckTitle = CurrentDeckName)
                        |> Async.AwaitTask
                    let deck = decks |> Seq.exactlyOne
                    deck.Cards.Add(flashcard)
                    do! deckRepository.Update(deck) |> Async.AwaitTask
                    }
                    |> Async.StartImmediate

                tableCreator.CreateTable<CardDeck>() |> ignore
                tableCreator.CreateTable<Deck>() |> ignore
                
                deckRepository.UpdateAll(deckTitles 
                |> List.mapi (fun id title -> Deck(DeckTitle = title, Cards = List<Flashcard>(), Id = id)))
                |> Async.AwaitTask
                |> Async.RunSynchronously
                |> ignore
                flashcardRepository.ObjectInserted.Add(fun f -> insertIntoDeck f)
