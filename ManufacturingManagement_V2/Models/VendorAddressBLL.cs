using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class VendorAddressBLL : DbContext
    {
        //
        internal DbSet<PromotionDetailMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static VendorAddressBLL Instance
        {
            get { return new VendorAddressBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, VendorAddressMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VendorId", mc.getForSqlIntString(dbobject.VendorId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VendorName", dbobject.VendorName.Trim(), DbType.String));//note
            cmd.Parameters.Add(mc.getPObject("@ContactPerson", dbobject.ContactPerson.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VAddress", dbobject.VAddress.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@City", dbobject.City.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ContactNo", dbobject.ContactNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FaxNo", dbobject.FaxNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateName", dbobject.StateName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateCode", dbobject.StateCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@GSTinNo", dbobject.GSTinNo.Trim(), DbType.String));
        }
        //
        private List<VendorAddressMdl> createObjectList(DataSet ds)
        {
            List<VendorAddressMdl> listObj = new List<VendorAddressMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                VendorAddressMdl objmdl = new VendorAddressMdl();
                objmdl.VendorAddId = Convert.ToInt32(dr["VendorAddId"].ToString());
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                if (dr.Table.Columns.Contains("VendorName"))
                {
                    objmdl.VendorName = dr["VendorName"].ToString();
                }
                if (dr.Table.Columns.Contains("ContactPerson"))
                {
                    objmdl.ContactPerson = dr["ContactPerson"].ToString();
                }
                if (dr.Table.Columns.Contains("VAddress"))
                {
                    objmdl.VAddress = dr["VAddress"].ToString();
                }
                if (dr.Table.Columns.Contains("City"))
                {
                    objmdl.City = dr["City"].ToString();
                }
                if (dr.Table.Columns.Contains("ContactNo"))
                {
                    objmdl.ContactNo = dr["ContactNo"].ToString();
                }
                if (dr.Table.Columns.Contains("EMail"))
                {
                    objmdl.EMail = dr["EMail"].ToString();
                }
                if (dr.Table.Columns.Contains("FaxNo"))
                {
                    objmdl.FaxNo = dr["FaxNo"].ToString();
                }
                if (dr.Table.Columns.Contains("StateName"))
                {
                    objmdl.StateName = dr["StateName"].ToString();
                }
                if (dr.Table.Columns.Contains("StateCode"))
                {
                    objmdl.StateCode = dr["StateCode"].ToString();
                }
                if (dr.Table.Columns.Contains("GSTinNo"))
                {
                    objmdl.GSTinNo = dr["GSTinNo"].ToString();
                }
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(VendorAddressMdl dbobject)
        {
            if (dbobject.VendorName == null)
            {
                Message = "Vendor not selected or entered!";
                return false;
            }
            if (dbobject.ContactPerson == null)
            {
                dbobject.ContactPerson = "";
            }
            if (dbobject.ContactNo == null)
            {
                dbobject.ContactNo = "";
            }
            if (dbobject.City == null)
            {
                dbobject.City = "";
            }
            if (dbobject.EMail == null)
            {
                dbobject.EMail = "";
            }
            if (dbobject.FaxNo == null)
            {
                dbobject.FaxNo = "";
            }
            if (dbobject.StateName == null)
            {
                dbobject.StateName = "";
            }
            if (dbobject.StateCode == null)
            {
                dbobject.StateCode = "";
            }
            if (dbobject.GSTinNo == null)
            {
                dbobject.GSTinNo = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(VendorAddressMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_vendoraddress";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string vaddid = mc.getRecentIdentityValue(cmd, dbTables.tbl_vendoraddress, "vendoraddid");
                mc.setEventLog(cmd, dbTables.tbl_vendoraddress, vaddid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VendorAddressBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(VendorAddressMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_vendoraddress";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@vendoraddid", dbobject.VendorAddId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_vendoraddress, dbobject.VendorAddId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VendorAddressBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int vendoraddid)
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
                cmd.CommandText = "usp_delete_tbl_vendoraddress";
                cmd.Parameters.Add(mc.getPObject("@vendoraddid", vendoraddid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_vendoraddress, vendoraddid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VendorAddressBLL", "deleteObject",ex.Message);
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
        internal VendorAddressMdl searchObject(int vendoraddid)
        {
            DataSet ds = new DataSet();
            VendorAddressMdl dbobject = new VendorAddressMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_vendoraddress";
            cmd.Parameters.Add(mc.getPObject("@vendoraddid", vendoraddid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_vendoraddress";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<VendorAddressMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal DataSet getAddressDataForVendor(int vendorid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_fill_vendoraddress_search_list";
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<VendorAddressMdl> getAddressListForVendor(int vendorid)
        {
            DataSet ds = getAddressDataForVendor(vendorid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}