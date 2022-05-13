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
    public class BillOsRptBLL : DbContext
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
        private void fillDtRptReceipt(DataTable dtrpt, DataTable dtbillos)
        {
            dtrpt.Rows.Clear();
            DataRow dr = dtrpt.NewRow();
            int x = 1;
            for (int i = 0; i < dtbillos.Rows.Count; i++)
            {
                if (Convert.ToDouble(dtbillos.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtbillos.Rows[i]["cramt"].ToString()) != 0)
                {
                    dr = dtrpt.NewRow();
                    dr["slno"] = x.ToString();
                    dr["accode"] = dtbillos.Rows[i]["accode"].ToString();
                    dr["acdesc"] = dtbillos.Rows[i]["acdesc"].ToString();
                    dr["billno"] = dtbillos.Rows[i]["billno"].ToString();
                    dr["billdate"] = dtbillos.Rows[i]["billdate"].ToString();
                    dr["billamount"] = dtbillos.Rows[i]["dramt"].ToString();
                    dr["received"] = dtbillos.Rows[i]["cramt"].ToString();
                    dr["balance"] = Convert.ToDouble(dtbillos.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtbillos.Rows[i]["cramt"].ToString());
                    //dr["orderno"] = dtbillos.Rows[i]["orderno"].ToString();
                    //dr["orderdate"] = dtbillos.Rows[i]["orderdate"].ToString();
                    dr["narration"] = dtbillos.Rows[i]["narration"].ToString();
                    dr["remarks"] = "";
                    dtrpt.Rows.Add(dr);
                    x += 1;
                }
            }
        }
        //
        private void fillDtRptPayment(DataTable dtrpt, DataTable dtbillos)
        {
            dtrpt.Rows.Clear();
            DataRow dr = dtrpt.NewRow();
            int x = 1;
            for (int i = 0; i < dtbillos.Rows.Count; i++)
            {
                if (Convert.ToDouble(dtbillos.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtbillos.Rows[i]["cramt"].ToString()) != 0)
                {
                    dr = dtrpt.NewRow();
                    dr["slno"] = x.ToString();
                    dr["accode"] = dtbillos.Rows[i]["accode"].ToString();
                    dr["acdesc"] = dtbillos.Rows[i]["acdesc"].ToString();
                    dr["billno"] = dtbillos.Rows[i]["billno"].ToString();
                    dr["billdate"] = dtbillos.Rows[i]["billdate"].ToString();
                    dr["billamount"] = dtbillos.Rows[i]["dramt"].ToString();
                    dr["received"] = dtbillos.Rows[i]["cramt"].ToString();
                    dr["balance"] = Convert.ToDouble(dtbillos.Rows[i]["dramt"].ToString()) - Convert.ToDouble(dtbillos.Rows[i]["cramt"].ToString());
                    //dr["orderno"] = dtbillos.Rows[i]["orderno"].ToString();
                    //dr["orderdate"] = dtbillos.Rows[i]["orderdate"].ToString();
                    dr["narration"] = dtbillos.Rows[i]["narration"].ToString();
                    dr["remarks"] = "";
                    dtrpt.Rows.Add(dr);
                    x += 1;
                }
            }
        }
        //
        private void prepareBillOSReportReceipt(DataTable dtrpt, string accode, string finyear, DateTime vdate, int ccode)
        {
            DataTable dt = new DataTable();
            dt = prepareTempBillOSReceipt(accode, finyear, objCookie.getFromDate(), vdate, "billos", ccode);
            fillDtRptReceipt(dtrpt, dt);
        }
        //
        private void prepareBillOSReportPayment(DataTable dtrpt, string accode, string finyear, DateTime vdate, int ccode)
        {
            DataTable dt = new DataTable();
            dt = prepareTempBillOSPayment(accode, finyear, objCookie.getFromDate(), vdate, "billos", ccode);
            fillDtRptPayment(dtrpt, dt);
        }
        //
        private void prepareBillOSCollectionReportReceipt(DataTable dtrpt, string accode, DateTime dtfrom, DateTime dtto, int ccode)
        {
            DataTable dt = new DataTable();
            dt = prepareTempBillOSReceipt(accode, objCookie.getFinYear(), dtfrom, dtto, "collection", ccode);
            fillDtRptReceipt(dtrpt, dt);
        }
        //
        private void prepareBillOSCollectionReportPayment(DataTable dtrpt, string accode, DateTime dtfrom, DateTime dtto, int ccode)
        {
            DataTable dt = new DataTable();
            dt = prepareTempBillOSPayment(accode, objCookie.getFinYear(), dtfrom, dtto, "collection", ccode);
            fillDtRptPayment(dtrpt, dt);
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal DataTable prepareTempBillOSReceipt(string accode, string finyear, DateTime dtfrom, DateTime dtto, string rtype, int ccode)
        {
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
                cmd.CommandTimeout = 0;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_temp_billos";
                cmd.ExecuteNonQuery();
                if (rtype == "billos")
                {
                    for (int x = 1; x <= 7; x++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_insert_tbl_temp_billosr_p" + x.ToString();
                        cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                        cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Parameters.Clear();
                }
                else if (rtype == "collection")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_temp_billosr_collection";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_temp_billosr_p7";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                    cmd.ExecuteNonQuery();
                }
                if (rtype == "billos")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_temp_billosr";
                }
                else if (rtype == "collection")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_billosr_collection_report";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                }
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BillOsRptBLL", "prepareTempBillOSReceipt", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ds.Tables[0];
        }
        //
        internal DataTable prepareTempBillOSPayment(string accode, string finyear, DateTime dtfrom, DateTime dtto, string rtype, int ccode)
        {
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
                cmd.CommandTimeout = 0;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_temp_billos";
                cmd.ExecuteNonQuery();
                if (rtype == "billos")
                {
                    for (int x = 1; x <= 7; x++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_insert_tbl_temp_billosp_p" + x.ToString();
                        cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                        cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                        cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Parameters.Clear();
                }
                else if (rtype == "collection")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_temp_billosp_collection";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_temp_billosp_p7";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                    cmd.ExecuteNonQuery();
                }
                if (rtype == "billos")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_temp_billosp";
                }
                else if (rtype == "collection")
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_billosp_collection_report";
                    cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                }
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("BillOsRptBLL", "prepareTempBillOSPayment", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ds.Tables[0];
        }
        //
        #endregion
        //
        #region fetching objects
        //
        public void prepareBillOSReport(billOSType billostype, DataTable dtrpt, string accode, string finyear, DateTime vdate, int ccode)
        {
            if (billostype == billOSType.Receipt)
            {
                prepareBillOSReportReceipt(dtrpt, accode, finyear, vdate, ccode);
            }
            else if (billostype == billOSType.Payment)
            {
                prepareBillOSReportPayment(dtrpt, accode, finyear, vdate, ccode);
            }
        }
        //
        public void prepareBillOSCollectionReport(billOSType billostype, DataTable dtrpt, string accode, DateTime dtfrom, DateTime dtto, int ccode)
        {
            if (billostype == billOSType.Receipt)
            {
                prepareBillOSCollectionReportReceipt(dtrpt, accode, dtfrom, dtto, ccode);
            }
            else if (billostype == billOSType.Payment)
            {
                prepareBillOSCollectionReportPayment(dtrpt, accode, dtfrom, dtto, ccode);
            }
        }
        //
        #endregion
        //
    }
}