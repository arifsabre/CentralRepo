
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
    public class TenderDetailBLL : DbContext
    {
        //
        //internal DbSet<SaleLedgerMdl> StockLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static TenderDetailBLL Instance
        {
            get { return new TenderDetailBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int tenderid, TenderDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemname", dbobject.ItemName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@l1l2", dbobject.L1L2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ourqty", dbobject.OurQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@unitname", dbobject.UnitName.Trim(), DbType.String));
        }
        //
        internal List<TenderDetailMdl> createObjectList(DataSet ds)
        {
            List<TenderDetailMdl> ledgers = new List<TenderDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TenderDetailMdl objmdl = new TenderDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ShortName = dr["ShortName"].ToString();
                objmdl.ItemName = dr["ItemName"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.L1L2 = dr["L1L2"].ToString();
                objmdl.OurQty = Convert.ToDouble(dr["OurQty"].ToString());
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void saveTenderDetail(SqlCommand cmd, int tenderid, TenderDetailMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_tenderdetail";
            addCommandParameters(cmd, tenderid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void updateTenderDetail(SqlCommand cmd, int tenderid, TenderDetailMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_update_tbl_tenderdetail";
            addCommandParameters(cmd, tenderid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataSet getTenderDetailData(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_gettenderdetail";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<TenderDetailMdl> getTenderDetailList(int tenderid)
        {
            DataSet ds = getTenderDetailData(tenderid);
            return createObjectList(ds);
        }
        //
        internal DataSet getTenderDetailForETender(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_gettenderdetailforetender";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        internal List<TenderDetailMdl> getTenderDetailForETenderList(int tenderid)
        {
            DataSet ds = getTenderDetailForETender(tenderid);
            return createObjectList(ds);
        }
        //
        //
        #endregion
        //
    }
}