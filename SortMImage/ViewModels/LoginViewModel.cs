using System.Collections.Generic;
using System.Security;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using SortMImage.Models;
using SortMImage.Services.DatabaseServices;

namespace SortMImage.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Declarations

        private bool isFaceLoginType;
        private int timeout;
        
        private string username;
        private DelegateCommand<object> loginCommand;

        #endregion

        #region Constructors

        public LoginViewModel()
        {
            IsFaceLoginType = false;
        }

        #endregion

        #region Propertis

        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                if (value == timeout)
                    return;

                timeout = value;
                OnPropertyChanged();
                SetLoginType(timeout);
            }
        }

        public bool IsFaceLoginType
        {
            get
            {
                return isFaceLoginType;
            }
            set
            {
                if (value == isFaceLoginType)
                    return;

                isFaceLoginType = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand<object> LoginCommand
        {
            get
            {
                if (loginCommand == null)
                    loginCommand = new DelegateCommand<object>(Login, CanLogin);

                return loginCommand;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (value == username)
                    return;

                username = value;
                OnPropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoggedIn { get; set; }

        #endregion

        #region Methods

        private void SetLoginType(int timeout)
        {
            IsFaceLoginType = timeout >= 10000 ? false : true;
        }

        private void Login(object parameter)

        {
            if (parameter == null)
                return;

            PasswordBox passwordBox = parameter as PasswordBox;

            UserDatabaseService userDBservice = new UserDatabaseService();
            IsLoggedIn = userDBservice.IsLoggedIn(Username, passwordBox.Password);
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrEmpty(Username);
        }

        #endregion
    }
}
