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
    public partial class Info : MaterialForm
    {
        public event EventHandler UpdateDisplayName;
        public Info(Account account)
        {
            InitializeComponent();
            txtDisplayname.Text = account.DisplayName;
            txtUsername.Text = account.Username;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                MessageBox.Show("Vui Lòng Nhập Mật Khẩu", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Controller.Instance.Login(DataProvider.Instance.ExecuteQuery("exec pLogin @Username" , new object[] { txtUsername.Text}), txtUsername.Text, txtPass.Text))
            {
                DataProvider.Instance.ExecuteNonQuery("pUpdateDisplayName @Username , @Displayname", new object[] { txtUsername.Text, txtDisplayname.Text });
                MessageBox.Show("Đổi Tên Thành Công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateDisplayName(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Sai Mật Khẩu, Vui Lòng Kiểm Tra Lại", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
