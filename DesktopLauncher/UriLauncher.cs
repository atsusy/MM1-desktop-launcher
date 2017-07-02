using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.System;

namespace DesktopLauncher
{
    public class UriLauncher : ILaunchable
    {
        private string uri;
        private string name;

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

        public ImageSource Icon => null;

        private int launched;
        public int Launched
        {
            get => launched;
            set
            {
                launched = value;
            }
        }

        public async void LaunchAsync(string parameterString)
        {            
            var parameters = parameterString.Split(" ".ToCharArray());
            await Launcher.LaunchUriAsync(new Uri(string.Format(uri, parameters)));
            Launched++;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", Name, Id);
        }
    }
}
