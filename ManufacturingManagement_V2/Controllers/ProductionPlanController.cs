using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ProductionPlanController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ProductionPlanBLL bllObject = new ProductionPlanBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int ppmonth = 0, int ppyear = 0, int itemid=0, string itemcode="")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.ppmonth = ppmonth;
            ViewBag.ppyear = ppyear;
            ViewBag.itemid = itemid;
            ViewBag.itemcode = itemcode;
        }

        // GET: /
        public ActionResult Index(int ppmonth = 0, int ppyear=0, int itemid = 0, string itemcode="")
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //tmp
            if (ppmonth == 0) { ppmonth = DateTime.Now.Month; };
            if (ppyear == 0) { ppyear = DateTime.Now.Year; };
            setViewObject(ppmonth, ppyear,itemid,itemcode);
            List<ProductionPlanMdl> modelObject = new List<ProductionPlanMdl> { };
            modelObject = bllObject.getObjectList(ppmonth, ppyear,itemid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string ppmonth = form["txtMonth"].ToString();
            string ppyear = form["txtYear"].ToString();
            int itemid = 0;
            string itemcode = "";
            return RedirectToAction("Index", new { ppmonth = ppmonth, ppyear = ppyear, itemid=itemid, itemcode=itemcode });
        }

        // GET: /CreateUpdate
        [HttpGet]
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.ProductionEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ProductionPlanMdl modelObject = new ProductionPlanMdl();
            modelObject.PPMonth = DateTime.Now.Month;
            modelObject.PPYear = DateTime.Now.Year;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                ViewBag.ItemId = modelObject.ItemId;
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ProductionPlanMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }//--note
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.ProductionEntry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.setProductionPlan(modelObject);
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: /
        public ActionResult IndexStatus(int ppmonth = 0, int ppyear = 0)
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //tmp
            if (ppmonth == 0) { ppmonth = DateTime.Now.Month; };
            if (ppyear == 0) { ppyear = DateTime.Now.Year; };
            setViewObject(ppmonth, ppyear);
            List<ProductionPlanStatusMdl> modelObject = new List<ProductionPlanStatusMdl> { };
            modelObject = bllObject.getProductionPlanStatusList(ppmonth, ppyear);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexStatus(FormCollection form)
        {
            string ppmonth = form["txtMonth"].ToString();
            string ppyear = form["txtYear"].ToString();
            return RedirectToAction("IndexStatus", new { ppmonth = ppmonth, ppyear = ppyear });
        }

        [HttpPost]
        public JsonResult generateProductionPlan(int itemid, int ppmonth, int ppyear, double prdqty, double inspqty)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            setViewData();
            setViewObject();
            ProductionPlanMdl modelObject = new ProductionPlanMdl();
            modelObject.PPMonth = ppmonth;
            modelObject.PPYear = ppyear;
            modelObject.ItemId = itemid;
            modelObject.PrdQty = prdqty;
            modelObject.InspQty = inspqty;
            if (prdqty > 0)
            {
                if (mc.getPermission(Entry.ProductionEntry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
                }
            }
            if (inspqty > 0)
            {
                if (mc.getPermission(Entry.InspectionEntry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
                }
            }
            bllObject.setProductionPlan(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult generateSubassemblyPlanTotal(int ppmonth, int ppyear)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            setViewData();
            setViewObject();
            if (mc.getPermission(Entry.ProductionEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject.setPlanForSubAssemblyTotal(ppmonth, ppyear);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // GET: /
        public ActionResult DPDetail(int ppmonth, int ppyear, int itemid, string itemcode, string shortname, string unit, int monthfor)
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<ProductionPlanDPDetailMdl> modelObject = new List<ProductionPlanDPDetailMdl> { };
            modelObject = bllObject.getProductionPlanDPDetail(ppmonth, ppyear,itemid, monthfor);
            string mth = mc.getNameByKey(mc.getMonths(), "monthid", ppmonth.ToString(), "monthname");
            ViewBag.Month = mth + "-" + ppyear.ToString();
            ViewBag.ItemCode = itemcode;
            ViewBag.ShortName = shortname;
            ViewBag.Unit = unit;
            return View(modelObject.ToList());
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Production_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ProductionPlanMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAdmin = "0";
            if (objCookie.getLoginType() == 0)
            {
                ViewBag.IsAdmin = "1";
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.ProductionEntry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteProductionPlan(id);
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
