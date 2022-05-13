using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class FormValueController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private FormValueBLL bllObject = new FormValueBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int ccode = 0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname", ccode);
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear", ccode);
        }

        // GET: /
        public ActionResult Index(int ccode = 0)
        {
            if (mc.getPermission(Entry.FormValue_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (ccode == 0) { ccode = Convert.ToInt32(objCookie.getCompCode()); };
            setViewObject(ccode);
            List<FormValueMdl> modelObject = new List<FormValueMdl> { };
            modelObject = bllObject.getObjectList(ccode);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            return RedirectToAction("Index", new { ccode = ccode });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.FormValue_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            FormValueMdl modelObject = new FormValueMdl();
            modelObject.PaymentDate = DateTime.Now;
            modelObject.FinYear = objCookie.getFinYear();
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
        public ActionResult CreateUpdate(FormValueMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.FormValue_Entry, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.FormValue_Entry, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.FormValue_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            FormValueMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.FormValue_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
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
