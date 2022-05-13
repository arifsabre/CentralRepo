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
    public class CollabPermissionReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        ItemGroupBLL igroupBLL = new ItemGroupBLL();
        //
        // GET: /AttendanceReport/

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
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.ItemGroupList = new SelectList(igroupBLL.getObjectList(), "GroupId", "GroupName");
            ViewBag.CategoryList = new SelectList(igroupBLL.getCollabCategoryList(), "CategoryId", "CategoryName");
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

        [HttpPost]
        public ActionResult CollabPermission(rptOptionMdl rptOption)
        {
            //[100085]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Collaboration_Dashboard, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Collaboration_Dashboard, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.EmpName == null) { rptOption.AgentId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "CollabPermissionReport/getCollabPermissionFile";
                string reportpms = "groupid=" + rptOption.GroupId + "";
                reportpms += "&categoryid=" + rptOption.CategoryId + "";
                reportpms += "&userid=" + rptOption.AgentId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getCollabPermissionFile", new { groupid = rptOption.GroupId, categoryid = rptOption.CategoryId, userid = rptOption.AgentId });
        }

        [HttpGet]
        public FileResult getCollabPermissionFile(int groupid=0, int categoryid=0, int userid=0)
        {
            //[100085]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            if (objCookie.getLoginType() != 0)//not admin
            {
                userid = Convert.ToInt32(objCookie.getUserId());
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "CollabPermissionRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptTitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptTitle"];
            string username = userid == 0 ? "ALL" : userid.ToString();
            string groupname = groupid == 0 ? "ALL" : groupid.ToString();
            string categoryname = categoryid == 0 ? "ALL" : categoryid.ToString();
            txtRptTitle.Text = "Group Id: " + groupname;
            txtRptTitle.Text += ", Category Id: " + categoryname;
            txtRptTitle.Text += ", User Id: " + username;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters   --usp_get_collabpermission_report
            rptDoc.SetParameterValue("@userid", userid);
            rptDoc.SetParameterValue("@groupid", groupid);
            rptDoc.SetParameterValue("@categoryid", categoryid);
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
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
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
            return File(stream, "application/pdf");
        }

    }
}
