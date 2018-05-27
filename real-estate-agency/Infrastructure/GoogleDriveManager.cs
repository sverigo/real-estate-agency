using System;
using System.Collections.Generic;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using DriveData = Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Web.Hosting;

namespace real_estate_agency.Infrastructure
{
    public class GoogleDriveManager
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "real-estate-agency";
        private static string client_secret_path = HostingEnvironment.MapPath("~/App_Data/client_secret.json");
        private static string credentials_save_path = HostingEnvironment.MapPath("~/.credentials/credentials.json");
        private DriveService service;
        private Dictionary<string, string> availableFolders = new Dictionary<string, string>()
        {
            { "CommonAccessON",  "11Dn1taet787i1EM-CMBhvFMpMXTFBRpI"},
        };

        /// <summary>
        /// Устанавливает соединение с сервисом Drive, используя credentials
        /// </summary>
        public GoogleDriveManager()
        {
            UserCredential credential;
            // upload settings from client_secret.json file
            var stream = new FileStream(client_secret_path, FileMode.Open, FileAccess.Read);

            // save credentials
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credentials_save_path, true)).Result;

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        /// <summary>
        /// Загрузка файла на диск и возвращает прямую ссылку на скачивание
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string DriveUploadAndGetSrc(HttpPostedFileBase file)
        {
            string folder = "CommonAccessON";
            DriveData.File metadata = new DriveData.File()
            {
                Description = file.FileName,
                Name = Guid.NewGuid().ToString(),
                Parents = new List<string>
                {
                    availableFolders[folder]
                }
            };

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            Stream stream = new MemoryStream(fileData);
            var request = service.Files.Create(metadata, stream, file.ContentType);
            request.Fields = "id";
            request.Upload();

            string result = "https://drive.google.com/uc?id=" + request.ResponseBody.Id;// + "&export=download";
            return result;
        }

        /// <summary>
        /// Помещает файл в корзину
        /// </summary>
        /// <param name="link"></param>
        public void DriveMoveFileToTrash(string link)
        {
            DriveData.File metadata = new DriveData.File()
            {
                Description = "Trashed by .NET application",
                Trashed = true
            };

            // extract ID from url address
            int from = link.IndexOf("id=") + 3;
            int to = link.IndexOf("&exp");
            string file_id = link.Substring(from);

            var req = service.Files.Update(metadata, file_id);
            req.Execute();
        }
    }
}