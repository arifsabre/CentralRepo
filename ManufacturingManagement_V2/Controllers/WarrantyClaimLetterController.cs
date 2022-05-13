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
    public class WarrantyClaimLetterController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
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
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "WarranyClaimLetter/GetReportFile";
                string reportpms = "salerecid=" + rptOption.SaleRecId + "";
                //reportpms += "&attyear=" + rptOption.AttYear.ToString() + "";
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
                string reporturl = "WarranyClaimLetter/GetReportFile";
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
            
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/DispatchRPT"), "WarrantyClaimLetter.rpt"));

            Reports.dsReport dsr = new Reports.dsReport();
            dsr.Tables["dtImage"].Rows.Clear();
            DataRow dr = dsr.Tables["dtImage"].NewRow();
            dr["imgbyte"] = GetCompanyLogo();//footer-image
            dr["imgbyteP1"] = GetCompanyLogoP1();//header-image
            dsr.Tables["dtImage"].Rows.Add(dr);

            DataSet ds = new DataSet();
            SaleBLL rptBll = new SaleBLL();
            ds = rptBll.GetWarrantyClaimReportData(salerecid);
            
            //tbl-1
            dsr.Tables["dtRDLC1"].Rows.Clear();
            dr = dsr.Tables["dtRDLC1"].NewRow();
            dr["Col_1"] = ds.Tables["tbl"].Rows[0]["letterno"].ToString();
            dr["Col_2"] = ds.Tables["tbl"].Rows[0]["letterdate"].ToString();
            dr["Col_3"] = ds.Tables["tbl"].Rows[0]["address1"].ToString();
            dr["Col_4"] = ds.Tables["tbl"].Rows[0]["address2"].ToString();
            dr["Col_5"] = ds.Tables["tbl"].Rows[0]["address3"].ToString();
            dr["Col_6"] = ds.Tables["tbl"].Rows[0]["address4"].ToString();
            dr["Col_7"] = ds.Tables["tbl"].Rows[0]["subjectline"].ToString();
            dr["Col_8"] = ds.Tables["tbl"].Rows[0]["footerline"].ToString();
            dr["Col_9"] = ds.Tables["tbl"].Rows[0]["forcmp"].ToString();
            dsr.Tables["dtRDLC1"].Rows.Add(dr);
            
            //tbl-2
            dsr.Tables["dtRDLC2"].Rows.Clear();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                dr = dsr.Tables["dtRDLC2"].NewRow();
                dr["Col_1"] = ds.Tables["tbl1"].Rows[i]["slno"].ToString();
                dr["Col_2"] = ds.Tables["tbl1"].Rows[i]["linestr"].ToString();
                dsr.Tables["dtRDLC2"].Rows.Add(dr);
            }

            rptDoc.SetDataSource(dsr);
            
            //logic to suppress header or footer 
            rptDoc.ReportDefinition.Sections["Section5"].ReportObjects["imgbyte"].Height = 700;
            if (!(objCookie.getCompCode() == "6" || objCookie.getCompCode() == "11"))//PP-B1 or PP-2
            {
                //footer hide
                rptDoc.ReportDefinition.Sections["Section2"].SectionFormat.EnableSuppress = true;
                rptDoc.ReportDefinition.Sections["Section5"].ReportObjects["imgbyte"].Height = 1785;
            }
            
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
            }
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult GetReportFile_Sample(int salerecid, string rptformat)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            
            SaleBLL rptBLL = new SaleBLL();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/DispatchRPT"), "Sample_Xsd_CRPT.rpt"));
            
            Reports.dsReport dsr = new Reports.dsReport();
            DataSet ds = new DataSet();
            ds = rptBLL.GetWarrantyClaimReportData(salerecid);
            
            //table-1
            dsr.Tables["dtRDLC1"].Rows.Clear();
            DataRow dr1 = dsr.Tables["dtRDLC1"].NewRow();
            for (int i = 0; i < ds.Tables["tbl"].Rows.Count; i++)
            {
                dr1 = dsr.Tables["dtRDLC1"].NewRow();
                dr1["Col_1"] = ds.Tables["tbl"].Rows[i]["cv1"].ToString();
                dr1["Col_2"] = ds.Tables["tbl"].Rows[i]["cv2"].ToString();
                //....
                dsr.Tables["dtRDLC1"].Rows.Add(dr1);
            }

            //table-2
            dsr.Tables["dtRDLC2"].Rows.Clear();
            DataRow dr2 = dsr.Tables["dtRDLC2"].NewRow();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                dr2 = dsr.Tables["dtRDLC1"].NewRow();
                dr2["Col_1"] = ds.Tables["tbl1"].Rows[i]["cv1"].ToString();
                dr2["Col_2"] = ds.Tables["tbl1"].Rows[i]["cv2"].ToString();
                //....
                dsr.Tables["dtRDLC2"].Rows.Add(dr2);
            }

            //additional values
            //CrystalDecisions.CrystalReports.Engine.TextObject txtcmpname = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section2"].ReportObjects["txtcmpname"];
            //txtcmpname.Text = "value";
            
            rptDoc.SetDataSource(dsr);
            
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rptDoc.RecordSelectionFormula = "";
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
            }
            if (rptformat.ToLower() == "excel")
            {
                return File(stream, "application/excel", "Form12.xls");
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
