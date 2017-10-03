using System.IO;


namespace SuperNotesWiFi3D.Model
{
    public interface IFileHelper
    {
        string TempFilePath(string fileName, bool createIntermediateDirectories);
        Stream CreateFile(string filePath);
        Stream OpenFile(string filePath);
        void DeleteFile(string filePath);
        void CopyFile(string srcPath, string dstPath);

        string PersistentLocalFilePath(string filename);
    }
}
