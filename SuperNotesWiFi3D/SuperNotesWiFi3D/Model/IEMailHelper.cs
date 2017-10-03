using System;

namespace SuperNotesWiFi3D.Model
{
    public interface IEMailHelper
    {
        void SendFileToEMail(string email, string title, string body, string filepath, Action<bool> completionHandler);
    }
}
