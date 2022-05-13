
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
    public class POrderDetailBLL : DbContext
    {
        //
        //internal DbSet<SaleLedgerMdl> StockLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static POrderDetailBLL Instance
        {
            get { return new POrderDetailBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int porderid, POrderDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@POrderId", porderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemName", dbobject.ItemName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ConsigneeId", dbobject.ConsigneeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@OrdQty", dbobject.OrdQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DspQty", dbobject.DspQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Amount", dbobject.Amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Discount", dbobject.Discount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExciseDutyPer", dbobject.ExciseDutyPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExciseDutyAmount", dbobject.ExciseDutyAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VATPer", dbobject.VATPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VATAmount", dbobject.VATAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SATPer", dbobject.SATPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SATAmount", dbobject.SATAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CSTPer", dbobject.CSTPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CSTAmount", dbobject.CSTAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EntryTaxPer", dbobject.EntryTaxPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EntryTaxAmount", dbobject.EntryTaxAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightRate", dbobject.FreightRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightAmount", dbobject.FreightAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BillingUnit", dbobject.BillingUnit.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BillPO", dbobject.BillPO.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InspClause", dbobject.InspClause.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvDate", dbobject.DelvDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@TransitDepot", dbobject.TransitDepot.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ModeOfDisp", dbobject.ModeOfDisp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvTerms", dbobject.DelvTerms.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DlvStatus", dbobject.DlvStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsVerified", dbobject.IsVerified, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@WayBill", dbobject.WayBill.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OCE", dbobject.OCE.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VerbalCommitment", dbobject.VerbalCommitment.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelayReason", dbobject.DelayReason.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ActualDP", dbobject.ActualDP.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@IsNonGst", dbobject.IsNonGst, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsInclGst", dbobject.IsInclGst, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UnitRate", dbobject.UnitRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ItemSlNo", dbobject.ItemSlNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CommDate", dbobject.CommDate.ToShortDateString(), DbType.DateTime));
        }
        //
        internal List<POrderDetailMdl> createObjectList(DataSet ds)
        {
            List<POrderDetailMdl> ledgers = new List<POrderDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                POrderDetailMdl objmdl = new POrderDetailMdl();
                objmdl.ItemRecId = Convert.ToInt32(dr["ItemRecId"].ToString());
                objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.ItemName = dr["ItemName"].ToString();
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.ConsigneeId = Convert.ToInt32(dr["ConsigneeId"].ToString());
                objmdl.ConsigneeName = dr["ConsigneeName"].ToString();//d
                objmdl.OrdQty = Convert.ToDouble(dr["OrdQty"].ToString());
                objmdl.DspQty = Convert.ToDouble(dr["DspQty"].ToString());
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
                objmdl.Discount = Convert.ToDouble(dr["Discount"].ToString());
                objmdl.ExciseDutyPer = Convert.ToDouble(dr["ExciseDutyPer"].ToString());
                objmdl.ExciseDutyAmount = Convert.ToDouble(dr["ExciseDutyAmount"].ToString());
                objmdl.VATPer = Convert.ToDouble(dr["VATPer"].ToString());
                objmdl.VATAmount = Convert.ToDouble(dr["VATAmount"].ToString());
                objmdl.SATPer = Convert.ToDouble(dr["SATPer"].ToString());
                objmdl.SATAmount = Convert.ToDouble(dr["SATAmount"].ToString());
                objmdl.CSTPer = Convert.ToDouble(dr["CSTPer"].ToString());
                objmdl.CSTAmount = Convert.ToDouble(dr["CSTAmount"].ToString());
                objmdl.EntryTaxPer = Convert.ToDouble(dr["EntryTaxPer"].ToString());
                objmdl.EntryTaxAmount = Convert.ToDouble(dr["EntryTaxAmount"].ToString());
                objmdl.FreightRate = Convert.ToDouble(dr["FreightRate"].ToString());
                objmdl.FreightAmount = Convert.ToDouble(dr["FreightAmount"].ToString());
                objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                objmdl.BillingUnit = dr["BillingUnit"].ToString();
                objmdl.BillPO = dr["BillPO"].ToString();
                objmdl.InspClause = dr["InspClause"].ToString();
                objmdl.DelvDate = Convert.ToDateTime(dr["DelvDate"].ToString());
                objmdl.TransitDepot = dr["TransitDepot"].ToString();
                objmdl.ModeOfDisp = dr["ModeOfDisp"].ToString();
                objmdl.DelvTerms = dr["DelvTerms"].ToString();
                objmdl.DlvStatus = dr["DlvStatus"].ToString();
                objmdl.DlvStatusName = dr["DlvStatusName"].ToString();//d
                objmdl.IsVerified = Convert.ToBoolean(dr["IsVerified"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.WayBill = dr["WayBill"].ToString();
                objmdl.OCE = dr["OCE"].ToString();
                objmdl.VerbalCommitment = dr["VerbalCommitment"].ToString();
                objmdl.DelayReason = dr["DelayReason"].ToString();
                objmdl.ActualDP = Convert.ToDateTime(dr["ActualDP"].ToString());
                objmdl.IsNonGst = Convert.ToBoolean(dr["IsNonGst"].ToString());
                objmdl.IsInclGst = Convert.ToBoolean(dr["IsInclGst"].ToString());
                objmdl.UnitRate = Convert.ToDouble(dr["UnitRate"].ToString());
                objmdl.ItemSlNo = Convert.ToInt32(dr["ItemSlNo"].ToString());
                objmdl.CommDate = Convert.ToDateTime(dr["CommDate"].ToString());
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        private bool checkSetValidModel(POrderDetailMdl dbobject)
        {
            Message = "";
            if (dbobject.POrderId == 0)
            {
                Message = "Invalid attempt!";
                return false;
            }
            if (dbobject.ItemId == 0)
            {
                Message = "Item Code not selected!";
                return false;
            }
            if (dbobject.ConsigneeId == 0)
            {
                Message = "Consignee not selected!";
                return false;
            }

            if (dbobject.UnitName.Contains("/") == false)
            {
                dbobject.BillingUnit = dbobject.UnitName;
            }
            else //it is multi-unit/same bom type item
            {
                ArrayList arl = new ArrayList();
                arl.AddRange(dbobject.UnitName.Split('/'));
                if (arl.Contains(dbobject.BillingUnit) == false)
                {
                    Message = "Invalid billing unit!";
                    return false;
                }
            }
            if (dbobject.VATPer + dbobject.SATPer > 0 && dbobject.CSTPer > 0)
            {
                Message = "Only one of SGST + CGST or IGST is applicable!";
                return false;
            }

            if (mc.isValidDate(dbobject.CommDate) == false)
            {
                Message = "Invalid commencement date!";
                return false;
            }
            if (mc.isValidDate(dbobject.DelvDate) == false)
            {
                Message = "Invalid delivery date!";
                return false;
            }
            DateTime dtm = new DateTime(1, 1, 1);
            if (dbobject.ActualDP == null || dbobject.ActualDP == dtm)
            {
                dbobject.ActualDP = dbobject.DelvDate;
            }
            
            if (dbobject.TransitDepot == null)
            {
                dbobject.TransitDepot = "";
            }
            if (dbobject.BillPO == null)
            {
                dbobject.BillPO = "";
            }
            if (dbobject.InspClause == null)
            {
                dbobject.InspClause = "";
            }
            if (dbobject.ModeOfDisp == null)
            {
                dbobject.ModeOfDisp = "";
            }
            if (dbobject.DelvTerms == null)
            {
                dbobject.DelvTerms = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.WayBill == null)
            {
                dbobject.WayBill = "";
            }
            if (dbobject.OCE == null)
            {
                dbobject.OCE = "";
            }
            if (dbobject.VerbalCommitment == null)
            {
                dbobject.VerbalCommitment = "";
            }
            if (dbobject.DelayReason == null)
            {
                dbobject.DelayReason = "";
            }

            return true;
        }
        #endregion
        //
        #region dml objects
        //
        internal void InsertPurchaseOrderItem(POrderDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_insert_tbl_porderdetail_v2";
                addCommandParameters(cmd, dbobject.POrderId, dbobject);
                cmd.Parameters.Add("@ItemRecId", SqlDbType.Int);
                cmd.Parameters["@ItemRecId"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int itemrecid = Convert.ToInt32(cmd.Parameters["@ItemRecId"].Value.ToString());
                mc.setEventLog(cmd, dbTables.tbl_porderdetail, itemrecid.ToString(), "PO Item Inserted");
                trn.Commit();
                Result = true;
                Message = "PO Item Saved Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_porderdetail") == true)
                {
                    Message = "Duplicate item serial number entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("POrderDetailBLL", "InsertPurchaseOrderItem", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void UpdatePurchaseOrderItem(POrderDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_porderdetail_v2";
                addCommandParameters(cmd, dbobject.POrderId, dbobject);
                cmd.Parameters.Add(mc.getPObject("@ItemRecId", dbobject.ItemRecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_porderdetail, dbobject.ItemRecId.ToString(), "PO Item Updated");
                trn.Commit();
                Result = true;
                Message = "PO Item Updated Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_porderdetail") == true)
                {
                    Message = "Duplicate item serial number entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("POrderDetailBLL", "UpdatePurchaseOrderItem", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updatePOrderDetailDelayReason(string delayreason, int itemrecid)
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
                cmd.CommandText = "usp_updateporderdetaildelayreason";
                cmd.Parameters.Add(mc.getPObject("@delayreason", delayreason, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_porderdetail, itemrecid.ToString(), "updation for delay reason");
                Result = true;
                Message = "Delay reason updated successfully!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("POrderDetailBLL", "updatePOrderDetailDelayReason", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void DeletePOItem(int itemrecid)
        {
            Result = false;
            Message = "";
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
                cmd.CommandText = "usp_delete_porderdetail_by_itemrecid_v2";
                cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 150);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                string msg = cmd.Parameters["@RetMsg"].Value.ToString();
                if (msg != "1")
                {
                    Message = msg;
                    return;
                }
                mc.setEventLog(cmd, dbTables.tbl_porderdetail, itemrecid.ToString(), "PO Item Deleted");
                trn.Commit();
                Result = true;
                Message = "Delay reason updated successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("POrderDetailBLL", "DeletePOItem", ex.Message);
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
        internal POrderDetailMdl searchPurchaseOrderItem(int itemrecid)
        {
            DataSet ds = new DataSet();
            POrderDetailMdl objmdl = new POrderDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_porderdetail";
            cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objmdl = createObjectList(ds)[0];
            }
            return objmdl;
        }
        //
        internal DataSet getPOrderDetailData(int porderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_porderdetail_v2";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<POrderDetailMdl> getItemLedgerList(int porderid)
        {
            DataSet ds = getPOrderDetailData(porderid);
            return createObjectList(ds);
        }
        //
        internal DataSet getItemRecordsForSale(int porderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getitemrecordsforsale";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleLedgerMdl> getItemLedgerListForSale(int porderid)
        {
            SaleLedgerBLL sLgrBll = new SaleLedgerBLL();
            DataSet ds = getItemRecordsForSale(porderid);
            return sLgrBll.createObjectList(ds);
        }
        //
        #endregion
        //
    }
}