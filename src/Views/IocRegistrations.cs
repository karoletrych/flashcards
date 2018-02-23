using Autofac;
using Flashcards.Models;
using Flashcards.Services.Database;
using Flashcards.ViewModels;
using SQLite;
using System;
using System.IO;
using System.Reflection;

namespace Flashcards.Views
{
    public static class IocRegistrations
    {
        public static void RegisterTypesInIocContainer(ContainerBuilder containerBuilder)
        {
            var assemblies = new[]
            {
                Assembly.GetAssembly(typeof(App)),
                Assembly.GetAssembly(typeof(AskingQuestionsViewModel)),
                Assembly.GetAssembly(typeof(Flashcard))
            };
            containerBuilder
                .RegisterAssemblyTypes(assemblies)
                .AsSelf()
                .AsImplementedInterfaces();

            containerBuilder
                .RegisterGeneric(typeof(AsyncRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerDependency();

            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "database.db3");
            containerBuilder
                .Register(_ => DatabaseConnectionFactory.CreateAsyncConnection(databasePath))
                .As<SQLiteAsyncConnection>()
                .SingleInstance();
        }
    }
}