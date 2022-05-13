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
    public class FamilyDetailBLL : DbContext
    {
        //
        internal DbSet<FamilyDetailMdl> familydetail { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, FamilyDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@MemberName", dbobject.MemberName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BirthDate", dbobject.BirthDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@MaritalStatus", dbobject.MaritalStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Relation", dbobject.Relation, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
        }
        //
        private List<FamilyDetailMdl> createObjectList(DataSet ds)
        {
            List<FamilyDetailMdl> familydetail = new List<FamilyDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FamilyDetailMdl objmdl = new FamilyDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.MemberName = dr["MemberName"].ToString();
                objmdl.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
                objmdl.Age = Convert.ToInt32(dr["Age"].ToString());//d
                objmdl.MaritalStatus = dr["MaritalStatus"].ToString();
                objmdl.Relation = dr["Relation"].ToString();
                familydetail.Add(objmdl);
            }
            return familydetail;
        }
        //
        private bool checkSetValidModel(FamilyDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.BirthDate) == false)
            {
                Message = "Invalid advance date!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(FamilyDetailMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_familydetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_familydetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("FamilyDetailBLL", "insertObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(FamilyDetailMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_familydetail";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_familydetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("FamilyDetailBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_familydetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FamilyDetailBLL", "deleteObject", ex.Message);
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
        internal FamilyDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            FamilyDetailMdl dbobject = new FamilyDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_familydetail";
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
            cmd.CommandText = "usp_get_tbl_familydetail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<FamilyDetailMdl> getObjectList(int newempid)
        {
            DataSet ds = getObjectData(newempid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}