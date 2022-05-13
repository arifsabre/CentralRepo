
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
    public class ModifyAdviceBLL : DbContext
    {
        //
        //internal DbSet<SaleLedgerMdl> StockLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static ModifyAdviceBLL Instance
        {
            get { return new ModifyAdviceBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int porderid, ModifyAdviceMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@POrderId", porderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RecSlNo", dbobject.RecSlNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ModifyAdvNo", dbobject.ModifyAdvNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MAFor", dbobject.MAFor.Trim(), DbType.String));
        }
        //
        internal List<ModifyAdviceMdl> createObjectList(DataSet ds)
        {
            List<ModifyAdviceMdl> ledgers = new List<ModifyAdviceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ModifyAdviceMdl objmdl = new ModifyAdviceMdl();
                objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                objmdl.RecSlNo = Convert.ToInt32(dr["RecSlNo"].ToString());
                objmdl.ModifyAdvNo = dr["ModifyAdvNo"].ToString();
                objmdl.MAFor = dr["MAFor"].ToString();
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        private bool checkSetValidModel(ModifyAdviceMdl dbobject)
        {
            Message = "";
            if (dbobject.POrderId == 0)
            {
                Message = "Invalid attempt!";
                return false;
            }

            return true;
        }
        #endregion
        //
        #region dml objects
        //
        internal void InsertModifyAdvice(ModifyAdviceMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_modifyadvlist";
                addCommandParameters(cmd, dbobject.POrderId, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_modifyadvlist, dbobject.POrderId.ToString()+"-"+dbobject.RecSlNo.ToString(), "Inserted");
                Result = true;
                Message = "MA Record Saved Successfully!";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("pk_tbl_modifyadvlist") == true)
                {
                    Message = "Duplicate MA serial number entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ModifyAdviceBLL", "InsertModifyAdvice", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void UpdateModifyAdvice(ModifyAdviceMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_modifyadvlist";
                addCommandParameters(cmd, dbobject.POrderId, dbobject);
                cmd.Parameters.Add(mc.getPObject("@editRecSlNo", dbobject.editRecSlNo, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_modifyadvlist, dbobject.POrderId.ToString() + "-" + dbobject.editRecSlNo.ToString(), "Updated");
                Result = true;
                Message = "MA Record Updated Successfully!";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("pk_tbl_modifyadvlist") == true)
                {
                    Message = "Duplicate MA serial number entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ModifyAdviceBLL", "UpdateModifyAdvice", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void DeleteModifyAdvice(int porderid, int recslno)
        {
            Result = false;
            Message = "";
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_modifyadvlist_record";
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@recslno", recslno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_modifyadvlist, porderid.ToString() + "-" + recslno.ToString(), "Deleted");
                Result = true;
                Message = "MA Record Deleted successfully!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ModifyAdviceBLL", "DeleteModifyAdvice", ex.Message);
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
        internal ModifyAdviceMdl searchModifyAdvice(int porderid, int recslno)
        {
            DataSet ds = new DataSet();
            ModifyAdviceMdl objmdl = new ModifyAdviceMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_modifyadvlist";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@recslno", recslno, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objmdl = createObjectList(ds)[0];
            }
            return objmdl;
        }
        //
        internal DataSet getModifyAdviceData(int porderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_modifyadvlist";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ModifyAdviceMdl> getModifyAdviceList(int porderid)
        {
            DataSet ds = getModifyAdviceData(porderid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}