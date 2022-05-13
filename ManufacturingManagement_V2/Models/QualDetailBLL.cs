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
    public class QualDetailBLL : DbContext
    {
        //
        internal DbSet<QualDetailMdl> QualDetails { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, QualDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@QualId", dbobject.QualId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PassingYear", dbobject.PassingYear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Institute", dbobject.Institute, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UnivBoard", dbobject.UnivBoard, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Division", dbobject.Division, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
        }
        //
        private List<QualDetailMdl> createObjectList(DataSet ds)
        {
            List<QualDetailMdl> qualdetail = new List<QualDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QualDetailMdl objmdl = new QualDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.QualId = Convert.ToInt32(dr["QualId"].ToString());
                objmdl.Qualification = dr["Qualification"].ToString();//d
                objmdl.PassingYear = Convert.ToInt32(dr["PassingYear"].ToString());
                objmdl.Institute = dr["Institute"].ToString();
                objmdl.UnivBoard = dr["UnivBoard"].ToString();
                objmdl.Division = Convert.ToDouble(dr["Division"].ToString());
                qualdetail.Add(objmdl);
            }
            return qualdetail;
        }
        //
        private bool checkSetValidModel(QualDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            //if (mc.isValidDate(dbobject.AdvDate) == false)
            //{
            //    Message = "Invalid advance date!";
            //    return false;
            //}
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(QualDetailMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_qualdetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_qualdetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("QualDetailBLL", "insertObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(QualDetailMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_qualdetail";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_qualdetail_tbl_employee") == true)
                {
                    Message = "Invalid Employee Code!";
                }
                else
                {
                    Message = mc.setErrorLog("QualDetailBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_qualdetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QualDetailBLL", "deleteObject", ex.Message);
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
        internal QualDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            QualDetailMdl dbobject = new QualDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_qualdetail";
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
            cmd.CommandText = "usp_get_tbl_qualdetail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<QualDetailMdl> getObjectList(int newempid)
        {
            DataSet ds = getObjectData(newempid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}