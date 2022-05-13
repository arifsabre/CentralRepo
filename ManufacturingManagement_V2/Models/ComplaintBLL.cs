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
    public class ComplaintBLL : DbContext
    {
        //
        internal DbSet<ComplaintMdl> mdls { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ComplaintMdl dbobject)
        {
            //not in use
            //cmd.Parameters.Add(mc.getPObject("@CompDT", DateTime.Now.ToString(), DbType.DateTime));
            //cmd.Parameters.Add(mc.getPObject("@CompUser", objCookie.getUserId(), DbType.Int32));
            //cmd.Parameters.Add(mc.getPObject("@CompMsg", dbobject.CompMsg.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@CompType", dbobject.CompType, DbType.Int32));
        }
        //
        private List<ComplaintMdl> createObjectList(DataSet ds)
        {
            List<ComplaintMdl> objlst = new List<ComplaintMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ComplaintMdl objmdl = new ComplaintMdl();
                objmdl.CompId = Convert.ToInt32(dr["CompId"].ToString());
                objmdl.CompDT = Convert.ToDateTime(dr["CompDT"].ToString());
                if (ds.Tables[0].Columns.Contains("CompUser"))
                {
                    objmdl.CompUser = Convert.ToInt32(dr["CompUser"].ToString());
                }
                if (ds.Tables[0].Columns.Contains("CompUserName"))
                {
                    objmdl.CompUserName = dr["CompUserName"].ToString();//d
                }
                objmdl.CompMsg = dr["CompMsg"].ToString();
                if (ds.Tables[0].Columns.Contains("CompType"))
                {
                    objmdl.CompType = Convert.ToInt32(dr["CompType"].ToString());
                }
                objmdl.CompTypeName = dr["CompTypeName"].ToString();//d
                if (ds.Tables[0].Columns.Contains("RecId"))
                {
                    objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                }
                if (ds.Tables[0].Columns.Contains("RecId"))
                {
                    objmdl.ReplyDT = Convert.ToDateTime(dr["ReplyDT"].ToString());
                    objmdl.ReplyDTStr = dr["ReplyDTStr"].ToString();//d
                    objmdl.ReplyUser = Convert.ToInt32(dr["ReplyUser"].ToString());
                    objmdl.ReplyUserName = dr["ReplyUserName"].ToString();//d
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (ds.Tables[0].Columns.Contains("ForwardedTo"))
                {
                    objmdl.ForwardedTo = Convert.ToInt32(dr["ForwardedTo"].ToString());
                    objmdl.ForwardedUserName = dr["ForwardedUserName"].ToString();//d
                }
                if (ds.Tables[0].Columns.Contains("CompStatus"))
                {
                    objmdl.CompStatus = dr["CompStatus"].ToString();
                }
                if (ds.Tables[0].Columns.Contains("CompStatusName"))
                {
                    objmdl.CompStatusName = dr["CompStatusName"].ToString();//d
                }
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private bool checkSetValidModel(ComplaintMdl dbobject)
        {
            //not in use
            //dbobject.SendingUser = Convert.ToInt32(objCookie.getUserId());
            //dbobject.ReplyUser = Convert.ToInt32(objCookie.getUserId());
            //dbobject.ReplyUser = Convert.ToInt32(objCookie.getUserId());
            return true;
        }
        //
        private bool isValidUserToUpdateComplaint(int compid, int compuser)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_user_to_update_complaint";
            cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compuser", compuser, DbType.Int32));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == false)
            {
                Message = "Invalid authentication to update!";
                return false;
            }
            return true;
        }
        //
        private string getEmailMessage(int compid)
        {
            ComplaintMdl objmdl = new ComplaintMdl();
            objmdl = searchObject(compid);

            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";

            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>Complaint Id</b></td>";
            reportmatter += "<td valign='top'>" + objmdl.CompId.ToString() + "</td>";
            reportmatter += "</tr>";

            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>Complaint By</b></td>";
            reportmatter += "<td valign='top'>" + objmdl.CompUserName + "</td>";
            reportmatter += "</tr>";

            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>Date</b></td>";
            reportmatter += "<td valign='top'>" + mc.getStringByDate(objmdl.CompDT)+" " + objmdl.CompDT.ToShortTimeString() + "</td>";
            reportmatter += "</tr>";

            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>Complaint Type</b></td>";
            reportmatter += "<td valign='top'>" + objmdl.CompTypeName + "</td>";
            reportmatter += "</tr>";

            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>Complaint</b></td>";
            reportmatter += "<td valign='top'>" + objmdl.CompMsg + "</td>";
            reportmatter += "</tr>";

            reportmatter += "</table>";

            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Complaint</b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the detail of complaint received:<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "<br/>Regards:<br/>PRAG ERP";
            return emailmsg;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertComplaint(int comptype, string compmsg)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (compmsg.Length == 0)
            {
                Message = "Complaint not entered!";
                return;
            }
            //for sms
            UserMdl umdl=new UserMdl();
            UserBLL ubll=new UserBLL();
            umdl = ubll.searchObject(Convert.ToInt32(objCookie.getUserId()));
            //
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_complaint";
                cmd.Parameters.Add(mc.getPObject("@CompDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@CompUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompMsg", compmsg.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompType", comptype, DbType.Int32));
                cmd.ExecuteNonQuery();
                string compid = mc.getRecentIdentityValue(cmd, dbTables.tbl_complaint, "compid");
                mc.setEventLog(cmd, dbTables.tbl_complaint, compid, "Complaint Inserted");
                Result = true;
                //
                mc.SendEmailByERP(getComplaintEmailAddress(), "ERP Complaint", getEmailMessage(Convert.ToInt32(compid)));
                string noticeInfo = "Date: " + mc.getStringByDate(DateTime.Now) + " " + DateTime.Now.ToShortTimeString();
                string noticeMsg = noticeInfo + "\r\n\r\nDear " + umdl.Title + " " + umdl.FullName + ",\r\nYour complaint with ID '" + compid.ToString() + "' has been registered. We will resolve the same as soon as possible.\r\n\r\nRegards\r\nPRAG ERP PRAG INDUSTRIES (INDIA) PVT LIMITED";
                mc.performAPICall_SendSMS(umdl.MobileNo, noticeMsg);
                //
                Message = "Complaint Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ComplaintBLL", "insertComplaint", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateComplaint(int compid, int comptype, string compmsg)
        {
            Result = false;
            if (compmsg.Length == 0)
            {
                Message = "Complaint not entered!";
                return;
            }
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (isValidUserToUpdateComplaint(compid, Convert.ToInt32(objCookie.getUserId())) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_complaint";
                cmd.Parameters.Add(mc.getPObject("@CompDT", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@CompUser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompMsg", compmsg.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompType", comptype, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompId", compid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_complaint, compid.ToString(), "Complaint Updated");
                Result = true;
                Message = "Complaint Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ComplaintBLL", "updateComplaint", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void replyComplaint(int compid, string remarks)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (remarks.Length == 0)
            {
                Message = "Reply not entered!";
                return;
            }
            //for sms
            UserMdl umdl = new UserMdl();
            UserBLL ubll = new UserBLL();
            umdl = ubll.searchObject(Convert.ToInt32(objCookie.getUserId()));
            //
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_complaintdetail_for_reply";
                cmd.Parameters.Add(mc.getPObject("@replyuser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@Remarks", remarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_complaint, compid.ToString(), "Complaint Replied");
                //
                string noticeInfo = "Date: " + mc.getStringByDate(DateTime.Now) + " " + DateTime.Now.ToShortTimeString();
                mc.SendEmailByERP(getComplaintEmailAddress(), "ERP Complaint Closed", noticeInfo + "<br/><br/>Dear Sir,<br/>Complaint with Id '" + compid.ToString()+ "' has been closed.<br/><br/>Regards<br/>PRAG ERP");
                string noticeMsg = noticeInfo + "\r\n\r\nDear " + umdl.Title + " " + umdl.FullName + ",\r\nYour complaint with ID '" + compid.ToString() + "' has been closed.\r\n\r\nRegards\r\nPRAG ERP PRAG INDUSTRIES (INDIA) PVT LIMITED";
                mc.performAPICall_SendSMS(umdl.MobileNo, noticeMsg);
                //
                Result = true;
                Message = "Complaint Replied Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ComplaintBLL", "replyComplaint", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void forwardComplaint(int compid, int forwardedto, string remarks)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            if (forwardedto == 0)
            {
                Message = "Forwarding user not selected!";
                return;
            }
            if (remarks.Length == 0)
            {
                Message = "Remarks not entered!";
                return;
            }
            //for sms
            UserMdl umdl = new UserMdl();
            UserBLL ubll = new UserBLL();
            umdl = ubll.searchObject(Convert.ToInt32(objCookie.getUserId()));
            //
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_complaintdetail_to_forward";
                cmd.Parameters.Add(mc.getPObject("@replyuser", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@forwardedto", forwardedto, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@Remarks", remarks.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_complaint, compid.ToString(), "Complaint Forwarded");
                //
                string noticeInfo = "Date: " + mc.getStringByDate(DateTime.Now) + " " + DateTime.Now.ToShortTimeString();
                mc.SendEmailByERP(getComplaintEmailAddress(), "ERP Complaint Forwarded", noticeInfo + "<br/><br/>Dear Sir,<br/>Complaint with Id '" + compid.ToString() + "' has been forwarded to resolve.<br/><br/>Regards<br/>PRAG ERP");
                string noticeMsg = noticeInfo + "\r\n\r\nDear " + umdl.Title + " " + umdl.FullName + ",\r\nYour complaint with ID '" + compid.ToString() + "' has been forwarded to resolve.\r\n\r\nRegards\r\nPRAG ERP PRAG INDUSTRIES (INDIA) PVT LIMITED";
                mc.performAPICall_SendSMS(umdl.MobileNo, noticeMsg);
                //
                Result = true;
                Message = "Complaint Forwarded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ComplaintBLL", "forwardComplaint", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteComplaint(int compid)
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
                cmd.CommandText = "usp_delete_tbl_complaint";
                cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_complaint,compid.ToString(), "Complaint Deleted");
                Result = true;
                Message = "Complaint Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ComplaintBLL", "deleteComplaint", ex.Message);
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
        public ArrayList getComplaintEmailAddress()
        {
            int emailset = 6; //note
            DataSet dsEmail = new DataSet();
            ArrayList arl = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_emailset_for_alert";
            cmd.Parameters.Add(mc.getPObject("@eset", emailset, DbType.Int32));
            mc.fillFromDatabase(dsEmail, cmd);
            if (dsEmail.Tables.Count > 0)
            {
                if (dsEmail.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmail.Tables[0].Rows.Count; i++)
                    {
                        arl.Add(dsEmail.Tables[0].Rows[i]["email"].ToString());
                    }
                }
            }
            return arl;
        }
        //
        internal ComplaintMdl searchObject(int compid)
        {
            DataSet ds = new DataSet();
            ComplaintMdl dbobject = new ComplaintMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_saerch_tbl_complaint";
            cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@replyuser", objCookie.getUserId(), DbType.Int32));
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
        #region compuser
        //
        private DataSet getComplaintListForUser()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_complaint_list_for_user";
            cmd.Parameters.Add(mc.getPObject("@compuser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ComplaintMdl> getComplaintListForUser_List()
        {
            DataSet ds = getComplaintListForUser();
            return createObjectList(ds);
        }
        //
        #endregion
        //
        #region reply
        //
        private DataSet getComplaintListToReply()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_complaint_list_to_reply";
            cmd.Parameters.Add(mc.getPObject("@replyuser", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ComplaintMdl> getComplaintListToReply_List()
        {
            DataSet ds = getComplaintListToReply();
            return createObjectList(ds);
        }
        //       
        private DataSet getComplaintListForReplyUser(string compstatus)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_complaint_list_for_reply_user";
            cmd.Parameters.Add(mc.getPObject("@replyuser", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compstatus", compstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ComplaintMdl> getComplaintListForReplyUser_List(string compstatus)
        {
            DataSet ds = getComplaintListForReplyUser(compstatus);
            return createObjectList(ds);
        }
        //
        #endregion
        //
        internal List<EntryGroupMdl> getComplaintGroupList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_complaintgroup_list";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<EntryGroupMdl> units = new List<EntryGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EntryGroupMdl objmdl = new EntryGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["CompType"].ToString());
                objmdl.GroupName = dr["CompTypeName"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        internal List<UserMdl> getReplyUserList(int compid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_list_of_reply_users";
            cmd.Parameters.Add(mc.getPObject("@compid", compid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<UserMdl> units = new List<UserMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UserMdl objmdl = new UserMdl();
                objmdl.UserId = Convert.ToInt32(dr["userid"].ToString());
                objmdl.FullName = dr["fullname"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        #endregion
        //
    }
}