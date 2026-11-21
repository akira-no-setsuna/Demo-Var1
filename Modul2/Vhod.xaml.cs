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
    /// Логика взаимодействия для Vhod.xaml
    /// </summary>
    public partial class Vhod : Window
    {
        public Vhod()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login_TextBox.Text.Trim();
            string password = PasswordBox.Password;

            using (var db = new user04Entities())
            { 
                var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    var win = new Glavnaya(user.FullName, (int)user.UserRole.RoleID);
                    win.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void QuestSignInButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new Glavnaya("Гость", 0);
            win.Show();
            this.Close();
        }
    }
}
