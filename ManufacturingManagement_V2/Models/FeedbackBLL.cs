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
    public class FeedbackBLL : DbContext
    {
        //
        //internal DbSet<FeedbackMdl> mdls { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, FeedbackMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@EntryDT", DateTime.Now.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@SendingUser", dbobject.SendingUser, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Suggestion", dbobject.Suggestion, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FBStatus", dbobject.FBStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ReplyMsg", dbobject.ReplyMsg.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ReplyUser", dbobject.ReplyUser, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ReplyDT", DateTime.Now.ToString(), DbType.DateTime));
        }
        //
        private List<FeedbackMdl> createObjectList(DataSet ds)
        {
            List<FeedbackMdl> objlst = new List<FeedbackMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FeedbackMdl objmdl = new FeedbackMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.EntryDT = Convert.ToDateTime(dr["EntryDT"].ToString());
                objmdl.EntryDTStr = dr["EntryDTStr"].ToString();//d
                objmdl.SendingUser = Convert.ToInt32(dr["SendingUser"].ToString());
                objmdl.Suggestion = dr["Suggestion"].ToString();
                objmdl.FBStatus = dr["FBStatus"].ToString();
                objmdl.ReplyMsg = dr["ReplyMsg"].ToString();
                objmdl.ReplyUser = Convert.ToInt32(dr["ReplyUser"].ToString());
                objmdl.SendingUserName = dr["SendingUserName"].ToString();//d
                objmdl.ReplyUserName = dr["ReplyUserName"].ToString();//d
                objmdl.ReplyDT = Convert.ToDateTime(dr["ReplyDT"].ToString());
                objmdl.ReplyDTStr = dr["ReplyDTStr"].ToString();//d
                objmdl.FBStatusName = dr["FBStatusName"].ToString();//d
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private bool checkSetValidModel(FeedbackMdl dbobject)
        {
            dbobject.SendingUser = Convert.ToInt32(objCookie.getUserId());
            dbobject.ReplyUser = Convert.ToInt32(objCookie.getUserId());
            return true;
        }
        /// <summary>
        /// To check that no user can modify another's feedback
        /// </summary>
        /// <param name="recid"></param>
        /// <param name="sendinguser"></param>
        /// <returns></returns>
        private bool isValidUserToUpdateFeedback(int recid, int sendinguser)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_user_to_update_feedback";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@sendinguser", sendinguser, DbType.Int32));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == false)
            {
                Message = "Invalid authentication to update!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertFeedback(string suggestion)
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
                cmd.CommandText = "usp_insert_tbl_feedback";
                cmd.Parameters.Add(mc.getPObject("@EntryDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@SendingUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@Suggestion", suggestion.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@FBStatus", "p", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ReplyMsg", "", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ReplyUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@ReplyDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_feedback, "recid");
                mc.setEventLog(cmd, dbTables.tbl_feedback, recid, "Feedback Inserted");
                Result = true;
                Message = "Feedback Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FeedbackBLL", "insertFeedback", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateFeedback(int recid, string suggestion)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (isValidUserToUpdateFeedback(recid, Convert.ToInt32(objCookie.getUserId())) == false) 
            {
                Message = "Record cannot be updated!";
                return; 
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
                cmd.CommandText = "usp_update_feedback";
                cmd.Parameters.Add(mc.getPObject("@EntryDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@Suggestion", suggestion.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_feedback, recid.ToString(), "Feedback Updated");
                Result = true;
                Message = "Feedback Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FeedbackBLL", "updateFeedback", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateFeedbackStatus(int recid, string replymsg)
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
                cmd.CommandText = "usp_update_feedback_status";
                cmd.Parameters.Add(mc.getPObject("@ReplyMsg", replymsg.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ReplyUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@ReplyDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_feedback, recid.ToString(), "Reply Updated");
                Result = true;
                Message = "Reply Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FeedbackBLL", "updateFeedbackStatus", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void resetFeedbackStatus(int recid)
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
                cmd.CommandText = "usp_reset_feedback_status";
                cmd.Parameters.Add(mc.getPObject("@ReplyUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@ReplyDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_feedback, recid.ToString(), "Reply Updated");
                Result = true;
                Message = "Feedback reset updated!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FeedbackBLL", "resetFeedbackStatus", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteFeedback(int recid)
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
                cmd.CommandText = "usp_delete_tbl_feedback";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_complaint, recid.ToString(), "Deleted");
                Result = true;
                Message = "Feedback Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FeedbackBLL", "deleteFeedback", ex.Message);
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
        internal FeedbackMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            FeedbackMdl dbobject = new FeedbackMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_feedback";
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
        private DataSet getFeedbackBySendingUserData(string fbstatus = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_feedback_by_sendinguser";
            cmd.Parameters.Add(mc.getPObject("@sendinguser", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@fbstatus", fbstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<FeedbackMdl> getFeedbackBySendingUserList(string fbstatus = "0")
        {
            DataSet ds = getFeedbackBySendingUserData(fbstatus);
            return createObjectList(ds);
        }
        //
        internal DataSet getObjectData(string fbstatus = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_feedback";
            cmd.Parameters.Add(mc.getPObject("@fbstatus", fbstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<FeedbackMdl> getObjectList(string fbstatus = "0")
        {
            DataSet ds = getObjectData(fbstatus);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}