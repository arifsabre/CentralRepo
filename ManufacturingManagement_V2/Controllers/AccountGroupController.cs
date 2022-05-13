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
    public class AccountGroupController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AccountBLL bllObject = new AccountBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject()
        {
            ViewData["AddEdit"] = "Save";
        }

        public ActionResult Index()
        {
            if (mc.getPermission(Entry.Account_Group, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<AccountMdl> modelObject = new List<AccountMdl> { };
            modelObject = bllObject.getAccountWithGroupList(recType.Group);
            return View(modelObject.ToList());
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Account_Group, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            AccountMdl modelObject = new AccountMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchAccountGroup(id);
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
        public ActionResult CreateUpdate(AccountMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["AddEdit"] = "Save";
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Account_Group, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.Insert_Record_AccountGroup(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Account_Group, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.Update_Record_AccountGroup(modelObject.AcCode, modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult Delete(int accode)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Account_Group, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject.Delete_Account(accode);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}