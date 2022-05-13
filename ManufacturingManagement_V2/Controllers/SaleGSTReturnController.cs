using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class SaleGSTReturnController : Controller
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
            stoptlist = stoptlist.Where(s => s.Value.ToLower().Equals("x")).ToList();
            ViewBag.ScrapTransferOptionList = new SelectList(stoptlist, "Value", "Text");
            //
            var cmplist = compBLL.getObjectList();
            //cmplist.Find(s => s.CompCode.Equals(Convert.ToInt32(objCookie.getCompCode())));
            cmplist.Remove(cmplist.Find(s => s.CompCode.Equals(Convert.ToInt32(objCookie.getCompCode()))));
            ViewBag.CompanyList = new SelectList(cmplist, "compcode", "cmpname");
            //
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<SaleMdl> modelObject = new List<SaleMdl> { };
            modelObject = bllObject.getSaleReturnList(dtfrom, dtto);
            ViewBag.lgtype = objCookie.getLoginType();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto });
        }

        [HttpPost]
        public ActionResult SearchSaleToReturn(FormCollection form)
        {
            string retinvoiceno = form["txtRetInvoiceNo"].ToString();
            int retsalerecid = 0;
            if (form["hfRetSaleRecId"].ToString().Length > 0)
            {
                retsalerecid = Convert.ToInt32(form["hfRetSaleRecId"].ToString());
            }
            if (retinvoiceno.Length == 0 || retsalerecid == 0)
            {
                string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
                baseurl += "SaleGSTReturn/CreateUpdate";
                return Content("<a href='" + baseurl + "'><h1>Invalid Invoice Number!</h1></a>");
            }
            return RedirectToAction("CreateUpdate", new { id = 0, retsalerecid = retsalerecid, invno= retinvoiceno });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int retsalerecid = 0, string invno = "")
        {
            if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Add) == false)
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
            //
            SaleMdl modelObject = new SaleMdl();
            modelObject.VDate = mc.getStringByDate(DateTime.Now);
            modelObject.ChInv = "x";
            modelObject.CashCredit = "r";
            ViewBag.LoginType = objCookie.getLoginType();
            if ((id == 0 && retsalerecid == 0) || (id > 0 && retsalerecid > 0))
            {
                //case add mode or invalid case
                return View(modelObject);
            }
            else if (id > 0 && retsalerecid == 0)//edit mode
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject.SaleRecId > 0)
                {
                    ViewData["AddEdit"] = "Update";
                    if (modelObject.InvoiceMode.ToLower() != "slr")
                    {
                        //must be sale return only
                        modelObject = new SaleMdl();
                    }
                }
            }
            else if (id == 0 && retsalerecid > 0)//search mode
            {
                modelObject = bllObject.searchObject(retsalerecid);
                if (modelObject.SaleRecId > 0)
                {
                    //to set page in add mode
                    modelObject.SaleRecId = 0;
                    //set invoice in return type 
                    modelObject.ChInv = "x";
                    //to add record against selected ret-invoiceno
                    modelObject.RetSaleRecId = retsalerecid;
                    modelObject.RetInvoiceNo = invno;
                    //check for ret-invoiceno is not from sale return
                    if (modelObject.InvoiceMode.ToLower() == "slr")
                    {
                        modelObject = new SaleMdl();
                    }
                }
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
            if (modelObject.ChInv.ToLower() != "x")
            {
                //must be sale return only
                return new JsonResult { Data = new { status = false, message = "Sale Return is not allowed from this entry" } };
            }
            bllObject = new SaleBLL();
            //defaults in context of sale entry others
            modelObject.InvoiceMode = mc.getInvoiceMode(modelObject.ChInv);
            modelObject.InvSeries = "m";
            modelObject.VType = "gx";
            //
            string addedit = "";
            if (modelObject.SaleRecId == 0)//add mode
            {
                addedit = "Save";
                if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = addedit } };
                }
                bllObject.generateMainInvoice(modelObject);
            }
            else//edit mode
            {
                addedit = "Update";
                if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = addedit } };
                }
                bllObject.updateSaleEntry(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = addedit } };
        }

        [HttpPost]
        public JsonResult DeleteLedgerItem(int lrecid, int salerecid)
        {
            if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            if (salerecid == 0)
            {
                return new JsonResult { Data = new { status = true, message = "OK" } };
            }
            bllObject = new SaleBLL();
            bllObject.deleteLedgerByLRecId(lrecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int salerecid)
        {
            if (mc.getPermission(Entry.Sale_Return_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new SaleBLL();
            bllObject.deleteSaleEntry(salerecid);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteInvoiceNoToReturn(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = bllObject.getInvoiceNoListToReturn();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["SaleRecId"].ToString(), ds.Tables[0].Rows[i]["InvoiceNo"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
