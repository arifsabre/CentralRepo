using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentLedgerBLL : DbContext
    {
        //
        //internal DbSet<IndentLedgerMdl> IndentLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static IndentLedgerBLL Instance
        {
            get { return new IndentLedgerBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, IndentLedgerMdl dbobject)
        {
            //some of fields values to be set in dbprocedure
            cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemSlNo", dbobject.ItemSlNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndQty", dbobject.IndQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitId", dbobject.UnitId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AppQty", dbobject.AppQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExpectedDT", mc.getStringByDateToStore(dbobject.ExpectedDT), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StockQty", dbobject.StockQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IssuedQty", dbobject.IssuedQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PurchaseMode", dbobject.PurchaseMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PRequiredQty", dbobject.PRequiredQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PurchasedQty", dbobject.PurchasedQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ApproxRate", dbobject.ApproxRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PurchaseRate", dbobject.PurchaseRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            //vendorid & slipno from dbp itself
            cmd.Parameters.Add(mc.getPObject("@AdminUserId", dbobject.AdminUserId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ExecutedBy", dbobject.ExecutedBy, DbType.Int32));
        }
        //
        private IndentLedgerMdl getItemFromDataRow(DataRow dr)
        {
            IndentLedgerMdl dbobject = new IndentLedgerMdl();
            if (dr != null)
            {

            }
            return dbobject;
        }
        //
        internal List<IndentLedgerMdl> createObjectList(DataSet ds)
        {
            List<IndentLedgerMdl> ledgers = new List<IndentLedgerMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                IndentLedgerMdl objmdl = new IndentLedgerMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                if (dr.Table.Columns.Contains("IndentId"))
                {
                    objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());
                }
                if (dr.Table.Columns.Contains("StrIndentNo"))
                {
                    objmdl.StrIndentNo = dr["StrIndentNo"].ToString();
                }
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
                    objmdl.IndentByName = dr["IndentByName"].ToString();
                }
                if (dr.Table.Columns.Contains("HODUserId"))
                {
                    objmdl.HODUserId = Convert.ToInt32(dr["HODUserId"].ToString());
                }
                if (dr.Table.Columns.Contains("HODUserName"))
                {
                    objmdl.HODUserName = dr["HODUserName"].ToString();
                }
                if (dr.Table.Columns.Contains("ItemSlNo"))
                {
                    objmdl.ItemSlNo = Convert.ToInt32(dr["ItemSlNo"].ToString());
                }
                if (dr.Table.Columns.Contains("ItemId"))
                {
                    objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                }
                if (dr.Table.Columns.Contains("ItemCode"))
                {
                    objmdl.ItemCode = dr["ItemCode"].ToString();
                }
                if (dr.Table.Columns.Contains("ItemDesc"))
                {
                    objmdl.ItemDesc = dr["ItemDesc"].ToString();
                }
                if (dr.Table.Columns.Contains("IndQty"))
                {
                    objmdl.IndQty = Convert.ToDouble(dr["IndQty"].ToString());
                }
                if (dr.Table.Columns.Contains("UnitId"))
                {
                    objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                }
                if (dr.Table.Columns.Contains("UnitName"))
                {
                    objmdl.UnitName = dr["UnitName"].ToString();
                }
                if (dr.Table.Columns.Contains("AppQty"))
                {
                    objmdl.AppQty = Convert.ToDouble(dr["AppQty"].ToString());
                }
                if (dr.Table.Columns.Contains("ExpectedDT"))
                {
                    objmdl.ExpectedDT = mc.getStringByDate(Convert.ToDateTime(dr["ExpectedDT"].ToString()));
                }
                if (dr.Table.Columns.Contains("ExpectedDTStr"))
                {
                    objmdl.ExpectedDTStr = dr["ExpectedDTStr"].ToString();
                }
                if (dr.Table.Columns.Contains("StockQty"))
                {
                    objmdl.StockQty = Convert.ToDouble(dr["StockQty"].ToString());
                }
                if (dr.Table.Columns.Contains("IssuedQty"))
                {
                    objmdl.IssuedQty = Convert.ToDouble(dr["IssuedQty"].ToString());
                }
                if (dr.Table.Columns.Contains("PurchaseMode"))
                {
                    objmdl.PurchaseMode = dr["PurchaseMode"].ToString();
                }
                if (dr.Table.Columns.Contains("PurchaseModeName"))
                {
                    objmdl.PurchaseModeName = dr["PurchaseModeName"].ToString();
                }
                if (dr.Table.Columns.Contains("PRequiredQty"))
                {
                    objmdl.PRequiredQty = Convert.ToDouble(dr["PRequiredQty"].ToString());
                }
                if (dr.Table.Columns.Contains("PurchasedQty"))
                {
                    objmdl.PurchasedQty = Convert.ToDouble(dr["PurchasedQty"].ToString());
                }
                if (dr.Table.Columns.Contains("ApproxRate"))
                {
                    objmdl.ApproxRate = Convert.ToDouble(dr["ApproxRate"].ToString());
                }
                if (dr.Table.Columns.Contains("PurchaseRate"))
                {
                    objmdl.PurchaseRate = Convert.ToDouble(dr["PurchaseRate"].ToString());
                }
                if (dr.Table.Columns.Contains("Remarks"))
                {
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (dr.Table.Columns.Contains("IssueBalance"))
                {
                    objmdl.IssueBalance = Convert.ToDouble(dr["IssueBalance"].ToString());
                }
                if (dr.Table.Columns.Contains("PurchaseBalance"))
                {
                    objmdl.PurchaseBalance = Convert.ToDouble(dr["PurchaseBalance"].ToString());
                }
                if (dr.Table.Columns.Contains("SlipNo"))
                {
                    objmdl.SlipNo = Convert.ToInt32(dr["SlipNo"].ToString());
                }
                if (dr.Table.Columns.Contains("VendorId"))
                {
                    objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                }
                if (dr.Table.Columns.Contains("VendorName"))
                {
                    objmdl.VendorName = dr["VendorName"].ToString();
                }
                if (dr.Table.Columns.Contains("IndentStatus"))
                {
                    objmdl.IndentStatus = dr["IndentStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("IndentStatusName"))
                {
                    objmdl.IndentStatusName = dr["IndentStatusName"].ToString();
                }
                if (dr.Table.Columns.Contains("HODApprovalOn"))
                {
                    objmdl.HODApprovalOn = mc.getStringByDate(Convert.ToDateTime(dr["HODApprovalOn"].ToString()));
                }
                if (dr.Table.Columns.Contains("HODApprovalOnStr"))
                {
                    objmdl.HODApprovalOnStr = dr["HODApprovalOnStr"].ToString();
                }
                if (dr.Table.Columns.Contains("AdminUserId"))
                {
                    objmdl.AdminUserId = Convert.ToInt32(dr["AdminUserId"].ToString());
                }
                if (dr.Table.Columns.Contains("AdminUserName"))
                {
                    objmdl.AdminUserName = dr["AdminUserName"].ToString();
                }
                if (dr.Table.Columns.Contains("AdminApprovalOn"))
                {
                    objmdl.AdminApprovalOn = mc.getStringByDate(Convert.ToDateTime(dr["AdminApprovalOn"].ToString()));
                }
                if (dr.Table.Columns.Contains("AdminApprovalOnStr"))
                {
                    objmdl.AdminApprovalOnStr = dr["AdminApprovalOnStr"].ToString();
                }
                if (dr.Table.Columns.Contains("AdminQuery"))
                {
                    objmdl.AdminQuery = dr["AdminQuery"].ToString();
                }
                if (dr.Table.Columns.Contains("HODReply"))
                {
                    objmdl.HODReply = dr["HODReply"].ToString();
                }
                if (dr.Table.Columns.Contains("HODRemarks"))
                {
                    objmdl.HODRemarks = dr["HODRemarks"].ToString();
                }
                if (dr.Table.Columns.Contains("IndentorReply"))
                {
                    objmdl.IndentorReply = dr["IndentorReply"].ToString();
                }
                if (dr.Table.Columns.Contains("AdminRemarks"))
                {
                    objmdl.AdminRemarks = dr["AdminRemarks"].ToString();
                }
                if (dr.Table.Columns.Contains("ExecutedBy"))
                {
                    objmdl.ExecutedBy = Convert.ToInt32(dr["ExecutedBy"].ToString());
                }
                if (dr.Table.Columns.Contains("ExecutedByName"))
                {
                    objmdl.ExecutedByName = dr["ExecutedByName"].ToString();
                }
                if (dr.Table.Columns.Contains("ExecutedOn"))
                {
                    objmdl.ExecutedOn = mc.getStringByDate(Convert.ToDateTime(dr["ExecutedOn"].ToString()));
                }
                if (dr.Table.Columns.Contains("ExecutedOnStr"))
                {
                    objmdl.ExecutedOnStr = dr["ExecutedOnStr"].ToString();
                }
                if (dr.Table.Columns.Contains("PurchaseStatus"))
                {
                    objmdl.PurchaseStatus = dr["PurchaseStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("IssueStatus"))
                {
                    objmdl.IssueStatus = dr["IssueStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("compcode"))
                {
                    objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                }
                if (dr.Table.Columns.Contains("finyear"))
                {
                    objmdl.FinYear = dr["FinYear"].ToString();
                }
                if (dr.Table.Columns.Contains("ApproxRate") && dr.Table.Columns.Contains("AppQty"))
                {
                    objmdl.ItemValue = Math.Round(Convert.ToDouble(dr["ApproxRate"].ToString()) * Convert.ToDouble(dr["AppQty"].ToString()), 0);
                }
                if (dr.Table.Columns.Contains("ApproxRate") && dr.Table.Columns.Contains("IndQty"))
                {
                    objmdl.ApproxValue = Math.Round(Convert.ToDouble(dr["ApproxRate"].ToString()) * Convert.ToDouble(dr["IndQty"].ToString()), 0);
                }
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertIndentledger(SqlCommand cmd, IndentLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_indentledger";
            addCommandParameters(cmd, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateIndentledgerForAppQty(SqlCommand cmd, IndentMdl dbobject)
        {
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_for_hod_approval";
                cmd.Parameters.Add(mc.getPObject("@AppQty", dbobject.Ledgers[i].AppQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@ExpectedDT", mc.getStringByDateToStore(dbobject.Ledgers[i].ExpectedDT), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.Ledgers[i].RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
            }
        }
        //
        internal void updateIndentledgerForAppQtyByAdmin(SqlCommand cmd, IndentMdl dbobject)
        {
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_for_admin_approval";
                cmd.Parameters.Add(mc.getPObject("@AppQty", dbobject.Ledgers[i].AppQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.Ledgers[i].RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
            }
        }
        //
        internal void updateIndentLedgerForPendingIndentIssue(SqlCommand cmd, IndentMdl dbobject)
        {
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_indentledger_for_pending_indent_issue";
                cmd.Parameters.Add(mc.getPObject("@issuedqty", dbobject.Ledgers[i].IssuedQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.Ledgers[i].RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
            }
        }
        //
        internal void deleteIndentLedger(SqlCommand cmd, int indentid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_indentledger";
            cmd.Parameters.Add(mc.getPObject("@IndentId", indentid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        internal int getNewSlipNoToIndentLedger(SqlCommand cmd, string purchasemode)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_new_slipno_to_indentledger";
            cmd.Parameters.Add(mc.getPObject("@purchasemode", purchasemode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            return Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal IndentLedgerMdl searchIndentLedger(int recid)
        {
            DataSet ds = new DataSet();
            IndentLedgerMdl dbobject = new IndentLedgerMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_indentledger";
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
        internal DataSet getObjectData(int indentid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_indentledger";
            cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<IndentLedgerMdl> getObjectList(int indentid)
        {
            DataSet ds = getObjectData(indentid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}