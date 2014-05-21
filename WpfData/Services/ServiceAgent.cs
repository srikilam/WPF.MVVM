using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfData.DAL;
using WpfData.Models;

namespace WpfData.Services
{
    public interface IServiceAgent
    {
        void GetProducts(Action<ObservableCollection<Product>, Exception> completed);
        void Flush(ObservableCollection<Product> list, Action<Exception> completed);
    }

    public class ServiceAgent : IServiceAgent
    {
        public void GetProducts(Action<ObservableCollection<Product>, Exception> completed)
        {
            try
            {
                ObservableCollection<Product> products = Core_DAL.FetchProducts();
                completed(products, null);
            }
            catch (Exception e)
            {
                completed(null, e);
            }
        }

        public void Flush(ObservableCollection<Product> list, Action<Exception> completed)
        {
            try
            {
                Core_DAL.Flush(list);
                completed(null);
            }
            catch (Exception e)
            {
                completed(e);
            }
        }
    }
}
