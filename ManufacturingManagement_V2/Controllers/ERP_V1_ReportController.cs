using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class ERP_V1_ReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        ERP_V1_ReportBLL bllObject = new ERP_V1_ReportBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        public ActionResult Index()//not-needed here
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = DateTime.Now;
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            return View(rptOption);
        }

        #region tender posting alert

        [HttpGet]
        public ActionResult sendTenderPostingReport(string strvalue = "")
        {
            //[100028]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DataSet ds = new DataSet();
            ds = bllObject.getTenderPostingReportData();
            if (ds.Tables.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            //
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";
            reportmatter += "<td valign='top'><b>Company</b></td>";
            reportmatter += "<td valign='top'><b>Railway</b></td>";
            reportmatter += "<td valign='top'><b>TenderNo</b></td>";
            reportmatter += "<td valign='top'><b>Item</b></td>";
            reportmatter += "<td align='right' valign='top'><b>Tender Qty</b></td>";
            reportmatter += "<td align='right' valign='top'><b>Our Qty</b></td>";
            reportmatter += "<td valign='top'><b>Unit</b></td>";
            reportmatter += "<td valign='top'><b>Uploading Date/<br>Time</b></td>";
            reportmatter += "<td valign='top'><b>Closing Date/<br>Time</b></td>";
            reportmatter += "<td valign='top'><b>Document<br/>Prepared</b></td>";
            reportmatter += "<td valign='top'><b>Remarks</b></td>";
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            string cldtstr = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                if (ds.Tables[0].Rows[i]["IsLapsed"].ToString() == "1")
                {
                    reportmatter += "<tr style='color:red;'>";
                }
                else
                {
                    reportmatter += "<tr>";
                }
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//sno
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["company"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["railwayname"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["tenderno"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["itemcode"].ToString() + "</td>";//item
                reportmatter += "<td align='right' valign='top'>" + ds.Tables[0].Rows[i]["qty"].ToString() + "</td>";//tenderqty
                reportmatter += "<td align='right' valign='top'>" + ds.Tables[0].Rows[i]["ourqty"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["unitname"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["UploadingDT"].ToString() + "</td>";//UploadingDT
                reportmatter += "<td valign='top'>" + cldtstr + "</td>";//Closing Date / Time
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["etdocprep"].ToString() + "</td>";//Doc.Prepared
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["remarks"].ToString() + "</td>";
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - Tender Posting</b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of tenders due in <b>next 48 hours</b>-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "<br/>Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(1, "PRAG ERP Tender Posting Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>"+emailresp+"</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region tender quoted/to be quoted report

        [HttpGet]
        public ActionResult sendTenderQuotedToBeQuotedReport(string strvalue="")
        {
            //[100031]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DataSet ds = new DataSet();
            ds = bllObject.getTenderQuotedToBeQuotedReportData();
            if (ds.Tables.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";
            reportmatter += "<td valign='top'><b>Status</b></td>";
            reportmatter += "<td valign='top'><b>Company</b></td>";
            reportmatter += "<td valign='top'><b>Railway</b></td>";
            reportmatter += "<td valign='top'><b>TenderNo</b></td>";
            reportmatter += "<td valign='top'><b>Item</b></td>";
            reportmatter += "<td align='right' valign='top'><b>Tender Qty</b></td>";
            reportmatter += "<td align='right' valign='top'><b>Our Qty</b></td>";
            reportmatter += "<td valign='top'><b>Unit</b></td>";
            reportmatter += "<td valign='top'><b>Uploading Date/<br>Time</b></td>";
            reportmatter += "<td valign='top'><b>Closing Date/<br>Time</b></td>";
            reportmatter += "<td valign='top'><b>Document<br/>Prepared</b></td>";
            reportmatter += "<td valign='top'><b>Remarks</b></td>";
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            string cldtstr = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//sno
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["tstatus"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["company"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["railwayname"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["tenderno"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["itemcode"].ToString() + "</td>";//item
                reportmatter += "<td align='right' valign='top'>" + ds.Tables[0].Rows[i]["qty"].ToString() + "</td>";//tenderqty
                reportmatter += "<td align='right' valign='top'>" + ds.Tables[0].Rows[i]["ourqty"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["unitname"].ToString() + "</td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["UploadingDT"].ToString() + "</td>";//UploadingDT
                reportmatter += "<td valign='top'>" + cldtstr + "</td>";//Closing Date / Time
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["etdocprep"].ToString() + "</td>";//Doc.Prepared
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["remarks"].ToString() + "</td>";
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - Tenders Quoted/To be Quoted</b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of tenders in <b>next 2 days</b> which are in status Quoted/To be Quoted-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "<br/>Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(2, "PRAG ERP Tender Status Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>" + emailresp + "</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region dispatch alert v2

        [HttpGet]
        public ActionResult sendDispatchAlertEmail(string strvalue="")
        {
            //[100004]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DataSet ds = new DataSet();
            ds = bllObject.getDispatchAlertV2(Convert.ToInt32(pms[1].ToString()));
            if (ds.Tables.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                return Content("<html><body style='font-family:verdana;font-size:10pt;'>No record found!</body></html>");
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";//0
            reportmatter += "<td valign='top'><b>Company</b></td>";//1
            reportmatter += "<td valign='top'><b>Item Code</b></td>";//2
            reportmatter += "<td valign='top'><b>Item</b></td>";//3
            reportmatter += "<td valign='top'><b>PO Number</b></td>";//4
            reportmatter += "<td valign='top'><b>PO Date</b></td>";//5
            reportmatter += "<td valign='top'><b>Contractual DP</b></td>";//6
            reportmatter += "<td valign='top'><b>Tentative DP</b></td>";//7
            reportmatter += "<td align='right' valign='top'><b>Balance Qty</b></td>";//8
            reportmatter += "<td align='right' valign='top'><b>Basic Value</b></td>";//9
            reportmatter += "<td valign='top'><b>Unit</b></td>";//10
            reportmatter += "<td valign='top'><b>Consignee</b></td>";//11
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//sno
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["company"].ToString() + "</td>";//1
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["ItemCode"].ToString() + "</td>";//2
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["item"].ToString() + "</td>";//3
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["PONumber"].ToString() + "</td>";//4
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["PODate"].ToString())) + "</td>";//5
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["DelvDate"].ToString())) + "</td>";//6
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["TentativeDP"].ToString())) + "</td>";//7
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["rqty"].ToString())) + "</td>";//8
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["basicvalue"].ToString())) + "</td>";//9
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["unitname"].ToString() + "</td>";//10
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[i]["consigneename"].ToString() + "</td>";//11
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - DPs in Next " + pms[1].ToString() + " Days </b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of <b>DPs in Next " + pms[1].ToString() + " Days</b> (for Railway+Private PO's only)-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "<br/>Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(1, "PRAG ERP DP Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>" + emailresp + "</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region material receipt alert

        private string getReportMatterStringDispNdUnloading(DateTime dtfrom,DateTime dtto,string compcode)
        {
            //[100006]
            string itemid = "0";
            bool filterbydt = true;
            //
            DataSet ds = new DataSet();
            ds = bllObject.getDispatchAndUnloadingDetail(itemid,filterbydt,dtfrom,dtto,compcode);
            //railway pos only & not-unloaded
            ds.Tables[0].DefaultView.RowFilter = "potype='t' and isunloaded=false";
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            if (dtr.Rows.Count == 0)
            {
                return "";
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>ChallanNo</b></td>";//0
            reportmatter += "<td valign='top'><b>Date</b></td>";//1
            reportmatter += "<td valign='top'><b>Consignee</b></td>";//2
            reportmatter += "<td valign='top'><b>CaseFileNo</b></td>";//3
            reportmatter += "<td valign='top'><b>Item</b></td>";//4
            reportmatter += "<td align='right' valign='top'><b>Dispatch<br/>Qty</b></td>";//5
            //reportmatter += "<td valign='top'><b>UnloadingDate</b></td>";//6
            //reportmatter += "<td valign='top'><b>UnloadingQty</b></td>";//7
            reportmatter += "<td valign='top'><b>R.C.Receiving<br/>Date</b></td>";//8
            reportmatter += "<td align='right' valign='top'><b>P.ReceivedP1/<br/>InvValue</b></td>";//9
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                //cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                //cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["challanno"].ToString() + "</td>";//0
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["vdate"].ToString())) + "</td>";//1
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["ConsigneeName"].ToString() + "</td>";//2
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["casefileno"].ToString() + "</td>";//3
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["DispItemCode"].ToString() + "</td>";//4
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["qty"].ToString())) + "</td>";//5
                //reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["unloadingdate"].ToString())) + "</td>";//6
                //reportmatter += "<td valign='top'>" + dtr.Rows[i]["delvqty"].ToString() + "</td>";//7
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["rcinfo"].ToString() + "</td>";//8
                if (mc.IsValidDouble(dtr.Rows[i]["BillP1RecInfo"].ToString()) == true)
                {
                    reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["BillP1RecInfo"].ToString())) + "</td>";//9
                }
                else
                {
                    reportmatter += "<td align='right' valign='top'>" + dtr.Rows[i]["BillP1RecInfo"].ToString() + "</td>";//9
                }
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            //
            return reportmatter;
        }

        [HttpGet]
        public ActionResult sendMaterialReceiptAlertReport(string strvalue="")
        {
            //[100006]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DateTime dtfrom = new DateTime(2016,1,1);//note
            DateTime dtto = DateTime.Now; ;// DateTime.Now.AddDays(-7);
            DataSet dscomp = new DataSet();
            dscomp = compBLL.getObjectData();
            string reportmatter = "<table><tr><td>";
            string rptstr = "";
            int cntr = 1;
            for (int i = 0; i < dscomp.Tables[0].Rows.Count; i++)
            {
                rptstr = getReportMatterStringDispNdUnloading(dtfrom, dtto, dscomp.Tables[0].Rows[i]["compcode"].ToString());
                if (rptstr.Length > 0)
                {
                    //reportmatter += "<tr><td colspan='9'><b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b></td></tr>";
                    //reportmatter += "<tr><td colspan='9'>&nbsp;</td></tr>";
                    reportmatter += "<b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b>";
                    reportmatter += rptstr+"<br/>";
                    cntr += 1;
                }
            }
            reportmatter += "</td></tr><table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - Material Receipt Status</b><br/> As on " + cdate + "<br/><br/>";
            //emailmsg += "Dear Sir,<br/>Here is the list of dispatches till " + mc.getStringByDate(dtto) + " <b>not updated that they are reached at destination (Railway POs Only)</b>-<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of dispatches <b>not updated that they are reached at destination (Railway POs Only)</b>-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(1, "PRAG ERP Material Receipt Status Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>" + emailresp + "</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region modify advise correction required alert

        private string getReportMatterStringCorrectionRequiredMAAlert(string compcode)
        {
            //[100011]
            DataSet ds = new DataSet();
            ds = bllObject.getCorrectionRequiredMAAlert(compcode);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            if (dtr.Rows.Count == 0)
            {
                return "";
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";//0
            reportmatter += "<td valign='top'><b>CaseFileNo</b></td>";//1
            reportmatter += "<td valign='top'><b>Railway</b></td>";//2
            reportmatter += "<td valign='top'><b>PO Number</b></td>";//3
            reportmatter += "<td valign='top'><b>PO Date</b></td>";//4
            reportmatter += "<td valign='top'><b>Item</b></td>";//5
            reportmatter += "<td valign='top'><b>Remarks</b></td>";//6
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                //cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                //cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i+1).ToString() + "</td>";//0
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["casefileno"].ToString() + "</td>";//1
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["railwayname"].ToString() + "</td>";//2
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["ponumber"].ToString() + "</td>";//3
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["podate"].ToString())) + "</td>";//4
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["dispitemcode"].ToString() + "</td>";//5
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["remarks"].ToString().Replace("\n","<br/>") + "</td>";//6
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            //
            return reportmatter;
        }

        [HttpGet]
        public ActionResult sendCorrectionRequiredMAAlertReport(string strvalue="")
        {
            //[100011]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DataSet dscomp = new DataSet();
            dscomp = compBLL.getObjectData();
            string reportmatter = "<table><tr><td>";
            string rptstr = "";
            int cntr = 1;
            for (int i = 0; i < dscomp.Tables[0].Rows.Count; i++)
            {
                rptstr = getReportMatterStringCorrectionRequiredMAAlert(dscomp.Tables[0].Rows[i]["compcode"].ToString());
                if (rptstr.Length > 0)
                {
                    //reportmatter += "<tr><td colspan='9'><b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b></td></tr>";
                    //reportmatter += "<tr><td colspan='9'>&nbsp;</td></tr>";
                    reportmatter += "<b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b>";
                    reportmatter += rptstr + "<br/>";
                    cntr += 1;
                }
            }
            reportmatter += "</td></tr><table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - Correction Required Purchase Orders</b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of Purchase Orders <b>with correction required</b>-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(1, "PRAG ERP MA Correction Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>" + emailresp + "</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region pending payment alert

        private string getReportMatterStringBillingReceiptAlertV2(string compcode)
        {
            //[Not In Use]
            DataSet ds = new DataSet();
            ds = bllObject.getBillingReceiptAlertV2(compcode);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            if (dtr.Rows.Count == 0)
            {
                return "";
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";//0
            reportmatter += "<td valign='top'><b>CaseFileNo</b></td>";//1
            reportmatter += "<td valign='top'><b>PO Number</b></td>";//2
            reportmatter += "<td valign='top'><b>PO Date</b></td>";//3
            reportmatter += "<td valign='top'><b>Item</b></td>";//4
            reportmatter += "<td valign='top'><b>Invoice No</b></td>";//5
            reportmatter += "<td valign='top'><b>Inv. Date</b></td>";//6
            reportmatter += "<td align='right' valign='top'><b>Pending Amount</b></td>";//7
            reportmatter += "<td align='right' valign='top'><b>Invoice Balance</b></td>";//8
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                //cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                //cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//0
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["casefileno"].ToString() + "</td>";//1
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["ponumber"].ToString() + "</td>";//2
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["podate"].ToString())) + "</td>";//3
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["dispitemcode"].ToString() + "</td>";//4
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["billno"].ToString() + "</td>";//5
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["vdate"].ToString())) + "</td>";//6
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["againstbill"].ToString())) + "</td>";//7
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["againstinvoice"].ToString())) + "</td>";//8
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            //
            return reportmatter;
        }

        private string getReportMatterStringBillingReceiptAlert(string compcode)
        {
            //[100016]
            DataSet ds = new DataSet();
            ds = bllObject.getBillingReceiptAlert(compcode);
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            if (dtr.Rows.Count == 0)
            {
                return "";
            }
            //--
            string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
            reportmatter += "<tr>";
            reportmatter += "<td valign='top'><b>S.No.</b></td>";//0
            reportmatter += "<td valign='top'><b>CaseFileNo</b></td>";//1
            reportmatter += "<td valign='top'><b>Paying Authority</b></td>";//2
            reportmatter += "<td valign='top'><b>PO Number</b></td>";//3
            reportmatter += "<td valign='top'><b>PO Date</b></td>";//4
            reportmatter += "<td valign='top'><b>Item</b></td>";//5
            reportmatter += "<td valign='top'><b>Invoice No</b></td>";//6
            reportmatter += "<td valign='top'><b>Inv. Date</b></td>";//7
            reportmatter += "<td align='right' valign='top'><b>Pending Amount</b></td>";//8
            reportmatter += "<td align='right' valign='top'><b>Invoice Balance</b></td>";//9
            reportmatter += "</tr>";
            DateTime cldt = DateTime.Now;
            for (int i = 0; i < dtr.Rows.Count; i++)
            {
                //cldt = Convert.ToDateTime(ds.Tables[0].Rows[i]["openingdt"].ToString());
                //cldtstr = mc.getStringByDate(cldt) + " " + cldt.ToShortTimeString();
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'>" + (i + 1).ToString() + "</td>";//0
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["casefileno"].ToString() + "</td>";//1
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["payingauthname"].ToString() + "</td>";//2
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["ponumber"].ToString() + "</td>";//3
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["podate"].ToString())) + "</td>";//4
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["dispitemcode"].ToString() + "</td>";//5
                reportmatter += "<td valign='top'>" + dtr.Rows[i]["billno"].ToString() + "</td>";//6
                reportmatter += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(dtr.Rows[i]["vdate"].ToString())) + "</td>";//7
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["recoverableamt"].ToString())) + "</td>";//8
                reportmatter += "<td align='right' valign='top'>" + mc.getINRCFormat(Convert.ToDouble(dtr.Rows[i]["InvBalance"].ToString())) + "</td>";//9
                reportmatter += "</tr>";
            }
            reportmatter += "</table>";
            //
            return reportmatter;
        }

        [HttpGet]
        public ActionResult sendBillingReceiptAlertReport(string strvalue="")
        {
            //[100016]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note --with download permission only
            if (mc.getPermission(Entry.Dashboard_Alerts, permissionType.Edit) == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string[] pms = strvalue.Split('/');
            //
            setViewData();
            DataSet dscomp = new DataSet();
            dscomp = compBLL.getObjectData();
            string reportmatter = "<table><tr><td>";
            string rptstr = "";
            int cntr = 1;
            for (int i = 0; i < dscomp.Tables[0].Rows.Count; i++)
            {
                //call v2 when ready
                rptstr = getReportMatterStringBillingReceiptAlert(dscomp.Tables[0].Rows[i]["compcode"].ToString());
                if (rptstr.Length > 0)
                {
                    //reportmatter += "<tr><td colspan='9'><b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b></td></tr>";
                    //reportmatter += "<tr><td colspan='9'>&nbsp;</td></tr>";
                    reportmatter += "<b> " + cntr.ToString() + " - " + dscomp.Tables[0].Rows[i]["cmpname"].ToString() + "</b>";
                    reportmatter += rptstr + "<br/>";
                    cntr += 1;
                }
            }
            reportmatter += "</td></tr><table>";
            string cdate = mc.getStringByDate(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString();
            string emailmsg = "";
            emailmsg = "<b>PRAG ERP Alert - Pending Payment</b><br/> As on " + cdate + "<br/><br/>";
            emailmsg += "Dear Sir,<br/>Here is the list of Pending Payments due from 60 days (by Invoice Date) with value above 10 Lacs</b>-<br/><br/>";
            emailmsg += reportmatter;
            emailmsg += "Regards:<br/>PRAG ERP System";
            //
            string emailresp = "";
            if (pms[0] == "1")
            {
                emailresp = mc.SendAlertsOnEmail(1, "PRAG ERP Pending Payment Alert", emailmsg);
                emailresp = "<span style='color:white;background-color:black;font-size:12pt;'>" + emailresp + "</span>";
            }
            if (emailresp.Length > 0) { emailmsg = emailresp + "<br/><br/>" + emailmsg; };
            return Content("<html><body style='font-family:verdana;font-size:10pt;'>" + emailmsg + "</body></html>");
        }

        #endregion

        #region agentwise pending payment report

        [HttpGet]
        public ActionResult AgentwisePaymentReport(string strvalue = "")
        {
            //[100057]/F1
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getAgentwisePaymentReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAgentwisePaymentReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getAgentwisePaymentReport(string strvalue = "")
        {
            //[100057]/F1
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwisePaymentDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Pending Payments";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For "+hdr.Substring(0,hdr.Length-2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_billing_receipt_agentwise_report 
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult AgentwisePaymentConsigneeGroup(string strvalue = "")
        {
            //[100057]/F2
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getAgentwisePaymentConsigneeGroupReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAgentwisePaymentConsigneeGroupReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getAgentwisePaymentConsigneeGroupReport(string strvalue = "")
        {
            //[100057]/F2
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwisePaymentConsigneeGrpDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Pending Payments (Consignee Group)";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_billing_receipt_agentwise_report
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult AgentwisePaymentForAgentCityGroup(string strvalue = "")
        {
            //[100057]/F3
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getAgentwisePaymentForAgentCityGroup";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAgentwisePaymentForAgentCityGroup", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getAgentwisePaymentForAgentCityGroup(string strvalue = "")
        {
            //[100057]/F3
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwisePaymentForAgentCityGrpDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_billing_receipt_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Pending Payments (Citywise Group)";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_billing_receipt_agentwise_report
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region agentwise correction required po report

        [HttpGet]
        public ActionResult AgentwiseCorrectionRequiredReport(string strvalue = "")
        {
            //[100058]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getAgentwiseCorrectionRequiredReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAgentwiseCorrectionRequiredReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getAgentwiseCorrectionRequiredReport(string strvalue = "")
        {
            //[100058]
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_correction_required_po_agentwise_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwiseCorrectionRequiredDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Correction Required PO's ";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region agentwise pending rc report

        [HttpGet]
        public ActionResult ReceiptedChallanPendingAgentwiseReport(string strvalue = "")
        {
            //[100059]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getReceiptedChallanPendingAgentwiseReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getReceiptedChallanPendingAgentwiseReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getReceiptedChallanPendingAgentwiseReport(string strvalue = "")
        {
            //[100059]
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_receipted_challan_pending_agentwise_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwiseRChallanPendingDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Pending R.C. Report";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region agentwise pending rnote report

        [HttpGet]
        public ActionResult ReceiptNotePendingAgentwiseReport(string strvalue = "")
        {
            //[100060]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getReceiptNotePendingAgentwiseReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getReceiptNotePendingAgentwiseReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getReceiptNotePendingAgentwiseReport(string strvalue = "")
        {
            //[100060]
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_receipt_note_pending_agentwise_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwiseRNotePendingDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Pending RO-II Report";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region agentwise purchase order report

        [HttpGet]
        public ActionResult PurchaseOrderAgentwiseReport(string strvalue = "")
        {
            //[100061]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getPurchaseOrderAgentwiseReport";
                string reportpms = "strvalue=" + strvalue + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPurchaseOrderAgentwiseReport", new { strvalue = strvalue });
        }

        [HttpGet]
        public FileResult getPurchaseOrderAgentwiseReport(string strvalue = "")
        {
            //[100061]
            //0=cdtstmp, 1=agent, 2=railway, 3=comapny & 4/5/6 for the names respectively
            string[] pms = strvalue.Split('/');
            //
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_purchase_order_agentwise_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentwisePurchaseOrderDsbRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string hdr = "";
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            if (pms[1].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.agentid}=" + pms[1] + "";
                hdr += " Agent: " + pms[4].ToString() + ", ";
            }
            if (pms[2].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.railwayid}=" + pms[2] + "";
                hdr += " Railway: " + pms[5].ToString() + ", ";
            }
            if (pms[3].ToString() != "0")
            {
                //rptDoc.RecordSelectionFormula += " and {usp_correction_required_po_agentwise_report;1.compcode}=" + pms[3] + "";
                hdr += " Company: " + pms[6].ToString() + ", ";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Purchase Order Report";
            if (hdr.Length > 0)
            {
                txtrpthead.Text += " For " + hdr.Substring(0, hdr.Length - 2);
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@agentid", Convert.ToInt32(pms[1].ToString()));
            rptDoc.SetParameterValue("@railwayid", Convert.ToInt32(pms[2].ToString()));
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(pms[3].ToString()));
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region orderwise payment report

        [HttpGet]
        public ActionResult OrderwisePaymentReport(string strvalue = "")
        {
            //[100053]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string[] pms = strvalue.Split('/');//1=porderid
            //
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getOrderwisePaymentReport";
                string reportpms = "porderid=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderwisePaymentReport", new { porderid = pms[1].ToString() });
        }

        [HttpGet]
        public ActionResult OrderwisePaymentReportLink(int porderid = 0)
        {
            //[100053]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getOrderwisePaymentReport";
                string reportpms = "porderid=" + porderid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderwisePaymentReport", new { porderid = porderid });
        }

        [HttpGet]
        public FileResult getOrderwisePaymentReport(int porderid = 0)
        {
            //[100053]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderwisePaymentMktg.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_billing_receipt_detail_purchase_order_wise
            rptDoc.SetParameterValue("@porderid", porderid);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult OrderwisePaymentReportV2(string strvalue = "")
        {
            //[Not In Use]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string[] pms = strvalue.Split('/');//1=porderid
            //
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getOrderwisePaymentReportV2";
                string reportpms = "porderid=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderwisePaymentReportV2", new { porderid = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getOrderwisePaymentReportV2(int porderid = 0)
        {
            //[Not In Use]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_purchase_order_agentwise_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderwisePaymentMktg.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_billing_receipt_detail_purchase_order_wise
            rptDoc.SetParameterValue("@porderid", porderid);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region pending security deposit amount report 

        //note: this is to be prepared by main billingndpaymentreportV2 by head wise filter
        //and to be removed form here

        [HttpGet]
        public ActionResult PendingSecurityAmountReport(string strvalue = "")
        {
            //[100026]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string[] pms = strvalue.Split('/');//0=status,1=compcode
            //
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getPendingSecurityAmountReport";
                string reportpms = "status=" + pms[0].ToString() + "";
                reportpms += "&compcode=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPendingSecurityAmountReport", new { status=pms[0].ToString(), compcode = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getPendingSecurityAmountReport(string status = "p", int compcode = 0)
        {
            //[100026]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PendingSecurityAmountMktg.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula --does not works when 
            //rpt is by stored procedure, it must have atleast one parameter
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptTitle"];
            txtRptTitle.Text = "Security Deposit Report";
            if (status.ToLower() == "p")
            {
                txtRptTitle.Text += " : Pending";
            }
            else if (status.ToLower() == "r")
            {
                txtRptTitle.Text += " : Received";
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters-- usp_pending_security_deposit_report
            rptDoc.SetParameterValue("@status", status);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region monthly sale report

        [HttpGet]
        public ActionResult SaleReportMonthly(string strvalue = "")
        {
            //[100025]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=invoicemode, 3=comapny
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSaleReportMonthly";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                reportpms += "&invoicemode=" + pms[2].ToString() + "";
                reportpms += "&compcode=" + pms[3].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleReportMonthly", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString(), invoicemode = pms[2].ToString(), compcode = pms[3].ToString() });
        }

        [HttpGet]
        public FileResult getSaleReportMonthly(DateTime dtfrom,DateTime dtto,string invoicemode,int compcode)
        {
            //[100025]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //usp_get_monthly_sale_report
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportMonthlyRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if(invoicemode != "0")
            {
                txtrpthead.Text += ", Mode: "+invoicemode.ToUpper();
            }
            txtrpthead.Text += " (*)";
            //setLoginInfo(rptDocSub);
            //dbp parameters- usp_get_monthly_sale_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@invoicemode", invoicemode);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region sales summary report -companywise

        [HttpGet]
        public ActionResult CompanywiseSalesSummaryReport(string strvalue = "")
        {
            //[100032]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getCompanywiseSalesSummary";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getCompanywiseSalesSummary", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getCompanywiseSalesSummary(DateTime dtfrom, DateTime dtto)
        {
            //[100032]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportCompanywiseSummary.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " for Railway + Private PO, GST";
            //setLoginInfo(rptDocSub);
            //dbp parameters-- usp_get_company_wise_sale_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region sales payment balance summary report

        [HttpGet]
        public ActionResult SalesPaymentBalanceSummaryReport(string strvalue = "")
        {
            //[100147]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSalesPaymentBalanceSummary";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalesPaymentBalanceSummary", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getSalesPaymentBalanceSummary(DateTime dtfrom, DateTime dtto)
        {
            //[100147]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SalesPaymentBalanceSummaryRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Invoice Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " for Railway + Private PO";
            //setLoginInfo(rptDocSub);
            //dbp parameters
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region sales payment receipt summary report

        [HttpGet]
        public ActionResult SalesPaymentReceiptSummaryReport(string strvalue = "")
        {
            //[100148]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Billing_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Billing_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSalesPaymentReceiptSummary";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSalesPaymentReceiptSummary", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getSalesPaymentReceiptSummary(DateTime dtfrom, DateTime dtto)
        {
            //[100148]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SalesPaymentReceiptSummaryRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Receipt Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + " for Railway + Private PO";
            //setLoginInfo(rptDocSub);
            //dbp parameters
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region sales summary- quarterwise

        [HttpGet]
        public ActionResult FinYearQuarterwiseSaleSummary(string strvalue = "")
        {
            //[100018]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=finyear, 1=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getFinYearQuarterwiseSaleSummary";
                string reportpms = "finyear=" + pms[0].ToString() + "";
                reportpms += "compcode=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFinYearQuarterwiseSaleSummary", new { finyear = pms[0].ToString(), compcode = pms[1].ToString() });
        }

        [HttpGet]
        public FileResult getFinYearQuarterwiseSaleSummary(string finyear, int compcode=0)
        {
            //[100018]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportFinYearQuarterwiseSummary.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Financial Year " + finyear + " for Railway+Private PO, GST+EDS";
            //setLoginInfo(rptDocSub);
            //dbp parameters-- usp_get_quarter_wise_sale_report
            rptDoc.SetParameterValue("@finyear", finyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region sales summary- 5 years finyear wise report

        [HttpGet]
        public ActionResult FinYearwiseSummaryReport(string strvalue = "")
        {
            //[100007]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getFinYearwiseSalesSummary";
                string reportpms = "finyear=" + pms[0].ToString() + "";
                //reportpms += "&dtto=" + pms[1].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getFinYearwiseSalesSummary", new { finyear = pms[0].ToString() });
        }

        [HttpGet]
        public FileResult getFinYearwiseSalesSummary(string finyear)
        {
            //[100007]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportFinYearwiseSummary.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Current Financial Year " + finyear + " for Railway+Private PO, GST+EDS";
            //setLoginInfo(rptDocSub);
            //dbp parameters-- usp_get_finyear_wise_sale_report
            rptDoc.SetParameterValue("@finyear", finyear);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region monthly sale report- groupwise

        [HttpGet]
        public ActionResult SaleReportMonthlyGroupwise(string strvalue = "")
        {
            //[100008]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=invoicemode, 3=comapny, 4=groupid, 5=potype, 6=groupname, 7=potypename,
            //8=itemid,9=itemcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            int itemid = 0;
            if (pms[8].ToString().ToLower() != "all")
            {
                itemid = Convert.ToInt32(pms[8].ToString());
            }
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSaleReportMonthlyGroupwise";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                reportpms += "&invoicemode=" + pms[2].ToString() + "";
                reportpms += "&compcode=" + pms[3].ToString() + "";
                reportpms += "&groupid=" + pms[4].ToString() + "";
                reportpms += "&potype=" + pms[5].ToString() + "";
                reportpms += "&groupname=" + pms[6].ToString() + "";
                reportpms += "&potypename=" + pms[7].ToString() + "";
                reportpms += "&itemid=" + itemid + "";
                reportpms += "&itemcode=" + pms[9].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleReportMonthlyGroupwise", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString(), invoicemode = pms[2].ToString(), compcode = pms[3].ToString(), groupid = pms[4].ToString(), potype = pms[5].ToString(), groupname = pms[6].ToString(), potypename = pms[7].ToString(), itemid = itemid, itemcode = pms[9].ToString() });
        }

        [HttpGet]
        public FileResult getSaleReportMonthlyGroupwise(DateTime dtfrom, DateTime dtto, string invoicemode, int compcode, int groupid, string potype, string groupname,string potypename, int itemid, string itemcode)
        {
            //[100008]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportMonthlyGroupwiseRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (potype != "0")
            {
                txtrpthead.Text += ", PO Type: " + potypename;
            }
            if (groupid != 0)
            {
                txtrpthead.Text += ", Item Group: " + groupname;
            }
            if (invoicemode != "0")
            {
                txtrpthead.Text += ", Mode: " + invoicemode.ToUpper();
            }
            if (itemid != 0)
            {
                txtrpthead.Text += ", Item: " + itemcode;
            }
            //else
            //{
            //    rptDoc.ReportDefinition.Sections["ReportFooterSection2"].SectionFormat.EnableSuppress = true;
            //}
            //setLoginInfo(rptDocSub);
            //dbp parameters- usp_get_monthly_sale_report_groupwise
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@invoicemode", invoicemode);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@POType", potype);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region monthly summary sale report

        [HttpGet]
        public ActionResult SaleReportMonthwiseSummary(string strvalue = "")
        {
            //[100012]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSaleReportMonthwiseSummary";
                string reportpms = "invoicemode=" + pms[0].ToString() + "";
                reportpms += "&groupid=" + pms[1].ToString() + "";
                reportpms += "&potype=" + pms[2].ToString() + "";
                reportpms += "&groupname=" + pms[3].ToString() + "";
                reportpms += "&potypename=" + pms[4].ToString() + "";
                reportpms += "&itemid=" + pms[5].ToString() + "";
                reportpms += "&itemcode=" + pms[6].ToString() + "";
                reportpms += "&finyear=" + pms[7].ToString() + "";
                reportpms += "&compcode=" + pms[8].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleReportMonthwiseSummary", new { invoicemode = pms[0].ToString(), groupid = pms[1].ToString(), potype = pms[2].ToString(), groupname = pms[3].ToString(), potypename = pms[4].ToString(), itemid = pms[5].ToString(), itemcode = pms[6].ToString(), finyear = pms[7].ToString(), compcode = pms[8].ToString() });
        }

        [HttpGet]
        public FileResult getSaleReportMonthwiseSummary(string invoicemode, int groupid, string potype, string groupname, string potypename, int itemid, string itemcode, string finyear, int compcode)
        {
            //[100012]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "SaleReportMonthwiseSummary.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Financial Year " + finyear;
            if (potype != "0")
            {
                txtrpthead.Text += ", PO Type: " + potypename;
            }
            if (groupid != 0)
            {
                txtrpthead.Text += ", Item Group: " + groupname;
            }
            if (invoicemode != "0")
            {
                txtrpthead.Text += ", Mode: " + invoicemode.ToUpper();
            }
            if (itemid != 0)
            {
                txtrpthead.Text += ", Item: " + itemcode;
            }
            //setLoginInfo(rptDocSub);
            //dbp parameters- usp_get_companywise_monthly_sale_summary
            rptDoc.SetParameterValue("@invoicemode", invoicemode);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@POType", potype);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@finyear", finyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region gst-deduction report

        [HttpGet]
        public ActionResult GSTDeductionReport(string strvalue = "")
        {
            //[100009]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //0=dtfrom, 1=dtto, 2=compcode
            string[] pms = strvalue.Split('/');
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getGstDeductionReportFile";
                string reportpms = "dtfrom=" + pms[0].ToString() + "";
                reportpms += "&dtto=" + pms[1].ToString() + "";
                reportpms += "&compcode=" + pms[3].ToString() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getGstDeductionReportFile", new { dtfrom = pms[0].ToString(), dtto = pms[1].ToString(), compcode = pms[2].ToString()});
        }

        [HttpGet]
        public FileResult getGstDeductionReportFile(DateTime dtfrom, DateTime dtto, int compcode)
        {
            //[100009]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "GSTDeductionRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            //setLoginInfo(rptDocSub);
            //dbp parameters-- usp_get_gst_deduction_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        private void setLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
        {
            DataTable lginfo = mc.getCrptLoginInfo();
            CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
            CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();
            CrystalDecisions.Shared.ConnectionInfo crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
            crConnectionInfo.ServerName = lginfo.Rows[0]["svrname"].ToString();
            crConnectionInfo.DatabaseName = lginfo.Rows[0]["dbname"].ToString();
            crConnectionInfo.UserID = lginfo.Rows[0]["userid"].ToString();
            crConnectionInfo.Password = lginfo.Rows[0]["passw"].ToString();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables = rptDoc.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }

    }
}
