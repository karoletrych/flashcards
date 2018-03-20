using Android.App;
using Android.Content.PM;
using Android.OS;
using Flashcards.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FlashCards.Droid.Repetitions
{
    [Activity(
        Label = "Flashcards",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class RepetitionActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            LoadApplication(new RepetitionApp(new AndroidPlatformInitializer(this)));
        }
    }
}