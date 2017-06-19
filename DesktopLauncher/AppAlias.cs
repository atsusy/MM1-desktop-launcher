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
        public ImageSource Icon => target.Icon;

        public void LaunchAsync(string parameters)
        {
            target.LaunchAsync(parameters);
        }
    }
}
