using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QuailFunctionBLL : DbContext
    {
        //
        //internal DbSet<QuailFunctionMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static QuailFunctionBLL Instance
        {
            get { return new QuailFunctionBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, QuailFunctionMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@FctnId", dbobject.FctnId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@FctnName", dbobject.FctnName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LdUserId", dbobject.LdUserId, DbType.Int32));
        }
        //
        private List<QuailFunctionMdl> createObjectList(DataSet ds)
        {
            List<QuailFunctionMdl> objlist = new List<QuailFunctionMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailFunctionMdl objmdl = new QuailFunctionMdl();
                objmdl.FctnId = Convert.ToInt32(dr["FctnId"].ToString());
                objmdl.FctnName = dr["FctnName"].ToString();
                objmdl.LdUserId = Convert.ToInt32(dr["LdUserId"].ToString());
                objmdl.FnLeader = dr["FnLeader"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string fctnname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_quailfunction";
            cmd.Parameters.Add(mc.getPObject("@fctnname", fctnname, DbType.String));
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
        private bool checkSetValidModel(QuailFunctionMdl dbobject)
        {
            if (dbobject.LdUserId == 0)
            {
                Message = "Function leader not selected!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(QuailFunctionMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.FctnName) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_quailfunction";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailfunction, dbobject.FctnId.ToString(), "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailFunctionBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(QuailFunctionMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_quailfunction";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailfunction, dbobject.FctnId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_quailfunction") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("QuailFunctionBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int fctnid)
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
                cmd.CommandText = "usp_delete_tbl_quailfunction";
                cmd.Parameters.Add(mc.getPObject("@fctnid", fctnid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailfunction, fctnid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailFunctionBLL", "deleteObject", ex.Message);
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
        internal QuailFunctionMdl searchObject(int fctnid)
        {
            DataSet ds = new DataSet();
            QuailFunctionMdl dbobject = new QuailFunctionMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_quailfunction";
            cmd.Parameters.Add(mc.getPObject("@fctnid", fctnid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_quailfunction";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<QuailFunctionMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}