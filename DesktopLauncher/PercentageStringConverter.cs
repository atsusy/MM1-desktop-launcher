using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DesktopLauncher
{
    public class PercentageStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal v = 0m;

            decimal.TryParse(value.ToString(), out v);
            return (int)(v * 100.0m) + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal v = 0m;

            decimal.TryParse(value.ToString().TrimEnd(new char[] { '%' }), out v);

            return (v / 100.0m);
        }
    }
}
