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
    public class CorrespondenceReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CorrespondenceBLL bllObject = new CorrespondenceBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
        }

        public ActionResult Index()//not created
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname",objCookie.getCompCode());
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.AttYear = DateTime.Now.Year;
            //rptOption.DateFrom = DateTime.Now;
            //rptOption.DateTo = DateTime.Now;
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

        #region correspondence letter print

        [HttpGet]
        public ActionResult CorrespondenceReport(int recid = 0)
        {
            //[Not In Use]
            //if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //setViewData();
            //bool viewper = mc.getPermission(Entry.Correspondence_Report, permissionType.Add);
            //if (viewper == false)//no permission
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            //bool downloadper = mc.getPermission(Entry.Correspondence_Report, permissionType.Edit);
            //Session["xsid"] = objCookie.getUserId();
            //if (viewper == true && downloadper == false)//view only
            //{
            //    string reporturl = "CorrespondenceReport/getCorrespondenceReportFile";
            //    string reportpms = "recid=" + recid + "";
            //    return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            //}
            ////full access with download (no escalation)
            //return RedirectToAction("getCorrespondenceReportFile", new { recid = recid });

            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (bllObject.isValidToModifyLetter(recid) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Session["xsid"] = objCookie.getUserId();
            return RedirectToAction("getCorrespondenceReportFile", new { recid = recid });

        }

        private byte[] getCompanyLogo(int recid)
        {
            //getting compcode fro recid
            CorrespondenceMdl mdlobj = bllObject.searchObject(recid);
            string imgpath = Server.MapPath("../App_Data") + "\\CmpLogo\\" + mdlobj.CompCode.ToString() + ".png";
            System.IO.FileStream fs;
            System.IO.BinaryReader br;
            if (System.IO.File.Exists(imgpath) == false)
            {
                imgpath = imgpath = Server.MapPath("../App_Data") + "\\CmpLogo\\blank.png";
            }
            fs = new System.IO.FileStream(imgpath, System.IO.FileMode.Open);
            br = new System.IO.BinaryReader(fs);
            byte[] imgbyte = new byte[fs.Length + 1];
            imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
            fs.Close();
            fs.Dispose();
            return imgbyte;
        }
        //

        private string getAttachmentsList(int recid)
        {
            string attch = "";
            CorrespondenceMdl mdlobj = bllObject.searchObject(recid);
            //if (mdlobj.Enclosures.Length > 0)
            //{
            //    attch = mdlobj.Enclosures;
            //    return attch;
            //}
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/"));
            int x = 1;
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    attch += "(" + x.ToString() + ") " + file.Name.Substring(0, file.Name.Length - file.Extension.Length) + "\r\n";
                    x += 1;
                }
            }
            return attch.Trim();
        }
        //

        public FileResult getCorrespondenceReportFile(int recid = 0)
        {
            //[Not In Use]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSubDet = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "CorrespondenceRpt.rpt"));
            rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "CompanyHeader.rpt"));
            rptDocSubDet.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "CorrespondenceDetailRpt.rpt"));
            setLoginInfo(rptDoc);
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["dtimage"].Rows.Clear();
            DataRow dr = dsr.Tables["dtimage"].NewRow();
            dr["imgbyte"] = getCompanyLogo(recid);
            dr["compcode"] = recid;
            dsr.Tables["dtimage"].Rows.Add(dr);
            // to check dsr.Tables["dtimage"].Rows.Add(getCompanyLogo(recid),recid);
            //setLoginInfo(rptDocSub);
            rptDoc.RecordSelectionFormula = "{vw_correspondence.recid}=" + recid + "";
            CrystalDecisions.CrystalReports.Engine.TextObject txtAttch = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtAttch"];
            txtAttch.Text = getAttachmentsList(recid);
            if (txtAttch.Text.Length == 0)
            {
                rptDoc.ReportDefinition.Sections["ReportFooterSection1"].SectionFormat.EnableSuppress = true;
            }
            rptDoc.Subreports[0].SetDataSource(dsr);
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
                rptDocSub.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

    }
}
