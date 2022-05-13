using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FormValueBLL : DbContext
    {
        //
        internal DbSet<FormValueMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static FormValueBLL Instance
        {
            get { return new FormValueBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, FormValueMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.CompCode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", dbobject.FinYear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NetProfit", dbobject.NetProfit, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PrvYrBonusAmt", dbobject.PrvYrBonusAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DepreciationAmt", dbobject.DepreciationAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RateOfDevRebate", dbobject.RateOfDevRebate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CapitalAmtPer", dbobject.CapitalAmtPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CapitalAmount", dbobject.CapitalAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RevSurpAmtPer", dbobject.RevSurpAmtPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RevSurpAmount", dbobject.RevSurpAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SurpAllocPer", dbobject.SurpAllocPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BonusPer", dbobject.BonusPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BonusAmount", dbobject.BonusAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NoOfMales", dbobject.NoOfMales, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NoOfFemales", dbobject.NoOfFemales, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NoOfWDays", dbobject.NoOfWDays, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PayableBonusAmt", dbobject.PayableBonusAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PaymentDate", dbobject.PaymentDate.ToShortDateString(), DbType.Date));
        }
        //
        private List<FormValueMdl> createObjectList(DataSet ds)
        {
            List<FormValueMdl> listObj = new List<FormValueMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FormValueMdl objmdl = new FormValueMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.FinYear = dr["FinYear"].ToString();
                objmdl.NetProfit = Convert.ToDouble(dr["NetProfit"].ToString());
                objmdl.PrvYrBonusAmt = Convert.ToDouble(dr["PrvYrBonusAmt"].ToString());
                objmdl.DepreciationAmt = Convert.ToDouble(dr["DepreciationAmt"].ToString());
                objmdl.RateOfDevRebate = Convert.ToDouble(dr["RateOfDevRebate"].ToString());
                objmdl.CapitalAmtPer = Convert.ToDouble(dr["CapitalAmtPer"].ToString());
                objmdl.CapitalAmount = Convert.ToDouble(dr["CapitalAmount"].ToString());
                objmdl.RevSurpAmtPer = Convert.ToDouble(dr["RevSurpAmtPer"].ToString());
                objmdl.RevSurpAmount = Convert.ToDouble(dr["RevSurpAmount"].ToString());
                objmdl.SurpAllocPer = Convert.ToDouble(dr["SurpAllocPer"].ToString());
                objmdl.BonusPer = Convert.ToDouble(dr["BonusPer"].ToString());
                objmdl.BonusAmount = Convert.ToDouble(dr["BonusAmount"].ToString());
                objmdl.CompanyName = dr["CompanyName"].ToString();
                objmdl.FinYearToDate = Convert.ToDateTime(dr["FinYearToDate"].ToString());
                objmdl.NoOfMales = Convert.ToInt32(dr["NoOfMales"].ToString());
                objmdl.NoOfFemales = Convert.ToInt32(dr["NoOfFemales"].ToString());
                objmdl.NoOfWDays = Convert.ToInt32(dr["NoOfWDays"].ToString());
                objmdl.PayableBonusAmt = Convert.ToDouble(dr["PayableBonusAmt"].ToString());
                objmdl.PaymentDate = Convert.ToDateTime(dr["PaymentDate"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(FormValueMdl dbobject)
        {
            //if (dbobject.ContactPerson == null)
            //{
            //    dbobject.ContactPerson = "";
            //}
            return true;
        }
        //
        private bool isFoundFormValue(int compcode, string finyear)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_formvalue";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
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
        internal void insertObject(FormValueMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundFormValue(dbobject.CompCode,dbobject.FinYear) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_formvalue";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_formvalue, "recid");
                mc.setEventLog(cmd, dbTables.tbl_formvalue, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FormValueBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(FormValueMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_formvalue";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_formvalue, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_formvalue") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("FormValueBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int recid)
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
                cmd.CommandText = "usp_delete_tbl_formvalue";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_formvalue, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FormValueBLL", "deleteObject", ex.Message);
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
        internal FormValueMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            FormValueMdl dbobject = new FormValueMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_formvalue";
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
        internal DataSet getObjectData(int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_formvalue";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<FormValueMdl> getObjectList(int ccode)
        {
            DataSet ds = getObjectData(ccode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}