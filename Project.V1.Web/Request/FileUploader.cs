namespace Project.V1.Web.Request
{
    public static class FileUploader
    {
        public static async Task<(string, string, string)> StartUpload(bool allowedExtension, FilesManager file, bool toClose)
        {
            switch (allowedExtension)
            {
                case true:
                    {
                        (string error, string path) = await UploadFile(file, toClose);

                        if (path.Length > 0)
                        {
                            return ("", path, error);
                        }

                        return ($"An Error occurred, could not save specified file", "", error);
                    }

                default:
                    return ("Invalid file type. Upload only excel documents. (.xls and .xlsx only)", "", "");
            }
        }

        private static async Task<(string, string)> UploadFile(FilesManager bufile, bool toClose)
        {
            try
            {
                return await Task.Run<(string, string)>(() =>
                {
                    bufile.UploadFile.Stream.WriteTo(bufile.Filestream);

                    if (toClose)
                    {
                        bufile.Filestream.Close();
                        bufile.UploadFile.Stream.Close();
                    }

                    return ("", bufile.UploadPath);
                });
            }
            catch (Exception ex)
            {
                if (File.Exists(bufile.UploadPath))
                    File.Delete(bufile.UploadPath);

                return (ex.Message, "");
            }
        }
    }
}
