using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLauncher
{
    public class ExtraFolderViewModel : ViewModelBase
    {
        public ExtraFolderViewModel()
        {
            _FolderPath = "";
            _Extentions = Options.DefaultExtentions;
        }

        public ExtraFolderViewModel(string folderPath, string extentions)
        {
            _FolderPath = folderPath;
            _Extentions = extentions ?? Options.DefaultExtentions;
        }

        private string _FolderPath;
        public string FolderPath
        {
            get => _FolderPath;            
            set
            {
                _FolderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        private string _Extentions;
        public string Extentions
        {
            get => _Extentions;
            set
            {
                _Extentions = value;
                OnPropertyChanged("Extentions");
            }
        }
    }
}
