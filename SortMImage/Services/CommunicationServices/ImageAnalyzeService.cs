using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using SortMImage.Models.AnalyzeModels;

namespace SortMImage.Services.CommunicationServices
{
    public class ImageAnalyzeService
    {
        public static IEnumerable<ImageAnalyzeResult> ParseImageAnalyzeResults(string json)
        {
            IList<ImageAnalyzeResult> parsedResults = new List<ImageAnalyzeResult>();

            var jsonObject = Json.Decode(json);

            foreach (var result in jsonObject.results)
            {
                ImageAnalyzeResult imageAnalyzeResult = new ImageAnalyzeResult()
                {
                    TaggingId = result.tagging_id,
                    ImageName = result.image,
                    ImageTags = ParseTagsArrayToAnalyzeResultTags((DynamicJsonArray)result.tags)
                };

                parsedResults.Add(imageAnalyzeResult);
            }

            return parsedResults;
        }

        public static IList<ImageTag> ParseTagsArrayToAnalyzeResultTags(DynamicJsonArray tags)
        {
            IList<ImageTag> imageTags = new List<ImageTag>();

            foreach (dynamic tag in tags)
            {
                ImageTag imageTag = new ImageTag()
                {
                    Confidence = tag.confidence,
                    Tag = tag.tag
                };

                imageTags.Add(imageTag);
            }

            return imageTags;
        }
    }
}
