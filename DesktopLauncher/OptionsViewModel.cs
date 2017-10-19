using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DesktopLauncher
{
    public class OptionsViewModel : ViewModelBase
    {
        private Options options;

        public OptionsViewModel() : this(Options.SingletonOptions)
        {
        }

        OptionsViewModel(Options options)
        {
            this.options = options;
            
            var modifiers = (ModifierKeys)options.HotKeyModifiers;

            Theme = options.Theme;
            Opacity = options.Opacity;

            HotKey = options.HotKeyCharacter;
            HotKeyControl = (modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            HotKeyShift = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            HotKeyAlt = (modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            HotKeyWin = (modifiers & ModifierKeys.Windows) == ModifierKeys.Windows;
            LaunchAtLogin = options.LaunchAtLogin;

            Aliases = new ObservableCollection<AliasViewModel>();
            if(options.Aliases != null)
            {
                foreach (var alias in options.Aliases)
                {
                    Aliases.Add(new AliasViewModel(alias));
                }
            }

            CustomURIs = new ObservableCollection<CustomURIViewModel>();
            foreach (var uriLauncher in UriLauncher.FindAllUriLaunchers())
            {
                CustomURIs.Add(new CustomURIViewModel(uriLauncher));
            }

            var extraFolderViewModels = options.ExtraFoldersAndExtenstions
                .Where((t) => t.Item1.Length > 0 && Directory.Exists(t.Item1))
                .Select((t) => new ExtraFolderViewModel(t.Item1, t.Item2));
            ExtraFolders = new ObservableCollection<ExtraFolderViewModel>(extraFolderViewModels);
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

        private decimal _Opacity;
        public decimal Opacity
        {
            get => _Opacity;
            set
            {
                _Opacity = value;
                OnPropertyChanged("Opacity");
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

                keys.Add(HotKey);
                return string.Join("+", keys.ToArray());
            }
        }

        private bool launchAtLogin;
        public bool LaunchAtLogin
        {
            get
            {
                return launchAtLogin;
            }
            set
            {
                launchAtLogin = value;
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

        private Dictionary<string, string> hotKeyDisplayMembers;
        private Dictionary<string, string> HotKeyDisplayMembers
        {
            get
            {
                if (hotKeyDisplayMembers == null)
                {
                    hotKeyDisplayMembers = new Dictionary<string, string> {
                        { "CapsLock", "Caps Lock" },
                        { "Scroll", "Scroll Lock" },
                        { "NumLock", "Num Lock" },
                        { "PageUp", "Page Up" },
                        { "PageDown", "Page Down" },
                    };
                }
                return hotKeyDisplayMembers;
            }
        }

        public IReadOnlyList<KeyValuePair<string, string>> HotKeyItems
        {
            get
            {                
                return Properties.Settings.Default.HotKeys.Cast<string>().ToList()
                    .Select((hotKey) => new KeyValuePair<string, string>(
                        hotKey,
                        HotKeyDisplayMembers.ContainsKey(hotKey) ? HotKeyDisplayMembers[hotKey]: hotKey))
                    .OrderBy(item => item.Value)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public void Save()
        {
            options.Theme = Theme;
            options.Opacity = Opacity;

            options.HotKeyCharacter = HotKey;

            var modifiers = 0;
            modifiers |= (HotKeyControl) ? (int)ModifierKeys.Control : 0;
            modifiers |= (HotKeyShift) ? (int)ModifierKeys.Shift : 0;
            modifiers |= (HotKeyAlt) ? (int)ModifierKeys.Alt : 0;
            modifiers |= (HotKeyWin) ? (int)ModifierKeys.Windows : 0;
            options.HotKeyModifiers = modifiers;
            options.LaunchAtLogin = LaunchAtLogin;

            options.Aliases.Clear();
            foreach (var alias in Aliases)
            {
                options.Aliases.Add(alias.ToString());
            }

            options.UriEntries.Clear();
            foreach (var customURI in CustomURIs)
            {
                options.UriEntries.Add(customURI.ToString());
            }

            if(ExtraFolders.Count > 0)
            {
                if(options.ExtraFolders == null)
                {
                    options.ExtraFolders = new System.Collections.Specialized.StringCollection();
                }
                if(options.ExtraExtentions == null)
                {
                    options.ExtraExtentions = new System.Collections.Specialized.StringCollection();
                }
                options.ExtraFolders.Clear();
                options.ExtraExtentions.Clear();
                foreach (var extraFolder in ExtraFolders)
                {
                    options.ExtraFolders.Add(extraFolder.FolderPath);
                    options.ExtraExtentions.Add(extraFolder.Extentions);
                }
            }               

            options.Save();
        }
    }
}
