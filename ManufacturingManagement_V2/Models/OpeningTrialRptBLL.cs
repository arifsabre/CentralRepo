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
    public class OpeningTrialRptBLL : DbContext
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
                dr["accode"] = dtacct.Rows[i]["accode"].ToString();
                dr["acdesc"] = dtacct.Rows[i]["acdesc"].ToString();
                dr["yobdr"] = dtacct.Rows[i]["yobdr"].ToString();
                dr["yobcr"] = dtacct.Rows[i]["yobcr"].ToString();
                dtrpt.Rows.Add(dr);
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareOpeningTrialReport(int ccode,string finyr,DataTable dtrpt)
        {
            //[100071]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_openingtrial_rpt";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            fillDtRpt(dtrpt, ds.Tables[0].DefaultView.ToTable());
        }
        //
        #endregion
        //
    }
}