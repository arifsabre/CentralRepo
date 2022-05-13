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
    public class AAA_SendSMS_BLL
    {
        private SqlConnection con;
        private void Connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constring);
        }
       public List<EmployeeMdl> Get_UpdateList()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<EmployeeMdl> HList = new List<EmployeeMdl>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_GetEmpStatus]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new EmployeeMdl
                    {
                        NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                        // By_User = Convert.ToInt32(rdr["By_User"]),
                        EmpName = rdr["EmpName"].ToString(),
                        SendSMS = Convert.ToInt32(rdr["SendSMS"]),
                        //ReportingTo = Convert.ToInt32(rdr["ReportingTo"])
                    });
                }
                return HList;
            }
        }
        public int UpdateStatus(EmployeeMdl hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_UpDate_SMS_Status", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@NewEmpId", hlu.NewEmpId);
                com.Parameters.AddWithValue("@SendSMS", hlu.SendSMS);
                // com.Parameters.AddWithValue("@ReportingTo", hlu.ReportingTo);
                i = com.ExecuteNonQuery();
            }
            return i;
        }


        public List<EmployeeMdl> Get_ReportingToUpdate()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<EmployeeMdl> HList = new List<EmployeeMdl>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_GetReportingTo]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new EmployeeMdl
                    {
                        NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                        // By_User = Convert.ToInt32(rdr["By_User"]),
                        EmpName = rdr["EmpName"].ToString(),
                        ReportingTo = Convert.ToInt32(rdr["ReportingTo"]),
                     
                    });
                }
                return HList;
            }
        }

        public int UpdateReportingTo(EmployeeMdl hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_UpDate_ReportingTO_Status", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@NewEmpId", hlu.NewEmpId);
                com.Parameters.AddWithValue("@EmpName", hlu.EmpName);
                com.Parameters.AddWithValue("@ReportingTo", hlu.ReportingTo);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public List<EmployeeMdl> Get_ReportingToEmployee()
        {
            clsMyClass mc = new clsMyClass();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Clear();
            cmd.CommandText = "[ZZZ_EmpIdReportingTo]";
            mc.fillFromDatabase(ds, cmd);
            List<EmployeeMdl> ls = new List<EmployeeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new EmployeeMdl
                {
                   NewEmpId = Convert.ToInt32(dr["NewEmpId"]),
                   EmpName =   dr["EmpName"].ToString(),
                });
            }
            return ls;
        }







    }
}