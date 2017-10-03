using System;
using System.Threading.Tasks;
using SuperNotesWiFi3D.Model;
using SuperNotesWiFi3D.Droid.libharu;
using Xamarin.Forms;
using Android.Content;
using System.IO;

namespace SuperNotesWiFi3D.Droid.Model
{
    public class PDFHelperAndroid : IPDFHelper
    {
		bool IPDFHelper.IsAvailable()
		{
            return true;
		}

        async Task<string> IPDFHelper.CreatePDFAsync(string header, string date, string imageURLstring, string text)
        {
			string pdfFilepath = null;

			await Task.Run(delegate ()
			{
                // get path for image to download to
                var tempImageFilePath = Foo(Guid.NewGuid().ToString());

                string tempPDFfilepath = null;
				// download image
				try
				{
					var webClient = new System.Net.WebClient();
					webClient.DownloadFile(imageURLstring, tempImageFilePath);


					// get filepath for pdf file
					tempPDFfilepath = Foo(Guid.NewGuid().ToString() + ".pdf");

					// create and save PDF file
					unsafe
					{
						// create PDF document
						var pdfDoc = HPDF_Doc.HPDF_New(null, null);
						if (pdfDoc != null)
						{
							// add single page
							var pdfPage = HPDF_Doc.Page.HPDF_AddPage(pdfDoc);
							if (pdfPage != null)
							{
								//TODO: add name of the note

								// add image
								byte[] tempImageFilePathByteArray = System.Text.Encoding.ASCII.GetBytes(tempImageFilePath);
								fixed (byte* bytePointer = tempImageFilePathByteArray)
								{
									void* pdfImage = HPDF_Doc.Image.HPDF_LoadJpegImageFromFile(pdfDoc, (sbyte*)bytePointer);
									if (pdfImage != null)
									{
										var imageWidth = HPDF_Doc.Image.HPDF_Image_GetWidth(pdfImage);
										var imageHeight = HPDF_Doc.Image.HPDF_Image_GetHeight(pdfImage);

										var status = HPDF_Doc.Page.HPDF_Page_SetWidth(pdfPage, imageWidth);
										status = HPDF_Doc.Page.HPDF_Page_SetHeight(pdfPage, imageHeight);

										status = HPDF_Doc.Image.HPDF_Page_DrawImage(pdfPage, pdfImage, 0, 0, imageWidth, imageHeight);
									}
									else
									{
										//TODO: error?
									}
								}

								// save PDF
								byte[] bytesString = System.Text.Encoding.ASCII.GetBytes(tempPDFfilepath);
								fixed (byte* tempPointer = bytesString)
								{
									sbyte* sbyteString = (sbyte*)tempPointer;
									var status = HPDF_Doc.HPDF_SaveToFile(pdfDoc, sbyteString);
									if (status == 0)
									{
										//TODO: no error?
									}
									else
									{
										//TODO: error?
									}
								}
							}

							// done
							HPDF_Doc.HPDF_Free(pdfDoc);
						}
					}

					// cleanup image
					App.FileHelper.DeleteFile(tempImageFilePath);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e);
				}

				pdfFilepath = tempPDFfilepath;
			});

			return pdfFilepath;
        }

        void IPDFHelper.OpenPDF(string pdfFilepath)
        {
			// get current activity
			MainActivity activity = Forms.Context as MainActivity;

            Intent intent = new Intent(Intent.ActionView);
            var uri = Android.Net.Uri.FromFile(new Java.IO.File(pdfFilepath));
            intent.SetDataAndType(uri, "application/pdf");
            intent.SetFlags(ActivityFlags.NoHistory);
			activity.StartActivity(intent);
        }

        bool IPDFHelper.isAndroid()
        {
            return true;
        }

        private string Foo(string filename)
        {
            var path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "temp");
            //var path = Path.Combine(Forms.Context.GetExternalFilesDir(null).AbsolutePath, "temp");
            Directory.CreateDirectory(path);
            return Path.Combine(path, filename);
        }
    }
}
