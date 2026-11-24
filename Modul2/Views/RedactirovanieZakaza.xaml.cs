using Modul2;
using Modul2.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Modul2.Views
{
    public partial class RedactirovanieZakaza : Window
    {
        private user04Entities _db = new user04Entities(); // Ваш контекст
        private ZakazDto _editingOrder;
        private int _orderId = 0;

        public RedactirovanieZakaza(ZakazDto order)
        {
            InitializeComponent();
            _editingOrder = order;

            // Загружаем адреса в ComboBox
            cbAddress.ItemsSource = _db.PunktVidachi.ToList();

            // Загружаем статусы
            cbStatus.ItemsSource = new[] { "Новый", "Выдан" };

            if (_editingOrder != null)
            {
                // Режим редактирования
                Title = "Редактирование заказа";
                _orderId = _editingOrder.ID;
                txtCode.Text = _editingOrder.Code?.ToString();
                cbStatus.SelectedItem = _editingOrder.IsIssued == true ? "Выдан" : "Новый";
                cbAddress.SelectedValue = _editingOrder.AddressID;
                dpCreateDate.SelectedDate = _editingOrder.CreateDate;
                dpDeliveryDate.SelectedDate = _editingOrder.DeliveryDate;
            }
            else
            {
                // Режим добавления
                Title = "Добавление заказа";
                cbStatus.SelectedIndex = 0; // Новый
                dpCreateDate.SelectedDate = DateTime.Now;
                dpDeliveryDate.SelectedDate = DateTime.Now.AddDays(7);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtCode.Text) || cbAddress.SelectedItem == null ||
                dpCreateDate.SelectedDate == null || dpDeliveryDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int code = int.Parse(txtCode.Text); // Может вызвать ошибку, если ввели буквы
                int addressId = (int)cbAddress.SelectedValue;
                bool isIssued = cbStatus.SelectedItem.ToString() == "Выдан";

                if (_editingOrder != null)
                {
                    // РЕДАКТИРОВАНИЕ
                    var zakaz = _db.Zakaz.Find(_orderId);
                    if (zakaz != null)
                    {
                        zakaz.Code = code;
                        zakaz.CreateDate = dpCreateDate.SelectedDate;
                        zakaz.DeliveryDate = dpDeliveryDate.SelectedDate;
                    }

                    var attr = _db.ZakazAtributes.Find(_orderId);
                    if (attr != null)
                    {
                        attr.AdressID = addressId;
                        attr.Staus = isIssued;
                    }
                }
                else
                {
                    // ДОБАВЛЕНИЕ
                    // Генерируем ID вручную (так как в вашей БД нет IDENTITY)
                    // ДОБАВЛЕНИЕ
                    int newId = _db.Zakaz.Any() ? _db.Zakaz.Max(z => z.ID) + 1 : 1;

                    var newZakaz = new Zakaz
                    {
                        ID = newId,
                        Code = code,
                        CreateDate = dpCreateDate.SelectedDate,
                        DeliveryDate = dpDeliveryDate.SelectedDate
                    };

                    var newAttr = new ZakazAtributes
                    {
                        OrderID = newId,
                        AdressID = addressId,
                        Staus = isIssued,
                        Login = "5d4zbu@tutanota.com" // Замените на реальный логин
                    };

                    // 1. Сначала сохраняем основной заказ
                    _db.Zakaz.Add(newZakaz);
                    _db.SaveChanges(); // Фиксируем заказ в БД

                    // 2. Затем отдельно сохраняем атрибуты
                    _db.ZakazAtributes.Add(newAttr);
                    _db.SaveChanges(); // Фиксируем атрибуты

                    // Для РЕДАКТИРОВАНИЯ (когда _editingOrder != null) проблема не возникает, 
                    // но можно также сохранять по очереди:
                    // _db.SaveChanges() после изменения zakaz, затем после изменения attr.
                }

                _db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}