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
    public class UserPermissionController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();
        private UserPermissionBLL bllObject = new UserPermissionBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index(int uid = 1, string uname = "admin", int hmenu = 0, int ccode = 2)
        {
            if (mc.getPermission(Entry.User_Permission, permissionType.Add) == false)
            {
                //return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a><br/>[" + Convert.ToInt32(Entry.User_Permission) + "]");
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a><br/>[" + Convert.ToInt32(Entry.User_Permission) + "]");
            }
            setViewData();
            ViewBag.MenuList = new SelectList(mc.getMainMenueList(), "Value", "Text", hmenu);
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(uid), "compcode", "cmpname", ccode);
            ViewBag.uid = uid;
            ViewBag.uname = uname;
            ViewBag.hmenu = hmenu;
            ViewBag.ccode = ccode;
            UserPermissionMdl modelObject = new UserPermissionMdl { };
            modelObject = bllObject.getUsersByPermissionForAllEntriesList(uid,ccode);
            
            List<Permission> per=new List<Permission> { };
            if (hmenu > 0)
            {
                //working for list ok--modelObject = modelObject.Where(m => m.Entries.All(p => p.EntryId.ToString().StartsWith(hmenu.ToString()))).ToList();
                modelObject.Entries = modelObject.Entries.Where(m => m.EntryId.ToString().StartsWith(hmenu.ToString())).ToList();
            }
            //if (sorton == "dp")
            //{
            //    modelObject = modelObject.OrderBy(x => x.Department).ToList();
            //}
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int uid = -1;
            if (form["hfUserId"].ToString().Length > 0)
            {
                uid = Convert.ToInt32(form["hfUserId"].ToString());
            }
            string uname = form["txtUserName"].ToString();
            int hmenu = 0;
            if (form["ddlMenu"].ToString().Length > 0)
            {
                hmenu = Convert.ToInt32(form["ddlMenu"].ToString());
            }
            int ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            return RedirectToAction("Index", new { uid = uid, uname = uname, hmenu = hmenu, ccode = ccode });
        }

        [HttpPost]
        public JsonResult updateUserPermission(UserPermissionMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.User_Permission, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new UserPermissionBLL();
            bllObject.saveUserPermission(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult updateDefaultPermission(UserPermissionMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.User_Permission, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new UserPermissionBLL();
            bllObject.setDefaultUserPermission(modelObject.UserId,modelObject.CompCode);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}