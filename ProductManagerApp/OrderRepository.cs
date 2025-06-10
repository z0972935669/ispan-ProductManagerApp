using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagerApp
{
    public class OrderRepository
    {
        private readonly string connectionString = "Data Source=localhost;Initial Catalog=ShopDB;Integrated Security=True;";

        public DataTable GetAllOrders()
        {
            string sql = @"SELECT OrderID, OrderTime, BuyerName, TotalAmount, PaymentMethod FROM Orders ORDER BY OrderTime DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public bool InsertOrder(DateTime orderTime, string buyerName, string paymentMethod, decimal totalAmount)
        {
            string sql = @"INSERT INTO Orders (OrderTime, BuyerName, PaymentMethod, TotalAmount)
                       VALUES (@OrderTime, @BuyerName, @PaymentMethod, @TotalAmount)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderTime", orderTime);
                cmd.Parameters.AddWithValue("@BuyerName", buyerName);
                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
