using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopLauncher
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public OptionsViewModel() : this(Properties.Settings.Default)
        {
        }

        OptionsViewModel(DesktopLauncher.Properties.Settings settings)
        {
            var keyConverter = new KeyConverter();
            var modifiers = (ModifierKeys)settings.HotKeyModifiers;

            Theme = settings.Theme;
            HotKey = settings.HotKeyCharacter;
            HotKeyControl = (modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            HotKeyShift = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            HotKeyAlt = (modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            HotKeyWin = (modifiers & ModifierKeys.Windows) == ModifierKeys.Windows;

            Aliases = new ObservableCollection<AliasViewModel>();
            if(settings.Aliases != null)
            {
                foreach (var alias in settings.Aliases)
                {
                    Aliases.Add(new AliasViewModel(alias));
                }
            }

            CustomURIs = new ObservableCollection<CustomURIViewModel>();
            foreach (var uriLauncher in UriLauncher.FindAllUriLaunchers())
            {
                CustomURIs.Add(new CustomURIViewModel(uriLauncher));
            }

            ExtraFolders = new ObservableCollection<ExtraFolderViewModel>();
            if (settings.ExtraFolders != null) {
                foreach (var extraFolder in settings.ExtraFolders)
                {
                    if (extraFolder.Length > 0 && Directory.Exists(extraFolder))
                    {
                        ExtraFolders.Add(new ExtraFolderViewModel(extraFolder));
                    }
                }
            }
        }

        private string _Theme;
        public string Theme
        {
            get
            {
                return _Theme;
            }
            set
            {
                _Theme = value;
                OnPropertyChanged("Theme");
            }
        }

        public string HotKey
        {
            get;
            set;
        }

        public bool HotKeyControl
        {
            get;
            set;
        }

        public bool HotKeyShift
        {
            get;
            set;
        }

        public bool HotKeyAlt
        {
            get;
            set;
        }

        public bool HotKeyWin
        {
            get;
            set;
        }

        public string HotKeyDescription
        {
            get
            {
                List<String> keys = new List<string>();
                if (HotKeyControl) { keys.Add("Ctrl"); }
                if (HotKeyShift) { keys.Add("Shift"); }
                if (HotKeyAlt) { keys.Add("Alt"); }
                if (HotKeyWin) { keys.Add("Win"); }

                keys.Add(HotKeysDictionary[HotKey]);
                return string.Join("+", keys.ToArray());
            }
        }

        private ObservableCollection<ILaunchable> apps;
        [Bindable(true)]
        public ObservableCollection<ILaunchable> Apps
        {
            get => apps;
            set
            {
                apps = value;
                OnPropertyChanged("Apps");
            }
        }

        [Bindable(true)]
        public ObservableCollection<AliasViewModel> Aliases
        {
            get;
            set;
        }

        [Bindable(true)]
        public ObservableCollection<CustomURIViewModel> CustomURIs
        {
            get;
            set;
        }

        [Bindable(true)]
        public ObservableCollection<ExtraFolderViewModel> ExtraFolders
        {
            get;
            set;
        }

        public IReadOnlyList<KeyValuePair<string, string>> HotKeyItems
        {
            get
            {               
                return HotKeysDictionary
                    .ToList()
                    .OrderBy(item => item.Value)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public Dictionary<string, string> HotKeysDictionary
        {
            get
            {
                var resourceManager = Properties.HotKeys.ResourceManager;
                var resourceSet = resourceManager.GetResourceSet(
                    CultureInfo.CurrentCulture,
                        true,
                        true
                    ).Cast<DictionaryEntry>();
                return resourceSet
                    .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString());
            }            
        }
 
        public void Save()
        {
            var settings = Properties.Settings.Default;

            settings.Theme = Theme;

            var keyConverter = new KeyConverter();
            settings.HotKeyCharacter = HotKey;

            var modifiers = 0;
            modifiers |= (HotKeyControl) ? (int)ModifierKeys.Control : 0;
            modifiers |= (HotKeyShift) ? (int)ModifierKeys.Shift : 0;
            modifiers |= (HotKeyAlt) ? (int)ModifierKeys.Alt : 0;
            modifiers |= (HotKeyWin) ? (int)ModifierKeys.Windows : 0;
            settings.HotKeyModifiers = modifiers;

            settings.Aliases.Clear();
            foreach (var alias in Aliases)
            {
                settings.Aliases.Add(alias.ToString());
            }

            settings.UriEntries.Clear();
            foreach (var customURI in CustomURIs)
            {
                settings.UriEntries.Add(customURI.ToString());
            }

            if(ExtraFolders.Count > 0)
            {
                if(settings.ExtraFolders == null)
                {
                    settings.ExtraFolders = new System.Collections.Specialized.StringCollection();
                }
                settings.ExtraFolders.Clear();
                foreach (var extraFolder in ExtraFolders)
                {
                    settings.ExtraFolders.Add(extraFolder.FolderPath);
                }
            }
            settings.Save();
        }
    }
}
