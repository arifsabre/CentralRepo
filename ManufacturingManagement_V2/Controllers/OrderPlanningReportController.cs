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
    public class OrderPlanningReportController : Controller
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
            ViewBag.ItemTypeList = new SelectList(mc.getItemTypeList(), "Value", "Text");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.FinYear = objCookie.getFinYear();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region order planning report

        [HttpPost]
        public ActionResult OrderPlanningReport(rptOptionMdl rptOption)
        {
            //[100108]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (rptOption.RmItemId == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Raw material not selected!</h1></a>");
            }
            setViewData();
            bool viewper = mc.getPermission(Entry.BOM_Definition, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.BOM_Definition, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "OrderPlanningReport/getOrderPlanningReport";
                string reportpms = "rmitemid=" + rptOption.RmItemId + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                reportpms += "&rmitemcode=" + rptOption.RmItemCode + "";
                reportpms += "&filterbydt=" + rptOption.FilterByDT + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getOrderPlanningReport", new { rmitemid = rptOption.RmItemId, dtfrom=rptOption.DateFrom, dtto=rptOption.DateTo, compcode=rptOption.CompCode, finyear = rptOption.FinYear, rmitemcode=rptOption.RmItemCode, filterbydt = rptOption.FilterByDT });
        }

        //get
        public FileResult getOrderPlanningReport(int rmitemid, DateTime dtfrom, DateTime dtto, int compcode, string finyear, string rmitemcode, bool filterbydt)
        {
            //[100108]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSubTender = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ItemsOrderPlanningRpt.rpt"));
            rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ItemsOrderPlanningRptDetail.rpt"));
            rptDocSubTender.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ItemsOrderPlanningRptDetailTender.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Order Planning Report From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto)+", ";
            txtRptHead.Text += "Stock Fin. Year: " + finyear;
            if (filterbydt == true)
            {
                txtRptHead.Text += " [DP within date range]";
            }
            txtRptHead.Text += "\r\nRaw Material : " + rmitemcode;
            //dbp parameters
            //dbp usp_order_planning_report
            rptDoc.SetParameterValue("@rmitemid", rmitemid);
            rptDoc.SetParameterValue("@filterbydt", filterbydt);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
            //#note
            //string dtfrom1 = string.Format("{0:yyyy/MM/dd}", dtfrom);
            //string dtto1 = string.Format("{0:yyyy/MM/dd}", dtto);
            //rptDoc.SetParameterValue("@dtfrom1", dtfrom1);
            //rptDoc.SetParameterValue("@dtto1", dtto1);//sf not working but working in crpt directly
            //rptDocSub.RecordSelectionFormula = "{usp_get_porderdetail_for_order_planning;1.delvdate}>=cdate('" + dtfrom1 + "') and {usp_get_porderdetail_for_order_planning;1.delvdate}<=cdate('" + dtto1 + "')";
            //note#
            //usp_get_porderdetail_for_order_planning
            rptDoc.SetParameterValue("@rmitemidP1", rmitemid);
            rptDoc.SetParameterValue("@filterbydtP1", filterbydt);
            rptDoc.SetParameterValue("@dtfromP1", dtfrom);
            rptDoc.SetParameterValue("@dttoP1", dtto);
            rptDoc.SetParameterValue("@compcodeP1", compcode);
            //usp_get_tenderdetail_for_order_planning
            rptDoc.SetParameterValue("@rmitemidP2", rmitemid);
            rptDoc.SetParameterValue("@dtfromP2", dtfrom);
            rptDoc.SetParameterValue("@dttoP2", dtto);
            rptDoc.SetParameterValue("@compcodeP2", compcode);
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
                rptDocSub.Close();
                rptDocSubTender.Close();
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
