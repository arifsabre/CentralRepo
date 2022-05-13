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
    public class StockRegisterController : Controller
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

        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.VTypeList = new SelectList(mc.getStockVoucherTypeList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region stock register report

        [HttpPost]
        public ActionResult StockRegisterReport(rptOptionMdl rptOption)
        {
            //[100151]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Inventory_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Inventory_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (rptOption.ItemCode == null) { rptOption.ItemCode = "0"; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "StockRegister/getStockRegister";
                string reportpms = "vtype=" + rptOption.ItemCode + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtfrom=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getStockRegister", new { vtype = rptOption.ItemCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode });
        }

        //get
        public ActionResult getStockRegister(string vtype, DateTime dtfrom, DateTime dtto, int compcode)
        {
            //[100151]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "StockRegisterRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            string vtypename = mc.getNameByKey(mc.getStockVoucherTypes(), "vtype", vtype, "vtypename");
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (vtype != "0")
            {
                txtRptHead.Text += ", Voucher Type: " + vtypename;
            }
            //dbp parameters
            rptDoc.SetParameterValue("@vtype", vtype);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
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

        #region stock slip report

        [HttpGet]
        public ActionResult StockSlipReport(int stkrecid)
        {
            //[100152]
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
                string reporturl = "StockRegister/getStockSlip";
                string reportpms = "stkrecid=" + stkrecid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getStockSlip", new { stkrecid = stkrecid });
        }

        //get
        public ActionResult getStockSlip(int stkrecid)
        {
            //[100152]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "StockSlipRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            //CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            //txtRptHead.Text = "Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            //
            //dbp parameters
            rptDoc.SetParameterValue("@recid", stkrecid);
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

    }
}
