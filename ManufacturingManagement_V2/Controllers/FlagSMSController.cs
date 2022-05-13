using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class FlagSMSController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private EmployeeBLL bllObject = new EmployeeBLL();
        private CompanyBLL compBLL = new CompanyBLL();
        private QualificationBLL qualBLL = new QualificationBLL();
        private ExperienceDetailBLL expDetBLL = new ExperienceDetailBLL();
        private QualDetailBLL qualDetBLL = new QualDetailBLL();
        private FamilyDetailBLL familyDetBLL = new FamilyDetailBLL();
        private PFNomineeBLL pfNomineeDetBLL = new PFNomineeBLL();
        private FlagUpdateMDI flagobj = new FlagUpdateMDI();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult IndexUpdateSMS()
        {
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return View();
            }
            ModelState.Clear();
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            EmployeeMdl model = new EmployeeMdl();
            model.GetempList = objbll.Get_AllFlagList();
            return View(model);
        }

        public JsonResult EditSMSLateRecord(int? id)
        {
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            var hl = objbll.Get_AllFlagList().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update_SMSLate(EmployeeMdl objModel)
        {
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return View(objModel);
            }
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            try
            {
                objbll.SMSUpdateLate(objModel);
                if (objbll.Result == true)
                {
                    ViewBag.Message = "SMS Status  Updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }
                return RedirectToAction("IndexUpdateSMS");
            }
            catch
            {
                throw;
            }
        }
        //

        public JsonResult EditSMSAbsentRecord(int? id)
        {
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            var hl = objbll.Get_AllFlagList().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update_SMSAbsent(EmployeeMdl objModel)
        {
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return View(objModel);
            }
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            try
            {
                objbll.SMSUpdateAbsent(objModel);
                if (objbll.Result == true)
                {
                    ViewBag.Message = "SMS Status  Updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }
                return RedirectToAction("IndexUpdateSMS");
            }
            catch
            {
                throw;
            }
        }
        //
        public JsonResult EditSMSMissingRecord(int? id)
        {
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            var hl = objbll.Get_AllFlagList().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update_SMSMissing(EmployeeMdl objModel)
        {
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return View(objModel);
            }
            FlagUpdateMDI objbll = new FlagUpdateMDI();
            try
            {
                objbll.SMSUpdateMissing(objModel);
                if (objbll.Result == true)
                {
                    ViewBag.Message = "SMS Status  Updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }
                return RedirectToAction("IndexUpdateSMS");
            }
            catch
            {
                throw;
            }
        }



    }
}