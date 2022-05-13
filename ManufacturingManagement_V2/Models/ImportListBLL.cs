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
    public class ImportListBLL : DbContext
    {
        //
        //internal DbSet<ImportListMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ImportListMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@ImpDate", dbobject.ImpDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
        }
        //
        private List<ImportListMdl> createObjectList(DataSet ds)
        {
            List<ImportListMdl> objlist = new List<ImportListMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ImportListMdl objmdl = new ImportListMdl();
                objmdl.ImpId = Convert.ToInt32(dr["ImpId"].ToString());
                objmdl.ImpDate = Convert.ToDateTime(dr["ImpDate"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.UnitName = dr["UnitName"].ToString();//d
                objmdl.Remarks = dr["Remarks"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<ImportListMdl> createObjectList_Projection(DataSet ds)
        {
            List<ImportListMdl> objlist = new List<ImportListMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ImportListMdl objmdl = new ImportListMdl();
                objmdl.ImpId = Convert.ToInt32(dr["ImpId"].ToString());
                objmdl.ImpDate = Convert.ToDateTime(dr["ImpDate"].ToString());
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.CurrentStock = Convert.ToDouble(dr["CurrentStock"].ToString());
                objmdl.TenderQty = Convert.ToDouble(dr["TenderQty"].ToString());
                objmdl.OpeningStock = Convert.ToDouble(dr["OpeningStock"].ToString());
                objmdl.TotalStock = Convert.ToDouble(dr["TotalStock"].ToString());
                objmdl.DelvQty = Convert.ToDouble(dr["DelvQty"].ToString());
                objmdl.BalanceQty = Convert.ToDouble(dr["BalanceQty"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(ImportListMdl dbobject)
        {
            if (mc.isValidDate(dbobject.ImpDate) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            if (dbobject.ItemId == 0)
            {
                Message = "Item not selected!";
                return false;
            }
            if (dbobject.Qty <= 0)
            {
                Message = "Invalid Quantity!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            return true;
        }
        //
        private bool isAlreadyFound(DateTime impdate,int itemid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_importlist";
            cmd.Parameters.Add(mc.getPObject("@impdate", impdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
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
        internal void insertObject(ImportListMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.ImpDate,dbobject.ItemId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_importlist";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string impid = mc.getRecentIdentityValue(cmd, dbTables.tbl_importlist, "impid");
                mc.setEventLog(cmd, dbTables.tbl_importlist, impid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ImportListBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ImportListMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_importlist";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@impid", dbobject.ImpId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_importlist, dbobject.ImpId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_importlist") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ImportListBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void saveImportList(ImportListMdl dbobject)
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
                cmd.CommandText = "usp_save_importlist";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@impid", dbobject.ImpId, DbType.Int32));
                cmd.ExecuteNonQuery();
                string impid = dbobject.ImpId.ToString();
                if (dbobject.ImpId == 0)
                {
                    impid = mc.getRecentIdentityValue(cmd, dbTables.tbl_importlist, "impid");
                }
                mc.setEventLog(cmd, dbTables.tbl_importlist, impid, "Edited for Plan");
                Result = true;
                Message = "Record Edited Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ImportListBLL", "saveImportList", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int impid)
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
                cmd.CommandText = "usp_delete_tbl_importlist";
                cmd.Parameters.Add(mc.getPObject("@impid", impid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_importlist, impid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ImportListBLL", "deleteObject", ex.Message);
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
        internal ImportListMdl searchObject(int impid)
        {
            DataSet ds = new DataSet();
            ImportListMdl dbobject = new ImportListMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_importlist";
            cmd.Parameters.Add(mc.getPObject("@impid", impid, DbType.Int32));
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
        internal DataSet getObjectData(int itemid, string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_importlist";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ImportListMdl> getObjectList(int itemid, string dtfrom, string dtto)
        {
            DataSet ds = getObjectData(itemid, dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal List<ImportListMdl> getPOProjectionImportList(int itemid, string dtfrom, string dtto, string finyear)
        {
            //[100100]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_poprojection_import_list";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList_Projection(ds);
        }
        //
        #endregion
        //
    }
}