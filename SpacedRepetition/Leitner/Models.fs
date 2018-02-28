namespace Flashcards.SpacedRepetition.Leitner
open System.Collections.Generic
open SQLite
open Flashcards.Services.DataAccess
open Flashcards.Services.DataAccess.Database
open Flashcards.Models
open Flashcards.SpacedRepetition.Provider

module Models =

    let progressDeckTitles = [
        [0;2;5;9];
        [1;3;6;0];
        [2;4;7;1];
        [3;5;8;2];
        [4;6;9;3];
        [5;7;0;4];
        [6;8;1;5];
        [7;9;2;6];
        [8;0;3;7];
        [9;1;4;8];
    ]

    type CardId = int
    type DeckTitle = 
    | CurrentDeck
    | RetiredDeck
    | ProgressDeck of int list

    type Deck = {
        Title : DeckTitle
        Cards : List<CardId>
    }

    type DeckTitleEnum =
    | CurrentDeck = 1
    | _0259 = 2
    | _1360 = 3
    | _2471 = 4
    | _3582 = 5
    | _4693 = 6
    | _5704 = 7
    | _6815 = 8
    | _7926 = 9
    | _8037 = 10
    | _9148 = 11
    | RetiredDeck = 12

    let toDeckTitle : DeckTitleEnum -> DeckTitle =
        function
        | DeckTitleEnum.CurrentDeck -> CurrentDeck
        | DeckTitleEnum.RetiredDeck -> RetiredDeck
        | DeckTitleEnum._0259 -> ProgressDeck [0;2;5;9]
        | DeckTitleEnum._1360 -> ProgressDeck [1;3;6;0]
        | DeckTitleEnum._2471 -> ProgressDeck [2;4;7;1]
        | DeckTitleEnum._3582 -> ProgressDeck [3;5;8;2]
        | DeckTitleEnum._4693 -> ProgressDeck [4;6;9;3]
        | DeckTitleEnum._5704 -> ProgressDeck [5;7;0;4]
        | DeckTitleEnum._6815 -> ProgressDeck [6;8;1;5]
        | DeckTitleEnum._7926 -> ProgressDeck [7;9;2;6]
        | DeckTitleEnum._8037 -> ProgressDeck [8;0;3;7]
        | DeckTitleEnum._9148 -> ProgressDeck [9;1;4;8]
        | _ -> failwith "unexpected DeckTitle"
    
    let toDeckTitleEnum : DeckTitle -> DeckTitleEnum =
        function
        | CurrentDeck -> DeckTitleEnum.CurrentDeck
        | RetiredDeck -> DeckTitleEnum.RetiredDeck
        | ProgressDeck [0;2;5;9] ->  DeckTitleEnum._0259
        | ProgressDeck [1;3;6;0] ->  DeckTitleEnum._1360
        | ProgressDeck [2;4;7;1] ->  DeckTitleEnum._2471
        | ProgressDeck [3;5;8;2] ->  DeckTitleEnum._3582
        | ProgressDeck [4;6;9;3] ->  DeckTitleEnum._4693
        | ProgressDeck [5;7;0;4] ->  DeckTitleEnum._5704
        | ProgressDeck [6;8;1;5] ->  DeckTitleEnum._6815
        | ProgressDeck [7;9;2;6] ->  DeckTitleEnum._7926
        | ProgressDeck [8;0;3;7] ->  DeckTitleEnum._8037
        | ProgressDeck [9;1;4;8] ->  DeckTitleEnum._9148
        | _ -> failwith "unexpected DeckTitle"


    type CardDeck (cardId : int, deck : DeckTitleEnum) = 
        let mutable _cardId : int = cardId
        let mutable _deckTitle : DeckTitleEnum = deck

        new() = CardDeck(0, DeckTitleEnum ())

        [<Indexed>]
        [<PrimaryKey>]
        member this.CardId
            with get() = _cardId
            and set(value) = _cardId <- value
        
        [<Indexed>]
        member this.DeckTitle
            with get() = _deckTitle
            and set(value) = _deckTitle <- value

     type LeitnerCardDeckCreator() =
        interface ITableCreator with
            member this.CreateTablesAsync(conn : SQLiteAsyncConnection) =
                ignore (conn.CreateTableAsync<CardDeck>())
            member this.CreateTables(conn : SQLiteConnection) =
                ignore (conn.CreateTable<CardDeck>())
                   
     type LeitnerInitializer(cardDeckRepository : IRepository<CardDeck>) =
        interface ISpacedRepetitionInitializer with
            member this.Initialize(flashcard : Flashcard) =
                let cardDeck = CardDeck(CardId = flashcard.Id, DeckTitle = DeckTitleEnum.CurrentDeck)
                cardDeckRepository.Insert(cardDeck) |> ignore
