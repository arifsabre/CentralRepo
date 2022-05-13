using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ProductionEntryController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ProductionEntryBLL bllObject = new ProductionEntryBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom="",string dtto="")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", int itemid = 0, string itemcode = "")
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom,dtto);
            List<ProductionEntryMdl> modelObject = new List<ProductionEntryMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, itemid);
            ViewBag.itemid = itemid;
            ViewBag.itemcode = itemcode;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string itemcode = form["txtItem"].ToString();
            int itemid = 0;
            if (form["hfItemId"].ToString().Length > 0)
            {
                itemid = Convert.ToInt32(form["hfItemId"].ToString());
            }
            if (itemcode.Length == 0)
            {
                itemid = 0;
            }
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, itemid = itemid, itemcode = itemcode});
        }

        // GET: /CreateUpdate
        [HttpGet]
        public ActionResult CreateUpdate(int id = 0, string monthyear = "", string msg = "")
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ProductionEntryMdl modelObject = new ProductionEntryMdl();
            modelObject.EntryDate = DateTime.Now;
            ViewBag.Message = msg;
            if (monthyear.Length == 0)
            {
                //modelObject.MonthYear = mc.RightNChars("0" + DateTime.Now.Month.ToString(),2) + "-" + DateTime.Now.Year.ToString();
            }
            else 
            {
                modelObject.MonthYear = monthyear;
            }
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                ViewBag.PlanRecId = modelObject.PlanRecId;
                modelObject.ItemCode = modelObject.ItemCode +" [" + modelObject.ShortName+"]";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ProductionEntryMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }//--note
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.ProductionEntry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.ProductionEntry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                if (modelObject.RecId == 0)//add mode
                {
                    return RedirectToAction("CreateUpdate", new { id = 0, monthyear = modelObject.MonthYear, msg = bllObject.Message });
                }
                else//edit mode
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = bllObject.Message;
                return View();
            }
        }

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (mc.getPermission(Entry.ProductionEntry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
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
