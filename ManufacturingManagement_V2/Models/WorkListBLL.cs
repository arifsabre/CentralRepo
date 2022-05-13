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
    public class WorkListBLL : DbContext
    {
        //
        //internal DbSet<WorkListMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, WorkListMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@RecDT", dbobject.RecDT.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@DepId", mc.getForSqlIntString(dbobject.DepId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TaskName", dbobject.TaskName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@WorkDesc", dbobject.WorkDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsCompleted", "0", DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompDT", dbobject.RecDT.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CompCode", mc.getForSqlIntString(dbobject.CompCode.ToString()), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@TaskOpt", dbobject.TaskOpt, DbType.Int32));
        }
        //
        private List<WorkListMdl> createObjectList(DataSet ds)
        {
            List<WorkListMdl> advances = new List<WorkListMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WorkListMdl objmdl = new WorkListMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.DepId = Convert.ToInt32(dr["DepId"].ToString());
                objmdl.DepName = dr["DepName"].ToString();//d
                objmdl.RecDT = Convert.ToDateTime(dr["RecDT"].ToString());
                objmdl.TaskName = dr["TaskName"].ToString();
                objmdl.WorkDesc = dr["WorkDesc"].ToString();
                objmdl.IsCompleted = Convert.ToBoolean(dr["IsCompleted"].ToString());
                objmdl.CompDT = Convert.ToDateTime(dr["CompDT"].ToString());
                objmdl.CompDTStr = dr["CompDTStr"].ToString();//d
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.CompName = dr["CompName"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.TaskOpt = Convert.ToInt32(dr["TaskOpt"].ToString());
                objmdl.TaskOptName = dr["TaskOptName"].ToString();//d
                objmdl.Doc = Convert.ToBoolean(dr["Doc"].ToString());
                advances.Add(objmdl);
            }
            return advances;
        }
        //
        private bool checkSetValidModel(WorkListMdl dbobject)
        {
            if (mc.isValidDate(dbobject.RecDT) == false)
            {
                Message = "Invalid date!";
                return false;
            }
            List<EntryGroupMdl> compGroup = getComplianceGroupList();
            bool flg = false;
            for (int i = 0; i < compGroup.Count; i++)
            {
                if (compGroup[i].GroupId == dbobject.TaskOpt)
                {
                    flg = true;
                    break;
                }
            }
            if (flg == false)
            {
                Message = "Permission denied!";
                return false;
            }
            return true;
        }
        //
        internal bool isValidToModifyWorkList(int recid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_to_modify_worklist";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        internal bool isValidToDeleteWorkList(int recid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_to_delete_worklist";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(WorkListMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_worklist";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_worklist, "recid");
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid, "Inserted");
                Result = true;
                Message = "Compliance Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateWorklistDocument(int recid, bool doc)
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
                cmd.CommandText = "usp_update_worklist_document";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@doc", doc, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid.ToString(), "Document Updated");
                Result = true;
                Message = "Document Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "updateWorklistDocument", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void insertCompliance(WorkListMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (mc.isValidDate(dbobject.CompDT) == false)
            {
                Message = "Invalid completion date!";
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
                cmd.CommandText = "usp_insert_compliance_worklist";
                cmd.Parameters.Add(mc.getPObject("@RecDT", dbobject.RecDT.ToString(), DbType.DateTime));//as next due on
                cmd.Parameters.Add(mc.getPObject("@DepId", mc.getForSqlIntString(dbobject.DepId.ToString()), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@TaskName", dbobject.TaskName.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@WorkDesc", dbobject.WorkDesc, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@IsCompleted", "0", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompDT", dbobject.CompDT.ToString(), DbType.DateTime));//for old work
                cmd.Parameters.Add(mc.getPObject("@CompCode", mc.getForSqlIntString(dbobject.CompCode.ToString()), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@TaskOpt", dbobject.TaskOpt, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_worklist, "recid");
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid, "Compliance Inserted");
                Result = true;
                Message = "Compliance Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "insertCompliance", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(WorkListMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_worklist";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_worklist, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Compliance Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setWorkCompleted(int recid)
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
                cmd.CommandText = "usp_set_work_completed";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid.ToString(), "Marked Completed");
                Result = true;
                Message = "Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "setWorkCompleted", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setWorkPending(int recid)
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
                cmd.CommandText = "usp_set_work_pending";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid.ToString(), "Marked Pending");
                Result = true;
                Message = "Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "setWorkPending", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_worklist";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_worklist, recid.ToString(), "Deleted");
                Result = true;
                Message = "Task Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("WorkListBLL", "deleteObject", ex.Message);
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
        internal WorkListMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            WorkListMdl dbobject = new WorkListMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_worklist";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
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
        internal DataSet getObjectData(bool iscompleted, int depid, string dtfrom, string dtto, int taskopt, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_worklist";
            cmd.Parameters.Add(mc.getPObject("@iscompleted", iscompleted, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@depid", depid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getDateByString(dtfrom), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getDateByString(dtto), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CompCode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@TaskOpt", taskopt, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<WorkListMdl> getObjectList(bool iscompleted, int depid, string dtfrom, string dtto, int taskopt, int compcode)
        {
            DataSet ds = getObjectData(iscompleted,depid, dtfrom,dtto,taskopt,compcode);
            return createObjectList(ds);
        }
        //
        internal List<EntryGroupMdl> getComplianceGroupList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_compliancegroup_list";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<EntryGroupMdl> units = new List<EntryGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EntryGroupMdl objmdl = new EntryGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["TaskOpt"].ToString());
                objmdl.GroupName = dr["TaskOptName"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        #endregion
        //
    }
}