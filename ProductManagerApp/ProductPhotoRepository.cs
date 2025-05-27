using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ProductManagerApp
{
    public class ProductPhotoRepository
    {
        private readonly string connectionString = "Data Source=localhost;Initial Catalog=ShopDB;Integrated Security=True;";

        public bool InsertPhoto(int productId, byte[] thumbPhoto, string thumbName, byte[] largePhoto, string largeName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 刪除舊圖片（1商品只儲存1圖）
                    string deleteSql = "DELETE FROM ProductPhoto WHERE ProductID = @ProductID";
                    SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
                    deleteCmd.Parameters.AddWithValue("@ProductID", productId);
                    deleteCmd.ExecuteNonQuery();

                    // 插入新圖片
                    string insertSql = @"
                        INSERT INTO ProductPhoto 
                        (ProductID, ThumbnailPhoto, ThumbnailFileName, LargePhoto, LargePhotoFileName, ModifiedDate)
                        VALUES 
                        (@ProductID, @ThumbPhoto, @ThumbName, @LargePhoto, @LargeName, GETDATE())";

                    SqlCommand insertCmd = new SqlCommand(insertSql, conn);
                    insertCmd.Parameters.AddWithValue("@ProductID", productId);
                    insertCmd.Parameters.AddWithValue("@ThumbPhoto", (object)thumbPhoto ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@ThumbName", (object)thumbName ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@LargePhoto", (object)largePhoto ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@LargeName", (object)largeName ?? DBNull.Value);

                    int rows = insertCmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                // 你可以改用 Log 寫入或顯示錯誤視窗
                Console.WriteLine("圖片儲存失敗：" + ex.Message);
                return false;
            }
        }

        // 取得大圖
        public byte[] GetPhotoByProductId(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT LargePhoto FROM ProductPhoto WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? (byte[])result : null;
            }
        }

        // ✅ 建議新增：取得縮圖
        public byte[] GetThumbnailByProductId(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ThumbnailPhoto FROM ProductPhoto WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? (byte[])result : null;
            }
        }
    }
}
