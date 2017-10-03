using System;
using SuperNotesWiFi3D.Model;

namespace SuperNotesWiFi3D.Droid.Model
{
    public class EMailHelperAndroid : IEMailHelper
    {
        void IEMailHelper.SendFileToEMail(string email, string title, string body, string filepath, Action<bool> completionHandler)
        {
            throw new NotImplementedException();
        }
    }
}
