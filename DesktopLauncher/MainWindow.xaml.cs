using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.System;

namespace DesktopLauncher
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ILaunchable> entries = new List<ILaunchable>();
        private HotKey.HotKey hotKey;
        private double initialHeight;
        private const double candidatesHeight = 150.0;

        public MainWindow()
        {
            InitializeComponent();
            initialHeight = Height;

            // 前バージョンの設定を引き継ぐ
            if (!Properties.Settings.Default.IsUpgraded)
            {
                Properties.Settings.Default.Upgrade();                    
                Properties.Settings.Default.IsUpgraded = true;
                Properties.Settings.Default.Save();
            }
        }        

        private void ShowLauncher()
        { 
            Show();
            Activate();
            InputText.Focus();
        }

        private void HideLauncher()
        {
            InputText.Text = "";
            Candidates.DataContext = null;
            Hide();
        }

        private void LoadOptions()
        {
            var settings = Properties.Settings.Default;

            (App.Current as App).ChangeTheme(settings.Theme);
            this.Opacity = (double)settings.Opacity;

            if (settings.WindowPosition.X >= 0)
            { 
                Left = settings.WindowPosition.X;
            }

            if(settings.WindowPosition.Y >= 0)
            {
                Top = Properties.Settings.Default.WindowPosition.Y;
            }

            if (hotKey != null)
            {
                hotKey.Pressed -= HotKey_Pressed;
                hotKey.Dispose();
            }
            try
            {
                hotKey = new HotKey.HotKey(this, (ModifierKeys)settings.HotKeyModifiers, HotKeyConverter.ConvertFromString(settings.HotKeyCharacter));
                hotKey.Pressed += HotKey_Pressed;
            }
            catch
            {
                MessageBox.Show("Hot-key is already in use. Please use another key.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            (NotifyIcon.ContextMenu.Items[0] as MenuItem).InputGestureText = new OptionsViewModel().HotKeyDescription;
        }

        private IList<ILaunchable> LoadAliases(IList<ILaunchable> entries)
        {
            var aliases = new List<ILaunchable>();
            var settings = Properties.Settings.Default;
            if (settings.Aliases?.Count > 0)
            {
                foreach (var alias in settings.Aliases)
                {
                    var fields = alias.Split(new char[] { '|' });
                    var name = fields[0];
                    var id = fields[1];
                    var targets = entries.Where(e => e.Id == id);
                    if(targets.Count() != 1)
                    {
                        continue;
                    }
                    aliases.Add(new AppAlias(name, targets.Single()));                    
                }
            }
            return aliases;
        }

        private void LoadLaunchedCounts(IReadOnlyList<ILaunchable> entries)
        {
            var settings = Properties.Settings.Default;
            if (settings.LaunchCounts?.Count > 0)
            {
                foreach(var launchCount in settings.LaunchCounts)
                {
                    var fields = launchCount.Split("|".ToCharArray());
                    if(fields.Count() != 2)
                    {
                        continue;
                    }
                    var id = fields[0];
                    var launched = 0;

                    int.TryParse(fields[1], out launched);

                    var targets = entries.Where((e) => e.Id == id && !(e is AppAlias));
                    if(targets.Count() != 1)
                    {
                        continue;
                    }
                    targets.Single().Launched = launched;
                }
            }
        }

        private void SaveLaunchedCounts(IReadOnlyList<ILaunchable> entries)
        {
            var settings = Properties.Settings.Default;
            var launchCounts = new Dictionary<string, int>();
            foreach(var entry in entries)
            {
                if(entry.Launched == 0)
                {
                    continue;
                }

                if (!launchCounts.ContainsKey(entry.Id))
                {
                    launchCounts.Add(entry.Id, entry.Launched);
                }
                else
                {
                    launchCounts[entry.Id] += entry.Launched;
                }
            }
          
            settings.LaunchCounts = new System.Collections.Specialized.StringCollection();
            foreach(var kv in launchCounts)
            {
                settings.LaunchCounts.Add(string.Format("{0}|{1}", kv.Key, kv.Value));
            }
            settings.Save();
        }

        private void SaveOptions()
        {
            var settings = Properties.Settings.Default;

            settings.WindowPosition = new System.Drawing.Point((int)Left, (int)Top);

            settings.Save();
        }

        private async Task Rescan(bool initial = false)
        {
            if (!initial) {
                SaveLaunchedCounts(entries);
            }

            LoadingIndicator.IsActive = true;
            InputText.IsEnabled = false;

            entries.Clear();
            entries.AddRange(await StoreApp.FindAllStoreApps());
            entries.AddRange(DesktopApp.FindAllDesktopApps());
            entries.AddRange(UriLauncher.FindAllUriLaunchers());
            entries.AddRange(LoadAliases(entries)); 
            entries.Sort((x, y) => x.Name.CompareTo(y.Name));

            LoadLaunchedCounts(entries);

            LoadingIndicator.IsActive = false;
            InputText.IsEnabled = true;
            InputText.Focus();
        }

        private void HotKey_Pressed(HotKey.HotKey obj)
        {
            if (IsVisible)
            {
                HideLauncher();
            }
            else
            {
                ShowLauncher();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOptions();
            await Rescan(true);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Escape)
            {
                HideLauncher();
                return;
            }

            if(e.Key == Key.Return)
            {
                var runAs = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                var launchable = Candidates.SelectedItem as ILaunchable;
                if (runAs && launchable.RunAs)
                {
                    launchable?.LaunchAsyncRunAs(InputText.Text);
                }
                else
                {
                    launchable?.LaunchAsync(InputText.Text);
                }

                HideLauncher();
                return;
            }
        }

        private void InputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text.ToLower();
            if(text.Length == 0)
            {
                Candidates.Height = 0.0;
                Height = initialHeight;

                Candidates.DataContext = null;
                return;
            }

            var keyword = text.Split(" ".ToCharArray()).First().ToLower();
            // ## Spec of listing order
            // 1. equals to keyword
            // 2. launched count
            // 3. forward match(any word)
            // 4. partial match
            var candidates = entries.Where(en => en.Name.ToLower().Contains(keyword)).OrderByDescending((en) =>
            {
                var name = en.Name.ToLower();
                if (name == keyword)
                {
                    return int.MaxValue;
                }
                var words = name.Split(" ".ToCharArray());
                var startsWith = words.Where((word) => word.StartsWith(keyword)).Count();
                return (en.Launched << 1) | (startsWith > 0 ? 1 : 0);
            }).Distinct(new LaunchableEqualityComparer());
            Candidates.DataContext = candidates;
            Candidates.SelectedIndex = 0;

            Candidates.Height = candidatesHeight;
            Height = initialHeight + Candidates.Height + Candidates.Margin.Bottom;
        }       

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if(Candidates.SelectedIndex > 0)
                {
                    Candidates.SelectedIndex--;
                }
            }

            if (e.Key == Key.Down)
            {
                if(Candidates.SelectedIndex < Candidates.Items.Count - 1)
                {
                    Candidates.SelectedIndex++;
                }
            }

            Candidates.ScrollIntoView(Candidates.SelectedItem);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void InputText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Candidates_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if(listBox == null || listBox.SelectedItem == null)
            {
                return;
            }
            (listBox.SelectedItem as ILaunchable).LaunchAsync(InputText.Text);
            HideLauncher();
        }

        private async void Options_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindows();

            optionsWindow.Owner = this;
            optionsWindow.Apps = entries.Where(entry => (entry is AppAlias) == false).ToList().AsReadOnly();

            var result = optionsWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                LoadOptions();
                await Rescan();
            }
        }

        private async void Rescan_Click(object sender, RoutedEventArgs e)
        {
            await Rescan();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SaveOptions();
            SaveLaunchedCounts(entries);
            App.Current.Shutdown();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            ShowLauncher();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            HideLauncher();
        }        
    }
}
