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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfData.Services;
using WpfData.ViewModels;

namespace WpfData.Views
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        private ProductsViewModel _vm;

        public ProductsView()
        {
            InitializeComponent();
            IServiceAgent sa = new ServiceAgent();
            _vm = new ProductsViewModel(sa);
            this.DataContext = _vm;

            _vm.LoadProducts();
        }
    }
}
