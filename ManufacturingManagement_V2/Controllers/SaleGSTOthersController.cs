using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class SaleGSTOthersController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private SaleBLL bllObject = new SaleBLL();
        private ItemBLL itemBLL = new ItemBLL();
        private CompanyBLL compBLL = new CompanyBLL();

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
            var stoptlist = mc.getScrapTransferOptionList();
            stoptlist.Remove(stoptlist.Find(s => s.Value.Equals("x")));
            ViewBag.ScrapTransferOptionList = new SelectList(stoptlist, "Value", "Text");
            //
            var cmplist = compBLL.getObjectList();
            cmplist.Remove(cmplist.Find(s => s.CompCode.Equals(Convert.ToInt32(objCookie.getCompCode()))));
            ViewBag.CompanyList = new SelectList(cmplist, "compcode", "cmpname");
            //
        }

        [HttpGet]
        public ActionResult Index(string dtfrom = "", string dtto = "", string invoicemode = "gst", string potype = "0", string railway = "", int railwayid = 0, string unloading = "0", string rc = "0", string billst = "0", string pmtst = "0", string rnote = "0", bool options = false)
        {
            if (mc.getPermission(Entry.Sales_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            //
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text", potype);
            ViewBag.InvoiceModeList = new SelectList(mc.getInvoiceModeRptList(), "Value", "Text", invoicemode);
            ViewBag.optListUnloading = new SelectList(mc.getPendingCompStatusList(), "Value", "Text", unloading);
            ViewBag.optListRC = new SelectList(mc.getPendingCompStatusList(), "Value", "Text", rc);
            ViewBag.optListBillSt = new SelectList(mc.getPendingCompStatusList(), "Value", "Text", billst);
            ViewBag.optListPmtSt = new SelectList(mc.getPendingCompStatusList(), "Value", "Text", pmtst);
            ViewBag.optListRNote = new SelectList(mc.getPendingCompStatusList(), "Value", "Text", rnote);
            List<SaleMdl> modelObject = new List<SaleMdl> { };
            if (options==true)
            {
                modelObject = bllObject.getObjectListV2(dtfrom, dtto, invoicemode, potype, railwayid, unloading, rc, billst, pmtst, rnote);
            }
            else
            {
                modelObject = bllObject.getObjectListV2(dtfrom, dtto, invoicemode, "0", 0, "0", "0", "0", "0", "0");
            }
            ViewBag.lgtype = objCookie.getLoginType();
            //
            ViewBag.dtfrom = dtfrom;
            ViewBag.dtto = dtto;
            ViewBag.railway = railway;
            ViewBag.railwayid = railwayid;
            ViewBag.Options = options;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string invoicemode = form["ddlInvoiceMode"].ToString();
            string potype = form["ddlPOType"].ToString();
            string railway = form["txtRailway"].ToString();
            int railwayid = 0;
            if (form["hfRailwayId"].ToString().Length > 0)
            {
                railwayid = Convert.ToInt32(form["hfRailwayId"].ToString());
            }
            if (railway.Length == 0)
            {
                railwayid = 0;
            }
            string unloading = form["ddlUnloading"].ToString();
            string rc = form["ddlRC"].ToString();
            string billst = form["ddlBillSt"].ToString();
            string pmtst = form["ddlPayment"].ToString();
            string rnote = form["ddlRNote"].ToString();
            bool options = false;
            if (form["chkOptions"] != null)
            {
                options = true;
            }
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, invoicemode = invoicemode, potype = potype, railway = railway, railwayid = railwayid, unloading = unloading, rc = rc, billst = billst, pmtst = pmtst, rnote = rnote, options = options });
        }

        [HttpGet]
        public ActionResult IndexConvertToMain(string dtfrom = "", string dtto = "", string invseries = "d")
        {
            if (mc.getPermission(Entry.Sales_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            //
            ViewBag.DraftPrfList = new SelectList(mc.getDraftProformaOptList(), "Value", "Text", invseries);
            List<SaleMdl> modelObject = new List<SaleMdl> { };
            modelObject = bllObject.getDraftProformaList(dtfrom, dtto, invseries);
            //
            ViewBag.dtfrom = dtfrom;
            ViewBag.dtto = dtto;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexConvertToMain(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string invseries = form["ddlInvSeries"].ToString();
            return RedirectToAction("IndexConvertToMain", new { dtfrom = dtfrom, dtto = dtto, invseries = invseries });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Scrap_Transfer, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //
            ViewData["AddEdit"] = "Save";
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.CashCreditList = new SelectList(mc.getCashCreditList(), "Value", "Text");
            ViewBag.SupplyTypeList = new SelectList(mc.getSupplyTypeList(), "Value", "Text");
            ViewBag.DocumentTypeList = new SelectList(mc.getDocumentTypeList(), "Value", "Text");
            ViewBag.OtherChgAcctList = new SelectList(bllObject.getOtherChargesAccountList(), "groupid", "groupname");
            //
            CompanyAddressBLL compaddr = new CompanyAddressBLL();
            List<CompanyAddressMdl> cmdl = compaddr.getObjectList(Convert.ToInt32(objCookie.getCompCode()));
            CompanyAddressMdl compaddrmdl = new CompanyAddressMdl();
            compaddrmdl.RecId = 0;
            compaddrmdl.CAddress = "---Select---";
            cmdl.Add(compaddrmdl);
            ViewBag.CompAddress = new SelectList(cmdl, "recid", "caddress");
            //admin-options
            ViewBag.AllowEdit = "1";
            ViewBag.AllowDisableGST = "0";
            if (objCookie.getLoginType() == 0)//is admin
            {
                ViewBag.AllowDisableGST = "1";
            }
            //
            SaleMdl modelObject = new SaleMdl();
            modelObject = bllObject.searchObject(id);
            if (modelObject.SaleRecId > 0)
            {
                ViewData["AddEdit"] = "Update";
                if (modelObject.InvoiceMode.ToLower() == "slr")
                {
                    //must not be sale return
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Attempt!</h1></a>");
                }
                //admin-options
                if (objCookie.getLoginType() != 0)//is not admin
                {
                    DateTime cdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    if (mc.getDateByString(modelObject.VDate).AddDays(1) < cdate)
                    {
                        ViewBag.AllowEdit = "0";
                    }
                }
                //
            }
            else
            {
                modelObject.VDate = mc.getStringByDate(DateTime.Now);
                modelObject.ChInv = "s";
                modelObject.CashCredit = "r";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(SaleMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            if (modelObject.ChInv.ToLower() == "x")//sale return
            {
                //must be be sale return
                return new JsonResult { Data = new { status = false, message = "Sale Return is not allowed from this entry" } };
            }
            bllObject = new SaleBLL();
            //defaults in context of sale entry others
            modelObject.InvoiceMode = mc.getInvoiceMode(modelObject.ChInv);
            modelObject.InvSeries = "m";
            modelObject.RetSaleRecId = 0;//note
            //
            string addedit = "";
            if (modelObject.SaleRecId == 0)//add mode
            {
                addedit = "Save";
                if (mc.getPermission(Entry.Scrap_Transfer, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = addedit } };
                }
                bllObject.generateMainInvoice(modelObject);
            }
            else//edit mode
            {
                addedit = "Update";
                if (mc.getPermission(Entry.Scrap_Transfer, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = addedit } };
                }
                bllObject.updateSaleEntry(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = addedit } };
        }

        [HttpGet]
        public ActionResult GoToEdit(int salerecid, string taxtype, string vtype)
        {
            if (taxtype.ToLower() == "s")
            {
                if (vtype.ToLower() == "si")
                {
                    return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/SaleEntry.aspx?salerecid="+salerecid+"" });
                }
                else
                {
                    return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/ScrapTransferEntry.aspx?salerecid=" + salerecid + "" });
                }
            }
            else if (taxtype.ToLower() == "g")
            {
                if (vtype.ToLower() == "gi")
                {
                    return RedirectToAction("../SaleGST/CreateUpdate", new { id = salerecid });
                }
                else
                {
                    return RedirectToAction("CreateUpdate", new { id = salerecid });
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DisplayCaseFile(int porderid)
        {
            //window.open('https://pragerp.com:130/Report/DisplayControlledDocument.aspx?strvalue=' + porderid + '.pdf?CaseFile?0?0?1', target = '_new');
            return RedirectToAction("DisplayErpV1File", "Home", new { url = "../Report/DisplayControlledDocument.aspx?strvalue=" + porderid + ".pdf?CaseFile?0?0?1" });
        }

        [HttpGet]
        public ActionResult InvoiceReportV1(int salerecid, string chinv)
        {
            int entryid = Convert.ToInt32(Entry.Sales_Report);
            return RedirectToAction("DisplayErpV1Report", "Home",
            new
            {
                reporturl = "../RptDetail/SaleInvoiceRptDetail.aspx",
                reportpms = "salerecid=" + salerecid + "*chinv=" + chinv, entryid = entryid,
                rptname = "Sale Invoice"
            });
        }

        [HttpPost]
        public JsonResult DeleteLedgerItem(int lrecid, int salerecid)
        {
            if (mc.getPermission(Entry.Scrap_Transfer, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            if (salerecid == 0)
            {
                return new JsonResult { Data = new { status = true, message = "OK" } };//note
            }
            bllObject = new SaleBLL();
            bllObject.deleteLedgerByLRecId(lrecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult UpdateSaleForReceiptedChallan(int salerecid, string rcinfo)
        {
            if (mc.getPermission(Entry.IC_DM_Updation, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new SaleBLL();
            bllObject.updateSaleForReceiptedChallan(salerecid,rcinfo);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult UpdateSaleForRNoteInfo(int salerecid, string rnoteinfo)
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new SaleBLL();
            bllObject.updateSaleForRNoteInfo(salerecid, rnoteinfo);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int salerecid)
        {
            if (objCookie.getLoginType() != 0)//not admin
            {
                return new JsonResult { Data = new { status = false, message = "Admin Access Required!", AddEdit = "Update" } };
            }
            if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new SaleBLL();
            bllObject.deleteSaleEntry(salerecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        public JsonResult DeleteDraftProformaSale(string invseries, int salerecid)
        {
            if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new SaleBLL();
            bllObject.deleteDraftProformaSaleEntry(invseries, salerecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteConsignee(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = bllObject.getConsigneeListData(0, "0");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ConsigneeId"].ToString(), ds.Tables[0].Rows[i]["ConsigneeIdName"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        //not in use
        [HttpGet] 
        public ActionResult IndexNew(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            bllObject = new SaleBLL();
            SaleMdl modelObject = new SaleMdl();
            modelObject.ObjectList = bllObject.getObjectList(dtfrom, dtto).ToList();
            ViewBag.lgtype = objCookie.getLoginType();
            return View(modelObject);
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
