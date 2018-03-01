using System;
using System.IO;
using System.Reflection;
using Autofac;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Flashcards.Services.Http;
using Flashcards.SpacedRepetition.Leitner;
using Flashcards.SpacedRepetition.Provider;
using Flashcards.ViewModels;
using Flashcards.Views;
using SQLite;

namespace FlashCards.Droid
{
    public static class IocRegistrations
    {
        public static void RegisterTypesInIocContainer(ContainerBuilder containerBuilder)
        {
            var assemblies = new[]
            {
                Assembly.GetAssembly(typeof(App)),
                Assembly.GetAssembly(typeof(AskingQuestionsViewModel)),
                Assembly.GetAssembly(typeof(Flashcard)),
                Assembly.GetAssembly(typeof(ITranslatorService)),
                Assembly.GetAssembly(typeof(ISpacedRepetition)),
                Assembly.GetAssembly(typeof(Algorithm.LeitnerRepetition)),
            };
            containerBuilder
                .RegisterAssemblyTypes(assemblies)
                .AsSelf()
                .AsImplementedInterfaces();

            containerBuilder
                .RegisterGeneric(typeof(AsyncRepository<>))
                .As(typeof(IRepository<>))
                .SingleInstance();

            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "database.db3");
            containerBuilder
                .Register(ctx => ctx
                    .Resolve<DatabaseConnectionFactory>()
                    .CreateAsyncConnection(databasePath))
                .As<SQLiteAsyncConnection>()
                .SingleInstance();
        }
    }
}