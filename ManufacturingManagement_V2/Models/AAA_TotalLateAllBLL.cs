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
    public class AAA_TotalLateAllBLL
    {
        //Return list of all Employees
        public List<AAA_TotalLateAll> LateCount()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_TotalLateAll> Late = new List<AAA_TotalLateAll>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_LateArrivalALL", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    Late.Add(new AAA_TotalLateAll
                    {
                        TCount = Convert.ToInt32(rdr["TCount"]),
                        });
                }
                return Late;
            }
        }

                
    }
}