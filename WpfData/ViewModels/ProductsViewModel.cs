using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using SimpleMvvmToolkit;
using WpfData.Services;
using WpfData.Models;

namespace WpfData.ViewModels
{
    public class ProductsViewModel : ViewModelBase<ProductsViewModel>
    {
        private IServiceAgent _serviceAgent;

        public ProductsViewModel() { }

        public ProductsViewModel(IServiceAgent serviceAgent)
        {
            this._serviceAgent = serviceAgent;
        }

        #region Notifications

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        private string productName;
        public string ProductName
        {
            get { return productName; }
            set
            {
                productName = value;
                NotifyPropertyChanged(m => m.ProductName);
            }
        }

        private int units;
        public int Units
        {
            get { return units; }
            set
            {
                units = value;
                NotifyPropertyChanged(m => m.Units);
            }
        }

        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                NotifyPropertyChanged(m => m.Products);
            }
        }

        private Product selectedProduct;
        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                this.Message = selectedProduct.ProductName;
                NotifyPropertyChanged(m => m.SelectedProduct);
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyPropertyChanged(m => m.message);
            }
        }

        #endregion

        #region Methods

        public void LoadProducts()
        {
            _serviceAgent.GetProducts((products, error) => ProductsLoaded(products, error));
        }
        private void AddProduct()
        {
            this.Products.Insert(0, new Product { ProductName = "TestProduct", UnitsInStock = 10, Discontinued = false });
            _serviceAgent.Flush(this.Products, (error) => ProductsFlushed(error));
        }

        private void EditProduct()
        {
            this.SelectedProduct.ProductName = "Edited Name";
            this.SelectedProduct.UnitsInStock = 10;
            this.SelectedProduct.Mode = emMode.update;
            _serviceAgent.Flush(this.Products, (error) => ProductsFlushed(error));
        }

        private void DeleteProduct()
        {
            this.SelectedProduct.Mode = emMode.delete;
            _serviceAgent.Flush(this.Products, (error) => ProductsFlushed(error));
        }

        private void Search()
        {
            _serviceAgent.GetProducts((productlist, error) => ProductsLoaded(productlist, error));
        }

        #endregion

        #region Callbacks

        private void ProductsLoaded(ObservableCollection<Product> products, Exception error)
        {
            if (error == null)
            {
                this.Products = products;
                NotifyError("Loaded", null);
            }
            else
            {
                NotifyError(error.Message, error);
            }
            // isbusy = false;
        }

        private void ProductsFlushed(Exception error)
        {
            if (error == null)
                NotifyError("Flushed", null);
            else
                NotifyError(error.Message, error);
        }

        #endregion

        #region Commands

        private DelegateCommand addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new DelegateCommand(AddProduct);
                return addCommand;
            }
            private set
            {
                addCommand = value;
            }
        }

        private DelegateCommand editCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                if (editCommand == null)
                    editCommand = new DelegateCommand(EditProduct);
                return editCommand;
            }
            private set
            {
                editCommand = value;
            }
        }

        private DelegateCommand deleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new DelegateCommand(DeleteProduct);
                return deleteCommand;
            }
            private set
            {
                deleteCommand = value;
            }
        }

        private DelegateCommand searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (searchCommand == null) searchCommand = new DelegateCommand(Search);
                return searchCommand;
            }
            private set
            {
                searchCommand = value;
            }
        }

        #endregion 

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            this.Message = message;
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}