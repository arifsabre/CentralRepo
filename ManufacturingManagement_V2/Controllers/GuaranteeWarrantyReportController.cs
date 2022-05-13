using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Data;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class GuaranteeWarrantyReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        FinancialLedgerRptBLL objBLL = new FinancialLedgerRptBLL();

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
            return View(rptOption);
        }       

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Dispatch_Document_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Dispatch_Document_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "GuaranteeWarrantyReport/GetReportFile";
                string reportpms = "salerecid=" + rptOption.SaleRecId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportFile", new { salerecid = rptOption.SaleRecId });
        }

        [HttpGet]
        public ActionResult GetReport(int salerecid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Dispatch_Document_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Dispatch_Document_Report, permissionType.Edit);
            //
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "GuaranteeWarrantyReport/GetReportFile";
                string reportpms = "salerecid=" + salerecid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportFile", new { salerecid = salerecid });
        }

        [HttpGet]
        public ActionResult GetReportFile(int salerecid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/DispatchRPT"), "GuaranteeWarrantyCertificate.rpt"));
            
            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["dtImage"].Rows.Clear();
            DataRow dr = dsr.Tables["dtImage"].NewRow();
            dr["imgbyte"] = GetCompanyLogo();//footer-image
            dr["imgbyteP1"] = GetCompanyLogoP1();//header-image
            dsr.Tables["dtImage"].Rows.Add(dr);

            DataSet ds = new DataSet();
            SaleBLL rptBll = new SaleBLL();
            ds = rptBll.GetGuaranteeWarrantyReportData(salerecid);
            
            //tbl-1
            dsr.Tables["dtRDLC1"].Rows.Clear();
            dr = dsr.Tables["dtRDLC1"].NewRow();
            dr["Col_1"] = ds.Tables["tbl"].Rows[0]["vdate"].ToString();
            dr["Col_2"] = ds.Tables["tbl"].Rows[0]["rptstr"].ToString();
            dsr.Tables["dtRDLC1"].Rows.Add(dr);
            
            rptDoc.SetDataSource(dsr);
            //logic to suppress header or footer 
            rptDoc.ReportDefinition.Sections["Section5"].ReportObjects["imgbyte"].Height = 700;
            if (!(objCookie.getCompCode() == "6" || objCookie.getCompCode() == "11"))//PP-B1 or PP-2
            {
                //footer hide
                rptDoc.ReportDefinition.Sections["Section1"].SectionFormat.EnableSuppress = true;
                rptDoc.ReportDefinition.Sections["Section5"].ReportObjects["imgbyte"].Height = 1785;
            }

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
            }
            return File(stream, "application/pdf");
        }

        private byte[] GetCompanyLogo()
        {
            //printed on footer
            string imgpath = Server.MapPath("../App_Data") + "\\CmpLogo\\" + objCookie.getCompCode() + ".png";
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
        private byte[] GetCompanyLogoP1()
        {
            //printed on header
            string imgpath = Server.MapPath("../App_Data") + "\\CmpLogo\\" + objCookie.getCompCode() + "P1.png";
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

    }
}
