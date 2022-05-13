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
    public class OrderBLL : DbContext
    {
        //
        //public DbSet<OrderMdl> Orders { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        OrderLedgerMdl ledgerObj = new OrderLedgerMdl();
        OrderLedgerBLL ledgerBLL = new OrderLedgerBLL();
        public static OrderBLL Instance
        {
            get { return new OrderBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, OrderMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode",objcoockie.getCompCode(),DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear",objcoockie.getFinYear(),DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OrderNo",dbobject.OrderNo,DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@OrderDate", mc.getStringByDateToStore(dbobject.OrderDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VendorId",dbobject.VendorId,DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DelvSchedule", dbobject.DelvSchedule.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvDate", mc.getStringByDateToStore(dbobject.DelvDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OrderTypeId",dbobject.OrderTypeId,DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SpecialInst", dbobject.SpecialInst.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefDetail", dbobject.RefDetail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemCategory",dbobject.ItemCategory,DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RevisionNo",dbobject.RevisionNo,DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Packing", dbobject.Packing.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Excise", dbobject.Excise.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SaleTax", dbobject.SaleTax.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrpMode", dbobject.TrpMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Freight", dbobject.Freight.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Insurance", dbobject.Insurance.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvPlace", dbobject.DelvPlace.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Inspection", dbobject.Inspection.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PaymentTerms", dbobject.PaymentTerms.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TDS", dbobject.TDS.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NetAmount",dbobject.NetAmount,DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VendorAddId", dbobject.VendorAddId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@OrderAmount", dbobject.OrderAmount, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Notes", dbobject.Notes, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Currency", dbobject.Currency, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentIds", dbobject.IndentIds, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentNo", dbobject.IndentNo, DbType.String));
        }
        //
        private List<OrderMdl> createObjectList(DataSet ds)
        {
            List<OrderMdl> orders = new List<OrderMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderMdl objmdl = new OrderMdl();
                objmdl.OrderId = Convert.ToInt32(dr["OrderId"].ToString());
                if (dr.Table.Columns.Contains("OrderNo"))
                {
                    objmdl.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());
                }
                if (dr.Table.Columns.Contains("OrderDate"))
                {
                    objmdl.OrderDate = mc.getStringByDate(Convert.ToDateTime(dr["OrderDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("VendorId"))
                {
                    objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                }
                if (dr.Table.Columns.Contains("VendorName"))
                {
                    objmdl.VendorName = dr["VendorName"].ToString();
                }
                if (dr.Table.Columns.Contains("DelvSchedule"))
                {
                    objmdl.DelvSchedule = dr["DelvSchedule"].ToString();
                }
                if (dr.Table.Columns.Contains("DelvDate"))
                {
                    objmdl.DelvDate = mc.getStringByDate(Convert.ToDateTime(dr["DelvDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("OrderTypeId"))
                {
                    objmdl.OrderTypeId = Convert.ToInt32(dr["OrderTypeId"].ToString());
                }
                if (dr.Table.Columns.Contains("OrderTypeName"))
                {
                    objmdl.OrderTypeName = dr["OrderTypeName"].ToString();
                }
                if (dr.Table.Columns.Contains("SpecialInst"))
                {
                    objmdl.SpecialInst = dr["SpecialInst"].ToString();
                }
                if (dr.Table.Columns.Contains("RefDetail"))
                {
                    objmdl.RefDetail = dr["RefDetail"].ToString();
                }
                if (dr.Table.Columns.Contains("ItemCategory"))
                {
                    objmdl.ItemCategory = dr["ItemCategory"].ToString();
                }
                if (dr.Table.Columns.Contains("RevisionNo"))
                {
                    objmdl.RevisionNo = dr["RevisionNo"].ToString();
                }
                if (dr.Table.Columns.Contains("Packing"))
                {
                    objmdl.Packing = dr["Packing"].ToString();
                }
                if (dr.Table.Columns.Contains("Excise"))
                {
                    objmdl.Excise = dr["Excise"].ToString();
                }
                if (dr.Table.Columns.Contains("SaleTax"))
                {
                    objmdl.SaleTax = dr["SaleTax"].ToString();
                }
                if (dr.Table.Columns.Contains("TrpMode"))
                {
                    objmdl.TrpMode = dr["TrpMode"].ToString();
                }
                if (dr.Table.Columns.Contains("Freight"))
                {
                    objmdl.Freight = dr["Freight"].ToString();
                }
                if (dr.Table.Columns.Contains("Insurance"))
                {
                    objmdl.Insurance = dr["Insurance"].ToString();
                }
                if (dr.Table.Columns.Contains("DelvPlace"))
                {
                    objmdl.DelvPlace = dr["DelvPlace"].ToString();
                }
                if (dr.Table.Columns.Contains("Inspection"))
                {
                    objmdl.Inspection = dr["Inspection"].ToString();
                }
                if (dr.Table.Columns.Contains("PaymentTerms"))
                {
                    objmdl.PaymentTerms = dr["PaymentTerms"].ToString();
                }
                if (dr.Table.Columns.Contains("TDS"))
                {
                    objmdl.TDS = dr["TDS"].ToString();
                }
                if (dr.Table.Columns.Contains("NetAmount"))
                {
                    objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("VendorAddId"))
                {
                    objmdl.VendorAddId = Convert.ToInt32(dr["VendorAddId"].ToString());
                }
                if (dr.Table.Columns.Contains("OrderAmount"))
                {
                    objmdl.OrderAmount = dr["OrderAmount"].ToString();
                }
                if (dr.Table.Columns.Contains("finyear"))
                {
                    objmdl.FinYear = dr["finyear"].ToString();
                }
                if (dr.Table.Columns.Contains("notes"))
                {
                    objmdl.Notes = dr["notes"].ToString();
                }
                if (dr.Table.Columns.Contains("Currency"))
                {
                    objmdl.Currency = dr["Currency"].ToString();
                }
                if (dr.Table.Columns.Contains("IndentIds"))//d
                {
                    objmdl.IndentIds = dr["IndentIds"].ToString();
                }
                if (dr.Table.Columns.Contains("IndentNo"))
                {
                    objmdl.IndentNo = dr["IndentNo"].ToString();
                }
                if (dr.Table.Columns.Contains("IsCancelled"))
                {
                    objmdl.IsCancelled = Convert.ToBoolean(dr["IsCancelled"].ToString());
                }
                if (dr.Table.Columns.Contains("CancelledOn"))
                {
                    objmdl.CancelledOn = mc.getStringByDate(Convert.ToDateTime(dr["CancelledOn"].ToString()));
                }
                if (dr.Table.Columns.Contains("Reason"))//d
                {
                    objmdl.Reason = dr["Reason"].ToString();
                }
                orders.Add(objmdl);
            }
            return orders;
        }
        //
        private void saveToLedger(SqlCommand cmd, OrderMdl dbobject)
        {
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                ledgerObj = new OrderLedgerMdl();
                ledgerObj.RecId = 0;
                ledgerObj.OrderId = Convert.ToInt32(dbobject.OrderId);
                ledgerObj.SlNo = i + 1;
                ledgerObj.ItemId = dbobject.Ledgers[i].ItemId;
                ledgerObj.ItemDesc = dbobject.Ledgers[i].ItemDesc;
                ledgerObj.OrdQty = dbobject.Ledgers[i].OrdQty;
                ledgerObj.DspQty = dbobject.Ledgers[i].DspQty;
                ledgerObj.UnitId = dbobject.Ledgers[i].UnitId;
                ledgerObj.Rate = dbobject.Ledgers[i].Rate;
                ledgerObj.Amount = dbobject.Ledgers[i].Amount;
                ledgerObj.Remarks = dbobject.Ledgers[i].Remarks;
                ledgerObj.IndentLgrId = dbobject.Ledgers[i].IndentLgrId;
                ledgerBLL.insertObject(cmd,dbobject.OrderId, ledgerObj);
            }
        }
        //
        private bool checkSetValidModel(OrderMdl dbobject)
        {
            Message = "";
            //if (dbobject.OrderDate == null)
            //{
            //    dbobject.OrderDate = "";
            //}
            //if (mc.isValidDateString(dbobject.OrderDate) == false)
            //{
            //    Message = "Invalid order date!";
            //    return false;
            //}
            if (dbobject.DelvDate == null)
            {
                dbobject.DelvDate = "";
            }
            if (mc.isValidDateString(dbobject.DelvDate) == false)
            {
                Message = "Invalid delv. date!";
                return false;
            }
            if (dbobject.VendorId == 0)
            {
                Message = "Vendor not selected!";
                return false;
            }
            if (dbobject.VendorAddId == 0)
            {
                Message = "Vendor address not selected!";
                return false;
            }
            if (dbobject.DelvSchedule == null)
            {
                dbobject.DelvSchedule = "";
            }
            if (dbobject.SpecialInst == null)
            {
                dbobject.SpecialInst = "";
            }
            if (dbobject.RefDetail == null)
            {
                dbobject.RefDetail = "";
            }
            if (dbobject.ItemCategory == null)
            {
                dbobject.ItemCategory = "";
            }
            if (dbobject.RevisionNo == null)
            {
                dbobject.RevisionNo = "0";
            }
            if (dbobject.Packing == null)
            {
                dbobject.Packing = "";
            }
            if (dbobject.Excise == null)
            {
                dbobject.Excise = "";
            }
            if (dbobject.SaleTax == null)
            {
                dbobject.SaleTax = "";
            }
            if (dbobject.TrpMode == null)
            {
                dbobject.TrpMode = "";
            }
            if (dbobject.Freight == null)
            {
                dbobject.Freight = "";
            }
            if (dbobject.Insurance == null)
            {
                dbobject.Insurance = "";
            }
            if (dbobject.DelvPlace == null)
            {
                dbobject.DelvPlace = "";
            }
            if (dbobject.Inspection == null)
            {
                dbobject.Inspection = "";
            }
            if (dbobject.PaymentTerms == null)
            {
                dbobject.PaymentTerms = "";
            }
            if (dbobject.TDS == null)
            {
                dbobject.TDS = "";
            }
            if (dbobject.Notes == null)
            {
                dbobject.Notes = "";
            }
            if (dbobject.IndentIds == null)
            {
                dbobject.IndentIds = "";
            }
            if (dbobject.IndentNo == null)
            {
                dbobject.IndentNo = "";
            }
            if (dbobject.OrderAmount == null)
            {
                dbobject.OrderAmount = "";
            }
            double amt = 0;
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].ItemId == 0)
                {
                    Message = "Invalid item entry not allowed!";
                    return false;
                }
                amt += dbobject.Ledgers[i].Amount;
                if (dbobject.Ledgers[i].Remarks == null)
                {
                    dbobject.Ledgers[i].Remarks = "";
                }
            }
            dbobject.NetAmount = amt;
            return true;
        }
        //
        internal bool isOrderUsedInExecution(SqlCommand cmd, int orderid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_is_order_used_in_execution";
            cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd,cmd.Connection));
        }
        //
        internal bool areValidIndentIdsForOrder(SqlCommand cmd,string indentids)
        {
            Message = "";
            //purchase without-indent
            if (indentids.Length == 0) { return true; };
            string[] indentid = indentids.Split(',');
            //check existence/validity of indentid
            for (int i = 0; i < indentid.Length; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_isvalid_indentid_for_order";
                cmd.Parameters.Add(mc.getPObject("@indentid", indentid[i].ToString(), DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd,cmd.Connection)) == false)
                {
                    Message = "Invalid Indent Id: +" + indentid[i].ToString();
                    return false;
                }
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(OrderMdl dbobject)
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
                if (areValidIndentIdsForOrder(cmd,dbobject.IndentIds) == false) { return; };
                if (dbobject.OrderNo == 0)//get new orderno
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_new_orderno";
                    cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                    dbobject.OrderNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                }
                else//chk dup
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_isfound_orderno";
                    cmd.Parameters.Add(mc.getPObject("@orderno", dbobject.OrderNo, DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
                    if (Convert.ToBoolean(mc.getFromDatabase(cmd,cmd.Connection)) == true)
                    {
                        Message = "Duplicate order number entry not allowed!";
                        return;
                    }
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_order";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.OrderId = Convert.ToInt32(mc.getRecentIdentityValue(cmd,dbTables.tbl_order,"orderid"));
                saveToLedger(cmd, dbobject);
                mc.setEventLog(cmd, dbTables.tbl_order, dbobject.OrderId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("OrderBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(OrderMdl dbobject)
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
                //ledger control
                DataSet ds = new DataSet();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_order_item_list_by_ledger";//stock ledger
                cmd.Parameters.Add(mc.getPObject("@orderid", dbobject.OrderId, DbType.Int32));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                ArrayList arl = new ArrayList();
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        arl.Add(ds.Tables[0].Rows[i]["orderlgrid"].ToString());
                    }
                }
                ds = new DataSet();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_tbl_orderledger";
                cmd.Parameters.Add(mc.getPObject("@orderid", dbobject.OrderId, DbType.Int32));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //if recid not found in stock ledger then delete it from order ledger
                        if (arl.Contains(ds.Tables[0].Rows[i]["recid"].ToString()) == false)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "usp_delete_orderledger_item_by_recid";
                            cmd.Parameters.Add(mc.getPObject("@recid", ds.Tables[0].Rows[i]["recid"].ToString(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    bool f= arl.Contains(dbobject.Ledgers[i].RecId);
                    if (arl.Contains(dbobject.Ledgers[i].RecId.ToString()))
                    {
                        //recid found- update existing order ledger record
                        ledgerBLL.updateObject(cmd, dbobject.OrderId, dbobject.Ledgers[i].RecId, dbobject.Ledgers[i]);
                    }
                    else//if recid not found- insert as new record
                    {
                        ledgerBLL.insertObject(cmd, dbobject.OrderId, dbobject.Ledgers[i]);
                    }
                }
                //updation
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_order";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@orderid", dbobject.OrderId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_order, dbobject.OrderId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_order") == true)
                {
                    Message = "Duplicate order number entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("OrderBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateOrderToCancelled(int orderid, string cancelledon, string reason)
        {
            Result = false;
            if (mc.isValidDateString(cancelledon) == false)
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
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_order_to_cancelled";
                cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@cancelledon", mc.getStringByDateToStore(cancelledon), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@reason", reason.Trim(), DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_order, orderid.ToString(), "Order Cancelled");
                Result = true;
                Message = "Order Cancelled Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("OrderBLL", "updateOrderToCancelled", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int orderid)
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
                //chk validation
                if (isOrderUsedInExecution(cmd, orderid) == true)
                {
                    Message = "This order has been used in execution, So it cannot be deleted.";
                    return;
                }
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_order";//deletion with ledger
                cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_order, orderid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("OrderBLL", "deleteObject", ex.Message);
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
        internal OrderMdl searchObject(int orderid)
        {
            DataSet ds = new DataSet();
            OrderMdl dbobject = new OrderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_order";
            cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getObjectList(orderid);
            return dbobject;
        }
        //
        internal OrderMdl getOrderBySlipNo(int slipno)
        {
            DataSet ds = new DataSet();
            OrderMdl dbobject = new OrderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_purchase_order_by_slipno";
            cmd.Parameters.Add(mc.getPObject("@slipno", slipno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getOrderLedgerBySlipNo(slipno);
            return dbobject;
        }
        //
        internal DataSet getObjectData(int vendorid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_order";
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<OrderMdl> getObjectList(int vendorid)
        {
            DataSet ds = getObjectData(vendorid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}