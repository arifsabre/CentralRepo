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
    public class ImportListReportController : Controller
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

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now.AddMonths(1);
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region

        [HttpPost]
        public ActionResult ProjectionReport(rptOptionMdl rptOption)
        {
            //[100100]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (rptOption.RmItemId == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Raw material not selected!</h1></a>");
            }
            setViewData();
            bool viewper = mc.getPermission(Entry.Import_List, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Import_List, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ImportListReport/getImportListFile";
                string reportpms = "itemid=" + rptOption.RmItemId + "";
                reportpms += "&itemcode=" + rptOption.RmItemCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getImportListFile", new { itemid = rptOption.RmItemId, itemcode= rptOption.RmItemCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        [HttpGet]
        public ActionResult ProjectionReportDirect(int itemid, string itemcode, string dtfrom, string dtto, string finyear)
        {
            //[100100]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Import_List, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Import_List, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            DateTime dtf = mc.getDateByString(dtfrom);
            DateTime dtt = mc.getDateByString(dtto);
            int ccode = Convert.ToInt32(objCookie.getCompCode());
            string fyr = objCookie.getFinYear();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ImportListReport/getImportListFile";
                string reportpms = "itemid=" + itemid + "";
                reportpms += "&itemcode=" + itemcode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(dtf) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(dtt) + "";
                reportpms += "&compcode=" + ccode + "";
                reportpms += "&finyear=" + fyr + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getImportListFile", new { itemid = itemid, itemcode = itemcode, dtfrom = dtf, dtto = dtt, compcode = ccode, finyear = fyr });
        }

        [HttpGet]
        public ActionResult getImportListFile(int itemid, string itemcode, DateTime dtfrom, DateTime dtto, int compcode, string finyear)
        {
            //[100100]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ImportListReport.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{usp_get_lic_worksheet_report;.age} <= 58";
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "PO Projection List From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto)+", Stock Fin. Year: "+finyear;
            txtrpthead.Text += "\r\nItem: " + itemcode;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for --usp_get_poprojection_import_list
            rptDoc.SetParameterValue("@itemid", itemid);
            rptDoc.SetParameterValue("@dtfrom", mc.getStringByDateToStore(dtfrom));
            rptDoc.SetParameterValue("@dtto", mc.getStringByDateToStore(dtto));
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
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
