using System;
using System.Collections.Generic;
using SuperNotesWiFi3D.Model;
using Xamarin.Forms;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;


namespace SuperNotesWiFi3D.Droid.Model
{
    public class FacebookApiClientAndroid : Java.Lang.Object, IFacebookApiClient, IFacebookCallback
    {
        public ICallbackManager CallbackManager { get; private set; }
        private Action<string, bool, Error> _lastCompletionHandler = null;


        void IFacebookApiClient.SetFacebookAppID(string appID)
        {
            FacebookSdk.ApplicationId = appID;
            FacebookSdk.AutoLogAppEventsEnabled = false;
            FacebookSdk.SdkInitialize(Forms.Context.ApplicationContext);
			CallbackManager = CallbackManagerFactory.Create();
			LoginManager.Instance.RegisterCallback(CallbackManager, this);
            LoginManager.Instance.SetLoginBehavior(LoginBehavior.WebOnly);
        }

        void IFacebookApiClient.SignIn(Action<string, bool, Error> completionHandler)
        {
            _lastCompletionHandler = completionHandler;

			// get current activity
			MainActivity activity = Forms.Context as MainActivity;

            LoginManager.Instance.LogInWithReadPermissions(activity, new List<string>(new string[] { "public_profile" }));
        }


        void IFacebookCallback.OnCancel()
        {
            _lastCompletionHandler?.Invoke(null, true, null);
            _lastCompletionHandler = null;
        }

        void IFacebookCallback.OnError(FacebookException error)
        {
            _lastCompletionHandler?.Invoke(null, false, new Error(ErrorCode.Facebook_Android_LoginError, error?.Message));
            _lastCompletionHandler = null;
        }

        void IFacebookCallback.OnSuccess(Java.Lang.Object result)
        {
            var loginResult = result as LoginResult;
            string tokenString = loginResult?.AccessToken?.Token;
            _lastCompletionHandler?.Invoke(tokenString, false, null);
            _lastCompletionHandler = null;
        }
    }
}
