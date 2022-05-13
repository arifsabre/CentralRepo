using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class InvoiceControlController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private InvoiceControlBLL bllObject = new InvoiceControlBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "")
        {
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Sale_Invoice_Main) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<InvoiceControlMdl> modelObject = new List<InvoiceControlMdl> { };
            modelObject = bllObject.getObjectList(mc.getDateByString(dtfrom),mc.getDateByString(dtto));
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto=dtto });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            setViewData();
            setViewObject();
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Sale_Invoice_Main) + "]";
                return Content(msg);
            }
            InvoiceControlMdl modelObject = new InvoiceControlMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);//search existing
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(InvoiceControlMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.updateInvoiceControl(modelObject);
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        // GET: /Update IRN
        public ActionResult UpdateIRNDetail(int id = 0, string msg="")
        {
            setViewData();
            setViewObject();
            if (id == 0)
            {
                msg = "<a href='#' onclick='javascript:window.close();'";
                msg += "<h1>Invoice not selected!</h1></a>";
                return Content(msg);
            }
            if (mc.getPermission(Entry.EInvoice_Updation, permissionType.Add) == false)
            {
                msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.EInvoice_Updation) + "]";
                return Content(msg);
            }
            ViewBag.Message = msg;
            SaleMdl slMdl = new SaleMdl();
            SaleBLL slBll = new SaleBLL();
            if (id != 0)
            {
                slMdl = slBll.searchObject(id);
                if (slMdl == null)
                {
                    return HttpNotFound();
                }
            }
            return View(slMdl);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult UpdateIRNDetail(SaleMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.EInvoice_Updation, permissionType.Delete) == false)
            {
                return View();
            }
            SaleBLL slBll = new SaleBLL();
            slBll.updateInvReferenceNo(modelObject);
            if (slBll.Result == true)
            {
                return RedirectToAction("UpdateIRNDetail", new { id = modelObject.SaleRecId, msg = slBll.Message });
            }
            ViewBag.Message = slBll.Message;
            return View(modelObject);
        }

        #region upload IRN information

        //get
        public ActionResult UploadIRNDetail(int salerecid = 0, string invoiceno = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (salerecid == 0)
            {
                string msg = "<a href='#' onclick='javascript:window.close();'";
                msg += "<h1>Invoice not selected!</h1></a>";
                return Content(msg);
            }
            ViewBag.salerecid = salerecid;
            ViewBag.invoiceno = invoiceno;
            ViewBag.Message = "";
            if (Session["upqrcode"] != null)
            {
                ViewBag.rfqid = Session["upqrcode"].ToString();
                Session.Remove("upqrcode");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadIRNDetail(HttpPostedFileBase docfile, int salerecid, string invoiceno = "")
        {
            if (mc.getPermission(Entry.EInvoice_Updation, permissionType.Edit) == false)
            {
                string msg1 = "<a href='#' onclick='javascript:window.history.back();'";
                msg1 += "<h1>Permission Denied!</h1></a>";
                msg1 += "<br/>[" + Convert.ToInt32(Entry.EInvoice_Updation) + "]";
                return Content(msg1);
            }
            setViewData();

            if (salerecid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }

            ViewBag.salerecid = salerecid;
            Session["upqrcode"] = salerecid;
            Session["updmsg"] = "Error in File Upload!";
            ViewBag.invoiceno = invoiceno;
            string msg = "";

            if (docfile != null && docfile.ContentLength > 0)
                try
                {
                    string dirpath = Server.MapPath("~/App_Data/TallyFile/");
                    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dirpath);
                    if (!dirinfo.Exists) {dirinfo.Create();};
                    //docfile.FileName
                    string path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(salerecid.ToString() + "_IRN.xml"));
                    docfile.SaveAs(path);
                    SaleBLL slBll = new SaleBLL();
                    try
                    {
                        //read xml in dataset
                        DataSet dsIRN = new DataSet();
                        dsIRN.ReadXml(path);
                        SaleMdl slMdl = new SaleMdl();
                        slMdl.InvReferenceNo = dsIRN.Tables[0].Rows[0]["IRN"].ToString();
                        slMdl.AckNo = dsIRN.Tables[0].Rows[0]["AckNo"].ToString();
                        slMdl.SekNo = dsIRN.Tables[0].Rows[0]["SekNo"].ToString();
                        slMdl.AckDate = dsIRN.Tables[0].Rows[0]["AckDate"].ToString();
                        slMdl.SaleRecId = salerecid;
                        slBll.updateInvReferenceNo(slMdl);
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if (slBll.Result == true)
                    {
                        msg = "File uploaded successfully. " + slBll.Message;
                    }
                    else
                    {
                        msg = "Error in File Upload. " + slBll.Message;
                    }
                    Session["updmsg"] = msg;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View();
                }
            else
            {
                Session["updmsg"] = "No file selected to upload!";
            }
            return RedirectToAction("UploadIRNDetail", new { salerecid = salerecid, invoiceno = invoiceno });
        }

        #endregion

        #region upload/download by database

        //get
        public ActionResult UploadFile(int salerecid = 0, string invoiceno = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            ViewBag.salerecid = salerecid;
            ViewBag.invoiceno = invoiceno;
            ViewBag.Message = "";
            if (Session["upqrcode"] != null)
            {
                ViewBag.rfqid = Session["upqrcode"].ToString();
                Session.Remove("upqrcode");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase docfile, int salerecid, string invoiceno = "")
        {
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Draft_Proforma_Invoice) + "]";
                return Content(msg);
            }
            setViewData();

            if (salerecid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }

            ViewBag.salerecid = salerecid;
            Session["upqrcode"] = salerecid;
            Session["updmsg"] = "Error in File Upload!";
            ViewBag.invoiceno = invoiceno;

            if (docfile != null)
            {
                System.IO.Stream str = docfile.InputStream;
                System.IO.BinaryReader Br = new System.IO.BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                InvoiceControlMdl modelObject = new InvoiceControlMdl();
                modelObject.FlName = docfile.FileName;
                modelObject.FileContent = FileDet;
                modelObject.SaleRecId = salerecid;
                bllObject.uploadInvoiceQRCodeFile(modelObject);
                Session["updmsg"] = bllObject.Message;
            }
            else
            {
                Session["updmsg"] = "No file selected to upload!";
            }
            return RedirectToAction("UploadFile", new { salerecid = salerecid, invoiceno = invoiceno });
        }

        [HttpGet]
        public FileContentResult ShowDocument(int salerecid = 0)
        {
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == false)
            {
                return null;
            }
            setViewData();
            InvoiceControlBLL bll = new InvoiceControlBLL();
            return File(bll.getInvoiceQRCodeFile(salerecid), bll.Message);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
