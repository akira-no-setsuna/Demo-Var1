using System;
using System.Data.Entity; // Обязательно для метода .Include()
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Modul2
{
    public partial class Glavnaya : Window
    {
        public Glavnaya(
            string user, int role)
        {
            InitializeComponent();

            UserFullName.Content = user;

            StaffPanel.Visibility = (role == (int)Role.Admin || role == (int)Role.Manager) ? Visibility.Visible : Visibility.Hidden;


            LoadProducts();
        }

        enum Role
        {
        Admin = 1,
        Manager = 2,
        Client = 3,
        Quest = 4,
        }

        private void LoadProducts()
        {
            try
            {
                using (var db = new user04Entities())
                {
                    var productsView = db.Tovar
                        .Select(t => new
                        {
                            PhotoPath = "resourсes\\" + t.Image ?? "1.JPG",
                            CategoryAndName = t.Name + " | " + (t.AtributiTovara.Categoria.Name ?? "Не указана"),
                            Description = t.Descriprion,
                            Manufacturer = t.AtributiTovara.Proizvoditel.Name ?? "Не указан",
                            Supplier = t.AtributiTovara.Postavshik.Name ?? "Не указана",
                            Cost = t.Cost,
                            Quantity = t.Quantity,
                            Sale = t.Sale,

                            FinalCost = t.Cost - (t.Cost * t.Sale / 100m),
                            HasSale = t.Sale > 0 ? Visibility.Visible : Visibility.Collapsed,
                            RowBackground = t.Quantity == 0 ? "#808080" : (t.Sale > 17 ? "#FFDEAD" : "Transparent")
                        });




                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            productsView = productsView.OrderBy(p => p.Quantity);
                            break;
                        case 2:
                            productsView = productsView.OrderByDescending(p => p.Quantity);
                            break;
                        default:
                            productsView = productsView.OrderBy(p => p.CategoryAndName);
                            break;
                    }

                    CardsContainer.ItemsSource = productsView.ToList();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка показа товаров!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new Vhod();
            win.Show();
            this.Close();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CardsContainer == null) return;
            LoadProducts();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
