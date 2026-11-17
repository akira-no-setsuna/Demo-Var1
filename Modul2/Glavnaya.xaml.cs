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
                CardsContainer.Items.Clear();
                var products = db.Tovar.ToList();
                var productAttributes = db.AtributiTovara.ToList();
                var categoies = db.Categoria.ToList();

                for (int i = 0; i < products.Count; i++)
                {
                    var pA = productAttributes[i].Categoria;

                    var card = new KartochkaTovara(products[i].Name, pA.Name);



                    CardsContainer.Items.Add(card);
                }
                foreach (var p in products)
                {

                    
                }
            }
        }

       
    }
}
