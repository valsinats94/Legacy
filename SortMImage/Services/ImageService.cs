using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using SortMImage.Helpers;
using SortMImage.Models;
using SortMImage.Services.DatabaseServices;
using System.Runtime.Serialization.Json;
using System.Configuration;

namespace SortMImage.Services
{
    public class ImageService
    {
        //ThreadBindingList<ImageModel> images = new ThreadBindingList<ImageModel>();

        public ThreadBindingList<ImageModel> LoadImagesAsync(string folderPath, ThreadBindingList<ImageModel> images)
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += (sender, args) => FetchImages(folderPath, images);
                bg.RunWorkerAsync();
            }

            return images;
        }

        public BitmapImage ConvertImageToThumbnail(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new Exception("Can't convert");

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = 100;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(imagePath.ToString());
            bi.EndInit();
            return bi;
        }

        private void FetchImages(string folderPath, ThreadBindingList<ImageModel> images)
        {
            if (!Directory.Exists(folderPath))
                return;
            
            foreach (string file in GetImagesFromFolder(folderPath))
            {
                ImageModel image = new ImageModel();
                image.ImagePath = file;
                string[] stringSeparators = { "\\" };
                image.Name = file.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                image.FileName = file;
                
                images.Add(image);
            }
        }

        private IEnumerable<string> GetImagesFromFolder(string folderPath)
        {
            HashSet<string> imageFiles = new HashSet<string>();

            foreach (string file in Directory.EnumerateFiles(folderPath))
            {
                if (ImageHelperService.IsImageFile(file))
                    imageFiles.Add(file);
            }

            return imageFiles;
        }

        public IEnumerable<PhotoItem> GetExternalImages()
        {
            List<PhotoItem> results = new List<PhotoItem>();

            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += (sender, args) => results.AddRange(GetImagesFromServiceDB());
                bg.RunWorkerAsync();
            }

            return results;
        }

        public PhotoItem[] GetImagesFromServiceDB()
        {
            try
            {
                // Create the REST request.
                string url = ConfigurationManager.AppSettings["serviceUrl"];/*"http://localhost:2557/photos";*/
                string requestUrl = string.Format("{0}/GetPhotos", url);

                WebRequest request = WebRequest.Create(requestUrl);
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(PhotoItem[]));
                        PhotoItem[] results = (PhotoItem[])dcs.ReadObject(stream);

                        // Adjust date/time zone
                        foreach (var photo in results)
                            photo.UploadedOn = new DateTime(photo.UploadedOn.Ticks, DateTimeKind.Utc).ToLocalTime();

                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HttpListenerException(1, "Errore while retrieving photos: " + ex.Message + "/n PhotoService");
            }
        }

        public void DeletePhotosFromServiceDB(List<PhotoItem> photosForDelete)
        {
            if (photosForDelete == null || photosForDelete.Count == 0)
                return;

            foreach (PhotoItem photo in photosForDelete)
            {
                try
                {
                    // Create the REST request.
                    string url = ConfigurationManager.AppSettings["serviceUrl"];/*"http://localhost:2557/photos";*/
                    string requestUrl = string.Format("{0}/DeletePhoto/{1}", url, photo.PhotoID);

                    WebRequest request = WebRequest.Create(requestUrl);
                    request.Method = "DELETE";

                    // Get response  
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        Console.WriteLine("HTTP/{0} {1} {2}", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);

                }
                catch (Exception ex)
                {
                    throw new HttpListenerException(2, "Errore while deleting the selected photo: " + ex.Message + "/n PhotoService");
                }
            }
        }

        public void SynchronizeWithServiceDB()
        {
            if (!InternetService.IsInternetConnected())
                return;

            try
            {                
                List<PhotoItem> photos = new List<PhotoItem>();
                foreach (PhotoItem item in GetImagesFromServiceDB())
                {
                    photos.Add(item);
                }

                DeletePhotosFromServiceDB(photos);

                ImageDatabaseService imgDbService = new ImageDatabaseService();
                foreach (ImageModel image in ImageHelperService.ParsePhotoItemsToImageModels(photos))
                {
                    image.IsProcessed = false;
                    imgDbService.SaveImageToDatabase(image);
                }
            }
            catch (Exception)
            {
                throw new Exception("Cannot synch with service DB");
            }
        }
    }
}
