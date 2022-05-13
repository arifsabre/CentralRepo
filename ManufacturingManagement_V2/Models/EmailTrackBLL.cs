using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EmailTrackBLL : DbContext
    {
        //
        //internal DbSet<PromotionDetailMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static EmailTrackBLL Instance
        {
            get { return new EmailTrackBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, EmailTrackMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@EMDate", dbobject.EmDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@EmFrom", dbobject.EmFrom.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Railway", dbobject.Railway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmSubject", dbobject.EmSubject.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UserId", dbobject.UserId, DbType.Int32));
            //cmd.Parameters.Add(mc.getPObject("@EmStatus", dbobject.EmStatus, DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@RepDate", dbobject.RepDate.ToShortDateString(), DbType.DateTime));
            //cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
        }
        //
        private List<EmailTrackMdl> createObjectList(DataSet ds)
        {
            List<EmailTrackMdl> listObj = new List<EmailTrackMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmailTrackMdl objmdl = new EmailTrackMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.EmDate = Convert.ToDateTime(dr["EmDate"].ToString());
                objmdl.EmFrom = dr["EmFrom"].ToString();
                objmdl.Railway = dr["Railway"].ToString();
                objmdl.EmSubject = dr["EmSubject"].ToString();
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                objmdl.UserName = dr["UserName"].ToString();
                objmdl.EmStatus = dr["EmStatus"].ToString();
                objmdl.EmStatusName = dr["EmStatusName"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(EmailTrackMdl dbobject)
        {
            if (dbobject.UserId == 0)
            {
                Message = "User not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.EmDate) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(EmailTrackMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_emailtrack";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_emailtrack, "recid");
                mc.setEventLog(cmd, dbTables.tbl_emailtrack, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("EmailTrackBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(EmailTrackMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_emailtrack";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_emailtrack, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("EmailTrackBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateEmailTrackReply(int recid, string emstatus, string remarks)
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
                cmd.CommandText = "usp_update_emailtrack_reply";
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@emstatus", emstatus, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@remarks", remarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                string lginfo = "Updated for Completion";
                if (emstatus == "p")
                {
                    lginfo = "Updated for Pending";
                }
                mc.setEventLog(cmd, dbTables.tbl_emailtrack, recid.ToString(), lginfo);
                Result = true;
                Message = "Record " + lginfo;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("EmailTrackBLL", "updateEmailTrackReply", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_emailtrack";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_emailtrack, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("EmailTrackBLL", "deleteObject", ex.Message);
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
        internal EmailTrackMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            EmailTrackMdl dbobject = new EmailTrackMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_emailtrack";
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
        internal DataSet getObjectData(DateTime dtfrom, DateTime dtto, string emstatus)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_emailtrack";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@emstatus", emstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EmailTrackMdl> getObjectList(DateTime dtfrom, DateTime dtto, string emstatus)
        {
            DataSet ds = getObjectData(dtfrom,dtto,emstatus);
            return createObjectList(ds);
        }
        //
        internal DataSet getEmailTrackListData(DateTime dtfrom, DateTime dtto, string emstatus)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_emailtrack_list";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@emstatus", emstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EmailTrackMdl> getEmailTrackList(DateTime dtfrom, DateTime dtto, string emstatus)
        {
            DataSet ds = getEmailTrackListData(dtfrom,dtto,emstatus);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}