using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CompanyController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL bllObject = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index()
        {
            if (mc.getPermission(Entry.Company_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<CompanyMdl> modelObject = new List<CompanyMdl> { };
            modelObject = bllObject.getObjectList();
            return View(modelObject.ToList());
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Company_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            CompanyMdl modelObject = new CompanyMdl();
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

        // GET: /CreateUpdate1 --Just to Check Mobile Responsiveness
        public ActionResult CreateUpdate1(int id = 0)
        {
            if (mc.getPermission(Entry.Company_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            CompanyMdl modelObject = new CompanyMdl();
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
        public ActionResult CreateUpdate(CompanyMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["AddEdit"] = "Save";
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.CompCode == 0)//add mode
                {
                    if (mc.getPermission(Entry.Company_Master, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertCompany(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.Company_Master, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateCompany(modelObject);
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
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Company_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            CompanyMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            //no provision to delete company at all
            return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //if (mc.getPermission(Entry.Company_Master, permissionType.Delete) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            //bllObject.deleteObject(id);
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}