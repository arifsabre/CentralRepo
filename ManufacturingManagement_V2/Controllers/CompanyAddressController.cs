using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CompanyAddressController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();
        private CompanyAddressBLL bllObject = new CompanyAddressBLL();

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
            if (mc.getPermission(Entry.Company_Address, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            }
            setViewData();
            ViewBag.baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            bllObject = new CompanyAddressBLL();
            CompanyAddressMdl modelObject = new CompanyAddressMdl();
            //modelObject.CircularList = bllObject.getObjectList()-
            //.Where(s => s.RptName.ToLower().Contains(rptname.ToLower())).ToList();
            modelObject.AddressList = bllObject.getObjectList().ToList();
            return View(modelObject);
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewData["AddEdit"] = "Save";
            CompanyAddressMdl modelObject = new CompanyAddressMdl();
            ViewBag.Status = "0";
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
        public ActionResult CreateUpdate(CompanyAddressMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add
            {
                if (mc.getPermission(Entry.Company_Address, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Company_Address, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                ViewBag.Status = "1";
            }
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Company_Address, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new CompanyAddressBLL();
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
