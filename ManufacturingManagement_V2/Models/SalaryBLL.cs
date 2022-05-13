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
    public class SalaryBLL : DbContext
    {
        //
        internal DbSet<SalaryMdl> Salaries { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        /// <summary>
        /// not in use
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="dbobject"></param>
        private void addCommandParameters(SqlCommand cmd, SalaryMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@BasicRate", dbobject.BasicRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BasicEarn", dbobject.BasicEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DARate", dbobject.DARate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DAEarn", dbobject.DAEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ConvRate", dbobject.ConvRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ConvEarn", dbobject.ConvEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@HraRate", dbobject.HraRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@HraEarn", dbobject.HraEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@MedAllowRate", dbobject.MedAllowRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@MedAllowEarn", dbobject.MedAllowEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompAllowRate", dbobject.CompAllowRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompAllowEarn", dbobject.CompAllowEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DressWashRate", dbobject.DressWashRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DressWashEarn", dbobject.DressWashEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SpecialPayRate", dbobject.SpecialPayRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SpecialPayEarn", dbobject.SpecialPayEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OthersRate", dbobject.OthersRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OthersPayEarn", dbobject.OthersPayEarn, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@GrossSalary", dbobject.GrossSalary, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IncHours", dbobject.IncHours, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TotalEarned", dbobject.TotalEarned, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PFRate", dbobject.PFRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PFDeduction", dbobject.PFDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FpfAmount", dbobject.FpfAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EpfAmount", dbobject.EpfAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ESIRate", dbobject.ESIRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ESIDeduction", dbobject.ESIDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@AdvDeduction", dbobject.AdvDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FineDeduction", dbobject.FineDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortLeaveRate", dbobject.ShortLeaveRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ShortLeaveDed", dbobject.ShortLeaveDed, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@LateAttRate", dbobject.LateAttRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@LateAttDed", dbobject.LateAttDed, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TDSRate", dbobject.TDSRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TDSDeduction", dbobject.TDSDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IsPaymentAck", dbobject.IsPaymentAck, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsProcessed", dbobject.IsProcessed, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BonusAmount", "0", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BonusPer", "0", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@MinWage", "0", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IncAmount", dbobject.IncAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IncEarn", dbobject.IncEarn, DbType.Double));
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
            ds.Tables[0].Columns.Add("NetPaid",typeof(System.Decimal));
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
        }
        //
        private List<SalaryMdl> createObjectList(DataSet ds)
        {
            setAdditionalColumns(ds);
            List<SalaryMdl> salaries = new List<SalaryMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SalaryMdl objmdl = new SalaryMdl();
                objmdl.AttMonth = Convert.ToInt32(dr["AttMonth"].ToString());
                objmdl.AttYear = Convert.ToInt32(dr["AttYear"].ToString());
                objmdl.AttShift = dr["AttShift"].ToString();
                objmdl.AttendanceId = Convert.ToInt32(dr["AttendanceId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.BasicRate = Convert.ToDouble(dr["BasicRate"].ToString());
                objmdl.BasicEarn = Convert.ToDouble(dr["BasicEarn"].ToString());
                objmdl.DARate = Convert.ToDouble(dr["DARate"].ToString());
                objmdl.DAEarn = Convert.ToDouble(dr["DAEarn"].ToString());
                objmdl.ConvRate = Convert.ToDouble(dr["ConvRate"].ToString());
                objmdl.ConvEarn = Convert.ToDouble(dr["ConvEarn"].ToString());
                objmdl.HraRate = Convert.ToDouble(dr["HraRate"].ToString());
                objmdl.HraEarn = Convert.ToDouble(dr["HraEarn"].ToString());
                objmdl.MedAllowRate = Convert.ToDouble(dr["MedAllowRate"].ToString());
                objmdl.MedAllowEarn = Convert.ToDouble(dr["MedAllowEarn"].ToString());
                objmdl.CompAllowRate = Convert.ToDouble(dr["CompAllowRate"].ToString());
                objmdl.CompAllowEarn = Convert.ToDouble(dr["CompAllowEarn"].ToString());
                objmdl.DressWashRate = Convert.ToDouble(dr["DressWashRate"].ToString());
                objmdl.DressWashEarn = Convert.ToDouble(dr["DressWashEarn"].ToString());
                objmdl.SpecialPayRate = Convert.ToDouble(dr["SpecialPayRate"].ToString());
                objmdl.SpecialPayEarn = Convert.ToDouble(dr["SpecialPayEarn"].ToString());
                objmdl.OthersRate = Convert.ToDouble(dr["OthersRate"].ToString());
                objmdl.OthersPayEarn = Convert.ToDouble(dr["OthersPayEarn"].ToString());
                objmdl.GrossSalary = Convert.ToDouble(dr["GrossSalary"].ToString());
                objmdl.IncHours = Convert.ToDouble(dr["IncHours"].ToString());
                objmdl.TotalEarned = Convert.ToDouble(dr["TotalEarned"].ToString());
                objmdl.PFRate = Convert.ToDouble(dr["PFRate"].ToString());
                objmdl.PFDeduction = Convert.ToDouble(dr["PFDeduction"].ToString());
                objmdl.FpfAmount = Convert.ToDouble(dr["FpfAmount"].ToString());
                objmdl.EpfAmount = Convert.ToDouble(dr["EpfAmount"].ToString());
                objmdl.ESIRate = Convert.ToDouble(dr["ESIRate"].ToString());
                objmdl.ESIDeduction = Convert.ToDouble(dr["ESIDeduction"].ToString());
                objmdl.AdvDeduction = Convert.ToDouble(dr["AdvDeduction"].ToString());
                objmdl.FineDeduction = Convert.ToDouble(dr["FineDeduction"].ToString());
                objmdl.ShortLeaveRate = Convert.ToDouble(dr["ShortLeaveRate"].ToString());
                objmdl.ShortLeaveDed = Convert.ToDouble(dr["ShortLeaveDed"].ToString());
                objmdl.LateAttRate = Convert.ToDouble(dr["LateAttRate"].ToString());
                objmdl.LateAttDed = Convert.ToDouble(dr["LateAttDed"].ToString());
                objmdl.TDSRate = Convert.ToDouble(dr["TDSRate"].ToString());
                objmdl.TDSDeduction = Convert.ToDouble(dr["TDSDeduction"].ToString());
                objmdl.TotalDeduction = Convert.ToDouble(dr["TotalDeduction"].ToString());
                objmdl.IsPaymentAck = Convert.ToBoolean(dr["IsPaymentAck"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.IncAmount = Convert.ToDouble(dr["IncAmount"].ToString());
                objmdl.IncEarn = Convert.ToDouble(dr["IncEarn"].ToString());
                objmdl.IncAmount = Convert.ToDouble(dr["IncAmount"].ToString());
                //calculated fields
                objmdl.IsProcessed = Convert.ToBoolean(dr["IsProcessed"].ToString());
                objmdl.NetPaid = Convert.ToDouble(dr["NetPaid"].ToString());
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
                salaries.Add(objmdl);
            }
            return salaries;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal bool isAttendanceFound(int attmonth, int attyear, string attshift)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_attendance";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == false)
            {
                Message = "Attendance record(s) not found for this month/shift!";
                return false;
            }
            else
            {
                return true;
            }
        }
        //
        internal bool isSalaryRecordFound(int attmonth, int attyear, string attshift)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_salary";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Salary has been prepared this month/shift!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        internal bool checkForResignedEmployees()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_check_for_resigned_employees";
            mc.fillFromDatabase(ds,cmd);
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["result"].ToString()) == false)
            {
                Message = ds.Tables[0].Rows[0]["msg"].ToString();
                return false;
            }
            return true;
        }
        //
        internal bool checkForMinimumWage(int attmonth,int attyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_check_for_minwage";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["result"].ToString()) == false)
            {
                Message = ds.Tables[0].Rows[0]["msg"].ToString();
                return false;
            }
            return true;
        }
        //
        internal void generateSalary(int attmonth, int attyear, string attshift, string allowmodify)
        {
            Result = false;
            if (mc.isValidDateToSetRecord(new DateTime(attyear,attmonth,1)) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return;
            }
            if (checkForResignedEmployees() == false) { return; };
            if (checkForMinimumWage(attmonth,attyear) == false) { return; };
            if (isAttendanceFound(attmonth, attyear, attshift) == false) { return; };
            if (isSalaryRecordFound(attmonth, attyear, attshift) == true && allowmodify != "1")
            {
                Message = "Salary has been prepared for this month/shift! Provide modify option to re-set salary!";
                return;
            }
            DateTime dtpm = new DateTime(attyear, attmonth, 1).AddMonths(-1);
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
                cmd.CommandText = "usp_generate_salary";
                cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@sldate", mc.getCurrentDateStringForSql(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@allowmodify", allowmodify, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.CommandTimeout = 300;//note
                cmd.ExecuteNonQuery();
                string logdesc = "Salary Generated";
                if (allowmodify == "1") { logdesc = "Salary Re-Generated"; };
                mc.setEventLog(cmd,dbTables.tbl_salary,attmonth.ToString() + "-" + attyear.ToString(),logdesc);
                trn.Commit();
                Result = true;
                Message = logdesc + " Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryBLL", "generateSalary", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void generateIncentive(int attmonth, int attyear, string attshift, string allowmodify)
        {
            Result = false;
            //if (isAttendanceFound(attmonth, attyear, attshift) == false) { return; };
            //if (isSalaryRecordFound(attmonth, attyear, attshift) == true && allowmodify != "1")
            //{
            //    Message = "Salary has been prepared for this month/shift! Provide modify option to re-set salary!";
            //    return;
            //}
            //DateTime dtpm = new DateTime(attyear, attmonth, 1).AddMonths(-1);
            //if (isSalaryLocked(dtpm.Month, dtpm.Year, attshift) == false)
            //{
            //    Message = "Previous month's salary has not been confirmed!";
            //    return;
            //}
            //if (isSalaryLocked(attmonth, attyear, attshift) == true) { return; };
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
                cmd.CommandText = "usp_generate_incentive";
                cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@sldate", mc.getCurrentDateStringForSql(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendancedetail, attmonth.ToString() + "-" + attyear.ToString(), "Incentive Generation");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryBLL", "generateIncentive", ex.Message);
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
        internal SalaryMdl searchObject(int attendanceid)
        {
            DataSet ds = new DataSet();
            SalaryMdl dbobject = new SalaryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_salary";
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
        internal DataSet getObjectData(int attmonth, int attyear, string attshift)
        {
            //[100118]/[100120]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_salary";
            cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SalaryMdl> getObjectList(int attmonth, int attyear, string attshift)
        {
            DataSet ds = getObjectData(attmonth, attyear, attshift);
            return createObjectList(ds);
        }
        //
        internal SetDeductionAmountMdl getAdvanceOSToSetDeductionList(DateTime vdate)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_advanceoutstanding";
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            SetDeductionAmountMdl objmdl = new SetDeductionAmountMdl();
            objmdl.VDate = mc.getStringByDate(vdate);
            List<DeductionListMdl> dedlst = new List<DeductionListMdl> { };
            foreach (DataRow dr in dtr.Rows)
            {
                DeductionListMdl dedmdl = new DeductionListMdl();
                dedmdl.EmpId = dr["EmpId"].ToString();//d
                dedmdl.NewEmpId = dr["newempid"].ToString();
                dedmdl.EmpName = dr["EmpName"].ToString();
                dedmdl.Balance = Convert.ToDouble(dr["Balance"].ToString());
                dedmdl.InstAmount = Convert.ToDouble(dr["Installment"].ToString());
                dedmdl.DeductionAmt = Convert.ToDouble(dr["DeductionAmt"].ToString());
                dedmdl.Remarks = dr["Remarks"].ToString();
                dedlst.Add(dedmdl);
            }
            objmdl.DeductionList = dedlst;
            return objmdl;
        }
        //
        internal void setAdvanceDeductionList(SetDeductionAmountMdl dbobject)
        {
            Result = false;
            for (int i = 0; i < dbobject.DeductionList.Count; i++)
            {
                if (dbobject.DeductionList[i].DeductionAmt > dbobject.DeductionList[i].Balance)
                {
                    Message = "Invalid deduction amount!\r\nIt must not be greater than balance.\r\n" + dbobject.DeductionList[i].EmpName.Trim();
                    return;
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
                cmd.CommandText = "usp_reset_deduction_amount";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.DeductionList.Count; i++)
                {
                    if (Convert.ToDouble(dbobject.DeductionList[i].DeductionAmt)>0)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_set_deduction_amount";
                        cmd.Parameters.Add(mc.getPObject("@newEmpId", dbobject.DeductionList[i].NewEmpId.Trim(), DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@DeductionAmt", dbobject.DeductionList[i].DeductionAmt, DbType.Double));
                        cmd.ExecuteNonQuery();
                    }
                }
                trn.Commit();
                Result = true;
                Message = "Record(s) Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryBLL", "setAdvanceDeductionList", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateSalaryForAdvanceDeduction(int attmonth, int attyear, string attshift)
        {
            Result = false;
            if (mc.isValidDateToSetRecord(new DateTime(attyear, attmonth, 1)) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
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
                cmd.CommandText = "usp_update_salary_for_advance_deduction";
                cmd.Parameters.Add(mc.getPObject("@attmonth", attmonth.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", attyear.ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attshift", attshift.ToString(), DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_salary, attmonth.ToString() + "-" + attyear.ToString(), "Salary Updation for Advance Deduction");
                trn.Commit();
                Result = true;
                Message = "Record(s) Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryBLL", "updateSalaryForAdvanceDeduction", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateAdvanceDeductionToSalarySingle(AdvanceMdl dbobject)
        {
            Result = false;
            DateTime dtchk = new DateTime(dbobject.AdvDate.Year, dbobject.AdvDate.Month, dbobject.AdvDate.Day);
            if (mc.isValidDateToSetRecord(dtchk) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return;
            }
            if (isSalaryRecordFound(dbobject.AdvDate.Month, dbobject.AdvDate.Year, "d") == false)
            {
                Message = "Salary has not been generated for the month/year!";
                return;
            }
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return;
            }
            if (dbobject.InstAmount < 0)
            {
                Message = "Invalid deduction amount!";
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
                cmd.CommandText = "usp_advance_deduction_to_salary_single";
                cmd.Parameters.Add(mc.getPObject("@attmonth", dbobject.AdvDate.Month, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@attyear", dbobject.AdvDate.Year, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@amount", dbobject.InstAmount, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.NewEmpId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_salary, dbobject.AdvDate.Month.ToString() + "-" + dbobject.AdvDate.Year.ToString() + " EmpId: " + dbobject.NewEmpId.ToString(), "Advance Updation Single Employee");
                trn.Commit();
                Message = "Advance Deduction Updated Successfully To Salary";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryBLL", "updateAdvanceDeductionToSalarySingle", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateAdvanceDeductionInstallment(AdvanceMdl dbobject)
        {
            Result = false;
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return;
            }
            if (dbobject.InstAmount < 0)
            {
                Message = "Invalid deduction amount!";
                return;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_advance_installment";
                cmd.Parameters.Add(mc.getPObject("@instamount", dbobject.InstAmount, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@remarks", dbobject.Remarks, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_advance, dbobject.RecId.ToString(), "Updation of Installment");
                Message = "Advance Installment Updated Successfully";
                Result = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SalaryBLL", "updateAdvanceDeductionInstallment", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
    }
}