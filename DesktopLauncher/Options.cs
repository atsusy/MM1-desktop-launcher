using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLauncher
{
    class Options
    {
        public const string DefaultExtentions = "*.exe|*.lnk";

        private Properties.Settings settings = Properties.Settings.Default;

        private Options()
        {
        }

        public static Options SingletonOptions => new Options();
            
        public int HotKeyModifiers
        {
            get => settings.HotKeyModifiers;
            set => settings.HotKeyModifiers = value;
        }

        public string HotKeyCharacter
        {
            get => settings.HotKeyCharacter;
            set => settings.HotKeyCharacter = value;
        }

        public Point WindowPosition
        {
            get => settings.WindowPosition;
            set => settings.WindowPosition = value;
        }

        public StringCollection UriEntries
        {
            get => settings.UriEntries;
            set => settings.UriEntries = value;
        }

        public StringCollection ExtraFolders
        {
            get => settings.ExtraFolders;
            set => settings.ExtraFolders = value;
        }

        public string Theme
        {
            get => settings.Theme;
            set => settings.Theme = value;
        }

        public StringCollection Aliases
        {
            get => settings.Aliases;
            set => settings.Aliases = value;
        }

        public bool LaunchAtLogin
        {
            get => settings.LaunchAtLogin;
            set
            {
                settings.LaunchAtLogin = value;

                RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                string productName = "MM1 desktop launcher";

                if (value)
                {
                    string startPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\MM1 desktop launcher.lnk";
                    rk.SetValue(productName, startPath);
                }
                else
                {
                    if (rk.GetValue(productName) != null)
                    {
                        rk.DeleteValue(productName);
                    }
                }
            }
        }

        public decimal Opacity
        {
            get => settings.Opacity;
            set => settings.Opacity = value;
        }

        public StringCollection LaunchCounts
        {
            get => settings.LaunchCounts;
            set => settings.LaunchCounts = value;
        }

        public StringCollection ExtraExtentions
        {
            get => settings.ExtraExtentions;
            set => settings.ExtraExtentions = value;
        }

        public IEnumerable<Tuple<string, string>> ExtraFoldersAndExtenstions
        {
            get
            {
                var folders = settings.ExtraFolders?.Cast<string>();
                if(folders == null)
                {
                    return new Tuple<string, string>[] { };
                }

                var extentions = settings.ExtraExtentions?.Cast<string>();
                if(extentions == null || folders.Count() != extentions.Count())
                {
                    extentions = new string[folders.Count()];
                }
                return folders.Zip(extentions, (f, e) => new Tuple<string, string>(f, e ?? DefaultExtentions)).ToList().AsReadOnly();
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
