using System.Threading.Tasks;

namespace SuperNotesWiFi3D.Model
{
    public interface IPDFHelper
    {
        bool IsAvailable();
        bool isAndroid();
        Task<string> CreatePDFAsync(string header, string date, string imageURLstring, string text);
        void OpenPDF(string pdfFilepath);
    }
}
