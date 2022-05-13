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
    public class NotificationBLL : DbContext
    {
        //
        internal DbSet<NotificationMdl> mdls { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, NotificationMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@NoticeDT", DateTime.Now.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@SendingUser", dbobject.SendingUser, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NoticeMsg", dbobject.NoticeMsg, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MsgType", dbobject.MsgType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ToUser", dbobject.ToUser, DbType.Int32));
        }
        //
        private List<NotificationMdl> createObjectList(DataSet ds)
        {
            List<NotificationMdl> objlst = new List<NotificationMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                NotificationMdl objmdl = new NotificationMdl();
                objmdl.NoticeId = Convert.ToInt32(dr["NoticeId"].ToString());
                objmdl.NoticeNo = Convert.ToInt32(dr["NoticeNo"].ToString());
                objmdl.NoticeDT = Convert.ToDateTime(dr["NoticeDT"].ToString());
                objmdl.SendingUser = Convert.ToInt32(dr["SendingUser"].ToString());
                objmdl.SendingUserName = dr["SendingUserName"].ToString();
                objmdl.NoticeMsg = dr["NoticeMsg"].ToString();
                objmdl.MsgType = dr["MsgType"].ToString();
                objmdl.ToUser = Convert.ToInt32(dr["ToUser"].ToString());
                objmdl.MsgTypeName = dr["MsgTypeName"].ToString();//d
                objmdl.ToUserName = dr["ToUserName"].ToString();//d
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private List<NotificationUserMdl> getNotificationUsersList(DataSet ds)
        {
            List<NotificationUserMdl> objlst = new List<NotificationUserMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                NotificationUserMdl objmdl = new NotificationUserMdl();
                objmdl.RecId = dr["recid"].ToString();
                objmdl.setUser = Convert.ToBoolean(dr["setUser"].ToString());
                objmdl.ReceivingUser = dr["ReceivingUser"].ToString();
                objmdl.ReceivingUsername = dr["ReceivingUsername"].ToString();
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private List<NotificationUserMdl> createNotificationReplyList(DataSet ds)
        {
            List<NotificationUserMdl> objlst = new List<NotificationUserMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                NotificationUserMdl objmdl = new NotificationUserMdl();
                objmdl.RecId = dr["RecId"].ToString();
                objmdl.NoticeNo = Convert.ToInt32(dr["NoticeNo"].ToString());
                objmdl.setUser = Convert.ToBoolean(dr["setUser"].ToString());
                objmdl.ReceivingUser = dr["ReceivingUser"].ToString();
                objmdl.ReceivingUsername = dr["ReceivingUsername"].ToString();//d
                objmdl.SendingUserName = dr["SendingUserName"].ToString();//d
                objmdl.IsAttended = dr["isattended"].ToString();//d
                objmdl.NoticeDT = Convert.ToDateTime(dr["NoticeDt"].ToString());//d
                objmdl.NoticeMsg = dr["NoticeMsg"].ToString();//d
                objmdl.AttendedAT = dr["AttendedAT"].ToString();//d
                objmdl.ReplyMsg = dr["ReplyMsg"].ToString();//d
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private bool checkSetValidModel(NotificationMdl dbobject)
        {
            dbobject.SendingUser = Convert.ToInt32(objCookie.getUserId());
            if (mc.isValidDate(dbobject.NoticeDT) == false)
            {
                Message = "Invalid notice date!";
                return false;
            }
            if (dbobject.MsgType == "m" || dbobject.MsgType == "a")
            {
                dbobject.ToUser = dbobject.SendingUser;
            }
            if (dbobject.MsgType == "s" && dbobject.ToUser == 0)
            {
                Message = "Receipient not selected!";
                return false;
            }
            return true;
        }
        /// <summary>
        /// To check that no user can modify another's 
        /// notification's message or users list
        /// </summary>
        /// <param name="noticeid"></param>
        /// <param name="sendinguser"></param>
        /// <returns></returns>
        private bool isValidUserToUpdateNotification(int noticeid,int sendinguser)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_user_to_update_notification";
            cmd.Parameters.Add(mc.getPObject("@noticeid", noticeid, DbType.Int32));
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
        internal void insertNotification(NotificationMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_notification";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.NoticeId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_notification, "noticeid"));
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_notification_number";
                cmd.Parameters.Add(mc.getPObject("@noticeid", dbobject.NoticeId, DbType.Int32));
                dbobject.NoticeNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_set_notificationuser";
                cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_notification, noticeid, "Inserted");
                if (dbobject.MsgType.ToString().ToLower() != "m")
                {
                    //sendNotificationOnUtilities(cmd, dbobject);
                }
                Result = true;
                trn.Commit();
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("NotificationBLL", "insertNotification", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateNotification(NotificationMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isValidUserToUpdateNotification(dbobject.NoticeId,dbobject.SendingUser) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_notification";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_set_notificationuser";
                cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_notification, dbobject.NoticeId.ToString(), "Updated");
                if (dbobject.MsgType.ToString().ToLower() != "m")
                {
                    //sendNotificationOnUtilities(cmd, dbobject);
                }
                Result = true;
                trn.Commit();
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("NotificationBLL", "updateNotification", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateNotificationUsers(NotificationMdl dbobject)
        {
            Result = false;
            if (isValidUserToUpdateNotification(dbobject.NoticeId, Convert.ToInt32(objCookie.getUserId())) == false) { return; };
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
                cmd.CommandText = "usp_set_msgtype_to_multiple";
                cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.NotificationUsers.Count; i++)
                {
                    if (dbobject.NotificationUsers[i].setUser == true)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_notificationuser";
                        cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@receivinguser", dbobject.NotificationUsers[i].ReceivingUser, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@isattended", "0", DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@attendedat", DateTime.Now.ToString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@replymsg", "", DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.NotificationUsers[i].RecId, DbType.Int32));
                        cmd.ExecuteNonQuery();
                    }
                    else if (dbobject.NotificationUsers[i].setUser == false)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_reset_notificationuser";
                        cmd.Parameters.Add(mc.getPObject("@NoticeId", dbobject.NoticeId, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@receivinguser", dbobject.NotificationUsers[i].ReceivingUser, DbType.Int32));
                        cmd.ExecuteNonQuery();
                    }
                }
                //mc.setEventLog(cmd, dbTables.tbl_notification, dbobject.NoticeId.ToString(), "Updated");
                //sendNotificationOnUtilities(cmd, dbobject);
                Result = true;
                trn.Commit();
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("NotificationBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void sendNotificationOnUtilities(SqlCommand cmd, NotificationMdl dbobject)
        {
            //under txn
            DataSet ds = new DataSet();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_notificationuser_detail";
            cmd.Parameters.Add(mc.getPObject("@noticeid", dbobject.NoticeId, DbType.Int32));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            if (ds.Tables.Count == 0) { return; };
            ArrayList arl = new ArrayList();
            string msg = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["MobileNo"].ToString().Length > 0)
                {
                    msg = "PRAG ERP Notification Number: " + dbobject.NoticeNo.ToString();
                    msg += "\r\nDate: " + mc.getStringByDate(dbobject.NoticeDT) + " " + DateTime.Now.ToShortTimeString();
                    msg += "\r\nFrom : " + ds.Tables[0].Rows[0]["sendingUser"].ToString(); 
                    msg += "\r\n\r\n" + "Dear " + ds.Tables[0].Rows[i]["UserName"].ToString() + ",";
                    msg += "\r\n\r\n" + dbobject.NoticeMsg + "\r\n\r\nRegards-\r\nPRAG ERP";
                    mc.performAPICall_SendSMS(ds.Tables[0].Rows[i]["MobileNo"].ToString(), msg);
                }
                if (ds.Tables[0].Rows[i]["EMail"].ToString().Length > 0)
                {
                    msg = "PRAG ERP Notification Number: " + dbobject.NoticeNo.ToString();
                    msg += "<br/>Date: " + mc.getStringByDate(dbobject.NoticeDT) + " " + DateTime.Now.ToShortTimeString();
                    msg += "<br/>From : " + ds.Tables[0].Rows[0]["sendingUser"].ToString();
                    msg += "<br/><br/>" + "Dear " + ds.Tables[0].Rows[i]["UserName"].ToString() + ",";
                    msg += "<br/><br/>" + dbobject.NoticeMsg + "<br/><br/>Regards-<br/>PRAG ERP";
                    arl.Add(ds.Tables[0].Rows[i]["EMail"].ToString());
                    mc.SendEmailByERP(arl, "PRAG ERP Notification", msg);
                    arl.Clear();
                }
            }
        }
        internal void deleteNotification(int noticeid)
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
                cmd.CommandText = "usp_delete_tbl_notification";
                cmd.Parameters.Add(mc.getPObject("@noticeid", noticeid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_notification, noticeid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("NotificationBLL", "deleteNotification", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateNotificationAttended(int recid,string replymsg)
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
                cmd.CommandText = "usp_update_notification_attended";
                cmd.Parameters.Add(mc.getPObject("@ReplyMsg", replymsg, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_notification, noticeid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("NotificationBLL", "updateNotificationAttended", ex.Message);
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
        internal NotificationMdl searchObject(int noticeid)
        {
            DataSet ds = new DataSet();
            NotificationMdl dbobject = new NotificationMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_notification";
            cmd.Parameters.Add(mc.getPObject("@noticeid", noticeid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_notificationuser";
            cmd.Parameters.Add(mc.getPObject("@noticeid", noticeid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            dbobject.NotificationUsers = getNotificationUsersList(ds);
            return dbobject;
        }
        //
        internal DataSet getNotificationAlertData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_notification_alert";
            cmd.Parameters.Add(mc.getPObject("@receivinguser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getRecentNotificationData(bool all = false)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (all == false)
            {
                cmd.CommandText = "usp_get_recent_notification_list";
            }
            else
            {
                cmd.CommandText = "usp_get_all_notification_list";
            }
            cmd.Parameters.Add(mc.getPObject("@receivinguser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<NotificationUserMdl> getRecentNotificationList(bool all=false)
        {
            DataSet ds = getRecentNotificationData(all);
            return createNotificationReplyList(ds);
        }
        //
        private DataSet getNotificationReplyData(int noticeid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_notification_reply_list";
            cmd.Parameters.Add(mc.getPObject("@noticeid", noticeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@sendinguser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<NotificationUserMdl> getNotificationReplyList(int noticeid)
        {
            DataSet ds = getNotificationReplyData(noticeid);
            return createNotificationReplyList(ds);
        }
        //
        internal DataSet getObjectData(string dtfrom, string dtto, string userid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_notification";
            //cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@sendinguser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<NotificationMdl> getObjectList(string dtfrom, string dtto, string empid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, empid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}