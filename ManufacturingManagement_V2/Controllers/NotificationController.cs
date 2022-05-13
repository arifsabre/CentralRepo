using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class NotificationController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private NotificationBLL bllObject = new NotificationBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string empid = "")
        {
            ViewData["AddEdit"] = "Send";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.EmpId = empid;
            ViewBag.MsgTypeList = new SelectList(mc.getMsgTypes(), "Value", "Text");
        }

        // GET: /Notification/
        public ActionResult Index(bool all = false)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            List<NotificationUserMdl> modelObject = new List<NotificationUserMdl> { };
            modelObject = bllObject.getRecentNotificationList(all);
            ViewBag.MsgInfo = "Recent " + modelObject.Count.ToString() + " Notifications";
            if (all == true)
            {
                ViewBag.MsgInfo = "All " + modelObject.Count.ToString() + " Notifications";
            }
            return View(modelObject.ToList());
        }

        public PartialViewResult NotificationLinks()
        {
            return PartialView();
        }

        #region testing
        // GET: /Notification/
        public JsonResult ChkMsg()
        {
            //ok and working, to get it in asp.net page use object performAPICall_GeneralPage()
            return Json(new{Data = new { recid = "1", noticemsg = "HelloBombay" }}, JsonRequestBehavior.AllowGet);
        }

        // GET: /Notification/
        public string ChkMsg1(string msg="")
        {
            return msg;
        }
        #endregion

        [HttpPost]
        public ActionResult getNotification()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            DataSet ds = new DataSet();
            ds = bllObject.getNotificationAlertData();
            DateTime msgdt = DateTime.Now;
            string recid = "0";
            string msg = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    msgdt = Convert.ToDateTime(ds.Tables[0].Rows[0]["noticedt"].ToString());
                    recid = ds.Tables[0].Rows[0]["recid"].ToString();
                    msg = "PRAG ERP Notification Number - " + ds.Tables[0].Rows[0]["NoticeNo"].ToString() +", Date "+ mc.getStringByDate(msgdt) + " " + msgdt.ToShortTimeString();
                    msg += "\r\n" + "From : " + ds.Tables[0].Rows[0]["SendingUserName"].ToString();
                    msg += "\r\n" + "Message : " + ds.Tables[0].Rows[0]["NoticeMsg"].ToString();
                }
            }
            return new JsonResult { Data = new { recid = recid, noticemsg = msg } };
        }

        //POST: /updateNotificationAttended
        //[HttpPost]
        public ActionResult updateNotificationAttended(int recid, string replymsg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject.updateNotificationAttended(recid,replymsg);
            return RedirectToAction("Index");
        }

        // GET: /Notification Reply List/
        public ActionResult NotificationReply(int noticeid=0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            List<NotificationUserMdl> modelObject = new List<NotificationUserMdl> { };
            modelObject = bllObject.getNotificationReplyList(noticeid);
            return View(modelObject.ToList());
        }

        // GET: /
        public ActionResult NoticeIndex(string dtfrom = "", string dtto = "", string userid = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.Notification_Entry, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            setViewData();
            setViewObject(dtfrom, dtto, userid);
            List<NotificationMdl> modelObject = new List<NotificationMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, userid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string userid = form["txtUserId"].ToString();
            return RedirectToAction("NoticeIndex", new { dtfrom = dtfrom, dtto = dtto, userid = userid });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.Notification_Entry, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            setViewData();
            setViewObject();
            NotificationMdl modelObject = new NotificationMdl();
            modelObject.NoticeDT = DateTime.Now;
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
        public ActionResult CreateUpdate(NotificationMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.NoticeId == 0)//add mode
                {
                    //if (mc.getPermission(Entry.Notification_Entry, permissionType.Add) == false)
                    //{
                    //    return View();
                    //}
                    bllObject.insertNotification(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    //if (mc.getPermission(Entry.Notification_Entry, permissionType.Edit) == false)
                    //{
                    //    return View();
                    //}
                    bllObject.updateNotification(modelObject);
                }
                if (bllObject.Result == true)
                {
                    if (modelObject.MsgType.ToLower() == "m")
                    {
                        return RedirectToAction("updateNotificationUsers", new { noticeid = modelObject.NoticeId });
                    }
                    return RedirectToAction("NoticeIndex");
                }
                else
                {
                    ViewBag.Message = bllObject.Message;
                    return View();
                }
            }
            return View(modelObject);
        }

        // GET: /updateNotificationUsers
        public ActionResult updateNotificationUsers(int noticeid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.Notification_Entry, permissionType.Edit) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            setViewData();
            setViewObject();
            NotificationMdl modelObject = new NotificationMdl();
            modelObject = bllObject.searchObject(noticeid);
            return View(modelObject);
        }

        // POST: /updateNotificationUsers
        [HttpPost]
        public JsonResult updateNotificationUsers(NotificationMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            //if (mc.getPermission(Entry.Notification_Entry, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            bllObject = new NotificationBLL();
            bllObject.updateNotificationUsers(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int noticeid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Notification_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new NotificationBLL();
            bllObject.deleteNotification(noticeid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
