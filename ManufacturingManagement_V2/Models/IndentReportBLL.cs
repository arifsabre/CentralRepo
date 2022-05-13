using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentReportBLL
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
        internal DataTable getIndentPurchaseSlipReportData(int vnofrom, int vnoto)
        {
            if (vnoto == 0) 
            {
                vnoto = vnofrom;
            }
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_indent_purchase_slip_report";
            cmd.Parameters.Add(mc.getPObject("@VNoFrom", vnofrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VNoTo", vnoto, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
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