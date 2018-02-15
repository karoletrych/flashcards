using System;
using System.IO;
using System.Reflection;
using Autofac;
using FlashCards.Models;
using FlashCards.Services.Database;
using FlashCards.ViewModels;
using SQLite;

namespace FlashCards.Views
{
    public static class IocRegistrations
    {
        public static IContainer RegisterTypesInIocContainer()
        {
            var containerBuilder = new ContainerBuilder();
            var assemblies = new[]
            {
                Assembly.GetAssembly(typeof(App)),
                Assembly.GetAssembly(typeof(AskingQuestionsViewModel)),
                Assembly.GetAssembly(typeof(FlashCard))
            };
            containerBuilder
                .RegisterAssemblyTypes(assemblies)
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder
                .RegisterGeneric(typeof(AsyncRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerDependency();

            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "database.db3");
            containerBuilder
                .Register(_ => DatabaseConnectionFactory.CreateAsyncConnection(databasePath))
                .As<SQLiteConnection>()
                .SingleInstance();

            return containerBuilder.Build();
        }
    }
}