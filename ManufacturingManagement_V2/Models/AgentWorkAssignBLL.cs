using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AgentWorkAssignBLL : DbContext
    {
        //
        //internal DbSet<AgentWorkAssignMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static AgentWorkAssignBLL Instance
        {
            get { return new AgentWorkAssignBLL(); }
        }
        //
        #region private objects
        //
        private List<AgentWorkAssignMdl> createObjectList(DataSet ds)
        {
            List<AgentWorkAssignMdl> objlist = new List<AgentWorkAssignMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                AgentWorkAssignMdl objmdl = new AgentWorkAssignMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.AgentId = Convert.ToInt32(dr["AgentId"].ToString());
                objmdl.AgentName = dr["AgentName"].ToString();//d
                objmdl.SVPTypeName = dr["SVPTypeName"].ToString();//d
                objmdl.WorkId = dr["WorkId"].ToString();
                objmdl.WorkName = dr["WorkName"].ToString();//d
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.WorkDetail = dr["WorkDetail"].ToString();
                objmdl.DueDays = Convert.ToInt32(dr["DueDays"].ToString());
                objmdl.DocNumber = dr["DocNumber"].ToString();
                objmdl.DocDate = Convert.ToDateTime(dr["DocDate"].ToString());
                objmdl.AssignDate = Convert.ToDateTime(dr["AssignDate"].ToString());
                objmdl.UniqueIdNo = dr["UniqueIdNo"].ToString();
                objmdl.AckStatus = dr["AckStatus"].ToString();
                objmdl.AckDate = Convert.ToDateTime(dr["AckDate"].ToString());
                objmdl.CompStatus = dr["CompStatus"].ToString();
                objmdl.CompDate = Convert.ToDateTime(dr["CompDate"].ToString());
                objmdl.BillStatus = dr["BillStatus"].ToString();
                objmdl.WorkStatus = dr["WorkStatus"].ToString();
                objmdl.ReqCompDate = Convert.ToDateTime(dr["ReqCompDate"].ToString());
                objmdl.DelayDays = Convert.ToInt32(dr["DelayDays"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updateAgentWorkAcknowledgement(int recid, string ackstatus, DateTime ackdate)
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
                cmd.CommandText = "usp_update_agentwork_acknowledgement";
                cmd.Parameters.Add(mc.getPObject("@ackstatus", ackstatus.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ackdate", ackdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_agentworkassign, recid.ToString(), "Acknowledgement Updated");
                Result = true;
                Message = "Acknowledgement Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentWorkAssignBLL", "updateAgentWorkAcknowledgement", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateAgentBillStatus(int recid, string billstatus)
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
                cmd.CommandText = "usp_update_agentwork_billstatus";
                cmd.Parameters.Add(mc.getPObject("@billstatus", billstatus.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_agentworkassign, recid.ToString(), "Bill Status Updated");
                Result = true;
                Message = "Bill Status Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentWorkAssignBLL", "updateAgentBillStatus", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_agentworkassign";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_agentworkassign, recid.ToString(), "Delete");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentWorkAssignBLL", "deleteObject", ex.Message);
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
        internal AgentWorkAssignMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            AgentWorkAssignMdl dbobject = new AgentWorkAssignMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_agentworkassign";
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
        internal DataSet getObjectData(int agentid, string workid, string ackstatus, string compstatus, string workstatus, string billstatus)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_agentworkassign";
            cmd.Parameters.Add(mc.getPObject("@agentid", agentid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@workid", workid.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ackstatus", ackstatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compstatus", compstatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@workstatus", workstatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billstatus", billstatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AgentWorkAssignMdl> getObjectList(int agentid, string workid, string ackstatus, string compstatus, string workstatus, string billstatus)
        {
            DataSet ds = getObjectData(agentid, workid, ackstatus, compstatus,workstatus, billstatus);
            return createObjectList(ds);
        }
        //
        internal DataSet getAgentWorkListData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_agentwork";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getAgentWorkList()
        {
            DataSet ds = getAgentWorkListData();
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectCode = dr["WorkId"].ToString();
                objmdl.ObjectName = dr["WorkName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}