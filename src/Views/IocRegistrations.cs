using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using FlashCards.Models;
using FlashCards.Services.Database;
using FlashCards.ViewModels;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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

            return containerBuilder.Build();
        }
    }
}