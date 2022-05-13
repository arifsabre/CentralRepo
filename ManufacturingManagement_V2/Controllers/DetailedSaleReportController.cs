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
    public class DetailedSaleReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();

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
            ViewBag.InvoiceModeList = new SelectList(mc.getInvoiceModeRptList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult SaleReportRPG(rptOptionMdl rptOption)
        {
            //[100025]//Railway+Private, GST Sale Report
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            int ccode = Convert.ToInt32(objCookie.getCompCode());
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ERP_V1_report/getSaleReportMonthly";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&invoicemode=gst";
                reportpms += "&compcode=" + ccode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleReportMonthly", "ERP_V1_Report", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, invoicemode = "gst", compcode = ccode });
        }

        #region detailed sale report

        [HttpPost]
        public ActionResult CallDetailedSaleReport(rptOptionMdl rptOption)
        {
            //[100021]/[100022]/[100023]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (rptOption.EmpName == null) { rptOption.AgentId = 0; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "DetailedSaleReport/getDetailedSaleReport";
                string reportpms = "itemid=" + rptOption.RmItemId + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&itemcode=" + rptOption.RmItemCode + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&groupname=" + rptOption.GroupName + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&railwayid=" + rptOption.AgentId + "";
                reportpms += "&invoicemode=" + rptOption.RegNo + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDetailedSaleReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, invoicemode= rptOption.RegNo, groupid=rptOption.GroupId, potype=rptOption.POType, itemid=rptOption.RmItemId, compcode = rptOption.CompCode, groupname=rptOption.GroupName, itemcode = rptOption.RmItemCode, railwayid=rptOption.AgentId });
        }

        //get
        public FileResult getDetailedSaleReport(DateTime dtfrom, DateTime dtto, string invoicemode , int groupid, string potype, int itemid, int compcode, string groupname, string itemcode, int railwayid)
        {
            //[100021]/[100022]/[100023]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSubPO = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSubPOVT = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleReport/"), "DetailedSaleReport.rpt"));
            rptDocSubPO.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleReport/"), "DetailedSaleReport_POType.rpt"));
            rptDocSubPOVT.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleReport/"), "DetailedSaleReport_POTypeVType.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Detailed Sale Report From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (potype != "0")
            {
                txtRptHead.Text += ", PO Type: " + mc.getNameByKey(mc.getPOTypesRpt(), "potype", potype, "potypename");
            }
            if (invoicemode != "0")
            {
                txtRptHead.Text += ", Invoice Type: " + mc.getNameByKey(mc.getInvoiceModeRpt(), "invtype", invoicemode, "invtypename");
            }
            if (groupname == null) { groupid = 0; };
            if (groupid != 0)
            {
                txtRptHead.Text += ", Group: " + groupname;
            }
            if (railwayid != 0)
            {
                DataSet ds = new DataSet();
                ERP_V1_ReportBLL rptbll = new ERP_V1_ReportBLL();
                ds = rptbll.getRailwayData();
                string rname = mc.getNameByKey(ds, "railwayid", railwayid.ToString(), "railwayname");
                txtRptHead.Text += ", Railway: " + rname;
            }
            if (itemcode == null) { itemid = 0; };
            if (itemid != 0)
            {
                txtRptHead.Text += "\r\nItem: " + itemcode;
            }
            //
            //dbp usp_get_detailed_sale_report
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@InvoiceMode", invoicemode);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@POType", potype);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@RailwayId", railwayid);
            rptDoc.SetParameterValue("@compcode", compcode);
            //#note
            //string dtfrom1 = string.Format("{0:yyyy/MM/dd}", dtfrom);
            //string dtto1 = string.Format("{0:yyyy/MM/dd}", dtto);
            //rptDoc.SetParameterValue("@dtfrom1", dtfrom1);
            //rptDoc.SetParameterValue("@dtto1", dtto1);//sf not working but working in crpt directly
            //rptDocSub.RecordSelectionFormula = "{usp_get_porderdetail_for_order_planning;1.delvdate}>=cdate('" + dtfrom1 + "') and {usp_get_porderdetail_for_order_planning;1.delvdate}<=cdate('" + dtto1 + "')";
            //note#
            //dbp usp_get_sale_report_on_potype
            rptDoc.SetParameterValue("@dtfromP", dtfrom);
            rptDoc.SetParameterValue("@dttoP", dtto);
            rptDoc.SetParameterValue("@InvoiceModeP", invoicemode);
            rptDoc.SetParameterValue("@GroupIdP", groupid);
            rptDoc.SetParameterValue("@POTypeP", potype);
            rptDoc.SetParameterValue("@ItemIdP", itemid);
            rptDoc.SetParameterValue("@RailwayIdP", railwayid);
            rptDoc.SetParameterValue("@compcodeP", compcode);
            //dbp usp_get_sale_report_on_potype_vtype
            rptDoc.SetParameterValue("@dtfromV", dtfrom);
            rptDoc.SetParameterValue("@dttoV", dtto);
            rptDoc.SetParameterValue("@InvoiceModeV", invoicemode);
            rptDoc.SetParameterValue("@GroupIdV", groupid);
            rptDoc.SetParameterValue("@POTypeV", potype);
            rptDoc.SetParameterValue("@ItemIdV", itemid);
            rptDoc.SetParameterValue("@RailwayIdV", railwayid);
            rptDoc.SetParameterValue("@compcodeV", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
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
                rptDocSubPO.Close();
                rptDocSubPOVT.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion
        //

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
