using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ERP_V1_ReportBLL
    {
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region fetching objects
        //
        internal DataSet getTenderPostingReportData()
        {
            //[100028]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tender_posting_report";
            cmd.Parameters.Add(mc.getPObject("@cmpdate", DateTime.Now, DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getTenderQuotedToBeQuotedReportData()
        {
            //[100031]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tender_quoted_to_bequoted_report";
            cmd.Parameters.Add(mc.getPObject("@cmpdate", DateTime.Now, DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getDispatchAndUnloadingDetail(string itemid, bool filterbydt, DateTime dtfrom, DateTime dtto, string compcode)
        {
            //[100006]
            //from erpv1/MISRptDAL
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_rpt_dispatch_unloading_detail";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@filterbydt", filterbydt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getDispatchAlertV2(int ndays)
        {
            //[100004]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_dispatch_alert";
            cmd.Parameters.Add(mc.getPObject("@ndays", ndays, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrectionRequiredMAAlert(string compcode)
        {
            //[100011]
            //from erpv1/misrpt-dbp 
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_correction_required_modify_advise_alert";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillingReceiptAlert(string compcode)
        {
            //[100016]
            //from erpv1/misrpt-dbp 
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_billing_receipt_alert";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillingReceiptAlertV2(string compcode)
        {
            //[Not In Use]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_billing_receipt_alert_v2";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getRailwayData(string rtype = "0")
        {
            //form v1
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_get_tbl_railway_by_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_get_tbl_railway";
            }
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetConsigneeData(int railwayid)
        {
            //form v1
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_fill_consignee_search_list";
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PartyMdl> getConsigneeList(int railwayid = 0)
        {
            DataSet ds = new DataSet();
            ds = GetConsigneeData(railwayid);
            List<PartyMdl> objlist = new List<PartyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PartyMdl objmdl = new PartyMdl();
                objmdl.AcCode = Convert.ToInt32(dr["ConsigneeId"].ToString());
                objmdl.AcDesc = dr["ConsigneeName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        internal DataSet getPayingAuthorityData(int railwayid = 0, string rtype = "0")
        {
            //form v1
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_fill_payingauthority_search_list_by_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_fill_payingauthority_search_list";
            }
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PartyMdl> getPayingAuthorityList(int railwayid = 0, string rtype = "0")
        {
            DataSet ds = new DataSet();
            ds = getPayingAuthorityData(railwayid,rtype);
            List<PartyMdl> objlist = new List<PartyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PartyMdl objmdl = new PartyMdl();
                objmdl.AcCode = Convert.ToInt32(dr["PayingAuthId"].ToString());
                objmdl.AcDesc = dr["PayingAuthName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal List<RailwayMdl> getRailwayList(string rtype="0")
        {
            DataSet ds = getRailwayData(rtype);
            List<RailwayMdl> railways = new List<RailwayMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RailwayMdl objmdl = new RailwayMdl();
                objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                objmdl.RailwayName = dr["RailwayName"].ToString();
                railways.Add(objmdl);
            }
            return railways;
        }
        //
        internal DataSet getAgentData()
        {
            //form v1
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_agent";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getTenderReportV2(DateTime dtfrom,DateTime dtto,int groupid,int itemid,int railwayid,string tenderstatus,int compcode)
        {
            //[100030]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tender_report_v2";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@tenderstatus", tenderstatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPendingPaymentReportCSVData(string potype, DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            //[100110]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_rpt_pending_payment_report";//to be changed for billos
            cmd.Parameters.Add(mc.getPObject("@potype", potype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetPendingPaymentReportHtml(string potype, DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            //[100110]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_rpt_pending_payment_report_html";
            cmd.Parameters.Add(mc.getPObject("@potype", potype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPaymentReceiptReportCSVData(DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            //[100015]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_payment_receipt_report";//to be changed for billos
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetPurchaseOrderReceivingReportHtml(DateTime dtfrom, DateTime dtto, int railwayid, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_purchaseorder_receiving_report_html";//to be changed for billos
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetQuotedTendersForSAReportL1Html(DateTime dtfrom, DateTime dtto, int compcode, string finyear, bool applydtr)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quoted_tenders_for_sa_report_l1_html";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@applydtr", applydtr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet GetQuotedTendersForSAReportL2Html(DateTime dtfrom, DateTime dtto, int compcode, string finyear, bool applydtr)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_quoted_tenders_for_sa_report_l2_html";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@applydtr", applydtr, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}