using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using MaterialSkin;
using System.Data;
using System.Security.Cryptography;
using Models;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
namespace Controler
{
    class DataProvider
    {
        private static string constring = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\""+ Directory.GetCurrentDirectory()+"\\hi.mdf\";Integrated Security=True;Connect Timeout=30";
        private static DataProvider ins = null;
        public static DataProvider Instance
        { get { if (ins == null) { ins = new DataProvider(); } return ins; } }

        public DataTable ExecuteQuery(string Query, object[] Parameter = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Query, con);
                if (Parameter != null)
                {
                    string[] l = Query.Split(' ');
                    uint i = 0;
                    foreach (string item in l)
                    {
                        if (item.Contains('@'))
                        {
                            cmd.Parameters.AddWithValue(item, Parameter[i]);
                            i++;
                        }
                    }
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(data);
                con.Close();
            }

            return data;
        }

        public void ExecuteNonQuery(string Query, object[] Parameter = null)
        {

            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Query, con);
                if (Parameter != null)
                {
                    string[] l = Query.Split(' ');
                    uint i = 0;
                    foreach (string item in l)
                    {
                        if (item.Contains('@'))
                        {
                            cmd.Parameters.AddWithValue(item, Parameter[i]);
                            i++;
                        }
                    }
                }
                cmd.ExecuteNonQuery();
                con.Close();
            }


        }
    }
    class Controller
    {
        private static Controller ins = null;
        public static Controller Instance
        { get { if (ins == null) ins = new Controller(); return ins; } }

        #region Privite Method
        private static string Hash(string pass)
        {
            SHA512 sha = SHA512.Create();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(pass))).Replace("-", null).ToLower();
        }

        #endregion
        #region Public Method
        public void InsertUser(string Username, bool Roll)
        {
            DataProvider.Instance.ExecuteNonQuery("pInsertUser @Username , @Hash , @Roll", new object[] { Username, Hash(Username + "1"), Roll });
        }
        public MenuFoodBill[] GetMenusByBillID(int BillID)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select Food.id as idFood ,foodname, foodcount, tBillInfo.price from tBillInfo, Food where  Food.id = idFood and idBill = " + BillID.ToString());
            MenuFoodBill[] menuFoods = new MenuFoodBill[data.Rows.Count];
            uint i = 0;
            foreach ( DataRow item in data.Rows)
            {
                menuFoods[i] = new MenuFoodBill(item);
                i++;
            }
            return menuFoods;

        }
        public string UpperString(string s)
        {
            s = s.Trim();
            StringBuilder Result = new StringBuilder();
            string[] SubName = s.Split(' ');

            for (int i = 0; i < SubName.Length; i++)
            {
                string FirstChar = SubName[i].Substring(0, 1);
                string OtherChar = SubName[i].Substring(1);
                SubName[i] = FirstChar.ToUpper() + OtherChar.ToLower();
                Result.Append(SubName[i]).Append(" ");
            }
            return Result.ToString().Trim();
        }
        public void UpdatePassword(string Username, string Password)
        {
            DataProvider.Instance.ExecuteNonQuery("exec pUpdatePassword @Username , @Hash", new object[] { Username, Hash(Username + Password) });
        }
        public MenuFood[] GetMenusByTableID(string TableID)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select idFood, foodname, foodCount, Price, Price*foodCount as totalPrice from tRAMBill, Food, tTable where tRAMBill.idFood = Food.id and tRAMBill.idTable = tTable.id and tTable.id = " + TableID);
            MenuFood[] menus = new MenuFood[data.Rows.Count];
            uint i = 0;
            foreach (DataRow item in data.Rows)
            {
                menus[i] = new MenuFood(item);
                i++;
            }
            return menus;
        }
        public BillInfo[] GetBillInfosByTableID(string TableID)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select *from BillInfo, Food where Food.id = idFood and BillInfo.idTable  = " + TableID);
            BillInfo[] bills = new BillInfo[data.Rows.Count];
            uint i = 0;
            foreach (DataRow item in data.Rows)
            {
                bills[i] = new BillInfo(item);
                i++;
            }
            return bills;
        }
        public Food[] GetFoodsByCatergoty(int CatergoryID)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select Food.id, Food.foodname, Food.foodCaId, Food.Price from Food, Catery where foodCaId = Catery.id and Catery.id = "+ CatergoryID.ToString());
            Food[] foods = new Food[data.Rows.Count];
            uint i = 0;
            foreach (DataRow item in data.Rows)
            {
                foods[i] = new Food(item);
                i++;
            }

            return foods;
        }
        public Table[] GetTables()
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("select tTable.id, tTable.tableName, tStatus.satusName from tTable, tStatus where tTable.idStatus = tStatus.id");
            Table[] tables = new Table[data.Rows.Count];
            uint i = 0;
            foreach (DataRow item in data.Rows)
            {
                tables[i] = new Table(item);
                i++;
            }
            return tables;
        }
        public void SettingForm()
        {
            
            MaterialSkinManager.Instance.Theme = MaterialSkinManager.Themes.LIGHT;
            MaterialSkinManager.Instance.ColorScheme = new ColorScheme(Primary.Blue300, Primary.Blue600, Primary.Amber100, Accent.Amber700, TextShade.WHITE);
        }
        public bool Login(DataTable Table, string Username, string Password)
        {
            if (Table.Rows.Count == 1)
            {
                return Hash(Username + Password) ==  Table.Rows[0]["pass"].ToString(); 
            }
            return false;
        }
        public Catergory[] GetCatergories()
        {
            DataTable table = DataProvider.Instance.ExecuteQuery("select * from Catery");
            Catergory[] catergories = new Catergory[table.Rows.Count];
            uint i = 0;
            foreach (DataRow item in table.Rows)
            {
                catergories[i] = new Catergory(item);
                i++;

            }
            return catergories;
        }
        public void SetDTGV(DataGridView[] gridView)
        {
            foreach (DataGridView item in gridView)
            {
                item.ReadOnly = true;
                item.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                item.AllowUserToAddRows = false;
                item.BorderStyle = BorderStyle.Fixed3D;
                item.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
                item.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
                item.DefaultCellStyle.SelectionBackColor = Color.Gray;
                item.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
                item.BackgroundColor = Color.GhostWhite;

                item.EnableHeadersVisualStyles = true;
                item.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                item.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSeaGreen;//Color.FromArgb(20, 25, 72);
                item.ColumnHeadersDefaultCellStyle.ForeColor = Color.Brown;
            }

        }
        public bool GetRollByUsername(string Username)
        {
            return (bool)DataProvider.Instance.ExecuteQuery("exec pGetRollFromUser @Username", new object[] { Username }).Rows[0][0];
        }
        public void UpdateUserRoll(string Username, bool Roll)
        {
            DataProvider.Instance.ExecuteNonQuery("pUpdateUser @Username , @Roll", new object[] { Username, Roll });
        }
        public void InsertFood(string FoodName, int CatergoryID, int Price)
        {
            DataProvider.Instance.ExecuteNonQuery("exec pInsertFood @FoodName , @CaID , @Price", new object[] { FoodName, CatergoryID, Price });
        }
        public void UpdateFood(int FoodID, string FoodName, int CatergoryID, int Price)
        {
            DataProvider.Instance.ExecuteNonQuery("exec pUpdateFood @FoodID , @FoodName , @CaID , @Price", new object[] { FoodID, FoodName, CatergoryID, Price });
        }
        public void DeleteFood(int FoodID)
        {
            DataProvider.Instance.ExecuteNonQuery("exec pDeleteFood @FoodID", new object[] { FoodID });
        }

        #endregion

    }
}
