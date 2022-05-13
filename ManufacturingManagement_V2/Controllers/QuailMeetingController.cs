using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class QuailMeetingController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private QuailMeetingBLL bllObject = new QuailMeetingBLL();
        private QuailFunctionBLL quailFctnBLL = new QuailFunctionBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string empid = "", string empname = "")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.QuailFctnList = new SelectList(quailFctnBLL.getObjectList(), "fctnid", "fctnname");
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.EmpId = empid;
            ViewBag.EmpName = empname;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", string empid = "", string empname = "")
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.QuailBook_Entry) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            setViewObject(dtfrom, dtto, empid, empname);
            List<QuailMeetingMdl> modelObject = new List<QuailMeetingMdl> { };
            modelObject = bllObject.getObjectList();
            return View(modelObject.ToList());
        }

        // GET: /
        public ActionResult CalendarView(int mth = 0, int yr = 0)
        {
            if (mc.getPermission(Entry.QuailBook_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.QuailBook_Entry) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            if (mth == 0) { mth = DateTime.Now.Month; };
            if (yr == 0) { yr = DateTime.Now.Year; };
            ViewBag.Mth = mth;
            ViewBag.Yr = yr;
            List<QuailCalendarMdl> modelObject = new List<QuailCalendarMdl> { };
            modelObject = bllObject.getQuailCalendar(mth,yr);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public PartialViewResult QuailDialogPartial(int rno)
        {
            AdvanceMdl advmdl = new AdvanceMdl();
            advmdl.RecId = rno;
            ViewBag.QualPList = new SelectList(mc.getQuailPriorityList(), "Value", "Text");
            return PartialView("QuailDialogPartial", advmdl);
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string empid = form["hfEmpId"].ToString();
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, empid = empid, empname = empname });
        }

        [HttpPost]
        public ActionResult FilterCalendarView(FormCollection form)
        {
            int month = Convert.ToInt32(form["ddlMonth"].ToString());
            int year = Convert.ToInt32(form["txtYear"].ToString());
            return RedirectToAction("CalendarView", new { mth = month, yr = year });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
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
            QuailMeetingMdl modelObject = new QuailMeetingMdl();
            modelObject.QlbDate = DateTime.Now;
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
        public ActionResult CreateUpdate(QuailMeetingMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.QmId == 0)//add mode
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
