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
        ImageSource Icon { get; }

        void LaunchAsync(string parameters);
    }
}
