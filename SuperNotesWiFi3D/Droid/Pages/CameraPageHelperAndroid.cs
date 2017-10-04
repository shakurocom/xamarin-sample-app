using SuperNotesWiFi3D.Pages;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Media;
using Xamarin.Forms;
using System.IO;
using System;


namespace SuperNotesWiFi3D.Droid.Pages
{
    public class CameraPageHelperAndroid : ICameraPageHelper
    {
        #region Activity Result

        private const int _intentRequestCode_PickImage = 1000;
        private const int _intentRequestCode_CaptureImage = 1001;
        private const int _intentRequestCode_RequestPermissions = 1002;

        private Action<string> _lastCompletionHandler = null;

        private string _takenPhotoPath = null;


        public void ProcessActivityResult(int requestCode, Result resultCode, Intent data, Activity activity) 
        {
            switch (requestCode)
            {
                case _intentRequestCode_PickImage:
                    if ((resultCode == Result.Ok) && 
                        (data != null) && 
                        (_lastCompletionHandler != null))
                    {
                        var imageStream = activity.ContentResolver.OpenInputStream(data.Data);

                        string tempImagePath = App.FileHelper.PersistentLocalFilePath(DateTime.Now.ToString("yyyy_MM_dd_HH_MM_ss.jpg"));

                        var file = App.FileHelper.CreateFile(tempImagePath);
                        imageStream.CopyTo(file);
                        imageStream.Close();
                        file.Close();

                        CorrectImageOrientation(tempImagePath);

                        _lastCompletionHandler(tempImagePath);
                    }
                    break;

                case _intentRequestCode_CaptureImage:
                    if ((resultCode == Result.Ok) &&
                        (_lastCompletionHandler != null) &&
                        (_takenPhotoPath != null))
					{
                        string tempImagePath = App.FileHelper.PersistentLocalFilePath(DateTime.Now.ToString("yyyy_MM_dd_HH_MM_ss.jpg"));

                        App.FileHelper.CopyFile(_takenPhotoPath, tempImagePath);

                        CorrectImageOrientation(tempImagePath);

                        _lastCompletionHandler(tempImagePath);
					}
					break;

                case _intentRequestCode_RequestPermissions:
                    // shoud not be here
                    break;

                default:
                    // do nothing - result is not for us
                    break;
            }

            _lastCompletionHandler = null;
            _takenPhotoPath = null;
        }

        private void CorrectImageOrientation(string imagePath)
        {
            ExifInterface ei = new ExifInterface(imagePath);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

            var matrix = new Android.Graphics.Matrix();
            bool rotationIsNeeded = true;
            switch (orientation)
            {
                case (int)Orientation.Rotate90:
                    matrix.PostRotate(90);
                    break;

                case (int)Orientation.Rotate180:
                    matrix.PostRotate(180);
                    break;

                case (int)Orientation.Rotate270:
                    matrix.PostRotate(270);
                    break;

                default:
                    rotationIsNeeded = false;
                    break;
            }
            if (rotationIsNeeded)
            {
                var sourceBitmap = Android.Graphics.BitmapFactory.DecodeFile(imagePath);
                var rotatedBitmap = Android.Graphics.Bitmap.CreateBitmap(sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height, matrix, true);
                sourceBitmap.Dispose();
                App.FileHelper.DeleteFile(imagePath);
                var outStream = App.FileHelper.CreateFile(imagePath);
                rotatedBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, outStream);
                rotatedBitmap.Dispose();
                outStream.Close();
                outStream.Dispose();
            }
        }

        #endregion


        #region ICameraPageHelper

        bool ICameraPageHelper.IsCameraAvailable()
        {
			// get current activity
			MainActivity activity = Forms.Context as MainActivity;
			// resolve intent
            return activity.PackageManager.QueryIntentActivities(IntentForImageFromCamera(), Android.Content.PM.PackageInfoFlags.MatchAll).Count > 0;
        }

        void ICameraPageHelper.GetImageModalAsync(bool isFromCamera, Action<string> completionHandler)
        {
            _lastCompletionHandler = completionHandler;
            _takenPhotoPath = null;

			// get current activity
			MainActivity activity = Forms.Context as MainActivity;

            Intent intent = null;
            int requestCode = 0;
            if (isFromCamera)
            {
				string imageFileName = string.Format("JPEG_{0:yyyyMMdd_HHmmss}_", System.DateTime.Now);
				//var imageFile = new Java.IO.File(imageFileName + ".jpg");

				Java.IO.File directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);

                if (activity.ShouldShowRequestPermissionRationale(Android.Manifest.Permission.WriteExternalStorage) ||
                    activity.ShouldShowRequestPermissionRationale(Android.Manifest.Permission.Camera))
                {
                    activity.RequestPermissions(new string[] { 
                        Android.Manifest.Permission.WriteExternalStorage, 
                        Android.Manifest.Permission.Camera }, 
                                                _intentRequestCode_RequestPermissions);
                    completionHandler(null);
                    _lastCompletionHandler = null;
                    return;
                }

				var imageFile = Java.IO.File.CreateTempFile(imageFileName, ".jpg", directory);
				var imageURI = Android.Support.V4.Content.FileProvider.GetUriForFile(activity,
																					 "com.shakuro.Super_Notes_WiFi_3D.fileprovider",
																					 imageFile);
				_takenPhotoPath = imageFile.Path;

				Intent cameraIntent = IntentForImageFromCamera();
				cameraIntent.PutExtra(MediaStore.ExtraOutput, imageURI);

                intent = cameraIntent;
                requestCode = _intentRequestCode_CaptureImage;
            }
            else
            {
                intent = IntentForPickImageFromLibrary();
                requestCode = _intentRequestCode_PickImage;
            }

            activity.StartActivityForResult(intent, requestCode);
        }

        private Intent IntentForImageFromCamera() 
        {
            Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
            return cameraIntent;
        }

		bool ICameraPageHelper.IsLibraryAvailable()
		{
			// get current activity
			MainActivity activity = Forms.Context as MainActivity;
            // resolve intent
			return activity.PackageManager.QueryIntentActivities(IntentForPickImageFromLibrary(), Android.Content.PM.PackageInfoFlags.MatchAll).Count > 0;
		}

        private Intent IntentForPickImageFromLibrary()
        {
			Intent pickImageIntent = new Intent();
			pickImageIntent.SetType("image/*");
			pickImageIntent.SetAction(Intent.ActionGetContent);

            return Intent.CreateChooser(pickImageIntent, "Select Image");
        }

        #endregion
    }
}
