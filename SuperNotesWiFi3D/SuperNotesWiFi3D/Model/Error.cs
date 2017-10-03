
namespace SuperNotesWiFi3D.Model
{
    public enum ErrorCode : int
    {
        UnknownError = 01,

        Firebase_Auth       = 11,
        Firebase_Client     = 12,
        Firebase_Storage    = 13,

        Facebook_iOS_LoginError     = 21,
        Facebook_Android_LoginError = 22,

        InternetUnreachable = 31,
    }

    public class Error
    {
        public readonly ErrorCode Code;
        public readonly string Message;

        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
