using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Module = Autofac.Module;

namespace Flashcards.Settings
{
	class SettingsModule : Module
	{
		private readonly Assembly[] _assemblies;

		public SettingsModule(Assembly[] assemblies)
		{
			_assemblies = assemblies;
		}

		protected override void AttachToComponentRegistration(
			IComponentRegistry componentRegistry,
			IComponentRegistration registration)
		{
			registration.Preparing += InjectSettings;
		}

		private void InjectSettings(object sender, PreparingEventArgs e)
		{
			e.Parameters = e.Parameters.Union(new[]
			{
				new ResolvedParameter(
					(info, context) => info.ParameterType.GetGenericTypeDefinition() == typeof(ISetting<>),
					(parameterInfo, context) =>
						context.ResolveNamed(parameterInfo.Name.ToLower(), parameterInfo.ParameterType))
			});
		}

		protected override void Load(ContainerBuilder builder)
		{
			bool IsISetting(Type type)
			{
				return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ISetting<>);
			}

			var settingTypes = _assemblies
				.SelectMany(assembly =>
					assembly.GetTypes()
						.Where(t => t.GetInterfaces()
							.Any(IsISetting)));
			foreach (var type in settingTypes)
				builder.RegisterType(type)
					.Named(type.Name.ToLower(), type.GetInterfaces().Single(IsISetting));
		}
	}
}