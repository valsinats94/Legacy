using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortMImage.Models;
using SortMImage.Models.AnalyzeModels;

namespace SortMImage.Services.DatabaseServices
{
    public class ImageDatabaseService
    {
        #region Get

        public ImageModel GetImageByName(string imageName)
        {
            using (var db = new SortMImageContext())
            {
                return db.Images.FirstOrDefault(img => img.Name == imageName);
            }
        }

        public ImageModel GetImageByFilePath(string imagePath)
        {
            using (var db = new SortMImageContext())
            {
                return db.Images.FirstOrDefault(img => img.ImagePath == imagePath);
            }
        }

        public ImageModel GetImageByImageData(byte[] imageData)
        {
            List<ImageModel> dbImages = GetAllImages();
            foreach (ImageModel img in dbImages)
            {
                if (img.ImageData.Count() == imageData.Count() && img.ImageData.SequenceEqual(imageData))
                    return img;

            }

            return null;
        }

        public List<ImageModel> GetAllImages()
        {
            List<ImageModel> retrievedImages = new List<ImageModel>();
            using (var db = new SortMImageContext())
            {
                foreach(ImageModel image in db.Images)
                {
                    db.Entry(image).Collection(img => img.ImageTags).Load();
                    retrievedImages.Add(image);
                }

                return retrievedImages;
            }
        }

        #endregion

        #region Save

        public bool SaveImageToDatabase(ImageModel image)
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += (sender, args) => SaveImage(image);
                bg.RunWorkerAsync();
            }

            return true;
        }

        private void SaveImage(ImageModel image)
        {
            using (var db = new SortMImageContext())
            {
                ImageModel img = GetImageByImageData(image.ImageData);
                if (img == null)
                {
                    db.Images.Add(image);

                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Delete

        public bool DeleteImageFromDatabase(ImageModel image)
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += (sender, args) => DeleteImage(image);
                bg.RunWorkerAsync();
            }

            return true;
        }

        private void DeleteImage(ImageModel image)
        {
            using (var db = new SortMImageContext())
            {
                db.Images.Remove(image);
                db.SaveChanges();
            }
        }

        #endregion

        #region Update

        public void UpdateImageTagsByImageName(string imageName, List<ImageTag> imageTags)
        {
            using (var db = new SortMImageContext())
            {
                ImageModel image = db.Images.FirstOrDefault(img => img.Name.Equals(imageName));
                if (image != null)
                {
                    image.ImageTags = imageTags;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateImageTagsByImageData(byte[] imageData, List<ImageTag> imageTags)
        {
            using (var db = new SortMImageContext())
            {
                ImageModel image = GetImageByImageData(imageData);
                if (image != null)
                {
                    image.ImageTags = imageTags;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateImagePathByImageData(byte[] imageData, string imagePath)
        {
            using (var db = new SortMImageContext())
            {
                ImageModel image = GetImageByImageData(imageData);
                if (image != null)
                {
                    image.ImagePath = imagePath;
                    db.SaveChanges();
                }
            }
        }

        #endregion
    }
}
