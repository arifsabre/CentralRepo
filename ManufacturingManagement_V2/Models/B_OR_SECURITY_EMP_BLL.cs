using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class B_OR_SECURITY_EMP_BLL
    {

        public List<B_OR_SECURITY_EMP_MODEL> Get_EMP_OR_List()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<B_OR_SECURITY_EMP_MODEL> HList = new List<B_OR_SECURITY_EMP_MODEL>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_Get_Emp_OR_EMP_Security", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new B_OR_SECURITY_EMP_MODEL
                    {
                        ECode = Convert.ToInt32(rdr["ECode"]),
                        EmpName = rdr["EmpName"].ToString(),
                        compcode = rdr["compcode"].ToString(),
                        IsActive = Convert.ToBoolean(rdr["IsActive"]),

                    });
                }
                return HList;
            }
        }


        public int Insert_EMP_OR(B_OR_SECURITY_EMP_MODEL hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_USP_Insert_OR_EMP_Security]", con);
                com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@ECode", hld.ECode);
                com.Parameters.AddWithValue("@EmpName1", hld.EmpName);
                com.Parameters.AddWithValue("@compcode", hld.compcode);
                // com.Parameters.AddWithValue("@IsActive", hld.IsActive);
                i = com.ExecuteNonQuery();
            }
            return i;
        }


        public int Update_EMP_OR(B_OR_SECURITY_EMP_MODEL hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_USP_Update_OR_EMP_Security]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ECode", hlu.ECode);
                com.Parameters.AddWithValue("@EmpName", hlu.EmpName);
                com.Parameters.AddWithValue("@compcode", hlu.compcode);
                com.Parameters.AddWithValue("@IsActive", hlu.IsActive);
                //com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Delete_EMP_OR(B_OR_SECURITY_EMP_MODEL hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_USP_Delete_OR_EMP_Security]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ECode", hlu.ECode);
                //com.Parameters.AddWithValue("@EmpName", hlu.EmpName);
                //com.Parameters.AddWithValue("@compcode", hlu.compcode);
                //com.Parameters.AddWithValue("@IsActive", hlu.IsActive);
                //com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }



    }
}






