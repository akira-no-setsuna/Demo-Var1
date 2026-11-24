using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Modul2
{
    public static class Utils
    {
        public static ImageSource FoundImage(this string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return null;

            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imageName);

                if (!System.IO.File.Exists(path))
                    path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "picture.png");

                if (!System.IO.File.Exists(path)) return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();

                bitmap.Freeze();

                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }

}
