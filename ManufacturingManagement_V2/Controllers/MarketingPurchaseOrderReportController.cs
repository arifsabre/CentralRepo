
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Text;

namespace ManufacturingManagement_V2.Controllers
{
    public class MarketingPurchaseOrderReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        ERP_V1_ReportBLL rptBll = new ERP_V1_ReportBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            ViewBag.OrderStatusList = new SelectList(mc.getOrderStatusList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region po report

        [HttpPost]
        public ActionResult DisplayPOReport(rptOptionMdl rptOption)
        {
            //[100158]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.GroupName == null) { rptOption.GroupId = 0; }
            if (rptOption.ItemCode == null) { rptOption.ItemId = 0; }
            if (rptOption.RailwayName == null) { rptOption.RailwayId = 0; }
            string methodName = "getPOReport";
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "MarketingPurchaseOrderReport/" + methodName + "";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&itemid=" + rptOption.ItemId + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                reportpms += "&orderstatus=" + rptOption.OrderStatus + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&groupname=" + rptOption.GroupName  + "";
                reportpms += "&itemcode=" + rptOption.ItemCode + "";
                reportpms += "&railwayname=" + rptOption.RailwayName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodName, new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, groupid = rptOption.GroupId, itemid = rptOption.ItemId, railwayid = rptOption.RailwayId, orderstatus = rptOption.OrderStatus, potype=rptOption.POType, compcode = rptOption.CompCode, groupname=rptOption.GroupName, itemcode=rptOption.ItemCode, railwayname= rptOption.RailwayName });
        }

        //get
        public FileResult getPOReport(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string orderstatus, string potype, int compcode, string groupname, string itemcode, string railwayname)
        {
            //[100158]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/MarketingPORPT/"), "PurchaseOrderMktgRpt.rpt"));
            //rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/MarketingPORPT/"), "PurchaseOrderGroupwiseRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "PO Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            string potypename = mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            txtRptHead.Text += ", PO Type: " + potypename;
            if (orderstatus != "0")
            {
                string osname = mc.getNameByKey(mc.getOrderStatus(), "status", orderstatus, "statusname");
                txtRptHead.Text += ", Order Status: " + osname;
            }
            if (railwayid > 0)
            {
                txtRptHead.Text += ", Railway : " + railwayname;
            }
            if (groupid > 0)
            {
                txtRptHead.Text += ", Group : " + groupname;
            }
            if (itemid > 0)
            {
                txtRptHead.Text += ", Item : " + itemcode;
            }
            //dbp
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@railwayid", railwayid);
            rptDoc.SetParameterValue("@orderstatus", orderstatus);
            rptDoc.SetParameterValue("@potype", potype);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                //if (rptformat.ToLower() == "pdf")
                //{
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //    //add these lines to download
                //    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //    //return File(stream, "application/pdf", "ReportName.pdf");
                //}
                //else if (rptformat.ToLower() == "excel")
                //{
                //    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                //}
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
            //if (rptformat.ToLower() == "excel")
            //{
            //    return File(stream, "application/excel", "SalaryReportHO.xls");
            //}
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
