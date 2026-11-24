using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modul2.DTOs
{
    public class ZakazDto
    {
        public int ID { get; set; }
        public int? Code { get; set; } // Артикул заказа
        public string StatusText { get; set; } // "Новый" или "Выдан"
        public string AddressName { get; set; } // Адрес пункта выдачи
        public DateTime? CreateDate { get; set; } // Дата заказа
        public DateTime? DeliveryDate { get; set; } // Дата выдачи
        public bool? IsIssued { get; set; } // Сам статус (bit)
        public int? AddressID { get; set; } // ID адреса для формы
    }
}
