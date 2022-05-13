using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UserOptionBLL : DbContext
    {
        //
        //internal DbSet<CityMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static UserOptionBLL Instance
        {
            get { return new UserOptionBLL(); }
        }
        //
        #region dml objects
        //
        internal void setUserOptionValue(string fieldname, string fieldvalue)
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
                cmd.CommandText = "usp_set_useroption_value";
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@fieldname", fieldname, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@fieldvalue", fieldvalue, DbType.String));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserOptionBLL", "setUserOptionValue", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal string getUserOptionValue(string fieldname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_useroption_value";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@fieldname", fieldname, DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //
        #endregion
        //
    }
}