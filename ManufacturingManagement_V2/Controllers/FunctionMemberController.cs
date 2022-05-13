using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class FunctionMemberController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private FunctionMemberBLL bllObject = new FunctionMemberBLL();
        private QuailFunctionBLL quailFctnBLL = new QuailFunctionBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        private void setViewObject(int fctnid = 0, int userid = 0, string username = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.QuailFctnList = new SelectList(quailFctnBLL.getObjectList(), "fctnid", "fctnname", fctnid);
            ViewBag.fnuserid = userid;
            ViewBag.fnusername = username;
        }

        // GET: /
        public ActionResult Index(int fctnid = 0, int userid = 0, string username = "")
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(fctnid,userid,username);
            List<FunctionMemberMdl> modelObject = new List<FunctionMemberMdl> { };
            modelObject = bllObject.getObjectList(fctnid,userid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int userid = Convert.ToInt32(form["hfUserId"].ToString());
            string username = form["txtFnMember"].ToString();
            int fctnid = 0;
            if (form["ddlFctnId"].ToString().Length > 0)
            {
                fctnid = Convert.ToInt32(form["ddlFctnId"].ToString());
            }
            return RedirectToAction("Index", new { fctnid = fctnid, userid = userid, username = username });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            FunctionMemberMdl modelObject = new FunctionMemberMdl();
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
        public ActionResult CreateUpdate(FunctionMemberMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    if (ViewData["AddEdit"].ToString() == "Update")
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            modelObject.UserId = 0;
            modelObject.FnMember = "";
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Delete) == false)
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
