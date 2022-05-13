
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
    public class StockLedgerBLL : DbContext
    {
        //
        //internal DbSet<StockLedgerMdl> StockLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static StockLedgerBLL Instance
        {
            get { return new StockLedgerBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, string vtype,int vno,DateTime vdate,StockLedgerMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(vdate), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@VDate", vdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitId", dbobject.UnitId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Amount", dbobject.Amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Discount", dbobject.Discount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TotalDiscount", dbobject.TotalDiscount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SgstPer", dbobject.SgstPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SgstAmount", dbobject.SgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CgstPer", dbobject.CgstPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CgstAmount", dbobject.CgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IgstPer", dbobject.IgstPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@IgstAmount", dbobject.IgstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightRate", dbobject.FreightRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FreightAmount", dbobject.FreightAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentLgrId", mc.getForSqlIntString(dbobject.IndentLgrId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@OrderLgrId", mc.getForSqlIntString(dbobject.OrderLgrId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PurchaseNo", dbobject.PurchaseNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PurchaseDate", dbobject.PurchaseDate.ToShortDateString(), DbType.DateTime));
        }
        //
        internal List<StockLedgerMdl> createObjectList(DataSet ds)
        {
            List<StockLedgerMdl> ledgers = new List<StockLedgerMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StockLedgerMdl objmdl = new StockLedgerMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.VType = dr["VType"].ToString();
                objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                objmdl.VDate = Convert.ToDateTime(dr["VDate"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
                objmdl.Discount = Convert.ToDouble(dr["Discount"].ToString());
                objmdl.TotalDiscount = Convert.ToDouble(dr["TotalDiscount"].ToString());
                objmdl.SgstPer = Convert.ToDouble(dr["SgstPer"].ToString());
                objmdl.SgstAmount = Convert.ToDouble(dr["SgstAmount"].ToString());
                objmdl.CgstPer = Convert.ToDouble(dr["CgstPer"].ToString());
                objmdl.CgstAmount = Convert.ToDouble(dr["CgstAmount"].ToString());
                objmdl.IgstPer = Convert.ToDouble(dr["IgstPer"].ToString());
                objmdl.IgstAmount = Convert.ToDouble(dr["IgstAmount"].ToString());
                objmdl.FreightRate = Convert.ToDouble(dr["FreightRate"].ToString());
                objmdl.FreightAmount = Convert.ToDouble(dr["FreightAmount"].ToString());
                objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.VDateStr = dr["VDateStr"].ToString();//d
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.UnitName = dr["UnitName"].ToString();//d
                if (dr.Table.Columns.Contains("VTypeName"))
                {
                    objmdl.VTypeName = dr["VTypeName"].ToString();//d
                }
                objmdl.IndentLgrId = Convert.ToInt32(dr["IndentLgrId"].ToString());
                objmdl.OrderLgrId = Convert.ToInt32(dr["OrderLgrId"].ToString());
                objmdl.PurchaseNo = dr["PurchaseNo"].ToString();
                objmdl.PurchaseDate = Convert.ToDateTime(dr["PurchaseDate"].ToString());
                if (dr.Table.Columns.Contains("PurchaseDateStr"))
                {
                    objmdl.PurchaseDateStr = dr["PurchaseDateStr"].ToString();//d
                }
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal int getNewVNoForStockLedger(SqlCommand cmd, string vtype)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_new_vno_for_stockledger";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            return Convert.ToInt32(mc.getFromDatabase(cmd,cmd.Connection));
        }
        //
        internal void insertObject(SqlCommand cmd, string vtype, int vno, DateTime vdate, StockLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_stockledger";
            addCommandParameters(cmd,vtype,vno,vdate, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateObject(SqlCommand cmd, int recid, string vtype, int vno, DateTime vdate, StockLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_tbl_stockledger";
            addCommandParameters(cmd,vtype,vno,vdate, dbobject);
            cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        /// <summary>
        /// --updates avgpurchaserate to indentledger also
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="recid"></param>
        internal void updatePurchaseQtyToIndentLedger(SqlCommand cmd, int recid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_purchase_qty_to_indent_ledger";
            cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// --updates avgpurchaserate and purchased qty to indentledger also
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="recid"></param>
        internal void updatePurchaseQtyToOrderLedger(SqlCommand cmd, int recid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_purchase_qty_to_order_ledger";
            cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        internal void deleteStockLedgerByRecId(SqlCommand cmd, int recid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_stockledger_by_recid";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
        }
        //
        internal void deleteStockLedger(SqlCommand cmd, string vtype,int vno)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_stockledger";
            cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal StockLedgerMdl searchStockLedger(int recid)
        {
            DataSet ds = new DataSet();
            StockLedgerMdl dbobject = new StockLedgerMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_stockledger";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
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
        internal DataSet getObjectData(string vtype,string vno,string cmpcode,string finyr)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_stockledger";
            cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", cmpcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", finyr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<StockLedgerMdl> getObjectList(string vtype, string vno, string cmpcode, string finyr)
        {
            DataSet ds = getObjectData(vtype, vno, cmpcode, finyr);
            return createObjectList(ds);
        }
        //
        internal List<StockLedgerMdl> getStockLedgerListForStock(string vtype, string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_stockledger_for_stock";
            cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        internal List<StockLedgerMdl> getStockLedgerListForPurchase(string vtype, string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_stockledger_for_purchase";
            cmd.Parameters.Add(mc.getPObject("@VType", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        public StockLedgerMdl setStockLedgerValues(StockLedgerMdl ledgerobject)
        {
            ledgerobject.Amount = ledgerobject.Qty * ledgerobject.Rate;
            ledgerobject.TotalDiscount = ledgerobject.Qty * ledgerobject.Discount;
            ledgerobject.SgstAmount = (ledgerobject.Amount - ledgerobject.TotalDiscount) * ledgerobject.SgstPer / 100;
            ledgerobject.CgstAmount = (ledgerobject.Amount - ledgerobject.TotalDiscount) * ledgerobject.CgstPer / 100;
            ledgerobject.IgstAmount = (ledgerobject.Amount - ledgerobject.TotalDiscount) * ledgerobject.IgstPer / 100;
            ledgerobject.FreightAmount = ledgerobject.Qty * ledgerobject.FreightRate;
            ledgerobject.NetAmount = ledgerobject.Amount - ledgerobject.TotalDiscount + ledgerobject.SgstAmount + ledgerobject.CgstAmount + ledgerobject.IgstAmount + ledgerobject.FreightAmount;
            if (ledgerobject.Remarks == null)
            {
                ledgerobject.Remarks = "";
            }
            if (ledgerobject.PurchaseNo == null)
            {
                ledgerobject.PurchaseNo = "";
                ledgerobject.PurchaseDate = DateTime.Now;
            }
            return ledgerobject;
        }
        //
        public List<StockLedgerMdl> setStockLedgerValues(List<StockLedgerMdl> ledgerobject)
        {
            for (int i = 0; i < ledgerobject.Count; i++)
            {
                ledgerobject[i].Amount = ledgerobject[i].Qty * ledgerobject[i].Rate;
                ledgerobject[i].TotalDiscount = ledgerobject[i].Qty * ledgerobject[i].Discount;
                ledgerobject[i].SgstAmount = (ledgerobject[i].Amount - ledgerobject[i].TotalDiscount) * ledgerobject[i].SgstPer / 100;
                ledgerobject[i].CgstAmount = (ledgerobject[i].Amount - ledgerobject[i].TotalDiscount) * ledgerobject[i].CgstPer / 100;
                ledgerobject[i].IgstAmount = (ledgerobject[i].Amount - ledgerobject[i].TotalDiscount) * ledgerobject[i].IgstPer / 100;
                ledgerobject[i].FreightAmount = ledgerobject[i].Qty * ledgerobject[i].FreightRate;
                ledgerobject[i].NetAmount = ledgerobject[i].Amount - ledgerobject[i].TotalDiscount + ledgerobject[i].SgstAmount + ledgerobject[i].CgstAmount + ledgerobject[i].IgstAmount + ledgerobject[i].FreightAmount;
                if (ledgerobject[i].Remarks == null)
                {
                    ledgerobject[i].Remarks = "";
                }
                if (ledgerobject[i].PurchaseNo == null)
                {
                    ledgerobject[i].PurchaseNo = "";
                    ledgerobject[i].PurchaseDate = DateTime.Now;
                }
            }
            return ledgerobject;
        }
        //
        internal bool isValidIssueAgainstApprovedIndentQty(SqlCommand cmd, int indentid, ArrayList arlitems)
        {
            DataSet ds = new DataSet();
            for (int i = 0; i < arlitems.Count; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_issue_against_approved_indent_qty";
                cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                if (Convert.ToDouble(ds.Tables[0].Rows[0]["issuedqty"].ToString()) > Convert.ToDouble(ds.Tables[0].Rows[0]["appqty"].ToString()))
                {
                    return false;
                }
            }
            return true;
        }
        //
        internal bool isValidPurchaseAgainstOrderQty(SqlCommand cmd, string orderids, ArrayList arlitems,int purchaseid,int orderid=0)
        {
            //if (orderids.Length == 0) { return true; };
            //string[] orders = orderids.Split(',');
            //DataSet ds = new DataSet();
            //double pcqty = 0;
            //double ordqty = 0;
            //double thisPurcahseQty = 0;
            //double thisOrderQty = 0;
            //for (int i = 0; i < arlitems.Count; i++)
            //{
            //    pcqty = 0;
            //    ordqty = 0;
            //    thisPurcahseQty = 0;
            //    thisOrderQty = 0;
            //    //ds = new DataSet();
            //    //cmd.Parameters.Clear();
            //    //cmd.CommandText = "usp_get_purchase_qty";
            //    //cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
            //    //cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
            //    //mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //    //thisPurcahseQty = Convert.ToDouble(ds.Tables[0].Rows[0]["pcqty"].ToString());
            //    //note --not required if this method is not to be used by order bll in update/delete
            //    //ds = new DataSet();
            //    //cmd.Parameters.Clear();
            //    //cmd.CommandText = "usp_get_order_qty";
            //    //cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
            //    //cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
            //    //mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //    //thisOrderQty = Convert.ToDouble(ds.Tables[0].Rows[0]["ordqty"].ToString());
            //    for (int j = 0; j < orders.Length; j++)
            //    {
            //        ds = new DataSet();
            //        cmd.Parameters.Clear();
            //        cmd.CommandText = "usp_get_purchase_against_order_qty";
            //        cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
            //        cmd.Parameters.Add(mc.getPObject("@orderid", orders[j].ToString(), DbType.Int32));
            //        mc.fillFromDatabase(ds, cmd, cmd.Connection);
            //        pcqty += Convert.ToDouble(ds.Tables[0].Rows[0]["pcqty"].ToString());
            //        ordqty += Convert.ToDouble(ds.Tables[0].Rows[0]["ordqty"].ToString());
            //    }
            //    if (pcqty - thisPurcahseQty > ordqty + thisOrderQty)
            //    {
            //        return false;
            //    }
            //}

            //to check
            if (orderids.Length == 0) { return true; };
            string[] orders = orderids.Split(',');
            DataSet ds = new DataSet();
            double pcqty = 0;
            double ordqty = 0;
            for (int i = 0; i < arlitems.Count; i++)
            {
                ordqty = 0;
                for (int j = 0; j < orders.Length; j++)
                {
                    ds = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_order_qty_to_check";
                    cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@orderid", orders[j].ToString(), DbType.Int32));
                    mc.fillFromDatabase(ds, cmd, cmd.Connection);
                    ordqty += Convert.ToDouble(ds.Tables[0].Rows[0]["ordqty"].ToString());
                }
            }
            for (int i = 0; i < arlitems.Count; i++)
            {
                pcqty = 0;
                for (int j = 0; j < orders.Length; j++)
                {
                    ds = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_order_qty_to_check";
                    cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@orderid", orders[j].ToString(), DbType.Int32));
                    mc.fillFromDatabase(ds, cmd, cmd.Connection);
                    pcqty += Convert.ToDouble(ds.Tables[0].Rows[0]["ordqty"].ToString());
                }
            }
            if (pcqty > ordqty)
            {
                return false;
            }
            return true;
        }
        //
        internal bool isValidPurchaseAgainstPurchaseRequiredQty(SqlCommand cmd,string indentids, ArrayList arlitems, int purchaseid,int indentid=0)
        {
            if (indentids.Length == 0) { return true; };
            string[] indents = indentids.Split(',');
            DataSet ds = new DataSet();
            double purchased = 0;
            double prequired = 0;
            double thisPurcahseQty = 0;
            double thisIndentPRQty = 0;
            for (int i = 0; i < arlitems.Count; i++)
            {
                purchased = 0;
                prequired = 0;
                thisPurcahseQty = 0;
                thisIndentPRQty = 0;
                //ds = new DataSet();
                //cmd.Parameters.Clear();
                //cmd.CommandText = "usp_get_purchase_qty";
                //cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                //cmd.Parameters.Add(mc.getPObject("@purchaseid", purchaseid, DbType.Int32));
                //mc.fillFromDatabase(ds, cmd, cmd.Connection);
                //thisPurcahseQty = Convert.ToDouble(ds.Tables[0].Rows[0]["pcqty"].ToString());
                //note --not required if it is not to be used by indent/stock(for indent) bll in update/delete
                //ds = new DataSet();
                //cmd.Parameters.Clear();
                //cmd.CommandText = "usp_get_indent_purchased_required_qty";
                //cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                //cmd.Parameters.Add(mc.getPObject("@indentid", indentid, DbType.Int32));
                //mc.fillFromDatabase(ds, cmd, cmd.Connection);
                //thisIndentPRQty = Convert.ToDouble(ds.Tables[0].Rows[0]["pcqty"].ToString());
                for (int j = 0; j < indents.Length; j++)
                {
                    ds = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_purchased_against_purchase_required_qty";
                    cmd.Parameters.Add(mc.getPObject("@itemid", arlitems[i].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@indentid", indents[j].ToString(), DbType.Int32));
                    mc.fillFromDatabase(ds, cmd, cmd.Connection);
                    purchased += Convert.ToDouble(ds.Tables[0].Rows[0]["purchasedqty"].ToString());
                    prequired += Convert.ToDouble(ds.Tables[0].Rows[0]["prequiredqty"].ToString());
                }
                if (purchased - thisPurcahseQty > prequired + thisIndentPRQty)
                {
                    return false;
                }
            }
            return true;
        }
        //
        #endregion
        //
    }
}