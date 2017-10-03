using System;
using System.IO;
using SuperNotesWiFi3D.Model;


namespace SuperNotesWiFi3D.iOS.Model
{
    public class FileHelperIOS : IFileHelper
    {
        string IFileHelper.TempFilePath(string fileName, bool createIntermediateDirectories)
		{
            string tempFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			tempFilePath = Path.Combine(tempFilePath, App.AppName);
			tempFilePath = Path.Combine(tempFilePath, "temp");
			tempFilePath = Path.Combine(tempFilePath, fileName);

            if (createIntermediateDirectories)
            {
				var directory = Directory.GetParent(tempFilePath);
				while (directory != null && !directory.Exists)
				{
					directory.Create();
					directory = directory.Parent;
				}
            }

			return tempFilePath;
		}

		Stream IFileHelper.CreateFile(string filePath)
		{
			return File.Create(filePath);
		}

		Stream IFileHelper.OpenFile(string filePath)
		{
			Stream fileStream = null;
			if (File.Exists(filePath))
			{
				fileStream = File.Open(filePath, FileMode.Open);
			}
			return fileStream;
		}

		void IFileHelper.DeleteFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		void IFileHelper.CopyFile(string srcPath, string dstPath)
		{
			if (File.Exists(srcPath))
			{
				File.Copy(srcPath, dstPath, true);
			}
		}

        string IFileHelper.PersistentLocalFilePath(string filename)
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (!Directory.Exists(documentsFolder))
            {
                Directory.CreateDirectory(documentsFolder);
            }
            return Path.Combine(documentsFolder, filename);
        }
    }
}
