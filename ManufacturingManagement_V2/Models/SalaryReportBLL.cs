using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class SalaryReportBLL
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
        internal DataTable getSalaryReportData(int attmonth, int attyear, string attshift, int compcode, string grade, string rptname)
        {
            //[100118]/[100120]
            DataSet ds = new DataSet();
            SalaryBLL bllObject = new SalaryBLL();
            ds = bllObject.getObjectData(attmonth, attyear, attshift);
            bllObject.setAdditionalColumns(ds);
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
            if (rptname == "salaryslip" && mc.getPermission(Models.Entry.DirectorsInfo,permissionType.Add) == false)
            {
                rowfilter += " and grade = 'w'";
            }
            else if (rptname == "salarybankdetail")
            {
                if (grade == "w")
                {
                    rowfilter += " and grade = 'w'";
                }
                else
                {
                    rowfilter += " and grade <> 'w'";
                }
                rowfilter += " and grade not in ('a','c','l')";
            }
            else
            {
                if (grade.Length > 0)
                {
                    rowfilter += " and grade = '" + grade + "'";
                }
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
        internal DataSet GetSalarySlipReceiptReportHtml(int attmonth, int attyear, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_salary_slip_receipt_list";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetForm12AgencyReportHtml(int attmonth, int attyear, string grade, int agencyid, int locationid, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_form12_report_v2";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@agencyid", agencyid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@locationid", locationid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetSalaryNeftReportHtml(int attmonth, int attyear, string grade, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_salary_neft_report";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataTable getForm12ReportData(int attmonth,int attyear,string attshift,int compcode,string grade)
        {
            //[100120]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_form12_report";
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
            if (mc.getPermission(Models.Entry.DirectorsInfo, permissionType.Add) == false)
            {
                rowfilter += " and grade <> 'd'";
            }
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
        internal DataTable getIncentiveReportData(int attmonth, int attyear, string attshift, int compcode, string grade)
        {
            //[100101]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_incentive_report";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            //
            //prepare report data
            string monthname = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            string shiftname = mc.getNameByKey(mc.getShift(), "shiftid", attshift, "shiftname");
            string gradename = "";
            Message = "Incentive For the Month of " + monthname + ", " + attyear.ToString();
            if (grade.ToLower() == "d" || grade.ToLower() == "m" || grade.ToLower() == "s")
            {
                gradename = "Director, Manager and Staff";
            }
            else
            {
                gradename = mc.getNameByKey(EmployeeBLL.Instance.getEmployeeGradeData(), "gradecode", grade, "gradename");
            }
            Message += ", Grade: " + gradename;
            //string rowfilter = "1=1";
            //filter
            //ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn + ' ' + rptOption.SortOrder;
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            //filter string
            //if (rptOption.JoiningUnit > 0 && dtr.Rows.Count > 0)
            //{
            //    rptOption.RptHeader += " Unit: " + dtr.Rows[0]["JoiningUnitName"].ToString() + ", ";
            //}
            //rptOption.RptHeader = rptOption.RptHeader.Substring(0, rptOption.RptHeader.Length - 2);
            return dtr;
        }
        //
        internal DataTable getESIReturnData(int attmonth, int attyear, int compcode)
        {
            //[100121]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_esi_return_report";
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds.Tables[0].DefaultView.ToTable();
        }
        //
        internal DataTable getPFReturnData(int attmonth, int attyear, int compcode)
        {
            //[100122]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pf_return_report";
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds.Tables[0].DefaultView.ToTable();
        }
        //
        internal DataTable getNonPFReportData(int attmonth, int attyear, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_non_pf_report";
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr["empid"] = "";
                dr["empname"] = "NIL";
                dr["netpaid"] = "0";
                ds.Tables[0].Rows.Add(dr);
            }
            return ds.Tables[0].DefaultView.ToTable();
        }
        //
        internal DataTable getPFSummaryData(int attmonth, int attyear, int compcode)
        {
            //[100123]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pf_summary_report";
            cmd.Parameters.Add(mc.getPObject("@joiningunit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds.Tables[0].DefaultView.ToTable();
        }
        //
        internal DataTable getAnnualSalaryReturnData(int attyear, int compcode)
        {
            //[100124]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_annual_salary_return";
            cmd.Parameters.Add(mc.getPObject("@JoiningUnit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttYear", attyear, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataTable getBonusReportFormCData(int ccode, string finyear,string grade)
        {
            //[100098]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_bonus_report_form_c";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal double getCompanyWorkingDaysFinYear(int ccode, string finyear)
        {
            //[100098]
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_company_workingdays_finyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            return Convert.ToDouble(mc.getFromDatabase(cmd));
        }
        //
        #endregion
        //
    }
}