using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SortMImage.Interfaces;

namespace SortMImage.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Declarations
        private IView view;
        #endregion

        #region Properties
        public IView View
        {
            get
            {
                return view;
            }
            set
            {
                if (value == view)
                    return;

                view = value;
                view.DataContext = this;
                OnPropertyChanged();
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
