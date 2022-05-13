using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MasterDocumentNameBLL : DbContext
    {
        //
        //internal DbSet<MasterDocumentNameMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static MasterDocumentNameBLL Instance
        {
            get { return new MasterDocumentNameBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, MasterDocumentNameMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@DocumentName", dbobject.DocumentName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", dbobject.CompCode, DbType.Int16));
        }
        //
        private List<MasterDocumentNameMdl> createObjectList(DataSet ds)
        {
            List<MasterDocumentNameMdl> objlist = new List<MasterDocumentNameMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                MasterDocumentNameMdl objmdl = new MasterDocumentNameMdl();
                objmdl.DocumentId = Convert.ToInt32(dr["DocumentId"].ToString());
                objmdl.DocumentName = dr["DocumentName"].ToString();
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.CmpName = dr["CmpName"].ToString();//d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string documentname, int compcode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_masterdocumentname";
            cmd.Parameters.Add(mc.getPObject("@documentname", documentname.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
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
        internal void insertObject(MasterDocumentNameMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.DocumentName,dbobject.CompCode) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_masterdocumentname";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string documentid = mc.getRecentIdentityValue(cmd, dbTables.tbl_masterdocumentname, "documentid");
                mc.setEventLog(cmd, dbTables.tbl_masterdocumentname, documentid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("MasterDocumentNameBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(MasterDocumentNameMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_masterdocumentname";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@documentid", dbobject.DocumentId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_masterdocumentname, dbobject.DocumentId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_masterdocumentname") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("MasterDocumentNameBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int documentid)
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
                cmd.CommandText = "usp_delete_tbl_masterdocumentname";
                cmd.Parameters.Add(mc.getPObject("@documentid", documentid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_masterdocumentname, documentid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("MasterDocumentNameBLL", "deleteObject", ex.Message);
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
        internal MasterDocumentNameMdl searchObject(int documentid)
        {
            DataSet ds = new DataSet();
            MasterDocumentNameMdl dbobject = new MasterDocumentNameMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_masterdocumentname";
            cmd.Parameters.Add(mc.getPObject("@documentid", documentid, DbType.Int32));
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
        internal DataSet getObjectData(int compcode=0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_masterdocumentname";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<MasterDocumentNameMdl> getObjectList(int compcode=0)
        {
            DataSet ds = getObjectData(compcode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}