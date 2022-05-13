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
    public class VoucherReportController : Controller
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
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            ViewBag.VTypeList = new SelectList(mc.getAccountVoucherTypeList(), "Value", "Text");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region voucher print

        [HttpPost]
        public ActionResult DisplayVoucherSlipReport(rptOptionMdl rptOption)
        {
            //[100154/100155/100156]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = false;
            bool downloadper = false;
            string methodName = "";
            if (rptOption.POType.ToLower() == "pt" || rptOption.POType.ToLower() == "rt")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipPTRT";
            }
            else if (rptOption.POType.ToLower() == "jv" || rptOption.POType.ToLower() == "co")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipJVCO";
            }
            else if (rptOption.POType.ToLower() == "bpr")
            {
                viewper = mc.getPermission(Entry.Receipt_Entry, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Receipt_Entry, permissionType.Edit);
                methodName = "getVoucherSlipBPRVPT";
            }
            else if (rptOption.POType.ToLower() == "vpt")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipBPRVPT";
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "VoucherReport/" + methodName;
                string reportpms = "vtype=" + rptOption.POType + "";
                reportpms += "&vno=" + rptOption.VNoFrom + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&finyear=" + rptOption.FinYear + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodName, new { vtype = rptOption.POType, vno = rptOption.VNoFrom, compcode = rptOption.CompCode, finyear = rptOption.FinYear });
        }

        [HttpGet]
        public ActionResult DisplayVoucherSlipDirect(string vtype, string vno)
        {
            //[100154/100155/100156]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = false;
            bool downloadper = false;
            string methodName = "";
            if (vtype == "pt" || vtype == "rt")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipPTRT";
            }
            else if (vtype == "jv" || vtype == "co")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipJVCO";
            }
            else if (vtype == "bpr")
            {
                viewper = mc.getPermission(Entry.Receipt_Entry, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Receipt_Entry, permissionType.Edit);
                methodName = "getVoucherSlipBPRVPT";
            }
            else if (vtype == "vpt")
            {
                viewper = mc.getPermission(Entry.Account_Report, permissionType.Add);
                if (viewper == false)//no permission
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                downloadper = mc.getPermission(Entry.Account_Report, permissionType.Edit);
                methodName = "getVoucherSlipBPRVPT";
            }
            Session["xsid"] = objCookie.getUserId();
            string ccode = objCookie.getCompCode();
            string finyr = objCookie.getFinYear();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "VoucherReport/" + methodName;
                string reportpms = "vtype=" + vtype + "";
                reportpms += "&vno=" + vno + "";
                reportpms += "&compcode=" + ccode + "";
                reportpms += "&finyear=" + finyr + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction(methodName, new { vtype = vtype, vno = vno, compcode = ccode, finyear = finyr });
        }

        [HttpGet]
        public FileResult getVoucherSlipPTRT(string vtype, string vno, int compcode, string finyear)
        {
            //[100154]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "VoucherSlipPTRT.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //dbp parameters
            rptDoc.SetParameterValue("@vtype", vtype);
            rptDoc.SetParameterValue("@vno", vno);
            rptDoc.SetParameterValue("@compcode", compcode);
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

        [HttpGet]
        public FileResult getVoucherSlipJVCO(string vtype, string vno, int compcode, string finyear)
        {
            //[100155]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "VoucherSlipJVCO.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //dbp parameters
            rptDoc.SetParameterValue("@vtype", vtype);
            rptDoc.SetParameterValue("@vno", vno);
            rptDoc.SetParameterValue("@compcode", compcode);
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

        [HttpGet]
        public FileResult getVoucherSlipBPRVPT(string vtype, string vno, int compcode, string finyear)
        {
            //[100156]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "VoucherSlipBPRVPT.rpt"));
            rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "VoucherSlipBPRVPT_BillOS.rpt"));
            setLoginInfo(rptDoc);
            //dbp parameters main rpt
            rptDoc.SetParameterValue("@vtype", vtype);
            rptDoc.SetParameterValue("@vno", vno);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@finyear", finyear);
            //dbp parameters sub rpt
            rptDoc.SetParameterValue("@vtype1", vtype);
            rptDoc.SetParameterValue("@vno1", vno);
            rptDoc.SetParameterValue("@compcode1", compcode);
            rptDoc.SetParameterValue("@finyear1", finyear);
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
                rptDocSub.Close();
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
