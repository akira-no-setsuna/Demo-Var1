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

            _isNew = (tovar == null);
            _tovar = tovar ?? new Tovar();

            if (_isNew)
            {
                Title = "DobavlenieTovara";
                _tovar.AtributiTovara = new AtributiTovara();
            }

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
            }

            this.DataContext = _tovar;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
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
            string oldImage = null;

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

                        oldImage = t.Image;   // запоминаем ДО перезаписи

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Удаляем старое фото (если поменялось и это не заглушка)
            if (!string.IsNullOrEmpty(oldImage)
    && !string.Equals(oldImage, "picture.png", StringComparison.OrdinalIgnoreCase)
    && !string.Equals(oldImage, _tovar.Image, StringComparison.OrdinalIgnoreCase))
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources",
                    Path.GetFileName(oldImage));

                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch (Exception ex)
                {
                    // ВРЕМЕННО: показываем ошибку
                    MessageBox.Show("Не удалось удалить файл: \n" + ex.Message, "Ошибка удаления");
                }
            }

            MessageBox.Show("Товар успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Выберите изображение товара",
                Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                // 1. Загружаем в память (OnLoad не блокирует исходный файл)
                var original = new BitmapImage();
                original.BeginInit();
                original.CacheOption = BitmapCacheOption.OnLoad;
                original.UriSource = new Uri(dialog.FileName);
                original.EndInit();
                original.Freeze();

                // 2. Уменьшаем пропорционально
                double scale = Math.Min(300.0 / original.PixelWidth, 200.0 / original.PixelHeight);
                if (scale > 1) scale = 1;
                var resized = new TransformedBitmap(original, new ScaleTransform(scale, scale));
                resized.Freeze();

                // 3. Всегда генерируем новое уникальное имя
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                Directory.CreateDirectory(folder);

                string newFileName = Guid.NewGuid().ToString("N").Substring(0, 6) + ".jpg";
                string fullPath = Path.Combine(folder, newFileName);

                // 4. Сохраняем новый файл
                var encoder = new JpegBitmapEncoder { QualityLevel = 90 };
                encoder.Frames.Add(BitmapFrame.Create(resized));

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                // 5. Обновляем данные и показываем превью
                _tovar.Image = newFileName;
                TovarPhoto.Source = resized;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки изображения: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}