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
    public class WorkListReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        WorkListBLL workListBLL = new WorkListBLL();
        private GovtDepartmentBLL govtDepBLL = new GovtDepartmentBLL();
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

        #region compliance report

        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.TaskOptList = new SelectList(workListBLL.getComplianceGroupList(), "GroupId", "GroupName");
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname");
            ViewBag.DepartmentList = new SelectList(govtDepBLL.getObjectList(), "depid", "depname");
            //note: starting date
            rptOption.DateFrom = new DateTime(2018,10,1);
            rptOption.DateTo = DateTime.Now;
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult ComplianceReport(rptOptionMdl rptOption)
        {
            //[100024]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = false;
            List<EntryGroupMdl> compGroup = workListBLL.getComplianceGroupList();
            if (compGroup.Count > 0)//no permission
            {
                viewper = true;
            }
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = true;//note: ? workListBLL.isValidToModifyWorkList();
            rptOption.Detailed = false;//is completed = false
            if (rptOption.AttShift.ToString().ToLower() == "c")
            {
                rptOption.Detailed = true;//is completed = true
            }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "WorkListReport/getComplianceReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&iscompleted=" + rptOption.Detailed + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&depid=" + rptOption.RailwayId + "";
                reportpms += "&taskopt=" + rptOption.ItemId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getComplianceReport", new { iscompleted=rptOption.Detailed, depid=rptOption.RailwayId, taskopt=rptOption.ItemId, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode });
        }

        [HttpGet]
        public FileResult getComplianceReport(bool iscompleted, int depid, int taskopt, DateTime dtfrom, DateTime dtto, int compcode)
        {
            //[100024]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ComplianceRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpAddress = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpAddress"];
            if (compcode == 0)
            {
                txtCmpName.Text = "PRAG GROUP OF INDUSTRIES";
                txtCmpAddress.Text = "E-7, Talkatora, Industrial Estate, Lucknow-226011";
            }
            else
            {
                CompanyMdl compmdl = new CompanyMdl();
                compmdl = compBLL.searchObject(compcode);
                txtCmpName.Text = compmdl.CmpName;
                txtCmpAddress.Text = compmdl.Footer1;
            }
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (depid > 0)
            {
                DataSet ds = new DataSet();
                ds = govtDepBLL.getObjectData();
                txtrpthead.Text += "\r\nDepartment: " + mc.getNameByKey(ds, "depid", depid.ToString(), "depname");
            }
            string comp = iscompleted == true ? "Completed" : "Pending";
            txtrpthead.Text += "\r\nStatus: " + comp;
            //dbp parameters   --usp_get_tbl_worklist
            rptDoc.SetParameterValue("@IsCompleted", iscompleted);
            rptDoc.SetParameterValue("@depid", depid);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@TaskOpt", taskopt);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@userid", Convert.ToInt32(objCookie.getUserId()));
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
