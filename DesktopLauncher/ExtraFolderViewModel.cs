using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLauncher
{
    public class ExtraFolderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExtraFolderViewModel()
        {
            _FolderPath = "";
        }

        public ExtraFolderViewModel(string folderPath)
        {
            _FolderPath = folderPath;
        }

        private string _FolderPath;
        public string FolderPath
        {
            get
            {
                return _FolderPath;
            }
            set
            {
                _FolderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }
    }
}
