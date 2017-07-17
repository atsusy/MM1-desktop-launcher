using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DesktopLauncher
{
    public class AppAlias : ILaunchable
    {
        private string name;
        private ILaunchable target;

        public AppAlias(string name, ILaunchable aLaunchable)
        {
            this.name = name;
            target = aLaunchable;
        }

        public string Id => target.Id;
        public string Name => name;
        public string DisplayName => target.Name;
        public ImageSource Icon => target.Icon;
        public int Launched
        {
            get => target.Launched;
            set
            {

            }
        }
        public bool RunAs => target.RunAs;

        public void LaunchAsync(string parameters)
        {
            target.LaunchAsync(parameters);
        }

        public void LaunchAsyncRunAs(string parameters)
        {
            target.LaunchAsyncRunAs(parameters);
        }
    }
}
