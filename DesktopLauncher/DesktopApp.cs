using Smdn.Windows.UserInterfaces.Shells;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            foreach(var extraFolder in Options.SingletonOptions.ExtraFoldersAndExtenstions)
            {
                desktopApps.AddRange(FindDesktopApps(extraFolder.Item1, extraFolder.Item2));
            }

            return desktopApps.AsReadOnly();
        }

        private static IReadOnlyList<ILaunchable> FindDesktopApps(string path, string extentions = Options.DefaultExtentions)
        {
            var desktopApps = new List<ILaunchable>();

            try
            {
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    DirectoryInfo dinfo = new DirectoryInfo(dir);
                    if (!dinfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        string[] files = DirectoryHelper.GetFilesEx(dir, extentions, SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var name = Path.GetFileNameWithoutExtension(file);

                            try
                            {
                                desktopApps.Add(new DesktopApp(name, file));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(string.Format("Failure to add desktop app:{0}\n{1}", file, ex.ToString()));
                            }
                        }
                    }
                }

                foreach (var file in DirectoryHelper.GetFilesEx(path, extentions, SearchOption.TopDirectoryOnly))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    try
                    {
                        desktopApps.Add(new DesktopApp(name, file));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(string.Format("Failure to add desktop app:{0}\n{1}", file, ex.ToString()));
                    }
                }
            }
            catch(UnauthorizedAccessException)
            {

            }

            return desktopApps.AsReadOnly();
        }

        private string name;
        private string executionPath;

        private DesktopApp(string name, string executionPath)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            if (String.IsNullOrEmpty(executionPath))
            {
                throw new ArgumentException("executionPath");
            }

            this.name = name;

            if(Path.GetExtension(executionPath) == ".lnk")
            {
                var targetPath = new ShellLink(executionPath).TargetPath;
                if (targetPath != null && targetPath.Length > 0)
                {
                    executionPath = targetPath;
                }
            }

            this.executionPath = executionPath;
        }

        public string Id => executionPath;
        public string Name => name;
        public string DisplayName => Name;
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

        private int launched;
        public int Launched
        {
            get => launched;
            set
            {
                launched = value;
            }
        }

        public bool RunAs => true;

        private void LaunchAsync(string parameters, bool runAs)
        {
            var args = parameters.Split(" ".ToCharArray());            
            var pi = new ProcessStartInfo(executionPath, string.Join(" ", args.Skip(1).ToArray()));
            if(runAs) { pi.Verb = "runas"; };
            try
            {
                Process.Start(pi);
                Launched++;
            }
            catch(Win32Exception)
            {
                
            }
        }

        public void LaunchAsync(string parameters)
        {
            LaunchAsync(parameters, false);
        }

        public void LaunchAsyncRunAs(string parameters)
        {
            LaunchAsync(parameters, true);
        }
    }
}
