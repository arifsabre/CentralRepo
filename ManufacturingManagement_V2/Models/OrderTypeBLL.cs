using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class OrderTypeBLL : DbContext
    {
        //
        internal DbSet<OrderTypeMdl> ots { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static OrderTypeBLL Instance
        {
            get { return new OrderTypeBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, OrderTypeMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@OrderTypeName", dbobject.OrderTypeName.Trim(), DbType.String));
        }
        //
        private List<OrderTypeMdl> createObjectList(DataSet ds)
        {
            List<OrderTypeMdl> objlst = new List<OrderTypeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderTypeMdl objmdl = new OrderTypeMdl();
                objmdl.OrderTypeId = Convert.ToInt32(dr["OrderTypeId"].ToString());
                objmdl.OrderTypeName = dr["OrderTypeName"].ToString();
                objlst.Add(objmdl);
            }
            return objlst;
        }
        //
        private bool isAlreadyFound(string ordertypename)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_ordertype";
            cmd.Parameters.Add(mc.getPObject("@ordertypename", ordertypename, DbType.String));
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
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(OrderTypeMdl dbobject)
        {
            Result = false;
            if (isAlreadyFound(dbobject.OrderTypeName) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_ordertype";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string ordertypeid = mc.getRecentIdentityValue(cmd, dbTables.tbl_ordertype, "ordertypeid");
                mc.setEventLog(cmd, dbTables.tbl_ordertype, ordertypeid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("OrderTypeBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(OrderTypeMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_ordertype";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@ordertypeid", dbobject.OrderTypeId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_ordertype, dbobject.OrderTypeId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_ordertype") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("OrderTypeBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int ordertypeid)
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
                cmd.CommandText = "usp_delete_tbl_ordertype";
                cmd.Parameters.Add(mc.getPObject("@ordertypeid", ordertypeid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_ordertype, ordertypeid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("OrderTypeBLL", "deleteObject", ex.Message);
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
        internal OrderTypeMdl searchObject(int ordertypeid)
        {
            DataSet ds = new DataSet();
            OrderTypeMdl dbobject = new OrderTypeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_ordertype";
            cmd.Parameters.Add(mc.getPObject("@ordertypeid", ordertypeid, DbType.Int32));
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
            cmd.CommandText = "usp_get_tbl_ordertype";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<OrderTypeMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}