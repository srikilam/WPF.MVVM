using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfData.Models
{
    public static class Products
    {
        public static ObservableCollection<Product> GetProducts()
        {
            List<Product> result = new List<Product>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ToString()))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("select * from products", conn))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Product prod = new Product();
                            prod.Id = Convert.ToInt32(rdr["productid"]);
                            prod.ProductName = rdr["productname"].ToString();
                            prod.UnitsInStock = Convert.ToInt32(rdr["unitsinstock"]);
                            prod.Discontinued = Convert.ToBoolean(rdr["discontinued"]);
                            prod.Mode = emMode.none;
                            result.Add(prod);
                        }
                    }
                }
                conn.Close();
            }

            var oc = new ObservableCollection<Product>();
            result.ForEach(x => oc.Add(x));
            return oc;
        }

        internal static void Flush(ObservableCollection<Product> products)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ToString()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    //ObservableCollection<Product> list = products.Distinct(x => x.Mode != emMode.none);
                    foreach (Product product in products)
                    {
                        if (product.Mode == emMode.none)
                            continue;
                        else
                            if (product.Mode == emMode.add)
                                Insert(product, conn, tran);
                            else
                                if (product.Mode == emMode.update)
                                    Update(product, conn, tran);
                                else if (product.Mode == emMode.delete)
                                    Delete(product.Id, conn, tran);
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
                conn.Close();
            }
        }

        private static void Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("delete from products where productid = @id", conn, tran))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private static void Update(Product product, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("update products set productname = @name, unitsinstock = @instock where productid = @id", conn, tran))
            {
                cmd.Parameters.AddWithValue("@id", product.Id);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@instock", product.UnitsInStock);
                cmd.ExecuteNonQuery();
            }
        }

        private static void Insert(Product product, SqlConnection conn, SqlTransaction tran)
        {
            using (SqlCommand cmd = new SqlCommand("insert into products(productname, unitsinstock) values (@name, @instock)", conn, tran))
            {
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@instock", product.UnitsInStock);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Product : ModelBase<Product>
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
        public emMode Mode { get; set; }
    }
    public enum emMode { add, update, delete, none };
}
