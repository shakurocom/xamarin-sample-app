using System;
using UIKit;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.iOS.Model
{
    public class FacebookApiClientIOS : IFacebookApiClient
    {
        void IFacebookApiClient.SetFacebookAppID(string appID)
        {
            Facebook.CoreKit.Settings.AppID = appID;
        }

        void IFacebookApiClient.SignIn(Action<string, bool, Error> completionHandler)
        {
            // get current VC
			UIViewController mainVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
            var currentPresentedVC = mainVC;
            while (currentPresentedVC.PresentedViewController != null)
            {
                currentPresentedVC = currentPresentedVC.PresentedViewController;
            }

            // show Facebook login flow
			var manager = new Facebook.LoginKit.LoginManager();
			manager.LogInWithReadPermissions(new string[] { "public_profile" }, currentPresentedVC, delegate (Facebook.LoginKit.LoginManagerLoginResult result, Foundation.NSError error)
			{
                if (error != null)
                {
                    completionHandler(null, false, new Error(ErrorCode.Facebook_iOS_LoginError, error.LocalizedDescription));
                }
                else if (result.IsCancelled)
                {
                    completionHandler(null, true, null);
                }
                else
                {
                    completionHandler(result.Token.TokenString, false, null);
                }
			});
        }
    }
}
