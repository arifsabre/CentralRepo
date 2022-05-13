using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class EMailTrackController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private EmailTrackBLL bllObject = new EmailTrackBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string emstatus = "p")
        {
            ViewBag.EmStatusList = new SelectList(mc.getBillStatusList(), "Value", "Text", emstatus);
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.emstatus = emstatus;
        }

        // GET:
        public ActionResult Index(string dtfrom = "", string dtto = "", string emstatus = "p")
        {
            if (mc.getPermission(Entry.Email_Track_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now.AddMonths(-6)); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddMonths(6)); };
            setViewObject(dtfrom, dtto, emstatus);
            List<EmailTrackMdl> modelObject = new List<EmailTrackMdl> { };
            modelObject = bllObject.getObjectList(mc.getDateByString(dtfrom), mc.getDateByString(dtto), emstatus);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string emstatus = form["ddlEmStatus"].ToString();
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, emstatus = emstatus });
        }

        // GET:
        public ActionResult IndexReply(string dtfrom = "", string dtto = "", string emstatus = "p")
        {
            if (mc.getPermission(Entry.Email_Track_Reply, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now.AddMonths(-6)); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddMonths(6)); };
            setViewObject(dtfrom, dtto, emstatus);
            List<EmailTrackMdl> modelObject = new List<EmailTrackMdl> { };
            modelObject = bllObject.getEmailTrackList(mc.getDateByString(dtfrom), mc.getDateByString(dtto), emstatus);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexReply(FormCollection form)
        {
            string emstatus = form["ddlEmStatus"].ToString();
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("IndexReply", new { dtfrom = dtfrom, dtto = dtto, emstatus = emstatus });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            EmailTrackMdl modelObject = new EmailTrackMdl();
            modelObject.EmDate = DateTime.Now;
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
        public ActionResult CreateUpdate(EmailTrackMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Email_Track_Entry, permissionType.Add) == false)
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Email_Track_Entry, permissionType.Edit) == false)
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
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
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult updateReply(int recid, string opt, string remarks)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Email_Track_Reply, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new EmailTrackBLL();
            bllObject.updateEmailTrackReply(recid, opt, remarks);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Email_Track_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmailTrackMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Email_Track_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
