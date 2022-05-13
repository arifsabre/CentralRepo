using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AdvanceReportBLL
    {
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region fetching objects
        //
        internal DataTable getAdvanceOutStanding(int compcode,DateTime vdate)
        {
            //[100062]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_advanceoutstanding";
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            //prepare report data
            //filter
            string rowfilter = "joiningunit='"+compcode.ToString()+"'";
            ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataTable getAdvanceDeductionList(int compcode, DateTime vdate)
        {
            //[100063]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_advancedeductionlist";
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            //prepare report data
            //filter
            string rowfilter = "joiningunit='" + compcode.ToString() + "'";
            ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataTable getEmployeeAdvanceDeductionLedger(int newempid,DateTime dtfrom,DateTime dtto)
        {
            //[100064]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_advance_deduction_ledger";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            //prepare report data
            //filter
            //string rowfilter = "joiningunit='" + rptOption.JoiningUnit + "'";
            //ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        #endregion
        //
    }
}