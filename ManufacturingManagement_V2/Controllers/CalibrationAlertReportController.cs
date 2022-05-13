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
    public class CalibrationAlertReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        private IMTETypeBLL imteTypeBLL = new IMTETypeBLL();
        private ImteBLL imteBLL = new ImteBLL();

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
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            ViewBag.ImteTypeList = new SelectList(imteTypeBLL.getObjectList(rptOption.CompCode), "imtetypeid", "imtetypename");
            ViewBag.ImteList = new SelectList(imteBLL.getObjectList(0, rptOption.CompCode), "imteid", "idno");
            rptOption.ReportDate = objCookie.getDispToDate();
            return View(rptOption);
        }

        #region calibration alert report

        [HttpPost]
        public ActionResult DisplayCalibrationAlertReport(rptOptionMdl rptOption)
        {
            //[100043]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Calibration_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Calibration_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "CalibrationAlertReport/getCalibrationAlertReport";
                string reportpms = "asondate=" + mc.getStringByDateForReport(rptOption.ReportDate) + "";
                reportpms += "&imtetypeid=" + rptOption.GroupId + "";
                reportpms += "&imteid=" + rptOption.RmItemId + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getCalibrationAlertReport", new { imtetypeid = rptOption.GroupId, imteid = rptOption.RmItemId, asondate = rptOption.ReportDate, compcode = rptOption.CompCode });
        }

        [HttpGet]
        public FileResult getCalibrationAlertReport(int imtetypeid, int imteid, DateTime asondate, int compcode)
        {
            //[100043]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "CalibrationAlertRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Next calibration due within 30 days from: " + mc.getStringByDate(asondate);
            if (imtetypeid > 0)
            {
                DataSet ds = new DataSet();
                ds = imteTypeBLL.getObjectData(compcode);
                txtrpthead.Text += ", IMTE Type : " + mc.getNameByKey(ds, "imtetypeid", imtetypeid.ToString(), "imtetypename");
            }
            if (imteid > 0)
            {
                DataSet ds = new DataSet();
                ds = imteBLL.getObjectData(imtetypeid, compcode);
                txtrpthead.Text += ", IMTE IdNo : " + mc.getNameByKey(ds, "imteid", imteid.ToString(), "idno");
            }
            //setLoginInfo(rptDocSub);
            //dbp parameters
            rptDoc.SetParameterValue("@imtetypeid", imtetypeid);
            rptDoc.SetParameterValue("@imteid", imteid);
            rptDoc.SetParameterValue("@asondate", asondate);
            rptDoc.SetParameterValue("@compcode", compcode);
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
