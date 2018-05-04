using CafeNextFramework.CafeConfiguration;
using System;
using System.IO;

namespace CafeNextFramework.Utilities
{
    public static class FileUtilities
    {
        private static string runHome = string.Empty;

        static FileUtilities()
        {
            runHome = CafeNextConfiguration.LogFolderPath;
            if (!string.IsNullOrEmpty(runHome))
            {
                runHome = runHome + "\\Executed_on_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                MakeFolder(runHome);
            }
            else
            {
                Console.WriteLine("Log Folder Path is not Provided");
            }
        }

        public static bool MakePath(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                return true;
            }
            if (!MakePath(file.Directory.ToString()))
            {
                return false;
            }
            try
            {
                file.Create();
            }
            catch (IOException)
            {
                return false;
            }
            return file.Exists;
        }

        public static bool MakeFolder(string folderPath)
        {
            bool folderExists = Directory.Exists(folderPath);
            if (!folderExists)
            {
                Directory.CreateDirectory(folderPath);
                return true;
            }
            else
            {
                return true;
            }
        }

        public static string GetRunResource(string relativePath)
        {
            return CreatePath(runHome, relativePath);
        }

        private static string CreatePath(string home, string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath))
            { home = home + "/" + relativePath; }
            return home;
        }
    }
}