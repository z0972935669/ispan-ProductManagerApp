using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CategoryManagerApp;
using ProductManagerApp;

namespace ProductManagerApp
{
    public partial class ProductControl : UserControl
    {
        private ProductRepository productRepository = new ProductRepository();
        private DataTable allProducts;
        private PaginationHelper paginator;
        private int pageSize = 10;

        public ProductControl()
        {
            InitializeComponent();
            LoadAllProducts();
        }

        // 載入全部商品 → 建立分頁物件
        private void LoadAllProducts()
        {
            try
            {
                allProducts = productRepository.GetAllProducts();
                paginator = new PaginationHelper(allProducts.Rows.Count, pageSize);
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取商品資料失敗：" + ex.Message);
            }
        }

        // 顯示目前頁資料
        private void LoadCurrentPage()
        {
            int start = paginator.GetStartIndex();
            int size = paginator.GetPageSize();

            var pagedRows = allProducts.AsEnumerable()
                .Skip(start)
                .Take(size);

            if (pagedRows.Any())
            {
                dgvProducts.DataSource = pagedRows.CopyToDataTable();
            }
            else
            {
                dgvProducts.DataSource = null;
            }

            dgvProducts.Columns["ProductID"].HeaderText = "商品編號";
            dgvProducts.Columns["ProductName"].HeaderText = "商品名稱";
            dgvProducts.Columns["OriginalPrice"].HeaderText = "原價";
            dgvProducts.Columns["SalePrice"].HeaderText = "售價";
            dgvProducts.Columns["Quantity"].HeaderText = "數量";
            dgvProducts.Columns["Stock"].HeaderText = "庫存";
            dgvProducts.Columns["Discontinued"].HeaderText = "下架";

            lblPageInfo.Text = $"第 {paginator.CurrentPage} 頁 / 共 {paginator.TotalPages} 頁";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAllProducts(); // 重新載入全部資料
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("請先選擇要編輯的商品！");
                return;
            }

            DataRowView rowView = dgvProducts.SelectedRows[0].DataBoundItem as DataRowView;
            if (rowView != null)
            {
                using (var form = new ProductAddForm(rowView.Row))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAllProducts(); // 重新整理
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選擇要刪除的商品！");
                return;
            }

            DataRowView rowView = dgvProducts.SelectedRows[0].DataBoundItem as DataRowView;
            int productId = Convert.ToInt32(rowView["ProductID"]);
            string name = rowView["ProductName"].ToString();

            var confirm = MessageBox.Show($"確定要刪除「{name}」？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                bool success = productRepository.DeleteProduct(productId);
                if (success)
                {
                    MessageBox.Show("商品刪除成功！");
                    LoadAllProducts();
                }
                else
                {
                    MessageBox.Show("刪除失敗！");
                }
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            paginator.PreviousPage();
            LoadCurrentPage();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            paginator.NextPage();
            LoadCurrentPage();
        }
    }
}
