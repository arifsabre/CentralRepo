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
    public class BomBLL : DbContext
    {
        //
        //internal DbSet<BomMdl> boms { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, BomMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@FgItemId", dbobject.FgItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RmItemId", dbobject.RmItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RmQty", dbobject.RmQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@WasteQty", dbobject.WasteQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks, DbType.String));
        }
        //
        private List<BomMdl> createObjectList(DataSet ds)
        {
            List<BomMdl> boms = new List<BomMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                BomMdl objmdl = new BomMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.FgItemId = Convert.ToInt32(dr["FgItemId"].ToString());
                objmdl.RmItemId = Convert.ToInt32(dr["RmItemId"].ToString());
                objmdl.RmQty = Convert.ToDouble(dr["RmQty"].ToString());
                objmdl.WasteQty = Convert.ToDouble(dr["WasteQty"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.RevNo = Convert.ToInt32(dr["RevNo"].ToString());
                objmdl.RevDate = Convert.ToDateTime(dr["RevDate"].ToString());
                objmdl.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                //d
                objmdl.FgUnit = Convert.ToInt32(dr["FgUnit"].ToString());
                objmdl.RmUnit = Convert.ToInt32(dr["RmUnit"].ToString());
                objmdl.FgItemCode = dr["FGItemCode"].ToString();
                objmdl.FgItemName = dr["FGItemName"].ToString();
                objmdl.ItemTypeDesc = dr["ItemTypeDesc"].ToString();
                objmdl.RmItemCode = dr["RMItemCode"].ToString();
                objmdl.RmItemName = dr["RMItemName"].ToString();
                objmdl.FgUnitName = dr["FgUnitName"].ToString();
                objmdl.RmUnitName = dr["RmUnitName"].ToString();
                //end-d
                boms.Add(objmdl);
            }
            return boms;
        }
        //
        private bool isAlreadyFound(int fgitemid, int rmitemid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_bom";
            cmd.Parameters.Add(mc.getPObject("@fgitemid", fgitemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@rmitemid", rmitemid, DbType.Int32));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate or reverse entry is not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(BomMdl dbobject)
        {
            if (dbobject.FgItemId == dbobject.RmItemId)
            {
                Message = "Finished and raw material must not be same item!";
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
        internal void insertObject(BomMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.FgItemId, dbobject.RmItemId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_bom";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_bom, "recid");
                mc.setEventLog(cmd, dbTables.tbl_item, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("BomBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(BomMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_bom";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RevisionUpdate", dbobject.RevisionUpdate, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RevDate", dbobject.RevDate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_item, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_bom") == true)
                {
                    Message = "Duplicate or reverse entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("BomBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_bom";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_item, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("BomBLL", "deleteObject", ex.Message);
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
        internal BomMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            BomMdl dbobject = new BomMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_bom";
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
        internal DataSet getObjectData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_bom";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getItemsDefinedInBOM(int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_items_defined_in_bom";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getOtherThanFinishedItemsForBOM(int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_other_than_finished_items_for_bom";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<BomMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }

        internal DataSet getLatestBomRevisionDetail(int fgitemid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_latest_bom_revision_detail";
            cmd.Parameters.Add(mc.getPObject("@fgitemid", fgitemid, DbType.Int32));
            mc.fillFromDatabase(ds,cmd);
            return ds;
        }
        //

        internal DataSet getMaterialRequirementReportHtml(int itemid, double prdqty)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_bom_requirement_list";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@prdqty", prdqty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}