using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using MaterialSkin;
using Controler;
using Models;
using System.Globalization;

namespace WindowsFormsApp1.Views
{
    public partial class MainFrom : MaterialForm
    {
        
        uint totalPrice = 0;
        Account acc;
        string username = null;
        public static CultureInfo cultureInfo =new CultureInfo("vi-vn");
        Info info;
        public static void CloseAdmin(bool i)
        {
            showFormAd = i;
        }
        static bool showFormAd = false;
        int TableID = 0;
        
        
        public MainFrom(Account account)
        {
            acc = account;           
            InitializeComponent();
            username = account.Username;
            lbDisplayName.Text = "Xin Chào " + account.DisplayName;
            if (account.Roll == false)
            {
               adminToolStripMenuItem.Enabled = false;
            }
            MaterialSkinManager.Instance.AddFormToManage(this);
            Controller.Instance.SettingForm();
            LoadData();
            
            
        }

        #region Method
        void LoadData()
        {
            LoadTableList();
            cmbCatergory.DataSource = Controller.Instance.GetCatergories();
            
        }


         void LoadTableList()
        {
            foreach (Table item in Controller.Instance.GetTables())
            {
                Button button = new Button();
                button.Width = button.Height = 80;
                button.Text = item.name + "\n" + item.status;
                button.Tag = item.id;
                if (item.status == "Trống")
                {
                    button.BackColor = ColorTranslator.FromHtml("#0be21a");
                }
                else
                {
                    button.BackColor = ColorTranslator.FromHtml("#ef0909");
                }
                button.Click += Button_Click;
                flpTableList.Controls.Add(button);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            
            numericUpDown1.Value = 0;
            lvBillInfo.Items.Clear();
            totalPrice = 0;
            Button button = sender as Button;
            TableID = Convert.ToInt32(button.Tag);
            LoadBillListView();

        }
        void LoadBillListView()
        {
            lvBillInfo.Items.Clear();
            totalPrice = 0;
            foreach (MenuFood item in Controller.Instance.GetMenusByTableID(TableID.ToString()))
            {

                ListViewItem viewItem = new ListViewItem(item.idFood.ToString());
                viewItem.SubItems.Add(item.foodName);
                viewItem.SubItems.Add(item.foodCount.ToString());
                viewItem.SubItems.Add(item.price.ToString());
                totalPrice = Convert.ToUInt32(totalPrice + item.totalPrice);
                lvBillInfo.Items.Add(viewItem);
            }
            lbTotalPrice.Text = totalPrice.ToString("c", cultureInfo).Split(',').First()+ "đ";
        }
        #endregion


        #region Event

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2.Instance.UpdateFood += Instance_UpdateFood;
            Form2.Instance.InsertFood += Instance_InsertFood;
            Form2.Instance.DeleteFood += Instance_DeleteFood;
            Form2.Instance.ChangeTableInfo += Instance_ChangeTableInfo;

            if (showFormAd)
            {
                Form2.Instance.BringToFront();
            }
            else
            {              
                Form2.Instance.Show();
                showFormAd = true;
            }
           
        }

        private void Instance_ChangeTableInfo(object sender, EventArgs e)
        {
            flpTableList.Controls.Clear();
            LoadTableList();
        }

        private void Instance_DeleteFood(object sender, EventArgs e)
        {
            LoadBillListView();
            cmbFood.DataSource = Controller.Instance.GetFoodsByCatergoty((cmbCatergory.SelectedItem as Catergory).id);
        }

        private void Instance_InsertFood(object sender, EventArgs e)
        {
            cmbFood.DataSource = Controller.Instance.GetFoodsByCatergoty((cmbCatergory.SelectedItem as Catergory).id);
        }

        private void Instance_UpdateFood(object sender, EventArgs e)
        {
            LoadBillListView();
            cmbFood.DataSource = Controller.Instance.GetFoodsByCatergoty((cmbCatergory.SelectedItem as Catergory).id);

        }
        private void cmbCatergory_SelectedIndexChanged(object sender, EventArgs e)
        {

            cmbFood.DataSource = Controller.Instance.GetFoodsByCatergoty((cmbCatergory.SelectedItem as Catergory).id);
        }

        private void MainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn Thực Sự Muốn Đăng Xuất ?", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                Form2.Instance.Hide();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (totalPrice < Convert.ToInt32(numericUpDown1.Value))
            {
                return;
            }
            lbTotalPrice.Text = (totalPrice - Convert.ToUInt32(numericUpDown1.Value)).ToString("c", cultureInfo).Split(',').First();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            if (TableID == 0)
            {
                MessageBox.Show("Xin Hãy Chọn Bàn", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataProvider.Instance.ExecuteNonQuery("exec pUpdateMenuFoodProvider @FoodID , @TableID", new object[] { (cmbFood.SelectedItem as Food).id, TableID });
            LoadBillListView();
            flpTableList.Controls.Clear();
            LoadTableList();
            Form2.Instance.LoadMethod(1);
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            if (TableID == 0 )
            {
                
                MessageBox.Show("Xin Hãy Chọn Bàn", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (lvBillInfo.Items.Count == 0)
            {
                MessageBox.Show("Không Thể Thanh Toán Bàn Trống", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataProvider.Instance.ExecuteQuery("exec pPayBill @TotalPrice , @LowPrice , @TableID", new object[] {Convert.ToInt32(lbTotalPrice.Text.Replace(".", null).Replace("đ", null)), Convert.ToInt32(numericUpDown1.Value), TableID });
            Form2.Instance.LoadMethod(0);
            LoadBillListView();
            flpTableList.Controls.Clear();
            LoadTableList();
            Form2.Instance.LoadMethod(1);
        }
        #endregion

        private void đặtLạiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePass cp = new ChangePass(acc);
            cp.ShowDialog();
        }

        private void thôngTinToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            info = new Info(acc);
            info.UpdateDisplayName += Info_UpdateDisplayName;
            info.ShowDialog();
        }

        private void Info_UpdateDisplayName(object sender, EventArgs e)
        {
            acc = new Account(DataProvider.Instance.ExecuteQuery("select * from tAccount where username = '" + username + "'").Rows[0]);
            lbDisplayName.Text = "Xin Chào " + acc.DisplayName;
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            if (TableID == 0)
            {
                MessageBox.Show("Xin Hãy Chọn Bàn", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (lvBillInfo.Items.Count == 0)
            {
                MessageBox.Show("Không Thể Bớt Món Cho Bàn Trống", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataProvider.Instance.ExecuteNonQuery("pUpdateSubtractFoodMenuProvider @FoodID , @TableID", new object[] { (cmbFood.SelectedItem as Food).id, TableID });
            LoadBillListView();
            flpTableList.Controls.Clear();
            LoadTableList();
            Form2.Instance.LoadMethod(1);
        }
    }
}
