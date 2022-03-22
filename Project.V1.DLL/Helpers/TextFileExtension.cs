using System.IO;

namespace Project.V1.DLL.Helpers
{
    public static class TextFileExtension
    {
        private static string _path;
        private static string _fpath;
        private static string _fname;

        private static string BPath
        {
            get { return _path; }
            set { _path = value; }
        }

        private static string FPath
        {
            get { return _fpath; }
            set { _fpath = value; }
        }

        private static string Filename
        {
            get { return _fname; }
            set { _fname = value; }
        }

        public static string Initialize(this string basePath, string filename)
        {
            BPath = basePath;
            Filename = filename;

            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), BPath);

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            FPath = Path.Combine(pathBuilt, Filename);

            return FPath;
        }

        public static StreamWriter GetStream(this string path)
        {
            if (File.Exists(path))
            {
                return new StreamWriter(path);
            }

            return File.CreateText(path);
        }

        public static (bool isWritten, string message) WriteToFile(this StreamWriter streamWriter, string text)
        {
            try
            {
                streamWriter.WriteLine(text);

                streamWriter.Close();
                streamWriter.Dispose();
                return (true, FPath);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static string ReadFromFile(this string path)
        {
            if (!File.Exists(path))
                return null;

            return File.ReadAllText(path);
        }
    }

    //public class A
    //{
    //    public async void a()
    //    {
    //        await TextFileExtension.Initialize("HUD_SiteID", "").GetStream().WriteToFile("this is the text");

    //        TextFileExtension.Initialize("HUD_SiteID", "").ReadFromFile();
    //    }
    //}
}
