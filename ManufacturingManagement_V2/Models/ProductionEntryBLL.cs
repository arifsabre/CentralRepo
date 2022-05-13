using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionEntryBLL : DbContext
    {
        //
        //internal DbSet<ProductionEntryMdl> productions { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static ProductionEntryBLL Instance
        {
            get { return new ProductionEntryBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ProductionEntryMdl dbobject)
        {
            //note
            cmd.Parameters.Add(mc.getPObject("@PlanRecId", mc.getForSqlIntString(dbobject.PlanRecId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@EntryDate", dbobject.EntryDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@PrdQty", dbobject.PrdQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RejQty", dbobject.RejQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RejReason", dbobject.RejReason.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
        }
        //
        private List<ProductionEntryMdl> createObjectList(DataSet ds)
        {
            List<ProductionEntryMdl> productions = new List<ProductionEntryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductionEntryMdl objmdl = new ProductionEntryMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.PlanRecId = Convert.ToInt32(dr["PlanRecId"].ToString());
                objmdl.MonthYear = dr["MonthYear"].ToString();
                objmdl.EntryDate = Convert.ToDateTime(dr["EntryDate"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());//d
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.PrdQty = Convert.ToDouble(dr["PrdQty"].ToString());
                objmdl.RejQty = Convert.ToDouble(dr["RejQty"].ToString());
                objmdl.ConfQty = Convert.ToDouble(dr["ConfQty"].ToString());//d
                objmdl.ToRG1 = Convert.ToDouble(dr["ToRG1"].ToString());
                objmdl.ToJobwork = Convert.ToDouble(dr["ToJobwork"].ToString());
                objmdl.WIP = Convert.ToDouble(dr["WIP"].ToString());//d
                objmdl.RejReason = dr["RejReason"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.UnitName = dr["UnitName"].ToString();//d
                productions.Add(objmdl);
            }
            return productions;
        }
        //
        private bool checkSetValidModel(ProductionEntryMdl dbobject)
        {
            Message = "";
            if (mc.isValidDate(dbobject.EntryDate) == false)
            {
                Message = "Invalid production date!";
                return false;
            }
            if (mc.isValidDateForFinYear(objCookie.getFinYear(),dbobject.EntryDate)==false)
            {
                Message = "Invalid production date for selected financial year!";
                return false;
            }
            if (dbobject.PlanRecId == 0)
            {
                //Message = "Production month not selected!";
                //return false;
                if (dbobject.ItemId == 0)
                {
                    Message = "Production item not selected!";
                    return false;
                }
            }
            if (dbobject.RejReason == null)
            {
                dbobject.RejReason = "";
            }
            if (dbobject.RejQty > 0 && dbobject.RejReason.Length == 0)
            {
                Message = "Reason for rejection not entered!";
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
        internal void insertObject(ProductionEntryMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_productionentry";
                addCommandParameters(cmd, dbobject);
                //note
                cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                //
                cmd.ExecuteNonQuery();
                //event log in db-prc itself
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ProductionEntryBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ProductionEntryMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_productionentry";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_productionentry, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ProductionEntryBLL", "updateObject", ex.Message);
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
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_productionentry";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_productionentry, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ProductionEntryBLL", "deleteObject", ex.Message);
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
        internal ProductionEntryMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            ProductionEntryMdl dbobject = new ProductionEntryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_productionentry";
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
            cmd.CommandText = "usp_get_tbl_productionentry";
            cmd.Parameters.Add(mc.getPObject("@monthyear", monthyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getDateByString(dtfrom), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getDateByString(dtto), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ProductionEntryMdl> getObjectList(string dtfrom, string dtto, int itemid = 0)
        {
            DataSet ds = getObjectData(dtfrom, dtto, itemid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}