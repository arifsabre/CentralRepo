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
    public class SaleBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        SaleLedgerMdl ledgerObj = new SaleLedgerMdl();
        SaleLedgerBLL ledgerBLL = new SaleLedgerBLL();
        public static SaleBLL Instance
        {
            get { return new SaleBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, SaleMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VType", dbobject.VType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", dbobject.VNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getDateByStringDDMMYYYY(dbobject.VDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CashCredit", dbobject.CashCredit, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AcCode", mc.getForSqlIntString(dbobject.AcCode.ToString()), DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@ChInv", dbobject.ChInv, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvSeries", dbobject.InvSeries.ToLower().Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Form38No", dbobject.Form38No.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FormCNo", dbobject.FormCNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SubTotal", dbobject.SubTotal, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BEDAdvPer", dbobject.BEDAdvPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@BEDAdvAmount", dbobject.BEDAdvAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EduCessPer", dbobject.EduCessPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EduCessAmount", dbobject.EduCessAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SheCessPer", dbobject.SheCessPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SheCessAmount", dbobject.SheCessAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ExciseDutyAmount", dbobject.ExciseDutyAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VATAmount", dbobject.VATAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SATAmount", dbobject.SATAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CSTAmount", dbobject.CSTAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EntryTaxAmount", dbobject.EntryTaxAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightAmount", dbobject.FreightAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OtherCharges", dbobject.OtherCharges, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Discount", dbobject.Discount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@GRNo", dbobject.GRNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@GRDate", mc.getDateByStringDDMMYYYY(dbobject.GRDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@TrpDetail", dbobject.TrpDetail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@POrderId", mc.getForSqlIntString(dbobject.POrderId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ConsigneeId", mc.getForSqlIntString(dbobject.ConsigneeId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RemovalDateTime", dbobject.RemovalDateTime.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MADate", mc.getDateByStringDDMMYYYY(dbobject.MADate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@MANo", dbobject.MANo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TarrifHeading", dbobject.TarrifHeading.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CertOption", dbobject.CertOption.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PayingAuthId", mc.getForSqlIntString(dbobject.PayingAuthId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@B1LetterNo", dbobject.B1LetterNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@B1LetterDate", mc.getDateByStringDDMMYYYY(dbobject.B1LetterDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@B1Agent", dbobject.B1Agent.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@B2LetterNo", dbobject.B2LetterNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@B2LetterDate", mc.getDateByStringDDMMYYYY(dbobject.B2LetterDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@B2Agent", dbobject.B2Agent.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IssueDateTime", dbobject.IssueDateTime.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RemovalTimeInWords", dbobject.RemovalTimeInWords.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrfCompCode", dbobject.TrfCompCode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@IntPONumber", dbobject.IntPONumber.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IntPODate", mc.getDateByStringDDMMYYYY(dbobject.IntPODate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BillPODesc", dbobject.BillPODesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ElctRefNo", dbobject.ElctRefNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ElctRefDate", mc.getDateByStringDDMMYYYY(dbobject.ElctRefDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@RevChgTaxAmount", dbobject.RevChgTaxAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TaxableAmount", dbobject.TaxableAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TaxType", dbobject.TaxType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrpMode", dbobject.TrpMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PackingDetail", dbobject.PackingDetail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvoiceMode", dbobject.InvoiceMode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvNote", dbobject.InvNote.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RNoteInfo", dbobject.RNoteInfo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AdjAmount", dbobject.AdjAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OtherChgAcCode", dbobject.OtherChgAcCode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@OtherChgPer", dbobject.OtherChgPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RetSaleRecId", mc.getForSqlIntString(dbobject.RetSaleRecId.ToString()), DbType.Int32));
            //added by dbp: cmd.Parameters.Add(objProgramme.getPObject("@VersionNumber", "", DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@InvReferenceNo", dbobject.InvReferenceNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SupTypeCode", dbobject.SupTypeCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DocTypeCode", dbobject.DocTypeCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PrecDocNo", dbobject.PrecDocNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PrecDocDate", mc.getDateByStringDDMMYYYY(dbobject.PrecDocDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@SupAddressId", mc.getForSqlIntString(dbobject.SupAddressId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DistanceKM", dbobject.DistanceKM, DbType.Double));
        }
        //
        private List<SaleMdl> createObjectList(DataSet ds)
        {
            List<SaleMdl> objlist = new List<SaleMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SaleMdl objmdl = new SaleMdl();
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.VType = dr["VType"].ToString();
                objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                objmdl.VDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["VDate"].ToString()));
                if (dr.Table.Columns.Contains("PONumber"))
                {
                    objmdl.PONumber = dr["PONumber"].ToString();
                }
                if (dr.Table.Columns.Contains("RailwayId"))
                {
                    objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                }
                if (dr.Table.Columns.Contains("BillNo"))
                {
                    objmdl.BillNo = dr["BillNo"].ToString();
                }
                if (dr.Table.Columns.Contains("RetSaleRecId"))
                {
                    objmdl.RetSaleRecId = Convert.ToInt32(dr["RetSaleRecId"].ToString());
                }
                if (dr.Table.Columns.Contains("RetInvoiceNo"))
                {
                    objmdl.RetInvoiceNo = dr["RetInvoiceNo"].ToString();
                }
                if (dr.Table.Columns.Contains("AcCode"))
                {
                    objmdl.AcCode = Convert.ToInt32(dr["AcCode"].ToString());
                }
                if (dr.Table.Columns.Contains("Party"))
                {
                    objmdl.Party = dr["Party"].ToString();//d
                }
                if (dr.Table.Columns.Contains("ConsigneeName"))
                {
                    objmdl.ConsigneeName = dr["ConsigneeName"].ToString();
                }
                if (dr.Table.Columns.Contains("ChInv"))
                {
                    objmdl.ChInv = dr["ChInv"].ToString();
                }
                if (dr.Table.Columns.Contains("Form38No"))
                {
                    objmdl.Form38No = dr["Form38No"].ToString();
                }
                if (dr.Table.Columns.Contains("FormCNo"))
                {
                    objmdl.FormCNo = dr["FormCNo"].ToString();
                }
                if (dr.Table.Columns.Contains("SubTotal"))
                {
                    objmdl.SubTotal = Convert.ToDouble(dr["SubTotal"].ToString());
                }
                if (dr.Table.Columns.Contains("BEDAdvPer"))
                {
                    objmdl.BEDAdvPer = Convert.ToDouble(dr["BEDAdvPer"].ToString());
                }
                if (dr.Table.Columns.Contains("BEDAdvAmount"))
                {
                    objmdl.BEDAdvAmount = Convert.ToDouble(dr["BEDAdvAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("EduCessPer"))
                {
                    objmdl.EduCessPer = Convert.ToDouble(dr["EduCessPer"].ToString());
                }
                if (dr.Table.Columns.Contains("EduCessAmount"))
                {
                    objmdl.EduCessAmount = Convert.ToDouble(dr["EduCessAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("SheCessPer"))
                {
                    objmdl.SheCessPer = Convert.ToDouble(dr["SheCessPer"].ToString());
                }
                if (dr.Table.Columns.Contains("SheCessAmount"))
                {
                    objmdl.SheCessAmount = Convert.ToDouble(dr["SheCessAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("ExciseDutyAmount"))
                {
                    objmdl.ExciseDutyAmount = Convert.ToDouble(dr["ExciseDutyAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("VATAmount"))
                {
                    objmdl.VATAmount = Convert.ToDouble(dr["VATAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("SATAmount"))
                {
                    objmdl.SATAmount = Convert.ToDouble(dr["SATAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("CSTAmount"))
                {
                    objmdl.CSTAmount = Convert.ToDouble(dr["CSTAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("EntryTaxAmount"))
                {
                    objmdl.EntryTaxAmount = Convert.ToDouble(dr["EntryTaxAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("FreightAmount"))
                {
                    objmdl.FreightAmount = Convert.ToDouble(dr["FreightAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("OtherCharges"))
                {
                    objmdl.OtherCharges = Convert.ToDouble(dr["OtherCharges"].ToString());
                }
                if (dr.Table.Columns.Contains("Discount"))
                {
                    objmdl.Discount = Convert.ToDouble(dr["Discount"].ToString());
                }
                if (dr.Table.Columns.Contains("NetAmount"))
                {
                    objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("GRNo"))
                {
                    objmdl.GRNo = dr["GRNo"].ToString();
                }
                if (dr.Table.Columns.Contains("GRDate"))
                {
                    objmdl.GRDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["GRDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("TrpDetail"))
                {
                    objmdl.TrpDetail = dr["TrpDetail"].ToString();
                }
                if (dr.Table.Columns.Contains("POrderId"))
                {
                    objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                }
                if (dr.Table.Columns.Contains("ConsigneeId"))
                {
                    objmdl.ConsigneeId = Convert.ToInt32(dr["ConsigneeId"].ToString());
                }
                if (dr.Table.Columns.Contains("MADate"))
                {
                    objmdl.MADate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["MADate"].ToString()));
                }
                if (dr.Table.Columns.Contains("MANo"))
                {
                    objmdl.MANo = dr["MANo"].ToString();
                }
                if (dr.Table.Columns.Contains("TarrifHeading"))
                {
                    objmdl.TarrifHeading = dr["TarrifHeading"].ToString();
                }
                if (dr.Table.Columns.Contains("CertOption"))
                {
                    objmdl.CertOption = dr["CertOption"].ToString();
                }
                if (dr.Table.Columns.Contains("PayingAuthId"))
                {
                    objmdl.PayingAuthId = Convert.ToInt32(dr["PayingAuthId"].ToString());
                }
                if (dr.Table.Columns.Contains("PayingAuthName"))
                {
                    objmdl.PayingAuthName = dr["PayingAuthName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("B1LetterNo"))
                {
                    objmdl.B1LetterNo = dr["B1LetterNo"].ToString();
                }
                if (dr.Table.Columns.Contains("B1LetterDate"))
                {
                    objmdl.B1LetterDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["B1LetterDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("B1Agent"))
                {
                    objmdl.B1Agent = dr["B1Agent"].ToString();
                }
                if (dr.Table.Columns.Contains("B2LetterNo"))
                {
                    objmdl.B2LetterNo = dr["B2LetterNo"].ToString();
                }
                if (dr.Table.Columns.Contains("B2LetterDate"))
                {
                    objmdl.B2LetterDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["B2LetterDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("B2Agent"))
                {
                    objmdl.B2Agent = dr["B2Agent"].ToString();
                }
                if (dr.Table.Columns.Contains("IssueDateTime"))
                {
                    objmdl.IssueDateTime = dr["IssueDateTime"].ToString();
                }
                if (dr.Table.Columns.Contains("RemovalTimeInWords"))
                {
                    objmdl.RemovalTimeInWords = dr["RemovalTimeInWords"].ToString();
                }
                if (dr.Table.Columns.Contains("RemovalDateTime"))
                {
                    objmdl.RemovalDateTime = dr["RemovalDateTime"].ToString();
                }
                if (dr.Table.Columns.Contains("IsUnloaded"))
                {
                    objmdl.IsUnloaded = Convert.ToBoolean(dr["IsUnloaded"].ToString());
                }
                if (dr.Table.Columns.Contains("UnloadingDate"))
                {
                    objmdl.UnloadingDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["UnloadingDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("TrfCompCode"))
                {
                    objmdl.TrfCompCode = Convert.ToInt32(dr["TrfCompCode"].ToString());
                }
                if (dr.Table.Columns.Contains("IntPONumber"))
                {
                    objmdl.IntPONumber = dr["IntPONumber"].ToString();
                }
                if (dr.Table.Columns.Contains("IntPODate"))
                {
                    objmdl.IntPODate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["IntPODate"].ToString()));
                }
                if (dr.Table.Columns.Contains("Remarks"))
                {
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (dr.Table.Columns.Contains("BillPODesc"))
                {
                    objmdl.BillPODesc = dr["BillPODesc"].ToString();
                }
                if (dr.Table.Columns.Contains("InvSeries"))
                {
                    objmdl.InvSeries = dr["InvSeries"].ToString().ToUpper();
                }
                if (dr.Table.Columns.Contains("ElctRefNo"))
                {
                    objmdl.ElctRefNo = dr["ElctRefNo"].ToString();
                }
                if (dr.Table.Columns.Contains("ElctRefDate"))
                {
                    objmdl.ElctRefDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["ElctRefDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("RevChgTaxAmount"))
                {
                    objmdl.RevChgTaxAmount = Convert.ToDouble(dr["RevChgTaxAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("TaxableAmount"))
                {
                    objmdl.TaxableAmount = Convert.ToDouble(dr["TaxableAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("NetReceivable"))
                {
                    objmdl.NetReceivable = Convert.ToDouble(dr["NetReceivable"].ToString());
                }
                if (dr.Table.Columns.Contains("TaxType"))
                {
                    objmdl.TaxType = dr["TaxType"].ToString();
                }
                if (dr.Table.Columns.Contains("TrpMode"))
                {
                    objmdl.TrpMode = dr["TrpMode"].ToString();
                }
                if (dr.Table.Columns.Contains("PackingDetail"))
                {
                    objmdl.PackingDetail = dr["PackingDetail"].ToString();
                }
                if (dr.Table.Columns.Contains("InvoiceMode"))
                {
                    objmdl.InvoiceMode = dr["InvoiceMode"].ToString();
                }
                if (dr.Table.Columns.Contains("InvNote"))
                {
                    objmdl.InvNote = dr["InvNote"].ToString();
                }
                if (dr.Table.Columns.Contains("RCInfo"))
                {
                    objmdl.RCInfo = dr["RCInfo"].ToString();
                }
                if (dr.Table.Columns.Contains("RNoteInfo"))
                {
                    objmdl.RNoteInfo = dr["RNoteInfo"].ToString();
                }
                if (dr.Table.Columns.Contains("AdjAmount"))
                {
                    objmdl.AdjAmount = Convert.ToDouble(dr["AdjAmount"].ToString());
                }
                if (dr.Table.Columns.Contains("CashCredit"))
                {
                    objmdl.CashCredit = dr["CashCredit"].ToString();
                }
                if (dr.Table.Columns.Contains("IsRCReceived"))
                {
                    objmdl.IsRCReceived = Convert.ToBoolean(dr["IsRCReceived"].ToString());
                }
                if (dr.Table.Columns.Contains("RCRecDate"))
                {
                    objmdl.RCRecDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["RCRecDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("IsRNoteReceived"))
                {
                    objmdl.IsRNoteReceived = Convert.ToBoolean(dr["IsRNoteReceived"].ToString());
                }
                if (dr.Table.Columns.Contains("RNoteDate"))
                {
                    objmdl.RNoteDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["RNoteDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("IsBillSubmitted"))
                {
                    objmdl.IsBillSubmitted = Convert.ToBoolean(dr["IsBillSubmitted"].ToString());
                }
                if (dr.Table.Columns.Contains("BillSubmitDate"))
                {
                    objmdl.BillSubmitDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["BillSubmitDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("BillSubmitDateP2"))
                {
                    objmdl.BillSubmitDateP2 = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["BillSubmitDateP2"].ToString()));
                }
                if (dr.Table.Columns.Contains("OtherChgAcCode"))
                {
                    objmdl.OtherChgAcCode = Convert.ToInt32(dr["OtherChgAcCode"].ToString());
                }
                if (dr.Table.Columns.Contains("OtherChgPer"))
                {
                    objmdl.OtherChgPer = Convert.ToDouble(dr["OtherChgPer"].ToString());
                }
                if (dr.Table.Columns.Contains("VersionNumber"))
                {
                    objmdl.VersionNumber = dr["VersionNumber"].ToString();
                }
                if (dr.Table.Columns.Contains("SupTypeCode"))
                {
                    objmdl.SupTypeCode = dr["SupTypeCode"].ToString();
                }
                if (dr.Table.Columns.Contains("DocTypeCode"))
                {
                    objmdl.DocTypeCode = dr["DocTypeCode"].ToString();
                }
                if (dr.Table.Columns.Contains("PrecDocNo"))
                {
                    objmdl.PrecDocNo = dr["PrecDocNo"].ToString();
                }
                if (dr.Table.Columns.Contains("PrecDocDate"))
                {
                    objmdl.PrecDocDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["PrecDocDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("SupAddressId"))
                {
                    objmdl.SupAddressId = Convert.ToInt32(dr["SupAddressId"].ToString());
                }
                if (dr.Table.Columns.Contains("AddressName"))
                {
                    objmdl.AddressName = dr["AddressName"].ToString();
                }
                if (dr.Table.Columns.Contains("DistanceKM"))
                {
                    objmdl.DistanceKM = Convert.ToDouble(dr["DistanceKM"].ToString());
                }
                if (dr.Table.Columns.Contains("InvReferenceNo"))
                {
                    objmdl.InvReferenceNo = dr["InvReferenceNo"].ToString();
                }
                if (dr.Table.Columns.Contains("AckNo"))
                {
                    objmdl.AckNo = dr["AckNo"].ToString();
                }
                if (dr.Table.Columns.Contains("AckDate"))
                {
                    objmdl.AckDate = dr["AckDate"].ToString();
                }
                if (dr.Table.Columns.Contains("SekNo"))
                {
                    objmdl.SekNo = dr["SekNo"].ToString();
                }
                if (dr.Table.Columns.Contains("ItemQty"))
                {
                    objmdl.ItemQty = dr["ItemQty"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(SaleMdl dbobject)
        {
            Message = "";
            //if (mc.isValidDate(dbobject.VDate) == false)
            //{
            //    Message = "Invalid date";
            //    return false;
            //}
            if (mc.isValidDateForFinYear(objcoockie.getFinYear(), mc.getDateByString(dbobject.VDate)) == false)
            {
                Message = "Invalid date for financial year!";
                return false;
            }
            if (dbobject.IntPODate == null)
            {
                dbobject.IntPODate = dbobject.VDate;
            }
            if (dbobject.IntPONumber == null)
            {
                dbobject.IntPONumber = "";
            }
            if (dbobject.ElctRefNo == null)
            {
                dbobject.ElctRefNo = "Nil";
            }
            if (dbobject.ElctRefDate == null)
            {
                dbobject.ElctRefDate = dbobject.VDate;
            }
            if (dbobject.MADate == null)
            {
                dbobject.MADate = dbobject.VDate;
            }
            if (dbobject.GRNo == null)
            {
                dbobject.GRNo = "";
            }
            if (dbobject.B1LetterNo == null)
            {
                dbobject.B1LetterNo = "";
            }
            if (dbobject.B2LetterNo == null)
            {
                dbobject.B2LetterNo = "";
            }
            if (dbobject.GRDate == null)
            {
                dbobject.GRDate = "01/01/1900";
            }
            if (dbobject.B1LetterDate == null)
            {
                dbobject.B1LetterDate = "01/01/1900";
            }
            if (dbobject.B2LetterDate == null)
            {
                dbobject.B2LetterDate = "01/01/1900";
            }
            if (dbobject.PackingDetail == null)
            {
                dbobject.PackingDetail = "";
            }
            if (dbobject.MANo == null)
            {
                dbobject.MANo = "";
            }
            if (dbobject.TrpMode == null)
            {
                dbobject.TrpMode = "";
            }
            if (dbobject.TrpDetail == null)
            {
                dbobject.TrpDetail = "";
            }
            if (dbobject.B1Agent == null)
            {
                dbobject.B1Agent = "";
            }
            if (dbobject.B2Agent == null)
            {
                dbobject.B2Agent = "";
            }
            if (dbobject.IssueDateTime == null)
            {
                dbobject.IssueDateTime = "";
            }
            if (dbobject.RemovalDateTime == null)
            {
                dbobject.RemovalDateTime = "";
            }
            if (dbobject.RemovalTimeInWords == null)
            {
                dbobject.RemovalTimeInWords = "";
            }
            if (dbobject.ChInv == "t")
            {
                //transfer
                if (dbobject.TrfCompCode == Convert.ToInt32(objcoockie.getCompCode()))
                {
                    Message = "Transfer to same company is not alloewd!";
                    return false;
                }
            }
            else
            {
                dbobject.TrfCompCode = Convert.ToInt32(objcoockie.getCompCode());
            }
            if (dbobject.InvNote == null)
            {
                dbobject.InvNote = "";
            }
            if (dbobject.ChInv == "m" && dbobject.InvNote.Length == 0)
            {
                dbobject.InvNote = "STOCK TRANSFER, GST NOT APPLICABLE.";
            }
            if (dbobject.TarrifHeading == null)
            {
                dbobject.TarrifHeading = "";
            }
            if (dbobject.Form38No == null)
            {
                dbobject.Form38No = "";
            }
            if (dbobject.FormCNo == null)
            {
                dbobject.FormCNo = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.BillPODesc == null)
            {
                dbobject.BillPODesc = "";
            }
            if (dbobject.RNoteInfo == null)
            {
                dbobject.RNoteInfo = "";
            }

            //versionno, supaddressid
            if (dbobject.InvReferenceNo == null)
            {
                dbobject.InvReferenceNo = "";
            }
            if (dbobject.SupTypeCode == null)
            {
                dbobject.SupTypeCode = "";
            }
            if (dbobject.DocTypeCode == null)
            {
                dbobject.DocTypeCode = "";
            }
            if (dbobject.PrecDocNo == null)
            {
                dbobject.PrecDocNo = "";
            }
            if (dbobject.PrecDocDate == null)
            {
                dbobject.PrecDocDate = dbobject.VDate;
            }

            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].InspCertificate == null)
                {
                    dbobject.Ledgers[i].InspCertificate = "";
                }
                if (dbobject.Ledgers[i].DispatchMemo == null)
                {
                    dbobject.Ledgers[i].DispatchMemo = "";
                }
                if (dbobject.Ledgers[i].Remarks == null)
                {
                    dbobject.Ledgers[i].Remarks = "";
                }
                if (dbobject.Ledgers[i].NoOfPckg == null)
                {
                    dbobject.Ledgers[i].NoOfPckg = "";
                }
            }
            return true;
        }
        //
        private void updateDispatchQtyAndOrderExecution(SqlCommand cmd, int porderid)
        {
            if (porderid == 0) { return; };
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_dspqty_and_porder_status_with_detail";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        private string getPOrderIdFromSale(SqlCommand cmd, int salerecid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getporderidforsale";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            return mc.getFromDatabase(cmd, cmd.Connection);
        }
        //
        internal string getNewVNo(string invseries, string invoicemode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_new_vno_for_sale";
            cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicemode", invoicemode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            return mc.getFromDatabase(cmd);
        }
        private DataSet getVTypeVNoByRecId(int purchaseid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_vtypevno_for_purchase";
            cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
        #region dml objects
        //
        private void generateBill(SqlCommand cmd, int salerecid, DateTime vdate)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_dispatchinfo_with_bill_creation_p1";
            cmd.Parameters.Add(mc.getPObject("@B1LetterNo", salerecid, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@B1LetterDate", vdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@B1Agent", "Self", DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billper", "100", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@billdate", DateTime.Now.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        private string getSaleRecId(SqlCommand cmd, string invseries, string invoicemode, string vno)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getsalerecid";
            cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicemode", invoicemode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            return mc.getFromDatabase(cmd, cmd.Connection);
        }
        //
        internal string getSaleRecId(string invseries, string invoicemode, string vno)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getsalerecid";
            cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicemode", invoicemode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //
        internal bool isInvoiceBilled(int salerecid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_isinvoice_billed";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        internal bool isVNoFound(string invseries, string invoicemode, string vno)
        {
            bool res = false;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_vno";
            cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicemode", invoicemode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry not allowed!";
                res = true;
            }
            return res;
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="dbobject"></param>
        internal void generateDraftProformaInvoice(SaleMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            string logdesc = "";
            if (dbobject.InvSeries.ToLower() == "d")
            {
                logdesc = "Draft Invoice Inserted";
            }
            else if (dbobject.InvSeries.ToLower() == "p")
            {
                logdesc = "Proforma Invoice Inserted";
            }
            else
            {
                Message = "Invalid series!";
                return;
            }
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
                cmd.CommandText = "usp_insert_tbl_sale";//1
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.SaleRecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_sale, "salerecid"));
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (Convert.ToDouble(dbobject.Ledgers[i].Qty) != 0)
                    {
                        ledgerBLL.saveItemLedger(cmd, dbobject.SaleRecId, dbobject.Ledgers[i]);
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), logdesc);
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "generateDraftProformaInvoice", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="salerecid"></param>
        /// <param name="dbobject"></param>
        internal void convertDraftProformaToMainInvoice(int salerecid, SaleMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            string logdesc = "";
            if (dbobject.InvSeries.ToLower() == "d")
            {
                logdesc = "Draft to Main Invoice Inserted";
            }
            else if (dbobject.InvSeries.ToLower() == "p")
            {
                logdesc = "Proforma to Main Invoice Inserted";
            }
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
                dbobject.SaleRecId = salerecid;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_convert_draft_or_proforma_to_main_invoice";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (Convert.ToDouble(dbobject.Ledgers[i].Qty) != 0)
                    {
                        ledgerBLL.updateItemLedger(cmd, salerecid, dbobject.Ledgers[i]);
                    }
                }
                updateDispatchQtyAndOrderExecution(cmd, dbobject.POrderId);
                if (!(dbobject.VType.ToLower() == "si" || dbobject.VType.ToLower() == "gi"))
                {
                    generateBill(cmd, salerecid, mc.getDateByStringDDMMYYYY(dbobject.VDate));
                }
                dbobject.SaleRecId = salerecid;//note
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), logdesc);
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "convertDraftProformaToMainInvoice", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void generateMainInvoice(SaleMdl dbobject)
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
                dbobject.InvSeries = "m";
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_sale";//2
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.SaleRecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_sale, "salerecid"));
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (Convert.ToDouble(dbobject.Ledgers[i].Qty) != 0)
                    {
                        ledgerBLL.saveItemLedger(cmd, dbobject.SaleRecId, dbobject.Ledgers[i]);
                    }
                }
                updateDispatchQtyAndOrderExecution(cmd, dbobject.POrderId);
                if(dbobject.ChInv.ToLower() != "x")//additional check differed from erp.v1
                {
                    if (!(dbobject.VType.ToLower() == "si" || dbobject.VType.ToLower() == "gi"))
                    {
                        generateBill(cmd, dbobject.SaleRecId, mc.getDateByStringDDMMYYYY(dbobject.VDate));
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "insertSaleEntry", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateSaleEntry(SaleMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isInvoiceBilled(dbobject.SaleRecId) == true)
            {
                Message = "Bill has been generated for this invoice, so it cannot be updated!";
                return;
            }
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
                //VoucherEntryMgt.VoucherBLL.Instance.deleteVoucher(cmd, vtype, vno);
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_valid_to_delete_sale";//note
                cmd.Parameters.Add(mc.getPObject("@salerecid", dbobject.SaleRecId, DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, conn)) == false)
                {
                    Message = "Bill receipt has been found for this VNo, so it cannot be updated!";
                    return;
                }
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_sale";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@salerecid", dbobject.SaleRecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    if (Convert.ToDouble(dbobject.Ledgers[i].Qty) != 0)
                    {
                        ledgerBLL.updateItemLedger(cmd, dbobject.SaleRecId, dbobject.Ledgers[i]);
                    }
                }
                string logdesc = "";
                if (dbobject.InvSeries.ToLower() == "d")
                {
                    logdesc = "Draft Invoice Updated";
                }
                else if (dbobject.InvSeries.ToLower() == "p")
                {
                    logdesc = "Proforma Invoice Updated";
                }
                else if (dbobject.InvSeries.ToLower() == "m")
                {
                    updateDispatchQtyAndOrderExecution(cmd, dbobject.POrderId);
                    if (dbobject.ChInv.ToLower() != "x")//additional check differed from erp.v1
                    {
                        if (!(dbobject.VType.ToLower() == "si" || dbobject.VType.ToLower() == "gi") && dbobject.InvoiceMode != "slr")
                        {
                            generateBill(cmd, dbobject.SaleRecId, mc.getDateByStringDDMMYYYY(dbobject.VDate));
                        }
                    }
                    logdesc = "Invoice Updated";
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), logdesc);
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "updateSaleEntry", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// admin access only
        /// </summary>
        internal void changeVNoForSale(string newvno, int salerecid)
        {
            Result = false;
            if (objcoockie.getLoginType() > 0)//not admin
            {
                Message = "Access Denied!";
                return;
            }
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
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_valid_to_change_sale_vno";
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@newvno", newvno, DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, conn)) == false)
                {
                    Message = "Bill receipt has been found for the existing VNo, so it cannot be changed!";
                    return;
                }
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_change_vno_for_sale";//with voucher/billos management
                cmd.Parameters.Add(mc.getPObject("@newvno", newvno, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Invoice No Changed");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "changeVNoForSale", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateMANo(int salerecid, string mano)
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
                cmd.CommandText = "usp_update_mano_for_sale";
                cmd.Parameters.Add(mc.getPObject("@mano", mano, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "MA No Updated");
                Result = true;
                Message = "MA No Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SaleBLL", "updateMANo", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="salerecid"></param>
        /// <param name="isunloaded"></param>
        /// <param name="unloadingdate"></param>
        /// <param name="dbobject"></param>
        internal void updateSaleForUnloading(int salerecid, bool isunloaded, DateTime unloadingdate, SaleMdl dbobject)
        {
            Result = false;
            //if (isInvoiceBilled(vtype, vno) == true)
            //{
            //    Message = "Bill has been generated for this invoice, so it cannot be updated!";
            //    return;
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
                cmd.CommandText = "usp_update_tbl_sale_for_unloading";
                cmd.Parameters.Add(mc.getPObject("@IsUnloaded", isunloaded, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@UnloadingDate", unloadingdate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.updateLedgerForDelvQtyNdCertificates(cmd, dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Updated for Unloading");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "updateSaleForUnloading", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateSaleForReceiptedChallan(int salerecid, string rcinfo)
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
                cmd.CommandText = "usp_update_tbl_sale_for_receiptedchallan";
                cmd.Parameters.Add(mc.getPObject("@rcinfo", rcinfo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Updated for Receipted Challan");
                Result = true;
                Message = "RC Information Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SaleBLL", "updateSaleForReceiptedChallan", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateSaleForRNoteInfo(int salerecid, string rnoteinfo)
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
                cmd.CommandText = "usp_update_tbl_sale_for_rnote";
                cmd.Parameters.Add(mc.getPObject("@rnoteinfo", rnoteinfo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Updated for Receipt Note");
                Result = true;
                Message = "R-Note Information Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SaleBLL", "updateSaleForRNoteInfo", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="salerecid"></param>
        /// <param name="certoption"></param>
        /// <param name="dbobject"></param>
        internal void updateSaleForCertificates(int salerecid, string certoption, SaleMdl dbobject)
        {
            Result = false;
            if (isInvoiceBilled(salerecid) == true)
            {
                Message = "Bill has been generated for this invoice, so it cannot be updated!";
                return;
            }
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
                cmd.CommandText = "usp_update_tbl_sale_for_certificates";
                cmd.Parameters.Add(mc.getPObject("@CertOption", certoption, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.updateLedgerForDelvQtyNdCertificates(cmd, dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Updated for Certificates");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "updateSaleForCertificates", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="arlslrecid"></param>
        /// <param name="billper"></param>
        /// <param name="ltrno"></param>
        /// <param name="ltrdate"></param>
        /// <param name="billdate"></param>
        /// <param name="agent"></param>
        /// <param name="replacep1"></param>
        internal void updateDispatchInfoWithBillCreation(ArrayList arlslrecid, string billper, string ltrno, DateTime ltrdate, DateTime billdate, string agent, bool replacep1)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                if (billper == "100" || billper == "98" || billper == "95" || billper == "90" || billper == "80" || billper == "60")
                {
                    for (int i = 0; i < arlslrecid.Count; i++)
                    {
                        ds = new DataSet();
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_get_billp1info";
                        cmd.Parameters.Add(mc.getPObject("@salerecid", arlslrecid[i].ToString(), DbType.Int32));
                        mc.fillFromDatabase(ds, cmd, cmd.Connection);
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (replacep1 == false)
                                {
                                    Message = "Part 1 bill has been already generated for this invoice! If you want to re-generate, please check replace option.";
                                    return;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < arlslrecid.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_dispatchinfo_with_bill_creation_p1";
                        cmd.Parameters.Add(mc.getPObject("@B1LetterNo", ltrno, DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@B1LetterDate", ltrdate.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@B1Agent", agent, DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@billper", billper, DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@billdate", DateTime.Now.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@salerecid", arlslrecid[i].ToString(), DbType.Int32));
                        cmd.ExecuteNonQuery();
                        mc.setEventLog(cmd, dbTables.tbl_sale, arlslrecid[i].ToString(), "Bill Part 1");
                    }
                }
                else if (billper == "2" || billper == "5" || billper == "10" || billper == "20" || billper == "40")
                {
                    for (int i = 0; i < arlslrecid.Count; i++)
                    {
                        ds = new DataSet();
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_get_billp1info";
                        cmd.Parameters.Add(mc.getPObject("@salerecid", arlslrecid[i].ToString(), DbType.Int32));
                        mc.fillFromDatabase(ds, cmd, cmd.Connection);
                        if (ds.Tables.Count == 0)
                        {
                            Message = "Bill part 1 not found for Invoice no!";
                            return;
                        }
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            Message = "Bill part 1 not found for Invoice no!";
                            return;
                        }
                        if (Convert.ToDouble(ds.Tables[0].Rows[0]["BillPerP1"].ToString()) + Convert.ToDouble(billper) != 100)
                        {
                            Message = "Invalid Bill part 2 generation for invoice no!";
                            return;
                        }
                    }
                    for (int i = 0; i < arlslrecid.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_update_dispatchinfo_with_bill_creation_p2";
                        cmd.Parameters.Add(mc.getPObject("@B2LetterNo", ltrno, DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@B2LetterDate", ltrdate.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@billdate", billdate.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@B2Agent", agent, DbType.String));
                        cmd.Parameters.Add(mc.getPObject("@billper", billper, DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@salerecid", arlslrecid[i].ToString(), DbType.Int32));
                        cmd.ExecuteNonQuery();
                        mc.setEventLog(cmd, dbTables.tbl_sale, arlslrecid[i].ToString(), "Bill Part 2");
                    }
                }
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "updateDispatchInfoWithBillCreation", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="arlslrecid"></param>
        /// <param name="billpodesc"></param>
        internal void updateBillPODescription(ArrayList arlslrecid, string billpodesc)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                for (int i = 0; i < arlslrecid.Count; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_update_dispatchinfo_podescription";
                    cmd.Parameters.Add(mc.getPObject("@billpodesc", billpodesc, DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@salerecid", arlslrecid[i].ToString(), DbType.Int32));
                    cmd.ExecuteNonQuery();
                }
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "updateBillPODescription", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        /// <summary>
        /// used in erp.v1 only
        /// </summary>
        /// <param name="salerecid"></param>
        /// <returns></returns>
        internal DataSet getDispatchInfoPODescription(string salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_dispatchinfo_podescription";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal void deleteLedgerByLRecId(int lrecid)
        {
            Result = false;
            //if (isInvoiceBilled(salerecid) == true)
            //{
            //    Message = "Bill has been generated for this invoice, so it cannot be deleted!";
            //    return;
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
                mc.setEventLog(cmd, dbTables.tbl_ledger, lrecid.ToString(), "item deletion");
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_ledger_by_lrecid";//updates status & dsp qty also
                cmd.Parameters.Add(mc.getPObject("@lrecid", lrecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "Item Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "deleteLedgerByLRecId", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteSaleEntry(int salerecid)
        {
            Result = false;
            if (isInvoiceBilled(salerecid) == true)
            {
                Message = "Bill has been generated for this invoice, so it cannot be deleted!";
                return;
            }
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
                cmd.CommandTimeout = 0;//note: long running procedure
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_valid_to_delete_sale";
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, conn)) == false)
                {
                    Message = "Bill receipt has been found for the existing VNo, so it cannot be deleted!";
                    return;
                }
                //
                int porderid = Convert.ToInt32(getPOrderIdFromSale(cmd, salerecid));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_sale";
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                updateDispatchQtyAndOrderExecution(cmd, porderid);
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Sale Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("fk_tbl_receipt_tbl_sale"))
                {
                    Message = "This entry has been used in receipt, So it cannot be deleted!";
                }
                else
                {
                    Message = mc.setErrorLog("SaleBLL", "deleteSaleEntry", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteDraftProformaSaleEntry(string invseries, int salerecid)
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
                cmd.CommandText = "usp_delete_draft_proforma_sale";
                cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                string logdesc = "Draft Deleted";
                if (invseries.ToLower() == "p")
                {
                    logdesc = "Proforma Deleted";
                }
                mc.setEventLog(cmd, dbTables.tbl_sale, salerecid.ToString(), logdesc);
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("SaleBLL", "deleteDraftProformaSaleEntry", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateInvReferenceNo(SaleMdl dbobject)
        {
            Result = false;
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_invoice_referenceno_detail";
                cmd.Parameters.Add(mc.getPObject("@InvReferenceNo", dbobject.InvReferenceNo.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@AckNo", dbobject.AckNo.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@SekNo", dbobject.SekNo.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@AckDate", dbobject.AckDate.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@salerecid", dbobject.SaleRecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), "IRN Updated");
                Result = true;
                Message = "IRN Updated Successfully.";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("SaleBLL", "updateInvReferenceNo", ex.Message);
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
        //not in use
        //internal DataSet getSaleData(DateTime dtfrom, DateTime dtto)
        //{
        //    DataSet ds = new DataSet();
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    cmd.CommandText = "usp_get_tbl_sale";
        //    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
        //    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
        //    cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
        //    cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
        //    mc.fillFromDatabase(ds, cmd);
        //    return ds;
        //}
        //
        internal DataSet getInvoiceNoList(int ccode = 0, string finyr = "")
        {
            if (ccode == 0) { ccode = Convert.ToInt32(objcoockie.getCompCode()); };
            if (finyr.Length == 0) { finyr = objcoockie.getFinYear(); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_invoiceno_list";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getOtherChargesAccountData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_account_list_for_invoice_othercharges";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ItemGroupMdl> getOtherChargesAccountList()
        {
            DataSet ds = getOtherChargesAccountData();
            List<ItemGroupMdl> objlist = new List<ItemGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ItemGroupMdl objmdl = new ItemGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["accode"].ToString());
                objmdl.GroupName = dr["acdesc"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal SaleMdl searchObject(int salerecid)
        {
            DataSet ds = new DataSet();
            SaleMdl dbobject = new SaleMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_sale";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getItemLedgerList(salerecid);
            return dbobject;
        }
        //
        internal DataSet getObjectData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_sale";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleMdl> getObjectList(string dtfrom, string dtto)
        {
            DataSet ds = getObjectData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet getObjectDataV2(string dtfrom, string dtto, string invoicemode, string potype, int railwayid, string unloadingst, string rcst, string billst, string pmtst, string rnotest)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_display_tbl_sale";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicemode", invoicemode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@potype", potype.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@unloadingst", unloadingst.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rcst", rcst.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billst", billst.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@pmtst", pmtst.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rnotest", rnotest.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleMdl> getObjectListV2(string dtfrom, string dtto, string invoicemode, string potype, int railwayid, string unloadingst, string rcst, string billst, string pmtst, string rnotest)
        {
            DataSet ds = getObjectDataV2(dtfrom, dtto, invoicemode, potype, railwayid, unloadingst, rcst, billst, pmtst, rnotest);
            return createObjectList(ds);
        }
        //
        internal DataSet getDraftProformaRecord(string dtfrom, string dtto, string invseries)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_draft_proforma_sale";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invseries", invseries, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleMdl> getDraftProformaList(string dtfrom, string dtto, string invseries)
        {
            DataSet ds = getDraftProformaRecord(dtfrom, dtto, invseries);
            return createObjectList(ds);
        }
        //
        internal DataSet getSaleReturnData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_return_list";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleMdl> getSaleReturnList(string dtfrom, string dtto)
        {
            DataSet ds = getSaleReturnData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet getConsigneeListData(int railwayid = 0, string rtype = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_fill_consignee_search_list_by_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_fill_Consignee_search_list";
            }
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getInvoiceNoListToReturn()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_invoiceno_to_return";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        //test chart
        internal DataSet getChartData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_chart_data";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetGuaranteeWarrantyReportData(int salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_guarantee_warranty_report";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetWarrantyClaimReportData(int salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_warranty_claim_report";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetConsigneeDetail(int consigneeid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_Consignee";
            cmd.Parameters.Add(mc.getPObject("@consigneeid", consigneeid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}