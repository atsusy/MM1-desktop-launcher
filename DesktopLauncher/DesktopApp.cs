using Smdn.Windows.UserInterfaces.Shells;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopLauncher
{
    class DesktopApp : ILaunchable
    {
        public static IReadOnlyList<ILaunchable> FindAllDesktopApps()
        {
            var desktopApps = new List<ILaunchable>();

            desktopApps.AddRange(FindDesktopApps(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)));
            desktopApps.AddRange(FindDesktopApps(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)));

            var extraFolders = Properties.Settings.Default.ExtraFolders;
            if(extraFolders != null && extraFolders.Count > 0)
            {
                foreach(var extraFolder in extraFolders)
                {
                    desktopApps.AddRange(FindDesktopApps(extraFolder));
                }
            }

            return desktopApps.AsReadOnly();
        }

        private static IReadOnlyList<ILaunchable> FindDesktopApps(string path)
        {
            var desktopApps = new List<ILaunchable>();

            string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                DirectoryInfo dinfo = new DirectoryInfo(dir);
                if (!dinfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    string[] files = DirectoryHelper.GetFilesEx(dir, "*.lnk|*.exe", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);

                        try
                        {
                            desktopApps.Add(new DesktopApp(name, file));
                        }
                        catch
                        {

                        }
                    }
                }
            }

            foreach (var file in DirectoryHelper.GetFilesEx(path, "*.lnk|*.exe", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                try
                {

                    desktopApps.Add(new DesktopApp(name, file));
                }
                catch
                {

                }
            }

            return desktopApps.AsReadOnly();
        }

        private string name;
        private string executionPath;

        private DesktopApp(string name, string executionPath)
        {
            this.name = name;

            if(Path.GetExtension(executionPath) == ".lnk")
            {
                executionPath = new ShellLink(executionPath).TargetPath;
            }
            this.executionPath = executionPath;
        }

        public string Id
        {
            get
            {
                return executionPath;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public string AliasName
        {
            get
            {
                return "";
            }
        }

        public ImageSource Icon
        {
            get
            {
                try
                {
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(executionPath);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        icon.ToBitmap().GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
                }
                catch
                {
                    return null;
                }
            }
        }        

        public void LaunchAsync(string parameters)
        {
            Process.Start(executionPath);
        }  
    }
}
