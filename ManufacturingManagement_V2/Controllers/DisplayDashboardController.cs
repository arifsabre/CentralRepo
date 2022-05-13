using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ManufacturingManagement_V2.Models;
using Newtonsoft.Json;

namespace ManufacturingManagement_V2.Controllers
{
    public class DisplayDashboardController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AlertsBLL alertsBLL = new AlertsBLL();
        private WorkListBLL workListBLL = new WorkListBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Quality_Dashboard()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (objCookie.getDeptMenuList().ToLower().Contains("quality") == false)
            {
                return RedirectToAction("Index", "Home", new { deptt = "Home" });
            }
            AlertsMdl amdl = new AlertsMdl();
            amdl.AlertDetail = getQualityDashboardInfo().AlertDetail;
            return View(amdl);
        }

        [HttpGet]
        public ActionResult Compliance_Dashboard()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (objCookie.getDeptMenuList().ToLower().Contains("compliance") == false)
            {
                return RedirectToAction("Index", "Home", new { deptt = "Home" });
            }
            AlertsMdl amdl = new AlertsMdl();
            amdl.AlertDetail = getComplianceDashboardInfo().AlertDetail;
            return View(amdl);
        }

        [HttpGet]
        public ActionResult TenderAlert()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (objCookie.getDeptMenuList().ToLower().Contains("marketing") == false)
            {
                return RedirectToAction("Index", "Home", new { deptt = "Home" });
            }
            if (mc.getPermission(Entry.Tender_Alert, permissionType.Add) == false)
            {
                return RedirectToAction("DisplayErpV1", "Home", new { url = "../Dashboard/MarketingHodDsb.aspx" });
            }
            CompanyBLL compBLL = new CompanyBLL();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", Convert.ToInt32(objCookie.getCompCode()));
            return View();
        }

        [HttpPost]
        public JsonResult getTenderAlertList(int ccode)
        {
            bool addlink = true;
            if (Convert.ToInt32(objCookie.getCompCode()) != ccode)
            {
                addlink = false;
            }
            AlertsBLL rptBLL = new AlertsBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            string rptcontent = rptBLL.getTenderAlert(addlink, ccode).Tables[0].Rows[0]["Result"].ToString();
            return new JsonResult { Data = new { alertlist = rptcontent } };
        }

        private AlertsMdl getAlertSummary()
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            amdl = alertsBLL.getAlertListForUser();
            return amdl;
        }

        private AlertsMdl getComplianceDashboardInfo()
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = alertsBLL.getWorkListAlert();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string info = "<table class='tablecontainer' style='width:100%;'>";//style='font-size:10pt;'
                info += "<tr>";
                info += "<th width='30px;' valign='top'><b>S.No.</b></td>";
                info += "<th width='40px;' valign='top'><b>Company</b></td>";
                info += "<th width='40px;' valign='top'><b>ComplianceOf</b></td>";
                info += "<th width='50px;' valign='top'><b>Task Date</b></td>";
                info += "<th width='auto;' valign='top'><b>Task Name</b></td>";
                info += "<th width='auto;' valign='top'><b>Description</b></td>";
                info += "</tr>";
                for (int i = 0; i<ds.Tables[0].Rows.Count; i++)
                {
                    info += "<tr>";
                    info += "<td valign='top' style='text-align:center;background-color:" + ds.Tables[0].Rows[i]["RecColor"].ToString() + ";color:white;'>" + (i + 1).ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["compname"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["depname"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["recdt"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["taskname"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["workdesc"].ToString() + "</td>";
                    info += "</tr>";
                }
                info += "</table>";
                amdl.AlertDetail = "<html><body>" + info + "</body></html>";
            }
            return amdl;
        }

        [HttpGet]
        public ActionResult DocumentControl_Dashboard()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //if (objCookie.getDeptMenuList().ToLower().Contains("compliance") == false)
            //{
            //    return RedirectToAction("Index", "Home", new { deptt = "Home" });
            //}
            AlertsMdl amdl = new AlertsMdl();
            //amdl.AlertDetail = getComplianceDashboardInfo().AlertDetail;
            return View(amdl);
        }

        private AlertsMdl getQualityDashboardInfo()
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            //[100043]--Set 1: 30 Days calibration alert
            CalibrationBLL calBLL = new CalibrationBLL();
            ds = calBLL.getCalibrationAlertDataV2(0, 0, DateTime.Now);
            DateTime cdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime duedate = new DateTime();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string info = "<table class='tablecontainer' style='width:100%;'>";//style='font-size:10pt;'
                info += "<tr>";
                info += "<th width='20px;' valign='top'><b>S.No.</b></td>";
                info += "<th width='20px;' valign='top'><b>Company</b></td>";
                //info += "<th width='20px;' valign='top'><b>Rec Id</b></td>";
                info += "<th width='auto;' valign='top'><b>Id&nbsp;No</b></td>";
                info += "<th width='auto;' valign='top'><b>Range</b></td>";
                info += "<th width='auto;' valign='top'><b>Location</b></td>";
                info += "<th width='30px;' valign='top'><b>Calibration&nbsp;Done&nbsp;On</b></td>";
                //info += "<td class='indexColumn' width='20px;' valign='top'><b>Certificate No</b></td>";
                //info += "<td class='indexColumn' width='20px;' valign='top'><b>Certified By</b></td>";
                info += "<th width='30px;' valign='top'><b>Next&nbsp;Calibration&nbsp;Due&nbsp;On</b></td>";
                info += "</tr>";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    info += "<tr>";
                    duedate = Convert.ToDateTime(ds.Tables[0].Rows[i]["nextcalibdate"].ToString());
                    //if (duedate < cdate) line formatted
                    //{
                    //    info += "<tr style='text-align:center;color:white;background-color:gray;'>";//background-color:red;
                    //}
                    //else
                    //{
                    //    info += "<tr style='text-align:center;color:black;font-weight:bold;background-color:white;'>";//background-color:yellow;
                    //}
                    info += "<td valign='top'>" + (i + 1).ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["CmpName"].ToString() + "</td>";
                    //info += "<td valign='top'>" + ds.Tables[0].Rows[i]["RecId"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["IdNo"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["ImteRange"].ToString() + "</td>";
                    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["Location"].ToString() + "</td>";
                    info += "<td valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["calibdate"].ToString())) + "</td>";
                    //info += "<td valign='top'>" + ds.Tables[0].Rows[i]["CertificateNo"].ToString() + "</td>";
                    //info += "<td valign='top'>" + ds.Tables[0].Rows[i]["CertifiedBy"].ToString() + "</td>";
                    if (duedate < cdate)//column formatted
                    {
                        info += "<td valign='top' style='color:white;background-color:gray;'>" + mc.getStringByDate(duedate) + "</td>";
                    }
                    else
                    {
                        info += "<td valign='top' style='color:black;background-color:yellow;'>" + mc.getStringByDate(duedate) + "</td>";
                    }
                    info += "</tr>";
                }
                info += "</table>";
                amdl.AlertDetail = "<html><body>" + info + "</body></html>";
            }
            return amdl;
        }

        [HttpGet]
        public ActionResult UserTrainingView(string schdno = "TS-1-2019", string trnstatus = "c")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //if (objCookie.getDeptMenuList().ToLower().Contains("quality") == false)
            //{
            //    return RedirectToAction("Index", "Home", new { deptt = "Home" });
            //}
            AlertsMdl amdl = new AlertsMdl();
            amdl.AlertDetail = getUserTrainingInfo(schdno, trnstatus).AlertDetail;
            //
            return View(amdl);
        }
        private AlertsMdl getUserTrainingInfo(string schdno, string trnstatus)
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            UserBLL bllObject = new UserBLL();
            ds = bllObject.getUserTrainingList(schdno,trnstatus);
            //string trndtstr = "";
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //string info = "<table class='table table-bordered'>";//style='font-size:10pt;'
            //info += "<tr><td colspan='6' ><b>Schedule: "+ ds.Tables[0].Rows[0]["SchdNo"].ToString() + ", Status: "+ ds.Tables[0].Rows[0]["TrnStatusName"].ToString() + "</b></td></tr>";
            //info += "<tr>";
            //info += "<td width='20px;' valign='top'><b>S.No.</b></td>";
            //info += "<td width='100px;' valign='top'><b>Date</b></td>";
            //info += "<td width='100px;' valign='top'><b>Time</b></td>";
            //info += "<td width='200px;' valign='top'><b>Subject</b></td>";
            //info += "<td width='60px;' valign='top'><b>Employee&nbsp;Id</b></td>";
            //info += "<td width='auto;' valign='top'><b>Employee&nbsp;Name</b></td>";
            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //{
            //    info += "<tr>";
            //    info += "<td valign='top'>" + (i + 1).ToString() + "</td>";
            //    trndtstr = mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["trndate"].ToString()));
            //    info += "<td valign='top'>" + trndtstr + "</td>";
            //    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["TrnTime"].ToString() + "</td>";
            //    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["TrnSubject"].ToString() + "</td>";
            //    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["EmpId"].ToString() + "</td>";
            //    info += "<td valign='top'>" + ds.Tables[0].Rows[i]["UserName"].ToString() + "</td>";
            //    info += "</tr>";
            //}
            //info += "</table>";
            //amdl.AlertDetail = "<html><body>" + info + "</body></html>";
            //}
            amdl.AlertDetail = "<html><body>" + ds.Tables[0].Rows[0]["result"].ToString() + "</body></html>";
            return amdl;
        }

    }
}
