using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CompanyAddressBLL : DbContext
    {
        //
        //internal DbSet<PromotionDetailMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static CompanyAddressBLL Instance
        {
            get { return new CompanyAddressBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, CompanyAddressMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", dbobject.CompCode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@Address1", dbobject.Address1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address2", dbobject.Address2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address3", dbobject.Address3.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AddressName", dbobject.AddressName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PinCode", dbobject.PinCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateName", dbobject.StateName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateCode", dbobject.StateCode.Trim(), DbType.String));
        }
        //
        private List<CompanyAddressMdl> createObjectList(DataSet ds)
        {
            List<CompanyAddressMdl> listObj = new List<CompanyAddressMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CompanyAddressMdl objmdl = new CompanyAddressMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                if (dr.Table.Columns.Contains("CmpName"))
                {
                    objmdl.CmpName = dr["CmpName"].ToString();
                }
                if (dr.Table.Columns.Contains("Address1"))
                {
                    objmdl.Address1 = dr["Address1"].ToString();
                }
                if (dr.Table.Columns.Contains("Address2"))
                {
                    objmdl.Address2 = dr["Address2"].ToString();
                }
                if (dr.Table.Columns.Contains("Address3"))
                {
                    objmdl.Address3 = dr["Address3"].ToString();
                }
                if (dr.Table.Columns.Contains("CAddress"))
                {
                    objmdl.CAddress = dr["CAddress"].ToString();
                }
                if (dr.Table.Columns.Contains("AddressName"))
                {
                    objmdl.AddressName = dr["AddressName"].ToString();
                }
                if (dr.Table.Columns.Contains("PinCode"))
                {
                    objmdl.PinCode = dr["PinCode"].ToString();
                }
                if (dr.Table.Columns.Contains("StateName"))
                {
                    objmdl.StateName = dr["StateName"].ToString();
                }
                if (dr.Table.Columns.Contains("StateCode"))
                {
                    objmdl.StateCode = dr["StateCode"].ToString();
                }
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(CompanyAddressMdl dbobject)
        {
            //address1 is set to required by model
            if (dbobject.Address2 == null)
            {
                dbobject.Address2 = "";
            }
            if (dbobject.Address3 == null)
            {
                dbobject.Address3 = "";
            }
            if (dbobject.AddressName == null)
            {
                dbobject.AddressName = "";
            }
            if (dbobject.PinCode == null)
            {
                dbobject.PinCode = "";
            }
            if (dbobject.StateName == null)
            {
                dbobject.StateName = "";
            }
            if (dbobject.StateCode == null)
            {
                dbobject.StateCode = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(CompanyAddressMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_compaddress";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_companyaddress, "recid");
                mc.setEventLog(cmd, dbTables.tbl_companyaddress, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CompanyAddressBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(CompanyAddressMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_compaddress";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_companyaddress, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CompanyAddressBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_compaddress";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_companyaddress, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CompanyAddressBLL", "deleteObject", ex.Message);
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
        internal CompanyAddressMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            CompanyAddressMdl dbobject = new CompanyAddressMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_compaddress";
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
        internal DataSet getObjectData(int compcode = 0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (compcode != 0)
            {
                cmd.CommandText = "usp_get_tbl_compaddress";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            }
            else
            {
                cmd.CommandText = "usp_get_tbl_compaddress_all";
            }
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CompanyAddressMdl> getObjectList(int compcode = 0)
        {
            DataSet ds = getObjectData(compcode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}