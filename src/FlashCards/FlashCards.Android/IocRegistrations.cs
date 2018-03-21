using System;
using System.IO;
using System.Reflection;
using Autofac;
using Flashcards.Droid;
using Flashcards.Models;
using Flashcards.PlatformDependentTools;
using Flashcards.Services.DataAccess;
using Flashcards.Services.DataAccess.Database;
using Flashcards.Services.Http;
using Flashcards.Settings;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.SpacedRepetition.Leitner;
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
                Assembly.GetAssembly(typeof(ITranslator)),
                Assembly.GetAssembly(typeof(ISpacedRepetition)),
                Assembly.GetAssembly(typeof(Algorithm.LeitnerRepetition)),
				Assembly.GetAssembly(typeof(MainActivity)),
	            Assembly.GetAssembly(typeof(SettingsModule)),
	            Assembly.GetAssembly(typeof(IMessage)),
			};

            containerBuilder
                .RegisterAssemblyTypes(assemblies)
                .AsSelf()
                .AsImplementedInterfaces();

            containerBuilder
                .RegisterGeneric(typeof(AsyncRepository<>))
                .As(typeof(IRepository<>))
                .SingleInstance();

	        containerBuilder
		        .RegisterType<AsyncTableCreator>()
		        .As<ITableCreator>();

            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "database.db3");
            containerBuilder
                .Register(ctx => ctx
                    .Resolve<DatabaseConnectionFactory>()
                    .CreateAsyncConnection(databasePath))
                .As<SQLiteAsyncConnection>()
                .SingleInstance();

            containerBuilder.RegisterModule(new SettingsModule(assemblies));
        }

	    public static IContainer DefaultContainer()
	    {
			var containerBuilder = new ContainerBuilder();
			RegisterTypesInIocContainer(containerBuilder);
		    return containerBuilder.Build();
	    }
    }
}