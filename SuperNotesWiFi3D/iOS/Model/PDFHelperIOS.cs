using System;
using System.Threading.Tasks;
using SuperNotesWiFi3D.Model;
using SuperNotesWiFi3D.iOS.libharu;
using UIKit;
using Foundation;

namespace SuperNotesWiFi3D.iOS.Model
{
    public class PDFHelperIOS : IPDFHelper
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
                var tempImageFilePath = App.FileHelper.TempFilePath(Guid.NewGuid().ToString(), true);

                // download image
                try
                {
                    var webClient = new System.Net.WebClient();
                    webClient.DownloadFile(imageURLstring, tempImageFilePath);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                // get filepath for pdf file
                var tempPDFfilepath = App.FileHelper.TempFilePath(Guid.NewGuid().ToString() + ".pdf", true);

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

                pdfFilepath = tempPDFfilepath;
            });

            return pdfFilepath;
        }

        void IPDFHelper.OpenPDF(string pdfFilepath)
        {
            var viewer = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(pdfFilepath));
            var rootVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
            var rootView = rootVC.View;
            var temp = viewer.PresentOpenInMenu(new CoreGraphics.CGRect(0, -200, 100, 100), rootView, true);
            System.Diagnostics.Debug.WriteLine(temp);
        }

        bool IPDFHelper.isAndroid()
        {
            return false;
        }
    }
}
