using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QualificationBLL : DbContext
    {
        //
        internal DbSet<QualificationMdl> Qualifications { get; set; }
        //
        string tblObject = "qualification";
        string keyParameter = "@qualid";
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static QualificationBLL Instance
        {
            get { return new QualificationBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, QualificationMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@Qualification", dbobject.Qualification.Trim(), DbType.String));
        }
        //
        private List<QualificationMdl> createObjectList(DataSet ds)
        {
            List<QualificationMdl> qualifications = new List<QualificationMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QualificationMdl objmdl = new QualificationMdl();
                objmdl.QualId = Convert.ToInt32(dr["QualId"].ToString());
                objmdl.Qualification = dr["Qualification"].ToString();
                qualifications.Add(objmdl);
            }
            return qualifications;
        }
        //
        private bool isAlreadyFound(string qualification)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_" + tblObject;
            cmd.Parameters.Add(mc.getPObject("@qualification", qualification, DbType.String));
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
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(QualificationMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.Qualification) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_" + tblObject;
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                //string vendorid = mc.getRecentIdentityValue(cmd, dbTables.tbl_vendor, "vendorid");
                //mc.setEventLog(cmd, dbTables.tbl_vendor, vendorid, "", "");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog(tblObject + "_Context", "insert_" + tblObject, ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(QualificationMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_" + tblObject;
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject(keyParameter, dbobject.QualId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_vendor, dbobject.VendorId.ToString(), "", "");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_" + tblObject) == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog(tblObject + "_Context", "update_" + tblObject, ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int id)
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
                cmd.CommandText = "usp_delete_tbl_" + tblObject;
                cmd.Parameters.Add(mc.getPObject(keyParameter, id, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_vendor, id.ToString(), "", "");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog(tblObject + "_Context", "delete_" + tblObject, ex.Message);
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
        internal QualificationMdl searchObject(int id)
        {
            DataSet ds = new DataSet();
            QualificationMdl dbobject = new QualificationMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_" + tblObject;
            cmd.Parameters.Add(mc.getPObject(keyParameter, id, DbType.Int32));
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
        internal DataSet getObjectData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_" + tblObject;
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<QualificationMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}