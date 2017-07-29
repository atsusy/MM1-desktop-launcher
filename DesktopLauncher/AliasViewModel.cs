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
    public class AliasViewModel : ViewModelBase
    {
        private string aliasName;
        private string appId;

        public AliasViewModel()
        {
            aliasName = "";
            appId = "";
        }

        public AliasViewModel(string text)
        {
            var texts = text.Split(new char[] { '|' });
            aliasName = texts[0];
            appId = texts[1];
        }

        public string AliasName
        {
            get
            {
                return aliasName;
            }
            set
            {
                aliasName = value;
                OnPropertyChanged("AliasName");
            }
        }

        public string AppId
        {
            get
            {
                return appId;
            }
            set
            {
                appId = value;
                OnPropertyChanged("AppId");                     
            }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", aliasName, appId);
        }
    }

    public class AliasValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            AliasViewModel alias = (value as BindingGroup).Items[0] as AliasViewModel;
            if (alias.AliasName.Length == 0 || alias.AppId.Length == 0)
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
