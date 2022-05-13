using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using QRCoder;
using System.IO;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class SaleInvoiceReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
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
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }
            
        #region Sale Invoice Report
        [HttpGet]
        public ActionResult DisplaySaleInvoice(int salerecid = 0, string chinv = "i")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SaleInvoiceReport/getSaleInvoiceReport";
                string reportpms = "salerecid=" + salerecid + "";
                reportpms += "chinv=" + chinv + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getSaleInvoiceReport", new { salerecid = salerecid, chinv = chinv });
        }

        [HttpGet]
        public ActionResult getSaleInvoiceReport(int salerecid, string chinv = "i")
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (salerecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invoice number not selected!</h1></a>");
            }
            //
            SaleBLL sbll = new SaleBLL();
            SaleMdl smdl = sbll.searchObject(salerecid);
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocHeader = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocLedger = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            if (smdl.InvoiceMode.ToLower() == "dbn" || smdl.InvoiceMode.ToLower() == "cdn")
            {
                rptDocHeader.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "DnCnHeader.rpt"));
                rptDocLedger.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "DnCnLedger.rpt"));
            }
            else
            {
                if (chinv.ToLower() == "i")
                {
                    rptDocHeader.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "InvoiceHeader.rpt"));
                    rptDocLedger.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "InvoiceLedger.rpt"));
                }
                else if (chinv.ToLower() == "c")
                {
                    rptDocHeader.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "ChallanHeader.rpt"));
                    rptDocLedger.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "ChallanLedger.rpt"));
                }
            }
            setLoginInfo(rptDocHeader);
            //
            //CrystalDecisions.CrystalReports.Engine.TextObject txtNumToWords = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDocSub1.ReportDefinition.Sections["PageHeaderSection2"].ReportObjects["txtNumToWords"];
            //txtNumToWords.Text = mc.getWordByNumericDouble(smdl.NetAmount.ToString());
            //dbp parameters
            rptDocHeader.SetParameterValue("@salerecid", salerecid);//header
            //also rptDocMain.Subreports[Index].Name.ToString() can be used for sub-reports
            if (smdl.InvoiceMode.ToLower() == "dbn" || smdl.InvoiceMode.ToLower() == "cdn")
            {
                rptDocHeader.SetParameterValue("@salerecid", salerecid, "DnCnLedger.rpt");
            }
            else
            {
                if (chinv.ToLower() == "i")
                {
                    rptDocHeader.SetParameterValue("@salerecid", salerecid, "InvoiceLedger.rpt");
                }
                else if (chinv.ToLower() == "c")
                {
                    rptDocHeader.SetParameterValue("@salerecid", salerecid, "ChallanLedger.rpt");
                }
            }
            //additional parameters --defined in crpt with @name/static
            rptDocHeader.SetParameterValue("@NumToWords", mc.getWordByNumericDouble(smdl.NetAmount.ToString()));
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDocHeader.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDocHeader.Close();
                rptDocLedger.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }
        #endregion

        #region Eformat - invoice report 

        [HttpPost]
        public ActionResult DisplayInvoice(rptOptionMdl rptOption)
        {
            //[100171]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SaleInvoiceReport/getInvoiceReport";
                string reportpms = "salerecid=" + rptOption.SaleRecId + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getInvoiceReport", new { salerecid = rptOption.SaleRecId });
        }

        [HttpGet]
        public ActionResult DisplayInvoiceForV1(int salerecid = 0)
        {
            //[100171]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "SaleInvoiceReport/getInvoiceReport";
                string reportpms = "salerecid=" + salerecid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getInvoiceReport", new { salerecid = salerecid });
        }

        [HttpGet]
        public ActionResult getInvoiceReport(int salerecid)
        {
            //string irn = "142110245514291c72d74cbda0a122a088df23141331875316808a3e5ad4d65c41af41dabf71dae";
            //SaleMdl slMdl = new SaleMdl();
            //SaleBLL slBll = new SaleBLL();
            //slMdl = slBll.searchObject(salerecid);
            //string irn = slMdl.InvReferenceNo;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    QRCoder.QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(irn, QRCodeGenerator.ECCLevel.Q);
            //    using (System.Drawing.Bitmap bitMap = qrCode.GetGraphic(20))
            //    {
            //        bitMap.Save(ms, ImageFormat.Png);
            //        //ok for display--ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            //        byte[] imgbyte = ms.ToArray();
            //        InvoiceControlMdl modelObject = new InvoiceControlMdl();
            //        modelObject.FlName = "IRN";
            //        modelObject.FileContent = imgbyte;
            //        modelObject.SaleRecId = salerecid;
            //        InvoiceControlBLL bllObject = new InvoiceControlBLL();
            //        bllObject.uploadInvoiceQRCodeFile(modelObject);
            //       --on CRPT blob img location T=45, L=13208, size H&W=2500
            //    }
            //}
            //------------------------------------------------------------------

            //[100171]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (salerecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invoice number not selected!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocHeader = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocLedger = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDocHeader.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "EFormatInvHeader.rpt"));
            rptDocLedger.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/SaleInvoiceRPT/"), "EFormatInvLedger.rpt"));
            setLoginInfo(rptDocHeader);
            //
            SaleBLL sbll = new SaleBLL();
            SaleMdl smdl = sbll.searchObject(salerecid);
            //CrystalDecisions.CrystalReports.Engine.TextObject txtNumToWords = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDocSub1.ReportDefinition.Sections["PageHeaderSection2"].ReportObjects["txtNumToWords"];
            //txtNumToWords.Text = mc.getWordByNumericDouble(smdl.NetAmount.ToString());
            //dbp parameters
            rptDocHeader.SetParameterValue("@salerecid", salerecid);//header
            //also rptDocMain.Subreports[Index].Name.ToString() can be used for sub-reports
            rptDocHeader.SetParameterValue("@salerecid", salerecid, "EFormatInvLedger.rpt");
            //additional parameters --defined in crpt with @name/static
            rptDocHeader.SetParameterValue("@NumToWords", mc.getWordByNumericDouble(smdl.NetAmount.ToString()));
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDocHeader.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDocHeader.Close();
                rptDocLedger.Close();
            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        /// <summary>
        /// To be used further
        /// </summary>
        /// <param name="salerecid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJSonForIRN(int salerecid = 0)
        {
            //[100171]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); }
            setViewData();
            if (salerecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invoice not selected!</h1></a>");
            }
            bool viewper = mc.getPermission(Entry.EInvoice_Updation, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.EInvoice_Updation, permissionType.Edit);
            if (viewper == false || downloadper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //Session["xsid"] = objCookie.getUserId();
            EInvoiceSaleMdl slMdl = new EInvoiceSaleMdl();
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            slMdl = slBll.getEInvoiceSaleObject(salerecid);
            string modelToJSon = Newtonsoft.Json.JsonConvert.SerializeObject(slMdl);
            return Content(modelToJSon);
        }

        [HttpPost]
        public ActionResult DownloadInvoiceXML(int salerecid = 0)
        {
            //[100171]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (salerecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invoice not selected!</h1></a>");
            }
            bool viewper = mc.getPermission(Entry.EInvoice_Updation, permissionType.Add);
            bool downloadper = mc.getPermission(Entry.EInvoice_Updation, permissionType.Edit);
            if (viewper == false || downloadper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //Session["xsid"] = objCookie.getUserId();
            EInvoiceSaleMdl slMdl = new EInvoiceSaleMdl();
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            //
            string dirpath = Server.MapPath("~/App_Data/TallyFile/");
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dirpath);
            if (!dirinfo.Exists) { dirinfo.Create(); };
            string path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(salerecid.ToString() + "_EInv.xml"));
            DataSet ds = slBll.getEInvoiceSaleData(salerecid);
            ds.WriteXml(path);
           
            //to read xml in dataset
            //DataSet dsRead = new DataSet();
            //string filepath = path;
            //dsRead.ReadXml(filepath);
            //----------------------------

            return File(path, System.Net.Mime.MediaTypeNames.Application.Octet, ds.Tables[0].Rows[0]["StrVNo"].ToString() + ".xml");
        }

        [HttpPost]
        public ActionResult getEInvoiceExcel(int salerecid = 0)
        {
            return RedirectToAction("DownloadEInvoiceExcel", new { salerecid = salerecid }); 
        }

        [HttpGet]
        public ActionResult DownloadEInvoiceExcel(int salerecid)
        {
            //[100171]--excel
            if (mc.getPermission(Entry.Sales_Report, permissionType.Edit) == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (salerecid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invoice not selected!</h1></a>");
            }
            //
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            DataSet ds = new DataSet();
            ds = slBll.getEInvoiceForExcel(salerecid);
            string rptname = ds.Tables[0].Rows[0]["Inv No."].ToString();
            //
            var grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = ds.Tables[0];
            grid.DataBind();
            //
            Response.ClearContent();
            Response.Buffer = true;

            Response.AddHeader("content-disposition", "attachment; filename=" + rptname + ".xls");
            Response.ContentType = "application/ms-excel";

            //
            Response.Charset = "";
            //
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            //
            grid.RenderControl(htw);
            //
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            //
            return View();
            //
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteInvoiceNumbers(string term, int ccode = 0, string finyr = "")
        {
            SaleBLL sbll = new SaleBLL();
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = sbll.getInvoiceNoList(ccode, finyr);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["SaleRecId"].ToString(), ds.Tables[0].Rows[i]["InvNo"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
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

    }
}
