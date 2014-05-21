using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfData.Models;

namespace WpfData.DAL
{
    public static class Core_DAL
    {
        public static ObservableCollection<Product> FetchProducts()
        {
            return Products.GetProducts();
        }

        public static void Flush(ObservableCollection<Product> products)
        {
            Products.Flush(products);
        }
    }
}
