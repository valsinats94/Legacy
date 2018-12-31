using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using SortMImage.Exceptions;
using SortMImage.Models;
using SortMImage.Models.AnalyzeModels;

namespace SortMImage.Services.CommunicationServices
{
    public class UploadService
    {
        #region Declarations

        private ObservableCollection<StatusResult> uploadResult;

        #endregion

        #region Properties

        public ObservableCollection<StatusResult> UploadResult
        {
            get
            {
                if (uploadResult == null)
                    uploadResult = new ObservableCollection<StatusResult>();

                return uploadResult;
            }
            set
            {
                if (value == uploadResult)
                    return;

                uploadResult = value;
            }
        }

        #endregion

        #region Methods

        public async Task<StatusResult> RunAsyncUpload(List<ImageModel> images)
        {
            if (images == null || images.Count == 0)
                throw new NoImagesForAnalyzingException();

            if (images.Count > 5)
                throw new MaxParallelAnalyzingImagesException();

            string apiKey = "acc_aaac6c75b4db2a4";
            string apiSecret = "df9d2861fdca6bc2d5a97aac409c6360";

            string basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.imagga.com/v1/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", basicAuthValue));

                MultipartFormDataContent uploads = new MultipartFormDataContent();
                foreach (ImageModel image in images)
                {
                    uploads.Add(new ByteArrayContent(image.ImageData), image.Name, image.ImagePath);
                }

                HttpResponseMessage uploadResponse = await client.PostAsync("content", uploads);

                HttpContent content = uploadResponse.Content;
                string result = await content.ReadAsStringAsync();

                StatusResult statusResult = ParseUploadResult(result, images);
                UploadResult.Add(statusResult);

                Console.WriteLine(result);
                Console.ReadLine();

                return statusResult;
            }
        }

        public StatusResult ParseUploadResult(string json, List<ImageModel> originalImages)
        {
            var jsonObject = Json.Decode(json);
            StatusResult statusResult = new StatusResult();
            statusResult.Status = jsonObject["status"];

            if (!statusResult.Status.ToLower().Equals("error"))
                statusResult.Images = ParseImagesFromJson(jsonObject["uploaded"], originalImages);

            return statusResult;
        }

        private IEnumerable<ImageFromResult> ParseImagesFromJson(DynamicJsonArray dynamicJsonArrayOfImages, List<ImageModel> originalImages)
        {
            IList<ImageFromResult> images = new List<ImageFromResult>();

            foreach (dynamic jasonImage in dynamicJsonArrayOfImages)
            {
                ImageFromResult image = new ImageFromResult();
                image.Id = jasonImage.id;
                image.ImageName = jasonImage.filename;
                image.OriginalImage = originalImages.FirstOrDefault(img => 
                    img.Name.Split('.').FirstOrDefault() == image.ImageName.Split('.').FirstOrDefault());

                

                images.Add(image);
            }

            return images;
        }

        #endregion

    }
}
