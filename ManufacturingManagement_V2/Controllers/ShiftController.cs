using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class ShiftController : Controller
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
        [HttpGet]
        public ActionResult Index()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            setViewData();
           if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return View();
            }
            ModelState.Clear();
            Emp_Shift_bll objbll = new Emp_Shift_bll();
            Emp_Shift model = new Emp_Shift();
            Emp_Shift model1 = new Emp_Shift();
            ViewBag.ShiftList = new SelectList(objbll.GetAllShift(), "ShiftId", "ShiftName");
            model.Item_List = objbll.GetAllShift_ByCompany(compcode);
            model.Item_List1 = objbll.GetAllShiftDetail();
            return View(model);
       }

         [HttpPost]
        public ActionResult Index(Emp_Shift objModel)
        {
            setViewData();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return View(objModel);
            }
            Emp_Shift_bll objbll = new Emp_Shift_bll();
            try
            {
               
             int result= objbll.UpdateShiftBy_Company(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Updated Successfully";
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
        [HttpGet]
        public JsonResult IndexRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            Emp_Shift_bll objbll = new Emp_Shift_bll();
            var hl = objbll.GetAllShift_ByCompany(compcode).Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddShift(Emp_Shift objModel)
        {
            setViewData();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return View(objModel);
            }
            Emp_Shift_bll objbll = new Emp_Shift_bll();
            try
            {

                int result = objbll.Insert_Shift(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Save Successfully";
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