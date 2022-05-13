using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_HolidayController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        AAA_HolidayBLL Lists = new AAA_HolidayBLL();
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ModelState.Clear();
            AAA_HolidayMDI model = new AAA_HolidayMDI();
            model.HolidayList = Lists.Get_Holiday_List();
            return View(model);
        }

        public JsonResult EditHolidayRecord(int? id)
        {

            var hl = Lists.Get_Holiday_List().Find(x => x.HolidayId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateHoliday(AAA_HolidayMDI objModel)
        {
            try
            {
                int result = Lists.UpdateHoliday(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Holiday  Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult InsertHoliday(AAA_HolidayMDI HolidayModel)
        {
            try
            {
                int result = Lists.Insert_Holiday(HolidayModel);
                if (result == 1)
                {
                    ViewBag.Message = "Holiday Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

    }
}

