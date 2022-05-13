using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VendorBLL : DbContext
    {
        //
        internal DbSet<VendorMdl> Vendors { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static VendorBLL Instance
        {
            get { return new VendorBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, VendorMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VendorName", dbobject.VendorName.Trim(), DbType.String));
        }
        //
        private List<VendorMdl> createObjectList(DataSet ds)
        {
            List<VendorMdl> vendors = new List<VendorMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                VendorMdl objmdl = new VendorMdl();
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                objmdl.VendorName = dr["VendorName"].ToString();
                vendors.Add(objmdl);
            }
            return vendors;
        }
        //
        private bool isAlreadyFound(string vendorname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_vendor";
            cmd.Parameters.Add(mc.getPObject("@vendorname", vendorname, DbType.String));
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
        internal void insertObject(VendorMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.VendorName) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_vendor";//with account
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string vendorid = mc.getRecentIdentityValue(cmd, dbTables.tbl_vendor, "vendorid");
                mc.setEventLog(cmd, dbTables.tbl_vendor, vendorid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VendorBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(VendorMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_vendor";//with account
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@vendorid", dbobject.VendorId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_vendor, dbobject.VendorId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_vendor") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("VendorBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int vendorid)
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
                cmd.CommandText = "usp_delete_tbl_vendor";//account
                cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_vendor, vendorid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VendorBLL", "deleteObject", ex.Message);
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
        internal VendorMdl searchObject(int vendorid)
        {
            DataSet ds = new DataSet();
            VendorMdl dbobject = new VendorMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_vendor";
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_vendor";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<VendorMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}