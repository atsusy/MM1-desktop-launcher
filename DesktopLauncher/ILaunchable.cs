using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DesktopLauncher
{
    public interface ILaunchable
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        ImageSource Icon { get; }
        int Launched { get; set; }

        void LaunchAsync(string parameters);
    }
}
