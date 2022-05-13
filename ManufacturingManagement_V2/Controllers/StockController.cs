using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class StockController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private StockBLL bllObject = new StockBLL();
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

        private void setViewObject(string dtfrom = "", string dtto = "", string vtype = "op")
        {
            ViewBag.VTypeList = new SelectList(mc.getStockVoucherTypeList(), "Value", "Text", vtype);
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "",string vtype = "op")
        {
            if (mc.getPermission(Entry.StoreEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto, vtype);
            List<StockLedgerMdl> modelObject = new List<StockLedgerMdl> { };
            modelObject = lgrBLL.getStockLedgerListForStock(vtype, dtfrom, dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string vtype = "op";
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            if (form["ddlVType"].ToString().Length > 0)
            {
                vtype = form["ddlVType"].ToString();
            }
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, vtype = vtype });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.StoreEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            ViewData["AddEdit"] = "Save";
            //note-this is by autocomplete now
            //ViewBag.ItemList = new SelectList(storeItemBLL.getObjectList(), "itemid", "itemname");
            //end-note
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.VTypeList = new SelectList(mc.getStockVoucherTypeList(), "Value", "Text","op");
            ViewBag.VDate = mc.getStringByDate(DateTime.Now);
            //
            StockMdl modelObject = new StockMdl();
            modelObject = bllObject.searchObject(id);
            if (modelObject.RecId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(StockMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new StockBLL();
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.StoreEntry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);//note
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.StoreEntry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateObject(modelObject);//note
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        #region indent issue view

        public ActionResult ViewIndentIssue(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.StoreEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(dtfrom, dtto);
            List<StockMdl> modelObject = new List<StockMdl> { };
            modelObject = bllObject.getIndentIssueList(dtfrom,dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult ViewIndentIssueFilter(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("ViewIndentIssue", new { dtfrom = dtfrom, dtto = dtto });
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.StoreEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            StockMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.StoreEntry, permissionType.Delete) == false)
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
