using System;
using System.IO;


namespace SuperNotesWiFi3D.Pages
{
    public delegate void CameraImageSelectedHandler(MemoryStream imageStream);

    public interface ICameraPageHelper
    {
        bool IsCameraAvailable();
        bool IsLibraryAvailable();

        void GetImageModalAsync(bool isFromCamera, Action<string> completionHandler);
    }
}
