using System;
using SuperNotesWiFi3D.Model;
using SuperNotesWiFi3D.iOS.libharu;

namespace SuperNotesWiFi3D.iOS.Model
{
    public class PDFHelperIOS : IPDFHelper
    {
		bool IPDFHelper.IsAvailable()
		{
			return true;
		}

        void IPDFHelper.CreatePDF(string header, string date, string imageURLstring, string text)
        {
            var t = new FH();
            unsafe
            {
                var pdfDoc = HPDF_Doc.HPDF_New(null, null);
            }

            //var t = new SuperNotesWiFi3D.iOS.libharu.FH();
            //t.Foo();
            throw new NotImplementedException();
        }
    }
}
