using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Models
{
    class Table
    {
        public int id;
        public string name;
        public string status;
        public Table(DataRow row)
        {
            id = (int) row["id"];
            name = row["tableName"].ToString();
            status = row["satusName"].ToString();
        }
    }
    class Catergory
    {
        public int id;
        public string name;
        public Catergory(DataRow row)
        {
            id = (int) row["id"];
            name = row["CaName"].ToString();
        }
        public override string ToString()
        {
            return name;
        }
    }
    class Food
    {
        public int id, CaID, price;
        public string name;
        public Food(DataRow row)
        {
            id = (int)row["id"];
            CaID = (int)row["foodCaId"];
            name = row["foodname"].ToString();
            price = (int)row["price"]; 
        }
        public override string ToString()
        {
            return name;
        }
    }
    class BillInfo
    {
        public int id, idFood, foodcount, idtable;
        public BillInfo(DataRow row)
        {
            id = (int)row["id"];
            idFood = (int)row["idFood"];
            foodcount = (int)row["foodcount"];
            idtable = (int)row["idTable"];
        }
    }
    class MenuFood
    {
        public int idFood, foodCount, price, totalPrice;
        public string foodName;
        public MenuFood(DataRow row)
        {
            idFood = (int)row["idFood"];
            foodCount = (int)row["foodCount"];
            price = (int)row["Price"];
            foodName = row["foodname"].ToString();
            totalPrice = (int)row["totalPrice"];
        }
    }
    class MenuFoodBill
    {
        public int idFood, foodCount, price;
        public string foodName;
        public MenuFoodBill(DataRow row)
        {
            idFood = (int)row["idFood"];
            foodCount = (int)row["foodCount"];
            price = (int)row["Price"];
            foodName = row["foodname"].ToString();
        }
    }
    public class Account
    {
        public string Username, DisplayName;
        public bool Roll;
        public Account(DataRow row)
        {
            Username = row["username"].ToString();
            DisplayName = row["displayname"].ToString();
            Roll = (bool)row["roll"];
        }
    }
    
}
