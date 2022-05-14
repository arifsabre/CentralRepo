using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net.Mail;
namespace ManufacturingManagement_V2.Models
{
    public class AAA_SiteMenu_BLL
    {
        clsMyClass mc = new clsMyClass();

        public List<Menu_MenuName> Get_ParentMenu_List()
        {
            //clsMyClass mc = new clsMyClass();

            string cs = mc.strconn;
            //List<AAA_SiteMenu> HList = new List<AAA_SiteMenu>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                List<Menu_MenuName> HList = new List<Menu_MenuName>();
                con.Open();
                SqlCommand com = new SqlCommand("Tree_List_ParentMenu", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new Menu_MenuName
                    {
                        ParentMenuId = Convert.ToInt32(rdr["ParentMenuId"]),
                        ParentMenuName = rdr["ParentMenuName"].ToString(),

                    });
                }
                return HList;
            }
        }

        public int Insert_ParentMenu(Menu_MenuName hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {

                con.Open();
                SqlCommand com = new SqlCommand("Tree_Save_ParentMenu", con);
                com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@Item_Type_Id", hld.ParentMenuName);
                com.Parameters.AddWithValue("@ParentMenuName", hld.ParentMenuName);
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Update_ParentMenu(Menu_MenuName hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Tree_Update_ParentMenu", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ParentMenuId", hld.ParentMenuId);
                com.Parameters.AddWithValue("@ParentMenuName", hld.ParentMenuName);
                i = com.ExecuteNonQuery();
            }
            return i;
        }
    }
}