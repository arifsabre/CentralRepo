using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ItemReceiptController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ItemReceiptBLL bllObject = new ItemReceiptBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        private void setViewObject(string dtfrom = "", string dtto = "")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
        }

        #region indent receipt

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<ItemReceiptMdl> modelObject = new List<ItemReceiptMdl> { };
            modelObject = bllObject.getItemReceivingByIndent();
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
        public ActionResult CreateUpdate(int id = 0, int indentlgrid = 0, string msg = "")
        {
            if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.Message = msg;
            ItemReceiptMdl modelObject = new ItemReceiptMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchReceiptForIndent(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            else
            {
                if (id == 0 && indentlgrid != 0)
                {
                    modelObject = bllObject.getIndentLedgerInfo(indentlgrid);
                }
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ItemReceiptMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) {}            
            ViewBag.Message = "Permission Denied!";
            if (modelObject.StkLgr.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertIndentReceipt(modelObject.StkLgr);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateIndentReceipt(modelObject.StkLgr);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = bllObject.Message;
                if (ViewData["AddEdit"].ToString() == "Save")//add mode
                {
                    return RedirectToAction("CreateUpdate", new { id = 0, indentlgrid = modelObject.StkLgr.IndentLgrId, msg = bllObject.Message });
                }
                else//edit mode
                {
                    return RedirectToAction("CreateUpdate", new { id = modelObject.StkLgr.RecId, indentlgrid = modelObject.StkLgr.IndentLgrId, msg = bllObject.Message });
                }
            }
        }

        #endregion indent receipt

        #region po receipt

        // GET: /
        public ActionResult IndexPOItemReceipt(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<ItemReceiptMdl> modelObject = new List<ItemReceiptMdl> { };
            modelObject = bllObject.getItemReceivingByPurchaseOrder();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexPOItemReceipt(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("IndexPOItemReceipt", new { dtfrom = dtfrom, dtto = dtto });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdatePOItemReceipt(int id = 0, int orderlgrid = 0,string msg = "")
        {
            if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.Message = msg;
            ItemReceiptMdl modelObject = new ItemReceiptMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchReceiptForPurchaseOrder(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            else
            {
                if (id == 0 && orderlgrid != 0)
                {
                    modelObject = bllObject.getOrderLedgerInfo(orderlgrid);
                }
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdatePOItemReceipt(ItemReceiptMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) {}            
            ViewBag.Message = "Permission Denied!";
            if (modelObject.StkLgr.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertPurchaseOrderReceipt(modelObject.StkLgr);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updatePurchaseOrderReceipt(modelObject.StkLgr);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("IndexPOItemReceipt");
            }
            else
            {
                ViewBag.Message = bllObject.Message;
                if (ViewData["AddEdit"].ToString() == "Save")//add mode
                {
                    return RedirectToAction("CreateUpdatePOItemReceipt", new { id = 0, orderlgrid = modelObject.StkLgr.OrderLgrId, msg = bllObject.Message });
                }
                else//edit mode
                {
                    return RedirectToAction("CreateUpdatePOItemReceipt", new { id = modelObject.StkLgr.RecId, orderlgrid = modelObject.StkLgr.OrderLgrId, msg = bllObject.Message });
                }
            }
        }

        #endregion po receipt

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.ItemReceiptEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ItemReceiptMdl modelObject = bllObject.searchReceiptForIndent(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteReceipt(id);
            if (bllObject.Result == false)
            {
                return Content(bllObject.Message);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
