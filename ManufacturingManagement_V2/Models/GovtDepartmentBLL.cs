using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class GovtDepartmentBLL : DbContext
    {
        //
        //internal DbSet<GovtDepartmentMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static GovtDepartmentBLL Instance
        {
            get { return new GovtDepartmentBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, GovtDepartmentMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@DepName", dbobject.DepName.Trim(), DbType.String));
        }
        //
        private List<GovtDepartmentMdl> createObjectList(DataSet ds)
        {
            List<GovtDepartmentMdl> objlist = new List<GovtDepartmentMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                GovtDepartmentMdl objmdl = new GovtDepartmentMdl();
                objmdl.DepId = Convert.ToInt32(dr["DepId"].ToString());
                objmdl.DepName = dr["DepName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string depname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_govtdepartment";
            cmd.Parameters.Add(mc.getPObject("@depname", depname, DbType.String));
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
        internal void insertObject(GovtDepartmentMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.DepName) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_govtdepartment";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string depid = mc.getRecentIdentityValue(cmd, dbTables.tbl_govtdepartment, "depid");
                mc.setEventLog(cmd, dbTables.tbl_govtdepartment, depid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("GovtDepartmentBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(GovtDepartmentMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_govtdepartment";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@depid", dbobject.DepId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_govtdepartment, dbobject.DepId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_govtdepartment") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("GovtDepartmentBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int depid)
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
                cmd.CommandText = "usp_delete_tbl_govtdepartment";
                cmd.Parameters.Add(mc.getPObject("@depid", depid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_govtdepartment, depid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("GovtDepartmentBLL", "deleteObject", ex.Message);
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
        internal GovtDepartmentMdl searchObject(int depid)
        {
            DataSet ds = new DataSet();
            GovtDepartmentMdl dbobject = new GovtDepartmentMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_govtdepartment";
            cmd.Parameters.Add(mc.getPObject("@depid", depid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_govtdepartment";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<GovtDepartmentMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}