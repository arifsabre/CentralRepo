using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SalaryReportArrearBLL
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
        internal DataTable getArrearReportData(int attmonth, int attyear, string attshift, int compcode, string grade, string rptname)
        {
            //[ArrearReport_SP/Arrear Report P2]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_arrear";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
            mc.fillFromDatabase(ds, cmd);

            //add columns 
            ds.Tables[0].Columns.Add("MonthName");
            ds.Tables[0].Columns.Add("ShiftName");
            ds.Tables[0].Columns.Add("GradeName");
            ds.Tables[0].Columns.Add("CategoryName");
            ds.Tables[0].Columns.Add("Designation");
            ds.Tables[0].Columns.Add("NetPaid", typeof(System.Decimal));
            //set column values in ds
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["MonthName"] = mc.getNameByKey(mc.getMonths(), "monthid", ds.Tables[0].Rows[i]["AttMonth"].ToString(), "monthname");
                ds.Tables[0].Rows[i]["ShiftName"] = mc.getNameByKey(mc.getShift(), "shiftid", ds.Tables[0].Rows[i]["AttShift"].ToString(), "shiftname");
                ds.Tables[0].Rows[i]["GradeName"] = mc.getNameByKey(mc.getGrades(), "grade", ds.Tables[0].Rows[i]["Grade"].ToString(), "gradename");
                ds.Tables[0].Rows[i]["CategoryName"] = mc.getNameByKey(mc.getEmpCategory(), "categoryid", ds.Tables[0].Rows[i]["categoryId"].ToString(), "categoryname");
                ds.Tables[0].Rows[i]["Designation"] = mc.getNameByKey(mc.getDesignation(), "desigid", ds.Tables[0].Rows[i]["DesigId"].ToString(), "designation");
                ds.Tables[0].Rows[i]["NetPaid"] = Convert.ToDouble(ds.Tables[0].Rows[i]["TotalEarned"].ToString()) - Convert.ToDouble(ds.Tables[0].Rows[i]["TotalDeduction"].ToString());
            }

            //
            //prepare report data
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string shiftname = mc.getNameByKey(mc.getShift(), "shiftid", attshift, "shiftname");
            Message = "For " + monthname + ", " + attyear.ToString() + " /" + shiftname + " Shift  ";
            string rowfilter = "1=1";
            //filter
            if (compcode > 0)
            {
                rowfilter += " and joiningunit = " + compcode + "";
            }
            if (grade.Length > 0)
            {
                rowfilter += " and grade = '" + grade + "'";
            }
            ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            //ds.Tables[0].DefaultView.Sort = "f12slno asc";
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            //filter string
            if (compcode > 0 && dtr.Rows.Count > 0)
            {
                Message += " Unit: " + dtr.Rows[0]["JoiningUnitName"].ToString() + ", ";
            }
            if (grade.Length > 0)
            {
                Message += " Grade: " + mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename") + ", ";
            }
            //
            Message = Message.Substring(0, Message.Length - 2);
            return dtr;
        }
        //
        internal DataTable getForm12ArrearReportData(int attmonth,int attyear,string attshift,int compcode,string grade)
        {
            //[ArrearReport_SP/Arrear Report P2]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_form12_arrear_report";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            AttendanceBLL bllObject = new AttendanceBLL();
            bllObject.setAdditionalColumns(ds);
            //
            //prepare report data
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string shiftname = mc.getNameByKey(mc.getShift(), "shiftid", attshift, "shiftname");
            Message = "For " + monthname + ", " + attyear.ToString() + " /" + shiftname + " Shift  ";
            string rowfilter = "1=1";
            //filter
            ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            //filter string
            if (compcode > 0 && dtr.Rows.Count > 0)
            {
                Message += " Unit: " + dtr.Rows[0]["JoiningUnitName"].ToString() + ", ";
            }
            if (grade.Length > 0)
            {
                Message += " Grade: " + mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename") + ", ";
            }
            //
            Message = Message.Substring(0, Message.Length - 2);
            return dtr;
        }
        //
        #endregion
        //
    }
}