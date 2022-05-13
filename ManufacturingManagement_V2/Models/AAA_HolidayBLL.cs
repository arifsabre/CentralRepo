using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_HolidayBLL
    {
        //Return list of all Employees
        public List<AAA_HolidayMDI> Get_Holiday_List()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_HolidayMDI> HList = new List<AAA_HolidayMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_Get_HolidayList", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_HolidayMDI
                    {
                        HolidayId = Convert.ToInt32(rdr["HolidayId"]),
                       // By_User = Convert.ToInt32(rdr["By_User"]),
                        HolidayName = rdr["HolidayName"].ToString(),
                        HolidayDate = Convert.ToDateTime(rdr["HolidayDate"]),
                       // CompCode = Convert.ToInt32(rdr["CompCode"]),
                        Description = rdr["Description"].ToString(),
                       // AdjustmentDate = Convert.ToDateTime(rdr["AdjustmentDate"]),
                        Day_of_Week = rdr["Day_of_Week"].ToString(),
                       /// By_User = Convert.ToInt32(rdr["By_User"]),
                    });
                }
                return HList;
               }
        }


        public int Insert_Holiday(AAA_HolidayMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Insert_Holiday", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@HolidayName", hld.HolidayName);
                com.Parameters.AddWithValue("@HolidayDate", hld.HolidayDate);
                com.Parameters.AddWithValue("@Description", hld.Description);
               // com.Parameters.AddWithValue("@AdjustmentDate", hld.AdjustmentDate);
               // com.Parameters.AddWithValue("@On_Day", hld.On_Day);
                //com.Parameters.AddWithValue("@By_User", hld.By_User);

                // con.Open();
                // com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

               
        public int UpdateHoliday(AAA_HolidayMDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_UpdateHolidaylist", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@HolidayId", hlu.HolidayId);
                com.Parameters.AddWithValue("@HolidayName", hlu.HolidayName);
                com.Parameters.AddWithValue("@HolidayDate", hlu.HolidayDate);
                com.Parameters.AddWithValue("@Description", hlu.Description);
                //com.Parameters.AddWithValue("@AdjustmentDate", hlu.AdjustmentDate);
                //com.Parameters.AddWithValue("@On_Day", hld.On_Day);
                //com.Parameters.AddWithValue("@By_User", hld.By_User);
               // con.Open();
                // com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        }
        }
    
