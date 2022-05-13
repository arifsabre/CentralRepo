using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_OR_EMP_SECURITYController : Controller
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

        B_OR_SECURITY_EMP_BLL Lists = new B_OR_SECURITY_EMP_BLL();

        public ActionResult Index()
        {

            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ModelState.Clear();
            B_OR_SECURITY_EMP_MODEL model = new B_OR_SECURITY_EMP_MODEL();
            model.EmpORList = Lists.Get_EMP_OR_List();
            return View(model);
        }


        public JsonResult Edit_OR_Security_Record(int? id)
        {

            var hl = Lists.Get_EMP_OR_List().Find(x => x.ECode.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_OR_Security_EMP(B_OR_SECURITY_EMP_MODEL objModel)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.Update_EMP_OR(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

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
        public ActionResult Insert_OR_SEcurity_EMP(B_OR_SECURITY_EMP_MODEL empmodel)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.Insert_EMP_OR(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
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
