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
    public class DailyCashBalanceRptBLL : DbContext
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
                dr["vdate"] = dtacct.Rows[i]["vdate"].ToString();
                dr["balance"] = dtacct.Rows[i]["balance"].ToString();
                dtrpt.Rows.Add(dr);
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareDailyCashBalanceReport(DataTable dtrpt, int ccode, string finyear)
        {
            //[100078]
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_cashbalance_rpt";
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