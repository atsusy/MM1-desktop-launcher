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
    public class AliasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UriLauncher uriLauncher;

        public AliasViewModel()
        {
            uriLauncher = new UriLauncher("", "", "");
        }

        public AliasViewModel(UriLauncher uriLauncher)             
        {
            this.uriLauncher = uriLauncher;
        }        

        public string Alias
        {
            get
            {
                return uriLauncher.AliasName;
            }
            set
            {
                uriLauncher.AliasName = value;
                OnPropertyChanged("Alias");
            }
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

    public class AliasValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            AliasViewModel alias = (value as BindingGroup).Items[0] as AliasViewModel;
            if (alias.Alias.Length == 0 || alias.Name.Length == 0 || alias.Uri.Length == 0)
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
