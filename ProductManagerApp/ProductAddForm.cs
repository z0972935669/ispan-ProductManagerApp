using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ProductManagerApp;
using System.Data.SqlClient;

namespace CategoryManagerApp
{
    public partial class ProductAddForm : Form
    {
        //新增 / 編輯視窗
        private readonly ProductRepository repository = new ProductRepository();
        private int? productId = null; // 用來識別是否是編輯
        private byte[] uploadedImage = null;
        private string uploadedFileName = null;

        public ProductAddForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        public ProductAddForm(DataRow row)
        {
            InitializeComponent();
            LoadCategories(); //載入分類清單

            this.Text = "編輯商品";
            productId = Convert.ToInt32(row["ProductID"]);
            txtName.Text = row["ProductName"].ToString();
            txtOriginalPrice.Text = row["OriginalPrice"].ToString();
            txtSalePrice.Text = row["SalePrice"].ToString();
            txtQuantity.Text = row["Quantity"].ToString();
            txtStock.Text = row["Stock"].ToString();
            chkDiscontinued.Checked = Convert.ToBoolean(row["Discontinued"]);

            if (row["CategoryID"] != DBNull.Value) 
            {
                cmbCategory.SelectedValue = Convert.ToInt64(row["CategoryID"]);
            }

            //顯示圖片
            var photoRepo = new ProductPhotoRepository();
            var bytes = photoRepo.GetPhotoByProductId(productId.Value);
            if (bytes != null)
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    picPreview.Image = Image.FromStream(ms);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("請輸入商品名稱");
                    txtName.Focus();
                    return;
                }

                if (!decimal.TryParse(txtOriginalPrice.Text, out decimal originalPrice) || originalPrice < 0)
                {
                    MessageBox.Show("原價必須為有效的數字且大於等於 0");
                    txtOriginalPrice.Focus();
                    return;
                }

                if (!decimal.TryParse(txtSalePrice.Text, out decimal salePrice) || salePrice < 0)
                {
                    MessageBox.Show("售價必須為有效的數字且大於等於 0");
                    txtSalePrice.Focus();
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("數量必須為正整數");
                    txtQuantity.Focus();
                    return;
                }

                if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("庫存必須為正整數");
                    txtStock.Focus();
                    return;
                }

                bool discontinued = chkDiscontinued.Checked;
                bool result;
                long? categoryId = null;

                if (cmbCategory.SelectedValue != null)
                {
                    categoryId = Convert.ToInt64(cmbCategory.SelectedValue);
                }

                if (productId == null)
                {
                    // 新增模式
                    result = repository.InsertProduct(txtName.Text.Trim(), originalPrice, salePrice, quantity, stock, discontinued, categoryId);
                }
                else
                {
                    // 編輯模式
                    result = repository.UpdateProduct(productId.Value, txtName.Text.Trim(), originalPrice, salePrice, quantity, stock, discontinued, categoryId);
                }
                if (result)
                {
                    // 取得商品ID（新增或編輯模式）
                    int currentProductId = productId ?? GetLastInsertedProductId();

                    // 如果有上傳圖片，產生縮圖並存入資料庫
                    if (uploadedImage != null)
                    {
                        byte[] thumbnailBytes = CreateThumbnail(uploadedImage, 100, 100);
                        var photoRepo = new ProductPhotoRepository();

                        bool success = photoRepo.InsertPhoto(
                            currentProductId,
                            thumbnailBytes,
                            uploadedFileName,
                            uploadedImage,
                            uploadedFileName
                        );

                        if (!success)
                        {
                            MessageBox.Show("圖片儲存失敗！");
                        }
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("資料儲存失敗！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("發生錯誤：" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //載入分類清單
        private void LoadCategories()
        {
            var categoryRepo = new CategoryRepository();
            var dt = categoryRepo.GetAllCategories();

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.DataSource = dt;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "圖片檔 (*.jpg;*.png)|*.jpg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    uploadedImage = File.ReadAllBytes(ofd.FileName);       // 圖片資料
                    uploadedFileName = Path.GetFileName(ofd.FileName);     // 檔名
                    picPreview.Image = new Bitmap(ofd.FileName);           // 顯示圖檔
                }
            }
        }

        //取得最後插入的產品 ID
        private int GetLastInsertedProductId()
        {
            using (SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=ShopDB;Integrated Security=True;"))
            {
                string sql = "SELECT TOP 1 ProductID FROM Products ORDER BY ProductID DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        //建立縮圖產生函式
        private byte[] CreateThumbnail(byte[] originalImageBytes, int width, int height)
        {
            using (MemoryStream ms = new MemoryStream(originalImageBytes))
            using (Image originalImage = Image.FromStream(ms))
            using (Bitmap thumb = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(thumb))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, width, height);
                using (MemoryStream thumbStream = new MemoryStream())
                {
                    thumb.Save(thumbStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return thumbStream.ToArray();
                }
            }
        }

    }
}
