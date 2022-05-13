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
    public class EInvoiceSaleBLL : DbContext
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
        private List<EInvoiceSaleMdl> createObjectList(DataSet ds)
        {
            List<EInvoiceSaleMdl> objlist = new List<EInvoiceSaleMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EInvoiceSaleMdl objmdl = new EInvoiceSaleMdl();
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.VType = dr["VType"].ToString();
                objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                objmdl.VDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["VDate"].ToString()));
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
                    objmdl.InvSeries = dr["InvSeries"].ToString();
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
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getEInvoiceSaleData(int salerecid)
        {
            DataSet ds = new DataSet();
            DataTable dtSL = new DataTable();
            EInvoiceSaleMdl dbobject = new EInvoiceSaleMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_invoice_report_header";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(dtSL, cmd);
            ds.Tables.Add(dtSL);
            ds.Tables.Add(ledgerBLL.getItemLedgerDataTable(salerecid));
            return ds;
        }
        internal EInvoiceSaleMdl getEInvoiceSaleObject(int salerecid)
        {
            DataSet ds = new DataSet();
            EInvoiceSaleMdl dbobject = new EInvoiceSaleMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_invoice_report_header";
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
        internal DataSet getEInvoiceForExcel(int salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_invoice_for_excel";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal EInvoiceJSonMdl GetEInvoiceJSon(int salerecid)
        {
            DataSet ds = new DataSet();
            EInvoiceJSonMdl dbobject = new EInvoiceJSonMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_sale_invoice_json_header";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = clsTableToModel.ConvertToList<EInvoiceJSonMdl>(ds)[0];

                    DataSet dsTran = clsTableToModel.getDataForModel<tranDtls>("", ds);
                    dbobject.TranDtls = clsTableToModel.ConvertToList<tranDtls>(dsTran)[0];

                    DataSet dsDoc = clsTableToModel.getDataForModel<docDtls>("", ds);
                    dbobject.DocDtls = clsTableToModel.ConvertToList<docDtls>(dsDoc)[0];

                    DataSet dsExp = clsTableToModel.getDataForModel<expDtls>("", ds);
                    dbobject.ExpDtls = clsTableToModel.ConvertToList<expDtls>(dsExp)[0];

                    DataSet dsSlr = clsTableToModel.getDataForModel<sellerDtls>("Slr_", ds);
                    dbobject.SellerDtls = clsTableToModel.ConvertToList<sellerDtls>(dsSlr)[0];

                    DataSet dsByr = clsTableToModel.getDataForModel<buyerDtls>("Byr_", ds);
                    dbobject.BuyerDtls = clsTableToModel.ConvertToList<buyerDtls>(dsByr)[0];

                    DataSet dsDsp = clsTableToModel.getDataForModel<dispDtls>("Dsp_", ds);
                    dbobject.DispDtls = clsTableToModel.ConvertToList<dispDtls>(dsDsp)[0];

                    DataSet dsShp = clsTableToModel.getDataForModel<shipDtls>("Shp_", ds);
                    dbobject.ShipDtls = clsTableToModel.ConvertToList<shipDtls>(dsShp)[0];

                    DataSet dsVal = clsTableToModel.getDataForModel<valDtls>("", ds);
                    dbobject.ValDtls = clsTableToModel.ConvertToList<valDtls>(dsVal)[0];

                    DataSet dsRef = clsTableToModel.getDataForModel<refDtls>("", ds);
                    dbobject.RefDtls = clsTableToModel.ConvertToList<refDtls>(dsRef)[0];

                    DataSet dsPay = clsTableToModel.getDataForModel<payDtls>("", ds);
                    dbobject.PayDtls = clsTableToModel.ConvertToList<payDtls>(dsPay)[0];

                    DataSet dsLgr = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_sale_invoice_json_ledger";
                    cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
                    mc.fillFromDatabase(dsLgr, cmd);
                    dbobject.ItemList = clsTableToModel.ConvertToList<itemList>(dsLgr);
                }
            }
            return dbobject;
        }
        //

    }
}