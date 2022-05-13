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
    public class PurchaseOrderBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        POrderDetailMdl ledgerObj = new POrderDetailMdl();
        POrderDetailBLL ledgerBLL = new POrderDetailBLL();
        ModifyAdviceBLL maListBLL = new ModifyAdviceBLL();
        public static PurchaseOrderBLL Instance
        {
            get { return new PurchaseOrderBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, PurchaseOrderMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@POType", dbobject.POType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RailwayId", mc.getForSqlIntString(dbobject.RailwayId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TenderId", mc.getForSqlIntString(dbobject.TenderId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@OrderStatus", dbobject.OrderStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@QuotationNo", dbobject.QuotationNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsIncOrder", dbobject.IsIncOrder, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderDesc", dbobject.TenderDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EntryType", dbobject.EntryType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PONumber", dbobject.PONumber.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CaseFileNo", dbobject.CaseFileNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PODate", dbobject.PODate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@WForPorderId", mc.getForSqlIntString(dbobject.WForPOrderId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@IsCaseClosed", dbobject.IsCaseClosed, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ClosureDate", dbobject.ClosureDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CompAddId", dbobject.CompAddId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AcCode", dbobject.AcCode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@PayingAuthId", mc.getForSqlIntString(dbobject.PayingAuthId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PaymentMode", dbobject.PaymentMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FreeAtStation", dbobject.FreeAtStation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getSqlPObject("@TermsCondition", dbobject.TermsCondition.Trim(), SqlDbType.VarChar));
            cmd.Parameters.Add(mc.getPObject("@MAReason", dbobject.MAReason.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getSqlPObject("@Remarks", dbobject.Remarks.Trim(), SqlDbType.VarChar));
            cmd.Parameters.Add(mc.getPObject("@CorrectionRequired", dbobject.CorrectionRequired, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
        }
        private List<PurchaseOrderMdl> createObjectList(DataSet ds)
        {
            List<PurchaseOrderMdl> objlist = new List<PurchaseOrderMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PurchaseOrderMdl objmdl = new PurchaseOrderMdl();
                objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                objmdl.PONumber = dr["PONumber"].ToString();
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TenderDesc = dr["TenderDesc"].ToString();
                if (dr.Table.Columns.Contains("AcCode"))
                {
                    objmdl.AcCode = Convert.ToInt32(dr["AcCode"].ToString());
                }
                if (dr.Table.Columns.Contains("PODate"))
                {
                    objmdl.PODate = Convert.ToDateTime(dr["PODate"].ToString());
                }
                if (dr.Table.Columns.Contains("POValue"))
                {
                    objmdl.POValue = Convert.ToDouble(dr["POValue"].ToString());
                }
                if (dr.Table.Columns.Contains("Qty"))
                {
                    objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                }
                if (dr.Table.Columns.Contains("PaymentMode"))
                {
                    objmdl.PaymentMode = dr["PaymentMode"].ToString();
                }
                if (dr.Table.Columns.Contains("QuotationNo"))
                {
                    objmdl.QuotationNo = dr["QuotationNo"].ToString();
                }
                if (dr.Table.Columns.Contains("FreeAtStation"))
                {
                    objmdl.FreeAtStation = dr["FreeAtStation"].ToString();
                }
                if (dr.Table.Columns.Contains("OrderStatus"))
                {
                    objmdl.OrderStatus = dr["OrderStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("OrderStatusName"))
                {
                    objmdl.OrderStatusName = dr["OrderStatusName"].ToString();
                }
                if (dr.Table.Columns.Contains("PayingAuthId"))
                {
                    objmdl.PayingAuthId = Convert.ToInt32(dr["PayingAuthId"].ToString());
                }
                if (dr.Table.Columns.Contains("PayingAuthName"))
                {
                    objmdl.PayingAuthName = dr["PayingAuthName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("MAReason"))
                {
                    objmdl.MAReason = dr["MAReason"].ToString();
                }
                if (dr.Table.Columns.Contains("Remarks"))
                {
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (dr.Table.Columns.Contains("CompAddId"))
                {
                    objmdl.CompAddId = Convert.ToInt32(dr["CompAddId"].ToString());
                }
                if (dr.Table.Columns.Contains("TermsCondition"))
                {
                    objmdl.TermsCondition = dr["TermsCondition"].ToString();
                }
                if (dr.Table.Columns.Contains("CorrectionRequired"))
                {
                    objmdl.CorrectionRequired = Convert.ToBoolean(dr["CorrectionRequired"].ToString());
                }
                if (dr.Table.Columns.Contains("AcDesc"))
                {
                    objmdl.AcDesc = dr["AcDesc"].ToString();//d
                }
                if (dr.Table.Columns.Contains("TenderNo"))
                {
                    objmdl.TenderNo = dr["TenderNo"].ToString();
                }
                if (dr.Table.Columns.Contains("RailwayId"))
                {
                    objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                }
                if (dr.Table.Columns.Contains("RailwayName"))
                {
                    objmdl.RailwayName = dr["RailwayName"].ToString();
                }
                if (dr.Table.Columns.Contains("POType"))
                {
                    objmdl.POType = dr["POType"].ToString();
                }
                if (dr.Table.Columns.Contains("POTypeName"))
                {
                    objmdl.POTypeName = dr["POTypeName"].ToString();
                }
                if (dr.Table.Columns.Contains("PmtStatus"))
                {
                    objmdl.PmtStatus = dr["PmtStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("IsIncOrder"))
                {
                    objmdl.IsIncOrder = Convert.ToBoolean(dr["IsIncOrder"].ToString());
                }
                if (dr.Table.Columns.Contains("IsPOVerified"))
                {
                    objmdl.IsPOVerified = Convert.ToBoolean(dr["IsPOVerified"].ToString());
                }
                if (dr.Table.Columns.Contains("CaseFileNo"))
                {
                    objmdl.CaseFileNo = dr["CaseFileNo"].ToString();
                }
                if (dr.Table.Columns.Contains("WForPorderId"))
                {
                    objmdl.WForPOrderId = Convert.ToInt32(dr["WForPOrderId"].ToString());
                }
                if (dr.Table.Columns.Contains("ProductDescChk"))
                {
                    objmdl.ProductDescChk = dr["ProductDescChk"].ToString();
                }
                if (dr.Table.Columns.Contains("DrgSpcChk"))
                {
                    objmdl.DrgSpcChk = dr["DrgSpcChk"].ToString();
                }
                if (dr.Table.Columns.Contains("BasicRateChk"))
                {
                    objmdl.BasicRateChk = dr["BasicRateChk"].ToString();
                }
                if (dr.Table.Columns.Contains("TaxesChk"))
                {
                    objmdl.TaxesChk = dr["TaxesChk"].ToString();
                }
                if (dr.Table.Columns.Contains("PmtTermsChk"))
                {
                    objmdl.PmtTermsChk = dr["PmtTermsChk"].ToString();
                }
                if (dr.Table.Columns.Contains("InspByChk"))
                {
                    objmdl.InspByChk = dr["InspByChk"].ToString();
                }
                if (dr.Table.Columns.Contains("ConsigneeChk"))
                {
                    objmdl.ConsigneeChk = dr["ConsigneeChk"].ToString();
                }
                if (dr.Table.Columns.Contains("DelvScheduleChk"))
                {
                    objmdl.DelvScheduleChk = dr["DelvScheduleChk"].ToString();
                }
                if (dr.Table.Columns.Contains("ConsgQtyDPChk"))
                {
                    objmdl.ConsgQtyDPChk = dr["ConsgQtyDPChk"].ToString();
                }
                if (dr.Table.Columns.Contains("DispatchModeChk"))
                {
                    objmdl.DispatchModeChk = dr["DispatchModeChk"].ToString();
                }
                if (dr.Table.Columns.Contains("BGSecurityChk"))
                {
                    objmdl.BGSecurityChk = dr["BGSecurityChk"].ToString();
                }
                if (dr.Table.Columns.Contains("AnyOtherChk"))
                {
                    objmdl.AnyOtherChk = dr["AnyOtherChk"].ToString();
                }
                if (dr.Table.Columns.Contains("EntryType"))
                {
                    objmdl.EntryType = dr["EntryType"].ToString();
                }
                if (dr.Table.Columns.Contains("IsCaseClosed"))
                {
                    objmdl.IsCaseClosed = Convert.ToBoolean(dr["IsCaseClosed"].ToString());
                }
                if (dr.Table.Columns.Contains("ClosureDate"))
                {
                    objmdl.ClosureDate = Convert.ToDateTime(dr["ClosureDate"].ToString());
                }
                if (dr.Table.Columns.Contains("ItemsQty"))
                {
                    objmdl.ItemsQty = dr["ItemsQty"].ToString();
                }
                if (dr.Table.Columns.Contains("PODateStr"))
                {
                    objmdl.PODateStr = dr["PODateStr"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(PurchaseOrderMdl dbobject)
        {
            Message = "";

            if (dbobject.OrderStatus == null)
            {
                dbobject.OrderStatus = dbobject.HdnOrderStatus;//note
            }

            if (dbobject.RailwayName == null)
            {
                dbobject.RailwayName = "";
            }
            if (dbobject.RailwayName.Length == 0)
            {
                dbobject.RailwayId = 0;
            }

            if (dbobject.AcDesc == null)
            {
                dbobject.AcDesc = "";
            }
            if (dbobject.AcDesc.Length == 0)
            {
                dbobject.AcCode = 0;
            }

            if (dbobject.PayingAuthName == null)
            {
                dbobject.PayingAuthName = "";
            }
            if (dbobject.PayingAuthName.Length == 0)
            {
                dbobject.PayingAuthId = 0;
            }

            if (dbobject.WForPONumber == null)
            {
                dbobject.WForPONumber = "";
            }
            if (dbobject.WForPONumber.Length == 0)
            {
                dbobject.WForPOrderId = 0;
            }

            if (dbobject.POType.ToLower() == "t")//railway
            {
                if (dbobject.TenderId == 0)
                {
                    Message = "Tender not selected!";
                    return false;
                }
                if (dbobject.PayingAuthId == 0)
                {
                    Message = "Paying authority not selected!";
                    return false;
                }
            }

            if (dbobject.TenderDesc == null)
            {
                dbobject.TenderDesc = "";
            }
            if (dbobject.TenderId == 0 && dbobject.TenderDesc.Length == 0)
            {
                Message = "Tender description not entered!";
                return false;
            }

            if (dbobject.PONumber.Length == 0)
            {
                Message = "PO number not entered!";
                return false;
            }
            if (mc.isValidDate(dbobject.PODate) == false)
            {
                Message = "Invalid PO date";
                return false;
            }

            if (dbobject.CaseFileNo == null)
            {
                dbobject.CaseFileNo = "";
            }
            if (dbobject.CaseFileNo.Length > 0)
            {
                if (dbobject.CaseFileNo.Length < 6)
                {
                    Message = "Invalid case file format!";
                    return false;
                }
                string l6 = dbobject.CaseFileNo.Trim();
                l6 = l6.Substring(l6.Length - 6);
                if (mc.IsValidInteger(l6.Remove(2, 1)) == false)
                {
                    Message = "Invalid case file format!";
                    return false;
                }
            }

            if (dbobject.AcCode == 0)
            {
                Message = "Order placing authority not selected";
                return false;
            }

            if (dbobject.PaymentMode == null)
            {
                dbobject.PaymentMode = "";
            }

            if (dbobject.QuotationNo == null)
            {
                dbobject.QuotationNo = "";
            }
            if (dbobject.FreeAtStation == null)
            {
                dbobject.FreeAtStation = "";
            }
            if (dbobject.TermsCondition == null)
            {
                dbobject.TermsCondition = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }

            if (dbobject.CorrectionRequired == true && dbobject.Remarks.Length == 0)
            {
                Message = "Correction required remarks not entered!";
                return false;
            }
            return true;
        }
        //
        private bool checkSetValidModelForChecksheet(PurchaseOrderMdl dbobject)
        {
            Message = "";

            if (dbobject.ProductDescChk == null)
            {
                dbobject.ProductDescChk = "";
            }

            if (dbobject.DrgSpcChk == null)
            {
                dbobject.DrgSpcChk = "";
            }

            if (dbobject.BasicRateChk == null)
            {
                dbobject.BasicRateChk = "";
            }

            if (dbobject.TaxesChk == null)
            {
                dbobject.TaxesChk = "";
            }

            if (dbobject.PmtTermsChk == null)
            {
                dbobject.PmtTermsChk = "";
            }

            if (dbobject.InspByChk == null)
            {
                dbobject.InspByChk = "";
            }

            if (dbobject.ConsigneeChk == null)
            {
                dbobject.ConsigneeChk = "";
            }

            if (dbobject.DelvScheduleChk == null)
            {
                dbobject.DelvScheduleChk = "";
            }

            if (dbobject.ConsgQtyDPChk == null)
            {
                dbobject.ConsgQtyDPChk = "";
            }

            if (dbobject.DispatchModeChk == null)
            {
                dbobject.DispatchModeChk = "";
            }

            if (dbobject.BGSecurityChk == null)
            {
                dbobject.BGSecurityChk = "";
            }

            if (dbobject.AnyOtherChk == null)
            {
                dbobject.AnyOtherChk = "";
            }
            
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal bool isItemRecordFoundInSaleLedger(int itemrecid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_itemrecord_in_sale_ledger";
            cmd.Parameters.Add(mc.getPObject("@itemrecid", itemrecid, DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        internal void GeneratePurchaseOrder(PurchaseOrderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            //rule: no to be checked
            //if (dbobject.POType.ToLower() == "w")//warranty
            //{
            //    if (isDuplicateWForPOrderId(0, dbobject.WForPorderId) == true) { return; };
            //    if (isValidItemPOQtyForWarranty(dbobject.WForPorderId, dbobject) == false) { return; };
            //}
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
                cmd.CommandText = "usp_generate_purchaseorder";
                addCommandParameters(cmd, dbobject);
                //
                cmd.Parameters.Add("@POrderId", SqlDbType.Int);
                cmd.Parameters["@POrderId"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 150);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                //
                cmd.ExecuteNonQuery();
                string RetMsg = cmd.Parameters["@RetMsg"].Value.ToString();
                if (RetMsg != "1")
                {
                    Message = RetMsg;
                    return;
                }
                //
                dbobject.POrderId = Convert.ToInt32(cmd.Parameters["@POrderId"].Value.ToString());
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, dbobject.POrderId.ToString(), "PO Generated");
                trn.Commit();
                Result = true;
                Message = "PO Generated Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseOrderBLL", "GeneratePurchaseOrder", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void UpdatePurchaseOrder(PurchaseOrderMdl dbobject)
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
                cmd.CommandText = "usp_update_purchaseorder_v2";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@POrderId", dbobject.POrderId, DbType.Int32));
                //
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 150);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                //
                cmd.ExecuteNonQuery();
                string RetMsg = cmd.Parameters["@RetMsg"].Value.ToString();
                if (RetMsg != "1")
                {
                    Message = RetMsg;
                    return;
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, dbobject.POrderId.ToString(), "PO Updated");
                trn.Commit();
                Result = true;
                Message = "PO Updated Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_purchaseorder"))
                {
                    Message = "Duplicate PO Number entry is not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("PurchaseOrderBLL", "UpdatePurchaseOrder", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void UpdatePOChecksheet(PurchaseOrderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModelForChecksheet(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_purchaseorder_checksheet";
                cmd.Parameters.Add(mc.getPObject("@ProductDescChk", dbobject.ProductDescChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@DrgSpcChk", dbobject.DrgSpcChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@BasicRateChk", dbobject.BasicRateChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@TaxesChk", dbobject.TaxesChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@PmtTermsChk", dbobject.PmtTermsChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@InspByChk", dbobject.InspByChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ConsigneeChk", dbobject.ConsigneeChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@DelvScheduleChk", dbobject.DelvScheduleChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@ConsgQtyDPChk", dbobject.ConsgQtyDPChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@DispatchModeChk", dbobject.DispatchModeChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@BGSecurityChk", dbobject.BGSecurityChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@AnyOtherChk", dbobject.AnyOtherChk.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@POrderId", dbobject.POrderId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, dbobject.POrderId.ToString(), "Checksheet Updated");
                Result = true;
                Message = "PO Checksheet Updated Successfully!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PurchaseOrderBLL", "UpdatePOChecksheet", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateCorrectionRequiredRemarks(int porderid, bool crqrd, string mareason, string remarks)
        {
            Result = false;
            PurchaseOrderMdl objpo = new PurchaseOrderMdl();
            objpo = searchPurchaseOrder(porderid);
            if (objpo.OrderStatus.ToLower() == "c" || objpo.OrderStatus.ToLower() == "a" || objpo.OrderStatus.ToLower() == "h")
            {
                Message = "Correction Required cannot be updated after PO Cancelled, is Executed by Admin or On-Hold!";
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
                cmd.CommandText = "usp_update_purchaseorder_for_correctionrequired_remarks_v2";
                cmd.Parameters.Add(mc.getPObject("@mareason", mareason, DbType.String));
                cmd.Parameters.Add(mc.getSqlPObject("@Remarks", remarks.Trim(), SqlDbType.VarChar));
                cmd.Parameters.Add(mc.getPObject("@CorrectionRequired", crqrd, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, porderid.ToString(), "Correction Required Remarks Updated");
                Result = true;
                Message = "Correction Required Updated Successfully!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PurchaseOrderBLL", "updatePurchaseOrder", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void resetPOfromHoldCancelStatus(int porderid)
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
                cmd.CommandText = "usp_reset_po_from_holdcancel_status";
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, porderid.ToString(), "Reset from Hold Cancel Status");
                trn.Commit();
                Result = true;
                Message = "Order Status Reset Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseOrderBLL", "resetPOfromHoldCancelStatus", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// to be used exclusively by admin only
        /// </summary>
        internal void updatePurchaseOrderStatusWithDetail()
        {
            Result = false;
            if (objcoockie.getUserId() != "1") { return; }//--note
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
                DataSet ds = new DataSet();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_purchase_orders_to_update_status";
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_update_dspqty_and_porder_status_with_detail";
                    cmd.Parameters.Add(mc.getPObject("@porderid", ds.Tables[0].Rows[i]["porderid"].ToString(), DbType.Int32));
                    cmd.ExecuteNonQuery();
                }
                trn.Commit();
                Result = true;
                Message = "Order Status Updated Successfully. " + ds.Tables[0].Rows.Count.ToString() + " Records affected.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseOrderBLL", "updatePurchaseOrderStatusWithDetail", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void verifyPurchaseOrder(int porderid)
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
                cmd.CommandText = "usp_verify_purchaseorder";
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, porderid.ToString(), "PO Verified");
                Result = true;
                Message = "PO Verfied!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PurchaseOrderBLL", "verifyPurchaseOrder", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updatePurchaseOrderForCaseClosure(int porderid, bool iscaseclosed, DateTime closuredate)
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
                cmd.CommandText = "usp_update_purchaseorder_for_caseclosure";
                cmd.Parameters.Add(mc.getPObject("@iscaseclosed", iscaseclosed, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@closuredate", closuredate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, porderid.ToString(), "Closure Updation");
                Result = true;
                Message = "Case Closure Updated!";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PurchaseOrderBLL", "updatePurchaseOrderForCaseClosure", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deletePurchaseOrder(int porderid)
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
                cmd.CommandText = "usp_delete_tbl_purchaseorder_v2";
                cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.String));
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 150);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                string RetMsg = cmd.Parameters["@RetMsg"].Value.ToString();
                if (RetMsg != "1")
                {
                    Message = RetMsg;
                    return;
                }
                mc.setEventLog(cmd, dbTables.tbl_purchaseorder, porderid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "PO Deleted Successfully!";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseOrderBLL", "deletePurchaseOrder", ex.Message);
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
        internal bool isValidItemPOQtyForWarranty(int porderid, PurchaseOrderMdl dbobject)
        {
            Message = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_item_wise_dispatched_qty_for_PO";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            //
            ArrayList arlIQty = new ArrayList();
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (arlIQty.Contains(dbobject.Ledgers[i].ItemId.ToString()) == false)
                {
                    arlIQty.Add(dbobject.Ledgers[i].ItemId.ToString());
                }
            }
            //
            string itm = "";
            //item must be in disp list
            bool f = false;
            for (int i = 0; i < arlIQty.Count; i++)
            {
                f = false;
                itm = arlIQty[i].ToString();
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    if (ds.Tables[0].Rows[j]["itemid"].ToString() == itm)
                    {
                        f = true;
                    }
                }
            }
            if (f == false)
            {
                Message = "Item mismatched for dispatched item!";
                return false;
            }
            //qty chk
            for (int i = 0; i < arlIQty.Count; i++)
            {
                double tlpoqty = 0;
                itm = arlIQty[i].ToString();
                //ord qty to be entered for item
                for (int j = 0; j < dbobject.Ledgers.Count; j++)
                {
                    if (dbobject.Ledgers[j].ItemId.ToString() == itm)
                    {
                        tlpoqty += dbobject.Ledgers[j].OrdQty;
                    }
                }
                //item qty which are dispatched
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    if (ds.Tables[0].Rows[j]["itemid"].ToString() == itm)
                    {
                        if (tlpoqty > Convert.ToDouble(ds.Tables[0].Rows[j]["DspQty"].ToString()))
                        {
                            Message = "Order qty for item must not be greater than dispatched qty!";
                            return false;
                        }
                    }
                }
            }
            //
            return true;
        }
        //
        internal string getPONumberById(int porderid)
        {
            PurchaseOrderMdl objpo = new PurchaseOrderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ponumber_by_id";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        internal PurchaseOrderMdl searchPurchaseOrder(int porderid)
        {
            DataSet ds = new DataSet();
            PurchaseOrderMdl objpo = new PurchaseOrderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_PurchaseOrder";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objpo = createObjectList(ds)[0];
            }
            objpo.Ledgers = ledgerBLL.getItemLedgerList(porderid);
            objpo.ModifyAdvList = maListBLL.getModifyAdviceList(porderid);
            return objpo;
        }
        //
        internal DataSet getPurchaseOrderInfo(int porderid)
        {
            DataSet ds = new DataSet();
            PurchaseOrderMdl objpo = new PurchaseOrderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_purchaseorder_info";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPurchaseOrderData(int accode, string orderstatus, int itemid)
        {
            //[100054]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_PurchaseOrder";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@orderstatus", orderstatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PurchaseOrderMdl> getObjectList(int accode, string orderstatus, int itemid)
        {
            DataSet ds = getPurchaseOrderData(accode, orderstatus, itemid);
            return createObjectList(ds);
        }
        //
        internal DataSet getPurchaseOrderDataV2(string potype, int railwayid, int groupid, int itemid, string orderstatus,int marequired, string mareason, int isverified)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_purchaseorder_v2";
            cmd.Parameters.Add(mc.getPObject("@potype", potype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@orderstatus", orderstatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@marequired", marequired, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@mareason", mareason, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@isverified", isverified, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PurchaseOrderMdl> getObjectListV2(string potype, int railwayid, int groupid, int itemid, string orderstatus, int marequired, string mareason, int isverified)
        {
            DataSet ds = getPurchaseOrderDataV2(potype, railwayid, groupid, itemid, orderstatus, marequired, mareason, isverified);
            return createObjectList(ds);
        }
        //
        //internal string getMAHistoryForPOChecksheet(int porderid)
        //{
        //    string ret = " ";
        //    DataSet ds = new DataSet();
        //    ds = maListBLL.getModifyAdviceData(porderid);
        //    string mano = "";
        //    string mafor = "";
        //    if (ds.Tables.Count > 0)
        //    {
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                mano = ds.Tables[0].Rows[i]["ModifyAdvNo"].ToString();
        //                mafor = ds.Tables[0].Rows[i]["mafor"].ToString();
        //                ret += mano + " " + mafor + "\n";
        //            }
        //        }
        //    }
        //    ret = ret.Substring(0, ret.Length - 1);
        //    return ret.Trim();
        //}
        ////
        internal string getModifyAdvNoInformation(int porderid)
        {
            DataSet dsma = new DataSet();
            dsma = maListBLL.getModifyAdviceData(porderid);
            ArrayList arlma = new ArrayList();
            string ma = "";
            for (int j = 0; j < dsma.Tables[0].Rows.Count; j++)
            {
                if (arlma.Contains(dsma.Tables[0].Rows[j]["ModifyAdvNo"].ToString()) == false)
                {
                    ma = ma + dsma.Tables[0].Rows[j]["ModifyAdvNo"].ToString() + ", ";
                    arlma.Add(dsma.Tables[0].Rows[j]["ModifyAdvNo"].ToString());
                }
            }
            if (ma.Length >= 2)
            {
                ma = ma.Substring(0, ma.Length - 2);
            }
            return ma;
        }
        //
        internal DataSet getPurchaseOrderHistory(string porderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pohistory";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPurchaseOrderHistoryDetail(int hrecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "get_pohistorydetail";
            cmd.Parameters.Add(mc.getPObject("@hrecid", hrecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getRecentPOItemInfo(int itemid)
        {
            DataSet dsret = new DataSet();
            dsret.Tables.Add();
            dsret.Tables[0].Columns.Add("slno");
            dsret.Tables[0].Columns.Add("ponumber");//p1
            dsret.Tables[0].Columns.Add("podate");//p1
            dsret.Tables[0].Columns.Add("railwayname");//p1
            dsret.Tables[0].Columns.Add("rate");//lpr-p2
            dsret.Tables[0].Columns.Add("ordqty");//p2
            dsret.Tables[0].Columns.Add("itemid");
            DataSet dspo = new DataSet();
            DataSet dsitm = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recentporderids";//[100056]/Sub1
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(dspo, cmd);
            int x = 0;
            DataRow dr = dsret.Tables[0].NewRow();
            for (int i = 0; i < dspo.Tables[0].Rows.Count; i++)
            {
                if (x == 2) { break; };
                dr = dsret.Tables[0].NewRow();
                dr["slno"] = (i + 1).ToString();
                dr["itemid"] = itemid;
                //p1
                dr["ponumber"] = dspo.Tables[0].Rows[i]["ponumber"].ToString();
                dr["podate"] = Convert.ToDateTime(dspo.Tables[0].Rows[i]["podate"].ToString()).ToShortDateString();
                dr["railwayname"] = dspo.Tables[0].Rows[i]["railwayname"].ToString();
                //p2
                dsitm = new DataSet();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_recentpoitem_lprinfo";//[100056]/Sub2
                cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@porderid", dspo.Tables[0].Rows[i]["porderid"].ToString(), DbType.Int32));
                mc.fillFromDatabase(dsitm, cmd);
                dr["rate"] = "0";
                dr["ordqty"] = "0";
                if (dsitm.Tables.Count > 0)
                {
                    if (dsitm.Tables[0].Rows.Count > 0)
                    {
                        dr["rate"] = dsitm.Tables[0].Rows[0]["rate"].ToString();
                        dr["ordqty"] = dsitm.Tables[0].Rows[0]["ordqty"].ToString();
                    }
                }
                dsret.Tables[0].Rows.Add(dr);
                x += 1;
            }
            return dsret;
        }
        //
        internal DataSet getRecentQuotedItemInfo(int itemid)
        {
            DataSet dsret = new DataSet();
            dsret.Tables.Add();
            dsret.Tables[0].Columns.Add("slno");
            dsret.Tables[0].Columns.Add("quotationno");//p1
            dsret.Tables[0].Columns.Add("quotationdate");//p1
            dsret.Tables[0].Columns.Add("railwayname");//p1
            dsret.Tables[0].Columns.Add("BasicRatePerUnit");//lqr-p2
            dsret.Tables[0].Columns.Add("FullTDQty");//p2
            dsret.Tables[0].Columns.Add("itemid");
            DataSet dspo = new DataSet();
            DataSet dsitm = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recenttenderids";//[100056]/Sub3
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(dspo, cmd);
            int x = 0;
            DataRow dr = dsret.Tables[0].NewRow();
            for (int i = 0; i < dspo.Tables[0].Rows.Count; i++)
            {
                if (x == 2) { break; };
                dr = dsret.Tables[0].NewRow();
                dr["slno"] = (i + 1).ToString();
                dr["itemid"] = itemid;
                //p1
                dr["quotationno"] = dspo.Tables[0].Rows[i]["quotationno"].ToString();
                dr["quotationdate"] = dspo.Tables[0].Rows[i]["quotationdate"].ToString();
                dr["railwayname"] = dspo.Tables[0].Rows[i]["railwayname"].ToString();
                //p2
                dsitm = new DataSet();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_recenttenderitem_lqrinfo";//[100056]/Sub4
                cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@tenderid", dspo.Tables[0].Rows[i]["tenderid"].ToString(), DbType.Int32));
                mc.fillFromDatabase(dsitm, cmd);
                dr["BasicRatePerUnit"] = "0";
                dr["FullTDQty"] = "0";
                if (dsitm.Tables.Count > 0)
                {
                    if (dsitm.Tables[0].Rows.Count > 0)
                    {
                        dr["BasicRatePerUnit"] = dsitm.Tables[0].Rows[0]["BasicRatePerUnit"].ToString();
                        dr["FullTDQty"] = dsitm.Tables[0].Rows[0]["FullTDQty"].ToString();
                    }
                }
                dsret.Tables[0].Rows.Add(dr);
                x += 1;
            }
            return dsret;
        }
        //
        internal DataSet getNonTenderPurchaseOrders()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_nontenderponumbers";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPONumbers()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ponumbers";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getAllPONumbers()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_allponumbers";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPONumbersAvailableForWarranty()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ponumbers_available_for_warranty";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getMACorrectionReportHtml(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string orderstatus, string potype, int compcode, string mareason)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_macorrection_report_html";
            cmd.Parameters.Add(mc.getPObject("@potype", potype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@orderstatus", orderstatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mareason", mareason, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal string getLprLqrDetailHtml(int itemid, int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt32(objcoockie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_lpr_lqr_info";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        #endregion
        //
    }
}