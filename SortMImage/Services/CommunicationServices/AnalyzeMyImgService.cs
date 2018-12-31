using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using SortMImage.Exceptions;
using SortMImage.Models;
using SortMImage.Models.AnalyzeModels;
using SortMImage.Services.DatabaseServices;

namespace SortMImage.Services.CommunicationServices
{
    public class AnalyzeMyImgService
    {
        public async Task AnalyzeInnerImages(List<ImageModel> imagesForAnalyse)
        {
            UploadService uploadService = new UploadService();
            AnalyzeUrlService analyzeUrlService = new AnalyzeUrlService();

            if (imagesForAnalyse == null || imagesForAnalyse.Count <= 0)
                throw new NoImagesForAnalyzingException();

            List<ImageModel> imagesForSending;

            imagesForSending = new List<ImageModel>();

            for (int i = 0; i < imagesForAnalyse.Count; i++)
            {
                imagesForSending.Add(imagesForAnalyse[i]);
                if (imagesForSending.Count % 5 == 0)
                {
                    uploadService.RunAsyncUpload(imagesForSending);
                    imagesForSending = new List<ImageModel>();
                }
            }

            StatusResult uploadResult = await uploadService.RunAsyncUpload(imagesForSending);
            ImageDatabaseService imageDBService = new ImageDatabaseService();
            List<ImageTag> resultImageTags = new List<ImageTag>();

            foreach (ImageFromResult uploadedImage in uploadResult.Images)
            {
                foreach(ImageModel image in imagesForAnalyse)
                {
                    if (image.Name.Equals(uploadedImage.ImageName))
                    {
                        resultImageTags = await RunAsyncAnalyzeImage(uploadedImage.Id);
                        imageDBService.UpdateImageTagsByImageName(uploadedImage.ImageName, resultImageTags);
                    }
                }
            }
        }

        public async Task<List<ImageTag>> RunAsyncAnalyzeImage(string id)
        {
            string apiKey = "acc_aaac6c75b4db2a4";
            string apiSecret = "df9d2861fdca6bc2d5a97aac409c6360";
            string imageUrl = id;
            string categorizerId = "personal_photos";

            string basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.imagga.com/v1/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", basicAuthValue));

                HttpResponseMessage response = await client.GetAsync((String.Format("categorizations/{0}?content={1}", categorizerId, imageUrl)));

                HttpContent content = response.Content;
                string result = await content.ReadAsStringAsync();
                List<ImageTag> resultImageTags = ParseJsonResultToImageTags(result).ToList();
                Console.WriteLine(result);

                return resultImageTags;
            }
        }

        public IEnumerable<ImageTag> ParseJsonResultToImageTags(string json)
        {
            IList<ImageTag> parsedResults = new List<ImageTag>();

            var jsonObject = Json.Decode(json);

            foreach (var result in jsonObject.results)
            {
                foreach(var category in result.categories)
                {
                    ImageTag tag = new ImageTag();
                    tag.Tag = category.name;
                    tag.Confidence = category.confidence;

                    parsedResults.Add(tag);
                }
            }

            return parsedResults;
        }
    }
}
