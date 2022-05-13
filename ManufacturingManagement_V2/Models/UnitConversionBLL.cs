using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UnitConversionBLL : DbContext
    {
        //
        //internal DbSet<UnitConversionMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static UnitConversionBLL Instance
        {
            get { return new UnitConversionBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, UnitConversionMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@MainUnit", dbobject.MainUnit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AltUnit", dbobject.AltUnit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ConvFactor", dbobject.ConvFactor, DbType.Double));
        }
        //
        private List<UnitConversionMdl> createObjectList(DataSet ds)
        {
            List<UnitConversionMdl> storeitems = new List<UnitConversionMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                UnitConversionMdl objmdl = new UnitConversionMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.MainUnit = Convert.ToInt32(dr["MainUnit"].ToString());
                objmdl.AltUnit = Convert.ToInt32(dr["AltUnit"].ToString());
                objmdl.MainUnitName = dr["MainUnitName"].ToString();
                objmdl.AltUnitName = dr["AltUnitName"].ToString();
                objmdl.ConvFactor = Convert.ToDouble(dr["ConvFactor"].ToString());
                storeitems.Add(objmdl);
            }
            return storeitems;
        }
        //
        private bool isFoundUnitConversion(int mainunit, int altunit)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_unitconv";
            cmd.Parameters.Add(mc.getPObject("@mainunit", mainunit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@altunit", altunit, DbType.Int32));
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
        private bool checkSetValidModel(UnitConversionMdl dbobject)
        {
            if (dbobject.MainUnit == 0)
            {
                Message = "Main unit not selected!";
                return false;
            }
            if (dbobject.AltUnit == 0)
            {
                Message = "Alternate unit not selected!";
                return false;
            }
            if (dbobject.AltUnit == dbobject.MainUnit)
            {
                Message = "Main and Alternate unit must not be equal!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(UnitConversionMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundUnitConversion(dbobject.MainUnit, dbobject.AltUnit) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_unitconversion";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_unitconversion, "recid");
                mc.setEventLog(cmd, dbTables.tbl_item, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UnitConversionBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(UnitConversionMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_unitconversion";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_unitconversion, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UnitConversionBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_unitconversion";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_unitconversion, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UnitConversionBLL", "deleteObject", ex.Message);
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
        internal UnitConversionMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            UnitConversionMdl dbobject = new UnitConversionMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_unitconversion";
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
        internal DataSet getObjectData(int mainunit=0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_unitconversion";
            cmd.Parameters.Add(mc.getPObject("@mainunit", mainunit, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UnitConversionMdl> getObjectList(int mainunit=0)
        {
            DataSet ds = getObjectData(mainunit);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}