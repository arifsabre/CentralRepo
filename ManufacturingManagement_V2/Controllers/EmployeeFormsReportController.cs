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
    public class EmployeeFormsReportController : Controller
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

        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            return View(rptOption);
        }

        #region employee forms report
        [HttpPost]
        public ActionResult DisplayEmployeeFormReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            if (rptOption.EmpId == null) { rptOption.NewEmpId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/EmployeeFormReportHTML";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("EmployeeFormReportHTML", new { newempid = rptOption.NewEmpId });
        }

        [HttpGet]
        public ActionResult EmployeeFormReportLink(int newempid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Employee_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Employee_Report, permissionType.Edit);
            //if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/EmployeeFormReportHTML";
                string reportpms = "newempid=" + newempid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("EmployeeFormReportHTML", new { newempid = newempid });
        }

        public ActionResult EmployeeFormReportHTML(int newempid)
        {
            //from dbProcedures/EmployeeFormRPT_SP.sql
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (newempid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Purchase number not selected!</h1></a>");
            }
            setViewData();
            EmployeeReportBLL rptBLL = new EmployeeReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            ReportModelObject.ReportHeader = rptBLL.getRptHeader(newempid).Tables[0].Rows[0]["rptheader"].ToString();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<b><u> Joining Detail </u></b>");
            sb.Append(rptBLL.getJoiningDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Payment Detail </u></b>");
            sb.Append(rptBLL.getPaymentDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Personal Detail </u></b>");
            sb.Append(rptBLL.getPersonalDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Contact Detail </u></b>");
            sb.Append(rptBLL.getContactDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Emergency Detail </u></b>");
            sb.Append(rptBLL.getEmergencyDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Reference Detail </u></b>");
            sb.Append(rptBLL.getReferenceDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Bank Detail </u></b>");
            sb.Append(rptBLL.getBankDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> PF/ESI Detail </u></b>");
            sb.Append(rptBLL.getPFESIDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Leaving/Updation Detail </u></b>");
            sb.Append(rptBLL.getLeavingUpdationDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Qualification Detail </u></b>");
            sb.Append(rptBLL.getQualificationDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Experience Detail </u></b>");
            sb.Append(rptBLL.getExperienceDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Family Detail </u></b>");
            sb.Append(rptBLL.getFamilyDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            sb.Append("<b><u> <br/> Nominee Detail </u></b>");
            sb.Append(rptBLL.getNomineeDetail(newempid).Tables[0].Rows[0]["Result"].ToString());

            ReportModelObject.ReportContent = sb.ToString();

            ViewBag.NewEmpIdImg = newempid;
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }
        #endregion

        private byte[] getEmployeePhoto(int newempid)
        {
            string imgpath = Server.MapPath("../App_Data") + "\\EmpDocs\\" + newempid.ToString() + "\\photo.png";
            System.IO.FileStream fs;
            System.IO.BinaryReader br;
            if (System.IO.File.Exists(imgpath) == false)
            {
                imgpath = imgpath = Server.MapPath("../App_Data") + "\\EmpDocs\\blank.png";
            }
            fs = new System.IO.FileStream(imgpath, System.IO.FileMode.Open);
            br = new System.IO.BinaryReader(fs);
            byte[] imgbyte = new byte[fs.Length + 1];
            imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
            fs.Close();
            fs.Dispose();
            return imgbyte;
        }

        #region ex- employement form-11
        //
        [HttpPost]
        public ActionResult Form11Report(rptOptionMdl rptOption)
        {
            //[100087]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/getForm11ReportFile";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getForm11ReportFile", new { newempid = rptOption.NewEmpId });
        }

        public ActionResult getForm11ReportFile(int newempid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //[100087]/NCRPT
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "EmployeeForm11Rpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@newempid", newempid);
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

        #region pf noninee form
        //
        [HttpPost]
        public ActionResult PFNomineeForm1(rptOptionMdl rptOption)
        {
            //[Not In Use] To be Modified for Proper Format
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/getPFNomineeForm1";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPFNomineeForm1", new { newempid = rptOption.NewEmpId });
        }

        public ActionResult getPFNomineeForm1(int newempid)
        {
            //[Not In Use] To be Modified for Proper Format
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PFNomineeP1.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@employeeid", newempid);
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

        [HttpPost]
        public ActionResult PFNomineeForm2(rptOptionMdl rptOption)
        {
            //[Not In Use] To be Modified for Proper Format
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/getPFNomineeForm2";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getPFNomineeForm2", new { newempid = rptOption.NewEmpId });
        }

        public ActionResult getPFNomineeForm2(int newempid)
        {
            //[Not In Use] To be Modified for Proper Format
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "PFNomineeP2.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@employeeid", newempid);
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

        #region gratuity noninee form
        //
        [HttpPost]
        public ActionResult GratuityNomineeForm1(rptOptionMdl rptOption)
        {
            //[Not In Use] To be Modified for Proper Format
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/getGratuityNomineeForm1";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getGratuityNomineeForm1", new { newempid = rptOption.NewEmpId });
        }

        public ActionResult getGratuityNomineeForm1(int newempid)
        {
            //[Not In Use] To be Modified for Proper Format
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "GratuityNomineeP1.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@newempid", newempid);
            rptDoc.SetParameterValue("@nomineefor", "b");
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

        [HttpPost]
        public ActionResult GratuityNomineeForm2(rptOptionMdl rptOption)
        {
            //[Not In Use] To be Modified for Proper Format
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Bonus_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Bonus_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "EmployeeFormsReport/getGratuityNomineeForm2";
                string reportpms = "newempid=" + rptOption.NewEmpId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getGratuityNomineeForm2", new { newempid = rptOption.NewEmpId });
        }

        public ActionResult getGratuityNomineeForm2(int newempid)
        {
            //[Not In Use] To be Modified for Proper Format
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "GratuityNomineeP2.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters
            rptDoc.SetParameterValue("@newempid", newempid);
            rptDoc.SetParameterValue("@nomineefor", "b");
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
