using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ItemGroupBLL : DbContext
    {
        //
        internal DbSet<ItemGroupMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static ItemGroupBLL Instance
        {
            get { return new ItemGroupBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ItemGroupMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@GroupName", dbobject.GroupName.Trim(), DbType.String));
        }
        //
        private List<ItemGroupMdl> createObjectList(DataSet ds)
        {
            List<ItemGroupMdl> storeitems = new List<ItemGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                ItemGroupMdl objmdl = new ItemGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                objmdl.GroupName = dr["GroupName"].ToString();
                storeitems.Add(objmdl);
            }
            return storeitems;
        }
        //
        private bool isFoundItemGroup(string groupname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_itemgroup";
            cmd.Parameters.Add(mc.getPObject("@groupname", groupname, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate item group not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(ItemGroupMdl dbobject)
        {
            //if (dbobject.GroupName == null)
            //{
            //    Message = "Invalid group name!";
            //}
            //if (dbobject.Remarks == null)
            //{
            //    dbobject.Remarks = "";
            //}
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(ItemGroupMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundItemGroup(dbobject.GroupName) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_itemgroup";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string groupid = mc.getRecentIdentityValue(cmd, dbTables.tbl_itemgroup, "groupid");
                mc.setEventLog(cmd, dbTables.tbl_item, groupid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ItemGroupBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ItemGroupMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_itemgroup";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@GroupId", dbobject.GroupId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_itemgroup, dbobject.GroupId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_itemgroup") == true)
                {
                    Message = "Duplicate item group not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ItemGroupBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int groupid)
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
                cmd.CommandText = "usp_delete_tbl_itemgroup";
                cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_itemgroup, groupid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ItemGroupBLL", "deleteObject", ex.Message);
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
        internal ItemGroupMdl searchObject(int groupid)
        {
            DataSet ds = new DataSet();
            ItemGroupMdl dbobject = new ItemGroupMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_itemgroup";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_itemgroup";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemGroupMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal List<ItemGroupMdl> getItemGroupListForCompany()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_itemgroup_for_company";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        internal List<CollabCategoryMdl> getCollabCategoryList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_collabcategory";//from v1
            mc.fillFromDatabase(ds, cmd);
            List<CollabCategoryMdl> objlist = new List<CollabCategoryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CollabCategoryMdl objmdl = new CollabCategoryMdl();
                objmdl.CategoryId = Convert.ToInt32(dr["CategoryId"].ToString());
                objmdl.CategoryName = dr["CategoryName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}