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
    public class ExperienceDetailBLL : DbContext
    {
        //
        internal DbSet<ExperienceDetailMdl> experiencedetail { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ExperienceDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@FirmName", dbobject.FirmName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DateFrom", dbobject.DateFrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@DateTo", dbobject.DateTo.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Designation", dbobject.Designation, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@JobDesc", dbobject.JobDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
        }
        //
        private List<ExperienceDetailMdl> createObjectList(DataSet ds)
        {
            List<ExperienceDetailMdl> experiencedetail = new List<ExperienceDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ExperienceDetailMdl objmdl = new ExperienceDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.FirmName = dr["FirmName"].ToString();
                objmdl.DateFrom = Convert.ToDateTime(dr["DateFrom"].ToString());
                objmdl.DateTo = Convert.ToDateTime(dr["DateTo"].ToString());
                objmdl.Designation = dr["Designation"].ToString();
                objmdl.JobDesc = dr["JobDesc"].ToString();
                experiencedetail.Add(objmdl);
            }
            return experiencedetail;
        }
        //
        private bool checkSetValidModel(ExperienceDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.DateFrom) == false)
            {
                Message = "Invalid from date!";
                return false;
            }
            if (mc.isValidDate(dbobject.DateTo) == false)
            {
                Message = "Invalid to date!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(ExperienceDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_experiencedetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_experiencedetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("ExperienceDetailBLL", "insertObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ExperienceDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_experiencedetail";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_experiencedetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("ExperienceDetailBLL", "updateObject", ex.Message);
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
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_experiencedetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ExperienceDetailBLL", "deleteObject", ex.Message);
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
        internal ExperienceDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            ExperienceDetailMdl dbobject = new ExperienceDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_experiencedetail";
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
        internal DataSet getObjectData(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_experiencedetail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ExperienceDetailMdl> getObjectList(int newempid)
        {
            DataSet ds = getObjectData(newempid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}