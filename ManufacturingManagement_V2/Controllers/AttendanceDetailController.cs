using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AttendanceDetailController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AttendanceDetailBLL bllObject = new AttendanceDetailBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string shift = "d", string attv = "inc", int newempid = 0, string empname = "", int joiningunit = 0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text",shift);
            ViewBag.AttValueList = new SelectList(mc.getgetAttedanceDetailTypeList(), "Value", "Text",attv);
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname",joiningunit);
            DateTime dtm = objCookie.getDispToDate();
            if (dtto.Length == 0) { dtto = mc.getStringByDate(dtm); };
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(dtm.AddDays(-7)); };
            ViewBag.DtTo = dtto;
            ViewBag.DtFrom = dtfrom;
            ViewBag.NewEmpId = newempid;
            ViewBag.EmpName = empname;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", string shift = "d", string attv = "inc", int newempid = 0, string empname = "", int joiningunit = 0)
        {
            if (mc.getPermission(Entry.Incentive_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(dtfrom,dtto,shift,attv,newempid,empname,joiningunit);
            List<AttendanceDetailMdl> modelObject = new List<AttendanceDetailMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto,shift,newempid,attv,joiningunit);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string shift = form["ddlShift"].ToString();
            int newempid = Convert.ToInt32(form["hfNewEmpId"].ToString());
            string empname = form["txtEmpName"].ToString();
            string attvalue = form["ddlAttValue"].ToString();
            string junit = form["ddlCompany"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, shift = shift, attv = attvalue, newempid = newempid,empname = empname ,joiningunit = junit });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string msg = "", int dy = 0, int mt = 0, int yr = 0)
        {
            if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            AttendanceDetailMdl modelObject = new AttendanceDetailMdl();
            if (dy > 0 && mt > 0 && yr > 0)
            {
                modelObject.AttDate = new DateTime(yr, mt, dy);
            }
            else
            {
                modelObject.AttDate = DateTime.Now;
            }
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(AttendanceDetailMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    if (ViewData["AddEdit"].ToString() == "Update")
                    {
                        //return RedirectToAction("Index", new { newempid = modelObject.NewEmpId });
                        return RedirectToAction("Index");
                    }
                    else //add mode
                    {
                        return RedirectToAction("CreateUpdate", new { msg = bllObject.Message, dy = modelObject.AttDate.Day, mt = modelObject.AttDate.Month, yr = modelObject.AttDate.Year });
                    }
                }
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        //GET: /getAttendanceDetailBatchwise
        public ActionResult getAttendanceDetailBatchwise()
        {
            setViewData();
            setViewObject();
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text");
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult getAttendanceDetailBatchwise(FormCollection form)
        {
            int day = Convert.ToInt32(form["txtAttDay"].ToString());
            int month = Convert.ToInt32(form["ddlMonth"].ToString());
            int year = Convert.ToInt32(form["txtAttYear"].ToString());
            string shift = form["ddlShift"].ToString();
            string grade = form["ddlGrade"].ToString();
            return RedirectToAction("SetAttendanceDetailBatchwise", new { attday = day, attmonth = month, attyear = year, shift = shift, grade = grade });
        }

        // GET: /SetAttendanceDetailBatchwise
        public ActionResult SetAttendanceDetailBatchwise(int attday, int attmonth, int attyear, string shift = "d", string grade = "0")
        {
            if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            AttendanceDetailBatchMdl modelObject = new AttendanceDetailBatchMdl();
            modelObject = bllObject.generateAndGetAttendanceDetailBatchwise(attday, attmonth, attyear, shift, grade);
            return View(modelObject);
        }

        // POST: /SetAttendanceDetailBatchwise
        [HttpPost]
        public JsonResult SetAttendanceDetailBatchwise(AttendanceDetailBatchMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            //string empid = modelObject.EmpAttendance[0].EmpId.Trim();
            bllObject = new AttendanceDetailBLL();
            if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.setAttendanceDetailBatchwise(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Incentive_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AttendanceDetailMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.AttendanceDetailEntry, permissionType.Delete) == false)
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
