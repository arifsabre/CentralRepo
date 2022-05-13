using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AmcDetailBLL : DbContext
    {
        //
        //internal DbSet<RfqTenderDetailMdl> OrderLedgers { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static AmcDetailBLL Instance
        {
            get { return new AmcDetailBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, int amcid, AmcDetailMdl dbobject)
        {
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Qty", dbobject.Qty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Rate", dbobject.Rate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Amount", dbobject.Amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VatPer", dbobject.VatPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VatAmount", dbobject.VatAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SatPer", dbobject.SatPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SatAmount", dbobject.SatAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CstPer", dbobject.CstPer, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CstAmount", dbobject.CstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UnitName", dbobject.UnitName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@HsnCode", dbobject.HsnCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemFor", dbobject.ItemFor.Trim(), DbType.String));
        }
        //
        private List<AmcDetailMdl> createObjectList(DataSet ds)
        {
            List<AmcDetailMdl> ledgers = new List<AmcDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AmcDetailMdl objmdl = new AmcDetailMdl();
                objmdl.AmcId = Convert.ToInt32(dr["AmcId"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.Qty = Convert.ToDouble(dr["Qty"].ToString());
                objmdl.Unit = Convert.ToInt32(dr["Unit"].ToString());//d
                objmdl.Rate = Convert.ToDouble(dr["Rate"].ToString());
                objmdl.Amount = Convert.ToDouble(dr["Amount"].ToString());
                objmdl.VatPer = Convert.ToDouble(dr["VatPer"].ToString());
                objmdl.VatAmount = Convert.ToDouble(dr["VatAmount"].ToString());
                objmdl.SatPer = Convert.ToDouble(dr["SatPer"].ToString());
                objmdl.SatAmount = Convert.ToDouble(dr["SatAmount"].ToString());
                objmdl.CstPer = Convert.ToDouble(dr["CstPer"].ToString());
                objmdl.CstAmount = Convert.ToDouble(dr["CstAmount"].ToString());
                objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.HsnCode = dr["HsnCode"].ToString();
                objmdl.ItemFor = dr["ItemFor"].ToString();
                objmdl.ItemForName = dr["ItemForName"].ToString();

                ledgers.Add(objmdl);
            }
            return ledgers;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(SqlCommand cmd, int rfqid, AmcDetailMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_insert_tbl_amcdetail";
            addCommandParameters(cmd, rfqid, dbobject);
            cmd.ExecuteNonQuery();
        }
        //
        internal void deleteAmcDetailLedger(SqlCommand cmd, int amcid)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_amcdetail";
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal AmcDetailMdl searchAmcDetailLedger(int amcid, int itemid)
        {
            DataSet ds = new DataSet();
            AmcDetailMdl dbobject = new AmcDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_amcdetail_item_info";
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
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
        internal DataSet getObjectData(int amcid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_amcdetail";
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AmcDetailMdl> getObjectList(int amcid)
        {
            DataSet ds = getObjectData(amcid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}