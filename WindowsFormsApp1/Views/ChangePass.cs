using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using Models;
using Controler;

namespace WindowsFormsApp1.Views
{
    public partial class ChangePass : MaterialForm
    {
        public ChangePass(Account account)
        {
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
            Controller.Instance.SettingForm();
            txtUsername.Text = account.Username;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            if (txtOldPass.Text == "")
            {
                MessageBox.Show("Vui Lòng Nhập Mật Khẩu Hiện Tại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtNewPass.Text.Length < 6)
            {
                MessageBox.Show("Mật Khẩu Phải Dài Hơn Hoặc Bàng 6 Kí Tự", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtNewPass.Text != txtComfirm.Text)
            {
                MessageBox.Show("Mật Khẩu Xác Nhận Không Khớp", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Controller.Instance.Login((DataProvider.Instance.ExecuteQuery("pLogin @Username", new object[] { txtUsername.Text})), txtUsername.Text, txtOldPass.Text))
            {
                Controller.Instance.UpdatePassword(txtUsername.Text, txtComfirm.Text);
                MessageBox.Show("Đổi Mật Khẩu Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComfirm.Text = txtNewPass.Text = txtOldPass.Text = "";
            }
            else
            {
                MessageBox.Show("Sai Mật Khẩu, Vui Lòng Kiểm Tra Lại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
    }
}
