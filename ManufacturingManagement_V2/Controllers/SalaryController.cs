using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class SalaryController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private SalaryBLL bllObject = new SalaryBLL();
        private MinimumWageBLL minwageBLL = new MinimumWageBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int attyr=0,int attmonth=0,string shift="d")
        {
            if (attmonth == 0) { attmonth = DateTime.Now.Month; }
            if (attyr == 0) { attyr = DateTime.Now.Year; }
            ViewBag.MonthList = new SelectList(mc.getMonthList(), "Value", "Text",attmonth);
            ViewBag.ShiftList = new SelectList(mc.getShiftList(), "Value", "Text",shift);
            ViewBag.AttYear = attyr;
            MinimumWageMdl minwageMdl = new MinimumWageMdl();
            minwageMdl = minwageBLL.getMonthlyMinimumWage(attmonth,attyr);
            ViewBag.Skilled = minwageMdl.Skilled;
            ViewBag.SemiSkilled = minwageMdl.SemiSkilled;
            ViewBag.UnSkilled = minwageMdl.UnSkilled;
            ViewBag.VDate = mc.getStringByDate(DateTime.Now);
        }
        
        //POST: /getMinimumWage
        [HttpPost]
        public JsonResult getMinimumWage(int attmonth, int attyear)
        {
            MinimumWageMdl minwageMdl = new MinimumWageMdl();
            minwageMdl = minwageBLL.getMonthlyMinimumWage(attmonth, attyear);
            return new JsonResult { Data = new { skilled = minwageMdl.Skilled, semiskilled = minwageMdl.SemiSkilled, unskilled = minwageMdl.UnSkilled } };
        }

        // GET: /GenerateSalary
        public ActionResult GenerateSalary()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            DateTime dtx = DateTime.Now.AddMonths(-1);
            ViewBag.attyear = dtx.Year;
            ViewBag.attmonth = dtx.Month;
            return View();
        }

        // POST: /GenerateSalary
        [HttpPost]
        public JsonResult GenerateSalary(SalaryMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            if (modelObject.AllowModify == null) { modelObject.AllowModify = "0"; };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false && modelObject.AllowModify == "1")
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.generateSalary(modelObject.AttMonth, modelObject.AttYear, modelObject.AttShift, modelObject.AllowModify);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // GET: /GenerateIncentive
        public ActionResult GenerateIncentive()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            return View();
        }

        // POST: /GenerateIncentive
        [HttpPost]
        public JsonResult GenerateIncentive(SalaryMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            if (modelObject.AllowModify == null) { modelObject.AllowModify = "0"; };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false && modelObject.AllowModify == "1")
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.generateIncentive(modelObject.AttMonth, modelObject.AttYear, modelObject.AttShift, modelObject.AllowModify);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // GET: /SetDeductionList
        public ActionResult SetDeductionList(int smonth,int syear)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            SetDeductionAmountMdl modelObject = new SetDeductionAmountMdl();
            //modelObject = bllObject.getAdvanceOSToSetDeductionList(vdate);
            DateTime vdate = new DateTime(syear,smonth,DateTime.DaysInMonth(syear,smonth));
            modelObject = bllObject.getAdvanceOSToSetDeductionList(vdate);
            return View(modelObject);
        }

        // POST: /SetDeductionList
        [HttpPost]
        public JsonResult SetDeductionList(SetDeductionAmountMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            //string empid = modelObject.EmpAttendance[0].EmpId.Trim();
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.setAdvanceDeductionList(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // POST: /UpdateSalaryForAdvanceDeduction
        [HttpPost]
        public JsonResult UpdateSalaryForAdvanceDeduction(SalaryMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            //string empid = modelObject.EmpAttendance[0].EmpId.Trim();
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.updateSalaryForAdvanceDeduction(modelObject.AttMonth,modelObject.AttYear,modelObject.AttShift);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #region advance deduction single

        // GET: 
        public ActionResult SetAdvanceDeductionSingle(int dy = 0, int month = 0, int yr = 0, string msg = "")
        {
            if (mc.getPermission(Entry.SalaryEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AdvanceMdl advMdl = new AdvanceMdl();
            ViewBag.Message = msg;
            advMdl.AdvDate = DateTime.Now.AddMonths(-1);
            if (dy > 0 && month > 0 && yr > 0)
            {
                advMdl.AdvDate = new DateTime(yr, month, dy);
            }
            return View(advMdl);
        }

        // POST:
        [HttpPost]
        public ActionResult SetAdvanceDeductionSingle(AdvanceMdl advMdl)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.SalaryEntry, permissionType.Edit) == false)
            {
                return View(advMdl);
            }
            bllObject.updateAdvanceDeductionToSalarySingle(advMdl);
            return RedirectToAction("SetAdvanceDeductionSingle", new { dy = advMdl.AdvDate.Day, month = advMdl.AdvDate.Month, yr = advMdl.AdvDate.Year, msg = bllObject.Message });
        }

        #endregion

        #region setting advance deduction installment

        // GET:
        public ActionResult SetAdvanceDeductionInstallmemnt(string msg = "")
        {
            if (mc.getPermission(Entry.SalaryEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AdvanceMdl advMdl = new AdvanceMdl();
            ViewBag.Message = msg;
            return View(advMdl);
        }

        // POST: 
        [HttpPost]
        public ActionResult SetAdvanceDeductionInstallmemnt(AdvanceMdl advMdl)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject = new SalaryBLL();
            if (mc.getPermission(Entry.SalaryEntry, permissionType.Edit) == false)
            {
                return View(advMdl);
            }
            bllObject.updateAdvanceDeductionInstallment(advMdl);
            return RedirectToAction("SetAdvanceDeductionInstallmemnt", new { msg = bllObject.Message });
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            SalaryMdl modelObject = bllObject.searchObject(id);
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

    }
}
