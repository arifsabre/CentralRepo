using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FlagUpdateMDI
    {
        //public int  NewEmpId { get; set; }
        //public bool SendSMSLate { get; set; }
        //public bool SendSMSAbsent { get; set; }
        //public bool SendSMSMissing { get; set; }

       // public List<EmployeeMdl> GetempList { get; set; }

        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static EmployeeBLL Instance
        {
            get { return new EmployeeBLL(); }
        }

        //
        public List<EmployeeMdl> Get_AllFlagList()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<EmployeeMdl> HList = new List<EmployeeMdl>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[Emp_GetEmpSMSStatus]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new EmployeeMdl
                    {
                        NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                        // By_User = Convert.ToInt32(rdr["By_User"]),
                        EmpName = rdr["EmpName"].ToString(),
                        SendSMSLate = Convert.ToBoolean(rdr["SendSMSLate"]),
                        SendSMSAbsent = Convert.ToBoolean(rdr["SendSMSAbsent"]),
                        SendSMSMissing = Convert.ToBoolean(rdr["SendSMSMissing"]),
                    });
                }
                return HList;
            }
        }
        //
        internal void SMSUpdateLate(EmployeeMdl empmdl)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "[Emp_UpDate_SMS_Late]";
                cmd.Parameters.Add(mc.getPObject("@NewEmpId", empmdl.NewEmpId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@SendSMSLate", empmdl.SendSMSLate, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, empmdl.NewEmpId.ToString(), "SMS Status Late Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                mc.setErrorLog("FlagUpdateMDI", "SMSUpdateLate", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
     
        //
        internal void SMSUpdateAbsent(EmployeeMdl empmdl)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "[Emp_UpDate_SMS_Absent]";
                cmd.Parameters.Add(mc.getPObject("@NewEmpId", empmdl.NewEmpId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@SendSMSAbsent", empmdl.SendSMSAbsent, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, empmdl.NewEmpId.ToString(), "SMS Status Absent Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                mc.setErrorLog("FlagUpdateMDI", "SMSUpdateAbsent", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        //
        internal void SMSUpdateMissing(EmployeeMdl empmdl)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "[Emp_UpDate_SMS_Missing]";
                cmd.Parameters.Add(mc.getPObject("@NewEmpId", empmdl.NewEmpId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@SendSMSMissing", empmdl.SendSMSMissing, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, empmdl.NewEmpId.ToString(), "SMS Status Missing Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                mc.setErrorLog("FlagUpdateMDI", "SMSUpdateMissing", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }



    }
}