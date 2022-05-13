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
    public class EmployeeOpeningBLL : DbContext
    {
        //
        internal DbSet<EmployeeOpeningMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, EmployeeOpeningMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VType", dbobject.VType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VDate", dbobject.VDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@DaysWorked", dbobject.DaysWorked, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Earning", dbobject.Earning, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RemainingPL", dbobject.RemainingPL, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EarnedPL", dbobject.EarnedPL, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PLEncashment", dbobject.PLEncashment, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            //for voucher
            cmd.Parameters.Add(mc.getPObject("@EncAmount", dbobject.EncAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
        }
        //
        private List<EmployeeOpeningMdl> createObjectList(DataSet ds)
        {
            List<EmployeeOpeningMdl> listObj = new List<EmployeeOpeningMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeOpeningMdl objmdl = new EmployeeOpeningMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.VType = dr["VType"].ToString();
                objmdl.VTypeName = dr["VTypeName"].ToString();
                objmdl.VDate = Convert.ToDateTime(dr["VDate"].ToString());
                objmdl.DaysWorked = Convert.ToDouble(dr["DaysWorked"].ToString());
                objmdl.Earning = Convert.ToDouble(dr["Earning"].ToString());
                objmdl.RemainingPL = Convert.ToDouble(dr["RemainingPL"].ToString());
                objmdl.EarnedPL = Convert.ToDouble(dr["EarnedPL"].ToString());
                objmdl.PLEncashment = Convert.ToDouble(dr["PLEncashment"].ToString());
                objmdl.EncAmount = Convert.ToDouble(dr["EncAmount"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(EmployeeOpeningMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.VDate) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            if (dbobject.VType.ToLower() != "en")
            {
                if (dbobject.PLEncashment + dbobject.EncAmount > 0)
                {
                    Message = "Invalid voucher type for encashment entry!";
                    return false;
                }
            }
            if (dbobject.VType.ToLower() == "en")
            {
                if (dbobject.PLEncashment == 0)
                {
                    Message = "Number of encashment leaves not entered!";
                    return false;
                }
                if (dbobject.EncAmount == 0)
                {
                    Message = "Encashment amount not entered!";
                    return false;
                }
                if (dbobject.DaysWorked + dbobject.Earning + dbobject.RemainingPL + dbobject.EarnedPL > 0)
                {
                    Message = "Only encashment amount can be entered by 'Encashment Voucher'!";
                    return false;
                }
                if (mc.isValidDateForFinYear(objCookie.getFinYear(), dbobject.VDate) == false)
                {
                    Message = "Invalid encashment date for financial year!";
                    return false;
                }
            }
            EmployeeBLL empBLL = new EmployeeBLL();
            if (empBLL.getEmployeeGradeNewEmpId(dbobject.NewEmpId).ToLower() == "d" && mc.getPermission(Models.Entry.DirectorsInfo, permissionType.Add) == false)
            {
                Message = "Unauthorised access!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(EmployeeOpeningMdl dbobject)
        {
            Result = false;
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
                cmd.CommandText = "usp_insert_tbl_employeeopening";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_employeeopening, "recid");
                mc.setEventLog(cmd, dbTables.tbl_employeeopening, recid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("EmployeeOpeningBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(EmployeeOpeningMdl dbobject)
        {
            Result = false;
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
                cmd.CommandText = "usp_update_tbl_employeeopening";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employeeopening, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("EmployeeOpeningBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_employeeopening";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employeeopening, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("EmployeeOpeningBLL", "deleteObject", ex.Message);
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
        internal EmployeeOpeningMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            EmployeeOpeningMdl dbobject = new EmployeeOpeningMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_employeeopening";
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
            cmd.CommandText = "usp_get_tbl_employeeopening";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EmployeeOpeningMdl> getObjectList(string dtfrom, string dtto, int newempid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, newempid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}