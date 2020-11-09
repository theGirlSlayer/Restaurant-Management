using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Animations;
using MaterialSkin.Controls;
using MaterialSkin;
using Controler;
using Models;

namespace WindowsFormsApp1.Views
{
    public partial class BillForm : MaterialForm
    {
        public BillForm(int BillID, int TotalPrice, int LowPrice)
        {
            InitializeComponent();
            LoadListView(BillID);
            lbBillID.Text = BillID.ToString();
            lbTotalPrice.Text = TotalPrice.ToString("c", MainFrom.cultureInfo).Split(',')[0]+"đ";
            lbLowPrice.Text = LowPrice.ToString("c", MainFrom.cultureInfo).Split(',')[0]+ "đ";
        }
        void LoadListView(int billid)
        {
            foreach (MenuFoodBill item in Controller.Instance.GetMenusByBillID(billid))
            {
                ListViewItem viewItem = new ListViewItem(item.idFood.ToString());
                viewItem.SubItems.Add(item.foodName);
                viewItem.SubItems.Add(item.foodCount.ToString());
                viewItem.SubItems.Add(item.price.ToString());
                lvBill.Items.Add(viewItem);
            }
        }
    }
}
