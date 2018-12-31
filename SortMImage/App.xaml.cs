using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SortMImage.Models;
using SortMImage.Services;
using SortMImage.Services.DatabaseServices;
using SortMImage.ViewModels;
using SortMImage.Views;

namespace SortMImage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel mainViewModel = MainViewModel.MainViewModelInstance;
            MainWindow mainWindow = new MainWindow();
            mainViewModel.View = mainWindow;

            if (!mainWindow.IsLoaded)
                mainWindow.Show();

            base.OnStartup(e);

            if (!InternetService.IsInternetConnected())
            {
                MessageBox.Show("Sorry! You do NOT have internet connection! Please, try again later!",
                                   "Error",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);

                Current.Shutdown();
            }

            //Login();
            SynchronizeWithServiceDB();
        }

        private void Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            LoginWindowView loginView = new LoginWindowView();

            loginViewModel.View = loginView;

            bool? res = loginView.ShowDialog() & loginViewModel.IsLoggedIn;

            if (res == null || res != true)
            {
                Current.Shutdown();
            }
        }

        private void SynchronizeWithServiceDB()
        {
            try
            {
                ImageDatabaseService imgDbService = new ImageDatabaseService();
                ImageService imgService = new ImageService();

                imgService.SynchronizeWithServiceDB();

                List<ImageModel> serviceImages = imgDbService.GetAllImages().Where(img => !img.IsProcessed).ToList();

                foreach (ImageModel img in serviceImages)
                {
                    ImageHelperService.SaveImageToDisk(img);
                }

                List<ImageModel> inconsistentImages = imgDbService.GetAllImages().Where(img => string.IsNullOrEmpty(img.ImagePath)).ToList();
                foreach(ImageModel image in inconsistentImages)
                {
                    imgDbService.DeleteImageFromDatabase(image);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
