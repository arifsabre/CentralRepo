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
    public class CircularBLL : DbContext
    {
        //
        //internal DbSet<CircularMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, CircularMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CircularDate", dbobject.CircularDate.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@DepCode", dbobject.DepCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UserId", dbobject.UserId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SubjectOn", dbobject.SubjectOn.Trim(), DbType.String));
        }
        //
        private List<CircularMdl> createObjectList(DataSet ds)
        {
            //[retained as sample...(AdminControl.sql)]
            //clsTableToModel.ConvertToList<Object>(DataTable) 
            //has been used instead of this method in this bll
            List<CircularMdl> objlist = new List<CircularMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CircularMdl objmdl = new CircularMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.CircularDate = Convert.ToDateTime(dr["CircularDate"].ToString());
                objmdl.CircularNo = Convert.ToInt32(dr["CircularNo"].ToString());
                objmdl.DepCode = dr["DepCode"].ToString();
                objmdl.Department = dr["Department"].ToString();//d
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                objmdl.UserName = dr["UserName"].ToString();
                objmdl.SubjectOn = dr["SubjectOn"].ToString();
                objmdl.Doc = Convert.ToBoolean(dr["Doc"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(CircularMdl dbobject)
        {
            if (mc.isValidDate(dbobject.CircularDate) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            if (dbobject.UserId == 0)
            {
                Message = "Circular From not selected!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(CircularMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_circular";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_circular, "recid");
                mc.setEventLog(cmd, dbTables.tbl_circular, recid, "Inserted");
                Result = true;
                Message = "Circular Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CircularBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateCircularDocument(int recid, bool doc)
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
                cmd.CommandText = "usp_update_circular_document";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@doc", doc, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_circular, recid.ToString(), "Document Uploaded");
                Result = true;
                Message = "Document Uploaded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CircularBLL", "updateCircularDocument", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(CircularMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_circular";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_circular, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Circular Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CircularBLL", "updateObject", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_circular";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_circular, recid.ToString(), "Deleted");
                Result = true;
                Message = "Circular Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CircularBLL", "deleteObject", ex.Message);
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
        internal CircularMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            CircularMdl dbobject = new CircularMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_circular";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = clsTableToModel.ConvertToList<CircularMdl>(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(DateTime dtfrom, DateTime dtto, string depcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_circular_list";
            cmd.Parameters.Add(mc.getPObject("@depcode", depcode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CircularMdl> getObjectList(DateTime dtfrom, DateTime dtto, string depcode)
        {
            DataSet ds = getObjectData(dtfrom, dtto, depcode);
            return clsTableToModel.ConvertToList<CircularMdl>(ds);
        }
        //
        #endregion
        //
    }
}