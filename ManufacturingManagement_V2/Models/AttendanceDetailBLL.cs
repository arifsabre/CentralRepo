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
    public class AttendanceDetailBLL : DbContext
    {
        //
        internal DbSet<AttendanceDetailMdl> AttendanceDetails { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, AttendanceDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@AttDate", dbobject.AttDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@AttShift", dbobject.AttShift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AttValue", dbobject.AttValue, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AttHours", dbobject.AttHours, DbType.Double));
        }
        //
        internal void setAdditionalColumns(DataSet ds)
        {
            if (ds.Tables.Count == 0) { return; };
            //add columns
            ds.Tables[0].Columns.Add("ShiftName");
            ds.Tables[0].Columns.Add("GradeName");
            ds.Tables[0].Columns.Add("CategoryName");
            ds.Tables[0].Columns.Add("Designation");
            ds.Tables[0].Columns.Add("AttValueName");
            //set column values in ds
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["ShiftName"] = mc.getNameByKey(mc.getShift(), "shiftid", ds.Tables[0].Rows[i]["AttShift"].ToString(), "shiftname");
                ds.Tables[0].Rows[i]["GradeName"] = mc.getNameByKey(mc.getGrades(), "grade", ds.Tables[0].Rows[i]["Grade"].ToString(), "gradename");
                ds.Tables[0].Rows[i]["CategoryName"] = mc.getNameByKey(mc.getEmpCategory(), "categoryid", ds.Tables[0].Rows[i]["categoryId"].ToString(), "categoryname");
                ds.Tables[0].Rows[i]["Designation"] = mc.getNameByKey(mc.getDesignation(), "desigid", ds.Tables[0].Rows[i]["DesigId"].ToString(), "designation");
                ds.Tables[0].Rows[i]["AttValueName"] = mc.getNameByKey(mc.getAttedanceDetailType(), "attvalue", ds.Tables[0].Rows[i]["attvalue"].ToString(), "attvaluename");
            }
        }
        //
        private List<AttendanceDetailMdl> createObjectList(DataSet ds)
        {
            setAdditionalColumns(ds);
            List<AttendanceDetailMdl> attendances = new List<AttendanceDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AttendanceDetailMdl objmdl = new AttendanceDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.AttDate = Convert.ToDateTime(dr["AttDate"].ToString());
                objmdl.AttShift = dr["AttShift"].ToString();
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.AttValue = dr["AttValue"].ToString();
                objmdl.AttHours = Convert.ToDouble(dr["AttHours"].ToString());
                //additional columns
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
                objmdl.AttValueName = dr["AttValueName"].ToString();
                attendances.Add(objmdl);
            }
            return attendances;
        }
        //
        private bool setDuration(AttendanceDetailMdl dbobject)
        {
            //--notes v1 to get 90 minutes as 1:30 hours
            //int hours = (minutes - minutes % 60) / 60;
            //return hours + ":" + (minutes - hours * 60);
            //----------------------
            bool res = false;
            DateTime dtfrom = DateTime.Now;
            DateTime dtto = DateTime.Now;
            try
            {
                //dtfrom = Convert.ToDateTime(dbobject.TimeFrom);
                //dtto = Convert.ToDateTime(dbobject.TimeTo);
                if (dtfrom >= dtto)
                {
                    Message = "Invalid time duration!";
                    res = false;
                }
                TimeSpan ts = dtto - dtfrom;
                dbobject.AttHours = Convert.ToDouble(ts.Hours + Convert.ToDouble(ts.Minutes) / 60);
                res = true;
            }
            catch
            {
                Message = "Invalid time entered!";
                res = false;
            }
            return res;
        }
        //
        private bool isValidIncTime(string inctime)
        {
            string decimalValue = "0";
            var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
            if (regex.IsMatch(inctime))
            {
                decimalValue = regex.Match(inctime).Value;
            }
            if (Convert.ToDouble(decimalValue) == 0 || Convert.ToDouble(decimalValue) == 5 || Convert.ToDouble(decimalValue) == 50)
            {
                return true;
            }
            return false;
        }
        //
        internal bool isFoundAttendanceDetail(AttendanceDetailMdl dbobject)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_attendancedetail";
            cmd.Parameters.Add(mc.getPObject("@attdate", dbobject.AttDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@attshift", dbobject.AttShift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attvalue", dbobject.AttValue.Trim(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(AttendanceDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            EmployeeBLL empbll = new EmployeeBLL();
            string empgrade = empbll.getEmployeeGradeNewEmpId(dbobject.NewEmpId).ToLower();
            if (!(empgrade == "a" || empgrade == "l" || empgrade == "w"))
            {
                Message = "Selected employee is not from worker incentive grade!";
                return false;
            }
            if (mc.isValidDate(dbobject.AttDate) == false)
            {
                Message = "Invalid attendance date!";
                return false;
            }
            //SalaryBLL sbll = new SalaryBLL();
            //if (sbll.isSalaryLocked(dbobject.AttDate.Month, dbobject.AttDate.Year, dbobject.AttShift) == true)
            //{
            //    Message = "Salary for this month/year has been locked! So further entry/updation is not allowed!";
            //    return false;
            //}
            if (isValidIncTime(dbobject.AttHours.ToString()) == false)
            {
                Message = "Invalid attendance hours!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(AttendanceDetailMdl dbobject)
        {
            Result = false;
            //if (setDuration(dbobject) == false) { return; };
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundAttendanceDetail(dbobject) == true) { return; };
            if (dbobject.AttHours <= 0)
            {
                Message = "Invalid hours! It must be greater than 0.";
                return;
            }
            DateTime ctdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (dbobject.AttDate >= ctdate.AddDays(1))
            {
                Message = "Post-dated entry is not allowed!";
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
                cmd.CommandText = "usp_insert_tbl_attendancedetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_attendancedetail, "recid");
                mc.setEventLog(cmd, dbTables.tbl_attendancedetail, recid, "Inserted Single Entry");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AttendanceDetailBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setAttendanceDetailBatchwise(AttendanceDetailBatchMdl dbobject)
        {
            Result = false;
            ArrayList arlrecid = new ArrayList();
            DateTime attdate = new DateTime(dbobject.AttYear, dbobject.AttMonth, dbobject.AttDay);
            if (mc.isValidDate(attdate) == false)
            {
                Message = "Invalid attendance date!";
                return;
            }
            DateTime ctdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (attdate >= ctdate.AddDays(1))
            {
                Message = "Post-dated entry is not allowed!";
                return;
            }
            //SalaryBLL sbll = new SalaryBLL();
            //if (sbll.isSalaryLocked(attdate.Month, attdate.Year, dbobject.AttShift) == true)
            //{
            //    Message = "Salary for this month/year has been locked! So further entry/updation is not allowed!";
            //    return;
            //}
            for (int i = 0; i < dbobject.EmpAttendance.Count; i++)
            {
                if (mc.IsValidDouble(dbobject.EmpAttendance[i].AttHours.Trim()) == false)
                {
                    Message = "Invalid attendance hours!";
                    return;
                }
                if (Convert.ToDouble(dbobject.EmpAttendance[i].AttHours) > 0)
                {
                    if (isValidIncTime(dbobject.EmpAttendance[i].AttHours.Trim()) == false)
                    {
                        Message = "Invalid attendance hours!";
                        return;
                    }
                }
                arlrecid.Add(dbobject.EmpAttendance[i].RecId);//getting list of valid entries
            }
            //
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
                for (int i = 0; i < dbobject.EmpAttendance.Count; i++)
                {
                    if (arlrecid.Contains(dbobject.EmpAttendance[i].RecId))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_tbl_attendancedetail_for_batch";//AttValue= "inc"-only
                        cmd.Parameters.Add(mc.getPObject("@AttHours", dbobject.EmpAttendance[i].AttHours, DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@recid", dbobject.EmpAttendance[i].RecId.Trim(), DbType.Int32));
                        cmd.ExecuteNonQuery();
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_attendancedetail, attdate.Month.ToString()+"-"+attdate.Year.ToString(), "Inserted/updated Batchwise");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AttendanceBLL", "setAttendanceDetailBatchwise", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(AttendanceDetailMdl dbobject)
        {
            Result = false;
            //if (setDuration(dbobject) == false) { return; };
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_attendancedetail";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendancedetail, dbobject.RecId.ToString(), "Updated Single Entry");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("fk_tbl_attendancedetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else if (ex.Message.Contains("uk_tbl_attendancedetail") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("AttendanceDetailBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int recid)
        {
            Result = false;
            //AttendanceDetailMdl attdetmdl = new AttendanceDetailMdl();
            //attdetmdl = searchObject(recid);
            //if (attdetmdl.RecId > 0)
            //{
            //    SalaryBLL sbll = new SalaryBLL();
            //    if (sbll.isSalaryLocked(attdetmdl.AttDate.Month, attdetmdl.AttDate.Year, attdetmdl.AttShift) == true)
            //    {
            //        Message = "Salary for this month/year has been locked! So entry cannot be deleted!";
            //        return;
            //    }
            //}
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_attendancedetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendancedetail, recid.ToString(), "delete");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AttendanceDetailBLL", "deleteObject", ex.Message);
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
        internal AttendanceDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            AttendanceDetailMdl dbobject = new AttendanceDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_attendancedetail";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
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
        internal DataSet getObjectData(string dtfrom, string dtto, string attshift, int newempid,string attvalue,int joiningunit=0,string grade="")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_attendancedetail";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attvalue", attvalue, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@joiningunit", joiningunit, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AttendanceDetailMdl> getObjectList(string dtfrom, string dtto, string attshift, int newempid, string attvalue,int joiningunit)
        {
            DataSet ds = getObjectData(dtfrom, dtto, attshift, newempid, attvalue,joiningunit);
            return createObjectList(ds);
        }
        //
        internal AttendanceDetailBatchMdl generateAndGetAttendanceDetailBatchwise(int attday, int attmonth, int attyear, string attshift, string grade)
        {
            DateTime attdate = new DateTime(attyear, attmonth, attday);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_generate_attendancedetail_batchwise";
                cmd.Parameters.Add(mc.getPObject("@AttDate", attdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AttendanceDetailBLL", "generateAndGetAttendanceDetailBatchwise", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_attendancedetail_for_batch";
            cmd.Parameters.Add(mc.getPObject("@AttDate", attdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            AttendanceDetailBatchMdl attbatch = new AttendanceDetailBatchMdl();
            attbatch.AttMonth = attmonth;
            attbatch.AttYear = attyear;
            attbatch.AttShift = attshift;
            attbatch.AttDay = attday;
            attbatch.Grade = grade;
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
            List<EmployeeAttendanceDetailMdl> empatt = new List<EmployeeAttendanceDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeAttendanceDetailMdl objmdl = new EmployeeAttendanceDetailMdl();
                objmdl.RecId = dr["RecId"].ToString();
                objmdl.EmpId = dr["EmpId"].ToString();
                objmdl.NewEmpId = dr["NewEmpId"].ToString();
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.FatherName = dr["FatherName"].ToString();
                objmdl.AttValue = dr["AttValue"].ToString();
                objmdl.AttHours = dr["AttHours"].ToString();
                empatt.Add(objmdl);
            }
            attbatch.EmpAttendance = empatt;
            return attbatch;
        }
        //
        #endregion
        //
    }
}