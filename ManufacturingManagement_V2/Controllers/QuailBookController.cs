using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class QuailBookController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private QuailBookBLL bllObject = new QuailBookBLL();
        private QuailMeetingBLL QmBLL = new QuailMeetingBLL();
        private QuailFunctionBLL quailFctnBLL = new QuailFunctionBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", int fctnid = 0)
        {
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.QuailFctnList = new SelectList(quailFctnBLL.getObjectList(), "fctnid", "fctnname", fctnid);
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int qmid = 0, int indx = 0, int colindex = 0, bool chksubitem = false, int qlbpriority = 0, int tp = 1)
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.QuailBook_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            //
            if (qmid == 0)
            {
                string strqmid = UserOptionBLL.Instance.getUserOptionValue(userOption.QmId.ToString());
                if (strqmid.Length == 0) { strqmid = "0"; };
                qmid = Convert.ToInt32(strqmid);
            }
            else
            {
                UserOptionBLL.Instance.setUserOptionValue(userOption.QmId.ToString(),qmid.ToString());
            }
            //
            QuailBookMdl modelObject = new QuailBookMdl();
            modelObject.QmId = qmid;
            ViewBag.indx = indx;
            ViewBag.colindex = colindex;
            if (qmid != 0)
            {
                modelObject = bllObject.searchQuailBook(qmid, chksubitem, qlbpriority);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            ViewBag.PrvQM = "";
            if (modelObject.QmStatus == 1)
            {
                ViewBag.PrvQM = "Previous/New";
            }
            ViewBag.chkSubItem = chksubitem;
            ViewBag.QlbPriority = qlbpriority;
            ViewBag.TP = tp;
            ViewBag.Function = modelObject.FctnName;
            ViewBag.FctnDate = "Date: " + mc.getStringByDate(modelObject.QlbDate);
            ViewBag.NextInfo = "Next Meeting On: " + modelObject.NextDateTime;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult UpdateQuailBookItem(QuailBookMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!", qmid = modelObject.QmId } }; };
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
            {
                { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            }
            setViewData();
            setViewObject();
            bllObject.updateQuailBookItem(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, qmid = modelObject.QmId } };
        }

        public JsonResult DeleteQlbItem(QuailBookMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!", qmid = modelObject.QmId } }; };
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
            {
                { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            }
            setViewData();
            setViewObject();
            bllObject.deleteQuailBookItem(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, qmid = modelObject.QmId } };
        }

        #region quail attendance

        [HttpGet]
        public ActionResult QuailAttendance(int qmid=0)
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.QuailBook_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            QuailBookMdl modelObject = new QuailBookMdl();
            List<QuailAttendanceMdl> objlist = new List<QuailAttendanceMdl>();
            modelObject.Attendance = objlist;
            if (qmid != 0)
            {
                //
                QuailMeetingMdl qlmeetingMdl = new QuailMeetingMdl();
                qlmeetingMdl = QmBLL.searchObject(qmid);
                ViewBag.QmInfo = "For: "+qlmeetingMdl.FctnName+", Date: "+ mc.getStringByDate(qlmeetingMdl.QlbDate);
                //
                modelObject = bllObject.getQuailAttendanceList(qmid, "main");
                if (modelObject.Attendance.Count == 0)
                {
                    modelObject = bllObject.getQuailAttendanceList(qmid, "default");
                    bllObject.setDefaultAttendance(modelObject);
                }
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            return View(modelObject);
        }

        public JsonResult AddAttendance(int qmid, int userid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
            {
                { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            }
            setViewData();
            setViewObject();
            bllObject.addAttendance(qmid, userid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, qmid=qmid } };
        }

        public JsonResult DeleteAttendance(int qmid, int userid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
            {
                { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            }
            setViewData();
            setViewObject();
            bllObject.deleteAttendance(qmid, userid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, qmid = qmid } };
        }

        #endregion

        public JsonResult Delete(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Edit) == false)
            {
                { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            }
            setViewData();
            setViewObject();
            //bllObject.deleteQuailBook(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
