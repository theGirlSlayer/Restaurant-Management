using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controler;
using Models;
using MaterialSkin.Controls;
using MaterialSkin.Animations;

namespace WindowsFormsApp1.Views
{
    public partial class AddCatergory : MaterialForm
    {
        private static AddCatergory ins = null;
        public static AddCatergory Instance
        { get { if (ins == null) { ins = new AddCatergory(); } return ins; } }
        public event EventHandler LoadCatergory;
        int idCatergory = 0;
        public AddCatergory()
        {
            InitializeComponent();
            cmbCatergory.DataSource = Controller.Instance.GetCatergories();
            idCatergory = (cmbCatergory.SelectedItem as Catergory).id;
        }
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DataProvider.Instance.ExecuteNonQuery("pInsertCatergry @CaName", new object[] { Controller.Instance.UpperString(cmbCatergory.Text) });
                cmbCatergory.DataSource = Controller.Instance.GetCatergories();
                LoadCatergory(this, new EventArgs());
                MessageBox.Show("Thêm Danh Mục Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form2.Instance.LoadMethod(2);
                
            }
            catch 
            {
                MessageBox.Show("Danh Mục Không Được Trùng Tên", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
           
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataProvider.Instance.ExecuteNonQuery("pUpdateCatergry @IDCatergory , @CaName", new object[] { idCatergory, Controller.Instance.UpperString(cmbCatergory.Text )});
                cmbCatergory.DataSource = Controller.Instance.GetCatergories();
                LoadCatergory(this, new EventArgs());
                MessageBox.Show("Sửa Danh Mục Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            catch (Exception g)
            {
                MessageBox.Show(g.Message);
                MessageBox.Show("Danh Mục Không Được Trùng Tên", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
        }

        private void cmbCatergory_SelectedIndexChanged(object sender, EventArgs e)
        {
            idCatergory = (cmbCatergory.SelectedItem as Catergory).id;
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn Chắc Chắn Muốn Xóa Danh Mục ? \nTất Cả Những Món Chưa Từng Được Sử Dụng Mà Thuộc Danh Mục Này Cũng Sẽ Bị Xóa", "Thông Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    DataProvider.Instance.ExecuteNonQuery("pDeleteCatergry @IDCatergory", new object[] { idCatergory });
                    cmbCatergory.DataSource = Controller.Instance.GetCatergories();
                    MessageBox.Show("Xóa Danh Mục Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch 
                {
                    MessageBox.Show("Danh Mục Có Chứa Món Đã Từng Qua Sử Dụng, Không Thể Xóa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
