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
    public class ETenderConsigneeBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static ETenderConsigneeBLL Instance
        {
            get { return new ETenderConsigneeBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ETenderConsigneeMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@ItemRecId", dbobject.ItemRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@ConsigneeId", dbobject.ConsigneeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TenderQty", dbobject.TenderQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OfferedQty", dbobject.OfferedQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SaleTaxType", dbobject.SaleTaxType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SaleTaxPer", dbobject.SaleTaxPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Freight", dbobject.Freight, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BasicRate", dbobject.BasicRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitRate", dbobject.UnitRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TotalAmount", dbobject.TotalAmount, DbType.Double));
        }
        //
        private List<ETenderConsigneeMdl> createObjectList(DataSet ds)
        {
            List<ETenderConsigneeMdl> objlist = new List<ETenderConsigneeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ETenderConsigneeMdl objmdl = new ETenderConsigneeMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.ItemRecId = Convert.ToInt32(dr["ItemRecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.ConsigneeId = Convert.ToInt32(dr["ConsigneeId"].ToString());
                objmdl.ConsigneeName = dr["ConsigneeName"].ToString();//d
                objmdl.TenderQty = Convert.ToDouble(dr["TenderQty"].ToString());
                objmdl.OfferedQty = Convert.ToDouble(dr["OfferedQty"].ToString());
                objmdl.SaleTaxType = dr["SaleTaxType"].ToString();
                objmdl.SaleTaxPer = Convert.ToDouble(dr["SaleTaxPer"].ToString());
                objmdl.Freight = Convert.ToDouble(dr["Freight"].ToString());
                objmdl.BasicRate = Convert.ToDouble(dr["BasicRate"].ToString());
                objmdl.UnitRate = Convert.ToDouble(dr["UnitRate"].ToString());
                objmdl.TotalAmount = Convert.ToDouble(dr["TotalAmount"].ToString());
                objmdl.LoaQty = Convert.ToDouble(dr["LoaQty"].ToString());
                objmdl.LoaRate = Convert.ToDouble(dr["LoaRate"].ToString());
                objmdl.LoaAmt = Convert.ToDouble(dr["LoaAmt"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(ETenderConsigneeMdl dbobject)
        {
            Message = "";
            if (dbobject.ItemRecId == 0)
            {
                Message = "Invalid entry!";
                return false;
            }
            if (dbobject.ConsigneeId == 0)
            {
                Message = "Consignee not selected!";
                return false;
            }
            return true;
        }
        //
        private bool isValidOfferedQty(ETenderConsigneeMdl dbobject)
        {
            Message = "";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_offeredQty";
            cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemrecid", dbobject.ItemRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@qty", dbobject.OfferedQty, DbType.Int32));
            bool res = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (res == false)
            {
                Message = "Total offered quantity must not be greater than full tender quantity!";
            }
            return res;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertETenderConsignee(ETenderConsigneeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isValidOfferedQty(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_etenderconsignee";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ETenderConsigneeBLL", "insertETenderConsignee", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateETenderConsignee(ETenderConsigneeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isValidOfferedQty(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_etenderconsignee";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ETenderConsigneeBLL", "updateETenderConsignee", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteETenderConsignee(int recid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            Message = "";
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
                cmd.CommandText = "usp_delete_tbl_etenderconsignee";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_etenderitem, tenderid.ToString(), "Deleted");
                trn.Commit();
                Message = "Record Deleted Successfully";
                Result = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ETenderConsigneeBLL", "deleteETenderConsignee", ex.Message);
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
        internal DataSet getETenderConsigneeData(int itemrecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_etenderconsignee";
            cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ETenderConsigneeMdl> getObjectList(int itemrecid)
        {
            DataSet ds = getETenderConsigneeData(itemrecid);
            return createObjectList(ds);
        }
        //
        internal ETenderConsigneeMdl searchETenderConsignee(int tenderid, int itemrecid, int recid)
        {
            DataSet ds = new DataSet();
            ETenderConsigneeMdl objmdl = new ETenderConsigneeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_etenderconsignee";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                objmdl.RecId = 0;
                objmdl.ItemRecId = itemrecid;
                objmdl.TenderId = tenderid;//d
                objmdl.ConsigneeId = 0;
                objmdl.ConsigneeName = "";//d
                objmdl.TenderQty = 0;
                objmdl.OfferedQty = 0;
                objmdl.SaleTaxType = "";
                objmdl.SaleTaxPer = 0;
                objmdl.Freight = 0;
                objmdl.BasicRate = 0;
                objmdl.UnitRate = 0;
                objmdl.TotalAmount = 0;
                objmdl.LoaQty = 0;
                objmdl.LoaRate = 0;
                objmdl.LoaAmt = 0;
            }
            else if(ds.Tables[0].Rows.Count > 0)
            {
                objmdl = createObjectList(ds)[0];
            }
            ETenderItemBLL etItemBll = new ETenderItemBLL();
            objmdl.ETenderItems = etItemBll.getObjectList(objmdl.TenderId);
            objmdl.ETenderConsigneeItems = getObjectList(objmdl.ItemRecId);
            return objmdl;
        }
        //

        #endregion
        //
    }
}