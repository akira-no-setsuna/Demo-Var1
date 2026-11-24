using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Modul2.DTOs
{

    // TODO: Не эффективно, нужно переходить на .Include()
    public class TovarDto
    {
        public string ID { get; set; }
        public BitmapImage PhotoPath { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }

        public string CategoryAndName { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
        public int Sale { get; set; }
        public int FinalCost { get; set; }
        public Visibility HasSale { get; set; }
        public string RowBackground { get; set; }


        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public int SupplierId { get; set; }
    }
}
