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
    public class QuailMeetingBLL : DbContext
    {
        //
        //internal DbSet<QuailMeetingMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, QuailMeetingMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@QlbDate", dbobject.QlbDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@QlbTime", dbobject.QlbTime.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FctnId", dbobject.FctnId, DbType.Int32));
        }
        //
        private List<QuailMeetingMdl> createObjectList(DataSet ds)
        {
            List<QuailMeetingMdl> objlist = new List<QuailMeetingMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailMeetingMdl objmdl = new QuailMeetingMdl();
                objmdl.QmId = Convert.ToInt32(dr["QmId"].ToString());
                objmdl.QlbDate = Convert.ToDateTime(dr["QlbDate"].ToString());
                objmdl.QlbTime = dr["QlbTime"].ToString();
                objmdl.FctnId = Convert.ToInt32(dr["FctnId"].ToString());
                objmdl.FctnName = dr["FctnName"].ToString();//d
                objmdl.FnLeader = dr["FnLeader"].ToString();//d
                objmdl.QmStatus = Convert.ToInt32(dr["QmStatus"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(DateTime qlbdate,int fctnid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_quailmeeting";
            cmd.Parameters.Add(mc.getPObject("@qlbdate", qlbdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@fctnid", fctnid, DbType.Int32));
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
        private bool checkSetValidModel(QuailMeetingMdl dbobject)
        {
            if (dbobject.FctnId == 0)
            {
                Message = "Function not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.QlbDate) == false)
            {
                Message = "Invalid meeting date!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(QuailMeetingMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.QlbDate,dbobject.FctnId) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_quailmeeting";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string qmid = mc.getRecentIdentityValue(cmd, dbTables.tbl_quailmeeting, "qmid");
                mc.setEventLog(cmd, dbTables.tbl_quailmeeting, qmid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailMeetingBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(QuailMeetingMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_quailmeeting";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@qmid", dbobject.QmId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailmeeting, dbobject.QmId.ToString(), "Updated");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_quailmeeting") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("QuailMeetingBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int qmid)
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
                cmd.CommandText = "usp_delete_tbl_quailmeeting";
                cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_quailmeeting, qmid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("QuailMeetingBLL", "deleteObject", ex.Message);
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
        internal QuailMeetingMdl searchObject(int qmid)
        {
            DataSet ds = new DataSet();
            QuailMeetingMdl dbobject = new QuailMeetingMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_quailmeeting";
            cmd.Parameters.Add(mc.getPObject("@qmid", qmid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_quailmeeting";
            //cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@empid", empid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<QuailMeetingMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal List<QuailCalendarMdl> getQuailCalendar(int mth, int yr)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quail_meeting_calendar";
            cmd.Parameters.Add(mc.getPObject("@mth", mth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@yr", yr, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<QuailCalendarMdl> objlst = new List<QuailCalendarMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                QuailCalendarMdl objmdl = new QuailCalendarMdl();
                objmdl.Mth = Convert.ToInt32(dr["Mth"].ToString());
                objmdl.Yr = Convert.ToInt32(dr["yr"].ToString());
                objmdl.Mth = Convert.ToInt32(dr["Mth"].ToString());
                objmdl.WKN = Convert.ToInt32(dr["WKN"].ToString());
                objmdl.Monday = Convert.ToDateTime(dr["Monday"].ToString());
                objmdl.Tuesday = Convert.ToDateTime(dr["Tuesday"].ToString());
                objmdl.Wednesday = Convert.ToDateTime(dr["Wednesday"].ToString());
                objmdl.Thursday = Convert.ToDateTime(dr["Thursday"].ToString());
                objmdl.Friday = Convert.ToDateTime(dr["Friday"].ToString());
                objmdl.Saturday = Convert.ToDateTime(dr["Saturday"].ToString());
                objmdl.Sunday = Convert.ToDateTime(dr["Sunday"].ToString());
                objmdl.MondayInfo = dr["MondayInfo"].ToString();
                objmdl.TuesdayInfo = dr["TuesdayInfo"].ToString();
                objmdl.WednesdayInfo = dr["WednesdayInfo"].ToString();
                objmdl.ThursdayInfo = dr["ThursdayInfo"].ToString();
                objmdl.FridayInfo = dr["FridayInfo"].ToString();
                objmdl.SaturdayInfo = dr["SaturdayInfo"].ToString();
                objmdl.SundayInfo = dr["SundayInfo"].ToString();
                objmdl.MondayQmId = Convert.ToInt32(dr["MondayQmId"].ToString());
                objmdl.TuesdayQmId = Convert.ToInt32(dr["TuesdayQmId"].ToString());
                objmdl.WednesdayQmId = Convert.ToInt32(dr["WednesdayQmId"].ToString());
                objmdl.ThursdayQmId = Convert.ToInt32(dr["ThursdayQmId"].ToString());
                objmdl.FridayQmId = Convert.ToInt32(dr["FridayQmId"].ToString());
                objmdl.SaturdayQmId = Convert.ToInt32(dr["SaturdayQmId"].ToString());
                objmdl.SundayQmId = Convert.ToInt32(dr["SundayQmId"].ToString());
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        #endregion
        //
    }
}