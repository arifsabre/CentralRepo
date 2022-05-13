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
    public class BankGuaranteeBLL : DbContext
    {
        //
        //internal DbSet<BankGuaranteeMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, BankGuaranteeMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@BGNumber", dbobject.BGNumber.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BGDate", dbobject.BGDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Validity", dbobject.Validity.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@ExtValidity", dbobject.ExtValidity.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@StatusId", dbobject.StatusId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BGType", dbobject.BGType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SdBgAmount", dbobject.SdBgAmount, DbType.Double));//to tender
            cmd.Parameters.Add(mc.getPObject("@TenderId", dbobject.TenderId, DbType.Int32));
        }
        //
        private List<BankGuaranteeMdl> createObjectList(DataSet ds)
        {
            List<BankGuaranteeMdl> objlist = new List<BankGuaranteeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TenderBLL tenderBLL = new TenderBLL();
                BankGuaranteeMdl objmdl = new BankGuaranteeMdl();
                objmdl.Railway = dr["RailwayName"].ToString();//d
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TenderNo = dr["TenderNo"].ToString();//d
                if (dr.Table.Columns.Contains("PODetail"))
                {
                    objmdl.PODetail = dr["PODetail"].ToString();//d
                }
                objmdl.OpeningDate = Convert.ToDateTime(dr["OpeningDate"].ToString());//d
                objmdl.BGNumber = dr["BGNumber"].ToString();
                objmdl.SdBgAmount = Convert.ToDouble(dr["SdBgAmount"].ToString());
                objmdl.BGDate = Convert.ToDateTime(dr["BGDate"].ToString());
                objmdl.Validity = Convert.ToDateTime(dr["Validity"].ToString());
                objmdl.ExtValidity = Convert.ToDateTime(dr["ExtValidity"].ToString());
                objmdl.StatusId = Convert.ToInt32(dr["StatusId"].ToString());
                objmdl.StatusName = dr["StatusName"].ToString();//d
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.BGType = dr["BGType"].ToString();
                objmdl.BGTypeName = dr["BGTypeName"].ToString();//d
                objmdl.AalCo = dr["AalCo"].ToString();
                objmdl.LoaNumber = dr["LoaNumber"].ToString();//d
                objmdl.LoaDate = Convert.ToDateTime(dr["LoaDate"].ToString());//d
                objmdl.LoaDetail = tenderBLL.getLoaDetail(objmdl.TenderId, objmdl.LoaNumber, mc.getStringByDate(objmdl.LoaDate), objmdl.AalCo);
                if (dr.Table.Columns.Contains("HistoryAvailable"))
                {
                    objmdl.HistoryAvailable = Convert.ToBoolean(dr["HistoryAvailable"].ToString());//d
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isDuplicateBGNumber(int tenderid, string bgnumber)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_is_duplicate_bgnumber";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@bgnumber", bgnumber.Trim(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate BG Number entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(BankGuaranteeMdl dbobject)
        {
            if (dbobject.TenderId == 0)
            {
                Message = "Tender Number not selected!";
                return false;
            }
            if (dbobject.BGNumber == null)
            {
                dbobject.BGNumber = "";
            }
            if (mc.isValidDate(dbobject.BGDate) == false)
            {
                Message = "Invalid BG date!";
                return false;
            }
            if (mc.isValidDate(dbobject.Validity) == false)
            {
                Message = "Invalid validity!";
                return false;
            }
            if (mc.isValidDate(dbobject.ExtValidity) == false)
            {
                Message = "Invalid extended validity!";
                return false;
            }
            if (dbobject.ExtValidity < dbobject.Validity)
            {
                dbobject.ExtValidity = dbobject.Validity;
            }
            if(dbobject.StatusId != 50)
            {
                //applicable
                if (dbobject.SdBgAmount == 0)
                {
                    Message = "SD/BG Amount must not be zero!";
                    return false;
                }
            }
            else if (dbobject.StatusId == 50)
            {
                //not applicable
                dbobject.SdBgAmount = 0;
                dbobject.BGNumber = "";
                dbobject.BGType = "n";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(BankGuaranteeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (dbobject.BGNumber.Length > 0)
            {
                if (isDuplicateBGNumber(dbobject.TenderId, dbobject.BGNumber.Trim()) == true) { return; };
            }
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
                cmd.CommandText = "usp_insert_tbl_bankguarantee";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_bankguarantee, dbobject.TenderId.ToString(), "BG Inserted");
                trn.Commit();
                Result = true;
                Message = "BG Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("pk_tbl_bankguarantee") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("BankGuaranteeBLL", "insertObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(BankGuaranteeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (dbobject.BGNumber.Length > 0)
            {
                if (isDuplicateBGNumber(dbobject.TenderId, dbobject.BGNumber.Trim()) == true) { return; };
            }
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
                cmd.CommandText = "usp_update_tbl_bankguarantee";
                dbobject.TenderId = dbobject.editTenderId;
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@HistoryOpt", dbobject.sendToHistory, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_bankguarantee, dbobject.TenderId.ToString(), "BG Updated");
                trn.Commit();
                Result = true;
                Message = "BG Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BankGuaranteeBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int tenderid)
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
                cmd.CommandText = "usp_delete_tbl_bankguarantee";
                cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_bankguarantee, tenderid.ToString(), "BG Deleted");
                Result = true;
                Message = "BG Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("BankGuaranteeBLL", "deleteObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateBGHistory(BankGuaranteeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_bghistory";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_bghistory, dbobject.TenderId.ToString()+"-"+dbobject.BGNumber, "BG History Updated");
                Result = true;
                Message = "BG History Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("pk_tbl_bghistory") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("BankGuaranteeBLL", "updateBGHistory", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteBGHistory(int tenderid, string bgnumber)
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
                cmd.CommandText = "usp_delete_tbl_bghistory";
                cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@bgnumber", bgnumber, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_bghistory, tenderid.ToString()+"-"+bgnumber, "BG History Deleted");
                Result = true;
                Message = "BG History Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("BankGuaranteeBLL", "deleteBGHistory", ex.Message);
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
        internal BankGuaranteeMdl searchObject(int tenderid)
        {
            DataSet ds = new DataSet();
            BankGuaranteeMdl dbobject = new BankGuaranteeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_bankguarantee";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
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
        internal string getPODetail(int tenderid)
        {
            DataSet ds = new DataSet();
            BankGuaranteeMdl dbobject = new BankGuaranteeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_podetail_for_bg";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        internal BankGuaranteeMdl getTenderDetail(int tenderid)
        {
            DataSet ds = new DataSet();
            BankGuaranteeMdl dbobject = new BankGuaranteeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_tender";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            TenderBLL tenderBll = new TenderBLL();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject.TenderId = tenderid;
                    dbobject.TenderNo = ds.Tables[0].Rows[0]["TenderNo"].ToString();
                    dbobject.LoaNumber = ds.Tables[0].Rows[0]["LoaNumber"].ToString();
                    dbobject.AalCo = ds.Tables[0].Rows[0]["AalCo"].ToString();
                    dbobject.LoaDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["LoaDate"].ToString());
                    dbobject.SdBgAmount = Convert.ToDouble(ds.Tables[0].Rows[0]["SdBgAmount"].ToString());
                    dbobject.DelvSchedule = ds.Tables[0].Rows[0]["DelvSchedule"].ToString();
                    dbobject.LoaDelvSchedule = ds.Tables[0].Rows[0]["LoaDelvSchedule"].ToString();
                    dbobject.RailwayName = ds.Tables[0].Rows[0]["RailwayName"].ToString();
                    dbobject.BGDate = DateTime.Now;
                    dbobject.Validity = DateTime.Now;
                    dbobject.ExtValidity = DateTime.Now;
                    dbobject.LoaDetail = tenderBll.getLoaDetail(dbobject.TenderId, dbobject.LoaNumber, mc.getStringByDate(dbobject.LoaDate), dbobject.AalCo);
                    dbobject.PODetail = getPODetail(tenderid);
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(int statusid, int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt32(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_bankguarantee";
            //cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@statusid", statusid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<BankGuaranteeMdl> getObjectList(int statusid, int ccode=0)
        {
            DataSet ds = getObjectData(statusid,ccode);
            return createObjectList(ds);
        }
        //
        internal DataSet getPurchaseNumbers(int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt32(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ponumbers";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBGStatusListData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_bgstatus";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemGroupMdl> getBGStatusList()
        {
            List<ItemGroupMdl> objlist = new List<ItemGroupMdl> { };
            DataSet ds = getBGStatusListData();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ItemGroupMdl objmdl = new ItemGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["StatusId"].ToString());
                objmdl.GroupName = dr["StatusName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #region BG History
        //
        internal DataSet getBGHistoryData(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_bghistory";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<BankGuaranteeMdl> getBGHistoryList(int tenderid)
        {
            DataSet ds = getBGHistoryData(tenderid);
            return createObjectList(ds);
        }
        //
        internal BankGuaranteeMdl searchBGHistory(int tenderid, string bgnumber)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_bghistory";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@bgnumber", bgnumber, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            BankGuaranteeMdl dbobject = new BankGuaranteeMdl();
            dbobject.TenderId = Convert.ToInt32(ds.Tables[0].Rows[0]["TenderId"].ToString());
            dbobject.TenderNo = ds.Tables[0].Rows[0]["TenderNo"].ToString();
            dbobject.BGNumber = ds.Tables[0].Rows[0]["BGNumber"].ToString();
            dbobject.BGDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BGDate"].ToString());
            dbobject.Validity = Convert.ToDateTime(ds.Tables[0].Rows[0]["Validity"].ToString());
            dbobject.ExtValidity = Convert.ToDateTime(ds.Tables[0].Rows[0]["ExtValidity"].ToString());
            dbobject.StatusId = Convert.ToInt32(ds.Tables[0].Rows[0]["StatusId"].ToString());
            dbobject.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
            dbobject.BGType = ds.Tables[0].Rows[0]["BGType"].ToString();
            dbobject.SdBgAmount = Convert.ToDouble(ds.Tables[0].Rows[0]["SdBgAmount"].ToString());
            return dbobject;
        }
        //
        #endregion BG History
        //
        #endregion
        //
    }
}