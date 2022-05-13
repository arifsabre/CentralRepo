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
    public class StockReportController : Controller
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
            
            var itemtypes = mc.getItemTypeList();
            itemtypes.RemoveAt(0);//option of ALL at Index 0
            ViewBag.ItemTypeList = new SelectList(itemtypes, "Value", "Text");

            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.FinYear = objCookie.getFinYear();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region stock statement report

        [HttpPost]
        public ActionResult StockStatementReport(rptOptionMdl rptOption)
        {
            //[100125]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Inventory_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Inventory_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "StockReport/getStockStatementReport";
                string reportpms = "itemtype=" + rptOption.ItemType + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&itemid=" + rptOption.RmItemId + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&itemcode=" + rptOption.RmItemCode + "";
                reportpms += "&groupname=" + rptOption.GroupName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getStockStatementReport", new { itemtype = rptOption.ItemType, groupid = rptOption.GroupId, itemid = rptOption.RmItemId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear, itemcode = rptOption.RmItemCode, groupname=rptOption.GroupName });
        }

        //get
        public ActionResult getStockStatementReport(string itemtype, int groupid, int itemid, DateTime dtfrom, DateTime dtto, int compcode, string finyear, string itemcode="", string groupname="")
        {
            //[100125]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtfrom) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid from date for selected financial year!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "StockStatementRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            string itemtypename = mc.getNameByKey(mc.getItemTypes(), "itemtype", itemtype, "itemtypename");
            if (itemcode.Length == 0) { itemid = 0; };
            if (groupname.Length == 0) { groupid = 0; };
            txtRptHead.Text += "\r\nItem Type : " + itemtypename;
            if (groupid > 0)
            {
                txtRptHead.Text += "\r\nGroup : " + groupname;
            }
            if (itemid > 0)
            {
                txtRptHead.Text += "\r\nItem : " + itemcode;
            }
            //dbp parameters
            rptDoc.SetParameterValue("@itemtype", itemtype);
            rptDoc.SetParameterValue("@groupid", groupid);
            rptDoc.SetParameterValue("@itemid", itemid);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion
        //

        #region stock ledger report

        [HttpPost]
        public ActionResult StockLedgerReport(rptOptionMdl rptOption)
        {
            //[100126]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (rptOption.RmItemId == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Item not selected!</h1></a>");
            }
            setViewData();
            bool viewper = mc.getPermission(Entry.Inventory_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Inventory_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "StockReport/getStockLedgerReport";
                string reportpms = "itemid=" + rptOption.RmItemId + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&itemcode=" + rptOption.RmItemCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getStockLedgerReport", new { itemid = rptOption.RmItemId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear, itemcode = rptOption.RmItemCode });
        }

        //get
        public ActionResult getStockLedgerReport(int itemid, DateTime dtfrom, DateTime dtto, int compcode, string finyear, string itemcode="")
        {
            //[100126]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtfrom) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid from date for selected financial year!</h1></a>");
            }
            if (mc.isValidDateForFinYear(finyear, dtto) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid to date for selected financial year!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "StockLedgerRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            txtRptHead.Text += "\r\nItem : " + itemcode;
            if (itemcode.Length == 0) { itemid = 0; };
            //dbp parameters
            rptDoc.SetParameterValue("@itemid", itemid);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
