﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ProductManagerApp;

namespace CategoryManagerApp
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=ShopDB;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            panelForm.Controls.Clear(); // 清除舊內容
            ProductControl productControl = new ProductControl();
            productControl.Dock = DockStyle.Fill;
            panelForm.Controls.Add(productControl); // 加入商品管理控制項
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            panelForm.Controls.Clear(); // 清除舊內容
            CategoryControl categoryControl = new CategoryControl();
            categoryControl.Dock = DockStyle.Fill;
            panelForm.Controls.Add(categoryControl); // 加入類別管理控制項
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            panelForm.Controls.Clear(); // 清除舊內容
            OrderControl orderControl = new OrderControl();
            orderControl.Dock = DockStyle.Fill;
            panelForm.Controls.Add(orderControl); // 加入訂單管理制項
        }
    }
}
