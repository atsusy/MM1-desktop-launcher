using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.System;
using Windows.Storage.Streams;
using System.IO;
using DesktopLauncher.Extensions;

namespace DesktopLauncher
{
    class StoreApp : ILaunchable
    {
        public static async Task<IReadOnlyList<ILaunchable>> FindAllStoreApps()
        {
            var storeApps = new List<StoreApp>();
            var pm = new PackageManager();
            foreach (var p in pm.FindPackagesForUser(""))
            {
                var apps = await FindStoreAppsInPackage(p);
                if (apps != null)
                {
                    storeApps.AddRange(apps);
                }
            }
            return storeApps.AsReadOnly();
        }

        static async Task<IReadOnlyList<StoreApp>> FindStoreAppsInPackage(Package p)
        {
            if(p.IsFramework || p.IsResourcePackage)
            {
                return null;
            }

            var entries = await p.GetAppListEntriesAsync();
            if(entries.Count == 0)
            {
                return null;
            }

            var apps = new List<StoreApp>();
            foreach(var entry in entries)
            {
                var app = new StoreApp(p, entry);
                app.logo = await app.GetLogo();
                apps.Add(app);
            }

            // Nameが重複する場合、最後の要素が代表することにする(今のところ、Edgeを起動できるようにするためだけ）
            return apps.GroupBy(app => app.Name).Select(g => g.Last()).ToList().AsReadOnly();           
        }

        private AppListEntry entry;
        private string id;
        private ImageSource logo;
             
        protected StoreApp(Package package, AppListEntry entry)
        {
            id =  string.Format("{0}/{1}", package.Id.FullName, entry.DisplayInfo.DisplayName);
            this.entry = entry;            
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
       
        public string Name
        {
            get
            {
                return entry.DisplayInfo.DisplayName;
            }
        }        

        public ImageSource Icon
        {
            get
            {                          
                return logo;
            }
        }

        private int launched;
        public int Launched
        {
            get => launched;
            set
            {
                launched = value;
            }
        }

        public async void LaunchAsync(string parameters)
        {
            var result = await entry.LaunchAsync();
            Launched++;
        }

        private async Task<ImageSource> GetLogo()
        {
            try
            {
                var rasRef = entry.DisplayInfo.GetLogo(new Windows.Foundation.Size(1024, 1024));
                using (var stream = await rasRef.OpenReadAsync())
                {
                    byte[] buffer = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);

                        reader.ReadBytes(buffer);
                        var image = new BitmapImage();

                        using (var memory = new MemoryStream(buffer))
                        {
                            memory.Position = 0;
                            image.BeginInit();
                            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.UriSource = null;
                            image.StreamSource = memory;
                            image.EndInit();
                        }

                        image.Freeze();
                        return image;
                    } 
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
