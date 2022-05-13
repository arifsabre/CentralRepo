using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    //modified-from-StockController
    public class WipToRg1Controller : Controller
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

        private void setViewObject(string dtfrom = "", string dtto = "")
        {
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.WipToRg1_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<StockLedgerMdl> modelObject = new List<StockLedgerMdl> { };
            modelObject = lgrBLL.getStockLedgerListForStock("wrg1", dtfrom, dtto);//note:vtype=wrg1
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.WipToRg1_Entry, permissionType.Add) == false)
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
            if (modelObject.VType.ToLower() != "wrg1")
            {
                return new JsonResult { Data = new { status = false, message = "Invalid attempt!" } };
            }
            bllObject = new StockBLL();
            modelObject.VType = "wrg1";//note
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.WipToRg1_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);//note
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.WipToRg1_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateObject(modelObject);//note
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (mc.getPermission(Entry.WipToRg1_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            if (recid == 0)
            {
                return new JsonResult { Data = new { status = true, message = "OK" } };//note
            }
            bllObject = new StockBLL();
            bllObject.deleteObject(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
