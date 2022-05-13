using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MinimumWageBLL : DbContext
    {
        //
        internal DbSet<MinimumWageMdl> MinimumWages { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static QualificationBLL Instance
        {
            get { return new QualificationBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, MinimumWageMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@AttYear", dbobject.AttYear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttMonth", dbobject.AttMonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Skilled", dbobject.Skilled, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SemiSkilled", dbobject.SemiSkilled, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnSkilled", dbobject.UnSkilled, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BonusPer", dbobject.BonusPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompCode", dbobject.CompCode, DbType.Int16));
        }
        //
        private List<MinimumWageMdl> createObjectList(DataSet ds)
        {
            List<MinimumWageMdl> objlst = new List<MinimumWageMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MinimumWageMdl objmdl = new MinimumWageMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.AttYear = Convert.ToInt32(dr["AttYear"].ToString());
                objmdl.AttMonth = Convert.ToInt32(dr["AttMonth"].ToString());
                objmdl.Skilled = Convert.ToDouble(dr["Skilled"].ToString());
                objmdl.SemiSkilled = Convert.ToDouble(dr["SemiSkilled"].ToString());
                objmdl.UnSkilled = Convert.ToDouble(dr["UnSkilled"].ToString());
                objmdl.BonusPer = Convert.ToDouble(dr["BonusPer"].ToString());
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.CmpName = dr["CmpName"].ToString();//d
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private bool isAlreadyFound(int attmonth,int attyear, int compcode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_minwage";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(MinimumWageMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.AttMonth,dbobject.AttYear, dbobject.CompCode) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_minwage";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_minwage, dbobject.AttMonth.ToString()+"-"+dbobject.AttYear.ToString(), "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("MinimumWageBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(MinimumWageMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_minwage";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_minwage, dbobject.AttMonth.ToString() + "-" + dbobject.AttYear.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_minwage") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("MinimumWageBLL", "updateObject", ex.Message);
                }
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
        internal MinimumWageMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            MinimumWageMdl dbobject = new MinimumWageMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_minwage";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal MinimumWageMdl getMonthlyMinimumWage(int attmonth,int attyear)
        {
            DataSet ds = new DataSet();
            MinimumWageMdl dbobject = new MinimumWageMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_monthly_minwage";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_minwage";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<MinimumWageMdl> getObjectList(int compcode)
        {
            DataSet ds = getObjectData(compcode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}