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
    public class ObjectInfoBLL : DbContext
    {
        //
        //internal DbSet<ObjectInfoMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ObjectInfoMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@EntryId", dbobject.EntryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SlNo", dbobject.SlNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RptName", dbobject.RptName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PatternAs", dbobject.PatternAs.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SPName", dbobject.SPName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RptDocName", dbobject.RptDocName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BllMethod", dbobject.BllMethod.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LocationMenu", dbobject.LocationMenu.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RptLink", dbobject.RptLink.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
        }
        //
        private List<ObjectInfoMdl> createObjectList(DataSet ds)
        {
            List<ObjectInfoMdl> objlist = new List<ObjectInfoMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectInfoMdl objmdl = new ObjectInfoMdl();
                objmdl.ObjectId = Convert.ToInt32(dr["ObjectId"].ToString());
                objmdl.EntryId = Convert.ToInt32(dr["EntryId"].ToString());
                objmdl.EntryName = dr["EntryName"].ToString();//d
                objmdl.SlNo = Convert.ToInt32(dr["SlNo"].ToString());
                objmdl.RptName = dr["RptName"].ToString();
                objmdl.PatternAs = dr["PatternAs"].ToString();
                objmdl.SPName = dr["SPName"].ToString();
                objmdl.RptDocName = dr["RptDocName"].ToString();
                objmdl.BllMethod = dr["BllMethod"].ToString();
                objmdl.LocationMenu = dr["LocationMenu"].ToString();
                objmdl.RptLink = dr["RptLink"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string rptname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_rptname";
            cmd.Parameters.Add(mc.getPObject("@rptname", rptname, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate object entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(ObjectInfoMdl dbobject)
        {
            if (dbobject.EntryId == 0)
            {
                Message = "Group Not Selected!";
                return false;
            }
            if (dbobject.PatternAs == null)
            {
                dbobject.PatternAs = "No Reference";
            }
            if (dbobject.RptDocName == null)
            {
                dbobject.RptDocName = "Not Entered";
            }
            if (dbobject.BllMethod == null)
            {
                dbobject.BllMethod = "Not Entered";
            }
            if (dbobject.LocationMenu == null)
            {
                dbobject.LocationMenu = "Not Entered";
            }
            if (dbobject.RptLink == null)
            {
                dbobject.RptLink = "Not Entered";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "Not Entered";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(ObjectInfoMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.RptName) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_objectinfo";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Object Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ObjectInfoBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ObjectInfoMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_objectinfo";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@ObjectId", dbobject.ObjectId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Object Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_objectinfo") == true)
                {
                    Message = "Duplicate object entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ObjectInfoBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int objectid)
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
                //cmd.CommandText = "usp_delete_tbl_objectinfo";
                //cmd.Parameters.Add(mc.getPObject("@objectid", objectid, DbType.Int32));
                //cmd.ExecuteNonQuery();
                Result = true;
                //Message = "Object Record Deleted Successfully";
                Message = "Not to Delete";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ObjectInfoBLL", "deleteObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //    
        #region for admin
        //
        internal ObjectInfoMdl searchObject(int objectid)
        {
            DataSet ds = new DataSet();
            ObjectInfoMdl dbobject = new ObjectInfoMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_objectinfo";
            cmd.Parameters.Add(mc.getPObject("@objectid", objectid, DbType.Int32));
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
        internal DataSet getObjectData(int entryid, string sorton)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_objectinfo";
            cmd.Parameters.Add(mc.getPObject("@entryid", entryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@sorton", sorton, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectInfoMdl> getObjectList(int entryid, string sorton)
        {
            DataSet ds = getObjectData(entryid, sorton);
            return createObjectList(ds);
        }
        //
        internal DataSet getObjectGroupData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_objectgroup";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemGroupMdl> getObjectGroupList()
        {
            DataSet ds = getObjectGroupData();
            List<ItemGroupMdl> objlist = new List<ItemGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ItemGroupMdl objmdl = new ItemGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["EntryId"].ToString());
                objmdl.GroupName = dr["EntryName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
        #region for users
        //
        internal DataSet getObjectDataForUsers(int entryid, string sorton)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_objectinfo_list";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@entryid", entryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@sorton", sorton, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectInfoMdl> getObjectListForUsers(int entryid, string sorton)
        {
            DataSet ds = getObjectDataForUsers(entryid, sorton);
            return createObjectList(ds);
        }
        //
        internal DataSet getObjectGroupDataForUsers()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_objectgroup_for_list";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemGroupMdl> getObjectGroupListForUsers()
        {
            DataSet ds = getObjectGroupDataForUsers();
            List<ItemGroupMdl> objlist = new List<ItemGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ItemGroupMdl objmdl = new ItemGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["EntryId"].ToString());
                objmdl.GroupName = dr["EntryName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}