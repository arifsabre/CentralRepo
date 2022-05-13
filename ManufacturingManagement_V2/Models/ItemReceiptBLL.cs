using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ItemReceiptBLL : DbContext
    {
        //
        public DbSet<StockMdl> Stocks { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        StockLedgerMdl ledgerObj = new StockLedgerMdl();
        StockLedgerBLL ledgerBLL = new StockLedgerBLL();
        public static ItemReceiptBLL Instance
        {
            get { return new ItemReceiptBLL(); }
        }
        //
        #region private objects
        //
        private List<ItemReceiptMdl> createObjectList(DataSet ds)
        {
            List<ItemReceiptMdl> stocks = new List<ItemReceiptMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ItemReceiptMdl objmdl = new ItemReceiptMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                //
                if (dr.Table.Columns.Contains("IndentId"))
                {
                    objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());
                }
                if (dr.Table.Columns.Contains("StrIndentNo"))
                {
                    objmdl.StrIndentNo = dr["StrIndentNo"].ToString();//d
                }
                if (dr.Table.Columns.Contains("IndentDate"))
                {
                    objmdl.IndentDate = dr["IndentDate"].ToString();//d
                }
                if (dr.Table.Columns.Contains("IndentLgrId"))
                {
                    objmdl.IndentLgrId = Convert.ToInt32(dr["IndentLgrId"].ToString());
                }
                //
                if (dr.Table.Columns.Contains("OrderId"))
                {
                    objmdl.OrderId = Convert.ToInt32(dr["OrderId"].ToString());
                }
                if (dr.Table.Columns.Contains("StrOrderNo"))
                {
                    objmdl.StrOrderNo = dr["StrOrderNo"].ToString();//d
                }
                if (dr.Table.Columns.Contains("OrderDate"))
                {
                    objmdl.OrderDate = dr["OrderDate"].ToString();//d
                }
                if (dr.Table.Columns.Contains("OrderLgrId"))
                {
                    objmdl.OrderLgrId = Convert.ToInt32(dr["OrderLgrId"].ToString());
                }
                //
                objmdl.PurchaseBalance = Convert.ToDouble(dr["PurchaseBalance"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.PurchaseNo = dr["PurchaseNo"].ToString();
                objmdl.PurchaseDateStr = dr["PurchaseDateStr"].ToString();
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.VNo = dr["VNo"].ToString();
                objmdl.VDateStr = dr["VDateStr"].ToString();
                //
                stocks.Add(objmdl);
            }
            return stocks;
        }
        //
        private bool checkSetValidModel(StockLedgerMdl dbobject, string rectype)
        {
            Message = "";
            if (mc.isValidDate(dbobject.VDate) == false)
            {
                Message = "Invalid receipt date";
                return false;
            }
            if (mc.isValidDate(dbobject.PurchaseDate) == false)
            {
                Message = "Invalid purchase date";
                return false;
            }
            if (rectype == "i")
            {
                if (dbobject.IndentLgrId == 0)
                {
                    Message = "Invalid entry for indent receipt!";
                    return false;
                }
            }
            if (rectype == "p")
            {
                if (dbobject.OrderLgrId == 0)
                {
                    Message = "Invalid entry for po receipt!";
                    return false;
                }
            }
            if (dbobject.PurchaseNo == null)
            {
                Message = "Purchase No not entered!";
                return false;
            }
            if (dbobject.Qty <= 0)
            {
                Message = "Invalid qty entered!";
                return false;
            }
            if (dbobject.Rate <= 0)
            {
                Message = "Invalid purchase rate entered!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            dbobject = ledgerBLL.setStockLedgerValues(dbobject);
            return true;
        }
        //
        internal bool checkIndentBalanceQty(SqlCommand cmd, int indentlgrid, double qty)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_indent_balance_qty";
            cmd.Parameters.Add(mc.getPObject("@indentlgrid", indentlgrid, DbType.Int32));
            if(qty > Convert.ToDouble(mc.getFromDatabase(cmd, cmd.Connection)))
            {
                Message = "Purchase Qty must not be greater than balance Qty!";
                return false;
            }
            return true;
        }
        //
        internal bool checkOrderBalanceQty(SqlCommand cmd, int orderlgrid, double qty)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_order_balance_qty";
            cmd.Parameters.Add(mc.getPObject("@orderlgrid", orderlgrid, DbType.Int32));
            if(qty > Convert.ToDouble(mc.getFromDatabase(cmd, cmd.Connection)))
            {
                Message = "Purchase Qty must not be greater than balance Qty!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertIndentReceipt(StockLedgerMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject,"i") == false) { return; };
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
                if (checkIndentBalanceQty(cmd, dbobject.IndentLgrId, dbobject.Qty) == false) { return; };
                dbobject.VType = "ip";//ip = indent purchase
                dbobject.VNo = ledgerBLL.getNewVNoForStockLedger(cmd,dbobject.VType);
                ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, dbobject.VDate, dbobject);
                dbobject.RecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_stockledger, "recid"));
                ledgerBLL.updatePurchaseQtyToIndentLedger(cmd, dbobject.RecId);
                mc.setEventLog(cmd, dbTables.tbl_stockledger, dbobject.RecId.ToString(), "Indent Receipt Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemReceiptBLL", "insertIndentReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateIndentReceipt(StockLedgerMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject, "i") == false) { return; };
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
                if (checkIndentBalanceQty(cmd, dbobject.IndentLgrId, dbobject.Qty) == false) { return; };
                dbobject.VType = "ip";//ip = indent purchase
                ledgerBLL.updateObject(cmd, dbobject.RecId, "ip", dbobject.VNo,dbobject.VDate, dbobject);
                ledgerBLL.updatePurchaseQtyToIndentLedger(cmd, dbobject.RecId);
                mc.setEventLog(cmd, dbTables.tbl_stockledger, dbobject.RecId.ToString(), "Indent Receipt Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemReceiptBLL", "updateIndentReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void insertPurchaseOrderReceipt(StockLedgerMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject, "p") == false) { return; };
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
                if (checkOrderBalanceQty(cmd, dbobject.OrderLgrId, dbobject.Qty) == false) { return; };
                dbobject.VType = "pp";//pp = po receipt
                dbobject.VNo = ledgerBLL.getNewVNoForStockLedger(cmd, dbobject.VType);
                ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, dbobject.VDate, dbobject);
                dbobject.RecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_stockledger, "recid"));
                ledgerBLL.updatePurchaseQtyToOrderLedger(cmd, dbobject.RecId);
                mc.setEventLog(cmd, dbTables.tbl_stockledger, dbobject.RecId.ToString(), "PO Receipt Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemReceiptBLL", "insertPurchaseOrderReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updatePurchaseOrderReceipt(StockLedgerMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject, "p") == false) { return; };
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
                if (checkOrderBalanceQty(cmd, dbobject.OrderLgrId, dbobject.Qty) == false) { return; };
                ledgerBLL.updateObject(cmd, dbobject.RecId,"pp", dbobject.VNo, dbobject.VDate, dbobject);
                ledgerBLL.updatePurchaseQtyToOrderLedger(cmd, dbobject.RecId);
                mc.setEventLog(cmd, dbTables.tbl_stockledger, dbobject.RecId.ToString(), "PO Receipt Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemReceiptBLL", "updatePurchaseOrderReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteReceipt(int recid)
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
                //this object is with qty updation also
                ledgerBLL.deleteStockLedgerByRecId(cmd,recid);
                mc.setEventLog(cmd, dbTables.tbl_stockledger, recid.ToString(), "Stock Ledger Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ItemReceiptBLL", "deleteReceipt", ex.Message);
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
        internal ItemReceiptMdl getIndentLedgerInfo(int indentlgrid)
        {
            ItemReceiptMdl irecmdl = new ItemReceiptMdl();
            IndentLedgerBLL indtlgrbll = new IndentLedgerBLL();
            IndentLedgerMdl indtlgr = new IndentLedgerMdl();
            indtlgr = indtlgrbll.searchIndentLedger(indentlgrid);
            irecmdl.IndentId = indtlgr.IndentId;
            irecmdl.ItemCode = indtlgr.ItemCode;
            irecmdl.ItemDesc = indtlgr.ItemDesc;
            irecmdl.UnitName = indtlgr.UnitName;
            irecmdl.PurchaseBalance = indtlgr.PRequiredQty-indtlgr.PurchasedQty;
            IndentBLL ibll = new IndentBLL();
            IndentMdl indtmdl = new IndentMdl();
            indtmdl = ibll.searchIndent(irecmdl.IndentId);
            irecmdl.IndentDate = indtmdl.VDate;
            irecmdl.StrIndentNo = indtmdl.VNo.ToString() + "/" + indtmdl.FinYear + "/" + indtmdl.ShortName;
            StockLedgerMdl stklgrmdl = new StockLedgerMdl();
            stklgrmdl.VNo = 0;
            stklgrmdl.VDate = DateTime.Now;
            stklgrmdl.PurchaseDate = DateTime.Now;
            stklgrmdl.IndentLgrId = indentlgrid;
            stklgrmdl.ItemId = indtlgr.ItemId;
            stklgrmdl.ItemCode = indtlgr.ItemCode;
            stklgrmdl.ItemDesc = indtlgr.ItemDesc;
            irecmdl.StkLgr = stklgrmdl;
            return irecmdl;
        }
        //
        internal ItemReceiptMdl getOrderLedgerInfo(int orderlgrid)
        {
            ItemReceiptMdl irecmdl = new ItemReceiptMdl();
            OrderLedgerBLL ordlgrbll = new OrderLedgerBLL();
            OrderLedgerMdl ordlgr = new OrderLedgerMdl();
            ordlgr = ordlgrbll.searchOrderLedger(orderlgrid);
            irecmdl.OrderId = ordlgr.OrderId;
            irecmdl.ItemCode = ordlgr.ItemCode;
            irecmdl.ItemDesc = ordlgr.ItemDesc;
            irecmdl.UnitName = ordlgr.UnitName;
            irecmdl.PurchaseBalance = ordlgr.OrdQty - ordlgr.DspQty;
            OrderBLL obll = new OrderBLL();
            OrderMdl omdl = new OrderMdl();
            omdl = obll.searchObject(irecmdl.OrderId);
            irecmdl.OrderDate = omdl.OrderDate;
            //irecmdl.OrderDate = mc.getStringByDate(omdl.OrderDate);
            irecmdl.StrOrderNo = omdl.OrderNo + "/" + omdl.FinYear;
            StockLedgerMdl stklgrmdl = new StockLedgerMdl();
            stklgrmdl.VNo = 0;
            stklgrmdl.VDate = DateTime.Now;
            stklgrmdl.PurchaseDate = DateTime.Now;
            stklgrmdl.OrderLgrId = orderlgrid;
            stklgrmdl.ItemId = ordlgr.ItemId;
            stklgrmdl.ItemCode = ordlgr.ItemCode;
            stklgrmdl.ItemDesc = ordlgr.ItemDesc;
            irecmdl.StkLgr = stklgrmdl;
            return irecmdl;
        }
        //
        internal ItemReceiptMdl searchReceiptForIndent(int recid)
        {
            StockLedgerBLL stklgrBll = new StockLedgerBLL();
            StockLedgerMdl stklgrMdl = new StockLedgerMdl();
            stklgrMdl = stklgrBll.searchStockLedger(recid);
            ItemReceiptMdl irecmdl = new ItemReceiptMdl();
            irecmdl = getIndentLedgerInfo(stklgrMdl.IndentLgrId);
            irecmdl.StkLgr = stklgrMdl;
            return irecmdl;
        }
        //
        internal ItemReceiptMdl searchReceiptForPurchaseOrder(int recid)
        {
            StockLedgerBLL stklgrBll = new StockLedgerBLL();
            StockLedgerMdl stklgrMdl = new StockLedgerMdl();
            stklgrMdl = stklgrBll.searchStockLedger(recid);
            ItemReceiptMdl irecmdl = new ItemReceiptMdl();
            irecmdl = getOrderLedgerInfo(stklgrMdl.OrderLgrId);
            irecmdl.StkLgr = stklgrMdl;
            return irecmdl;
        }
        //
        internal DataSet getItemReceivingByIndentData()
        {
            DataSet ds = new DataSet();
            StockMdl dbobject = new StockMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_item_receiving_by_indent";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemReceiptMdl> getItemReceivingByIndent()
        {
            DataSet ds = getItemReceivingByIndentData();
            return createObjectList(ds);
        }
        //
        internal DataSet getItemReceivingByPOData()
        {
            DataSet ds = new DataSet();
            StockMdl dbobject = new StockMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_item_receiving_by_purchaseorder";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemReceiptMdl> getItemReceivingByPurchaseOrder()
        {
            DataSet ds = getItemReceivingByPOData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}