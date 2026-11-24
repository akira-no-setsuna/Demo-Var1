
using Modul2.DTOs;
using System;
using System.IO;
using System.Linq;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Modul2.Views
{
    /// <summary>
    /// Логика взаимодействия для RedactirovanieTovara.xaml
    /// </summary>
    public partial class RedactirovanieTovara : Window
    {
        private Tovar _tovar;
        private bool _isNew;
        public RedactirovanieTovara(Tovar tovar)
        {
            InitializeComponent();

            _isNew = (tovar ==  null);
            _tovar = tovar;

            LoadComboBoxes();
            FillFields();

        }

        private void LoadComboBoxes()
        {
            using (var db = new user04Entities())
            {
                CbCategory.ItemsSource = db.Categoria.ToList();
                CbManufacturer.ItemsSource = db.Proizvoditel.ToList();
                CbSupplier.ItemsSource = db.Postavshik.ToList();
            }
        }

        private void FillFields()
        {
            if (!_isNew)
            {
                TbId.IsReadOnly = true;
                TbId.Background = Brushes.Gray;

                TovarPhoto.Source = _tovar.Image.FoundImage();

                this.DataContext = _tovar;

            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_tovar == null) return;

            // 1. Валидация
            if (string.IsNullOrWhiteSpace(_tovar.Name))
            {
                MessageBox.Show("Введите наименование товара!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_tovar.Cost == null || _tovar.Cost < 0)
            {
                MessageBox.Show("Цена не может быть пустой или отрицательной!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_tovar.Quantity == null || _tovar.Quantity < 0)
            {
                MessageBox.Show("Количество не может быть пустым или отрицательным!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_tovar.Sale < 0 || _tovar.Sale > 100)
            {
                MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100%!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var a = _tovar.AtributiTovara;
            if (a == null || a.CategoryID <= 0 || a.ManufacturerID <= 0 || a.SuplierID <= 0)
            {
                MessageBox.Show("Пожалуйста, выберите категорию, производителя и поставщика!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Сохранение
            try
            {
                using (var db = new user04Entities())
                {
                    if (string.IsNullOrEmpty(_tovar.Image))
                        _tovar.Image = "picture.png";

                    if (_isNew)
                    {
                        db.Tovar.Add(_tovar);
                    }
                    else
                    {
                        var t = db.Tovar.Find(_tovar.ID);
                        if (t == null)
                        {
                            MessageBox.Show("Товар не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        t.Name = _tovar.Name;
                        t.Descriprion = _tovar.Descriprion;
                        t.Cost = _tovar.Cost;
                        t.Quantity = _tovar.Quantity;
                        t.Sale = _tovar.Sale;
                        t.Image = _tovar.Image;

                        if (t.AtributiTovara == null)
                            t.AtributiTovara = new AtributiTovara();

                        t.AtributiTovara.CategoryID = a.CategoryID;
                        t.AtributiTovara.ManufacturerID = a.ManufacturerID;
                        t.AtributiTovara.SuplierID = a.SuplierID;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Товар успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
