using System;


namespace SuperNotesWiFi3D.Model
{
    public interface IFacebookApiClient
    {
        void SetFacebookAppID(string appID);

        /// <param name="completionHandler">Completion handler (tokenString, isCanceled, error).</param>
        void SignIn(Action<string, bool, Error> completionHandler);
    }
}
