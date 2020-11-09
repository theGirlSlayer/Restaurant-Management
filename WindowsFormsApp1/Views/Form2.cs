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
using System.Threading;

namespace WindowsFormsApp1.Views
{
    public partial class Form2 : MaterialForm
    {
        private static Form2 ins = null;
        public static  Form2 Instance
        { get { if (ins == null) { ins = new Form2(); }return ins; } }
        public MainFrom MF;
        string username = null;
        ulong totalBill = 0;
        public void LoadMethod(byte Signal)
        {
            switch (Signal)
            {
                case 0: LoadBillInfo(); break;
                case 1: LoadTable(); break;
                case 2: LoadData(); break;
            }
            
        }
        bool Ad = false;
        BindingSource FoodBind = new BindingSource(), AccBind = new BindingSource(), TableBind = new BindingSource();
        public event EventHandler UpdateFood;
        public event EventHandler InsertFood;
        public event EventHandler DeleteFood;
        public event EventHandler ChangeTableInfo;
        public Form2()
        {
     
            //Thread.CurrentThread.CurrentCulture = MainFrom.cultureInfo;
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
            Controller.Instance.SettingForm();
            Controller.Instance.SetDTGV(new DataGridView[] { dtgvFood, dtgvTable,dtgvAccount, dtgrBill });
            LoadData();
            BindData();
            
        }
        #region Method
        void LoadData()
        {    
            LoadFood();
            LoadAccount();
            LoadTable();
            LoadBillInfo();
            cmbFoodCartergory.DataSource = Controller.Instance.GetCatergories();
        }
        #region Load Method
        void LoadBillInfo()
        {
            totalBill = 0;
            DataTable data = DataProvider.Instance.ExecuteQuery("exec pGetBillByDate @DateFrom , @DateTo", new object[] { dtpkFrom.Value, dtpkTo.Value });
            dtgrBill.DataSource = data;
            foreach (DataRow item in data.Rows)
            {
                totalBill = totalBill +Convert.ToUInt64(item["Tổng Tiền"]);
            }
            lbTotalPrice.Text = totalBill.ToString( "c",MainFrom.cultureInfo).Split(',')[0] + "đ";
        }
        void LoadAccount()
        {
            dtgvAccount.DataSource = AccBind;
            AccBind.DataSource = DataProvider.Instance.ExecuteQuery("select username as [Tên Đăng Nhập], roll as [Quyền] from tAccount ");
        }
        void LoadTable()
        {
            dtgvTable.DataSource = TableBind;
            TableBind.DataSource = DataProvider.Instance.ExecuteQuery("select tTable.id as [ID], tableName as [Tên Bàn], tStatus.satusName as [Trạng Thái] from tTable, tStatus where idStatus = tStatus.id");
        }
        void LoadFood()
        {
            dtgvFood.DataSource = FoodBind;
            FoodBind.DataSource = DataProvider.Instance.ExecuteQuery("select Food.id as [ID], Food.foodname as [Tên Món], CaName as [Danh Mục], Price as [Giá] from Food, Catery where foodCaId = Catery.id ");
        }
        #endregion

        void BindData()
        {
            ///Food Binding///////
            txtFoodID.DataBindings.Add(new Binding("text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtFoodName.DataBindings.Add(new Binding("text", dtgvFood.DataSource, "Tên Món", true, DataSourceUpdateMode.Never));
            txtPrice.DataBindings.Add(new Binding("text", dtgvFood.DataSource, "Giá", true, DataSourceUpdateMode.Never));
            cmbFoodCartergory.DataBindings.Add(new Binding("text", dtgvFood.DataSource, "Danh Mục", true, DataSourceUpdateMode.Never));
            ///Food Binding///////

            ///Account Binding///////////
            txtUser.DataBindings.Add(new Binding("text", dtgvAccount.DataSource, "Tên Đăng Nhập", true, DataSourceUpdateMode.Never));


            /////Table Binding//////////
            txtTableID.DataBindings.Add(new Binding("text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtTableName.DataBindings.Add(new Binding("text", dtgvTable.DataSource, "Tên Bàn", true, DataSourceUpdateMode.Never));

        }
        #endregion


        #region Event

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainFrom.CloseAdmin(false);
            e.Cancel = true;
            Hide();
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            LoadFood();
        }
        private void cmbRoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRoll.Text == "Nhân Viên")
            {
                Ad = false;
            }
            else
            {
                Ad = true;
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            username = txtUser.Text;
            if (txtUser.Text == "")
            {
                return;
            }
            if ((bool)dtgvAccount.SelectedCells[0].OwningRow.Cells["Quyền"].Value)
            {
                cmbRoll.Text = "Quản Trị Viên";
                Ad = true;
            }
            else
            {
                cmbRoll.Text = "Nhân Viên";
                Ad = false;
            }
        }

        private void materialRaisedButton11_Click(object sender, EventArgs e)
        {
            Controller.Instance.UpdateUserRoll(username, Ad);
            LoadAccount();
        }

        private void materialRaisedButton15_Click(object sender, EventArgs e)
        {
            dtgvAccount.DataSource = AccBind;
            AccBind.DataSource = DataProvider.Instance.ExecuteQuery("exec pSearchAcc @KeyWord", new object[] { txtSearchAcc.Text });
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            Catergory catergory = cmbFoodCartergory.SelectedItem as Catergory;
            try
            {
                Controller.Instance.InsertFood(Controller.Instance.UpperString(txtFoodName.Text.Trim()), Convert.ToInt32(catergory.id), Convert.ToInt32(txtPrice.Text));
                LoadFood();
                InsertFood(this, new EventArgs());
                
            }
            catch 
            {

                MessageBox.Show("Thông Tin Nhập Vào Không Hợp Lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            Catergory catergory = cmbFoodCartergory.SelectedItem as Catergory;
            try
            {
                Controller.Instance.UpdateFood(Convert.ToInt32(txtFoodID.Text), Controller.Instance.UpperString(txtFoodName.Text), catergory.id, Convert.ToInt32(txtPrice.Text));
                LoadFood();
                    UpdateFood(this, new EventArgs());
                
            }
            catch (Exception g)
            {

                MessageBox.Show(g.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {
            try
            {
                Controller.Instance.DeleteFood(Convert.ToInt32(txtFoodID.Text));
                LoadFood();
                DeleteFood(this, new EventArgs());
            }
            catch (Exception)
            {
                MessageBox.Show("Bạn Không Thể Xóa Món Đã Từng Sử Dụng", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }

        private void materialRaisedButton14_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void materialRaisedButton13_Click(object sender, EventArgs e)
        {
            try
            {
                Controller.Instance.InsertUser(txtUser.Text, Ad);
                LoadAccount();
            }
            catch 
            {

                MessageBox.Show("Tài Khoản Đã Tồn Tại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
        }

        private void materialRaisedButton12_Click(object sender, EventArgs e)
        {
            DataProvider.Instance.ExecuteNonQuery("pDeleteUser @Username", new object[] { username });
            LoadAccount();
        }

        private void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            LoadTable();
        }
        

        private void materialRaisedButton17_Click(object sender, EventArgs e)
        {
            DataProvider.Instance.ExecuteNonQuery("pUpdateTable @TableID , @TableName", new object[] { Convert.ToInt32(txtTableID.Text), txtTableName.Text });
            LoadTable();
            ChangeTableInfo(this, new EventArgs());
        }

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {
            if (dtgvTable.SelectedCells[0].OwningRow.Cells["Trạng Thái"].Value.ToString() == "Trống")
            {
                DataProvider.Instance.ExecuteNonQuery("pDeleteTable @TableID", new object[] { Convert.ToInt32(txtTableID.Text) });
                LoadTable();
                ChangeTableInfo(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Không Thể Xóa Bàn Đang Có Khách", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dtgrBill_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            BillForm bf = new BillForm(Convert.ToInt32(dtgrBill.SelectedCells[0].OwningRow.Cells["Mã Hóa Đơn"].Value), Convert.ToInt32(dtgrBill.SelectedCells[0].OwningRow.Cells["Tổng Tiền"].Value), Convert.ToInt32(dtgrBill.SelectedCells[0].OwningRow.Cells["Giảm Giá"].Value));
            bf.ShowDialog();
        }

        private void materialRaisedButton16_Click(object sender, EventArgs e)
        {
            AddCatergory catergory = new AddCatergory();
            catergory.LoadCatergory += Catergory_LoadCatergory;
            catergory.ShowDialog();
            
        }

        private void Catergory_LoadCatergory(object sender, EventArgs e)
        {
            LoadFood();
            cmbFoodCartergory.DataSource = Controller.Instance.GetCatergories();
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            LoadBillInfo();
        }


        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            DataProvider.Instance.ExecuteNonQuery("insert tTable default values");
            LoadTable();
            ChangeTableInfo(this, new EventArgs());
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            dtgvFood.DataSource = FoodBind;
            FoodBind.DataSource = DataProvider.Instance.ExecuteQuery("exec pSreachFood @KeyWord", new object[] { txtSearchFood.Text });
        }
        #endregion
    }
}
