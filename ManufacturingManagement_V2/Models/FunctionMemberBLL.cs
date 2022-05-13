using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FunctionMemberBLL : DbContext
    {
        //
        //internal DbSet<FunctionMemberMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static FunctionMemberBLL Instance
        {
            get { return new FunctionMemberBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, FunctionMemberMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@FctnId", dbobject.FctnId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@UserId", dbobject.UserId, DbType.Int32));
        }
        //
        private List<FunctionMemberMdl> createObjectList(DataSet ds)
        {
            List<FunctionMemberMdl> objlist = new List<FunctionMemberMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FunctionMemberMdl objmdl = new FunctionMemberMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.FctnId = Convert.ToInt32(dr["FctnId"].ToString());
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                objmdl.FctnName = dr["FctnName"].ToString();//d
                objmdl.FnMember = dr["FnMember"].ToString();//d
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());//d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(int fctnid,int userid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_functionmember";
            cmd.Parameters.Add(mc.getPObject("@fctnid", fctnid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
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
        private bool checkSetValidModel(FunctionMemberMdl dbobject)
        {
            if (dbobject.UserId == 0)
            {
                Message = "Member not selected!";
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
        internal void insertObject(FunctionMemberMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.FctnId,dbobject.UserId) == true) { return; };
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
                cmd.CommandText = "usp_insert_tbl_functionmember";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_functionmember, "recid");
                mc.setEventLog(cmd, dbTables.tbl_functionmember, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FunctionMemberBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(FunctionMemberMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_functionmember";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_functionmember, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_functionmember") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("FunctionMemberBLL", "updateObject", ex.Message);
                }
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
                cmd.CommandText = "usp_delete_tbl_functionmember";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_functionmember, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("FunctionMemberBLL", "deleteObject", ex.Message);
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
        internal FunctionMemberMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            FunctionMemberMdl dbobject = new FunctionMemberMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_functionmember";
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
        internal DataSet getObjectData(int fctnid, int userid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_functionmember";
            cmd.Parameters.Add(mc.getPObject("@fctnid", fctnid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<FunctionMemberMdl> getObjectList(int fctnid, int userid)
        {
            DataSet ds = getObjectData(fctnid,userid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}