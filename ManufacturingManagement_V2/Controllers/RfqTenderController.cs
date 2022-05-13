using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class RfqTenderController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private RfqTenderBLL bllObject = new RfqTenderBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", bool filterbydt = true, int itemid = 0, string itemcode = "", string offertype = "")
        {
            ViewBag.OfferTypeList = new SelectList(mc.getOfferTypeList(), "Value", "Text", offertype);
            ViewBag.FilterByDT = filterbydt;
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.OfferType = offertype;
            ViewBag.ItemCode = itemcode;
            ViewBag.ItemId = itemid;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "", bool filterbydt = true, int itemid = 0, string itemcode = "", string offertype = "")
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto, filterbydt, itemid, itemcode, offertype);
            List<RfqTenderMdl> modelObject = new List<RfqTenderMdl> { };
            modelObject = bllObject.getObjectList(mc.getDateByString(dtfrom),mc.getDateByString(dtto),filterbydt,itemid,offertype);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            bool filterbydt = false;
            if (form["chkFilterByDT"] != null)
            {
                filterbydt = true;
            }
            string offertype = form["ddlOfferType"].ToString();
            string itemid = form["hfItemId"].ToString();
            string itemcode = form["txtItemCode"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, filterbydt = filterbydt, itemid = itemid, itemcode = itemcode, offertype = offertype });
        }

        #region report

        [HttpPost]
        public ActionResult DisplayReport(FormCollection form)
        {
            //[100170]
            string strdtfrom = form["txtDtFrom"].ToString();
            string strdtto = form["txtDtTo"].ToString();
            bool filterbydt = false;
            if (form["chkFilterByDT"] != null)
            {
                filterbydt = true;
            }
            string offertype = form["ddlOfferType"].ToString();
            string stritemid = form["hfItemId"].ToString();
            string itemcode = form["txtItemCode"].ToString();
            //
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Tender_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Tender_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            DateTime dtfrom = mc.getDateByString(strdtfrom);
            DateTime dtto = mc.getDateByString(strdtto);
            if (stritemid.Length == 0 || itemcode.Length == 0)
            {
                stritemid = "0";
            }
            int itemid = Convert.ToInt32(stritemid);
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "RfqTender/getRfqTenderReport";
                string reportpms = "&dtfrom=" + mc.getStringByDateForReport(dtfrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(dtto) + "";
                reportpms += "&filterbydt=" + filterbydt + "";
                reportpms += "&itemid=" + itemid + "";
                reportpms += "&itemcode=" + itemcode + "";
                reportpms += "&offertype=" + offertype + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getRfqTenderReport", new { dtfrom = dtfrom, dtto = dtto, filterbydt = filterbydt, itemid = itemid, itemcode = itemcode, offertype = offertype });
        }

        public ActionResult getRfqTenderReport(DateTime dtfrom, DateTime dtto, bool filterbydt = false, int itemid=0, string itemcode="", string offertype = "")
        {
            //[100170]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "RfqTenderRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{vw_order_report.orderno}=" + orderno + " and {vw_order_report.compcode}=" + objCookie.getCompCode() + " and {vw_order_report.finyear}='" + objCookie.getFinYear() + "'";
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpttitle = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpttitle"];
            txtrpttitle.Text = "RFQ Tender List";
            if (filterbydt == true)
            {
                txtrpttitle.Text += ", Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            }
            if (offertype.Length > 0)
            {
                string offertypename = "RFQ";
                if (offertype.ToLower() == "b") { offertypename = "Budgetary"; };
                txtrpttitle.Text += ", Offer Type: " + offertypename;
            }
            if (itemid > 0)
            {
                txtrpttitle.Text += ", Item: " + itemcode;
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for- usp_jobwork_issue_receipt
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@filterbydt", filterbydt);
            rptDoc.SetParameterValue("@itemid", itemid);
            rptDoc.SetParameterValue("@offertype", offertype);
            rptDoc.SetParameterValue("@compcode", Convert.ToInt16(objCookie.getCompCode()));
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

        #endregion

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            //
            RfqTenderMdl modelObject = new RfqTenderMdl();
            modelObject.OfferType = "r";
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            modelObject = bllObject.searchObject(id);
            if (modelObject.RfqId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(RfqTenderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new RfqTenderBLL();
            if (modelObject.RfqId == 0)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateObject(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        [HttpPost]
        public JsonResult deleteRFQ(int rfqid)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            bllObject.deleteObject(rfqid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #region upload/download section

        //get
        public ActionResult UploadFile(int rfqid = 0, string referenceno = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            ViewBag.rfqid = rfqid;
            ViewBag.refno = referenceno;
            ViewBag.Message = "";
            if (Session["uprfqid"] != null)
            {
                ViewBag.rfqid = Session["uprfqid"].ToString();
                Session.Remove("uprfqid");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        #region upload/download by database

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase docfile, int rfqid, string referenceno = "")
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Tender_Entry) + "]";
                return Content(msg);
            }
            setViewData();

            if (rfqid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            
            ViewBag.rfqid = rfqid;
            Session["uprfqid"] = rfqid;
            Session["updmsg"] = "Error in File Upload!";
            ViewBag.refno = referenceno;

            if (docfile != null)
            {
                System.IO.Stream str = docfile.InputStream;
                System.IO.BinaryReader Br = new System.IO.BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                RfqTenderMdl modelObject = new RfqTenderMdl();
                modelObject.FlName = docfile.FileName;
                modelObject.FileContent = FileDet;
                modelObject.RfqId = rfqid;
                bllObject.uploadRfqTenderFile(modelObject);
                Session["updmsg"] = bllObject.Message;
            }
            else 
            {
                Session["updmsg"] = "No file selected to upload!";
            }
            return RedirectToAction("UploadFile", new { rfqid = rfqid, referenceno = referenceno });
        }

        [HttpGet]
        public ActionResult ShowDocument(int rfqid = 0)
        {
            //note: ActionResult instead of FileContentResult
            string st = "";
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                return null;
            }
            setViewData();
            RfqTenderBLL bll = new RfqTenderBLL();
            try
            {
                return File(bll.getRfqTenderFile(rfqid), bll.Message);
            }
            catch (Exception ex)
            {
                st = ex.Message; 
                st = "File Not Uploaded!";//note
            }
            return Content("<a href='#' onclick='javascript:window.close();'><h1>" + st + "</h1></a>");
        }

        #endregion

        #region upload/download by file system -not in use

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile_1x(HttpPostedFileBase docfile, int rfqid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Tender_Entry) + "]";
                return Content(msg);
            }

            if (rfqid.ToString().Length == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            ViewBag.recid = rfqid;
            Session["updrecid"] = rfqid;
            string dirpath = Server.MapPath("~/App_Data/RfqTenderDocs/");
            string path = "";
            int cntr = 0;
            try
            {
                if (docfile != null && docfile.ContentLength > 0)
                {
                    path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(rfqid + ".pdf"));
                    docfile.SaveAs(path);
                    cntr += 1;
                }
                ViewBag.Message = cntr.ToString() + " File(s) uploaded.";
                if (cntr == 0)
                {
                    ViewBag.Message = "No file selected to upload!";
                }
                if (cntr > 0)//note
                {
                    //implement if required
                    //bllObject.updateWorklistDocument(Convert.ToInt32(rfqid), true);
                }
                Session["updmsg"] = ViewBag.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                return View();
            }
            return RedirectToAction("UploadFile");
        }

        [HttpGet]
        public ActionResult ShowDocument_1x(int rfqid = 0)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                return null;
            }
            string path = Server.MapPath("~/App_Data/RfqTenderDocs/");
            if (System.IO.File.Exists(path + rfqid + ".pdf") == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>File Not Uploaded!</h1></a>");
            }
            return File(path + rfqid + ".pdf", mc.getMimeType(rfqid + ".pdf"));
        }

        #endregion

        #endregion //upload/download section

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
