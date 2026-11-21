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
        public Glavnaya()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
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
                        SaleBackground = t.Sale > 17 ? "#FFDEAD" : "Transparent"
                    }).ToList();

                CardsContainer.Items.Clear();


                CardsContainer.ItemsSource = productsView;
            }
        }

       
    }
}
