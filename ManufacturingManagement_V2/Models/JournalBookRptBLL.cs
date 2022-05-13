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
    public class JournalBookRptBLL : DbContext
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
        private void fillDtRpt(DataTable dtrpt, DataTable dtacct)
        {
            dtrpt.Rows.Clear();
            DataRow dr = dtrpt.NewRow();
            for (int i = 0; i < dtacct.Rows.Count; i++)
            {
                dr = dtrpt.NewRow();
                dr["voucherid"] = dtacct.Rows[i]["voucherid"].ToString();
                dr["vtype"] = dtacct.Rows[i]["vtype"].ToString();
                dr["vno"] = dtacct.Rows[i]["vno"].ToString();
                dr["intvno"] = dtacct.Rows[i]["intvno"].ToString();
                dr["vdate"] = dtacct.Rows[i]["vdate"].ToString();
                dr["acdesc"] = dtacct.Rows[i]["acdesc"].ToString();
                dr["narration"] = dtacct.Rows[i]["narration"].ToString();
                dr["dramt"] = dtacct.Rows[i]["dramt"].ToString();
                dr["cramt"] = dtacct.Rows[i]["cramt"].ToString();
                dr["vtpvno"] = dtacct.Rows[i]["vtpvno"].ToString();
                dtrpt.Rows.Add(dr);
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareJournalBookReport(DateTime dtfrom, DateTime dtto, DataTable dtrpt, int ccode, string finyear)
        {
            //[100077]
            //dtVoucher
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_journalbook";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(dt, cmd);
            fillDtRpt(dtrpt, dt);
        }
        //
        #endregion
        //
    }
}