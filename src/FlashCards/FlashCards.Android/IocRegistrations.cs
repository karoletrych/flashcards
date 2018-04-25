using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
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

	        var databasePath = Path.Combine(
		        Environment.GetFolderPath(Environment.SpecialFolder.Personal),
		        "database.db3");

			containerBuilder.RegisterType<ConnectionProvider>()
		        .WithParameter(new TypedParameter(typeof(string), databasePath))
		        .AsImplementedInterfaces()
		        .SingleInstance();

            containerBuilder
                .RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
	            .As(typeof(INotifyObjectInserted<>))
                .SingleInstance();

	        containerBuilder
		        .Register(c => c.Resolve<IConnectionProvider>().Connection)
		        .InstancePerDependency()
		        .AsImplementedInterfaces();

	        var exportPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

	        containerBuilder.Register(_ => new ExportParameters(databasePath, exportPath));
			
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