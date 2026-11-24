using Modul2.DTOs;
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

namespace Modul2.Views
{
    /// <summary>
    /// Логика взаимодействия для Zakazy.xaml
    /// </summary>
    public partial class Zakazy : Window
    {
        // Замените Entities на название вашего контекста БД
        private user04Entities _db = new user04Entities();

        public Zakazy()
        {
            InitializeComponent();
            LoadData();
            CheckRole();
        }

        private void CheckRole()
        {
            // ЗАМЕНИТЕ ЭТО НА ВАШУ ПРОВЕРКУ РОЛИ
            // Например, если вы храните текущего юзера в UserSession.RoleName
            string currentRole = "Администратор"; // Заглушка для теста. Должно быть: UserSession.RoleName;

            if (currentRole != "Администратор")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            var orders = _db.Zakaz.Select(z => new ZakazDto
            {
                ID = z.ID,
                Code = z.Code,
                CreateDate = z.CreateDate,
                DeliveryDate = z.DeliveryDate,
                // Связь через таблицу атрибутов
                StatusText = z.ZakazAtributes.Staus == true ? "Выдан" : "Новый",
                AddressName = z.ZakazAtributes.PunktVidachi.Name,
                IsIssued = z.ZakazAtributes.Staus,
                AddressID = z.ZakazAtributes.AdressID
            }).ToList();

            dgZakazy.ItemsSource = orders;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new RedactirovanieZakaza(null); // null = добавление
            if (editWindow.ShowDialog() == true)
            {
                LoadData(); // Обновляем таблицу после добавления
            }
        }

        private void dgZakazy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentRole = "Администратор"; // Заглушка. UserSession.RoleName;
            if (currentRole != "Администратор") return; // Менеджер не может редактировать

            if (dgZakazy.SelectedItem is ZakazDto selectedOrder)
            {
                var editWindow = new RedactirovanieZakaza(selectedOrder);
                if (editWindow.ShowDialog() == true)
                {
                    LoadData(); // Обновляем таблицу после редактирования
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgZakazy.SelectedItem is ZakazDto selectedOrder)
            {
                var result = MessageBox.Show($"Удалить заказ {selectedOrder.Code}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var order = _db.Zakaz.Find(selectedOrder.ID);
                    if (order != null)
                    {
                        // Удаляем сначала атрибуты, потом сам заказ
                        var attr = _db.ZakazAtributes.Find(selectedOrder.ID);
                        if (attr != null) _db.ZakazAtributes.Remove(attr);

                        _db.Zakaz.Remove(order);
                        _db.SaveChanges();
                        LoadData();
                    }
                }
            }
        }
    }
}
