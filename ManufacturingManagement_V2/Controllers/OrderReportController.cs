using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;

namespace ManufacturingManagement_V2.Controllers
{
    public class OrderReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.AttYear = DateTime.Now.Year;
            //rptOption.DateFrom = DateTime.Now;
            //rptOption.DateTo = DateTime.Now;
            return View(rptOption);
        }

        #region order print

        [HttpPost]
        public ActionResult OrderReport(rptOptionMdl rptOption)
        {
            //[100109]/F1
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "OrderReport/getOrderReportFile";
                string reportpms = "orderno=" + rptOption.VNoFrom + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderReportFile", new { orderno = rptOption.VNoFrom });
        }

        [HttpGet]
        public ActionResult OrderReportDirect(int orderno)
        {
            //[100109]/F1
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "OrderReport/getOrderReportFile";
                string reportpms = "orderno=" + orderno + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderReportFile", new { orderno = orderno });
        }

        [HttpGet]
        public ActionResult getOrderReportFile(int orderno = 0)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100109]/F1/NCRPT
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderReport.rpt"));
            rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            setLoginInfo(rptDocSub);
            rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            //txtCmpName.Text = objCookie.getCmpName();
            //rptDoc.Subreports[0].SetDataSource(ds);
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
                rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf"); 
        }

        #endregion

        #region order pending report

        [HttpGet]
        public ActionResult OrderPendingReport()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (mc.getPermission(Entry.Stores_PO_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult OrderPendingReport(rptOptionMdl rptOption)
        {
            //[100109]/F2
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Stores_PO_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "OrderReport/getOrderPendingReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&vendorid=" + rptOption.VendorId + "";
                reportpms += "&itemid=" + rptOption.ItemId + "";
                reportpms += "&vendorname=" + rptOption.VendorName + "";
                reportpms += "&itemcode=" + rptOption.ItemCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderPendingReport", new { dtfrom = rptOption.DateFrom,dtto=rptOption.DateTo,vendorid=rptOption.VendorId,itemid=rptOption.ItemId,vendorname=rptOption.VendorName,itemcode=rptOption.ItemCode });
        }

        [HttpGet]
        public ActionResult getOrderPendingReport(DateTime dtfrom,DateTime dtto,int vendorid=0,int itemid=0,string vendorname="",string itemcode="")
        {
            //[100109]/F2
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            if (itemcode.Length == 0) { itemid = 0; };
            if (vendorname.Length == 0) { vendorid = 0; };
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderPendingRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            string sf = "{vw_order_report.compcode}=" + objCookie.getCompCode() + "";
            //sf += " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            sf += " and {vw_order_report.orderdate} >= cdate('" + string.Format("{0:MM/dd/yyyy}", dtfrom) + "')";
            sf += " and {vw_order_report.orderdate} <= cdate('" + string.Format("{0:MM/dd/yyyy}", dtto) + "')";
            sf += " and {vw_order_report.dspqty} <> {vw_order_report.ordqty}";
            if (vendorid != 0)
            {
                sf += " and {vw_order_report.vendorid} = " + vendorid + "";
            }
            if (itemid != 0)
            {
                sf += " and {vw_order_report.itemid} = " + itemid + "";
            }
            rptDoc.RecordSelectionFormula = sf;
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptTitle"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptTitle.Text = "Order Pending Report";
            txtRptHead.Text = "Date From: " + mc.getStringByDate(dtfrom) + " To: " + mc.getStringByDate(dtto);
            if (vendorid != 0)
            {
                txtRptHead.Text += ", Vendor: " + vendorname;
            }
            if (itemid != 0)
            {
                txtRptHead.Text += ", Item: " + itemcode;
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
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
