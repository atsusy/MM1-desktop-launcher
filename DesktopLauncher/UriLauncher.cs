using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.System;

namespace DesktopLauncher
{
    public class UriLauncher : ILaunchable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string uri;
        private string name;
        private ImageSource favicon;

        public static IReadOnlyList<UriLauncher> FindAllUriLaunchers()
        {
            var uriLaunchers = new List<UriLauncher>();
            var settings = Properties.Settings.Default.UriEntries;

            foreach(var setting in settings)
            {
                var columns = setting.Split("|".ToCharArray());

                if(columns.Length != 2)
                {
                    continue;
                }

                uriLaunchers.Add(new UriLauncher(columns[0], columns[1]));
            }

            return uriLaunchers.AsReadOnly();
        }

        public UriLauncher(string name, string uri)
        {
            this.uri = uri;
            this.name = name;
            DownloadFaviconAsync();
        }

        public string Id {
            get => uri;
            set
            {
                uri = value;
            }
        }        

        public string Name
        {
            get => name;
            set
            {
                name = value;
            }
        }

        public string DisplayName
        {
            get => Name;
        }

        public ImageSource Icon => favicon;

        private int launched;

        public int Launched
        {
            get => launched;
            set
            {
                launched = value;
            }
        }

        public bool RunAs => false;

        private int ParametersCount {
            get
            {
                var regex = new Regex(@"\{(?<no>\d+?)\}");
                if (!regex.IsMatch(uri))
                {
                    return 1;
                }
                return regex.Matches(uri).Cast<Match>().Select((g) => int.Parse(g.Groups["no"].ToString())).Max() + 1;
            }
        }

        public async void LaunchAsync(string parameterString)
        {
            var separator = " ";
            var parameters = parameterString.Split(separator.ToCharArray());
            if(parameters.Length > ParametersCount)
            {
                var p = parameters.Take(ParametersCount - 1).ToList();
                p.Add(string.Join(separator, parameters.Skip(ParametersCount - 1)));
                parameters = p.ToArray();
            }

            if(parameters.Length != ParametersCount)
            {
                return;
            }

            parameters = parameters.Select((q) => Uri.EscapeDataString(q)).ToArray();
            await Launcher.LaunchUriAsync(new Uri(string.Format(uri, parameters)));
            Launched++;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", Name, Id);
        }

        public void LaunchAsyncRunAs(string parameters)
        {
            throw new NotImplementedException();
        }

        private async void DownloadFaviconAsync()
        {
            if (string.IsNullOrEmpty(uri))
            {
                return;
            }

            using (var client = new HttpClient())
            {
                var hostName = new Uri(this.uri).Host;
                var uri = new Uri($"http://www.google.com/s2/favicons?domain={hostName}");
                try {
                    var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

                    using (var httpStream = await response.Content.ReadAsStreamAsync())
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = httpStream;
                        bitmapImage.EndInit();

                        favicon = bitmapImage;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Icon"));
                    }
                }
                catch
                {

                }
            }
        }
    }
}
