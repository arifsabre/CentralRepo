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
    public class BalancePostingBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        /// <summary>
        /// Sets balance field of tbl_actrans to zero
        /// </summary>
        /// <param name="cmd"></param>
        private void setTrialBalanceToZero(SqlCommand cmd, int ccode, string finyear)
        {
            //[100072]/[100079]/[100080]
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_set_tbl_actrans_balance_to_zero";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        private void performBalancePostingByAcTrans(SqlCommand cmd, int ccode, string finyear)
        {
            //[100072]/[100079]/[100080]
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_actrans_records_for_trial_balance";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataSet dsAct = new DataSet();
            mc.fillFromDatabase(dsAct, cmd, cmd.Connection);
            AccountBLL objaccountbll = new AccountBLL();
            for (int i = 0; i < dsAct.Tables[0].Rows.Count; i++)
            {
                objaccountbll.performBalancePosting(cmd, dsAct.Tables[0].Rows[i]["accode"].ToString(), dsAct.Tables[0].Rows[i]["amount"].ToString(), ccode, finyear);
            }
        }
        //
        private DataTable performBalancePostingByVoucher(SqlCommand cmd, DateTime dtfrom, DateTime dtto, int ccode, string finyear)
        {
            //[100072]/[100079]/[100080]
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_voucher_records_for_trial_balance";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataSet dsVou = new DataSet();
            mc.fillFromDatabase(dsVou, cmd, cmd.Connection);
            AccountBLL objaccountbll = new AccountBLL();
            for (int i = 0; i < dsVou.Tables[0].Rows.Count; i++)
            {
                objaccountbll.performBalancePosting(cmd, dsVou.Tables[0].Rows[i]["accode"].ToString(), dsVou.Tables[0].Rows[i]["amount"].ToString(),ccode, finyear);
            }
            return dsVou.Tables[0];
        }
        //
        private DataTable performBalancePostingByVoucher(DateTime dtfrom, DateTime dtto, int ccode, string finyear)
        {
            //[100072]
            DataSet dsVou = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_voucher_records_for_trial_balance";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(dsVou, cmd);
            return dsVou.Tables[0];
        }
        //
        private DataTable getRecordsFromAcTrans(SqlCommand cmd, int ccode, string finyear)
        {
            //[100072]/[100079]/[100080]
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_actrans_records_for_revised_balance";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            return ds.Tables[0];
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void transferAccountBalances(string compcode, string finyearfrom, string finyearto)
        {
            Result = false;
            Message = "";
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
                cmd.CommandText = "usp_transfer_balance";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyearfrom", finyearfrom, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@finyearto", finyearto, DbType.String));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Account balance transfered successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BalancePostingDAL", "transferAccountBalances", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void transferStockOpeningForLedger(string compcode, string finyearfrom, string finyearto, DateTime vdate)
        {
            Result = false;
            Message = "";
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
                cmd.CommandText = "usp_transfer_stock_opening";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyearfrom", finyearfrom, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@finyearto", finyearto, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Item stock transfered successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BalancePostingDAL", "transferStockOpeningForLedger", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void transferBillOSReceipt(string oldfinyear, string newfinyear, int ccode)
        {
            Result = false;
            Message = "";
            BillOsRptBLL objbll = new BillOsRptBLL();
            DataTable dt = new DataTable();
            dt = objbll.prepareTempBillOSReceipt("0", oldfinyear, objCookie.getFromDate(), objCookie.getToDate(), "billos", ccode);
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
                cmd.CommandText = "usp_transferbillosr";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@newfinyear", newfinyear, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", objCookie.getToDate().AddDays(1).ToShortDateString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Bill outstanding transfered successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BalancePostingDAL", "transferBillOSReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void transferBillOSPayment(string oldfinyear, string newfinyear, int ccode)
        {
            Result = false;
            Message = "";
            BillOsRptBLL objbll = new BillOsRptBLL();
            DataTable dt = new DataTable();
            dt = objbll.prepareTempBillOSPayment("0", oldfinyear, objCookie.getFromDate(), objCookie.getToDate(), "billos", ccode);
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
                cmd.CommandText = "usp_transferbillosp";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@newfinyear", newfinyear, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", objCookie.getToDate().AddDays(1).ToShortDateString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Bill outstanding transfered successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BalancePostingDAL", "transferBillOSPayment", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataTable getRevisedRecordsAfterBalancePosting(DateTime dtfrom, DateTime dtto, int ccode, string finyear)
        {
            //[100072]/[100079]/[100080]
            DataTable dtactrans = new DataTable();
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
                setTrialBalanceToZero(cmd, ccode, finyear);
                //--old--to review its use
                //DateTime dtfromLogin = DateTime.Now;//*get from date
                //if (dtfromLogin == dtfrom)
                //{
                //    performBalancePostingByAcTrans(cmd, ccode, finyear);
                //}
                //new--modified instead of above
                performBalancePostingByAcTrans(cmd, ccode, finyear);
                //-----------------
                performBalancePostingByVoucher(cmd, dtfrom, dtto, ccode, finyear);
                dtactrans = getRecordsFromAcTrans(cmd, ccode, finyear);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                string str = ex.Message;
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return dtactrans;
        }
        //
        #endregion
        //
    }
}