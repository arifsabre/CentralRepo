using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class MinimumWageController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private MinimumWageBLL bllObject = new MinimumWageBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int attyr = 0, int attmonth = 0, int compcode = 0)
        {
            ViewData["AddEdit"] = "Save";
            if (attmonth == 0) { attmonth = DateTime.Now.Month; }
            if (attyr == 0) { attyr = DateTime.Now.Year; }
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text", attmonth);
            ViewBag.AttYear = attyr;
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname",compcode);
        }

        // GET: /
        public ActionResult Index(int attyear = 0, int attmonth = 0, int compcode = 0)
        {
            if (mc.getPermission(Entry.MinimumWage, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            if (compcode == 0) { compcode = Convert.ToInt32(objCookie.getCompCode()); };
            setViewObject(attyear,attmonth,compcode);
            List<MinimumWageMdl> modelObject = new List<MinimumWageMdl> { };
            modelObject = bllObject.getObjectList(compcode);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int compcode = 0;
            if (form["ddlCompany"].ToString().Length > 0)
            {
                compcode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            return RedirectToAction("Index", new { compcode = compcode });
        }

        // GET: /CreateUpdate
        [HttpGet]
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.MinimumWage, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            setViewObject();
            MinimumWageMdl modelObject = new MinimumWageMdl();
            modelObject.AttYear = DateTime.Now.Year;
            modelObject.AttMonth = DateTime.Now.Month;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(MinimumWageMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }//--note
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.MinimumWage, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.MinimumWage, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = bllObject.Message;
                return View();
            }
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.MinimumWage, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            MinimumWageMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.MinimumWage, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a>");
            }
            //bllObject.deleteObject(id);//not applicable
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
