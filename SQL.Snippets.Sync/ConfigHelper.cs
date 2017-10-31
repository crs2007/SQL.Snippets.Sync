using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL.Snippets.Sync
{

    public static class ConfigHelper
    {

        #region JSON Config Helper
        private static string json;

        
        public static string GetConfigValue(string Key)
        {
            string strReturn = "No key configuration has been entered!";
            if (!(string.IsNullOrEmpty(Key)))
            {
                load(@"config.json");
                try
                {
                    JArray ConfigItemArray = JArray.Parse(json);
                    strReturn = ConfigItemArray[0][Key].ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(@"There is a problem with 'Config.json' file. Please make sure all JSON is valid.
You can copy the file content and paste it in - https://jsonformatter.curiousconcept.com", ex);
                }

            }
            return strReturn;
        }

        private static void load(string configFile)
        {
            String FilePath;

            try
            {
                FilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
                StreamReader r = new StreamReader(FilePath);
                json = r.ReadToEnd();
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region File Handle Helper
        public static void DeleteFile(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
        
        public static void deleteOldFiles()
        {
            string dirName = GetConfigValue("UploadedPath");
            string ext = GetConfigValue("FileExtensionToBackup");
            int dayToSave = 30;
            Int32.TryParse(GetConfigValue("DayToDelete"), out dayToSave);

            string[] files = Directory.GetFiles(dirName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-dayToSave) && fi.Extension == ext)
                {
                    try
                    {
                        fi.Delete();
                        Console.WriteLine(fi.FullName + " has has deleted");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(fi.FullName + " has an error - " + ex.Message);
                    }
                }
            }
        }
        public static void ClearFolder(string FolderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderPath);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
        public static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
        }
        public static int CopyIfNewer(string sourcePath, string destDir)
        {
            int rc = 0;
            FileInfo file = new FileInfo(sourcePath);
            FileInfo destFile = new FileInfo(Path.Combine(destDir, file.Name));
            if (destFile.Exists)
            {
                if (file.LastWriteTime > destFile.LastWriteTime)
                {
                    // now you can safely overwrite it
                    file.CopyTo(destFile.FullName, true);
                    rc++;
                }
            }
            else
            {
                file.CopyTo(destFile.FullName, true);
                rc++;
            }
            return rc;
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                int rc = 0;
                //check if the target directory exists
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }

                //copy all the files into the new directory

                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    rc += CopyIfNewer(string.Concat(source.FullName, "\\", fi.Name), target.FullName);
                }
                if (rc == 0)
                {
                    Console.WriteLine("There ara NO new files to copy!");
                }
                else
                {
                    Console.WriteLine(string.Format("Success! Copy total of {0} files into {1}", rc, target));
                }
            }
            catch (IOException ie)
            {
                Console.WriteLine(ie.Message);
            }
        }
        #endregion
    }
}
