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
    public class FinancialLedgerRptBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        public string Opening { get; set; }
        public string OpDrCr { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region fetching objects
        //
        internal void prepareFinancialLedger(string accode, string acdesc, DateTime dtfrom, DateTime dtto, DataTable dtrpt, int ccode, string finyear)
        {
            //[100073]
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            string res = "";
            dtrpt.Rows.Clear();
            //
            DataTable dttemprpt = new DataTable();
            dttemprpt.Columns.Add("vtype");
            dttemprpt.Columns.Add("vno");
            dttemprpt.Columns.Add("vdate");
            dttemprpt.Columns.Add("accode");
            dttemprpt.Columns.Add("acdesc");
            dttemprpt.Columns.Add("dramt");
            dttemprpt.Columns.Add("cramt");
            dttemprpt.Columns.Add("balance");
            dttemprpt.Columns.Add("narration");
            dttemprpt.Columns.Add("opening");
            dttemprpt.Columns.Add("accounthead");
            dttemprpt.Columns.Add("accountname");
            dttemprpt.Columns.Add("slno");
            //getting opening balance by actrans
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_opening_balance_by_actrans";//[100073]
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            res = mc.getFromDatabase(cmd);
            if (res == "") { res = "0"; };
            double opb = Convert.ToDouble(res);
            //getting opening balance by voucher
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_opening_balance_by_voucher";//[100073]
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@vdate", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            res = mc.getFromDatabase(cmd);
            if (res == "") { res = "0"; };
            opb = opb + Convert.ToDouble(res);
            //getting records by voucher
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_records_from_tbl_voucher";//[100073]
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataTable dtVou = new DataTable();
            mc.fillFromDatabase(dtVou, cmd, cmd.Connection);
            DataRow drtemp = dttemprpt.NewRow();
            dttemprpt.Rows.Clear();
            //setting op balance
            drtemp = dttemprpt.NewRow();
            drtemp["vtype"] = "";
            drtemp["vno"] = "0";
            drtemp["vdate"] = dtfrom.ToShortDateString();
            drtemp["accode"] = "0";
            drtemp["acdesc"] = "opb";
            drtemp["dramt"] = "0";
            drtemp["cramt"] = "0";
            drtemp["balance"] = "0";//to set further
            drtemp["narration"] = "opb";
            drtemp["opening"] = opb.ToString();
            drtemp["accounthead"] = "0";
            drtemp["accountname"] = acdesc;
            drtemp["slno"] = "0";
            dttemprpt.Rows.Add(drtemp);
            //
            for (int i = 0; i < dtVou.Rows.Count; i++)
            {
                drtemp = dttemprpt.NewRow();
                drtemp["vtype"] = dtVou.Rows[i]["vtype"].ToString();
                drtemp["vno"] = dtVou.Rows[i]["vno"].ToString();
                drtemp["vdate"] = dtVou.Rows[i]["vdate"].ToString();
                drtemp["accode"] = dtVou.Rows[i]["accontra"].ToString();
                drtemp["acdesc"] = dtVou.Rows[i]["acdesc"].ToString();
                drtemp["dramt"] = dtVou.Rows[i]["dramt"].ToString();
                drtemp["cramt"] = dtVou.Rows[i]["cramt"].ToString();
                drtemp["balance"] = "0";//to set further
                drtemp["narration"] = dtVou.Rows[i]["narration"].ToString();
                drtemp["opening"] = opb.ToString();
                drtemp["accounthead"] = accode;
                drtemp["accountname"] = acdesc;
                drtemp["slno"] = "0";
                dttemprpt.Rows.Add(drtemp);
            }
            //--setting balance & slno
            double balance = opb;
            for (int i = 0; i < dttemprpt.Rows.Count; i++)
            {
                balance = opb + Convert.ToDouble(dttemprpt.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dttemprpt.Rows[i]["cramt"].ToString());
                dttemprpt.Rows[i]["balance"] = balance.ToString();
                opb = balance;
                dttemprpt.Rows[i]["slno"] = i.ToString();
            }
            //
            //sending to maint dtrpt
            DataRow dr = dtrpt.NewRow();
            for (int i = 0; i < dttemprpt.Rows.Count; i++)
            {
                dr = dtrpt.NewRow();
                dr["vtype"] = dttemprpt.Rows[i]["vtype"].ToString();
                dr["vno"] = dttemprpt.Rows[i]["vno"].ToString();
                dr["vdate"] = dttemprpt.Rows[i]["vdate"].ToString();
                dr["accode"] = dttemprpt.Rows[i]["accode"].ToString();
                dr["acdesc"] = dttemprpt.Rows[i]["acdesc"].ToString();
                dr["dramt"] = dttemprpt.Rows[i]["dramt"].ToString();
                dr["cramt"] = dttemprpt.Rows[i]["cramt"].ToString();
                dr["balance"] = dttemprpt.Rows[i]["balance"].ToString();
                dr["narration"] = dttemprpt.Rows[i]["narration"].ToString();
                dr["opening"] = dttemprpt.Rows[i]["opening"].ToString();
                dr["accounthead"] = dttemprpt.Rows[i]["accounthead"].ToString();
                dr["accountname"] = dttemprpt.Rows[i]["accountname"].ToString();
                dr["slno"] = dttemprpt.Rows[i]["slno"].ToString();
                dtrpt.Rows.Add(dr);
            }
        }
        //
        internal void prepareFinancialLedger_1x(string accode, DateTime dtfrom, DateTime dtto, DataTable dtrpt, bool printhead, bool printnarr, int ccode, string finyear)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            string res = "";
            dtrpt.Rows.Clear();

            //getting opening balance by actrans
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_opening_balance_by_actrans";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            res = mc.getFromDatabase(cmd);
            if (res == "") { res = "0"; };
            double opb = Convert.ToDouble(res);
            //getting opening balance by voucher
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_opening_balance_by_voucher";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@vdate", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            res = mc.getFromDatabase(cmd);
            if (res == "") { res = "0"; };
            opb = opb + Convert.ToDouble(res);
            //getting records by voucher
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_records_from_tbl_voucher";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataTable dtVou = new DataTable();
            mc.fillFromDatabase(dtVou, cmd, cmd.Connection);
            DataRow dr = dtrpt.NewRow();
            dtrpt.Rows.Clear();
            //setting op balance
            Opening = opb.ToString("f2");
            if (opb < 0)
            {
                OpDrCr = "Cr";
            }
            else
            {
                OpDrCr = "Dr";
            }

            for (int i = 0; i < dtVou.Rows.Count; i++)
            {
                dr = dtrpt.NewRow();
                dr["vtype"] = dtVou.Rows[i]["vtype"].ToString();
                dr["vno"] = dtVou.Rows[i]["vno"].ToString();
                dr["vdate"] = dtVou.Rows[i]["vdate"].ToString();
                dr["accode"] = dtVou.Rows[i]["accontra"].ToString();
                dr["acdesc"] = dtVou.Rows[i]["acdesc"].ToString();
                dr["dramt"] = dtVou.Rows[i]["dramt"].ToString();
                dr["cramt"] = dtVou.Rows[i]["cramt"].ToString();
                dr["balance"] = "0";//to set further
                dr["narration"] = dtVou.Rows[i]["narration"].ToString();
                dr["opening"] = opb.ToString();
                dr["accounthead"] = accode;
                dr["accountname"] = "";
                dr["slno"] = "0";
                dr["particulars"] = "";
                dtrpt.Rows.Add(dr);
            }
            //--setting balance & slno
            double balance = opb;
            for (int i = 0; i < dtrpt.Rows.Count; i++)
            {
                balance = opb + Convert.ToDouble(dtrpt.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtrpt.Rows[i]["cramt"].ToString());
                dtrpt.Rows[i]["balance"] = Math.Abs(balance).ToString("f2");
                opb = balance;
                dtrpt.Rows[i]["slno"] = (i + 1).ToString();
                if (printhead && printnarr)
                {
                    dtrpt.Rows[i]["particulars"] = dtrpt.Rows[i]["acdesc"].ToString();
                    if (dtrpt.Rows[i]["narration"].ToString().Length > 0)
                    {
                        dtrpt.Rows[i]["particulars"] += " <" + dtrpt.Rows[i]["narration"].ToString() + ">";
                    }
                }
                else if (printhead && !printnarr)
                {
                    dtrpt.Rows[i]["particulars"] = dtrpt.Rows[i]["acdesc"].ToString();
                }
                else if (!printhead && printnarr)
                {
                    dtrpt.Rows[i]["particulars"] = dtrpt.Rows[i]["narration"].ToString();
                }
                if (balance < 0)
                {
                    dtrpt.Rows[i]["drcr"] = "Cr";
                }
                else
                {
                    dtrpt.Rows[i]["drcr"] = "Dr";
                }
            }
        }
        //
        internal void prepareFinancialLedgerDetail(string accode, string accontra, DateTime dtfrom, DateTime dtto, DataTable dtrpt, int ccode, string finyear)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            dtrpt.Rows.Clear();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recordsforvoucherdetail";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@accontra", accontra, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataTable dtVou = new DataTable();
            mc.fillFromDatabase(dtVou, cmd);
            DataRow dr = dtrpt.NewRow();
            for (int i = 0; i < dtVou.Rows.Count; i++)
            {
                dr = dtrpt.NewRow();
                dr["vtype"] = dtVou.Rows[i]["vtype"].ToString();
                dr["vno"] = dtVou.Rows[i]["vno"].ToString();
                dr["vdate"] = dtVou.Rows[i]["vdate"].ToString();
                dr["accode"] = dtVou.Rows[i]["accontra"].ToString();
                dr["acdesc"] = dtVou.Rows[i]["acdesccontra"].ToString();
                dr["dramt"] = "0";
                dr["cramt"] = "0";
                if (dtVou.Rows[i]["drcr"].ToString() == "d")
                {
                    dr["dramt"] = dtVou.Rows[i]["amount"].ToString();
                }
                else if (dtVou.Rows[i]["drcr"].ToString() == "c")
                {
                    dr["cramt"] = dtVou.Rows[i]["amount"].ToString();
                }
                dr["balance"] = "0";//to set further
                dr["narration"] = dtVou.Rows[i]["drcr"].ToString();
                dr["opening"] = "0";//opb.ToString();
                dr["accounthead"] = "0";
                dr["accountname"] = dtVou.Rows[i]["aliasname"].ToString();
                dr["slno"] = "0";
                dtrpt.Rows.Add(dr);
            }
            //--setting balance & slno
            double balance = 0;
            double opb = 0;
            for (int i = 0; i < dtrpt.Rows.Count; i++)
            {
                balance = opb + Convert.ToDouble(dtrpt.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtrpt.Rows[i]["cramt"].ToString());
                dtrpt.Rows[i]["balance"] = balance.ToString();
                opb = balance;
                dtrpt.Rows[i]["slno"] = i.ToString();
            }
        }
        //
        internal DataSet GetFinancialLedgerReportHtml(int accode, DateTime dtfrom, DateTime dtto, int ccode, string finyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_financial_ledger_report_html";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            DataTable dtVou = new DataTable();
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}