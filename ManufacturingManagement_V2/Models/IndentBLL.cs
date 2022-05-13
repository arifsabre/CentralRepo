using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentBLL : DbContext
    {
        //
        //public DbSet<IndentMdl> Stocks { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        IndentLedgerMdl ledgerObj = new IndentLedgerMdl();
        IndentLedgerBLL ledgerBLL = new IndentLedgerBLL();
        public static IndentBLL Instance
        {
            get { return new IndentBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, IndentMdl dbobject)
        {
            //some of fields values to be set in dbprocedure
            cmd.Parameters.Add(mc.getPObject("@VNo", dbobject.VNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(dbobject.VDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentBy", dbobject.IndentBy, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@HODUserId", dbobject.HODUserId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
        }
        //
        private List<IndentMdl> createObjectList(DataSet ds)
        {
            List<IndentMdl> indents = new List<IndentMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                IndentMdl objmdl = new IndentMdl();
                objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());
                if (dr.Table.Columns.Contains("VNo"))
                {
                    objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                }
                if (dr.Table.Columns.Contains("VDate"))
                {
                    objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("IndentBy"))
                {
                    objmdl.IndentBy = Convert.ToInt32(dr["IndentBy"].ToString());
                }
                if (dr.Table.Columns.Contains("IndentByName"))
                {
                    objmdl.IndentByName = dr["IndentByName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("HODUserId"))
                {
                    objmdl.HODUserId = Convert.ToInt32(dr["HODUserId"].ToString());
                }
                if (dr.Table.Columns.Contains("HODUserName"))
                {
                    objmdl.HODUserName = dr["HODUserName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("CompCode"))
                {
                    objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());//d
                }
                if (dr.Table.Columns.Contains("ShortName"))
                {
                    objmdl.ShortName = dr["ShortName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("FinYear"))
                {
                    objmdl.FinYear = dr["FinYear"].ToString();//d
                }
                if (dr.Table.Columns.Contains("StrIndentNo"))
                {
                    objmdl.StrIndentNo = dr["StrIndentNo"].ToString();
                }
                indents.Add(objmdl);
            }
            return indents;
        }
        //
        private void saveIndentLedger(SqlCommand cmd, IndentMdl dbobject)
        {
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].AdminUserId == 0)
                {
                    dbobject.Ledgers[i].AdminUserId = dbobject.HODUserId;
                }
                if (dbobject.Ledgers[i].ExecutedBy == 0)
                {
                    dbobject.Ledgers[i].ExecutedBy = dbobject.Ledgers[i].AdminUserId;
                }
                if (dbobject.Ledgers[i].AdminQuery == null)
                {
                    dbobject.Ledgers[i].AdminQuery = "";
                }
                if (dbobject.Ledgers[i].HODReply == null)
                {
                    dbobject.Ledgers[i].HODReply = "";
                }
                if (dbobject.Ledgers[i].HODRemarks == null)
                {
                    dbobject.Ledgers[i].HODRemarks = "";
                }
                if (dbobject.Ledgers[i].IndentorReply == null)
                {
                    dbobject.Ledgers[i].IndentorReply = "";
                }
                if (dbobject.Ledgers[i].AdminRemarks == null)
                {
                    dbobject.Ledgers[i].AdminRemarks = "";
                }
                ledgerObj = new IndentLedgerMdl();
                ledgerObj.RecId = 0;
                ledgerObj.IndentId = dbobject.IndentId;
                ledgerObj.ItemSlNo = i + 1;
                ledgerObj.ItemId = dbobject.Ledgers[i].ItemId;
                ledgerObj.ItemDesc = dbobject.Ledgers[i].ItemDesc;
                ledgerObj.IndQty = dbobject.Ledgers[i].IndQty;
                ledgerObj.UnitId = dbobject.Ledgers[i].UnitId;
                ledgerObj.AppQty = dbobject.Ledgers[i].AppQty;
                ledgerObj.ExpectedDT = dbobject.Ledgers[i].ExpectedDT;
                ledgerObj.StockQty = dbobject.Ledgers[i].StockQty;
                ledgerObj.IssuedQty = dbobject.Ledgers[i].IssuedQty;
                ledgerObj.PurchaseMode = dbobject.Ledgers[i].PurchaseMode;
                ledgerObj.PRequiredQty = dbobject.Ledgers[i].PRequiredQty;
                ledgerObj.PurchasedQty = dbobject.Ledgers[i].PurchasedQty;
                ledgerObj.ApproxRate = dbobject.Ledgers[i].ApproxRate;
                ledgerObj.PurchaseRate = dbobject.Ledgers[i].PurchaseRate;
                ledgerObj.Remarks = dbobject.Ledgers[i].Remarks;
                ledgerObj.AdminUserId = dbobject.Ledgers[i].AdminUserId;
                ledgerObj.ExecutedBy = dbobject.Ledgers[i].ExecutedBy;
                ledgerBLL.insertIndentledger(cmd, ledgerObj);
            }
        }
        //
        private bool checkSetValidModel(IndentMdl dbobject)
        {
            Message = "";
            if (mc.isValidDateString(dbobject.VDate) == false)
            {
                Message = "Invalid date";
                return false;
            }
            if (dbobject.IndentBy == 0)
            {
                dbobject.IndentBy = Convert.ToInt32(objcoockie.getUserId());
            }
            if (dbobject.HODUserId == 0)
            {
                dbobject.HODUserId = dbobject.IndentBy;
            }
            return true;
        }
        //
        private bool checkSetValidModelIndentLedger(IndentMdl dbobject)
        {
            Message = "";
            string dtstr = mc.getStringByDate(mc.getDateByString(dbobject.VDate).AddDays(2));
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].ItemDesc == null)
                {
                    Message = "Item description not entered!";
                    return false;
                }
                if (dbobject.Ledgers[i].Remarks == null)
                {
                    dbobject.Ledgers[i].Remarks = "";
                }
                if (dbobject.Ledgers[i].PurchaseMode == null)
                {
                    dbobject.Ledgers[i].PurchaseMode = "n";
                }
                if (dbobject.Ledgers[i].IndQty == 0)
                {
                    Message = "Indent qty not entered!";
                    return false;
                }
                if (dbobject.Ledgers[i].ExpectedDT == null)
                {
                    dbobject.Ledgers[i].ExpectedDT = dtstr;
                }
            }
            return true;
        }
        //
        internal DataSet getIndentVNoInfoByIndentId(int indentid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indent_vno";
            cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        private string getIndentStatus(int recid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indent_status";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        private bool isEditableIndent(int indentid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_is_editable_indent";
            cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        private DataSet getIndentItemSlNoNdHODToAddNewItem(int indentid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_itemslnondhod_to_addnew_indentitem";
            cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
            mc.fillFromDatabase(ds,cmd);
            return ds;
        }
        //
        internal bool isValidLedgerQtyForIndentExecution(IndentLedgerMdl dbobject)
        {
            Message = "";
            //check validation that appqty <= indqty and issuedqty <= appqty
            if (!(dbobject.PurchaseMode.ToLower() == "n" || dbobject.PurchaseMode.ToLower() == "i" || dbobject.PurchaseMode.ToLower() == "p"))
            {
                Message = "Invalid purchase mode!<br/>It must be I = Indent or P = PO or N for no purchase required!";
                return false;
            }
            if (dbobject.PRequiredQty < 0)
            {
                Message = "Purchase required qty must not be negative!";
                return false;
            }
            if (dbobject.PurchaseMode.ToLower() == "n" && dbobject.PRequiredQty > 0)
            {
                Message = "Invalid purchase required qty!";
                return false;
            }
            if (dbobject.PRequiredQty > 0 && dbobject.StockQty >= dbobject.AppQty)
            {
                Message = "If stock available then no need to purchase!";
                return false;
            }
            if (dbobject.PRequiredQty > (dbobject.AppQty - dbobject.StockQty))
            {
                Message = "Invalid purchase qty in reference to stock and approved qty!";
                return false;
            }
            if (dbobject.StockQty + dbobject.PRequiredQty == 0)
            {
                Message = "Invalid execution!";
                return false;
            }
            if (dbobject.PRequiredQty == 0 & dbobject.PurchaseMode.ToLower() != "n")
            {
                Message = "Invalid execution!";
                return false;
            }
            return true;
        }
        //
        internal bool isValidLedgerQtyForIndentIssueUpdate(SqlCommand cmd,IndentMdl dbobject)
        {
            Message = "";
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_indentledger";
            cmd.Parameters.Add(mc.getPObject("@indentid", dbobject.IndentId, DbType.Int32));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //check validation for issuedqty <= issuebalance
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < dbobject.Ledgers.Count; j++)
                {
                    if (dbobject.Ledgers[j].IssuedQty < 0)
                    {
                        Message = "Issued qty must not be negative!\r\nItem -" + dbobject.Ledgers[j].ItemCode.Trim() + ".";
                        return false;
                    }
                    if (ds.Tables[0].Rows[i]["recid"].ToString().ToLower() == dbobject.Ledgers[j].RecId.ToString().ToLower())
                    {
                        if (dbobject.Ledgers[j].IssuedQty > Convert.ToDouble(ds.Tables[0].Rows[i]["issuebalance"].ToString()))
                        {
                            Message = "Issued qty must not be greater than balance qty!\r\nItem -" + dbobject.Ledgers[j].ItemCode.Trim() + ".";
                            return false;
                        }
                        else break;
                    }
                }
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertIndent(IndentMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (checkSetValidModelIndentLedger(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_get_new_indent_vno";
                cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                dbobject.VNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_indent";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.IndentId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_indent, "indentid"));
                saveIndentLedger(cmd, dbobject);
                mc.setEventLog(cmd, dbTables.tbl_indent, dbobject.IndentId.ToString(), "Indent Inserted");
                trn.Commit();
                Result = true;
                Message = "Indent Saved Successfully with Indent No : " + dbobject.VNo;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "insertIndent", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndent(IndentMdl dbobject)
        {
            Result = false;
            if (isEditableIndent(dbobject.IndentId) == false)
            {
                Message = "This indent cannot be updated! It has been proceeded further.";
                return;
            }
            if (dbobject.HODUserId == 0)
            {
                Message = "HOD not selected!";
                return;
            }
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indent";
                cmd.Parameters.Add(mc.getPObject("@HODUserId", dbobject.HODUserId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indent, dbobject.IndentId.ToString(), "Indent Updated");
                Result = true;
                Message = "Indent Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "updateIndent", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentLedgerByIndentor(IndentLedgerMdl dbobject)
        {
            Result = false;
            if (getIndentStatus(dbobject.RecId).ToLower() != "p")
            {
                Message = "This indent cannot be updated! It has been proceeded further.";
                return;
            }
            if (dbobject.IndentorReply == null)
            {
                dbobject.IndentorReply = "";
            }
            if (dbobject.ItemId == 0)
            {
                Message = "Item not selected!";
                return;
            }
            if (dbobject.ItemDesc == null)
            {
                Message = "Item description is not entered!";
                return;
            }
            if (dbobject.IndQty <= 0)
            {
                Message = "Invalid indent qty!";
                return;
            }
            if (dbobject.ApproxRate <= 0)
            {
                Message = "Invalid approx rate entered!";
                return;
            }
            if (dbobject.Remarks == null)
            {
                Message = "Reason for is not entered!";
                return;
            }
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_by_indentor";
                cmd.Parameters.Add(mc.getPObject("@IndentorReply", dbobject.IndentorReply.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@IndQty", dbobject.IndQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@UnitId", dbobject.UnitId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@ApproxRate", dbobject.ApproxRate, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Ledger Updated by Indentor");
                Result = true;
                Message = "Indent Item Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "updateIndentLedgerByIndentor", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void addIndentItem(IndentLedgerMdl dbobject)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            if (dbobject.ItemId == 0)
            {
                Message = "Item not selected!";
                return;
            }
            DataSet ds = getIndentItemSlNoNdHODToAddNewItem(dbobject.IndentId);
            if (ds.Tables[0].Rows.Count == 5)
            {
                Message = "Only a maximum of 5 items per indent is allowed!";
                return;
            }
            System.Collections.ArrayList arl = new System.Collections.ArrayList();
            int islno = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                arl.Add(ds.Tables[0].Rows[i]["itemslno"].ToString());
            }
            for (int i = 1; i <= 5; i++)
            {
                if (arl.Contains(i.ToString()) == false)
                {
                    islno = i;
                }
            }
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                dbobject.HODUserId = Convert.ToInt32(ds.Tables[0].Rows[0]["hoduserid"].ToString());
                dbobject.AdminUserId = dbobject.HODUserId;
                dbobject.ExecutedBy = dbobject.HODUserId;
                if (dbobject.AdminQuery == null)
                {
                    dbobject.AdminQuery = "";
                }
                if (dbobject.HODReply == null)
                {
                    dbobject.HODReply = "";
                }
                if (dbobject.HODRemarks == null)
                {
                    dbobject.HODRemarks = "";
                }
                if (dbobject.IndentorReply == null)
                {
                    dbobject.IndentorReply = "";
                }
                if (dbobject.AdminRemarks == null)
                {
                    dbobject.AdminRemarks = "";
                }
                ledgerObj = new IndentLedgerMdl();
                ledgerObj.RecId = 0;
                ledgerObj.IndentId = dbobject.IndentId;
                ledgerObj.ItemSlNo = islno;
                ledgerObj.ItemId = dbobject.ItemId;
                ledgerObj.ItemDesc = dbobject.ItemDesc;
                ledgerObj.IndQty = dbobject.IndQty;
                ledgerObj.UnitId = dbobject.UnitId;
                ledgerObj.AppQty = dbobject.AppQty;
                ledgerObj.ExpectedDT = mc.getStringByDate(DateTime.Now.AddDays(2));
                ledgerObj.StockQty = dbobject.StockQty;
                ledgerObj.IssuedQty = dbobject.IssuedQty;
                ledgerObj.PurchaseMode = "n";
                ledgerObj.PRequiredQty = dbobject.PRequiredQty;
                ledgerObj.PurchasedQty = dbobject.PurchasedQty;
                ledgerObj.ApproxRate = dbobject.ApproxRate;
                ledgerObj.PurchaseRate = dbobject.PurchaseRate;
                ledgerObj.Remarks = dbobject.Remarks;
                ledgerObj.AdminUserId = dbobject.AdminUserId;
                ledgerObj.ExecutedBy = dbobject.ExecutedBy;
                ledgerBLL.insertIndentledger(cmd, ledgerObj);
                dbobject.RecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_indentledger, "recid"));
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Item Inserted");
                Result = true;
                Message = "Indent Item Added Successfult";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "addIndentItem", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentApprovalHOD(IndentLedgerMdl dbobject)
        {
            Result = false;
            string indentstatus = getIndentStatus(dbobject.RecId).ToLower();
            if (!(indentstatus == "p" || indentstatus == "c"))
            {
                Message = "This record cannot be updated! It has been proceeded further.";
                return;
            }
            if (dbobject.AdminUserId == 0)
            {
                Message = "Admin not selected!";
                return;
            }
            if (dbobject.AppQty <= 0)
            {
                Message = "Invalid Approved Qty!";
                return;
            }
            if (dbobject.AppQty > dbobject.IndQty)
            {
                Message = "Aproved qty exceeds from indent qty!";
                return;
            }
            if (dbobject.ApproxRate <= 0)
            {
                Message = "Invalid approx rate entered!";
                return;
            }
            if (mc.isValidDateString(dbobject.ExpectedDT) == false)
            {
                Message = "Invalid expected date!";
                return;
            }
            DataSet ds = new DataSet();
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
                cmd.CommandText = "usp_update_indent_approval_hod";
                cmd.Parameters.Add(mc.getPObject("@HODUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@AdminUserId", dbobject.AdminUserId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@AppQty", dbobject.AppQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@ApproxRate", dbobject.ApproxRate, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@ExpectedDT", mc.getStringByDateToStore(dbobject.ExpectedDT), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Approved by HOD");
                trn.Commit();
                Result = true;
                Message = "Indent Approval(HOD) Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentApprovalHOD", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentForHODRemarks(IndentLedgerMdl dbobject)
        {
            Result = false;
            string indentstatus = getIndentStatus(dbobject.RecId).ToLower();
            if (!(indentstatus == "p" || indentstatus == "c"))
            {
                Message = "This indent cannot be updated! It has been proceeded further.";
                return;
            }
            if (dbobject.HODRemarks == null)
            {
                Message = "Remarks not entered!";
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
                cmd.CommandText = "usp_update_indent_for_hod_remarks";
                cmd.Parameters.Add(mc.getPObject("@HODUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@HODRemarks", dbobject.HODRemarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Updated for HOD Remarks");
                trn.Commit();
                Result = true;
                Message = "Indent Remarks(HOD) Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentForHODRemarks", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentCancelHOD(int recid)
        {
            Result = false;
            string indentstatus = getIndentStatus(recid).ToLower();
            if (!(indentstatus == "p" || indentstatus == "h"))
            {
                Message = "This indent cannot be updated! It has been proceeded further.";
                return;
            }
            DataSet ds = new DataSet();
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
                cmd.CommandText = "usp_update_indent_cancel_hod";
                cmd.Parameters.Add(mc.getPObject("@HODUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, recid.ToString(), "Indent Cancelled by HOD");
                trn.Commit();
                Result = true;
                Message = "Indent Cancelled";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentCancelHOD", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentReplyByHOD(int recid, string hodreply)
        {
            Result = false;
            if (getIndentStatus(recid).ToLower() != "p")
            {
                Message = "This indent cannot be updated! It has been proceeded further.";
                return;
            }
            if (hodreply.Length == 0)
            {
                Message = "Reply not entered!";
                return;
            }
            DataSet ds = new DataSet();
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
                cmd.CommandText = "usp_update_indent_hod_reply";
                cmd.Parameters.Add(mc.getPObject("@HODUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@hodreply", hodreply.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, recid.ToString(), "Indent Reply by HOD");
                trn.Commit();
                Result = true;
                Message = "Reply Updated";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentReplyByHOD", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentApprovalAdmin(IndentLedgerMdl dbobject)
        {
            Result = false;
            string indentstatus = getIndentStatus(dbobject.RecId).ToLower();
            if (!(indentstatus == "h" || indentstatus == "r"))
            {
                Message = "This indent cannot be updated!";
                return;
            }
            if (dbobject.AdminRemarks == null)
            {
                dbobject.AdminRemarks = "";
            }
            if (dbobject.AppQty <= 0)
            {
                Message = "Invalid Approved Qty!";
                return;
            }
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indent_approval_admin";
                cmd.Parameters.Add(mc.getPObject("@AdminUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@AdminRemarks", dbobject.AdminRemarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@AppQty", dbobject.AppQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Approved by Admin");
                Result = true;
                Message = "Indent Approval(Admin) Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "updateIndentApprovalAdmin", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentRejectionAdmin(IndentLedgerMdl dbobject)
        {
            Result = false;
            string indentstatus = getIndentStatus(dbobject.RecId).ToLower();
            if (!(indentstatus == "h" || indentstatus == "a"))
            {
                Message = "This indent cannot be updated!";
                return;
            }
            if (dbobject.AdminRemarks == null)
            {
                dbobject.AdminRemarks = "";
            }
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indent_rejection_admin";
                cmd.Parameters.Add(mc.getPObject("@AdminUserId", objcoockie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@AdminRemarks", dbobject.AdminRemarks, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Rejected by Admin");
                Result = true;
                Message = "Indent Rejected";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "updateIndentRejectionAdmin", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentQueryByAdmin(int recid, string adminquery)
        {
            Result = false;
            if (getIndentStatus(recid).ToLower() != "h")
            {
                Message = "This indent cannot be updated!";
                return;
            }
            if (adminquery.Length == 0)
            {
                Message = "Query not entered!";
                return;
            }
            DataSet ds = new DataSet();
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
                cmd.CommandText = "usp_update_indent_admin_query";
                cmd.Parameters.Add(mc.getPObject("@adminquery", adminquery.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, recid.ToString(), "Indent Query by Admin");
                trn.Commit();
                Result = true;
                Message = "Query Updated";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentQueryByAdmin", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentExecution(IndentLedgerMdl dbobject)
        {
            Result = false;
            if (getIndentStatus(dbobject.RecId).ToLower() != "a")
            {
                Message = "This indent is not approved by admin! So it cannot be executed.";
                return;
            }
            DataSet ds = new DataSet();
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
                if (isValidLedgerQtyForIndentExecution(dbobject) == false) { return; };
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_for_indent_execution";
                cmd.Parameters.Add(mc.getPObject("@stockqty", dbobject.StockQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@purchasemode", dbobject.PurchaseMode, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@PRequiredQty", dbobject.PRequiredQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, dbobject.RecId.ToString(), "Indent Updated for Execution");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentExecution", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentForSlipNoNdVendor(IndentPurchaseSlipMdl dbobject)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                dbobject.SlipNo = ledgerBLL.getNewSlipNoToIndentLedger(cmd, "i");
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (dbobject.Ledgers[i].chkItem == true)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_slipno_nd_vendor_to_indent_ledger";
                        cmd.Parameters.Add(mc.getPObject("@vendorid", dbobject.VendorId, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@SlipNo", dbobject.SlipNo, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.Ledgers[i].RecId, DbType.Int32));
                        cmd.ExecuteNonQuery();
                    }
                }
                //
                //mc.setEventLog(cmd, dbTables.tbl_indentledgder, dbobject.IndentId.ToString(), "update indent execution");
                trn.Commit();
                Result = true;
                Message = "Purchase Slip Generated.\r\nSlip No: " + dbobject.SlipNo.ToString();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentForSlipNoNdVendor", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentLedgerForPurchaseOrder(IndentPurchaseSlipMdl dbobject)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                dbobject.SlipNo = ledgerBLL.getNewSlipNoToIndentLedger(cmd, "p");
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (dbobject.Ledgers[i].chkItem == true)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_slipno_to_indent_ledger_for_purchase_order";
                        cmd.Parameters.Add(mc.getPObject("@approxrate", dbobject.Ledgers[i].ApproxRate, DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@vendorid", dbobject.VendorId, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@SlipNo", dbobject.SlipNo, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.Ledgers[i].RecId, DbType.Int32));
                        cmd.ExecuteNonQuery();
                    }
                }
                //
                //mc.setEventLog(cmd, dbTables.tbl_indentledgder, dbobject.IndentId.ToString(), "update indent execution");
                trn.Commit();
                Result = true;
                Message = "Purchase Order Slip Generated.\r\nSlip No: " + dbobject.SlipNo.ToString();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updateIndentLedgerForPurchaseOrder", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updatePendingIndentIssueToStock(IndentMdl dbobject)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                string vtype = "ii";
                if (isValidLedgerQtyForIndentIssueUpdate(cmd, dbobject) == false) { return; };
                //invoke stock issue insert object procedure here
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_new_stock_vno";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                dbobject.VNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_stock";
                cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@VNo", dbobject.VNo, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(DateTime.Now), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecDesc", "pending indent issue", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.String));
                cmd.ExecuteNonQuery();
                StockLedgerMdl ledgerObj = new StockLedgerMdl();
                StockLedgerBLL ledgerBLL = new StockLedgerBLL();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (dbobject.Ledgers[i].IssuedQty > 0)
                    {
                        ledgerObj = new StockLedgerMdl();
                        ledgerObj.RecId = 0;
                        ledgerObj.ItemId = dbobject.Ledgers[i].ItemId;
                        ledgerObj.ItemDesc = dbobject.Ledgers[i].ItemDesc.Trim();
                        ledgerObj.Qty = dbobject.Ledgers[i].IssuedQty;
                        ledgerObj.UnitId = dbobject.Ledgers[i].UnitId;
                        ledgerObj.Rate = dbobject.Ledgers[i].ApproxRate;
                        ledgerObj.Amount = dbobject.Ledgers[i].IssuedQty * dbobject.Ledgers[i].ApproxRate;
                        ledgerObj.Discount = 0;
                        ledgerObj.TotalDiscount = 0;
                        ledgerObj.SgstPer = 0;
                        ledgerObj.SgstAmount = 0;
                        ledgerObj.CgstPer = 0;
                        ledgerObj.CgstAmount = 0;
                        ledgerObj.IgstPer = 0;
                        ledgerObj.IgstAmount = 0;
                        ledgerObj.FreightRate = 0;
                        ledgerObj.FreightAmount = 0;
                        ledgerObj.NetAmount = ledgerObj.Amount;
                        ledgerObj.Remarks = "";
                        ledgerObj.IndentLgrId = dbobject.Ledgers[i].RecId;
                        ledgerObj.OrderLgrId = 0;
                        ledgerObj.PurchaseNo = "";
                        ledgerObj.PurchaseDate = DateTime.Now;
                        ledgerBLL.insertObject(cmd,"ii",dbobject.VNo, DateTime.Now, ledgerObj);
                    }
                }
                //updation of issuedqty=sum(qty) by stockledger
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_by_stockledger";
                cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_indent, dbobject.IndentId.ToString(), "Issued for Pending Indent");
                dbobject.StkRecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd,dbTables.tbl_stock,"recid"));
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully with Issue VNo: " + dbobject.VNo.ToString();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "updatePendingIndentIssueToStock", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteIndentItem(int recid)
        {
            Result = false;
            if (getIndentStatus(recid).ToLower() != "p")
            {
                Message = "This indent item cannot be deleted! It has been proceeded further.";
                return;
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_indentledger_item";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indentledger, recid.ToString(), "Item Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("IndentBLL", "deleteIndentItem", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteIndent(int indentid)
        {
            Result = false;
            if (isEditableIndent(indentid) == false)
            {
                Message = "This indent cannot be deleted! It has been proceeded further.";
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
                //deletes indentledger also with status chk
                cmd.CommandText = "usp_delete_tbl_indent";
                cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_indent, indentid.ToString(), "Indent Delete");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("IndentBLL", "deleteIndent", ex.Message);
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
        internal IndentMdl searchIndent(int indentid)
        {
            DataSet ds = new DataSet();
            IndentMdl dbobject = new IndentMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_indent";
            cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getObjectList(indentid);
            return dbobject;
        }
        //
        internal DataSet getObjectData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_indent";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<IndentMdl> getObjectList(string dtfrom, string dtto)
        {
            DataSet ds = getObjectData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndents()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents";
            cmd.Parameters.Add(mc.getPObject("@indentby", objcoockie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getAllIndents()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_all_indents";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndentsToApproveByHOD()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_to_approve_by_hod";
            cmd.Parameters.Add(mc.getPObject("@hoduserid", objcoockie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndentsToApproveByAdmin()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_to_approve_by_admin";
            cmd.Parameters.Add(mc.getPObject("@adminuserid", objcoockie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndentsQueriedByAdmin()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_queried_by_admin";
            cmd.Parameters.Add(mc.getPObject("@adminuserid", objcoockie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndentsRepliedByHOD()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_replied_by_hod";
            cmd.Parameters.Add(mc.getPObject("@hoduserid", objcoockie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        internal List<IndentLedgerMdl> getIndentsToExecute()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_to_execute";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return IndentLedgerBLL.Instance.createObjectList(ds);
        }
        //
        private List<PurchaseSlipItemMdl> createListIndentsPurchaseSlipItems(DataSet ds)
        {
            List<PurchaseSlipItemMdl> ledgers = new List<PurchaseSlipItemMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PurchaseSlipItemMdl objmdl = new PurchaseSlipItemMdl();
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.StrIndentNo = dr["StrIndentNo"].ToString();
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.VendorName = dr["VendorName"].ToString();
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.UnitName = dr["UnitName"].ToString();//d
                objmdl.PurchaseBalance = Convert.ToDouble(dr["PurchaseBalance"].ToString());//d
                objmdl.chkItem = Convert.ToBoolean(dr["chkItem"].ToString());//d
                objmdl.ApproxRate = Convert.ToDouble(dr["ApproxRate"].ToString());
                objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.AdminRemarks = dr["AdminRemarks"].ToString();//d
                objmdl.IndentByName = dr["IndentByName"].ToString();//d
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        internal IndentPurchaseSlipMdl getIndentsToGeneratePurchaseSlip()
        {
            IndentPurchaseSlipMdl objmdl = new IndentPurchaseSlipMdl();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_to_generate_purchase_slip";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    objmdl.PSRecId = 1;
                    objmdl.SlipNo = 0;
                }
            }
            objmdl.Ledgers = createListIndentsPurchaseSlipItems(ds);
            return objmdl;
        }
        //
        internal IndentPurchaseSlipMdl getIndentsToGeneratePurchaseOrder()
        {
            IndentPurchaseSlipMdl objmdl = new IndentPurchaseSlipMdl();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_to_generate_purchase_order";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    objmdl.PSRecId = 1;
                    objmdl.SlipNo = 0;
                }
            }
            objmdl.Ledgers = createListIndentsPurchaseSlipItems(ds);
            return objmdl;
        }
        //
        internal List<IndentPurchaseSlipMdl> getIndentPurchaseSlipToPrint()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indent_purchase_slip_to_print";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            List<IndentPurchaseSlipMdl> ps = new List<IndentPurchaseSlipMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                IndentPurchaseSlipMdl objmdl = new IndentPurchaseSlipMdl();
                objmdl.SlipNo = Convert.ToInt32(dr["SlipNo"].ToString());
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                objmdl.VendorName = dr["VendorName"].ToString();
                ps.Add(objmdl);
            }
            return ps;
        }
        //
        internal List<IndentMdl> getIndentsPendingToIssue()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_pending_to_issue";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        /// <summary>
        /// revise as pending indents to receipt by full outer join as jw
        /// </summary>
        /// <returns></returns>
        internal List<IndentMdl> getIndentsPendingToPurchaseByIndent()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_pending_to_purchase_by_indent";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        internal List<IndentMdl> getIndentsPendingToPurchaseByOrder()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indents_pending_to_purchase_by_order";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}