using Prism;
using Prism.Autofac;
using Prism.Ioc;

namespace FlashCards.Droid
{
    class AndroidPlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            IocRegistrations.RegisterTypesInIocContainer(containerRegistry.GetBuilder());
        }
    }
}