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
    public class AccountListRptBLL : DbContext
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
                dr["acdesc"] = dtacct.Rows[i]["acdesc"].ToString();
                dr["contper"] = dtacct.Rows[i]["contper"].ToString();
                dr["address1"] = dtacct.Rows[i]["address1"].ToString();
                dr["address2"] = dtacct.Rows[i]["address2"].ToString();
                dr["address3"] = dtacct.Rows[i]["address3"].ToString();
                dr["tinno"] = dtacct.Rows[i]["tinno"].ToString();
                dr["phoneoff"] = dtacct.Rows[i]["phoneoff"].ToString();
                dr["phoneresi"] = dtacct.Rows[i]["phoneresi"].ToString();
                dr["mobileno"] = dtacct.Rows[i]["mobileno"].ToString();
                dr["cityname"] = dtacct.Rows[i]["cityname"].ToString();
                dr["areaname"] = dtacct.Rows[i]["areaname"].ToString();
                dtrpt.Rows.Add(dr);
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareAccountListReport(DataTable dtrpt, string grcode, string areaid, string cityid, string stateid)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_account_detail";
            cmd.Parameters.Add(mc.getPObject("@grcode", grcode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@areaid", areaid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@cityid", cityid, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@stateid", stateid, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(dt, cmd);
            fillDtRpt(dtrpt, dt);
        }
        //
        #endregion
        //
    }
}