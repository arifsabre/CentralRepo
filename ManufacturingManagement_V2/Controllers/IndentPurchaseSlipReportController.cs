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
    public class IndentPurchaseSlipReportController : Controller
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
        }

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.Above58 = false;//for printrate
            return View(rptOption);
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
        //

        #region indent purchase slip

        [HttpGet]
        public ActionResult IndentPurchaseSlip(int slipno = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (slipno == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Slip Number not entered!</h1></a>");
            }
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentPurchaseSlipReport/getIndentPurchaseSlipFile";
                string reportpms = "slipnofrom=" + slipno + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentPurchaseSlipFile", new { slipnofrom = slipno });
        }

        [HttpPost]
        public ActionResult IndentPurchaseSlip(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (rptOption.VNoFrom == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Slip Number not entered!</h1></a>");
            }
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentPurchaseSlipReport/getIndentPurchaseSlipFile";
                string reportpms = "slipnofrom=" + rptOption.VNoFrom + "";
                reportpms += "&slipnoto=" + rptOption.VNoTo + "";
                reportpms += "&printrate=" + rptOption.Above58 + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentPurchaseSlipFile", new { slipnofrom = rptOption.VNoFrom, slipnoto = rptOption.VNoTo, printrate = rptOption.Above58 });
        }

        public FileResult getIndentPurchaseSlipFile(int slipnofrom, int slipnoto = 0, bool printrate = false)
        {
            if (mc.isValidToDisplayReport() == false)//note
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            if (slipnoto == 0 || slipnoto < slipnofrom)
            {
                slipnoto = slipnofrom;
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IndentPurchaseSlip.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for dbp- usp_get_indent_purchase_slip
            rptDoc.SetParameterValue("@slipnofrom", slipnofrom);
            rptDoc.SetParameterValue("@slipnoto", slipnoto);
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
            rptDoc.SetParameterValue("@finyear", objCookie.getFinYear());
            //additional parameters --defined in crpt with @name/static
            rptDoc.SetParameterValue("@printrate", printrate);
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

        #region indent purchase slip detail as ledger

        [HttpPost]
        public ActionResult IndentPurchaseSlipDetail(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentPurchaseSlipReport/getIndentPurchaseSlipDetailFile";
                string reportpms = "?purchasemode=i";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentPurchaseSlipDetailFile", new { puchasemode = "i" });
        }

        [HttpPost]
        public ActionResult PurchaseOrderSlipDetail(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.IndentReport, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.IndentReport, permissionType.Edit);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "IndentPurchaseSlipReport/getIndentPurchaseSlipDetailFile";
                string reportpms = "?purchasemode=p";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getIndentPurchaseSlipDetailFile", new { purchasemode="p" });
        }

        public FileResult getIndentPurchaseSlipDetailFile(string purchasemode = "i")
        {
            if (mc.isValidToDisplayReport() == false)//note
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "IndentPurchaseSlipDetail.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptTitle"];
            txtRptTitle.Text = "Indent Purchase Slip Detail";
            if (purchasemode == "p")
            {
                txtRptTitle.Text = "Purchase Order Slip Detail";
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "Execution Financial Year: " + objCookie.getFinYear();
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for dbp -usp_get_indent_purchase_slip_detail
            rptDoc.SetParameterValue("@purchasemode", purchasemode);
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
            rptDoc.SetParameterValue("@finyear", objCookie.getFinYear());
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

    }
}
