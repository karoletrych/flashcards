using System;
using System.IO;
using System.Reflection;
using Android.App;
using Android.Content;
using Autofac;
using FlashCards.Models;
using FlashCards.Services;
using FlashCards.Services.Database;
using FlashCards.ViewModels;
using FlashCards.Views;
using SQLite;
using Tesseract.Droid;
using DatabaseConnectionFactory = FlashCards.Models.DatabaseConnectionFactory;

namespace FlashCards.Droid
{
    public static class IocRegistrations
    {
        public static IContainer RegisterTypesInIocContainer(Context applicationContext)
        {
            var containerBuilder = new ContainerBuilder();
            var assemblies = new[]
            {
                Assembly.GetAssembly(typeof(App)),
                Assembly.GetAssembly(typeof(AskingQuestionsViewModel)),
                Assembly.GetAssembly(typeof(FlashCard)),
                Assembly.GetAssembly(typeof(ITranslator))
            };
            containerBuilder
                .RegisterAssemblyTypes(assemblies)
                .AsSelf()
                .AsImplementedInterfaces();

            containerBuilder
                .RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>));

            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "database.db3");
            containerBuilder
                .Register(_ => DatabaseConnectionFactory.Connect(databasePath))
                .As<SQLiteAsyncConnection>()
                .SingleInstance();

            containerBuilder.Register(ctx =>
                    new TesseractApi(applicationContext, AssetsDeployment.OncePerInitialization))
                .AsImplementedInterfaces();

            return containerBuilder.Build();
        }
    }
}