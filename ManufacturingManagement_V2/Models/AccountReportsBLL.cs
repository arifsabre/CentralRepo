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
    /// <summary>
    /// from dbProcedures/Account_Reports_SP.sql
    /// </summary>
    public class AccountReportsBLL : DbContext
    {
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        internal DataSet GetOpeningTrialReportHtml(int ccode, string finyr)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_openingtrial_rpt_html";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetTrialBalanceReportHtml(int ccode, string finyr, DateTime dtfrom, DateTime dtto, int grcode)
        {
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
                cmd.Parameters.Clear();
                //under txn with use of usp_process_trial_balance
                cmd.CommandText = "usp_get_trial_balance_rpt_html";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom, DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@dtto", dtto, DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@grcode", grcode, DbType.Int64));
                mc.fillFromDatabase(ds, cmd);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ds;
        }
        //
        internal DataSet GetBookReportHtml(reportName rptname, int ccode, int accode, DateTime dtfrom, DateTime dtto, bool printvtpvno, bool printnarr, string finyear)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            string res = "";
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("VDate", typeof(System.DateTime));
            ds.Tables[0].Columns.Add("CrPart", typeof(System.String));
            ds.Tables[0].Columns.Add("CrAmount", typeof(System.Double));
            ds.Tables[0].Columns.Add("DrPart", typeof(System.String));
            ds.Tables[0].Columns.Add("DrAmount", typeof(System.Double));
            //
            DataTable dttemprpt = new DataTable();
            dttemprpt.Columns.Add("crdesc");
            dttemprpt.Columns.Add("crnarr");
            dttemprpt.Columns.Add("cramt");
            dttemprpt.Columns.Add("drdesc");
            dttemprpt.Columns.Add("drnarr");
            dttemprpt.Columns.Add("dramt");
            dttemprpt.Columns.Add("crvtype");
            dttemprpt.Columns.Add("crvno");
            dttemprpt.Columns.Add("drvtype");
            dttemprpt.Columns.Add("drvno");
            DataRow drtemp = dttemprpt.NewRow();
            //
            int nod = mc.DateDifference(dtto, dtfrom) + 1;
            DateTime vdate = dtfrom;
            //
            for (int x = 0; x < nod; x++)
            {
                //getting opening balance by actrans for cash in hand (50)
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_opening_balance_by_actrans";//[100074]/[100075]/[100076]
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                res = mc.getFromDatabase(cmd);
                if (res == "") { res = "0"; };
                double opb = Convert.ToDouble(res);
                //getting opening cash for the day by voucher for cash in hand (50)
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_opening_balance_by_voucher";//[100074]/[100075]/[100076]
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                res = mc.getFromDatabase(cmd);
                if (res == "") { res = "0"; };
                opb = opb + Convert.ToDouble(res);
                //add first record as opening cash
                dttemprpt.Rows.Clear();
                drtemp = dttemprpt.NewRow();
                if (opb >= 0)//keeping in cr side
                {
                    drtemp["crdesc"] = "By Opening Cash";
                    drtemp["crnarr"] = "";
                    drtemp["cramt"] = opb.ToString("f2");
                    drtemp["drdesc"] = "";
                    drtemp["drnarr"] = "";
                    drtemp["dramt"] = "0";
                }
                else//if opb<0 then keeping in dr side
                {
                    drtemp["crdesc"] = "";
                    drtemp["crnarr"] = "";
                    drtemp["cramt"] = "0";
                    drtemp["drdesc"] = "To Opening Cash";
                    drtemp["drnarr"] = "";
                    drtemp["dramt"] = Math.Abs(opb).ToString("f2");
                }
                drtemp["crvtype"] = "";
                drtemp["crvno"] = "0";
                drtemp["drvtype"] = "";
                drtemp["drvno"] = "";
                dttemprpt.Rows.Add(drtemp);
                //getting records of credit entries from voucher
                cmd.Parameters.Clear();
                if (rptname == reportName.DayBook)
                {
                    cmd.CommandText = "usp_get_credit_vouchers_for_day_book";//[100074]/P1
                }
                else if (rptname == reportName.CashBook)
                {
                    cmd.CommandText = "usp_get_credit_vouchers_for_cash_book";//[100075]/P1
                }
                else if (rptname == reportName.BankBook)
                {
                    cmd.CommandText = "usp_get_credit_vouchers_for_bank_book";//[100076]/P1
                    cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                }
                cmd.Parameters.Add(mc.getPObject("@narr", printnarr.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vtvno", printvtpvno.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                DataTable dtCredit = new DataTable();
                mc.fillFromDatabase(dtCredit, cmd);
                //getting records of debit entries from voucher
                cmd.Parameters.Clear();
                if (rptname == reportName.DayBook)
                {
                    cmd.CommandText = "usp_get_debit_vouchers_for_day_book";//[100074]/P2
                }
                else if (rptname == reportName.CashBook)
                {
                    cmd.CommandText = "usp_get_debit_vouchers_for_cash_book";//[100075]/P2
                }
                else if (rptname == reportName.BankBook)
                {
                    cmd.CommandText = "usp_get_debit_vouchers_for_bank_book";//[100076]/P2
                    cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                }
                cmd.Parameters.Add(mc.getPObject("@narr", printnarr.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vtvno", printvtpvno.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                DataTable dtDebit = new DataTable();
                mc.fillFromDatabase(dtDebit, cmd);
                int cnt = dtCredit.Rows.Count < dtDebit.Rows.Count ? dtCredit.Rows.Count : dtDebit.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    drtemp = dttemprpt.NewRow();
                    drtemp["crdesc"] = dtCredit.Rows[i]["acdesc"].ToString();
                    drtemp["crnarr"] = dtCredit.Rows[i]["narration"].ToString();
                    drtemp["cramt"] = dtCredit.Rows[i]["cramt"].ToString();
                    drtemp["drdesc"] = dtDebit.Rows[i]["acdesc"].ToString();
                    drtemp["drnarr"] = dtDebit.Rows[i]["narration"].ToString();
                    drtemp["dramt"] = dtDebit.Rows[i]["dramt"].ToString();
                    drtemp["crvtype"] = dtCredit.Rows[i]["vtype"].ToString();
                    drtemp["crvno"] = dtCredit.Rows[i]["vno"].ToString();
                    drtemp["drvtype"] = dtDebit.Rows[i]["vtype"].ToString();
                    drtemp["drvno"] = dtDebit.Rows[i]["vno"].ToString();
                    dttemprpt.Rows.Add(drtemp);
                }
                if (dtCredit.Rows.Count != dtDebit.Rows.Count)
                {
                    if (dtCredit.Rows.Count > dtDebit.Rows.Count)
                    {
                        //add remaining credit side records
                        for (int i = cnt; i < dtCredit.Rows.Count; i++)
                        {
                            drtemp = dttemprpt.NewRow();
                            drtemp["crdesc"] = dtCredit.Rows[i]["acdesc"].ToString();
                            drtemp["crnarr"] = dtCredit.Rows[i]["narration"].ToString();
                            drtemp["cramt"] = dtCredit.Rows[i]["cramt"].ToString();
                            drtemp["drdesc"] = "";
                            drtemp["drnarr"] = "";
                            drtemp["dramt"] = "0";
                            drtemp["crvtype"] = dtCredit.Rows[i]["vtype"].ToString();
                            drtemp["crvno"] = dtCredit.Rows[i]["vno"].ToString();
                            drtemp["drvtype"] = "";
                            drtemp["drvno"] = "";
                            dttemprpt.Rows.Add(drtemp);
                        }
                    }
                    else//if debit records > credit records
                    {
                        //add remaining dedit side records
                        for (int i = cnt; i < dtDebit.Rows.Count; i++)
                        {
                            drtemp = dttemprpt.NewRow();
                            drtemp["crdesc"] = "";
                            drtemp["crnarr"] = "";
                            drtemp["cramt"] = "0";
                            drtemp["drdesc"] = dtDebit.Rows[i]["acdesc"].ToString();
                            drtemp["drnarr"] = dtDebit.Rows[i]["narration"].ToString();
                            drtemp["dramt"] = dtDebit.Rows[i]["dramt"].ToString();
                            drtemp["crvtype"] = "";
                            drtemp["crvno"] = "";
                            drtemp["drvtype"] = dtDebit.Rows[i]["vtype"].ToString();
                            drtemp["drvno"] = dtDebit.Rows[i]["vno"].ToString();
                            dttemprpt.Rows.Add(drtemp);
                        }
                    }
                }
                //
                double sumcr = 0;
                double sumdr = 0;
                //because sumcr will include opb by 0th record
                for (int i = 1; i < dttemprpt.Rows.Count; i++)
                {
                    sumcr = sumcr + Convert.ToDouble(dttemprpt.Rows[i]["cramt"].ToString());
                    sumdr = sumdr + Convert.ToDouble(dttemprpt.Rows[i]["dramt"].ToString());
                }
                //add last record as closing cash
                drtemp = dttemprpt.NewRow();
                double clg = opb + sumcr - sumdr;
                if (clg >= 0)//keeping in dr side
                {
                    drtemp["crdesc"] = "";
                    drtemp["crnarr"] = "";
                    drtemp["cramt"] = "0";
                    drtemp["drdesc"] = "To Closing Cash";
                    drtemp["drnarr"] = "";
                    drtemp["dramt"] = Math.Abs(clg).ToString("f2");
                }
                else//if clg<0 then keeping in cr side
                {
                    drtemp["crdesc"] = "By Closing Cash";
                    drtemp["crnarr"] = "";
                    drtemp["cramt"] = Math.Abs(clg).ToString("f2");
                    drtemp["drdesc"] = "";
                    drtemp["drnarr"] = "";
                    drtemp["dramt"] = "0";
                }
                drtemp["crvtype"] = "";
                drtemp["crvno"] = "";
                drtemp["drvtype"] = "";
                drtemp["drvno"] = "";
                dttemprpt.Rows.Add(drtemp);
                //
                //sending to ds.tbl
                DataRow dr = ds.Tables[0].NewRow();
                string particulars = "";
                for (int i = 0; i < dttemprpt.Rows.Count; i++)
                {
                    dr = ds.Tables[0].NewRow();
                    particulars = dttemprpt.Rows[i]["crdesc"].ToString();//case 00
                    if (printvtpvno == true && printnarr == true)//case 11
                    {
                        if (dttemprpt.Rows[i]["crvtype"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "[" + dttemprpt.Rows[i]["crvtype"].ToString() + dttemprpt.Rows[i]["crvno"].ToString() + "]";
                        }
                        if (dttemprpt.Rows[i]["crnarr"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "(" + dttemprpt.Rows[i]["crnarr"].ToString() + ")";
                        }
                    }
                    else if (printvtpvno == false && printnarr == true)//case 01
                    {
                        if (dttemprpt.Rows[i]["crnarr"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "(" + dttemprpt.Rows[i]["crnarr"].ToString() + ")";
                        }
                    }
                    else if (printvtpvno == true && printnarr == false)//case 10
                    {
                        if (dttemprpt.Rows[i]["crvtype"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "[" + dttemprpt.Rows[i]["crvtype"].ToString() + dttemprpt.Rows[i]["crvno"].ToString() + "]";
                        }
                    }
                    dr["crpart"] = particulars;
                    dr["cramount"] = dttemprpt.Rows[i]["cramt"].ToString();
                    particulars = dttemprpt.Rows[i]["drdesc"].ToString();//case 00
                    if (printvtpvno == true && printnarr == true)//case 11
                    {
                        if (dttemprpt.Rows[i]["drvtype"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "[" + dttemprpt.Rows[i]["drvtype"].ToString() + dttemprpt.Rows[i]["drvno"].ToString() + "]";
                        }
                        if (dttemprpt.Rows[i]["drnarr"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "(" + dttemprpt.Rows[i]["drnarr"].ToString() + ")";
                        }
                    }
                    else if (printvtpvno == false && printnarr == true)//case 01
                    {
                        if (dttemprpt.Rows[i]["drnarr"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "(" + dttemprpt.Rows[i]["drnarr"].ToString() + ")";
                        }
                    }
                    else if (printvtpvno == true && printnarr == false)//case 10
                    {
                        if (dttemprpt.Rows[i]["drvtype"].ToString() != "")
                        {
                            particulars = particulars + "<br/>" + "[" + dttemprpt.Rows[i]["drvtype"].ToString() + dttemprpt.Rows[i]["drvno"].ToString() + "]";
                        }
                    }
                    dr["drpart"] = particulars;
                    dr["dramount"] = dttemprpt.Rows[i]["dramt"].ToString();
                    //
                    dr["vdate"] = vdate;
                    ds.Tables[0].Rows.Add(dr);
                }
                //increment vdate by 1 day for next loop
                vdate = vdate.AddDays(1);
                //
            }
            return ds;
        }
        //
        internal DataSet GetJournalBookReportHtml(DateTime dtfrom, DateTime dtto, int ccode, string finyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_journal_book_report_html";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetDailyCashBalanceReportHtml(int ccode, string finyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_cashbalance_report_html";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetBalanceSheetReportHtml(int ccode, string finyr, DateTime vdate)
        {
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
                cmd.Parameters.Clear();
                //under txn with use of usp_process_trial_balance
                cmd.CommandText = "usp_get_balance_sheet_report_html";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate, DbType.DateTime));
                mc.fillFromDatabase(ds, cmd);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ds;
        }
        //
        internal DataSet GetProfitAndLossReportHtml(int ccode, string finyr, DateTime vdate)
        {
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
                cmd.Parameters.Clear();
                //under txn with use of usp_process_trial_balance
                cmd.CommandText = "usp_get_profit_and_loss_report_html";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vdate", vdate, DbType.DateTime));
                mc.fillFromDatabase(ds, cmd);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                throw ex;
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ds;
        }
        //
    }
}