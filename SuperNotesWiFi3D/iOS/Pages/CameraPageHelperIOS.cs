using System;
using System.IO;
using Xamarin.Forms;
using Foundation;
using UIKit;
using SuperNotesWiFi3D.Pages;


namespace SuperNotesWiFi3D.iOS.Pages
{
    public class CameraPageHelperIOS : NSObject, ICameraPageHelper
    {
		bool ICameraPageHelper.IsCameraAvailable()
        {
            return UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
        }

		bool ICameraPageHelper.IsLibraryAvailable()
        {
            return UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.PhotoLibrary);
        }

        void ICameraPageHelper.GetImageModalAsync(bool isFromCamera, Action<string> completionHandler)
        {
			UIImagePickerController pickerVC = new UIImagePickerController();
			pickerVC.SourceType = isFromCamera ? UIImagePickerControllerSourceType.Camera : UIImagePickerControllerSourceType.PhotoLibrary;
			pickerVC.MediaTypes = new string[] { "public.image" };
			pickerVC.AllowsEditing = true;
			pickerVC.FinishedPickingMedia += delegate (object sender, UIImagePickerMediaPickedEventArgs e) {
				bool isImage = false;

				switch (e.Info[UIImagePickerController.MediaType].ToString())
				{
					case "public.image":
						{
							isImage = true;
						}
						break;

					case "public.video":
						break;
				}

                UIImage image = null;

				if (isImage)
				{
					image = e.Info[UIImagePickerController.EditedImage] as UIImage;
					if (image == null)
					{
						image = e.Info[UIImagePickerController.OriginalImage] as UIImage;
					}
				}

                string imageFilePath = null;
                if (image != null)
                {
                    imageFilePath = App.FileHelper.TempFilePath(DateTime.Now.ToString("yyyy_MM_dd_HH_MM_ss.jpg"), true);
                    image.AsJPEG(1.0f).Save(imageFilePath, true);
                }

				// pass it to caller of camera
                completionHandler(imageFilePath);

				pickerVC.DismissViewController(true, null);
			};
			pickerVC.Canceled += delegate (object sender, EventArgs e) {
				pickerVC.DismissViewController(true, null);
			};

			UIViewController mainVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
			mainVC.PresentViewController(pickerVC, true, null);
        }
    }
}
