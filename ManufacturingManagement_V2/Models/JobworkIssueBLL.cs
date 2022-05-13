using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class JobworkIssueBLL : DbContext
    {
        //
        internal DbSet<JobworkIssueMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static JobworkIssueBLL Instance
        {
            get { return new JobworkIssueBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, JobworkIssueMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@ChallanNo", dbobject.ChallanNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ChallanDate", dbobject.ChallanDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@EntryType", dbobject.EntryType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VendorId", dbobject.VendorId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VendorAddId", dbobject.VendorAddId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RMItemId", dbobject.RMItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ProcessDesc", dbobject.ProcessDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IssuedQty", dbobject.IssuedQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TrpMode", dbobject.TrpMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrpDetail", dbobject.TrpDetail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvNote", dbobject.InvNote.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@HSNCode", dbobject.HSNCode.Trim(), DbType.String));
        }
        //
        private List<JobworkIssueMdl> createObjectList(DataSet ds)
        {
            List<JobworkIssueMdl> listObj = new List<JobworkIssueMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                JobworkIssueMdl objmdl = new JobworkIssueMdl();
                objmdl.DispId = Convert.ToInt32(dr["DispId"].ToString());
                objmdl.ChallanNo = dr["ChallanNo"].ToString();
                objmdl.ChallanDate = Convert.ToDateTime(dr["ChallanDate"].ToString());
                objmdl.EntryType = dr["EntryType"].ToString();
                objmdl.EntryTypeName = dr["EntryTypeName"].ToString();//d
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                objmdl.VendorAddId = Convert.ToInt32(dr["VendorAddId"].ToString());
                objmdl.RMItemId = Convert.ToInt32(dr["RMItemId"].ToString());
                objmdl.ProcessDesc = dr["ProcessDesc"].ToString();
                objmdl.IssuedQty = Convert.ToDouble(dr["IssuedQty"].ToString());
                objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.VendorName = dr["VendorName"].ToString();//d
                objmdl.RMItemCode = dr["RMItemCode"].ToString();//d
                objmdl.RMShortName = dr["RMShortName"].ToString();//d
                objmdl.RMUnitName = dr["RMUnitName"].ToString();//d
                objmdl.ChallanDateStr = dr["ChallanDateStr"].ToString();//d
                objmdl.ApproxValue = Convert.ToDouble(dr["ApproxValue"].ToString());//d
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.TrpMode = dr["TrpMode"].ToString();
                objmdl.TrpDetail = dr["TrpDetail"].ToString();
                objmdl.InvNote = dr["InvNote"].ToString();
                objmdl.HSNCode = dr["HSNCode"].ToString();
                objmdl.IsCancelled = Convert.ToBoolean(dr["IsCancelled"].ToString());
                objmdl.CancelledOn = Convert.ToDateTime(dr["CancelledOn"].ToString());
                if (dr.Table.Columns.Contains("RemIssueQty"))
                {
                    objmdl.RemIssueQty = Convert.ToDouble(dr["RemIssueQty"].ToString());
                }
                objmdl.Reason = dr["Reason"].ToString();
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool isAlreadyFound(string challanno,int rmitemid)
        {
            //chk-- uk_tbl_jobworkissue
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_jwissueitem";
            cmd.Parameters.Add(mc.getPObject("@challanno", challanno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rmitemid", rmitemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
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
        private bool checkSetValidModel(JobworkIssueMdl dbobject)
        {
            if (dbobject.IssuedQty == 0)
            {
                Message = "Invalid qty!";
                return false;
            }
            if (dbobject.HSNCode == null)
            {
                Message = "Invalid HSN Code!";
                return false;
            }
            if (dbobject.TrpMode == null)
            {
                dbobject.TrpMode = "";
            }
            if (dbobject.TrpDetail == null)
            {
                dbobject.TrpDetail = "";
            }
            if (dbobject.InvNote == null)
            {
                dbobject.InvNote = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(JobworkIssueMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.ChallanNo,dbobject.RMItemId) == true) { return; };
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
                //to jobwork issue
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_jobworkissue";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.DispId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_jobworkissue, "dispid"));
                mc.setEventLog(cmd, dbTables.tbl_jobworkissue, dbobject.DispId.ToString(), "Inserted");
                //
                trn.Commit();
                Result = true;
                Message = "Record Added Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkIssueBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(JobworkIssueMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_jobworkissue";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@dispid", dbobject.DispId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_jobworkissue, dbobject.DispId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_jobworkissue") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("JobworkIssueBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void GenerateJobworkIssueByWIP(int wipdispid, string jwchallanno, double qty)
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
                cmd.CommandText = "usp_generate_jobworkissue_by_wip";
                cmd.Parameters.Add(mc.getPObject("@wipdispid", wipdispid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@qty", qty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@jwchallanno", jwchallanno.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_jobworkissue, wipdispid.ToString(), "Entered to Jobwork");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkIssueBLL", "GenerateJobworkIssueByWIP", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateJobworkToCancelled(int dispid, DateTime cancelledon, string reason)
        {
            Result = false;
            if (mc.isValidDate(cancelledon) == false)
            {
                Message = "Invalid cancelletion date!";
                return;
            }
            if (reason.Length == 0)
            {
                Message = "Cancellation reason not entered!";
                return;
            }
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
                cmd.CommandText = "usp_update_jobworkissue_to_cancelled";
                cmd.Parameters.Add(mc.getPObject("@dispid", dispid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@cancelledon", cancelledon.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@reason", reason.Trim(), DbType.String));
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 150);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                string retmsg = cmd.Parameters["@RetMsg"].Value.ToString();
                if (retmsg != "1")
                {
                    Message = retmsg;
                    return;
                }
                mc.setEventLog(cmd, dbTables.tbl_jobworkissue, dispid.ToString(), "Jobwork Issue Cancelled");
                trn.Commit();
                Result = true;
                Message = "Jobwork Issue Cancelled Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("JobworkIssueBLL", "updateJobworkToCancelled", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int dispid)
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
                cmd.CommandText = "usp_delete_tbl_jobworkissue";
                cmd.Parameters.Add(mc.getPObject("@dispid", dispid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_jobworkissue, dispid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("fk_tbl_jobworkreceipt_tbl_jobworkissue"))
                {
                    Message = "Item has been used in receipt entry, so it cannot be deleted!";
                }
                else
                {
                    Message = mc.setErrorLog("JobworkIssueBLL", "deleteObject", ex.Message);
                }
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
        internal JobworkIssueMdl searchObject(int dispid)
        {
            DataSet ds = new DataSet();
            JobworkIssueMdl dbobject = new JobworkIssueMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_jobworkissue";
            cmd.Parameters.Add(mc.getPObject("@dispid", dispid, DbType.Int32));
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
        internal DataSet getObjectData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_jobworkissue";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<JobworkIssueMdl> getObjectList(string dtfrom, string dtto)
        {
            DataSet ds = getObjectData(dtfrom,  dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet getJobworkIssueForWipData(string dtfrom, string dtto, int filteropt)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_jobworkissue_wiplist";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@filteropt", filteropt, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<JobworkIssueMdl> getJobworkIssueForWipList(string dtfrom, string dtto, int filteropt)
        {
            DataSet ds = getJobworkIssueForWipData(dtfrom, dtto, filteropt);
            return createObjectList(ds);
        }
        //
        internal DataSet getPendingWipToJobworkData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pending_wip_to_jobwork_list";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<JobworkIssueMdl> getPendingWipToJobworkList(string dtfrom, string dtto)
        {
            DataSet ds = getPendingWipToJobworkData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet GetChallanItemData(string challanno, bool iswip = false)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_jobwork_issued_items_by_challan";
            cmd.Parameters.Add(mc.getPObject("@challanno", challanno.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@iswip", iswip, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal double getJobworkItemAmount(int dispid, int ccode = 0, string finyear = "", string challanno = "")
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_jobwork_item_amount";
            cmd.Parameters.Add(mc.getPObject("@dispid", dispid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@challanno", challanno, DbType.String));
            double amount = Convert.ToDouble(mc.getFromDatabase(cmd));
            return amount;
        }
        //
        #endregion
        //
    }
}