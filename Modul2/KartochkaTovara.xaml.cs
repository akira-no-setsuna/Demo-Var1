using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Modul2
{
    /// <summary>
    /// Логика взаимодействия для KartochkaTovara.xaml
    /// </summary>
    public partial class KartochkaTovara : UserControl
    {
        public KartochkaTovara()
        {
            InitializeComponent();
        }

        private void Tovar_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var img = (Image)sender;
            if (img.Tag as string == "placeholder") return;
            img.Tag = "placeholder";
            img.Source = new BitmapImage(new Uri("resourсes/picture.png", UriKind.Relative));
        }
    }
}
