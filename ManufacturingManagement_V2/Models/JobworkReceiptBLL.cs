using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class JobworkReceiptBLL : DbContext
    {
        //
        internal DbSet<JobworkReceiptMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static JobworkReceiptBLL Instance
        {
            get { return new JobworkReceiptBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, JobworkReceiptMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@DispId", dbobject.DispId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RecDate", dbobject.RecDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@InvNo", dbobject.InvNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvDate", dbobject.InvDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@FGItemId", mc.getForSqlIntString(dbobject.FGItemId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@FgItemQty", dbobject.FgItemQty, DbType.Double));
            //entered by bdp //cmd.Parameters.Add(mc.getPObject("@UnitId", dbobject.UnitId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ReceivedQty", dbobject.ReceivedQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@WasteQty", dbobject.WasteQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@InvWQty", dbobject.InvWQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks, DbType.String));
        }
        //
        private List<JobworkReceiptMdl> createObjectList(DataSet ds)
        {
            List<JobworkReceiptMdl> listObj = new List<JobworkReceiptMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JobworkReceiptMdl objmdl = new JobworkReceiptMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.DispId = Convert.ToInt32(dr["DispId"].ToString());
                objmdl.RecDate = Convert.ToDateTime(dr["RecDate"].ToString());
                objmdl.InvNo = dr["InvNo"].ToString();
                objmdl.InvDate = Convert.ToDateTime(dr["InvDate"].ToString());
                objmdl.FGItemId = Convert.ToInt32(dr["FGItemId"].ToString());
                objmdl.FGItemCode = dr["FGItemCode"].ToString();//d
                objmdl.FgItemQty = Convert.ToDouble(dr["FgItemQty"].ToString());
                objmdl.ReceivedQty = Convert.ToDouble(dr["ReceivedQty"].ToString());
                objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.FGUnitName = dr["FGUnitName"].ToString();//d
                objmdl.WasteQty = Convert.ToDouble(dr["WasteQty"].ToString());
                objmdl.InvWQty = Convert.ToDouble(dr["InvWQty"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();//d
                //
                objmdl.ChallanNo = dr["ChallanNo"].ToString();//d
                objmdl.ChallanDate = Convert.ToDateTime(dr["ChallanDate"].ToString());//d
                objmdl.IssuedQty = Convert.ToDouble(dr["IssuedQty"].ToString());//d
                objmdl.VendorName = dr["VendorName"].ToString();//d
                objmdl.RMItemCode = dr["RMItemCode"].ToString();//d
                objmdl.RMShortName = dr["RMShortName"].ToString();//d
                objmdl.RMUnitName = dr["RMUnitName"].ToString();//d
                objmdl.RemainingQty = Convert.ToDouble(dr["RemainingQty"].ToString());//d
                objmdl.ProcessDesc = dr["ProcessDesc"].ToString();//d
                objmdl.ChallanDateStr = dr["ChallanDateStr"].ToString();//d
                objmdl.RecDateStr = dr["RecDateStr"].ToString();//d
                objmdl.InvDateStr = dr["InvDateStr"].ToString();//d
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(JobworkReceiptMdl dbobject)
        {
            if (dbobject.DispId == 0)
            {
                Message = "Invalid Entry!";
                return false;
            }
            if (dbobject.FGItemCode == null)
            {
                dbobject.FGItemId = 0;
            }
            if (dbobject.ReceivedQty+dbobject.InvWQty == 0)
            {
                Message = "No Qty Entered!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.FGItemId == 0 && dbobject.Remarks.Length == 0)
            {
                Message = "Either processed item or remarks is compulsory to enter!";
                return false;
            }
            if (dbobject.FGItemId != 0 && dbobject.FgItemQty == 0)
            {
                Message = "Processed qty is not entered!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(JobworkReceiptMdl dbobject)
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
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_jobworkreceipt";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.RecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_jobworkreceipt, "recid"));
                mc.setEventLog(cmd, dbTables.tbl_jobworkreceipt, dbobject.RecId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkReceiptBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(JobworkReceiptMdl dbobject)
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
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_jobworkreceipt";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_jobworkreceipt, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkReceiptBLL", "updateObject", ex.Message);
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
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_jobworkreceipt";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_jobworkreceipt, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkReceiptBLL", "deleteObject", ex.Message);
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
        internal JobworkReceiptMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            JobworkReceiptMdl dbobject = new JobworkReceiptMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_jobworkreceipt";
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
        internal double getJobworkBalanceQty(int dispid)
        {
            DataSet ds = new DataSet();
            JobworkReceiptMdl dbobject = new JobworkReceiptMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_jobwork_balance_qty";
            cmd.Parameters.Add(mc.getPObject("@dispid", dispid, DbType.Int32));
            return Convert.ToDouble(mc.getFromDatabase(cmd));
        }
        //
        internal DataSet getPendingJobworksToReceiveData(string dtfrom, string dtto, string opt, int vendorid, string challanno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_jobwork_issue_receipt";
            cmd.Parameters.Add(mc.getPObject("@opt", opt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getDateByString(dtfrom), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getDateByString(dtto), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@challanno", challanno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<JobworkReceiptMdl> getListPendingJobworksToReceive(string dtfrom, string dtto, string opt, int vendorid, string challanno)
        {
            DataSet ds = getPendingJobworksToReceiveData(dtfrom, dtto, opt, vendorid, challanno);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}