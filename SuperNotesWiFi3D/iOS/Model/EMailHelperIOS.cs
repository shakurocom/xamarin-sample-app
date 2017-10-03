using System;
using SuperNotesWiFi3D.Model;
using SuperNotesWiFi3D.iOS.SMTPLite;

namespace SuperNotesWiFi3D.iOS.Model
{
    public class EMailHelperIOS : IEMailHelper
    {
        void IEMailHelper.SendFileToEMail(string email, string title, string body, string filepath, Action<bool> completionHandler)
        {
			var message = new SMTPMessage();
			message.From = "supernoteswifi3d.sender@gmail.com";
			message.To = email;
			message.Host = "smtp.gmail.com";
			message.Account = "supernoteswifi3d.sender@gmail.com";
			message.Pwd = "2wsx4rfv8ik,";

			message.Subject = title;
			message.Content = body;
			var attach = new SMTPAttachment();
			attach.Name = @"note.pdf";
			attach.FilePath = filepath;
			message.Attachments = new SMTPAttachment[] { attach };

            message.Send(delegate (SMTPMessage msg, double current, double total)
            {
                // do nothing
            },
                         delegate (SMTPMessage msg) 
            {
                completionHandler(true);
            }, 
                         delegate (SMTPMessage msg, Foundation.NSError error) 
            {
                completionHandler(false);
            });
        }
    }
}
