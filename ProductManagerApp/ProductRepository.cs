using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CategoryManagerApp
{
    //資料存取封裝
    public class ProductRepository
    {
        private readonly string connectionString = "Data Source=localhost;Initial Catalog=ShopDB;Integrated Security=True;";

        public DataTable GetAllProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ProductID,
                        ProductName,
                        OriginalPrice,
                        SalePrice,
                        Quantity,
                        Stock,
                        Discontinued,
                        CategoryID
                    FROM Products";

                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public bool InsertProduct(string name, decimal originalPrice, decimal salePrice, int quantity, int stock, bool discontinued, long? categoryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            INSERT INTO Products 
            (ProductName, CreateAt, OriginalPrice, SalePrice, Quantity, Stock, Discontinued, CategoryID)
            VALUES 
            (@ProductName, GETDATE(), @OriginalPrice, @SalePrice, @Quantity, @Stock, @Discontinued, @CategoryID)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductName", name);
                cmd.Parameters.AddWithValue("@OriginalPrice", originalPrice);
                cmd.Parameters.AddWithValue("@SalePrice", salePrice);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Stock", stock);
                cmd.Parameters.AddWithValue("@Discontinued", discontinued);
                cmd.Parameters.AddWithValue("@CategoryID", (object)categoryId ?? DBNull.Value);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool UpdateProduct(int id, string name, decimal originalPrice, decimal salePrice, int quantity, int stock, bool discontinued, long? categoryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            UPDATE Products SET 
                ProductName = @ProductName,
                OriginalPrice = @OriginalPrice,
                SalePrice = @SalePrice,
                Quantity = @Quantity,
                Stock = @Stock,
                Discontinued = @Discontinued,
                CategoryID = @CategoryID
            WHERE ProductID = @ProductID";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductID", id);
                cmd.Parameters.AddWithValue("@ProductName", name);
                cmd.Parameters.AddWithValue("@OriginalPrice", originalPrice);
                cmd.Parameters.AddWithValue("@SalePrice", salePrice);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Stock", stock);
                cmd.Parameters.AddWithValue("@Discontinued", discontinued);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 用 Transaction 確保兩個刪除一起成功或一起失敗
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1️⃣ 先刪除 ProductPhoto 中的資料
                    string deletePhotosSql = "DELETE FROM ProductPhoto WHERE ProductID = @ProductID";
                    SqlCommand cmdDeletePhotos = new SqlCommand(deletePhotosSql, conn, transaction);
                    cmdDeletePhotos.Parameters.AddWithValue("@ProductID", productId);
                    cmdDeletePhotos.ExecuteNonQuery();

                    // 2️⃣ 再刪除 Products 中的資料
                    string deleteProductSql = "DELETE FROM Products WHERE ProductID = @ProductID";
                    SqlCommand cmdDeleteProduct = new SqlCommand(deleteProductSql, conn, transaction);
                    cmdDeleteProduct.Parameters.AddWithValue("@ProductID", productId);
                    int affected = cmdDeleteProduct.ExecuteNonQuery();

                    transaction.Commit();
                    return affected > 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("刪除失敗：" + ex.Message);
                    return false;
                }
            }
        }


    }
}
