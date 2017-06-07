using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopLauncher
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public void ChangeTheme(string theme)
        {
            var themeDict = new ResourceDictionary();
            themeDict.Source = new Uri(string.Format("pack://application:,,,/Themes/{0}.xaml", theme));

            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            Application.Current.Resources.MergedDictionaries.Insert(0, themeDict);
        }
    }
}
