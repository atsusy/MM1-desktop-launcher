using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace DesktopLauncher
{
    public class CustomURIViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UriLauncher uriLauncher;

        public CustomURIViewModel()
        {
            uriLauncher = new UriLauncher( "", "");
        }

        public CustomURIViewModel(UriLauncher uriLauncher)             
        {
            this.uriLauncher = uriLauncher;
        }        

        public string Name
        {
            get
            {
                return uriLauncher.Name;
            }
            set
            {
                uriLauncher.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Uri
        {
            get
            {
                return uriLauncher.Id;
            }
            set
            {
                uriLauncher.Id = value;
                OnPropertyChanged("Uri");                     
            }
        }

        public override string ToString()
        {
            return uriLauncher.ToString();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomURIValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            CustomURIViewModel customURI = (value as BindingGroup).Items[0] as CustomURIViewModel;
            if (customURI.Name.Length == 0 || customURI.Uri.Length == 0)
            {
                return new ValidationResult(false, "Input all columns.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }

}
