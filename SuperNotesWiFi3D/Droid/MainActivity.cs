using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;


namespace SuperNotesWiFi3D.Droid
{
    [Activity(Label = "SuperNotesWiFi3D.Droid", 
              Icon = "@drawable/icon", 
              Theme = "@style/MyTheme", 
              MainLauncher = true, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            CachedImageRenderer.Init();

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            App.Init(new Pages.CameraPageHelperAndroid(),
                     new Model.FileHelperAndroid(),
                     new SuperNotesWiFi3D.Pages.StyleHelper(), 
                     new Model.FacebookApiClientAndroid(), 
                     new Model.PDFHelperAndroid(),
                     new Model.EMailHelperAndroid(), 
                     new Model.TestHelperAndroid());

            LoadApplication(new App());
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            FFImageLoading.ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            base.OnTrimMemory(level);
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            var facebookClient = App.FacebookApiClient as Model.FacebookApiClientAndroid;
            facebookClient.CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);

            ((Pages.CameraPageHelperAndroid)App.CameraPageHelper).ProcessActivityResult(requestCode, resultCode, data, this);
        }
    }
}
