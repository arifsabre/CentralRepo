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
    public class BillOsBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, string vtype, string vno, DateTime vdate, BillOsMdl objbillos)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vdate", mc.getStringByDateToStore(vdate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", objbillos.AcCode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@billtype", objbillos.BillType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billno", objbillos.BillNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billamt", objbillos.BillAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@billdate", mc.getStringByDateToStore(objbillos.BillDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@duedate", mc.getStringByDateToStore(objbillos.DueDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dramt", objbillos.DrAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@cramt", objbillos.CrAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@drcr", objbillos.DrCr.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@narration", objbillos.Narration.Trim(), DbType.String));
        }
        ////
        #endregion
        //
        #region dml objects
        //
        internal void insertBillOSReceipt(SqlCommand cmd, string vtype, string vno, DateTime vdate, BillOsMdl[] objbillos)
        {
            for (int i = 0; i < objbillos.Length; i++)
            {
                if (Convert.ToDouble(objbillos[i].DrAmount) + Convert.ToDouble(objbillos[i].CrAmount) != 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_billosr";
                    addCommandParameters(cmd, vtype, vno, vdate, objbillos[i]);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //
        internal void insertBillOSPayment(SqlCommand cmd, string vtype, string vno, DateTime vdate, BillOsMdl[] objbillos)
        {
            for (int i = 0; i < objbillos.Length; i++)
            {
                if (Convert.ToDouble(objbillos[i].DrAmount) + Convert.ToDouble(objbillos[i].CrAmount) != 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_billosp";
                    addCommandParameters(cmd, vtype, vno, vdate, objbillos[i]);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //
        public void insertBillOS(billOSType billostype, SqlCommand cmd, string vtype, string vno, DateTime vdate, BillOsMdl[] objbillos)
        {
            if (objbillos == null) { return; };
            if (billostype == billOSType.Receipt)
            {
                insertBillOSReceipt(cmd, vtype, vno, vdate, objbillos);
            }
            else if (billostype == billOSType.Payment)
            {
                insertBillOSPayment(cmd, vtype, vno, vdate, objbillos);
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataSet getBillOSReceipt(string vtype, string vno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_billosr";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillOSPayment(string vtype, string vno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_billosp";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillOSReceiptForOpening(string accode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_billosr_for_opening";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillOSPaymentForOpening(string accode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_billosp_for_opening";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillListReceipt(string accode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_billlist_by_billosr";//usp_display_bill_list
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillListPayment(string accode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_billlist_by_billosp";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        public DataSet getBillOS(string vtype, string vno, billOSType billostype)
        {
            DataSet ds = new DataSet();
            if (billostype == billOSType.Receipt)
            {
                ds = getBillOSReceipt(vtype, vno);
            }
            else if (billostype == billOSType.Payment)
            {
                ds = getBillOSPayment(vtype, vno);
            }
            return ds;
        }
        //
        public DataSet getBillOSReceiptForOpening(string accode, billOSType billostype)
        {
            DataSet ds = new DataSet();
            if (billostype == billOSType.Receipt)
            {
                ds = getBillOSReceiptForOpening(accode);
            }
            else if (billostype == billOSType.Payment)
            {
                ds = getBillOSPaymentForOpening(accode);
            }
            return ds;
        }
        //
        public DataSet getBillList(string accode, billOSType billostype)
        {
            DataSet ds = new DataSet();
            if (billostype == billOSType.Receipt)
            {
                ds = getBillListReceipt(accode);
            }
            else if (billostype == billOSType.Payment)
            {
                ds = getBillListPayment(accode);
            }
            return ds;
        }
        //
        //new 
        internal void deleteBillOS(SqlCommand cmd, string vtype, string vno)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_billOs";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        internal void insertBillOS(SqlCommand cmd, BillOsMdl objbillos)
        {
            cmd.Parameters.Clear();
            //instead of usp_insert_tbl_billos
            cmd.CommandText = "usp_insert_billos_entry";
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vtype", objbillos.VType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", objbillos.VNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(objbillos.VDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", objbillos.AcCode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@billtype", objbillos.BillType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billno", objbillos.BillNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@billamt", objbillos.BillAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@billdate", mc.getStringByDateToStore(objbillos.BillDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@duedate", mc.getStringByDateToStore(objbillos.DueDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dramt", objbillos.DrAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@cramt", objbillos.CrAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@drcr", objbillos.DrCr.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@narration", objbillos.Narration.Trim(), DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        internal DataSet getBillOS_New(string vtype, string vno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_display_billos";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<BillOsMdl> getBillOSList(string vtype, string vno)
        {
            DataSet ds = new DataSet();
            ds = getBillOS_New(vtype, vno);
            List<BillOsMdl> objlist = new List<BillOsMdl> { };
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        BillOsMdl objmdl = new BillOsMdl();
                        objmdl.VType = dr["vtype"].ToString();
                        objmdl.VNo = dr["vno"].ToString();
                        objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                        objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                        objmdl.AcDesc = dr["acdesc"].ToString();
                        objmdl.BillType = dr["billtype"].ToString();
                        objmdl.BillNo = dr["billno"].ToString();
                        objmdl.BillAmount = Convert.ToDouble(dr["billamt"].ToString());
                        objmdl.BillDate = mc.getStringByDate(Convert.ToDateTime(dr["BillDate"].ToString()));
                        objmdl.DueDate = mc.getStringByDate(Convert.ToDateTime(dr["DueDate"].ToString()));
                        objmdl.DrAmount = Convert.ToDouble(dr["dramt"].ToString());
                        objmdl.CrAmount = Convert.ToDouble(dr["cramt"].ToString());
                        objmdl.DrCr = dr["drcr"].ToString();
                        objmdl.Narration = dr["narration"].ToString();
                        objlist.Add(objmdl);
                    }
                }
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}