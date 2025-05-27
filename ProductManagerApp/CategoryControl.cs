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

namespace ProductManagerApp
{
    public partial class CategoryControl : UserControl
    {
        private CategoryRepository categoryRepository = new CategoryRepository();
        private DataTable allCategories;
        private PaginationHelper paginator;
        private int pageSize = 10;

        public CategoryControl()
        {
            InitializeComponent();
            LoadAllCategories();
        }

        // 載入分類資料並初始化分頁
        private void LoadAllCategories()
        {
            try
            {
                allCategories = categoryRepository.GetAllCategories();
                paginator = new PaginationHelper(allCategories.Rows.Count, pageSize);
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("讀取分類資料失敗：" + ex.Message);
            }
        }

        // 顯示目前頁資料
        private void LoadCurrentPage()
        {
            int start = paginator.GetStartIndex();
            int size = paginator.GetPageSize();

            var pagedRows = allCategories.AsEnumerable()
                .Skip(start)
                .Take(size);

            if (pagedRows.Any())
            {
                dgvCategories.DataSource = pagedRows.CopyToDataTable();
            }
            else
            {
                dgvCategories.DataSource = null;
            }

            dgvCategories.Columns["CategoryID"].HeaderText = "分類編號";
            dgvCategories.Columns["CategoryName"].HeaderText = "分類名稱";
            dgvCategories.Columns["Description"].HeaderText = "說明";

            lblPageInfo.Text = $"第 {paginator.CurrentPage} 頁 / 共 {paginator.TotalPages} 頁";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CategoryAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAllCategories();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選擇要編輯的分類！");
                return;
            }

            DataRowView rowView = dgvCategories.SelectedRows[0].DataBoundItem as DataRowView;
            if (rowView != null)
            {
                using (var form = new CategoryAddForm(rowView.Row))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAllCategories();
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選擇要刪除的分類！");
                return;
            }

            DataRowView rowView = dgvCategories.SelectedRows[0].DataBoundItem as DataRowView;
            long categoryId = Convert.ToInt64(rowView["CategoryID"]);
            string name = rowView["CategoryName"].ToString();

            var confirm = MessageBox.Show($"確定要刪除「{name}」？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                bool success = categoryRepository.DeleteCategory(categoryId);
                if (success)
                {
                    MessageBox.Show("分類刪除成功！");
                    LoadAllCategories();
                }
                else
                {
                    MessageBox.Show("刪除失敗，可能已與商品關聯！");
                }
            }
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
