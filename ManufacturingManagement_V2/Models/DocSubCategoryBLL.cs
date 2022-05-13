using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class DocSubCategoryBLL : DbContext
    {
        //
        //internal DbSet<DocSubCategoryMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static DocSubCategoryBLL Instance
        {
            get { return new DocSubCategoryBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, DocSubCategoryMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@SubCategoryName", dbobject.SubCategoryName.Trim(), DbType.String));
        }
        //
        private List<DocSubCategoryMdl> createObjectList(DataSet ds)
        {
            List<DocSubCategoryMdl> objlist = new List<DocSubCategoryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DocSubCategoryMdl objmdl = new DocSubCategoryMdl();
                objmdl.SubCategoryId = Convert.ToInt32(dr["SubCategoryId"].ToString());
                objmdl.SubCategoryName = dr["SubCategoryName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string subcategoryname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_documentsubcategory";
            cmd.Parameters.Add(mc.getPObject("@subcategoryname", subcategoryname.Trim(), DbType.String));
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
        internal void insertObject(DocSubCategoryMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.SubCategoryName) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_documentsubcategory";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string subcategoryid = mc.getRecentIdentityValue(cmd, dbTables.tbl_documentsubcategory, "depid");
                mc.setEventLog(cmd, dbTables.tbl_documentsubcategory, subcategoryid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("DocSubCategoryBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(DocSubCategoryMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_documentsubcategory";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@SubCategoryId", dbobject.SubCategoryId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_documentsubcategory, dbobject.SubCategoryId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_documentsubcategory") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("DocSubCategoryBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int subcategoryid)
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
                cmd.CommandText = "usp_delete_tbl_documentsubcategory";
                cmd.Parameters.Add(mc.getPObject("@subcategoryid", subcategoryid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_documentsubcategory, subcategoryid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("DocSubCategoryBLL", "deleteObject", ex.Message);
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
        internal DocSubCategoryMdl searchObject(int subcategoryid)
        {
            DataSet ds = new DataSet();
            DocSubCategoryMdl dbobject = new DocSubCategoryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_documentsubcategory";
            cmd.Parameters.Add(mc.getPObject("@subcategoryid", subcategoryid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_documentsubcategory";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<DocSubCategoryMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}