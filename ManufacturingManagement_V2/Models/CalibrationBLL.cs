using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    /// <summary>
    /// dbprc : Calibration_SP
    /// </summary>
    public class CalibrationBLL : DbContext
    {
        //
        //internal DbSet<CalibrationMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, CalibrationMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@ImteId", dbobject.ImteId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CalibDate", dbobject.CalibDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CertificateNo", dbobject.CertificateNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CertifiedBy", dbobject.CertifiedBy.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NextCalibDate", dbobject.NextCalibDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@IsTested", dbobject.IsTested, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TestRecId", dbobject.TestRecId, DbType.Int32));
        }
        //
        private List<CalibrationMdl> createObjectList(DataSet ds)
        {
            List<CalibrationMdl> objlist = new List<CalibrationMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CalibrationMdl objmdl = new CalibrationMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.ImteId = Convert.ToInt32(dr["ImteId"].ToString());
                objmdl.CalibDate = Convert.ToDateTime(dr["CalibDate"].ToString());
                objmdl.CertificateNo = dr["CertificateNo"].ToString();
                objmdl.CertifiedBy = dr["CertifiedBy"].ToString();
                objmdl.NextCalibDate = Convert.ToDateTime(dr["NextCalibDate"].ToString());
                objmdl.IsTested = Convert.ToBoolean(dr["IsTested"].ToString());
                objmdl.TestRecId = Convert.ToInt32(dr["TestRecId"].ToString());
                //d
                objmdl.IdNo = dr["IdNo"].ToString();
                objmdl.ImteTypeId = Convert.ToInt32(dr["ImteTypeId"].ToString());
                objmdl.ImteTypeName = dr["ImteTypeName"].ToString();
                objmdl.ImteRange = dr["ImteRange"].ToString();
                objmdl.Location = dr["Location"].ToString();
                objmdl.IsInUse = Convert.ToBoolean(dr["IsInUse"].ToString());
                //--d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(CalibrationMdl dbobject)
        {
            if (dbobject.ImteId == 0)
            {
                Message = "Id No not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.CalibDate) == false)
            {
                Message = "Invalid calibration date!";
                return false;
            }
            if (dbobject.CertificateNo == null)
            {
                Message = "Certificate No not entered!";
                return false;
            }
            if (dbobject.CertifiedBy == null)
            {
                Message = "Certified By not entered!";
                return false;
            }
            if (mc.isValidDate(dbobject.NextCalibDate) == false)
            {
                Message = "Invalid next calibration date!";
                return false;
            }
            return true;
        }
        //
        private bool isAlreadyFound(DateTime calibdate, int imteid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_calibration";
            cmd.Parameters.Add(mc.getPObject("@calibdate", calibdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
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
        internal void insertObject(CalibrationMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.CalibDate,dbobject.ImteId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_calibration";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_calibration, "recid");
                mc.setEventLog(cmd, dbTables.tbl_calibration, recid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CalibrationBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void insertCalibrationForDamage(CalibrationMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.CalibDate, dbobject.ImteId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_calibration_for_damage";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_calibration, "recid");
                mc.setEventLog(cmd, dbTables.tbl_calibration, recid, "Inserted Damage");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CalibrationBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(CalibrationMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_calibration";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_calibration, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_calibration") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("CalibrationBLL", "updateObject", ex.Message);
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
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_calibration";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                int x = cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_calibration, recid.ToString(), "Deleted");
                trn.Commit();
                if (x == -1)
                {
                    Message = "Record is in use, so it can not be deleted!";
                }
                else if (x == 1)
                {
                    Message = "Record Deleted";
                }
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CalibrationBLL", "deleteObject", ex.Message);
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
        internal CalibrationMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            CalibrationMdl dbobject = new CalibrationMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_calibration";
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
        internal DataSet getObjectData(int imtetypeid, int imteid, DateTime dtfrom, DateTime dtto)
        {
            //--[100044]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_calibration_history";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CalibrationMdl> getObjectList(int imtetypeid, int imteid, DateTime dtfrom, DateTime dtto)
        {
            DataSet ds = getObjectData(imtetypeid, imteid, dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet getCalibrationAlertData(int imtetypeid, int imteid, DateTime asondate)
        {
            //[100043]--Set 1: 30 Days
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_calibration_alert";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@asondate", asondate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCalibrationAlertDataV2(int imtetypeid, int imteid, DateTime asondate)
        {
            //[100043]--Set 1: 30 Days for all Companies/by User
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_calibration_alert_v2";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@asondate", asondate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CalibrationMdl> getCalibrationAlertList(int imtetypeid, int imteid, DateTime asondate)
        {
            DataSet ds = getCalibrationAlertData(imtetypeid, imteid, asondate);
            return createObjectList(ds);
        }
        #endregion
        //
    }
}