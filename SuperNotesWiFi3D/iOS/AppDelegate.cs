using Foundation;
using UIKit;
using SuperNotesWiFi3D.iOS.Pages;


namespace SuperNotesWiFi3D.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var config = new FFImageLoading.Config.Configuration();
            config.VerboseMemoryCacheLogging = false;
            config.AllowUpscale = false;
            config.AnimateGifs = false;
            //config.VerboseLogging = true;
            //config.VerbosePerformanceLogging = true;
            FFImageLoading.ImageService.Instance.Initialize(config);

            FFImageLoading.Forms.Touch.CachedImageRenderer.Init();

            Xamarin.Forms.Forms.Init();
            App.Init(new CameraPageHelperIOS(),
                     new Model.FileHelperIOS(),
                     new StyleHelperIOS(),
                     new Model.FacebookApiClientIOS(),
                     new Model.PDFHelperIOS(),
                     new Model.EMailHelperIOS(), 
                     new Model.TestHelperIOS());

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            // handle facebook redirections
            var genericOptions = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(options.Values, options.Keys);
            if (Facebook.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl(app, url, genericOptions))
            {
                return true;
            }
            else
            {
                return base.OpenUrl(app, url, options);
            }
        }
    }
}
