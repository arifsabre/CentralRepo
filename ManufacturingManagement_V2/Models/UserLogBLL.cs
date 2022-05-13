using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ManufacturingManagement_V2.Models
{
    public class UserLogBLL
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        //
        clsCookie objCoockie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, UserLogMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@UserId", dbobject.UserId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@LoginAt", dbobject.LoginAt.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@OtpAt", dbobject.OtpAt.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OTP", dbobject.OTP, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsLogout", dbobject.IsLogout, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LogoutAt", dbobject.LogoutAt.ToString(), DbType.DateTime));
        }
        //
        #endregion
        //
        internal void insertObject(string userid, string otpat, string otp)
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
                cmd.CommandText = "usp_insert_tbl_userlog";
                cmd.Parameters.Add(mc.getPObject("@UserId", userid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@LoginAt", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@otpat", otpat, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@OTP", otp, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@IsLogout", "0", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@LogoutAt", DateTime.Now.ToString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserLogDAL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setUserLogToLogout(string userid)
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
                //with clearing of users-variable-tables
                cmd.CommandText = "usp_set_userlogtologout";
                cmd.Parameters.Add(mc.getPObject("@LogoutAt", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@UserId", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserLogDAL", "setUserLogToLogout", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal DataSet getUserLogData(string userid, DateTime dtfrom, DateTime dtto)
        {
            //to be updated by erp v1
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_userlog";
            cmd.Parameters.Add(mc.getPObject("@UserId", userid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom.ToShortDateString()), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto.ToShortDateString()), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal void deleteUserLog(string recid)
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
                cmd.CommandText = "usp_delete_tbl_userlog";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserAdminDAL", "deleteUserLog", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
    }
}