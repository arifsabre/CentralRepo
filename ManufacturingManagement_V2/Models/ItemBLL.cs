using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ItemBLL : DbContext
    {
        //
        internal DbSet<ItemMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static ItemBLL Instance
        {
            get { return new ItemBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ItemMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@ItemCode", dbobject.ItemCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ShortName", dbobject.ShortName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemName", dbobject.ItemName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SaleRate", dbobject.SaleRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PurchaseRate", dbobject.PurchaseRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Specification", dbobject.Specification.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Unit", dbobject.Unit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemType", dbobject.ItemType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@HSNCode", dbobject.HSNCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OldItemId", "0", DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@GroupId", dbobject.GroupId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@IsActive", dbobject.IsActive, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MsbQty", dbobject.MsbQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@LotSize", dbobject.LotSize, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@WarrantyDays", dbobject.WarrantyDays, DbType.Int32));
        }
        //
        private List<ItemMdl> createObjectList(DataSet ds)
        {
            List<ItemMdl> storeitems = new List<ItemMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                ItemMdl objmdl = new ItemMdl();
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ShortName = dr["ShortName"].ToString();
                objmdl.ItemName = dr["ItemName"].ToString();
                objmdl.SaleRate = Convert.ToDouble(dr["SaleRate"].ToString());
                objmdl.PurchaseRate = Convert.ToDouble(dr["PurchaseRate"].ToString());
                objmdl.Specification = dr["Specification"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.ItemType = dr["ItemType"].ToString();
                objmdl.HSNCode = dr["HSNCode"].ToString();
                objmdl.OldItemId = Convert.ToInt32(dr["OldItemId"].ToString());
                objmdl.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                objmdl.GroupName = dr["GroupName"].ToString();
                objmdl.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                objmdl.MsbQty = Convert.ToDouble(dr["MsbQty"].ToString());
                objmdl.LotSize = Convert.ToDouble(dr["LotSize"].ToString());
                objmdl.WarrantyDays = Convert.ToInt32(dr["WarrantyDays"].ToString());
                storeitems.Add(objmdl);
            }
            return storeitems;
        }
        //
        private bool isFoundItemCode(string itemcode, string itemtype, string itemid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_itemcode";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@itemcode", itemcode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate item code not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool isFoundShortName(string shortname, string itemtype, string itemid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_itemshortname";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@shortname", shortname, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate short name not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(ItemMdl dbobject)
        {
            if (dbobject.GroupId == 0)
            {
                dbobject.GroupId = 1;//note
            }
            if (dbobject.ShortName == null)
            {
                dbobject.ShortName = dbobject.ItemCode;
            }
            if (dbobject.ItemName == null)
            {
                dbobject.ItemName = dbobject.ItemCode;
            }
            if (dbobject.Specification == null)
            {
                dbobject.Specification = "";
            }
            if (dbobject.HSNCode == null)
            {
                dbobject.HSNCode = "";
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
        internal void insertObject(ItemMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundItemCode(dbobject.ItemCode,dbobject.ItemType,"0") == true) { return; };
            if (isFoundShortName(dbobject.ShortName, dbobject.ItemType, "0") == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_item";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string itemid = mc.getRecentIdentityValue(cmd, dbTables.tbl_item, "itemid");
                mc.setEventLog(cmd, dbTables.tbl_item, itemid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ItemBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ItemMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundItemCode(dbobject.ItemCode, dbobject.ItemType, dbobject.ItemId.ToString()) == true) { return; };
            if (isFoundShortName(dbobject.ShortName, dbobject.ItemType, dbobject.ItemId.ToString()) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                //check editable if not admin and having no permission
                if (objCookie.getLoginType() != 0 && mc.getPermission(Models.Entry.Masters_Updation_for_DI,permissionType.Add) == false)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_iseditable_item";
                    cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.ItemId, DbType.Int32));
                    if (Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection)) == false)
                    {
                        Message = "This record is in use, so it cannot be edited!";
                        return;
                    }
                }
                //update
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_item";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.ItemId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_item, dbobject.ItemId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ItemBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateItemUnit(int unitid, int itemid)
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
                cmd.CommandText = "usp_update_item_unit";
                cmd.Parameters.Add(mc.getPObject("@unitid", unitid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
                int x = cmd.ExecuteNonQuery();
                if (x == 1)
                {
                    mc.setEventLog(cmd, dbTables.tbl_item, itemid.ToString(), "Item Unit Updated");
                    Result = true;
                }
                else
                {
                    Message = "Item unit cannot be update!\r\nIt has been used in BOM definition or in stock transaction.";
                    Result = false;
                }
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemBLL", "updateItemUnit", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int itemid)
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
                cmd.CommandText = "usp_delete_tbl_item";
                cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_item, itemid.ToString(), "Delete");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ItemBLL", "deleteObject", ex.Message);
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
        internal ItemMdl searchObject(int itemid)
        {
            DataSet ds = new DataSet();
            ItemMdl dbobject = new ItemMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_item";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
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
        internal int getItemIdByItemCode(string itemcode)
        {
            DataSet ds = new DataSet();
            ItemMdl dbobject = new ItemMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_itemid_by_code";
            cmd.Parameters.Add(mc.getPObject("@itemcode", itemcode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            return Convert.ToInt32(mc.getFromDatabase(cmd));
        }
        //
        internal ItemMdl getObjectByItemCode(string itemcode)
        {
            DataSet ds = new DataSet();
            ItemMdl dbobject = new ItemMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_item_by_code";
            cmd.Parameters.Add(mc.getPObject("@itemcode", itemcode, DbType.String));
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
        internal DataSet getObjectData(int groupid = 0, string itemtype = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_item";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemtype", itemtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemMdl> getObjectList(int groupid, string itemtype)
        {
            DataSet ds = getObjectData(groupid, itemtype);
            return createObjectList(ds);
        }
        //
        internal DataSet getFinishedItemsList(int groupid, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_finished_items_list";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getStockItemsList(string itemtype, int groupid, int ccode=0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_stock_items_list";
            cmd.Parameters.Add(mc.getPObject("@itemtype", itemtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPOSaleItemsList(int groupid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_po_sale_items_list";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getVendorPOItemsList(int vendorid, int purchaseid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_vendor_po_item_list";
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getAllItemsListGroup(int groupid, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_all_items_list_group";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getAllItemList(int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_all_items_list";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getFinishedAndAssembledItems(int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_finished_and_assembled_items";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UnitMdl> getItemUnitList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_unit";
            mc.fillFromDatabase(ds, cmd);
            List<UnitMdl> units = new List<UnitMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UnitMdl objmdl = new UnitMdl();
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        internal DataSet getTransactionUnitData(int itemid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_txn_unit_list";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UnitMdl> getTransactionUnitList(int itemid)
        {
            DataSet ds = new DataSet();
            ds = getTransactionUnitData(itemid);
            List<UnitMdl> units = new List<UnitMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UnitMdl objmdl = new UnitMdl();
                objmdl.Unit = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        internal DataSet GetItemReportHtml(string itemtype, int groupid, string status, string hsncode, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_item_master_report";
            cmd.Parameters.Add(mc.getPObject("@itemtype", itemtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@status", status, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@hsncode", hsncode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}