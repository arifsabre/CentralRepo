using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IMTETypeBLL : DbContext
    {
        //
        //internal DbSet<IMTETypeMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static IMTETypeBLL Instance
        {
            get { return new IMTETypeBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, IMTETypeMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@IMTETypeName", dbobject.IMTETypeName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.String));
        }
        //
        private List<IMTETypeMdl> createObjectList(DataSet ds)
        {
            List<IMTETypeMdl> objlist = new List<IMTETypeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                IMTETypeMdl objmdl = new IMTETypeMdl();
                objmdl.IMTETypeId = Convert.ToInt32(dr["IMTETypeId"].ToString());
                objmdl.IMTETypeName = dr["IMTETypeName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string imtetypename)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_imtetype";
            cmd.Parameters.Add(mc.getPObject("@imtetypename", imtetypename, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.String));
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
        internal void insertObject(IMTETypeMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.IMTETypeName) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_imtetype";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string imtetypeid = mc.getRecentIdentityValue(cmd, dbTables.tbl_imtetype, "imtetypeid");
                mc.setEventLog(cmd, dbTables.tbl_imtetype, imtetypeid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IMTETypeBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(IMTETypeMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_imtetype";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@IMTETypeId", dbobject.IMTETypeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_imtetype, dbobject.IMTETypeId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_imtetype") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("IMTETypeBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int imtetypeid)
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
                cmd.CommandText = "usp_delete_tbl_imtetype";
                cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_imtetype, imtetypeid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IMTETypeBLL", "deleteObject", ex.Message);
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
        internal IMTETypeMdl searchObject(int imtetypeid)
        {
            DataSet ds = new DataSet();
            IMTETypeMdl dbobject = new IMTETypeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_imtetype";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
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
        internal DataSet getObjectData(int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_imtetype";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<IMTETypeMdl> getObjectList(int ccode=0)
        {
            DataSet ds = getObjectData(ccode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}