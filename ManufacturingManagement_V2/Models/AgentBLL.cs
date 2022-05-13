using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AgentBLL : DbContext
    {
        //
        //internal DbSet<AgentMdl> Items { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static AgentBLL Instance
        {
            get { return new AgentBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, AgentMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@AgentName", dbobject.AgentName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SVPTypeId", dbobject.SVPTypeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CityId", dbobject.CityId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ContactNo", dbobject.ContactNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Company", dbobject.Company.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PostalAddress", dbobject.PostalAddress.Trim(), DbType.String));
        }
        //
        private List<AgentMdl> createObjectList(DataSet ds)
        {
            List<AgentMdl> storeitems = new List<AgentMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //if(dr.Table.Columns.Contains("ItemName"))-chkcolumn
                AgentMdl objmdl = new AgentMdl();
                objmdl.AgentId = Convert.ToInt32(dr["AgentId"].ToString());
                objmdl.AgentName = dr["AgentName"].ToString();
                objmdl.SVPTypeId = Convert.ToInt32(dr["SVPTypeId"].ToString());
                objmdl.SVPTypeName = dr["SVPTypeName"].ToString();//d
                objmdl.CityId = Convert.ToInt32(dr["CityId"].ToString());
                objmdl.CityName = dr["CityName"].ToString();//d
                objmdl.ContactNo = dr["ContactNo"].ToString();
                objmdl.Company = dr["Company"].ToString();
                objmdl.EMail = dr["EMail"].ToString();
                objmdl.PostalAddress = dr["PostalAddress"].ToString();
                objmdl.Railway = dr["Railway"].ToString();
                storeitems.Add(objmdl);
            }
            return storeitems;
        }
        //
        private bool checkSetValidModel(AgentMdl dbobject)
        {
            Message = "";
            if (dbobject.AgentName.Length == 0)
            {
                Message = "Empty Agent Name!";
                return false;
            }
            if (dbobject.ContactNo == null)
            {
                dbobject.ContactNo = "";
            }
            if (dbobject.Company == null)
            {
                dbobject.Company = "";
            }
            if (dbobject.EMail == null)
            {
                dbobject.EMail = "";
            }
            if (dbobject.PostalAddress == null)
            {
                dbobject.PostalAddress = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(AgentMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_agent";
                addCommandParameters(cmd, dbobject);
                //
                cmd.Parameters.Add("@AgentId", SqlDbType.Int);
                cmd.Parameters["@AgentId"].Direction = ParameterDirection.Output;
                //
                cmd.ExecuteNonQuery();
                string agentid = cmd.Parameters["@AgentId"].Value.ToString();
                //
                mc.setEventLog(cmd, dbTables.tbl_agent, agentid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                HandleException(ex.Message, "insertObject");
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(AgentMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_agent";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@AgentId", dbobject.AgentId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_agent, dbobject.AgentId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                HandleException(ex.Message, "updateObject");
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        private void HandleException(string ex, string cobj)
        {
            if (ex.ToLower().Contains("uk_tbl_agent") == true)
            {
                Message = "Duplicate entry not allowed!";
            }
            else
            {
                Message = mc.setErrorLog("AgentBLL", cobj, ex);
            }
        }
        //
        #region file management
        internal void uploadAgentFile(AgentMdl dbobject)
        {
            //dbobject.FileContent = ConvertToByte(File);
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_save_agent_file";
                cmd.Parameters.AddWithValue("@AgentId", dbobject.AgentId);
                cmd.Parameters.AddWithValue("@FlName", dbobject.FlName);
                cmd.Parameters.AddWithValue("@FileContent", dbobject.FileContent);
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_agent, dbobject.AgentId.ToString(), "File Uploaded");
                Result = true;
                Message = "File Uploaded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentBLL", "uploadAgentFile", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal byte[] getAgentFile(int agentid = 0)
        {
            System.Data.SqlClient.SqlDataReader dataReader;
            byte[] fileContent = null;
            string fileName = "";
            Message = "";
            string constr = System.Configuration.ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed) { conn.Open(); };
                    cmd.Connection = conn;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_agent_file";
                    cmd.Parameters.Add(mc.getPObject("@agentid", agentid, DbType.Int32));
                    dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        dataReader.Read();
                        fileContent = (byte[])dataReader["FileContent"];
                        fileName = dataReader["FLName"].ToString();
                    }
                    if (conn != null) { conn.Close(); };
                }
                Message = fileName;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentBLL", "getAgentFile", ex.Message);
            }
            return fileContent;
        }
        //
        #endregion file management
        internal void deleteObject(int agentid)
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
                cmd.CommandText = "usp_delete_tbl_agent";
                cmd.Parameters.Add(mc.getPObject("@agentid", agentid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_agent, agentid.ToString(), "Delete");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AgentBLL", "deleteObject", ex.Message);
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
        internal AgentMdl searchObject(int agentid)
        {
            DataSet ds = new DataSet();
            AgentMdl dbobject = new AgentMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_agent";
            cmd.Parameters.Add(mc.getPObject("@agentid", agentid, DbType.Int32));
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
        internal DataSet getObjectData(int svptypeid = 0, int cityid = 0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_agent_new";
            cmd.Parameters.Add(mc.getPObject("@svptypeid", svptypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@cityid", cityid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AgentMdl> getObjectList(int svptypeid = 0, int cityid = 0)
        {
            DataSet ds = getObjectData(svptypeid, cityid);
            return createObjectList(ds);
        }
        //
        internal DataSet getServicePartnerTypeData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_servicepartnerptype";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AgentMdl> getServicePartnerTypeList()
        {
            DataSet ds = new DataSet();
            ds = getServicePartnerTypeData();
            List<AgentMdl> objlist = new List<AgentMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AgentMdl objmdl = new AgentMdl();
                objmdl.SVPTypeId = Convert.ToInt32(dr["SVPTypeId"].ToString());
                objmdl.SVPTypeName = dr["SVPTypeName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}