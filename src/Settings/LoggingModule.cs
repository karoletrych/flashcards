using System.Linq;
using Autofac;
using Autofac.Core;

namespace Settings
{
	class LoggingModule : Module
	{
		private static void OnComponentPreparing(object sender, PreparingEventArgs e)
		{
			e.Parameters = e.Parameters.Union(
				new[]
				{
					new ResolvedParameter(
						(p, c) => p.ParameterType == typeof(ISetting<>),
						(p, c) => c.ResolveNamed(p.Name, typeof(ISetting<>))
					),
				});
		}

		protected override void AttachToComponentRegistration(
			IComponentRegistry componentRegistry,
			IComponentRegistration registration)
		{
			registration.Preparing += OnComponentPreparing;
		}
	}
}