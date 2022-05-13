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
    public class DocumentFileBLL : DbContext
    {
        //
        //internal DbSet<DocumentFileMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, DocumentFileMdl dbobject)
        {

            cmd.Parameters.Add(mc.getPObject("@DocumentTypeId", dbobject.DocumentTypeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Categoryid", dbobject.CategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SubCategoryid", dbobject.SubCategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", mc.getForSqlIntString(dbobject.ItemId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DocFileName", dbobject.DocFileName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FileDesc", dbobject.FileDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ModifyUser", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ModifyDate", dbobject.ModifyDate.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CompCode", mc.getForSqlIntString(dbobject.CCode.ToString()), DbType.Int16));
        }
        //
        private List<DocumentFileMdl> createObjectList(DataSet ds)
        {
            List<DocumentFileMdl> advances = new List<DocumentFileMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DocumentFileMdl objmdl = new DocumentFileMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                //objmdl.DocumentTypeId = Convert.ToInt32(dr["DocumentTypeId"].ToString());
                objmdl.CategoryId = Convert.ToInt32(dr["Categoryid"].ToString());
                objmdl.SubCategoryId = Convert.ToInt32(dr["SubCategoryid"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.CCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.DocFileName = dr["DocFileName"].ToString();
                objmdl.FileDesc = dr["FileDesc"].ToString();
                objmdl.CategoryName = dr["CategoryName"].ToString();
                objmdl.SubCategoryName = dr["SubCategoryName"].ToString();
                objmdl.ModifyDate = Convert.ToDateTime(dr["ModifyDate"].ToString());
                advances.Add(objmdl);
            }
            return advances;
        }
        //
        private bool checkSetValidModel(DocumentFileMdl dbobject)
        {
            if (dbobject.FileDesc == null)
            {
                dbobject.FileDesc = "";
            }
            dbobject.ModifyDate = DateTime.Now;//note
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        private bool isDocumentFileFound(DocumentFileMdl dbobject)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_documentfile";
            cmd.Parameters.Add(mc.getPObject("@DocumentTypeId", dbobject.DocumentTypeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CategoryId", dbobject.CategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SubCategoryId", dbobject.SubCategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", mc.getForSqlIntString(dbobject.ItemId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DocFileName", dbobject.DocFileName.Trim(), DbType.String));
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
        internal void insertObject(DocumentFileMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isDocumentFileFound(dbobject) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_documentfile";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("DocumentFileBLL", "insertObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_documentfile";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("DocumentFileBLL", "deleteObject", ex.Message);
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
        internal DocumentFileMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            DocumentFileMdl dbobject = new DocumentFileMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_documentfile";
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
        internal DataSet getObjectData(int documenttypeid, int categoryid, int subcategoryid, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_documentfile";
            cmd.Parameters.Add(mc.getPObject("@documenttypeid", documenttypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@categoryid", categoryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@subcategoryid", subcategoryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<DocumentFileMdl> getObjectList(int documenttypeid, int categoryid, int subcategoryid, int ccode)
        {
            DataSet ds = getObjectData(documenttypeid, categoryid, subcategoryid, ccode);
            return createObjectList(ds);
        }
        //
        internal List<DocumentCategoryMdl> getDocumentCategoryData(int documenttypeid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_documentcategory";
            cmd.Parameters.Add(mc.getPObject("@documenttypeid", documenttypeid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<DocumentCategoryMdl> objlst = new List<DocumentCategoryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DocumentCategoryMdl objmdl = new DocumentCategoryMdl();
                objmdl.CategoryId = Convert.ToInt32(dr["CategoryId"].ToString());
                objmdl.CategoryName = dr["CategoryName"].ToString();
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        internal string getAvailableItemDocuments(int itemid)
        {
            string docs = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_availableitemdocs";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                docs += ds.Tables[0].Rows[i]["CategoryName"].ToString() + ", ";
            }
            if (docs.Length > 1)
            {
                docs = docs.Substring(0, docs.Length - 2);
            }
            return docs;
        }
        //
        #endregion
        //
    }
}