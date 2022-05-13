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
    public class TenderDispatchBLL : DbContext
    {
        //
        //internal DbSet<TenderDispatchMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, TenderDispatchMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@TenderId", dbobject.TenderId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DelvDate", dbobject.DelvDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
        }
        //
        private List<TenderDispatchMdl> createObjectList(DataSet ds)
        {
            List<TenderDispatchMdl> objlist = new List<TenderDispatchMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TenderDispatchMdl objmdl = new TenderDispatchMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TenderNo = dr["TenderNo"].ToString();//d
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.DelvDate = Convert.ToDateTime(dr["DelvDate"].ToString());
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();//d
                objmdl.Remarks = dr["Remarks"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(TenderDispatchMdl dbobject)
        {
            if (dbobject.TenderId == 0)
            {
                Message = "Tender not selected!";
                return false;
            }
            if (dbobject.ItemId == 0)
            {
                Message = "Item not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.DelvDate) == false)
            {
                Message = "Invalid DP date!";
                return false;
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
        internal void insertObject(TenderDispatchMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_tenderdispatch";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_tenderdispatch, "recid");
                mc.setEventLog(cmd, dbTables.tbl_tenderdispatch, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("TenderDispatchBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(TenderDispatchMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_tenderdispatch";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_tenderdispatch, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("TenderDispatchBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_tenderdispatch";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_tenderdispatch, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("TenderDispatchBLL", "deleteObject", ex.Message);
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
        internal TenderDispatchMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            TenderDispatchMdl dbobject = new TenderDispatchMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_tenderdispatch";
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
        internal DataSet getObjectData(int itemid, int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tenderdispatch";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<TenderDispatchMdl> getObjectList(int itemid,int tenderid)
        {
            DataSet ds = getObjectData(itemid, tenderid);
            return createObjectList(ds);
        }
        //
        internal List<TenderItemsMdl> getTenderItemsList(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tender_items";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<TenderItemsMdl> objlist = new List<TenderItemsMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TenderItemsMdl objmdl = new TenderItemsMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());//d
                objmdl.OurQty = Convert.ToDouble(dr["OurQty"].ToString());//d
                objmdl.UnitName = dr["UnitName"].ToString();//d
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.DispCount = Convert.ToInt32(dr["DispCount"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}