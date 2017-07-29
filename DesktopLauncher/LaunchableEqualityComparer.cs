using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopLauncher
{
    class LaunchableEqualityComparer : EqualityComparer<ILaunchable>
    {
        public override bool Equals(ILaunchable x, ILaunchable y)
        {
            return x.Id.Equals(y.Id);
        }

        public override int GetHashCode(ILaunchable obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
