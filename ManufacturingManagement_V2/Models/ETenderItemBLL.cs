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
    public class ETenderItemBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static ETenderItemBLL Instance
        {
            get { return new ETenderItemBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ETenderItemMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@TenderId", dbobject.TenderId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TDRecId", dbobject.TDRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SpcChkList", dbobject.SpcChkList.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SpcRemarks", dbobject.SpcRemarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemDescOpt", dbobject.ItemDescOpt.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemDescOffered", dbobject.ItemDescOffered.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FullTDQty", dbobject.FullTDQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitOfMeasurement", dbobject.UnitOfMeasurement.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BasicRatePerUnit", dbobject.BasicRatePerUnit, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UncDiscountPer", dbobject.UncDiscountPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BasicPkgPer", dbobject.BasicPkgPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EDType", dbobject.EDType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EDMaxPerApplicable", dbobject.EDMaxPerApplicable, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CessOnEDPer", dbobject.CessOnEDPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OtherChgType", dbobject.OtherChgType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OtherChgPerUnit", dbobject.OtherChgPerUnit, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@PriceVarReq", dbobject.PriceVarReq, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PriceVarClause", dbobject.PriceVarClause.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
        }
        //
        private List<ETenderItemMdl> createObjectList(DataSet ds)
        {
            List<ETenderItemMdl> objlist = new List<ETenderItemMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ETenderItemMdl objmdl = new ETenderItemMdl();
                objmdl.ItemRecId = Convert.ToInt32(dr["ItemRecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TDRecId = Convert.ToInt32(dr["TDRecId"].ToString());
                objmdl.SpcChkList = dr["SpcChkList"].ToString();
                objmdl.SpcRemarks = dr["SpcRemarks"].ToString();
                objmdl.ItemDescOpt = dr["ItemDescOpt"].ToString();
                objmdl.ItemDescOffered = dr["ItemDescOffered"].ToString();
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemName = dr["ItemName"].ToString();
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ShortName = dr["ShortName"].ToString();
                objmdl.FullTDQty = Convert.ToDouble(dr["FullTDQty"].ToString());
                objmdl.UnitOfMeasurement = dr["UnitOfMeasurement"].ToString();
                objmdl.BasicRatePerUnit = Convert.ToDouble(dr["BasicRatePerUnit"].ToString());
                objmdl.UncDiscountPer = Convert.ToDouble(dr["UncDiscountPer"].ToString());
                objmdl.BasicPkgPer = Convert.ToDouble(dr["BasicPkgPer"].ToString());
                objmdl.EDType = dr["EDType"].ToString();
                objmdl.EDMaxPerApplicable = Convert.ToDouble(dr["EDMaxPerApplicable"].ToString());
                objmdl.CessOnEDPer = Convert.ToDouble(dr["CessOnEDPer"].ToString());
                objmdl.OtherChgType = dr["OtherChgType"].ToString();
                objmdl.OtherChgPerUnit = Convert.ToDouble(dr["OtherChgPerUnit"].ToString());
                objmdl.PriceVarReq = Convert.ToBoolean(dr["PriceVarReq"].ToString());
                objmdl.PriceVarClause = dr["PriceVarClause"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(ETenderItemMdl dbobject)
        {
            Message = "";
            if (dbobject.TenderId == 0)
            {
                Message = "Empty Tender No!";
                return false;
            }
            if (dbobject.TDRecId == 0)
            {
                Message = "Invalid entry!";
                return false;
            }
            if (dbobject.SpcChkList == null)
            {
                dbobject.SpcChkList = "";
            }
            if (dbobject.SpcRemarks == null)
            {
                dbobject.SpcRemarks = "";
            }
            if (dbobject.ItemDescOpt == null)
            {
                dbobject.ItemDescOpt = "";
            }
            if (dbobject.ItemDescOffered == null)
            {
                dbobject.ItemDescOffered = "";
            }
            if (dbobject.UnitOfMeasurement == null)
            {
                dbobject.UnitOfMeasurement = "";
            }
            if (dbobject.EDType == null)
            {
                dbobject.EDType = "";
            }
            if (dbobject.PriceVarClause == null)
            {
                dbobject.PriceVarClause = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.OtherChgType == null)
            {
                dbobject.OtherChgType = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertETenderItem(ETenderItemMdl dbobject)
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
                cmd.CommandText = "usp_insert_tbl_etenderitem";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_etenderitem_tbl_etender"))
                {
                    Message = "Step 1 not completed!";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderItemBLL", "insertETenderItem", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateETenderItem(ETenderItemMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_etenderitem";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@itemrecid", dbobject.ItemRecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_etenderitem"))
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderItemBLL", "updateETenderItem", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateETenderItemWithConsigneeLevelCalculation(ETenderItemMdl dbobject)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
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
                cmd.CommandText = "usp_update_tbl_etenderitem";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@itemrecid", dbobject.ItemRecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //for (int i = 0; i < objetenderconsignee.Length; i++)
                //{
                //    cmd.Parameters.Clear();
                //    cmd.CommandText = "usp_update_tbl_etenderconsignee";
                //    cmd.Parameters.Add(mc.getPObject("@ItemRecId", itemrecid, DbType.Int32));
                //    cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                //    cmd.Parameters.Add(mc.getPObject("@ConsigneeId", objetenderconsignee[i].ConsigneeId, DbType.Int32));
                //    cmd.Parameters.Add(mc.getPObject("@TenderQty", objetenderconsignee[i].TenderQty, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@OfferedQty", objetenderconsignee[i].OfferedQty, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@SaleTaxType", objetenderconsignee[i].SaleTaxType, DbType.String));
                //    cmd.Parameters.Add(mc.getPObject("@SaleTaxPer", objetenderconsignee[i].SaleTaxPer, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@Freight", objetenderconsignee[i].Freight, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@BasicRate", objetenderconsignee[i].BasicRate, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@UnitRate", objetenderconsignee[i].UnitRate, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@TotalAmount", objetenderconsignee[i].TotalAmount, DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@recid", objetenderconsignee[i].RecId, DbType.Int32));
                //    cmd.ExecuteNonQuery();
                //}
                trn.Commit();
                //objSqlObject.setEventLog(cmd, dbTables.tbl_etender, tenderid, "", "");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_etenderitem"))
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderItemBLL", "updateETenderItemWithConsigneeLevelCalculation", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteETenderItem(int itemrecid)
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
                cmd.CommandText = "usp_delete_tbl_etenderitem";
                cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //mc.setEventLog(cmd, dbTables.tbl_etenderitem, tenderid.ToString(), "Deleted");
                trn.Commit();
                Message = "E-Tender Item Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                if (st.ToLower().Contains("fk_tbl_etenderconsignee_tbl_etenderitem"))
                {
                    Message = "This record cannot be deleted! It has been used in Step-3.";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderItemBLL", "deleteETenderItem", st);
                }
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
        internal DataSet getETenderItemData(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_etenderitem";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ETenderItemMdl> getObjectList(int tenderid)
        {
            DataSet ds = getETenderItemData(tenderid);
            return createObjectList(ds);
        }
        //
        internal ETenderItemMdl searchETenderItem(int tenderid, int itemrecid)
        {
            DataSet ds = new DataSet();
            ETenderItemMdl objmdl = new ETenderItemMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_etenderitem";
            cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count == 0)
            {
                objmdl.ItemRecId = 0;
                objmdl.TenderId = tenderid;
                objmdl.TDRecId = 0;
                objmdl.SpcChkList = "a";//as default
                objmdl.SpcRemarks = "";
                objmdl.ItemDescOpt = "a";//as default
                objmdl.ItemDescOffered = "";
                objmdl.ItemId = 0;
                objmdl.ItemName = "";
                objmdl.FullTDQty = 0;
                objmdl.UnitOfMeasurement = "";
                objmdl.BasicRatePerUnit = 0;
                objmdl.UncDiscountPer = 0;
                objmdl.BasicPkgPer = 0;
                objmdl.EDType = "";
                objmdl.EDMaxPerApplicable = 0;
                objmdl.CessOnEDPer = 0;
                objmdl.OtherChgType = "";
                objmdl.OtherChgPerUnit = 0;
                objmdl.PriceVarReq = false;// as default
                objmdl.PriceVarClause = "";
                objmdl.Remarks = "";
            }
            else if(ds.Tables[0].Rows.Count > 0)
            {
                objmdl = createObjectList(ds)[0];
            }
            TenderDetailBLL tDetailBll = new TenderDetailBLL();
            objmdl.TendetDetailItems = tDetailBll.getTenderDetailForETenderList(objmdl.TenderId);
            objmdl.ETenderItems = getObjectList(objmdl.TenderId);
            return objmdl;
        }
        //
        internal DataSet getETenderItemInfo(int itemrecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_etenderitem_info";
            cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}