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
    public class B_Daily_Attendance_StatusController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

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
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");

            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());

            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.DateTo = DateTime.Now;
            //rptOption.DateFrom = DateTime.Now;
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.Days = "D01";
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
        public ActionResult Get_B_Daly_Present_Absent_Status(rptOptionMdl rptOption)
        {
            //[100129]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_Daily_Attendance_Status/Get_B_Daily_Attendance_Status_File";

                string reportpms = "Attmonth=" + rptOption.AttMonth + "";
                reportpms += "Attyear=" + rptOption.AttYear + "";
                reportpms += "Day=" + rptOption.Days + "";
                reportpms += "&rpptformat=" + rptOption.ReportFormat + "";

                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_Daily_Attendance_Status_File", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, compcode = rptOption.CompCode, Day = rptOption.Days, rptformat = rptOption.ReportFormat });
        }
        public ActionResult Get_B_Daily_Attendance_Status_File(int attmonth, int attyear, int compcode, string day, string rptformat)
        {
            //[100129]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_DailyAbsent.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //rptDoc.SetParameterValue("@from", dtfrom);
            //rptDoc.SetParameterValue("@to", dtto);
            //for dbp= ZZZ_Daily_Attendance_tt
            rptDoc.SetParameterValue("@Day", day);
            rptDoc.SetParameterValue("@year", attyear);
            rptDoc.SetParameterValue("@month", attmonth);
            rptDoc.SetParameterValue("@Cmp", compcode);
            // //newempid = 443;
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
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
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "ArrivalDeparture.xls");
            }
            return File(stream, "application/pdf");
        }
  //
       [HttpPost]
        public ActionResult Get_B_Daly_Present_Absent_Status1(rptOptionMdl rptOption)
        {
            //[100129]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_Daily_Attendance_Status/Get_B_Daily_Attendance_Status_File1";

                string reportpms = "Attmonth=" + rptOption.AttMonth + "";
                reportpms += "Attyear=" + rptOption.AttYear + "";
                reportpms += "Day=" + rptOption.Days + "";
                reportpms += "&rpptformat=" + rptOption.ReportFormat + "";

                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_Daily_Attendance_Status_File1", new { attmonth = rptOption.AttMonth, attyear = rptOption.AttYear, Day = rptOption.Days, rptformat = rptOption.ReportFormat });
        }
        public ActionResult Get_B_Daily_Attendance_Status_File1(int attmonth, int attyear, string day, string rptformat)
        {
            //[100129]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_DailyAbsentAllCompany.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //rptDoc.SetParameterValue("@from", dtfrom);
            //rptDoc.SetParameterValue("@to", dtto);
            //for dbp= ZZZ_Daily_Attendance_tt
            rptDoc.SetParameterValue("@Day", day);
            rptDoc.SetParameterValue("@year", attyear);
            rptDoc.SetParameterValue("@month", attmonth);
            // rptDoc.SetParameterValue("@Cmp", compcode);
            // //newempid = 443;
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
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
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "ArrivalDeparture.xls");
            }
            return File(stream, "application/pdf");
        }


        //AbsentDatewise

        [HttpPost]
        public ActionResult Get_AbsentListDate(rptOptionMdl rptOption)
        {
            //[100137]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_Daily_Attendance_Status/Get_AbsentListDateFile";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                //portpms += "&grade=" + rptOption.Grade + "";
                reportpms += "&rpptformat=" + rptOption.ReportFormat + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_AbsentListDateFile", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode,rptformat = rptOption.ReportFormat });
        }
        [HttpGet]
        public ActionResult Get_AbsentListDateFile(DateTime dtfrom, DateTime dtto, int compcode, string rptformat)
        {
            //[100137]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "A_AbsentMoreThanthee.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //dbp --ZZZ_USP_GET_FirstIn_Lastout--ZZZ_USP_GET_FirstIn_Lastout_Night
            rptDoc.SetParameterValue("@from", dtfrom);
            rptDoc.SetParameterValue("@to", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //rptDoc.SetParameterValue("@gradecode", grade);
            //
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                if (rptformat.ToLower() == "pdf")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //add these lines to download
                    //stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ReportName.pdf");
                }
                else if (rptformat.ToLower() == "excel")
                {
                    stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                }
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
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "ArrivalDeparture.xls");
            }
            return File(stream, "application/pdf");
        }
        [HttpGet]
        public ActionResult AbsentList()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");

            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());

            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.DateTo = DateTime.Now;
            //rptOption.DateFrom = DateTime.Now;
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now;

            return View(rptOption);
        }

        public ActionResult AbsentAllCompany()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.Days = "D01";

            return View(rptOption);
        }

        public ActionResult AbsentOneday()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.Days = "D01";

            return View(rptOption);
        }






    }
}
