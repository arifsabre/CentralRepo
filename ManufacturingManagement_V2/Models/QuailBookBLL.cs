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
    public class QuailBookBLL : DbContext
    {
        //
        //internal DbSet<QuailBookMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, QuailBookMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@QmId", dbobject.QmId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@MainSNo", dbobject.Items[0].MainSNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SubSNo", dbobject.Items[0].SubSNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@QlbItem", dbobject.Items[0].QlbItem.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemType", dbobject.Items[0].ItemType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@QlbPriority", dbobject.Items[0].QlbPriority, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@QlbUserId", dbobject.Items[0].QlbUserId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompTime", dbobject.Items[0].CompTime.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Items[0].Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AddNew", dbobject.AddNew, DbType.String));
        }
        //
        private List<QuailBookMdl> createObjectList(DataSet ds)
        {
            List<QuailBookMdl> objlist = new List<QuailBookMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailBookMdl objmdl = new QuailBookMdl();
                objmdl.QmId = Convert.ToInt32(dr["QmId"].ToString());
                objmdl.QlbDate = Convert.ToDateTime(dr["QlbDate"].ToString());
                objmdl.FctnId = Convert.ToInt32(dr["FctnId"].ToString());
                objmdl.FctnName = dr["FctnName"].ToString();
                if (dr.Table.Columns.Contains("PrvDate"))
                {
                    objmdl.PrvDate = Convert.ToDateTime(dr["PrvDate"].ToString());
                }
                if (dr.Table.Columns.Contains("FnLeader"))
                {
                    objmdl.FnLeader = dr["FnLeader"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<QuailAttendanceMdl> objCreateAttendanceList(DataSet ds)
        {
            List<QuailAttendanceMdl> objlist = new List<QuailAttendanceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailAttendanceMdl objmdl = new QuailAttendanceMdl();
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                objmdl.UserName = dr["QMember"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<QuailBookItemMdl> createObjectLedgerList(DataSet ds)
        {
            List<QuailBookItemMdl> objlist = new List<QuailBookItemMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailBookItemMdl objmdl = new QuailBookItemMdl();
                objmdl.DispSNo = dr["DispSNo"].ToString();//d
                objmdl.MainSNo = Convert.ToInt32(dr["MainSNo"].ToString());
                objmdl.SubSNo = Convert.ToInt32(dr["SubSNo"].ToString());
                objmdl.QlbItem = dr["QlbItem"].ToString();
                objmdl.ItemType = dr["ItemType"].ToString();
                objmdl.QlbPriority = Convert.ToInt32(dr["QlbPriority"].ToString());
                objmdl.QlbUserId = Convert.ToInt32(dr["QlbUserId"].ToString());
                objmdl.QlbUserName = dr["QlbUserName"].ToString();//d
                objmdl.CompTime = dr["CompTime"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(QuailBookMdl dbobject)
        {
            if (dbobject.QmId == 0)
            {
                Message = "Invalid entry!";
                return false;
            }
            //add other validations
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updateQuailBookItem(QuailBookMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_quailbook_item";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string pkv = dbobject.QmId.ToString() + "-" + dbobject.Items[0].MainSNo.ToString() + "-" + dbobject.Items[0].SubSNo.ToString();
                mc.setEventLog(cmd, dbTables.tbl_quailbook, pkv, "Updated");
                Result = true;
                Message = "Item Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "updateQuailItem", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteQuailBook(int qmid)
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
                cmd.CommandText = "usp_delete_tbl_quailbook";
                cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailbook, qmid.ToString(), "QUAIL Card Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "deleteQuailBook", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteQuailBookItem(QuailBookMdl dbobject)
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
                cmd.CommandText = "usp_delete_quailbook_item";
                cmd.Parameters.Add(mc.getPObject("@qmid", dbobject.QmId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@MainSNo", dbobject.Items[0].MainSNo, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@SubSNo", dbobject.Items[0].SubSNo, DbType.Int32));
                cmd.ExecuteNonQuery();
                string pkv = dbobject.QmId.ToString() + "-" + dbobject.Items[0].MainSNo.ToString() + "-" + dbobject.Items[0].SubSNo.ToString();
                mc.setEventLog(cmd, dbTables.tbl_quailbook, pkv, "QUAIL Card Item Deleted");
                Result = true;
                Message = "Quail Item Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "deleteQuailBookItem", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setDefaultAttendance(QuailBookMdl dbobject)
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
                for (int i = 0; i < dbobject.Attendance.Count; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_quailattendance";
                    cmd.Parameters.Add(mc.getPObject("@QmId", dbobject.QmId, DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@UserId", dbobject.Attendance[i].UserId, DbType.Int32));
                    cmd.ExecuteNonQuery();
                }
                Result = true;
                Message = "Attendance Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "setDefaultAttendance", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void addAttendance(int qmid,int userid)
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
                cmd.CommandText = "usp_insert_tbl_quailattendance";
                cmd.Parameters.Add(mc.getPObject("@QmId", qmid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@UserId", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Attendance Added Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "addAttendance", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteAttendance(int qmid, int userid)
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
                cmd.CommandText = "usp_delete_quailattendance";
                cmd.Parameters.Add(mc.getPObject("@QmId", qmid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@UserId", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Attendance Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailBookBLL", "deleteAttendance", ex.Message);
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
        internal QuailBookMdl searchQuailBook(int qmid, bool chksubitem, int qlbpriority)
        {
            DataSet ds = new DataSet();
            QuailBookMdl dbobject = new QuailBookMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quailbook_info";
            cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject.QmId = Convert.ToInt32(ds.Tables[0].Rows[0]["QmId"].ToString());
                    dbobject.QlbDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["QlbDate"].ToString());
                    dbobject.FctnId = Convert.ToInt32(ds.Tables[0].Rows[0]["FctnId"].ToString());
                    dbobject.FctnName = ds.Tables[0].Rows[0]["FctnName"].ToString();
                    dbobject.PrvDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["PrvDate"].ToString());
                    dbobject.QmStatus = Convert.ToInt32(ds.Tables[0].Rows[0]["QmStatus"].ToString());
                    dbobject.PrvQmId = Convert.ToInt32(ds.Tables[0].Rows[0]["PrvQmId"].ToString());
                    dbobject.NextQmId = Convert.ToInt32(ds.Tables[0].Rows[0]["NextQmId"].ToString());
                    dbobject.NextDateTime = "";
                    if(dbobject.NextQmId !=0)
                    {
                        dbobject.NextDateTime = mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[0]["NextDate"].ToString())) + " at " + ds.Tables[0].Rows[0]["NextQlbTime"].ToString();
                    }
                }
            }
            DataSet dsLgr = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quailbook_item_ledger";
            cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@chksubitem", chksubitem, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@qlbpriority", qlbpriority, DbType.Int32));
            mc.fillFromDatabase(dsLgr, cmd);
            dbobject.Items = createObjectLedgerList(dsLgr);
            return dbobject;
        }
        //
        internal DataSet getDefaultQuailAttendance(int qmid)
        {
            DataSet ds = new DataSet();
            QuailBookMdl dbobject = new QuailBookMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_default_meeting_members";
            cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getQuailAttendanceMain(int qmid)
        {
            DataSet ds = new DataSet();
            QuailBookMdl dbobject = new QuailBookMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quailattendance";
            cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal QuailBookMdl getQuailAttendanceList(int qmid, string opt)
        {
            QuailBookMdl dbobject = new QuailBookMdl();
            DataSet ds = new DataSet();
            if (opt == "default")
            {
                ds = getDefaultQuailAttendance(qmid);
            }
            else if (opt == "main")
            {
                ds = getQuailAttendanceMain(qmid);
            }
            dbobject.QmId = qmid;
            dbobject.Attendance = objCreateAttendanceList(ds);
            return dbobject;
        }
        //
        #endregion
        //
    }
}