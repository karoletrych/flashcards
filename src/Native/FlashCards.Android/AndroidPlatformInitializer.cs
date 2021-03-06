﻿using Android.Content;
using Autofac;
using Flashcards.Android;
using Prism;
using Prism.Autofac;
using Prism.Ioc;

namespace Flashcards.Droid
{
    class AndroidPlatformInitializer : IPlatformInitializer
    {
	    private readonly Context _context;

	    public AndroidPlatformInitializer(Context context)
	    {
		    _context = context;
	    }

	    public void RegisterTypes(IContainerRegistry containerRegistry)
	    {
		    var containerBuilder = containerRegistry.GetBuilder();
		    IocRegistrations.RegisterTypesInIocContainer(containerBuilder);
		    containerBuilder.Register(_ => _context).AsSelf().SingleInstance();
	    }
    }
}