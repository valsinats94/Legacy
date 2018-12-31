using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;
using SortMImage.Exceptions;
using SortMImage.Helpers;
using SortMImage.Models;
using SortMImage.Models.AnalyzeModels;
using SortMImage.Properties;
using SortMImage.Services;
using SortMImage.Services.CommunicationServices;
using SortMImage.Services.DatabaseServices;
using SortMImage.Views;

namespace SortMImage.ViewModels
{
    public class ImagesViewModel : BaseViewModel
    {
        #region Declarations

        private ThreadBindingList<ImageModel> images;
        private ICollectionView selectedItems;
        private string folderPath;

        private SortedImagesViewModel sortedImagesViewModel;

        private ObservableCollection<StatusResult> uploadResult;

        private DelegateCommand<object> openFolderDialogCommand;
        private DelegateCommand<ICollection> analyzeCommand;
        private DelegateCommand<ICollection> uploadCommand;
        private DelegateCommand<object> clearCommand;

        #endregion

        #region Constructors

        public ImagesViewModel()
        {
            ImageService imageService = new ImageService();
            SyncOnBackGrndWithService();
        }

        #endregion

        #region Properties

        public SortedImagesViewModel SortedImagesViewModel
        {
            get
            {
                if (sortedImagesViewModel == null)
                {
                    sortedImagesViewModel = new SortedImagesViewModel();
                }

                return sortedImagesViewModel;
            }
        }

        public ThreadBindingList<ImageModel> Images
        {
            get
            {
                if (images == null)
                    images = new ThreadBindingList<ImageModel>();

                return images;
            }
        }

        public string FolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(folderPath))
                    folderPath = string.Empty;

                return folderPath;
            }
            set
            {
                if (value == folderPath)
                    return;

                folderPath = value;
                OnPropertyChanged();
                LoadImagesAsync(folderPath, Images);
            }
        }

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
                AnalyzeCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<object> OpenFolderDialogCommand
        {
            get
            {
                if (openFolderDialogCommand == null)
                    openFolderDialogCommand = new DelegateCommand<object>(OpenFolderDialog);

                return openFolderDialogCommand;
            }
        }

        public DelegateCommand<ICollection> AnalyzeCommand
        {
            get
            {
                if (analyzeCommand == null)
                    analyzeCommand = new DelegateCommand<ICollection>(AnalyzeImagesSync, CanAnalyze);

                return analyzeCommand;
            }
        }

        public DelegateCommand<ICollection> UploadCommand
        {
            get
            {
                if (uploadCommand == null)
                    uploadCommand = new DelegateCommand<ICollection>(UploadImagesSync);

                return uploadCommand;
            }
        }

        public DelegateCommand<object> ClearCommand
        {
            get
            {
                if (clearCommand == null)
                    clearCommand = new DelegateCommand<object>(ClearImages);

                return clearCommand;
            }
        }

        #endregion

        #region Methods

        private void LoadImagesAsync(string folderPath, ThreadBindingList<ImageModel> images)
        {
            ImageService imageService = new ImageService();
            imageService.LoadImagesAsync(folderPath, images);
        }

        private void OpenFolderDialog(object obj)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the folder with pictures";
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = Settings.Default.FolderPath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = dialog.SelectedPath;
                Settings.Default.FolderPath = dialog.SelectedPath;
            }
        }

        private void ClearImages(object obj)
        {
            Images.Clear();
            images.Clear();
        }

        #region Image Processing

        private void UploadImagesSync(ICollection selectedImages)
        {
            UploadImagesTest(selectedImages);
        }    

        private async Task UploadImagesTest(ICollection selectedImages)
        {
            if (selectedImages == null || selectedImages.Count <= 0)
                return;

            List<ImageModel> images = new List<ImageModel>();

            foreach (var image in selectedImages)
            {
                images.Add((ImageModel)image);
            }

            UploadService uploadService = new UploadService();
            StatusResult result = await uploadService.RunAsyncUpload(images);
            UploadResult.Add(result);
            AnalyzeCommand.RaiseCanExecuteChanged();
            foreach (ImageModel image in images)
            {
                Images.Remove(image);
            }
        }

        private void AnalyzeImagesSync(ICollection images)
        {
            AnalyzeImagesTest(images);
        }

        private async Task AnalyzeImagesTest(ICollection images)
        {
            ObservableCollection<StatusResult> tempUploadResult = new ObservableCollection<StatusResult>(UploadResult);
            UploadResult.Clear();
            AnalyzeCommand.RaiseCanExecuteChanged();

            AnalyzeMyImgService analyzeImageService = new AnalyzeMyImgService();
            ImageDatabaseService imageDbService = new ImageDatabaseService();

            foreach (StatusResult imageStatusResult in tempUploadResult)
            {
                foreach (ImageFromResult image in imageStatusResult.Images)
                {
                    ImageModel tmpImg = imageDbService.GetImageByImageData(image.OriginalImage.ImageData);
                    if (tmpImg == null)
                    {
                        tmpImg = image.OriginalImage;
                        tmpImg.ImageTags = await analyzeImageService.RunAsyncAnalyzeImage(image.Id);
                        tmpImg.IsProcessed = true;
                        imageDbService.SaveImageToDatabase(tmpImg);
                    }

                    SortImageInApropriateCollection(tmpImg);
                }
            }
        }

        private void SortImageInApropriateCollection(ImageModel tmpImg)
        {
            if (tmpImg.ImageTags == null || tmpImg.ImageTags.Count == 0)
                return;

            tmpImg.ImageTags.OrderByDescending(imgTag => imgTag.Confidence);

            switch (tmpImg.ImageTags.FirstOrDefault().Tag)
            {
                case "interior objects":
                    SortedImagesViewModel.InteriorObjects.Add(tmpImg);
                    break;
                case "nature landscape":
                    SortedImagesViewModel.NatureLandscape.Add(tmpImg);
                    break;
                case "beaches seaside":
                    SortedImagesViewModel.BeachesSeaside.Add(tmpImg);
                    break;
                case "events parties":
                    SortedImagesViewModel.EventsParties.Add(tmpImg);
                    break;
                case "food drinks":
                    SortedImagesViewModel.FoodDrinks.Add(tmpImg);
                    break;
                case "paintings art":
                    SortedImagesViewModel.PaintingsArt.Add(tmpImg);
                    break;
                case "pets animals":
                    SortedImagesViewModel.PetsAnimals.Add(tmpImg);
                    break;
                case "text visuals":
                    SortedImagesViewModel.TextVisuals.Add(tmpImg);
                    break;
                case "sunrises sunsets":
                    SortedImagesViewModel.SunrisesSunsets.Add(tmpImg);
                    break;
                case "cars vehicles":
                    SortedImagesViewModel.CarsVehicles.Add(tmpImg);
                    break;
                case "macro flowers":
                    SortedImagesViewModel.MacroFlowers.Add(tmpImg);
                    break;
                case "streetview architecture":
                    SortedImagesViewModel.StreetviewArchitecture.Add(tmpImg);
                    break;
                case "people portraits":
                    SortedImagesViewModel.PeoplePortraits.Add(tmpImg);
                    break;
                default:

                    break;
            }
        }

        #endregion

        #region CanExecute

        private bool CanClear(object arg)
        {
            return Images.Count > 0;
        }

        private bool CanAnalyze(ICollection arg)
        {
            return UploadResult != null && UploadResult.Count > 0;
        }

        #endregion

        #region Sync

        private void SyncOnBackGrndWithService()
        {
            using (BackgroundWorker bg = new BackgroundWorker())
            {
                bg.DoWork += (sender, args) => SyncAsyncImages();
                bg.RunWorkerAsync();
            }
        }

        private void SyncAsyncImages()
        {
            try
            {
                ImageDatabaseService imgDbService = new ImageDatabaseService();
                ImageService imgService = new ImageService();
                imgService.SynchronizeWithServiceDB();

                List<ImageModel> serviceImages = imgDbService.GetAllImages().Where(img => !img.IsProcessed).ToList();

                foreach (ImageModel img in serviceImages)
                {
                    img.ImagePath = ImageHelperService.SaveImageToDisk(img);
                    Images.Add(img);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion

        #endregion
    }
}
