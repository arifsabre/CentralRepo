using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class OrderLedgerBLL : DbContext
    {
        //
        //internal DbSet<OrderLedgerMdl> OrderLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static OrderLedgerBLL Instance
        {
            get { return new OrderLedgerBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd,int orderid, OrderLedgerMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@OrderId", orderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SlNo", dbobject.SlNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OrdQty", dbobject.OrdQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DspQty", dbobject.DspQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitId", dbobject.UnitId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Amount", dbobject.Amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentLgrId", mc.getForSqlIntString(dbobject.IndentLgrId.ToString()), DbType.Int32));
        }
        //
        private OrderLedgerMdl getItemFromDataRow(DataRow dr)
        {
            OrderLedgerMdl dbobject = new OrderLedgerMdl();
            if (dr != null)
            {
                
            }
            return dbobject;
        }
        //
        private List<OrderLedgerMdl> createObjectList(DataSet ds)
        {
            List<OrderLedgerMdl> ledgers = new List<OrderLedgerMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderLedgerMdl objmdl = new OrderLedgerMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.OrderId = Convert.ToInt32(dr["OrderId"].ToString());
                objmdl.SlNo = Convert.ToInt32(dr["SlNo"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.OrdQty = Convert.ToDouble(dr["OrdQty"].ToString());
                objmdl.DspQty = Convert.ToDouble(dr["DspQty"].ToString());
                objmdl.UnitId = Convert.ToInt32(dr["UnitId"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();//d
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                if (dr.Table.Columns.Contains("IndentId"))
                {
                    objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());//d
                }
                if (dr.Table.Columns.Contains("IndentLgrId"))
                {
                    objmdl.IndentLgrId = Convert.ToInt32(dr["IndentLgrId"].ToString());//d
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
        internal void insertObject(SqlCommand cmd,int orderid,OrderLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_orderledger";
            addCommandParameters(cmd,orderid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateObject(SqlCommand cmd,int orderid,int recid,OrderLedgerMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_tbl_orderledger";
            addCommandParameters(cmd,orderid, dbobject);
            cmd.Parameters.Add(mc.getPObject("@RecId", recid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        internal void deleteOrderLedger(SqlCommand cmd,int orderid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_orderledger";
            cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal OrderLedgerMdl searchOrderLedger(int recid)
        {
            DataSet ds = new DataSet();
            OrderLedgerMdl dbobject = new OrderLedgerMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_orderledger";
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
        internal DataSet getObjectData(int orderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_orderledger";
            cmd.Parameters.Add(mc.getPObject("@orderid", orderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<OrderLedgerMdl> getObjectList(int orderid)
        {
            DataSet ds = getObjectData(orderid);
            return createObjectList(ds);
        }
        //
        internal DataSet getOrderLedgerBySlipNoData(int slipno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_orderledger_by_slipno";
            cmd.Parameters.Add(mc.getPObject("@slipno", slipno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<OrderLedgerMdl> getOrderLedgerBySlipNo(int slipno)
        {
            DataSet ds = getOrderLedgerBySlipNoData(slipno);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}