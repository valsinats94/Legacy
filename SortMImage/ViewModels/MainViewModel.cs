using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using SortMImage.Models;
using SortMImage.Services;
using SortMImage.Services.DatabaseServices;
using SortMImage.Views;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SortMImage.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Declarations

        private List<BaseViewModel> steps;
        private BaseViewModel currentStep;
        private int step;
        private static MainViewModel mainViewModelInstance;

        private bool isLoggedIn;

        private LoginViewModel loginViewModel;
        private ImagesViewModel imagesViewModel;

        private DelegateCommand<object> synchronizeCommand;

        #endregion

        #region Constructors

        private MainViewModel()
        {
            Step = 0;
            InitSteps();
            ChangeViewModelDueToStep();
        }

        #endregion

        #region Properties

        public static MainViewModel MainViewModelInstance
        {
            get
            {
                if (mainViewModelInstance == null)
                    mainViewModelInstance = new MainViewModel();

                return mainViewModelInstance;
            }
        }

        public DelegateCommand<object> SynchronizeCommand
        {
            get
            {
                if (synchronizeCommand == null)
                    synchronizeCommand = new DelegateCommand<object>(SyncImages);

                return synchronizeCommand;
            }
        }

        public List<BaseViewModel> Steps
        {
            get
            {
                if (steps == null)
                    steps = new List<BaseViewModel>();

                return steps;
            }
        }

        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                if (value == step)
                    return;

                if (step < 0)
                    return;

                step = value;

                OnPropertyChanged("Step");
                InitSteps();
                ChangeViewModelDueToStep();
            }
        }

        public BaseViewModel CurrentStep
        {
            get
            {
                return currentStep;
            }

            set
            {
                if (currentStep == value)
                    return;

                currentStep = value;
                OnPropertyChanged();
                OnPropertyChanged("CurrentStep");
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }
            set
            {
                if (value == isLoggedIn)
                    return;

                isLoggedIn = value;
                OnPropertyChanged();
                Step++;
                OnPropertyChanged("Step");
            }
        }

        #endregion

        #region Methods

        private void InitSteps()
        {
            switch (Step)
            {
                case 1:
                    if (loginViewModel == null)
                    {
                        loginViewModel = new LoginViewModel();
                        //LoginView loginView = new LoginView();
                        //loginViewModel.View = loginView;
                        Steps.Add(loginViewModel);
                    }
                    break;
                case 0:
                    if (imagesViewModel == null)
                    {
                        imagesViewModel = new ImagesViewModel();
                        ImagesView imagesView = new ImagesView();
                        imagesViewModel.View = imagesView;
                        Steps.Add(imagesViewModel);
                    }
                    break;
            }

            //ChangeViewModelDueToStep();
        }

        private void ChangeViewModelDueToStep()
        {
            switch (Step)
            {
                case 0:
                    CurrentStep = Steps[0];
                    break;
                case 1:
                    CurrentStep = Steps[1];
                    break;
            }
        }

        private void SyncImages(object obj)
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

                if (imagesViewModel != null)
                {
                    List<ImageModel> serviceImages = imgDbService.GetAllImages().Where(img => !img.IsProcessed).ToList();

                    foreach (ImageModel img in serviceImages)
                    {
                        img.ImagePath = ImageHelperService.SaveImageToDisk(img);
                        imagesViewModel.Images.Add(img);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion
    }
}
