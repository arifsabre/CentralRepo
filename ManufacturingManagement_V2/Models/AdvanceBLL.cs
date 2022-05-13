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
    public class AdvanceBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, AdvanceMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@AdvDate", dbobject.AdvDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@AdvAmount", dbobject.AdvAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RecAmount", dbobject.RecAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@InstAmount", dbobject.InstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
            //for voucher
            cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
        }
        //
        internal void setAdditionalColumns(DataSet ds)
        {
            if (ds.Tables.Count == 0) { return; };
            //add columns
            ds.Tables[0].Columns.Add("GradeName");
            ds.Tables[0].Columns.Add("CategoryName");
            ds.Tables[0].Columns.Add("Designation");
            //set column values in ds
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["GradeName"] = mc.getNameByKey(mc.getGrades(), "grade", ds.Tables[0].Rows[i]["Grade"].ToString(), "gradename");
                ds.Tables[0].Rows[i]["CategoryName"] = mc.getNameByKey(mc.getEmpCategory(), "categoryid", ds.Tables[0].Rows[i]["categoryId"].ToString(), "categoryname");
                ds.Tables[0].Rows[i]["Designation"] = mc.getNameByKey(mc.getDesignation(), "desigid", ds.Tables[0].Rows[i]["DesigId"].ToString(), "designation");
            }
        }
        //
        private List<AdvanceMdl> createObjectList(DataSet ds)
        {
            setAdditionalColumns(ds);
            List<AdvanceMdl> advances = new List<AdvanceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AdvanceMdl objmdl = new AdvanceMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.AdvDate = Convert.ToDateTime(dr["AdvDate"].ToString());
                objmdl.AdvAmount = Convert.ToDouble(dr["AdvAmount"].ToString());
                objmdl.RecAmount = Convert.ToDouble(dr["RecAmount"].ToString());
                objmdl.InstAmount = Convert.ToDouble(dr["InstAmount"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                //additional columns
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.Grade = dr["Grade"].ToString();
                objmdl.GradeName = dr["GradeName"].ToString();
                objmdl.JoiningUnit = Convert.ToInt32(dr["JoiningUnit"].ToString());
                objmdl.JoiningUnitName = dr["JoiningUnitName"].ToString();
                objmdl.WorkingUnit = Convert.ToInt32(dr["WorkingUnit"].ToString());
                objmdl.WorkingUnitName = dr["WorkingUnitName"].ToString();
                objmdl.CategoryId = dr["CategoryId"].ToString();
                objmdl.CategoryName = dr["CategoryName"].ToString();
                advances.Add(objmdl);
            }
            return advances;
        }
        //
        private bool checkSetValidModel(AdvanceMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.AdvDate) == false)
            {
                Message = "Invalid advance date!";
                return false;
            }
            if (mc.isValidDateToSetRecord(dbobject.AdvDate) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return false;
            }
            if (mc.isValidDateForFinYear(objCookie.getFinYear(), dbobject.AdvDate) == false)
            {
                Message = "Invalid advance date for financial year!";
                return false;
            }
            if (dbobject.AdvAmount == 0 && dbobject.RecAmount == 0)
            {
                Message = "Either advance amount or received amount must be entered!";
                return false;
            }
            if (dbobject.AdvAmount != 0 && dbobject.RecAmount != 0)
            {
                Message = "Either advance amount or received amount can be saved/updated!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(AdvanceMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_advance";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_advance, "recid");
                mc.setEventLog(cmd, dbTables.tbl_advance, recid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AdvanceBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(AdvanceMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_advance";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_advance, dbobject.RecId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("fk_tbl_advance_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("AdvanceBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_advance";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_advance, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AdvanceBLL", "deleteObject", ex.Message);
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
        internal AdvanceMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            AdvanceMdl dbobject = new AdvanceMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_advance";
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
        internal DataSet getObjectData(string dtfrom, string dtto, string empid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_advance";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@empid", empid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AdvanceMdl> getObjectList(string dtfrom, string dtto, string empid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, empid);
            return createObjectList(ds);
        }
        internal string getAdvanceDeductionInfo(int newempid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_advance_deduction_information";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        #endregion
        //
    }
}