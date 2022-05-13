using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AttendanceController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AttendanceBLL bllObject = new AttendanceBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int attyr = 0, int attmonth = 0, string attshift = "d", int joiningunit = 0, string grade = "0", int empid = 0, string empname = "")
        {
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text", attmonth);
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text", attshift);
            ViewBag.AttTypeList = new SelectList(mc.getAttendanceTypeList(), "Value", "Text");
            ViewBag.AttValueList = new SelectList(bllObject.getAttendanceValueList(), "attcode", "attname");
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname", joiningunit);
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text", grade);
            ViewBag.AttYear = attyr;
            ViewBag.EmpId = empid;
            ViewBag.EmpName = empname;
        }
        
        // GET: /Attendance/
        public ActionResult Index(string shift = "d", int attyear = 0, int attmonth = 0, int joiningunit = 0, string grade = "0", int empid = 0, string empname = "")
        {
            if (mc.getPermission(Entry.Attendance_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //note:
            DateTime dtx = DateTime.Now;
            if (attmonth == 0) { attmonth = dtx.Month; };
            if (attyear == 0) { attyear = dtx.Year; };
            //
            setViewObject(attyear,attmonth,shift,joiningunit,grade,empid,empname);
            List<AttendanceMdl> modelObject = new List<AttendanceMdl> { };
            if (empid == 0)
            {
                modelObject = bllObject.getObjectList(attmonth, attyear, shift, joiningunit, grade);
            }
            else
            {
                modelObject = bllObject.getObjectList(attmonth, attyear, shift, joiningunit, grade).Where(s => s.NewEmpId.Equals(empid)).ToList();
            }
            ViewBag.MonthYear = attmonth.ToString() + "-" + attyear.ToString();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string shift = form["ddlShift"].ToString();
            int month = Convert.ToInt32(form["ddlMonth"].ToString());
            int year = Convert.ToInt32(form["txtAttYear"].ToString());

            string junit = "0";
            if (form["ddlCompany"].ToString().Length > 0)
            {
                junit = form["ddlCompany"].ToString();
            }
            string empid = form["hfEmpId"].ToString();
            string empname = form["txtEmpName"].ToString();
            string grade = form["ddlGrade"].ToString();
            if (empname.Length == 0) { empid = "0"; }
            return RedirectToAction("Index", new { shift = shift, attyear = year, attmonth = month, joiningunit = junit, grade = grade, empid = empid, empname = empname });
        }

        // GET: /GenerateAttendance
        public ActionResult GenerateAttendance()
        {
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            return View();
        }

        // POST: /GenerateAttendance
        [HttpPost]
        public JsonResult GenerateAttendance(AttendanceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new AttendanceBLL();
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            if (modelObject.AllowModify == null) { modelObject.AllowModify = "0"; };
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false && modelObject.AllowModify == "1")
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.generateAttendance(modelObject.AttDay,modelObject.AttMonth,modelObject.AttYear,modelObject.AttShift,modelObject.AttValue,modelObject.AllowModify);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        //GET: /getAttendanceBatchwise
        public ActionResult getAttendanceBatchwise()
        {
            setViewData();
            setViewObject();
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            return View();
        }

        [HttpPost]
        public ActionResult getAttendanceBatchwise(FormCollection form)
        {
            int day = Convert.ToInt32(form["txtAttDay"].ToString());
            int month = Convert.ToInt32(form["ddlMonth"].ToString());
            int year = Convert.ToInt32(form["txtAttYear"].ToString());
            string shift = form["ddlShift"].ToString();
            int unit = Convert.ToInt32(form["ddlJoiningUnit"].ToString());
            string grade = form["ddlGrade"].ToString();
            return RedirectToAction("SetAttendanceBatchwise", new { attday = day, attmonth = month, attyear = year, shift = shift, unit = unit, grade = grade });
        }

        // GET: /SetAttendanceBatchwise
        public ActionResult SetAttendanceBatchwise(int attday,int attmonth,int attyear,string shift="d",int unit=0, string grade="0")
        {
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            AttendanceBatchMdl modelObject = new AttendanceBatchMdl();
            modelObject = bllObject.getAttendanceBatchObject(attday,attmonth, attyear, shift,unit,grade);
            return View(modelObject);
        }

        // POST: /SetAttendanceBatchwise
        [HttpPost]
        public JsonResult SetAttendanceBatchwise(AttendanceBatchMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            //string empid = modelObject.EmpAttendance[0].NewEmpId.Trim();
            bllObject = new AttendanceBLL();
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.setAttendanceBatchwise(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // GET: /EditAttendance
        public ActionResult EditAttendance(int id = 0)
        {
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.AttValueList = new SelectList(bllObject.getAttendanceValueList(), "attcode", "attname");
            AttendanceMdl modelObject = new AttendanceMdl();
            if (id > 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            return View(modelObject);
        }

        // POST: /EditAttendance
        [HttpPost]
        public ActionResult EditAttendance(AttendanceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.AttValueList = new SelectList(bllObject.getAttendanceValueList(), "attcode", "attname");
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //if (ModelState.IsValid) ... return View(modelObject);//note
            bllObject.updateObject(modelObject);
            ViewBag.Result = "0";
            if (bllObject.Result == true)
            {
                //return RedirectToAction("Index", new { result = "1"});
                ViewBag.Result = "1";
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        #region attendance record entry for employees joined in the mid of the month

        // GET: /GenerateAttendance
        public ActionResult GenerateAttendanceForBackdateJoinedEmployees()
        {
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.month = DateTime.Now.AddMonths(-1).Month;
            ViewBag.year = DateTime.Now.Year;
            return View();
        }

        [HttpPost]
        public JsonResult GenerateAttendanceForBackdateJoinedEmployees(AttendanceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new AttendanceBLL();
            if (mc.getPermission(Entry.AttendanceEntry, permissionType.Edit) == false && modelObject.AllowModify == "1")
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.generateAttendanceForBackdateJoinedEmployees(modelObject.AttMonth, modelObject.AttYear);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Attendance_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AttendanceMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

        // GET: /Attendance/
        public ActionResult chkmenu()
        {
            return View();
        }

        // GET: /Attendance/
        public ActionResult chkmenu1()
        {
            return View();
        }

    }
}
