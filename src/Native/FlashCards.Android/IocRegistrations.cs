using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Flashcards.Domain.SpacedRepetition.Leitner;
using Flashcards.Domain.SpacedRepetition.Leitner.Models;
using Flashcards.Domain.ViewModels;
using Flashcards.Droid;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Infrastructure.HttpClient;
using Flashcards.Infrastructure.PlatformDependentTools;
using Flashcards.Infrastructure.Settings;
using Flashcards.Models;
using Flashcards.Services.DataAccess;
using Flashcards.Services.Http;
using Flashcards.SpacedRepetition.Interface;
using Flashcards.Views;

namespace Flashcards.Android
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
                Assembly.GetAssembly(typeof(CardDeck)),
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

	        containerBuilder.RegisterType<MultipleWordsImageBrowser>()
		        .WithParameter(new ResolvedParameter(
			        (info, context) => info.ParameterType == typeof(IImageBrowser),
			        (info, context) => context.Resolve<PixabayImageBrowser>()))
		        .AsImplementedInterfaces();

	        var exportPath = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

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