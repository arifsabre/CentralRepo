using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class RfqTenderDetailBLL : DbContext
    {
        //
        //internal DbSet<RfqTenderDetailMdl> OrderLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static RfqTenderDetailBLL Instance
        {
            get { return new RfqTenderDetailBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int rfqid, RfqTenderDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@RfqId", rfqid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemName", dbobject.ItemName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@UnitName", dbobject.UnitName.Trim(), DbType.String));
        }
        //
        private List<RfqTenderDetailMdl> createObjectList(DataSet ds)
        {
            List<RfqTenderDetailMdl> ledgers = new List<RfqTenderDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RfqTenderDetailMdl objmdl = new RfqTenderDetailMdl();
                objmdl.RfqId = Convert.ToInt32(dr["RfqId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.ItemName = dr["ItemName"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());//d
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(SqlCommand cmd, int rfqid, RfqTenderDetailMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_rfqtenderdetail";
            addCommandParameters(cmd, rfqid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void deleteRfqLedger(SqlCommand cmd, int rfqid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_rfqtenderdetail";
            cmd.Parameters.Add(mc.getPObject("@RfqId", rfqid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal RfqTenderDetailMdl searchRfqLedger(int rfqid, int itemid)
        {
            DataSet ds = new DataSet();
            RfqTenderDetailMdl dbobject = new RfqTenderDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_rfqtender_item_info";
            cmd.Parameters.Add(mc.getPObject("@rfqid", rfqid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
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
        internal DataSet getObjectData(int rfqid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_rfqtenderdetail";
            cmd.Parameters.Add(mc.getPObject("@rfqid", rfqid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<RfqTenderDetailMdl> getObjectList(int rfqid)
        {
            DataSet ds = getObjectData(rfqid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}