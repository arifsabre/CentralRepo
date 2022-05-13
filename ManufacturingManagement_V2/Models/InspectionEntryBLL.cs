using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class InspectionEntryBLL : DbContext
    {
        //
        //internal DbSet<InspectionEntryMdl> productions { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static InspectionEntryBLL Instance
        {
            get { return new InspectionEntryBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, InspectionEntryMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@PlanRecId", dbobject.PlanRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@EntryDate", dbobject.EntryDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@InspQty", dbobject.InspQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
        }
        //
        private List<InspectionEntryMdl> createObjectList(DataSet ds)
        {
            List<InspectionEntryMdl> productions = new List<InspectionEntryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                InspectionEntryMdl objmdl = new InspectionEntryMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.PlanRecId = Convert.ToInt32(dr["PlanRecId"].ToString());
                objmdl.MonthYear = dr["MonthYear"].ToString();
                objmdl.EntryDate = Convert.ToDateTime(dr["EntryDate"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());//d
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.InspQty = Convert.ToDouble(dr["InspQty"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.UnitName = dr["UnitName"].ToString();//d
                productions.Add(objmdl);
            }
            return productions;
        }
        //
        private bool checkSetValidModel(InspectionEntryMdl dbobject)
        {
            Message = "";
            if (mc.isValidDate(dbobject.EntryDate) == false)
            {
                Message = "Invalid inspetion date!";
                return false;
            }
            if (dbobject.PlanRecId == 0)
            {
                Message = "Production month not selected!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(InspectionEntryMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_inspectionentry";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_inspectionentry, "recid");
                mc.setEventLog(cmd, dbTables.tbl_productionentry, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("InspectionEntryBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(InspectionEntryMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_inspectionentry";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_inspectionentry, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("InspectionEntryBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_inspectionentry";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_inspectionentry, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("InspectionEntryBLL", "deleteObject", ex.Message);
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
        internal InspectionEntryMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            InspectionEntryMdl dbobject = new InspectionEntryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_inspectionentry";
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
        internal DataSet getObjectData(string dtfrom, string dtto, int itemid = 0, string monthyear = "")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_inspectionentry";
            cmd.Parameters.Add(mc.getPObject("@monthyear", monthyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getDateByString(dtfrom), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getDateByString(dtto), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<InspectionEntryMdl> getObjectList(string dtfrom, string dtto)
        {
            DataSet ds = getObjectData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}