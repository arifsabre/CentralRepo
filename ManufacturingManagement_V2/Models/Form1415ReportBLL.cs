using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class Form1415ReportBLL
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
        internal DataTable getForm1415ReportData(int attyear, int newempid)
        {
            //[100094]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_form_14_15_report";
            cmd.Parameters.Add(mc.getPObject("@rptyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@IsRpt", "1", DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SetAttCal", "1", DbType.String));
            cmd.CommandTimeout = 180;//note
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
        internal DataTable getLeaveStatementData(int newempid, DateTime dtfrom, DateTime dtto)
        {
            //[100095]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_leave_statement_report";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.CommandTimeout = 180;//note
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataTable getLeaveSummaryData(DateTime dtfrom, DateTime dtto,int compcode, string grade)
        {
            //[100096]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_leave_summary_report";
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.CommandTimeout = 180;//note
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        #endregion
        //
    }
}