
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
    public class AgentReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        AgentBLL bllObject = new AgentBLL();

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
            ViewBag.SVPTypeList = new SelectList(bllObject.getServicePartnerTypeList(), "SVPTypeId", "SVPTypeName");
            return View(rptOption);
        }

        #region tender report

        [HttpPost]
        public ActionResult DisplayAgentReport(rptOptionMdl rptOption)
        {
            //[100169]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Agent_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Agent_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "AgentReport/getAgentReport";
                string reportpms = "svptypeid=" + rptOption.EmpType + "";
                reportpms += "&cityid=" + rptOption.GroupId + "";
                reportpms += "&cityname=" + rptOption.GroupName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getAgentReport", new { svptypeid = rptOption.EmpType, cityid = rptOption.GroupId, cityname = rptOption.GroupName });
        }

        //get
        public FileResult getAgentReport(int svptypeid = 0, int cityid = 0, string cityname = "")
        {
            //[100169]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "AgentReport.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            string svpname = "ALL";
            if (svptypeid > 0)
            {
                DataSet ds = new DataSet();
                ds = AgentBLL.Instance.getServicePartnerTypeData();
               svpname= mc.getNameByKey(ds, "svptypeid", svptypeid.ToString(), "svptypename");
            }
            txtRptHead.Text = "Service Partner: " + svpname;
            if (cityname.Length == 0) { cityid = 0; };
            if (cityid > 0)
            {
                txtRptHead.Text += ", City: " + cityname;
            }
            //dbp
            rptDoc.SetParameterValue("@svptypeid", svptypeid);
            rptDoc.SetParameterValue("@cityid", cityid);
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
