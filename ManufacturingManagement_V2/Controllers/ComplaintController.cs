using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ComplaintController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ComplaintBLL bllObject = new ComplaintBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "",string comptype = "0",string compstatus = "p")
        {
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.CompTypeList = new SelectList(mc.getComplaintTypes(), "Value", "Text", comptype);
            ViewBag.CompStatusList = new SelectList(mc.getComplaintStatus(), "Value", "Text", compstatus);
        }

        // GET:
        public ActionResult ComplaintView()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            List<ComplaintMdl> modelObject = new List<ComplaintMdl> { };
            modelObject = bllObject.getComplaintListForUser_List();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult CreateComplaint(int comptype, string compmsg)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject.insertComplaint(comptype, compmsg);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("ComplaintView");
        }

        [HttpPost]
        public ActionResult UpdateComplaint(int compid, int comptype, string compmsg)
        {
            //not in use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject.updateComplaint(compid, comptype, compmsg);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("ComplaintView");
        }

        // GET: /ReplyView/
        public ActionResult ReplyView()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Complaint_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.CompStatusList = new SelectList(mc.getComplaintReplyStatus(), "Value", "Text");
            List<ComplaintMdl> modelObject = new List<ComplaintMdl> { };
            modelObject = bllObject.getComplaintListToReply_List();
            return View(modelObject.ToList());
        }

        // GET:
        public ActionResult ReplyViewForward()
        {
            //not in use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Complaint_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.CompStatusList = new SelectList(mc.getComplaintReplyStatus(), "Value", "Text");
            List<ComplaintMdl> modelObject = new List<ComplaintMdl> { };
            modelObject = bllObject.getComplaintListForReplyUser_List("f");//forwarded
            return View(modelObject.ToList());
        }

        // GET:
        public ActionResult ReplyViewClosed()
        {
            //not in use
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Complaint_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.CompStatusList = new SelectList(mc.getComplaintReplyStatus(), "Value", "Text");
            List<ComplaintMdl> modelObject = new List<ComplaintMdl> { };
            modelObject = bllObject.getComplaintListForReplyUser_List("c");//closed
            return View(modelObject.ToList());
        }

        // GET:
        public ActionResult ResolveComplaint(int compid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.ReplyUserList = new SelectList(bllObject.getReplyUserList(compid), "userid", "fullname");
            ComplaintMdl modelObject = new ComplaintMdl();
            modelObject = bllObject.searchObject(compid);
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult ReplyComplaint(ComplaintMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) 
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; 
            }
            setViewData();
            bllObject.replyComplaint(modelObject.CompId, modelObject.Remarks);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public ActionResult ForwardComplaint(ComplaintMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            bllObject.forwardComplaint(modelObject.CompId, modelObject.ForwardedTo, modelObject.Remarks);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        public PartialViewResult ComplaintLinks()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Delete(int recid = 0)
        {
            if (mc.getPermission(Entry.Complaint_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteComplaint(recid);
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
