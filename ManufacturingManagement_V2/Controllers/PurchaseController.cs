using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class PurchaseController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private PurchaseBLL bllObject = new PurchaseBLL();
        private VendorBLL vendorBLL = new VendorBLL();
        private ItemBLL itemBLL = new ItemBLL();
        private StockLedgerBLL lgrBLL = new StockLedgerBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "",string vtype="pc")
        {
            ViewBag.VTypeList = new SelectList(mc.getStockVoucherTypeList(), "Value", "Text",vtype);
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        public ActionResult Index(string dtfrom="",string dtto="",string vtype="pc")
        {
            if (mc.getPermission(Entry.PurchaseReport_Store, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom,dtto,vtype);
            List<StockLedgerMdl> modelObject = new List<StockLedgerMdl> { };
            modelObject = lgrBLL.getStockLedgerListForPurchase(vtype, dtfrom, dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto,vtype="pc"});
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.PurchaseEntry_Store, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.ItemOptList = new SelectList(mc.getItemOptionList(), "Value", "Text");
            //
            ViewData["AddEdit"] = "Save";
            //note
            //ViewBag.ItemList = new SelectList(storeItemBLL.getObjectList(), "itemid", "itemname");
            //ViewBag.VendorList = new SelectList(vendorBLL.getObjectList(), "vendorid", "vendorname");
            //end-note
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.VTypeList = new SelectList(mc.getStockVoucherTypeList(), "Value", "Text");
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            ViewBag.CashCreditList = new SelectList(mc.getCashCreditList(), "Value", "Text");
            //
            PurchaseMdl modelObject = new PurchaseMdl();
            modelObject = bllObject.searchObject(id);
            if (modelObject.PurchaseId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(PurchaseMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new PurchaseBLL();
            if (modelObject.PurchaseId == 0)//add mode
            {
                if (mc.getPermission(Entry.PurchaseEntry_Store, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.PurchaseEntry_Store, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateObject(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.PurchaseEntry_Store, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            PurchaseMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.PurchaseEntry_Store, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
