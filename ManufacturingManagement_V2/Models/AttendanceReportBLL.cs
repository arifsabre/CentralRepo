using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AttendanceReportBLL
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
        internal DataTable getAttendanceReportData(int attmonth,int attyear,string shift,int compcode,string grade)
        {
            //[100066]
            DataSet ds = new DataSet();
            AttendanceBLL bllObject = new AttendanceBLL();
            ds = bllObject.getObjectData(attmonth,attyear,shift);
            bllObject.setAdditionalColumns(ds);
            //prepare report data
            if (grade == null) { grade = ""; };
            string monthname= mc.getNameByKey(mc.getMonths(),"monthid",attmonth.ToString(),"monthname");
            string shiftname = mc.getNameByKey(mc.getShift(), "shiftid", shift, "shiftname");
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
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            //sorting
            //ds.Tables[0].DefaultView.Sort = rptOption.SortColumn +' '+rptOption.SortOrder;
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
        internal DataTable getAbsentSummaryData(int attmonth,int attyear,int compcode,string grade)
        {
            //[100069]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_absent_summary";
            cmd.Parameters.Add(mc.getPObject("@AttMonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttYear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@JoiningUnit", compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            //setting FOgt3CA values
            int cntx = 0;
            int cnt = 0;
            int g = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)//records
            {
                cntx = 0;
                cnt = 0;
                for (int d = 1; d <= 34; d++)//34th as group terminator
                {
                    if (g == 0) 
                    {
                        cnt = 0;
                    }
                    if (ds.Tables[0].Rows[i][d].ToString().ToLower() == "a")
                    {
                        cnt = cnt + 1;
                        g = 1;
                    }
                    else
                    {
                        g = 0;
                    }
                    if(cnt >= 3 && g == 0)
                    {
                        cntx = cntx + 1;
                        cnt = 0;
                    }
                }
                ds.Tables[0].Rows[i]["fogt3ca"] = cntx;
            }
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataTable getDailyAbsentReportData(int attday,int attmonth,int attyear,string shift,string grade,int compcode)
        {
            //[100068]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_rpt_get_attendance_value_for_the_day";
            cmd.Parameters.Add(mc.getPObject("@AttDay", attday, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttMonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttYear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttShift", shift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", compcode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            return dtr;
        }
        //
        internal DataSet getRemainingLeavesData(int attmonth, int attyear, int newempid)
        {
            //[100068]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_remaining_leaves_report_for_employee";
            cmd.Parameters.Add(mc.getPObject("@AttMonth", attmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttYear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}