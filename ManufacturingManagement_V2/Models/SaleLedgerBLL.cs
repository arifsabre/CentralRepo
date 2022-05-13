
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
    public class SaleLedgerBLL : DbContext
    {
        //
        //internal DbSet<SaleLedgerMdl> StockLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static SaleLedgerBLL Instance
        {
            get { return new SaleLedgerBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int salerecid, SaleLedgerMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@SaleRecId", salerecid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemRecId", mc.getForSqlIntString(dbobject.ItemRecId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NoOfPckg", dbobject.NoOfPckg, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DelvQty", dbobject.DelvQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitRate", dbobject.UnitRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Amount", dbobject.Amount, DbType.Double));
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
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InspCertificate", dbobject.InspCertificate, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DispatchMemo", dbobject.DispatchMemo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ExciseDutyInc", dbobject.ExciseDutyInc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SaleTaxInc", dbobject.SaleTaxInc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LgrItemId", mc.getForSqlIntString(dbobject.LgrItemId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Discount", dbobject.Discount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TaxableAmount", dbobject.TaxableAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IsInclGst", dbobject.IsInclGst, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UnitName", dbobject.UnitName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@HSNCode", dbobject.HSNCode, DbType.String));
        }
        //
        internal List<SaleLedgerMdl> createObjectList(DataSet ds)
        {
            List<SaleLedgerMdl> ledgers = new List<SaleLedgerMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SaleLedgerMdl objmdl = new SaleLedgerMdl();
                objmdl.LRecId = Convert.ToInt32(dr["LRecId"].ToString());
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.ItemRecId = Convert.ToInt32(dr["ItemRecId"].ToString());
                objmdl.NoOfPckg = dr["NoOfPckg"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.DelvQty = Convert.ToDouble(dr["DelvQty"].ToString());
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.UnitRate = Convert.ToDouble(dr["UnitRate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
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
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.InspCertificate = dr["InspCertificate"].ToString();
                objmdl.DispatchMemo = dr["DispatchMemo"].ToString();
                objmdl.ExciseDutyInc = Convert.ToBoolean(dr["ExciseDutyInc"].ToString());
                objmdl.SaleTaxInc = Convert.ToBoolean(dr["SaleTaxInc"].ToString());
                objmdl.LgrItemId = Convert.ToInt32(dr["LgrItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.Discount = Convert.ToDouble(dr["Discount"].ToString());
                objmdl.TaxableAmount = Convert.ToDouble(dr["TaxableAmount"].ToString());
                objmdl.IsInclGst = Convert.ToBoolean(dr["IsInclGst"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());
                objmdl.HSNCode = dr["HSNCode"].ToString();
                //additional from porderdetail
                if (dr.Table.Columns.Contains("ConsigneeId"))
                {
                    objmdl.ConsigneeId = Convert.ToInt32(dr["ConsigneeId"].ToString());
                }
                if (dr.Table.Columns.Contains("ConsigneeName"))
                {
                    objmdl.ConsigneeName = dr["ConsigneeName"].ToString();
                }
                if (dr.Table.Columns.Contains("OrdQty"))
                {
                    objmdl.OrdQty = Convert.ToDouble(dr["OrdQty"].ToString());
                }
                if (dr.Table.Columns.Contains("DspQty"))
                {
                    objmdl.DspQty = Convert.ToDouble(dr["DspQty"].ToString());
                }
                if (dr.Table.Columns.Contains("RemQty"))
                {
                    objmdl.RemQty = Convert.ToDouble(dr["RemQty"].ToString());
                }
                if (dr.Table.Columns.Contains("HsQty"))
                {
                    objmdl.HsQty = Convert.ToDouble(dr["HsQty"].ToString());
                }
                if (dr.Table.Columns.Contains("DelvDateStr"))
                {
                    objmdl.DelvDateStr = dr["DelvDateStr"].ToString();
                }
                if (dr.Table.Columns.Contains("ItemSlNo"))
                {
                    objmdl.ItemSlNo = Convert.ToInt32(dr["ItemSlNo"].ToString());
                }
                //
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void saveItemLedger(SqlCommand cmd, int salerecid, SaleLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_ledger";
            addCommandParameters(cmd, salerecid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateItemLedger(SqlCommand cmd, int salerecid, SaleLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            if (dbobject.LRecId == 0)
            {
                cmd.CommandText = "usp_insert_tbl_ledger";
                addCommandParameters(cmd, salerecid, dbobject);
            }
            else if (dbobject.LRecId != 0)
            {
                cmd.CommandText = "usp_update_tbl_ledger";
                addCommandParameters(cmd, salerecid, dbobject);
                cmd.Parameters.Add(mc.getPObject("@LRecId", dbobject.LRecId, DbType.Int32));
            }
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateLedgerForDelvQtyNdCertificates(SqlCommand cmd, SaleLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_ledger_for_delvqtyndcertificates";
            cmd.Parameters.Add(mc.getPObject("@LRecId", dbobject.LRecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DelvQty", dbobject.DelvQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DispatchMemo", dbobject.DispatchMemo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InspCertificate", dbobject.InspCertificate, DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataTable getItemLedgerDataTable(int salerecid)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ledger";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(dt, cmd);
            return dt;
        }
        //
        internal DataSet getItemLedger(int salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ledger";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<SaleLedgerMdl> getItemLedgerList(int salerecid)
        {
            DataSet ds = getItemLedger(salerecid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}