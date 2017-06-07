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
            foreach(var uriLauncher in UriLauncher.FindAllUriLaunchers())
            {
                Aliases.Add(new AliasViewModel(uriLauncher));
            }

            ExtraFolders = new ObservableCollection<ExtraFolderViewModel>();
            if(settings.ExtraFolders != null) {
                foreach (var extraFolder in settings.ExtraFolders)
                {
                    if(extraFolder.Length > 0 && Directory.Exists(extraFolder))
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

        [Bindable(true)]
        public ObservableCollection<AliasViewModel> Aliases
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
                var resourceManager = Properties.HotKeys.ResourceManager;
                var resourceSet = resourceManager.GetResourceSet(
                    CultureInfo.CurrentCulture, 
                    true, 
                    true
                ).Cast<DictionaryEntry>();
                return resourceSet
                    .ToDictionary(r => r.Key.ToString(), r => r.Value.ToString())
                    .ToList()
                    .OrderBy(item => item.Value)
                    .ToList()
                    .AsReadOnly();                
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

            settings.UriEntries.Clear();
            foreach (var alias in Aliases)
            {
                settings.UriEntries.Add(alias.ToString());
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
