using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ProductManagerApp;

namespace CategoryManagerApp
{
    public partial class OrderControl : UserControl
    {
        private OrderRepository orderRepository = new OrderRepository();
        private DataTable allOrders;
        private PaginationHelper paginator;
        private int pageSize = 10;

        public OrderControl()
        {
            InitializeComponent();
            LoadAllOrders();
        }

        private void LoadAllOrders()
        {
            try
            {
                allOrders = orderRepository.GetAllOrders();
                paginator = new PaginationHelper(allOrders.Rows.Count, pageSize);
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取訂單資料失敗：" + ex.Message);
            }
        }

        private void LoadCurrentPage()
        {
            int start = paginator.GetStartIndex();
            int size = paginator.GetPageSize();

            var pagedRows = allOrders.AsEnumerable()
                .Skip(start)
                .Take(size);

            if (pagedRows.Any())
            {
                dgvOrders.DataSource = pagedRows.CopyToDataTable();
                dgvOrders.Columns["OrderID"].HeaderText = "訂單編號";
                dgvOrders.Columns["OrderTime"].HeaderText = "下訂時間";
                dgvOrders.Columns["BuyerName"].HeaderText = "買家名稱";
                dgvOrders.Columns["PaymentMethod"].HeaderText = "付款方式";
                dgvOrders.Columns["TotalAmount"].HeaderText = "總金額";
            }
            else
            {
                dgvOrders.DataSource = null;
            }

            lblPageInfo.Text = $"第 {paginator.CurrentPage} 頁 / 共 {paginator.TotalPages} 頁";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new OrderAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAllOrders();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllOrders();
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (paginator == null) return;
            paginator.PreviousPage();
            LoadCurrentPage();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (paginator == null) return;
            paginator.NextPage();
            LoadCurrentPage();
        }
    }
}
