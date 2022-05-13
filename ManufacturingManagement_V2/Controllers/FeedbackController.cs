using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class FeedbackController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private FeedbackBLL bllObject = new FeedbackBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string fbstatus = "p")
        {
            ViewBag.FBStatusList = new SelectList(mc.getFeedbackStatus(), "Value", "Text", fbstatus);
        }

        // GET: /FeedbackView/
        public ActionResult FeedbackView(string fbstatus = "p")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            List<FeedbackMdl> modelObject = new List<FeedbackMdl> { };
            modelObject = bllObject.getFeedbackBySendingUserList(fbstatus);
            ViewBag.Status = fbstatus;
            ViewBag.ReplyMsg = "0";
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Edit) == true)
            {
                ViewBag.ReplyMsg = "1";
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterFeedbackView(FormCollection form)
        {
            string fbstatus = form["ddlFBStatus"].ToString();
            return RedirectToAction("FeedbackView", new { fbstatus = fbstatus });
        }

        //POST: /create feedback
        [HttpPost]
        public ActionResult CreateFeedback(string suggestion)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (suggestion.Length == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Entry!</h1></a>");
            }
            bllObject.insertFeedback(suggestion);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("FeedbackView");
        }

        // GET: /update feedback/
        //[HttpPost]
        public ActionResult UpdateFeedback(int recid, string suggestion)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (suggestion.Length == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Entry!</h1></a>");
            }
            bllObject.updateFeedback(recid, suggestion);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("FeedbackView");
        }

        // GET: /ReplyView/
        public ActionResult ReplyView(string fbstatus = "p")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<FeedbackMdl> modelObject = new List<FeedbackMdl> { };
            modelObject = bllObject.getObjectList(fbstatus);
            ViewBag.Status = fbstatus;
            ViewBag.DeletePer = "0";
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Delete) == true)
            {
                ViewBag.DeletePer = "1";
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterReplyView(FormCollection form)
        {
            string fbstatus = form["ddlFBStatus"].ToString();
            return RedirectToAction("ReplyView", new { fbstatus = fbstatus });
        }

        // GET: /update feedbackstatus/
        //[HttpPost]
        public ActionResult UpdateFeedbackStatus(int recid, string replymsg)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (replymsg.Length==0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Entry!</h1></a>");
            }
            setViewData();
            bllObject.updateFeedbackStatus(recid, replymsg);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("ReplyView");
        }

        //[HttpPost]
        public ActionResult ResetFeedbackStatus(int recid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.resetFeedbackStatus(recid);
            ViewBag.Msg = bllObject.Message;
            return RedirectToAction("ReplyView");
        }

        [HttpPost]
        public ActionResult Delete(int recid = 0)
        {
            if (mc.getPermission(Entry.Feedback_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteFeedback(recid);
            if (bllObject.Result == false)
            {
                return Content(bllObject.Message);
            }
            return RedirectToAction("ReplyView");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
