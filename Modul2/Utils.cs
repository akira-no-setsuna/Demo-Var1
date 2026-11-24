using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Modul2
{
    public static class Utils
    {
        public static BitmapImage FoundImage(this string name)
        {
            string fullPath = System.IO.Path.GetFullPath($@"Resources\{name}");

            try
            {
                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }
            catch (Exception)
            {
                fullPath = System.IO.Path.GetFullPath($@"Resources\picture.png");
                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }

        }
    }

}
