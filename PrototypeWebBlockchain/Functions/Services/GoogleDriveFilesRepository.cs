using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PrototypeWebBlockchain.Functions.Services
{
    public class GoogleDriveFilesRepository
    {
        //defined scope.
        public static string[] Scopes = { DriveService.Scope.Drive };
        public static string ApplicationName = "AFP Client";
        public static DriveService Service;

        static GoogleDriveFilesRepository()
        {
            UserCredential credential;

            string config_path = ConfigurationManager.AppSettings["ConfigurationPath"];

            string client_path = Path.Combine(config_path, "client_id.json");

            using (var stream = new FileStream(client_path, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(config_path, true)).Result;
            }

            Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = GoogleDriveFilesRepository.ApplicationName,
            });

        }

        /// <summary>
        /// Download Files
        /// </summary>
        /// <param name="service"></param>
        /// <param name="file"></param>
        /// <param name="saveTo"></param>
        public static void DownloadFile(Google.Apis.Drive.v3.Data.File file)
        {

            var request = Service.Files.Get(file.Id);
            var stream = new System.IO.MemoryStream();

            string FolderPath = ConfigurationManager.AppSettings["FileImagePath"];
            string FilePath = System.IO.Path.Combine(FolderPath, file.Name);

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Completed:
                        {
                            SaveStream(stream, FilePath);
                            Console.WriteLine("Download complete.");
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream);

        }

        private static void SaveStream(System.IO.MemoryStream stream, string saveTo)
        {
            using (System.IO.FileStream file = new System.IO.FileStream(saveTo, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        public static string UploadImage(string path, string filename, string mimetype)
        {
            var fileMetaData = new Google.Apis.Drive.v3.Data.File();
            fileMetaData.Parents = new List<string>()
            {
                "10TVA8fya_XoPa0nwg7m2WPsqh_scfyK4"
            };

            fileMetaData.Name = filename;
            fileMetaData.MimeType = mimetype;
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = Service.Files.Create(fileMetaData, stream, mimetype);
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            return file.Id;
        }

        public static void ListFiles()
        {
            // Define parameters of request.

            FilesResource.ListRequest listRequest = Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.PageToken = null;
            listRequest.Spaces = "drive";
            listRequest.Q = "'10TVA8fya_XoPa0nwg7m2WPsqh_scfyK4' in parents";
            //listRequest.Q = "'root' in parents";

            var request = listRequest.Execute();

            if (request.Files != null && request.Files.Count > 0)
            {

                //foreach (var file in request.Files)
                //{
                //    GoogleDriveFilesRepository.DownloadFile(file);

                //}

            }

        }

        public static void RestoreFile(string id)
        {
            FilesResource.ListRequest listRequest = Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.PageToken = null;
            listRequest.Spaces = "drive";
            listRequest.Q = "'10TVA8fya_XoPa0nwg7m2WPsqh_scfyK4' in parents";
            //listRequest.Q = "'root' in parents";

            var request = listRequest.Execute();

            var file = request.Files.Where(r => r.Id == id).FirstOrDefault();


            if (file != null)
            {
               GoogleDriveFilesRepository.DownloadFile(file);
            }

        }

    }
}