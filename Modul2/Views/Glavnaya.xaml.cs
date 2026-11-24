using Modul2.DTOs;
using Modul2.Views;
using System;
using System.CodeDom;
using System.Data.Entity; // Обязательно для метода .Include()
using System.IO;
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

            // StaffPanel.Visibility = (role == (int)Role.Admin || role == (int)Role.Manager) ? Visibility.Visible : Visibility.Collapsed;
            // AdminPanel.Visibility = (role == (int)Role.Admin) ? Visibility.Visible : Visibility.Collapsed;

            LoadSupliers();
            LoadProducts();
        }

        private void LoadSupliers()
        {
            using (var db = new user04Entities())
            {
                var supliers = db.Postavshik;

                foreach (var suplier in supliers)
                    FiltrationComboBox.Items.Add(suplier.Name);
            }



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
                    string search = SearchTextBox.Text.ToLower();

                    var query = string.IsNullOrEmpty(search)
                        ? db.Tovar
                        : db.Tovar.Where(t =>
                            t.Name.ToLower().Contains(search) ||
                            t.Descriprion.ToLower().Contains(search) ||
                            t.AtributiTovara.Categoria.Name.ToLower().Contains(search) ||
                            t.AtributiTovara.Proizvoditel.Name.ToLower().Contains(search) ||
                            t.AtributiTovara.Postavshik.Name.ToLower().Contains(search)
                        );

                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            query = query.OrderBy(q => q.Quantity);
                            break;
                        case 2:
                            query = query.OrderByDescending(q => q.Quantity);
                            break;
                        default:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }

                    if (FiltrationComboBox.SelectedIndex != 0)
                        query = query
                            .Where(t => t.AtributiTovara.Postavshik.Name.Contains((string)FiltrationComboBox.SelectedValue));

                    var productsView = query
                        .ToList()
                        .Select(t => new TovarDto
                        {
                            ID = t.ID,
                            PhotoPath = t.Image.FoundImage(),
                            Name = t.Name,
                            Category = t.AtributiTovara.Categoria.Name,
                            CategoryAndName = t.Name + " | " + t.AtributiTovara.Categoria.Name,
                            Description = t.Descriprion,
                            Manufacturer = t.AtributiTovara.Proizvoditel.Name ?? "Не указан",
                            Supplier = t.AtributiTovara.Postavshik.Name ?? "Не указана",

                            Cost = (int)(t.Cost ?? -1),
                            Quantity = t.Quantity ?? -1,
                            Sale = t.Sale ?? -1,

                            FinalCost = (int)((t.Cost ?? 0) - ((t.Cost ?? 0) * (t.Sale ?? 0) / 100m)),
                            HasSale = (t.Sale ?? 0) > 0 ? Visibility.Visible : Visibility.Collapsed,
                            RowBackground = (t.Quantity ?? 0) == 0 ? "#808080" : ((t.Sale ?? 0) > 17 ? "#FFDEAD" : "Transparent"),

                            CategoryId = (int)t.AtributiTovara.CategoryID,
                            ManufacturerId = (int)t.AtributiTovara.ManufacturerID,
                            SupplierId = (int)t.AtributiTovara.SuplierID
                        });

                    CardsContainer.ItemsSource = productsView;

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка показа товаров!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (CardsContainer.SelectedItem is Tovar selectedTovar)
            {
                if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var db = new user04Entities())
                    {
                        // ПРОВЕРКА: есть ли товар в составе заказа (SostavZakaza)
                        bool isInOrder = db.SostavZakaza.Any(sz => sz.ProductID == selectedTovar.ID);

                        if (isInOrder)
                        {
                            MessageBox.Show("Нельзя удалить товар, который есть в заказе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                         
                        // Удаляем старое фото с диска
                        if (!string.IsNullOrEmpty(selectedTovar.Image) && selectedTovar.Image != "picture.png")
                        {
                            string oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", selectedTovar.Image);
                            if (File.Exists(oldPath)) File.Delete(oldPath);
                        }

                        // Удаляем из БД
                        db.Tovar.Remove(selectedTovar);
                        db.SaveChanges();

                        MessageBox.Show("Товар удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (CardsContainer == null) return;
            LoadProducts();
        }

        private void CardsContainer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            using (var db = new user04Entities())
            {
                if (CardsContainer.SelectedItem is TovarDto tovar)
                    new RedactirovanieTovara(db.Tovar.FirstOrDefault(t => t.ID == tovar.ID)).Show();
            }

        }

        private void CardsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
