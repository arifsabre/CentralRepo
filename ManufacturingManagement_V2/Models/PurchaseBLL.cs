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
    public class PurchaseBLL : DbContext
    {
        //
        public DbSet<PurchaseMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        StockLedgerMdl ledgerObj = new StockLedgerMdl();
        StockLedgerBLL ledgerBLL = new StockLedgerBLL();
        public static PurchaseBLL Instance
        {
            get { return new PurchaseBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, PurchaseMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VType", dbobject.VType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", dbobject.VNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(dbobject.VDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CashCredit", dbobject.CashCredit, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VendorId", dbobject.VendorId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SubTotal", dbobject.SubTotal, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Discount", dbobject.Discount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SgstAmount", dbobject.SgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CgstAmount", dbobject.CgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IgstAmount", dbobject.IgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightAmount", dbobject.FreightAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@OtherCharges", dbobject.OtherCharges, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RoundOffAmt", dbobject.RoundOffAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentIds", dbobject.IndentIds, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OrderIds", dbobject.OrderIds, DbType.String));
        }
        //
        private List<PurchaseMdl> createObjectList(DataSet ds)
        {
            List<PurchaseMdl> purchases = new List<PurchaseMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PurchaseMdl objmdl = new PurchaseMdl();
                objmdl.PurchaseId = Convert.ToInt32(dr["PurchaseId"].ToString());
                objmdl.VType = dr["VType"].ToString();
                objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                objmdl.CashCredit = dr["CashCredit"].ToString();
                objmdl.VendorId = Convert.ToInt32(dr["VendorId"].ToString());
                objmdl.SubTotal = Convert.ToDouble(dr["SubTotal"].ToString());
                objmdl.Discount = Convert.ToDouble(dr["Discount"].ToString());
                objmdl.SgstAmount = Convert.ToDouble(dr["SgstAmount"].ToString());
                objmdl.CgstAmount = Convert.ToDouble(dr["CgstAmount"].ToString());
                objmdl.IgstAmount = Convert.ToDouble(dr["IgstAmount"].ToString());
                objmdl.FreightAmount = Convert.ToDouble(dr["FreightAmount"].ToString());
                objmdl.OtherCharges = Convert.ToDouble(dr["OtherCharges"].ToString());
                objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                if (dr.Table.Columns.Contains("VendorName"))
                {
                    objmdl.VendorName = dr["VendorName"].ToString();
                }
                if (dr.Table.Columns.Contains("VTypeName"))
                {
                    objmdl.VTypeName = dr["VTypeName"].ToString();//d
                }
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.IndentIds = dr["IndentIds"].ToString();
                objmdl.OrderIds = dr["OrderIds"].ToString();
                purchases.Add(objmdl);
            }
            return purchases;
        }
        //
        private bool checkSetValidModel(PurchaseMdl dbobject)
        {
            Message = "";
            //if (mc.isValidDate(dbobject.VDate) == false)
            //{
            //    Message = "Invalid date";
            //    return false;
            //}
            if (dbobject.VendorId == 0)
            {
                Message = "Vendor not selected!";
                return false;
            }
            dbobject.VType = "pc";//cash purchase
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.IndentIds == null)
            {
                dbobject.IndentIds = "";
            }
            if (dbobject.OrderIds == null)
            {
                dbobject.OrderIds = "";
            }
            double amt = 0;
            double sgst = 0;
            double cgst = 0;
            double igst = 0;
            double disc = 0;
            double frt = 0;
            dbobject.Ledgers = ledgerBLL.setStockLedgerValues(dbobject.Ledgers);
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                amt += dbobject.Ledgers[i].Amount;
                disc += dbobject.Ledgers[i].TotalDiscount;
                sgst += dbobject.Ledgers[i].SgstAmount;
                cgst += dbobject.Ledgers[i].CgstAmount;
                igst += dbobject.Ledgers[i].IgstAmount;
                frt += dbobject.Ledgers[i].FreightAmount;
            }
            dbobject.SubTotal = amt;
            dbobject.Discount = disc;
            dbobject.SgstAmount = sgst;
            dbobject.CgstAmount = cgst;
            dbobject.IgstAmount = igst;
            dbobject.FreightAmount = frt;
            double netamt = amt - disc + sgst + cgst + igst + frt + dbobject.OtherCharges;
            dbobject.RoundOffAmt = netamt - Math.Round(netamt);
            dbobject.NetAmount = Math.Round(netamt);
            return true;
        }
        //
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
        private DataTable getOrderLedgersData(SqlCommand cmd,string[] orderids,string opt)
        {
            DataSet dsPOrders = new DataSet();
            dsPOrders.Tables.Add();
            dsPOrders.Tables[0].Columns.Add("orderid", typeof(System.Int32));
            dsPOrders.Tables[0].Columns.Add("itemid", typeof(System.Int32));
            dsPOrders.Tables[0].Columns.Add("dspqty", typeof(System.Double));
            dsPOrders.Tables[0].Columns.Add("remqty", typeof(System.Double));
            dsPOrders.Tables[0].Columns.Add("recid", typeof(System.Int32));
            DataRow dr = dsPOrders.Tables[0].NewRow();
            DataSet dsOrdLgr = new DataSet();
            if (opt == "add")
            {
                for (int i = 0; i < orderids.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_orderledger";
                    cmd.Parameters.Add(mc.getPObject("@orderid", orderids[i], DbType.Int32));
                    mc.fillFromDatabase(dsOrdLgr, cmd, cmd.Connection);
                    for (int j = 0; j < dsOrdLgr.Tables[0].Rows.Count; j++)
                    {
                        if (Convert.ToDouble(dsOrdLgr.Tables[0].Rows[j]["remqty"].ToString()) > 0)
                        {
                            dr = dsPOrders.Tables[0].NewRow();
                            dr["orderid"] = orderids[i];
                            dr["itemid"] = dsOrdLgr.Tables[0].Rows[j]["itemid"].ToString();
                            dr["dspqty"] = dsOrdLgr.Tables[0].Rows[j]["dspqty"].ToString();
                            dr["remqty"] = dsOrdLgr.Tables[0].Rows[j]["remqty"].ToString();
                            dr["recid"] = dsOrdLgr.Tables[0].Rows[j]["recid"].ToString();
                            dsPOrders.Tables[0].Rows.Add(dr);
                        }
                    }
                }
                dsPOrders.Tables[0].DefaultView.Sort = "orderid asc,itemid asc";
            }
            else if (opt == "remove")
            {
                for (int i = 0; i < orderids.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_orderledger";
                    cmd.Parameters.Add(mc.getPObject("@orderid", orderids[i], DbType.Int32));
                    mc.fillFromDatabase(dsOrdLgr, cmd, cmd.Connection);
                    for (int j = 0; j < dsOrdLgr.Tables[0].Rows.Count; j++)
                    {
                        dr = dsPOrders.Tables[0].NewRow();
                        dr["orderid"] = orderids[i];
                        dr["itemid"] = dsOrdLgr.Tables[0].Rows[j]["itemid"].ToString();
                        dr["dspqty"] = dsOrdLgr.Tables[0].Rows[j]["dspqty"].ToString();
                        dr["remqty"] = dsOrdLgr.Tables[0].Rows[j]["remqty"].ToString();
                        dr["recid"] = dsOrdLgr.Tables[0].Rows[j]["recid"].ToString();
                        dsPOrders.Tables[0].Rows.Add(dr);
                    }
                }
                dsPOrders.Tables[0].DefaultView.Sort = "orderid desc,itemid desc";
            }
            return dsPOrders.Tables[0].DefaultView.ToTable();
        }
        //
        private DataTable getIndentLedgersData(SqlCommand cmd, string[] indentids,string opt)
        {
            DataSet dsIndents = new DataSet();
            dsIndents.Tables.Add();
            dsIndents.Tables[0].Columns.Add("indentid", typeof(System.Int32));
            dsIndents.Tables[0].Columns.Add("itemid", typeof(System.Int32));
            dsIndents.Tables[0].Columns.Add("dspqty", typeof(System.Double));//purchasedqty
            dsIndents.Tables[0].Columns.Add("remqty", typeof(System.Double));//purchasebalance
            dsIndents.Tables[0].Columns.Add("recid", typeof(System.Int32));
            DataRow dr = dsIndents.Tables[0].NewRow();
            DataSet dsOrdLgr = new DataSet();
            if (opt == "add")
            {
                for (int i = 0; i < indentids.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_indentledger";
                    cmd.Parameters.Add(mc.getPObject("@indentid", indentids[i], DbType.Int32));
                    mc.fillFromDatabase(dsOrdLgr, cmd, cmd.Connection);
                    for (int j = 0; j < dsOrdLgr.Tables[0].Rows.Count; j++)
                    {
                        if (Convert.ToDouble(dsOrdLgr.Tables[0].Rows[j]["purchasebalance"].ToString()) > 0)
                        {
                            dr = dsIndents.Tables[0].NewRow();
                            dr["indentid"] = indentids[i];
                            dr["itemid"] = dsOrdLgr.Tables[0].Rows[j]["itemid"].ToString();
                            dr["dspqty"] = dsOrdLgr.Tables[0].Rows[j]["purchasedqty"].ToString();//note
                            dr["remqty"] = dsOrdLgr.Tables[0].Rows[j]["purchasebalance"].ToString();//note
                            dr["recid"] = dsOrdLgr.Tables[0].Rows[j]["recid"].ToString();
                            dsIndents.Tables[0].Rows.Add(dr);
                        }
                    }
                }
                dsIndents.Tables[0].DefaultView.Sort = "indentid asc,itemid asc";
            }
            else if (opt == "remove")
            {
                for (int i = 0; i < indentids.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_indentledger";
                    cmd.Parameters.Add(mc.getPObject("@indentid", indentids[i], DbType.Int32));
                    mc.fillFromDatabase(dsOrdLgr, cmd, cmd.Connection);
                    for (int j = 0; j < dsOrdLgr.Tables[0].Rows.Count; j++)
                    {
                        dr = dsIndents.Tables[0].NewRow();
                        dr["indentid"] = indentids[i];
                        dr["itemid"] = dsOrdLgr.Tables[0].Rows[j]["itemid"].ToString();
                        dr["dspqty"] = dsOrdLgr.Tables[0].Rows[j]["purchasedqty"].ToString();//note
                        dr["remqty"] = dsOrdLgr.Tables[0].Rows[j]["purchasebalance"].ToString();//note
                        dr["recid"] = dsOrdLgr.Tables[0].Rows[j]["recid"].ToString();
                        dsIndents.Tables[0].Rows.Add(dr);
                    }
                }
                dsIndents.Tables[0].DefaultView.Sort = "indentid desc,itemid desc";
            }
            return dsIndents.Tables[0].DefaultView.ToTable();
        }
        //
        private DataTable getQuantityLedger(PurchaseMdl dbobject)
        {
            DataTable dtr = new DataTable();
            dtr.Columns.Add("itemid", typeof(System.Int32));
            dtr.Columns.Add("qty", typeof(System.Double));
            DataRow drlgr = dtr.NewRow();
            System.Collections.ArrayList arl = new System.Collections.ArrayList();
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (arl.Contains(dbobject.Ledgers[i].ItemId) == false)
                {
                    drlgr = dtr.NewRow();
                    drlgr["itemid"] = dbobject.Ledgers[i].ItemId;
                    drlgr["qty"] = dbobject.Ledgers[i].Qty;
                    dtr.Rows.Add(drlgr);
                    arl.Add(dbobject.Ledgers[i].ItemId);
                }
                else
                {
                    for (int j = 0; j < dtr.Rows.Count; j++)
                    {
                        if (dbobject.Ledgers[i].ItemId.ToString() == dtr.Rows[j]["itemid"].ToString())
                        {
                            dtr.Rows[j]["qty"] = Convert.ToDouble(dtr.Rows[j]["qty"].ToString()) + dbobject.Ledgers[i].Qty;
                        }
                    }
                }
            }
            dtr.DefaultView.Sort = "itemid asc";
            return dtr;
        }
        //
        private bool UpdateOrderDispatchQty_Add(SqlCommand cmd,PurchaseMdl dbobject)
        {
            if (dbobject.OrderIds.Length == 0) { return true; };//purchase without-order
            //get list of items for dsp qty<ordqty grouped by orderid/itemid
            string[] orderids = dbobject.OrderIds.Split(',');
            //check existence/validity of orderid
            for (int i = 0; i < orderids.Length; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_isfound_orderid";
                cmd.Parameters.Add(mc.getPObject("@orderid", orderids[i].ToString(), DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection)) == false)
                {
                    Message = "Invalid Order Id: +"+orderids[i].ToString();
                    return false;
                }
            }
            //...
            DataTable dtlgr = getOrderLedgersData(cmd,orderids,"add");
            //get sum(pqty) grouped by item for entered ledger
            DataTable dtr = getQuantityLedger(dbobject);
            //update dspqty so for
            double adjQty = 0;
            double remQty = 0;
            double dQty = 0;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                adjQty = Convert.ToDouble(dtr.Rows[i]["qty"].ToString());
                for (int j = 0; j < dtlgr.Rows.Count; j++)
                {
                    if (dtr.Rows[i]["itemid"].ToString() == dtlgr.Rows[j]["itemid"].ToString())
                    {
                        remQty = Convert.ToDouble(dtlgr.Rows[j]["remqty"].ToString());
                        if (remQty <= adjQty)
                        {
                            dQty = remQty;
                            adjQty = adjQty-remQty;
                        }
                        else if(remQty > adjQty)
                        {
                            //dQty = remQty - adjQty;//note
                            dQty = adjQty;
                            adjQty = 0;

                        }
                        if(dQty > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "usp_update_orderledger_dspqty_add";
                            cmd.Parameters.Add(mc.getPObject("@qty", dQty, DbType.Double));
                            cmd.Parameters.Add(mc.getPObject("@recid", dtlgr.Rows[j]["recid"].ToString(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            return true;
        }
        //
        private bool UpdatePurchasedQtyToIndentLedger_Add(SqlCommand cmd, PurchaseMdl dbobject)
        {
            if (dbobject.IndentIds.Length == 0) { return true; };//purchase without-indent
            //get list of items for purchased qty < prequired qty grouped by indentid/itemid
            string[] indentids = dbobject.IndentIds.Split(',');
            //check existence/validity of indentid
            for (int i = 0; i < indentids.Length; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_isfound_indentid";
                cmd.Parameters.Add(mc.getPObject("@indentid", indentids[i].ToString(), DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection)) == false)
                {
                    Message = "Invalid Indent Id: +" + indentids[i].ToString();
                    return false;
                }
            }
            //...
            DataTable dtlgr = getIndentLedgersData(cmd, indentids,"add");
            //get sum(pqty) grouped by item for entered ledger
            DataTable dtr = getQuantityLedger(dbobject);
            //update dspqty so for
            double adjQty = 0;
            double remQty = 0;
            double dQty = 0;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                adjQty = Convert.ToDouble(dtr.Rows[i]["qty"].ToString());
                for (int j = 0; j < dtlgr.Rows.Count; j++)
                {
                    if (dtr.Rows[i]["itemid"].ToString() == dtlgr.Rows[j]["itemid"].ToString())
                    {
                        remQty = Convert.ToDouble(dtlgr.Rows[j]["remqty"].ToString());//purchasebalanceqty
                        if (remQty <= adjQty)
                        {
                            dQty = remQty;
                            adjQty = adjQty - remQty;
                        }
                        else if (remQty > adjQty)
                        {
                            //dQty = remQty - adjQty;//note
                            dQty = adjQty;
                            adjQty = 0;
                        }
                        if (dQty > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "usp_update_indentledger_purchasedqty_add";
                            cmd.Parameters.Add(mc.getPObject("@qty", dQty, DbType.Double));
                            cmd.Parameters.Add(mc.getPObject("@recid", dtlgr.Rows[j]["recid"].ToString(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            return true;
        }
        //
        private void UpdateOrderDispatchQty_Remove(SqlCommand cmd, int purchaseid)
        {
            //existing purchase orderids and stock qtys
            DataSet ds = new DataSet();
            DataSet dslgr = new DataSet();
            PurchaseMdl dbobject = new PurchaseMdl();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_purchase";
            cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //return if purchase was without-order
            if (ds.Tables[0].Rows[0]["orderids"].ToString().Length == 0) { return; };
            dbobject.Ledgers = ledgerBLL.getObjectList("xp", "0", "0", "");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_stockledger";
                    cmd.Parameters.Add(mc.getPObject("@VType", ds.Tables[0].Rows[0]["vtype"].ToString(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@VNo", ds.Tables[0].Rows[0]["vno"].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@CompCode", ds.Tables[0].Rows[0]["compcode"].ToString(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@FinYear", ds.Tables[0].Rows[0]["finyear"].ToString(), DbType.String));
                    mc.fillFromDatabase(dslgr, cmd, cmd.Connection);
                    dbobject.Ledgers = ledgerBLL.createObjectList(dslgr);
                }
            }
            //get list of items for dsp qty<ordqty grouped by orderid/itemid
            string[] orderids = ds.Tables[0].Rows[0]["orderids"].ToString().Split(',');
            DataTable dtlgr = getOrderLedgersData(cmd, orderids,"remove");
            //get sum(pqty) grouped by item from stock ledger
            DataTable dtr = getQuantityLedger(dbobject);
            //update dspqty so for
            double adjQty = 0;
            double dspQty = 0;
            double dQty = 0;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                adjQty = Convert.ToDouble(dtr.Rows[i]["qty"].ToString());
                for (int j = 0; j < dtlgr.Rows.Count; j++)
                {
                    if (dtr.Rows[i]["itemid"].ToString() == dtlgr.Rows[j]["itemid"].ToString())
                    {
                        dspQty = Convert.ToDouble(dtlgr.Rows[j]["dspqty"].ToString());
                        if (dspQty <= adjQty)
                        {
                            dQty = dspQty;
                            adjQty = adjQty - dspQty;
                        }
                        else if (dspQty > adjQty)
                        {
                            dQty = adjQty;
                            adjQty = 0;
                        }
                        if (dQty > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "usp_update_orderledger_dspqty_remove";
                            cmd.Parameters.Add(mc.getPObject("@qty", dQty, DbType.Double));
                            cmd.Parameters.Add(mc.getPObject("@recid", dtlgr.Rows[j]["recid"].ToString(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //
        private void UpdatePurchasedQtyToIndentLedger_Remove(SqlCommand cmd, int purchaseid)
        {
            //existing purchase indentids and stock qtys
            DataSet ds = new DataSet();
            DataSet dslgr = new DataSet();
            PurchaseMdl dbobject = new PurchaseMdl();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_purchase";
            cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //return if purchase was without-indent
            if (ds.Tables[0].Rows[0]["indentids"].ToString().Length == 0) { return; };
            dbobject.Ledgers = ledgerBLL.getObjectList("xp", "0", "0", "");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_stockledger";
                    cmd.Parameters.Add(mc.getPObject("@VType", ds.Tables[0].Rows[0]["vtype"].ToString(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@VNo", ds.Tables[0].Rows[0]["vno"].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@CompCode", ds.Tables[0].Rows[0]["compcode"].ToString(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@FinYear", ds.Tables[0].Rows[0]["finyear"].ToString(), DbType.String));
                    mc.fillFromDatabase(dslgr, cmd, cmd.Connection);
                    dbobject.Ledgers = ledgerBLL.createObjectList(dslgr);
                }
            }
            //get list of items for purchased qty < prequired qty grouped by indentid/itemid
            string[] indentids = ds.Tables[0].Rows[0]["indentids"].ToString().Split(',');
            DataTable dtlgr = getIndentLedgersData(cmd, indentids,"remove");
            //get sum(pqty) grouped by item from stock ledger
            DataTable dtr = getQuantityLedger(dbobject);
            //update purchasedqty to indentledger so for
            double adjQty = 0;
            double dspQty = 0;
            double dQty = 0;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                adjQty = Convert.ToDouble(dtr.Rows[i]["qty"].ToString());
                for (int j = 0; j < dtlgr.Rows.Count; j++)
                {
                    if (dtr.Rows[i]["itemid"].ToString() == dtlgr.Rows[j]["itemid"].ToString())
                    {
                        dspQty = Convert.ToDouble(dtlgr.Rows[j]["dspqty"].ToString());//purchasedqty
                        if (dspQty <= adjQty)
                        {
                            dQty = dspQty;
                            adjQty = adjQty - dspQty;
                        }
                        else if (dspQty > adjQty)
                        {
                            dQty = adjQty;
                            adjQty = 0;
                        }
                        if (dQty > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "usp_update_indentledger_purchasedqty_remove";
                            cmd.Parameters.Add(mc.getPObject("@qty", dQty, DbType.Double));
                            cmd.Parameters.Add(mc.getPObject("@recid", dtlgr.Rows[j]["recid"].ToString(), DbType.Int32));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(PurchaseMdl dbobject)
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
                cmd.CommandText = "usp_get_new_purchase_vno";
                cmd.Parameters.Add(mc.getPObject("@vtype", dbobject.VType, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                dbobject.VNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_purchase";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.PurchaseId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_purchase, "purchaseid"));
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, mc.getDateByString(dbobject.VDate), dbobject.Ledgers[i]);
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_purchase, dbobject.PurchaseId.ToString(), "Inserted " + dbobject.VType);
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(PurchaseMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_valid_to_delete_purchase";
                cmd.Parameters.Add(mc.getPObject("@purchaseid", dbobject.PurchaseId, DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, conn)) == false)
                {
                    Message = "Bill payment has been found for this purchase number, so it cannot be updated!";
                    return;
                }
                //
                //delete existing stockledger
                ledgerBLL.deleteStockLedger(cmd, dbobject.VType, dbobject.VNo);
                //insert new stockledger
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, mc.getDateByString(dbobject.VDate), dbobject.Ledgers[i]);
                }
                //updation
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_purchase";//deletes store ledger also
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@purchaseid", dbobject.PurchaseId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_purchase, dbobject.PurchaseId.ToString(), "Updated " + dbobject.VType);
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int purchaseid)
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
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_valid_to_delete_purchase";
                cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, conn)) == false)
                {
                    Message = "Bill payment has been found for this purchase number, so it cannot be deleted!";
                    return;
                }
                //
                //reverse dspqtys by existing stockledger to orderledger
                UpdateOrderDispatchQty_Remove(cmd, purchaseid);
                //reverse purchased qty by existing stockledger to indentledger
                UpdatePurchasedQtyToIndentLedger_Remove(cmd, purchaseid);
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_purchase";//deletes stock ledger also
                cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_order, purchaseid.ToString(), "Deleted");
                //
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("PurchaseBLL", "deleteObject", ex.Message);
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
        internal PurchaseMdl searchObject(int purchaseid)
        {
            DataSet ds = new DataSet();
            PurchaseMdl dbobject = new PurchaseMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_purchase";
            cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            ds = new DataSet();
            ds = getVTypeVNoByRecId(purchaseid);
            dbobject.Ledgers = ledgerBLL.getObjectList("xp", "0", "0", "");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject.Ledgers = ledgerBLL.getObjectList(ds.Tables[0].Rows[0]["vtype"].ToString(), ds.Tables[0].Rows[0]["vno"].ToString(), ds.Tables[0].Rows[0]["compcode"].ToString(), ds.Tables[0].Rows[0]["finyear"].ToString());
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(string dtfrom,string dtto,string vtype = "pc")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_purchase";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PurchaseMdl> getObjectList(string dtfrom,string dtto,string vtype)
        {
            DataSet ds = getObjectData(dtfrom,dtto,vtype);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}