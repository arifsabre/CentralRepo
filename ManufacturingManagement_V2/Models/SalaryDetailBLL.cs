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
    public class SalaryDetailBLL : DbContext
    {
        //
        internal DbSet<SalaryDetailMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, SalaryDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@IncDate", dbobject.IncDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@BasicRate", dbobject.BasicRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DA", dbobject.DA, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ConvAllowance", dbobject.ConvAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@HRA", dbobject.HRA, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@MedicalAllowance", dbobject.MedicalAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompAllowance", dbobject.CompAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DWAllowance", dbobject.DWAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SpecialPay", dbobject.SpecialPay, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Others", dbobject.Others, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IncAmount", dbobject.IncAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EsiApplicable", dbobject.EsiApplicable, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFDeductable", dbobject.PFDeductable, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdsDeduction", dbobject.TdsDeduction, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
        }
        //
        private List<SalaryDetailMdl> createObjectList(DataSet ds)
        {
            List<SalaryDetailMdl> listObj = new List<SalaryDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SalaryDetailMdl objmdl = new SalaryDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.EmpName = dr["EmpName"].ToString();//d
                objmdl.IncDate = Convert.ToDateTime(dr["IncDate"].ToString());
                objmdl.BasicRate = Convert.ToDouble(dr["BasicRate"].ToString());
                objmdl.DA = Convert.ToDouble(dr["DA"].ToString());
                objmdl.ConvAllowance = Convert.ToDouble(dr["ConvAllowance"].ToString());
                objmdl.HRA = Convert.ToDouble(dr["HRA"].ToString());
                objmdl.MedicalAllowance = Convert.ToDouble(dr["MedicalAllowance"].ToString());
                objmdl.CompAllowance = Convert.ToDouble(dr["CompAllowance"].ToString());
                objmdl.DWAllowance = Convert.ToDouble(dr["DWAllowance"].ToString());
                objmdl.SpecialPay = Convert.ToDouble(dr["SpecialPay"].ToString());
                objmdl.Others = Convert.ToDouble(dr["Others"].ToString());
                objmdl.GrossSalary = Convert.ToDouble(dr["GrossSalary"].ToString());
                objmdl.OldGross = Convert.ToDouble(dr["OldGross"].ToString());
                objmdl.IncAmount = Convert.ToDouble(dr["IncAmount"].ToString());
                objmdl.EsiApplicable = Convert.ToBoolean(dr["EsiApplicable"].ToString());
                objmdl.PFDeductable = Convert.ToBoolean(dr["PFDeductable"].ToString());
                objmdl.TdsDeduction = Convert.ToDouble(dr["TdsDeduction"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool isAlreadyFound(DateTime incdate, int newempid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_salarydetail";
            cmd.Parameters.Add(mc.getPObject("@incdate", incdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
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
        private bool checkSetValidModel(SalaryDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.IncDate) == false)
            {
                Message = "Invalid w.e.f. date!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertSalaryDetail(SalaryDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.IncDate,dbobject.NewEmpId) == true) { return; };
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
                cmd.Parameters.Clear();
                cmd.Transaction = trn;
                cmd.CommandText = "usp_insert_tbl_salarydetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd,dbTables.tbl_salarydetail,"recid");
                mc.setEventLog(cmd, dbTables.tbl_salarydetail, recid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryDetailBLL", "insertSalaryDetail", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateSalaryDetail(SalaryDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            //if (mc.isValidDateToSetRecord(dbobject.IncDate) == false)
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
                cmd.Parameters.Clear();
                cmd.Transaction = trn;
                cmd.CommandText = "usp_update_salarydetail";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_salarydetail, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SalaryDetailBLL", "updateSalaryDetail", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        /// <summary>
        /// Only by admin permission
        /// </summary>
        /// <param name="recid"></param>
        internal void deleteObject(int recid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_salarydetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_salarydetail, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SalaryDetailBLL", "deleteObject", ex.Message);
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
        internal SalaryDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            SalaryDetailMdl dbobject = new SalaryDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_salarydetail";
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
        internal DataSet getObjectData(string dtfrom, string dtto, int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_salarydetail";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SalaryDetailMdl> getObjectList(string dtfrom, string dtto, int newempid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, newempid);
            return createObjectList(ds);
        }
        //
        internal int getRecentSalaryDetailRecId(int newempid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recent_salary_detail_recid";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            return Convert.ToInt32(mc.getFromDatabase(cmd));
        }
        //
        #endregion
        //
    }
}