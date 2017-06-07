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
        private string alias;

        public static IReadOnlyList<UriLauncher> FindAllUriLaunchers()
        {
            var uriLaunchers = new List<UriLauncher>();
            var settings = Properties.Settings.Default.UriEntries;

            foreach(var setting in settings)
            {
                var columns = setting.Split("|".ToCharArray());

                if(columns.Length != 3)
                {
                    continue;
                }

                uriLaunchers.Add(new UriLauncher(columns[0], columns[1], columns[2]));
            }

            return uriLaunchers.AsReadOnly();
        }

        public UriLauncher(string alias, string name, string uri)
        {
            this.uri = uri;
            this.name = name;
            this.alias = alias;
        }

        public string Id
        {
            get
            {
                return uri;
            }
            set
            {
                uri = value;
            }
        }        

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string AliasName
        {
            get
            {
                return alias;
            }
            set
            {
                alias = value;
            }
        }

        public ImageSource Icon => null;

        public async void LaunchAsync(string parameterString)
        {            
            var parameters = parameterString.Split(" ".ToCharArray());
            await Launcher.LaunchUriAsync(new Uri(string.Format(uri, parameters)));
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", AliasName, Name, Id);
        }
    }
}
