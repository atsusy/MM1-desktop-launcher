﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DesktopLauncher.Extensions;
using System.Reflection;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace DesktopLauncher
{
    /// <summary>
    /// OptionsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionsWindows : Window
    {
        public OptionsWindows()
        {
            InitializeComponent();
            ApplicationTitle.Content = string.Format(ApplicationTitle.Content as string, Assembly.GetExecutingAssembly().GetName().Version);
        }

        public IReadOnlyList<ILaunchable> Apps
        {
            set
            {
                (this.Resources["ViewModel"] as OptionsViewModel).Apps = new ObservableCollection<ILaunchable>(value);
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            (this.Resources["ViewModel"] as OptionsViewModel).Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource optionsViewModelViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("optionsViewModelViewSource")));
            // CollectionViewSource.Source プロパティを設定してデータを読み込みます:
            // optionsViewModelViewSource.Source = [汎用データ ソース]
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var rowItem = (e.Source as Button).Tag;

                    // 既存行の場合
                    if(rowItem is ExtraFolderViewModel)
                    {
                        (rowItem as ExtraFolderViewModel).FolderPath = dialog.SelectedPath;
                    }
                    // 新規行の場合
                    else
                    {
                        var viewModel = FindResource("ViewModel") as OptionsViewModel;
                        viewModel.ExtraFolders.Add(new ExtraFolderViewModel(dialog.SelectedPath, ""));
                    }
                }
            }
        }

        private void HotKey_KeyUp(object sender, KeyEventArgs e)
        {
            HotKey.SelectedValue = HotKeyConverter.ConvertToString(e.Key);
        }

        private void RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
