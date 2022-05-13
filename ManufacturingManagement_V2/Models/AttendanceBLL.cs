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
    public class AttendanceBLL : DbContext
    {
        //
        //internal DbSet<AttendanceMdl> Attendances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, AttendanceMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@AttMonth", dbobject.AttMonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttYear", dbobject.AttYear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttShift", dbobject.AttShift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@D01", dbobject.D01.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D02", dbobject.D02.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D03", dbobject.D03.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D04", dbobject.D04.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D05", dbobject.D05.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D06", dbobject.D06.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D07", dbobject.D07.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D08", dbobject.D08.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D09", dbobject.D09.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D10", dbobject.D10.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D11", dbobject.D11.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D12", dbobject.D12.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D13", dbobject.D13.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D14", dbobject.D14.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D15", dbobject.D15.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D16", dbobject.D16.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D17", dbobject.D17.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D18", dbobject.D18.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D19", dbobject.D19.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D20", dbobject.D20.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D21", dbobject.D21.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D22", dbobject.D22.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D23", dbobject.D23.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D24", dbobject.D24.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D25", dbobject.D25.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D26", dbobject.D26.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D27", dbobject.D27.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D28", dbobject.D28.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D29", dbobject.D29.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D30", dbobject.D30.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@D31", dbobject.D31.Trim(), DbType.String));
        }
        //
        internal void setAdditionalColumns(DataSet ds)
        {
            if (ds.Tables.Count == 0) { return; };
            //add columns
            ds.Tables[0].Columns.Add("MonthName");
            ds.Tables[0].Columns.Add("ShiftName");
            ds.Tables[0].Columns.Add("GradeName");
            ds.Tables[0].Columns.Add("CategoryName");
            ds.Tables[0].Columns.Add("Designation");
            //set column values in ds
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["MonthName"] = mc.getNameByKey(mc.getMonths(), "monthid", ds.Tables[0].Rows[i]["AttMonth"].ToString(), "monthname");
                ds.Tables[0].Rows[i]["ShiftName"] = mc.getNameByKey(mc.getShift(), "shiftid", ds.Tables[0].Rows[i]["AttShift"].ToString(), "shiftname");
                ds.Tables[0].Rows[i]["GradeName"] = mc.getNameByKey(mc.getGrades(), "grade", ds.Tables[0].Rows[i]["Grade"].ToString(), "gradename");
                ds.Tables[0].Rows[i]["CategoryName"] = mc.getNameByKey(mc.getEmpCategory(), "categoryid", ds.Tables[0].Rows[i]["categoryId"].ToString(), "categoryname");
                ds.Tables[0].Rows[i]["Designation"] = mc.getNameByKey(mc.getDesignation(), "desigid", ds.Tables[0].Rows[i]["DesigId"].ToString(), "designation");
            }
        }
        //
        private List<AttendanceMdl> createObjectList(DataSet ds)
        {
            setAdditionalColumns(ds);
            List<AttendanceMdl> attendances = new List<AttendanceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AttendanceMdl objmdl = new AttendanceMdl();
                objmdl.AttendanceId = Convert.ToInt32(dr["AttendanceId"].ToString());
                objmdl.AttMonth = Convert.ToInt32(dr["AttMonth"].ToString());
                objmdl.AttYear = Convert.ToInt32(dr["AttYear"].ToString());
                objmdl.AttShift = dr["AttShift"].ToString();
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.D01 = dr["D01"].ToString().ToUpper();
                objmdl.D02 = dr["D02"].ToString().ToUpper();
                objmdl.D03 = dr["D03"].ToString().ToUpper();
                objmdl.D04 = dr["D04"].ToString().ToUpper();
                objmdl.D05 = dr["D05"].ToString().ToUpper();
                objmdl.D06 = dr["D06"].ToString().ToUpper();
                objmdl.D07 = dr["D07"].ToString().ToUpper();
                objmdl.D08 = dr["D08"].ToString().ToUpper();
                objmdl.D09 = dr["D09"].ToString().ToUpper();
                objmdl.D10 = dr["D10"].ToString().ToUpper();
                objmdl.D11 = dr["D11"].ToString().ToUpper();
                objmdl.D12 = dr["D12"].ToString().ToUpper();
                objmdl.D13 = dr["D13"].ToString().ToUpper();
                objmdl.D14 = dr["D14"].ToString().ToUpper();
                objmdl.D15 = dr["D15"].ToString().ToUpper();
                objmdl.D16 = dr["D16"].ToString().ToUpper();
                objmdl.D17 = dr["D17"].ToString().ToUpper();
                objmdl.D18 = dr["D18"].ToString().ToUpper();
                objmdl.D19 = dr["D19"].ToString().ToUpper();
                objmdl.D20 = dr["D20"].ToString().ToUpper();
                objmdl.D21 = dr["D21"].ToString().ToUpper();
                objmdl.D22 = dr["D22"].ToString().ToUpper();
                objmdl.D23 = dr["D23"].ToString().ToUpper();
                objmdl.D24 = dr["D24"].ToString().ToUpper();
                objmdl.D25 = dr["D25"].ToString().ToUpper();
                objmdl.D26 = dr["D26"].ToString().ToUpper();
                objmdl.D27 = dr["D27"].ToString().ToUpper();
                objmdl.D28 = dr["D28"].ToString().ToUpper();
                objmdl.D29 = dr["D29"].ToString().ToUpper();
                objmdl.D30 = dr["D30"].ToString().ToUpper();
                objmdl.D31 = dr["D31"].ToString().ToUpper();
                //additional columns
                objmdl.MonthName = dr["MonthName"].ToString();
                objmdl.ShiftName = dr["ShiftName"].ToString();
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.FatherName = dr["FatherName"].ToString();
                objmdl.Grade = dr["Grade"].ToString();
                objmdl.GradeName = dr["GradeName"].ToString();
                objmdl.JoiningUnit = Convert.ToInt32(dr["JoiningUnit"].ToString());
                objmdl.JoiningUnitName = dr["JoiningUnitName"].ToString();
                objmdl.WorkingUnit = Convert.ToInt32(dr["WorkingUnit"].ToString());
                objmdl.WorkingUnitName = dr["WorkingUnitName"].ToString();
                objmdl.CategoryId = dr["CategoryId"].ToString();
                objmdl.CategoryName = dr["CategoryName"].ToString();
                objmdl.RemainingCL = Convert.ToDouble(dr["RemainingCL"].ToString());
                //
                if (ds.Tables[0].Columns.Contains("d0x"))
                {
                    objmdl.D0X = dr["D0X"].ToString();
                }
                //
                attendances.Add(objmdl);
            }
            return attendances;
        }
        //
        private bool isValidAttendanceDate(int attday, int attmonth, int attyear)
        {
            DateTime attdate = new DateTime(attyear, attmonth, attday);
            DateTime ctdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (mc.isValidDate(attdate) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            if (attdate >= ctdate.AddDays(1))
            {
                if (mc.getPermission(Models.Entry.BackdateAttendanceUpdation, permissionType.Add) == false)
                {
                    Message = "Post-dated attendance entry is not allowed!";
                    return false;
                }
            }
            if (attdate < ctdate.AddDays(-2))
            {
                if (mc.getPermission(Models.Entry.BackdateAttendanceUpdation, permissionType.Add) == false)
                {
                    Message = "Backdate attendance entry (except 2 days) is not allowed!";
                    return false;
                }
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal bool isAttendanceFound(int attday, int attmonth, int attyear, string attshift)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_attendance_values_for_the_month";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                //particular day index value
                if (ds.Tables[0].Rows[i][attday].ToString().ToLower() == "p" || ds.Tables[0].Rows[i][attday].ToString().ToLower() == "wd" || ds.Tables[0].Rows[i][attday].ToString().ToLower() == "hld")
                {
                    Message = "Attendance entries for this day/shift already found!";
                    return true;
                }
            }
            return false;
        }
        //
        internal void generateAttendance(int attday,int attmonth,int attyear,string attshift,string attvalue,string allowmodify)
        {
            Result = false;
            DateTime dtchk = new DateTime(attyear,attmonth,attday);
            if (mc.isValidDateToSetRecord(dtchk) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return;
            }
            if (attday > 0)
            {
                if (isValidAttendanceDate(attday,attmonth,attyear) == false) { return; };
                if (isAttendanceFound(attday, attmonth, attyear, attshift) == true && allowmodify != "1")
                {
                    Message = "Attendance entries for this day/shift already found! Provide modify option to re-set attendance!";
                    return;
                }
                if (allowmodify == "1")
                {
                    if (mc.getPermission(Models.Entry.DirectorsInfo, permissionType.Add) == false)
                    {
                        Message = "Permission denied to re-set attendance!";
                        return;
                    }
                }
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                //note: includes usp_remove_attendance_records_for_inactive_employees
                cmd.CommandText = "usp_generate_attendance_record_for_month";
                cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
                cmd.ExecuteNonQuery();
                if (attday > 0)
                {
                    cmd.Parameters.Clear();
                    //with usp_set_attendance_calculation_for_the_month
                    //and usp_set_attendance_values_by_form_14_15  for the year
                    cmd.CommandText = "usp_set_attendance_for_the_day";
                    cmd.Parameters.Add(mc.getPObject("@attday", attday.ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@attvalue", attvalue.Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                    cmd.ExecuteNonQuery();
                    DataSet dsEmp = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_PostDatedJoinedEmployeesInTheMonth";
                    cmd.Parameters.Add(mc.getPObject("@attday", attday.ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                    mc.fillFromDatabase(dsEmp,cmd,cmd.Connection);
                    DateTime dtatt = new DateTime(attyear,attmonth,attday);
                    DateTime dtj = DateTime.Now;
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        dtj = Convert.ToDateTime(dsEmp.Tables[0].Rows[i]["JoiningDate"].ToString());
                        for (int x = 1; x <= dtj.Day; x++)//days upto joining in att month
                        {
                            cmd.Parameters.Clear();
                            //with usp_set_attendance_calculation by attid
                            //and usp_get_form_14_15_report for year/empid
                            cmd.CommandText = "usp_set_attendance_value_for_day_empid";//with cl-to-lwp change
                            cmd.Parameters.Add(mc.getPObject("@attday", x, DbType.Int32));
                            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth, DbType.Int32));
                            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
                            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
                            cmd.Parameters.Add(mc.getPObject("@newempid", dsEmp.Tables[0].Rows[i]["newempid"].ToString().Trim(), DbType.Int32));
                            cmd.Parameters.Add(mc.getPObject("@attvalue", "-", DbType.String));
                            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_attendance, attmonth.ToString() + "-" + attyear.ToString(), "Attendance Generated");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AttendanceBLL", "generateAttendance",ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void generateAttendanceForBackdateJoinedEmployees(int attmonth, int attyear)
        {
            Result = false;
            DateTime dtchk = new DateTime(attyear, attmonth, 1);
            if (mc.isValidDateToSetRecord(dtchk) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return;
            }
            if (isAttendanceFound(1, attmonth, attyear, "d") == false)
            {
                Message = "Attendance entry not found for the month/year!";
                return;
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_generate_attendance_for_backdate_joined_employees";
                cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendance, attmonth.ToString() + "-" + attyear.ToString(), "Attendance for back-date joined employees");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AttendanceBLL", "generateAttendanceForBackdateJoinedEmployees", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setAttendanceBatchwise(AttendanceBatchMdl dbobject)
        {
            Result = false;
            //note: by admin
            Message = "Restricted!";
            return;
            //
            //DateTime dtchk = new DateTime(dbobject.AttYear, dbobject.AttMonth, dbobject.AttDay);
            //if (mc.isValidDateToSetRecord(dtchk) == false)
            //{
            //    Message = "[Date check failed] Old record modification is not allowed!";
            //    return;
            //}
            //if (isValidAttendanceDate(dbobject.AttDay, dbobject.AttMonth, dbobject.AttYear) == false) { return; };
            //if (isAttendanceFound(dbobject.AttDay,dbobject.AttMonth, dbobject.AttYear, dbobject.AttShift) == false)
            //{
            //    Message = "Attendance entry not found for the day/shift!";
            //    return;
            //}
            //for (int i = 0; i < dbobject.EmpAttendance.Count; i++)
            //{
            //    if (mc.getNameByKey(getAttendanceValues(), "attcode", dbobject.EmpAttendance[i].AttValue.Trim(), "attname").Length == 0)
            //    {
            //        Message = "Invalid attendance option(s) found!";
            //        return;
            //    }
            //}
            ////
            //SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = mc.strconn;
            //SqlTransaction trn = null;
            //try
            //{
            //    if (conn.State == ConnectionState.Closed) { conn.Open(); };
            //    trn = conn.BeginTransaction();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Connection = conn;
            //    cmd.Transaction = trn;
            //    for (int i = 0; i < dbobject.EmpAttendance.Count; i++)
            //    {
            //        cmd.Parameters.Clear();
            //        //with usp_set_attendance_calculation by attid
            //        //and usp_get_form_14_15_report for year/empid
            //        cmd.CommandText = "usp_set_attendance_value_for_day_empid";//with cl-to-lwp change
            //        cmd.Parameters.Add(mc.getPObject("@attday", dbobject.AttDay, DbType.Int32));
            //        cmd.Parameters.Add(mc.getPObject("@attmonth", dbobject.AttMonth, DbType.Int32));
            //        cmd.Parameters.Add(mc.getPObject("@attyear", dbobject.AttYear, DbType.Int32));
            //        cmd.Parameters.Add(mc.getPObject("@attshift", dbobject.AttShift, DbType.String));
            //        cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.EmpAttendance[i].NewEmpId.Trim(), DbType.Int32));
            //        cmd.Parameters.Add(mc.getPObject("@attvalue", dbobject.EmpAttendance[i].AttValue.Trim(), DbType.String));
            //        cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            //        cmd.ExecuteNonQuery();
            //    }
            //    mc.setEventLog(cmd, dbTables.tbl_attendance, dbobject.AttDay.ToString() + "-" + dbobject.AttMonth.ToString() + "-" + dbobject.AttYear.ToString(), "Attendance Updation Batchwise");
            //    trn.Commit();
            //    Result = true;
            //    Message = "Record Saved Successfully";
            //}
            //catch (Exception ex)
            //{
            //    trn.Rollback();
            //    Message = mc.setErrorLog("AttendanceBLL", "setAttendanceBatchwise", ex.Message);
            //}
            //finally
            //{
            //    if (conn != null) { conn.Close(); };
            //}
        }
        //
        internal void updateObject(AttendanceMdl dbobject)
        {
            Result = false;
            //DateTime dtchk = new DateTime(dbobject.AttYear, dbobject.AttMonth, 1);
            //if (mc.isValidDateToSetRecord(dtchk) == false)
            //{
            //    Message = "[Date check failed] Old record modification is not allowed!";
            //    return;
            //}
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                //with usp_set_attendance_calculation by attid
                //and usp_get_form_14_15_report for year/empid
                cmd.CommandText = "usp_update_tbl_attendance";
                addCommandParameters(cmd, dbobject);//newempid for form_14/15
                cmd.Parameters.Add(mc.getPObject("@attendanceid", dbobject.AttendanceId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //history
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_generate_attendance_history";
                cmd.Parameters.Add(mc.getPObject("@attendanceid", dbobject.AttendanceId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@modifydt", DateTime.Now, DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@modifyuser", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                //log
                mc.setEventLog(cmd, dbTables.tbl_attendance, dbobject.AttendanceId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AttendanceBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal AttendanceMdl searchObject(int attendanceid)
        {
            DataSet ds = new DataSet();
            AttendanceMdl dbobject = new AttendanceMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_attendance";
            cmd.Parameters.Add(mc.getPObject("@attendanceid", attendanceid.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(int attmonth,int attyear,string attshift,int joiningunit=0,string grade="0")
        {
            //[100066]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_attendance";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@joiningunit", joiningunit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AttendanceMdl> getObjectList(int attmonth, int attyear, string attshift,int joiningunit = 0,string grade ="0")
        {
            DataSet ds = getObjectData(attmonth,attyear,attshift,joiningunit,grade);
            return createObjectList(ds);
        }
        //
        internal AttendanceBatchMdl getAttendanceBatchObject(int attday,int attmonth, int attyear, string attshift,int joiningunit,string grade)
        {
            DataSet ds = getObjectData(attmonth, attyear, attshift,joiningunit,grade);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)//setting particular day's att value
            {
                ds.Tables[0].Rows[i]["D0X"] = ds.Tables[0].Rows[i][attday].ToString().ToLower();
            }
            AttendanceBatchMdl attbatch = new AttendanceBatchMdl();
            attbatch.AttMonth = attmonth;
            attbatch.AttYear = attyear;
            attbatch.AttShift = attshift;
            attbatch.AttDay = attday;
            attbatch.JoiningUnit = joiningunit;
            attbatch.Grade = grade;
            if (joiningunit != 0)
            {
                CompanyMdl compMdl = new CompanyMdl();
                CompanyBLL compBLL = new CompanyBLL();
                compMdl = compBLL.searchObject(joiningunit);
                attbatch.UnitName = compMdl.CmpName;
            }
            else
            {
                attbatch.UnitName = "ALL";
            }
            if (grade != "0")
            {
                attbatch.GradeName = mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename");
            }
            else
            {
                attbatch.GradeName = "ALL";
            }
            attbatch.MonthName = mc.getNameByKey(mc.getMonths(), "monthid", attmonth.ToString(), "monthname");
            attbatch.ShiftName = mc.getNameByKey(mc.getShift(), "shiftid", attshift, "shiftname");
            List<EmployeeAttendanceMdl> empatt = new List<EmployeeAttendanceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeAttendanceMdl objmdl = new EmployeeAttendanceMdl();
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.NewEmpId = dr["NewEmpId"].ToString();
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.FatherName = dr["FatherName"].ToString();
                objmdl.AttValue = dr["D0X"].ToString();
                objmdl.remainingCL = dr["remainingCL"].ToString();
                empatt.Add(objmdl);
            }
            attbatch.EmpAttendance = empatt;
            return attbatch;
        }
        //
        private DataSet getAttendanceValues()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_attvalue";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AttendanceValuesMdl> getAttendanceValueList()
        {
            DataSet ds = new DataSet();
            ds = getAttendanceValues();
            List<AttendanceValuesMdl> attendances = new List<AttendanceValuesMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AttendanceValuesMdl objmdl = new AttendanceValuesMdl();
                objmdl.AttCode = dr["attcode"].ToString();
                objmdl.AttName = dr["attname"].ToString();
                attendances.Add(objmdl);
            }
            return attendances;
        }
        //
        internal DataSet getAttendanceHistoryData(int attendanceid)
        {
            //[100067]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_attendance_history";
            cmd.Parameters.Add(mc.getPObject("@attendanceid", attendanceid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getYearlyAttendanceData(int attyear, int ccode)
        {
            //[100175]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_yearly_working_days";
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}