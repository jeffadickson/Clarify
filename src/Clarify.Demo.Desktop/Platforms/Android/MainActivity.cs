using Android.App;
using Android.Content.PM;
using Microsoft.Maui;
namespace Clarify.Demo.Desktop;
[Activity(Theme="@android:style/Theme.Material.Light.NoActionBar", MainLauncher=true, ConfigurationChanges=ConfigChanges.ScreenSize|ConfigChanges.Orientation|ConfigChanges.UiMode|ConfigChanges.ScreenLayout|ConfigChanges.SmallestScreenSize|ConfigChanges.Density)]
public class MainActivity: MauiAppCompatActivity {}
