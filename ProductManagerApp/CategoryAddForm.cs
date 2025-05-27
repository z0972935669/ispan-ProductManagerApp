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
    public partial class CategoryAddForm : Form
    {
        //新增 / 編輯視窗
        private readonly CategoryRepository repository = new CategoryRepository();
        private long? categoryId = null;

        public CategoryAddForm()
        {
            InitializeComponent();
            this.Text = "新增類別";
        }

        // 編輯模式用的建構子
        public CategoryAddForm(DataRow row)
        {
            InitializeComponent();
            this.Text = "編輯類別";
            categoryId = Convert.ToInt64(row["CategoryID"]);
            txtName.Text = row["CategoryName"].ToString();
            txtDescription.Text = row["Description"].ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 驗證
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("請輸入類別名稱");
                txtName.Focus();
                return;
            }

            bool result;

            if (categoryId == null)
            {
                result = repository.InsertCategory(name, description);
            }
            else
            {
                result = repository.UpdateCategory(categoryId.Value, name, description);
            }

            if (result)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("儲存失敗！");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
