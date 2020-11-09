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
namespace WindowsFormsApp1.Views
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
            MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.LIGHT;
            MaterialSkinManager.Instance.ColorScheme = new ColorScheme(Primary.Blue300, Primary.Blue600, Primary.Amber100, Accent.Amber700, TextShade.BLACK);

            
        }
        MainFrom mf;
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("exec pLogin @Username", new object[] { txtUser.Text });
            if (Controller.Instance.Login(data, txtUser.Text, txtPasword.Text))
            {
                mf = new MainFrom(new Account(data.Rows[0]));
                Hide();
                mf.ShowDialog();
                Show();
            }
            else
            {
                MessageBox.Show("Sai Tài Khoản Hoặc Mật Khẩu", "Không Thể Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            Dispose();
            Application.Exit();
        }
    }
}
