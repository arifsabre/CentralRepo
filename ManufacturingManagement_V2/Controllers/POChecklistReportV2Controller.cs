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
    public class POChecklistReportV2Controller : Controller
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

        [HttpGet]//not required here
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            //ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            //ViewBag.RailwayList = new SelectList(rptBll.getRailwayList(), "railwayid", "railwayname");
            //ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            //ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            //rptOption.DateFrom = objCookie.getFromDate();
            //rptOption.DateTo = objCookie.getDispToDate();
            //rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region po checklist report

        [HttpGet]
        public ActionResult DisplayPOChecklistReportV2(int porderid = 0)
        {
            //[100173]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "POChecklistReportV2/getPOChecklistReportV2";
                string reportpms = "porderid=" + porderid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPOChecklistReportV2", new { porderid = porderid });
        }

        [HttpGet]
        public FileResult getPOChecklistReportV2(int porderid = 0)
        {
            //[100173]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub1 = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/POChecklistRPT/"), "POChecklistV2Rpt.rpt"));
            rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/POChecklistRPT/"), "POChecklistV2_ItemDetail.rpt"));
            rptDocSub1.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/POChecklistRPT/"), "POChecklistV2_DPDetail.rpt"));
            setLoginInfo(rptDoc);
            //
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //txtrpthead.Text = "Invoice Date From: " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            //setLoginInfo(rptDocSub);
            //parameters-dbp
            rptDoc.SetParameterValue("@porderid", porderid);
            rptDoc.SetParameterValue("@porderid", porderid, rptDoc.Subreports[0].Name.ToString());
            rptDoc.SetParameterValue("@porderid", porderid, rptDoc.Subreports[1].Name.ToString());
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
                rptDocSub1.Close();
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
