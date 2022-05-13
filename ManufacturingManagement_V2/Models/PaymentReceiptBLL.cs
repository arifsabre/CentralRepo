using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PaymentReceiptBLL : DbContext
    {
        //
        //internal DbSet<PaymentReceiptMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static PaymentReceiptBLL Instance
        {
            get { return new PaymentReceiptBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, PaymentReceiptMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@SaleRecId", dbobject.SaleRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@BillPerP1", dbobject.BillPerP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BillAmountP1", dbobject.BillAmountP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NeftDetailP1", dbobject.NeftDetailP1.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@RecAmountP1", dbobject.RecAmountP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExcessAmountP1", dbobject.ExcessAmountP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortAmountP1", dbobject.ShortAmountP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortAmtReasonP1", dbobject.ShortAmtReasonP1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RecoverableAmtP1", dbobject.RecoverableAmtP1, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RNoteQty", dbobject.RNoteQty, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BillPerP2", dbobject.BillPerP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BillAmountP2", dbobject.BillAmountP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NeftDetailP2", dbobject.NeftDetailP2.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@RecAmountP2", dbobject.RecAmountP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExcessAmountP2", dbobject.ExcessAmountP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortAmountP2", dbobject.ShortAmountP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortAmtReasonP2", dbobject.ShortAmtReasonP2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RecoverableAmtP2", dbobject.RecoverableAmtP2, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@LDCharge", dbobject.LDCharge, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EDDeduction", dbobject.EDDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VatDiff", dbobject.VatDiff, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RateDiff", dbobject.RateDiff, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Rejection", dbobject.Rejection, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@WtClaim", dbobject.WtClaim, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CalMist", dbobject.CalMist, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BankCharge", dbobject.BankCharge, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Miscellaneous", dbobject.Miscellaneous, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VNoP1", dbobject.VNoP1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNoP2", dbobject.VNoP2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SecurityAmount", dbobject.SecurityAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SDReceivedAmount", dbobject.SDReceivedAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SDRemarks", dbobject.SDRemarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdsDedSgst", dbobject.TdsDedSgst, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TdsDedIgst", dbobject.TdsDedIgst, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TcsDeduction", dbobject.TcsDeduction, DbType.Double));
            //to update rcinfo/rnoteinfo of sale
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RNoteInfo", dbobject.RNoteInfo.Trim(), DbType.String));
        }
        //
        private List<PaymentReceiptMdl> createObjectList(DataSet ds)
        {
            List<PaymentReceiptMdl> storeitems = new List<PaymentReceiptMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                PaymentReceiptMdl objmdl = new PaymentReceiptMdl();
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.BillPerP1 = Convert.ToDouble(dr["BillPerP1"].ToString());
                objmdl.BillAmountP1 = Convert.ToDouble(dr["BillAmountP1"].ToString());
                objmdl.BillNo = dr["BillNo"].ToString();//d
                objmdl.BillAmount = Convert.ToDouble(dr["BillAmount"].ToString());//d
                objmdl.PONumber = dr["PONumber"].ToString();//d
                objmdl.PayingAuthName = dr["PayingAuthName"].ToString();//d
                objmdl.BillDate = Convert.ToDateTime(dr["BillDate"].ToString());
                objmdl.NeftDetailP1 = Convert.ToDateTime(dr["NeftDetailP1"].ToString());
                objmdl.RecAmountP1 = Convert.ToDouble(dr["RecAmountP1"].ToString());
                objmdl.ExcessAmountP1 = Convert.ToDouble(dr["ExcessAmountP1"].ToString());
                objmdl.ShortAmountP1 = Convert.ToDouble(dr["ShortAmountP1"].ToString());
                objmdl.ShortAmtReasonP1 = dr["ShortAmtReasonP1"].ToString();
                objmdl.RecoverableAmtP1 = Convert.ToDouble(dr["RecoverableAmtP1"].ToString());
                objmdl.RNoteQty = Convert.ToDouble(dr["RNoteQty"].ToString());
                objmdl.BillPerP2 = Convert.ToDouble(dr["BillPerP2"].ToString());
                objmdl.BillAmountP2 = Convert.ToDouble(dr["BillAmountP2"].ToString());
                objmdl.NeftDetailP2 = Convert.ToDateTime(dr["NeftDetailP2"].ToString());
                objmdl.RecAmountP2 = Convert.ToDouble(dr["RecAmountP2"].ToString());
                objmdl.ExcessAmountP2 = Convert.ToDouble(dr["ExcessAmountP2"].ToString());
                objmdl.ShortAmountP2 = Convert.ToDouble(dr["ShortAmountP2"].ToString());
                objmdl.ShortAmtReasonP2 = dr["ShortAmtReasonP2"].ToString();
                objmdl.RecoverableAmtP2 = Convert.ToDouble(dr["RecoverableAmtP2"].ToString());
                objmdl.LDCharge = Convert.ToDouble(dr["LDCharge"].ToString());
                objmdl.EDDeduction = Convert.ToDouble(dr["EDDeduction"].ToString());
                objmdl.VatDiff = Convert.ToDouble(dr["VatDiff"].ToString());
                objmdl.RateDiff = Convert.ToDouble(dr["RateDiff"].ToString());
                objmdl.Rejection = Convert.ToDouble(dr["Rejection"].ToString());
                objmdl.WtClaim = Convert.ToDouble(dr["WtClaim"].ToString());
                objmdl.CalMist = Convert.ToDouble(dr["CalMist"].ToString());
                objmdl.BankCharge = Convert.ToDouble(dr["BankCharge"].ToString());
                objmdl.Miscellaneous = Convert.ToDouble(dr["Miscellaneous"].ToString());
                objmdl.TdsDedSgst = Convert.ToDouble(dr["TdsDedSgst"].ToString());
                objmdl.TdsDedIgst = Convert.ToDouble(dr["TdsDedIgst"].ToString());
                objmdl.TcsDeduction = Convert.ToDouble(dr["TcsDeduction"].ToString());
                objmdl.VNoP1 = dr["VNoP1"].ToString();
                objmdl.VNoP2 = dr["VNoP2"].ToString();
                objmdl.SecurityAmount = Convert.ToDouble(dr["SecurityAmount"].ToString());
                objmdl.SDReceivedAmount = Convert.ToDouble(dr["SDReceivedAmount"].ToString());
                objmdl.SDRemarks = dr["SDRemarks"].ToString();
                //from sale
                objmdl.Remarks = dr["Remarks"].ToString();//d by rcinfo of sale
                objmdl.RNoteInfo = dr["RNoteInfo"].ToString();//d
                if (dr["B1LetterDate"].ToString().Length > 0)
                {
                    objmdl.B1LetterDate = Convert.ToDateTime(dr["B1LetterDate"].ToString());//d
                }
                if (dr["B2LetterDate"].ToString().Length > 0)
                {
                    objmdl.B2LetterDate = Convert.ToDateTime(dr["B2LetterDate"].ToString());//d
                }
                storeitems.Add(objmdl);
            }
            return storeitems;
        }
        //
        private bool checkSetValidModel(PaymentReceiptMdl dbobject)
        {
            if (dbobject.SaleRecId == 0)
            {
                Message = "Bill not selected!";
                return false;
            }
            if (dbobject.RecAmountP1 > 0)
            {
                if (mc.isValidDate(dbobject.NeftDetailP1) == false)
                {
                    Message = "Invalid NEFT Date Part-1!";
                    return false;
                }
            }
            if (dbobject.RecAmountP2 > 0)
            {
                if (mc.isValidDate(dbobject.NeftDetailP2) == false)
                {
                    Message = "Invalid NEFT Date Part-2!";
                    return false;
                }
            }
            if (dbobject.SDReceivedAmount > dbobject.SecurityAmount)
            {
                Message = "Security received amount must not be greater than security amount!";
                return false;
            }
            if (dbobject.BillPerP2 == 0)
            {
                dbobject.NeftDetailP2 = new DateTime(1900,1,1);
                dbobject.ShortAmtReasonP2 = "";
                dbobject.RecAmountP2 = 0;
                dbobject.ExcessAmountP2 = 0;
                dbobject.ShortAmountP2 = 0;
                dbobject.RecoverableAmtP2 = 0;
            }
            if (dbobject.ShortAmountP1 + dbobject.ShortAmountP2 != dbobject.LDCharge + dbobject.EDDeduction + dbobject.VatDiff + dbobject.RateDiff + dbobject.Rejection + dbobject.WtClaim + dbobject.CalMist + dbobject.BankCharge + dbobject.Miscellaneous + dbobject.TdsDedSgst + dbobject.TdsDedIgst + dbobject.TcsDeduction)
            {
                Message = "Invalid deduction/short amount!";
                return false;
            }
            if (dbobject.BillAmountP1 + dbobject.ExcessAmountP1 != dbobject.RecAmountP1 + dbobject.ShortAmountP1 + dbobject.RecoverableAmtP1)
            {
                Message = "Invalid amounts for bill part 1!";
                return false;
            }
            if (dbobject.RecAmountP2 > 0)
            {
                if (dbobject.BillAmountP2 + dbobject.ExcessAmountP2 != dbobject.RecAmountP2 + dbobject.ShortAmountP2 + dbobject.RecoverableAmtP2)
                {
                    Message = "Invalid amounts for bill part 2!";
                    return false;
                }
            }

            if (dbobject.ShortAmtReasonP1 == null)
            {
                dbobject.ShortAmtReasonP1 = "";
            }
            if (dbobject.ShortAmtReasonP2 == null)
            {
                dbobject.ShortAmtReasonP2 = "";
            }
            if (dbobject.VNoP1 == null)
            {
                dbobject.VNoP1 = "";
            }
            if (dbobject.VNoP2 == null)
            {
                dbobject.VNoP2 = "";
            }
            if (dbobject.RNoteInfo == null)
            {
                dbobject.RNoteInfo = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.SDRemarks == null)
            {
                dbobject.SDRemarks = "";
            }

            return true;
        }
        //
        private bool isDispatchUnloaded(int salerecid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_isunloaded_dispatch";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updateReceipt(PaymentReceiptMdl dbobject)
        {
            Result = false;
            if (isDispatchUnloaded(dbobject.SaleRecId) == false)
            {
                Message = "Dispatch is not unloaded. Payment cannot be updated!";
                return;
            }
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
                cmd.CommandText = "usp_update_tbl_receipt";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_receipt, dbobject.SaleRecId.ToString(), "Updated");
                Result = true;
                Message = "Receipt Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PaymentReceiptBLL", "updateReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteReceipt(int salerecid)
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
                cmd.CommandText = "usp_delete_tbl_receipt";//resets sale attributes also
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_receipt, salerecid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PaymentReceiptBLL", "deleteReceipt", ex.Message);
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
        internal PaymentReceiptMdl searchReceipt(int salerecid)
        {
            DataSet ds = new DataSet();
            PaymentReceiptMdl dbobject = new PaymentReceiptMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_receipt_v2";//instead of usp_search_tbl_receipt
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
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
        internal DataSet getSaleInfo(int salerecid)
        {
            //usp_get_receiptinfo not to be used in V2
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_info";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getReceiptData(DateTime dtfrom, DateTime dtto, int payingauthid, int pmtstatus)
        {
            //usp_get_tbl_receipt not to be used in V2
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_receipt_v2";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@payingauthid", payingauthid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@pmtstatus", pmtstatus, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PaymentReceiptMdl> getReceiptList(DateTime dtfrom, DateTime dtto, int payingauthid, int pmtstatus)
        {
            DataSet ds = getReceiptData(dtfrom, dtto, payingauthid, pmtstatus);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}