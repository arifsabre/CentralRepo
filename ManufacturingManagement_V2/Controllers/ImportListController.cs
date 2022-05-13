using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ImportListController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ImportListBLL bllObject = new ImportListBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", int itemid = 0, string itemcode = "", string finyear="")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddMonths(1)); };
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.ItemId = itemid;
            ViewBag.ItemCode = itemcode;
            ViewBag.fyr = finyear;
            ViewBag.IListCCode = objCookie.getCompCode();
        }

        // GET: /Attendance/
        public ActionResult Index(string dtfrom = "", string dtto = "", int itemid = 0, string itemcode = "")
        {
            if (mc.getPermission(Entry.Import_List, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(dtfrom,dtto,itemid,itemcode);
            List<ImportListMdl> modelObject = new List<ImportListMdl> { };
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddMonths(1)); };
            modelObject = bllObject.getObjectList(itemid, dtfrom, dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string icode = form["hfItemId"].ToString();
            int itemid = 0;
            if (icode.Length > 0) { itemid = Convert.ToInt32(icode); };
            string itemcode = form["txtItemCode"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, itemid = itemid, itemcode = itemcode });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int yr = 0, int mth = 0,int dy = 0, string msg = "")
        {
            if (mc.getPermission(Entry.Import_List, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ImportListMdl modelObject = new ImportListMdl();
            if (yr == 0 && mth == 0 && dy == 0)
            {
                modelObject.ImpDate = DateTime.Now;
            }
            else
            {
                modelObject.ImpDate = new DateTime(yr, mth, dy);
            }
            ViewBag.unitname = "Unit";
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                ViewBag.unitname = modelObject.UnitName;
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ImportListMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            bool editMode = false;
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.ImpId == 0)//add mode
                {
                    if (mc.getPermission(Entry.Import_List, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    editMode = true;
                    if (mc.getPermission(Entry.Import_List, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    if (editMode == false)//add mode
                    {
                        return RedirectToAction("CreateUpdate", new { id = 0, yr = modelObject.ImpDate.Year, mth = modelObject.ImpDate.Month, dy = modelObject.ImpDate.Day, msg = bllObject.Message });
                    }
                    else//edit mode
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Message = bllObject.Message;
                    return View(modelObject);
                }
            }
            return View(modelObject);
        }

        public ActionResult ProjectionList(string dtfrom = "", string dtto = "", int itemid = 0, string itemcode = "", string finyear="")
        {
            if (mc.getPermission(Entry.Import_List, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (finyear == "") { finyear = objCookie.getFinYear(); };
            setViewObject(dtfrom, dtto, itemid, itemcode, finyear);
            List<ImportListMdl> modelObject = new List<ImportListMdl> { };
            modelObject = bllObject.getPOProjectionImportList(itemid, dtfrom, dtto, finyear);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterProjectionList(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string icode = form["hfItemId"].ToString();
            string finyear = form["ddlFinYear"].ToString();
            int itemid = 0;
            if (icode.Length > 0) { itemid = Convert.ToInt32(icode); };
            string itemcode = form["txtItemCode"].ToString();
            return RedirectToAction("ProjectionList", new { dtfrom = dtfrom, dtto = dtto, itemid = itemid, itemcode = itemcode, finyear = finyear });
        }

        [HttpPost]
        public JsonResult saveImportList(int impid, int itemid, string impdate, double qty)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Import_List, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            ImportListMdl modelObject = new ImportListMdl();
            modelObject.ImpId = impid;
            modelObject.ImpDate = mc.getDateByString(impdate);
            modelObject.Qty = qty;
            modelObject.ItemId = itemid;
            modelObject.ItemCode = "x";
            bllObject = new ImportListBLL();
            bllObject.saveImportList(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int impid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Import_List, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new ImportListBLL();
            bllObject.deleteObject(impid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
