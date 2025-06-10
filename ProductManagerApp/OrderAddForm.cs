using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManagerApp
{
    public partial class OrderAddForm : Form
    {
        public OrderAddForm()
        {
            InitializeComponent();
        }

        private void OrderAddForm_Load(object sender, EventArgs e)
        {
            cmbPaymentMethod.Items.Add("現金");
            cmbPaymentMethod.Items.Add("信用卡");
            cmbPaymentMethod.Items.Add("轉帳");
            cmbPaymentMethod.SelectedIndex = 0; // 預設選第一個

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuyerName.Text))
            {
                MessageBox.Show("請輸入買家名稱！");
                txtBuyerName.Focus();
                return;
            }

            if (!decimal.TryParse(txtTotalAmount.Text, out decimal total) || total < 0)
            {
                MessageBox.Show("總金額格式錯誤！");
                txtTotalAmount.Focus();
                return;
            }

            string buyerName = txtBuyerName.Text.Trim();
            string payment = cmbPaymentMethod.SelectedItem.ToString();
            DateTime orderTime = dtpOrderTime.Value;

            var repo = new OrderRepository();
            bool result = repo.InsertOrder(orderTime, buyerName, payment, total);

            if (result)
            {
                MessageBox.Show("訂單新增成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("新增失敗！");
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
