using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using FlashCards.Services;
using App = FlashCards.Views.App;

namespace FlashCards.Droid
{
    [Activity(
        Label = "FlashCards",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = global::FlashCards.Droid.Resource.Layout.Tabbar;
            ToolbarResource = global::FlashCards.Droid.Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            var container = IocRegistrations.RegisterTypesInIocContainer(ApplicationContext);

            var r = GetType().Assembly.GetManifestResourceNames();
            var rs = Assembly.GetExecutingAssembly().GetManifestResourceNames();



            var importer = container.Resolve<ImageImporter>();
            Task.Run(async () => await importer.Import(LoadSample("sample2.jpg")));

            var app = container.Resolve<App>();

            LoadApplication(app);

        }

        private static Stream LoadSample(string name)
        {
            var assembly = Assembly.GetAssembly(typeof(MainActivity));
            var stream = assembly.GetManifestResourceStream("FlashCards.Droid.Samples." + name);
            return stream;
        }
    }
}

