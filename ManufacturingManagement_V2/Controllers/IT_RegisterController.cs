using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class IT_RegisterController : Controller
    {
        // GET: IT_Register

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
        IT_Register model = new IT_Register();
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        [HttpGet]
        public ActionResult Index()
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.EMPName = new SelectList(Lists.get_EMPName_To_Issue(), "NewEmpId", "EmpName");
            ModelState.Clear();
            model.Item_List = model.IT_Register_GetAll_Record();

            model.Location = " ";
            model.Uses = " ";
            model.DeviceName = " ";
            model.SerialNumber = " ";
            model.LabelName = " ";
            model.Remark = " ";
            model.OSVersion = " ";
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(IT_Register empmodel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model.IT_Register_Insert(empmodel);
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

        //[HttpGet]
        //public ActionResult Edit_AddinstockItem_Record(int? id)
        //{

        //    var hl = Lists.Get_Item_List().Find(x => x.Tran_Id.Equals(id));
        //    return Json(hl, JsonRequestBehavior.AllowGet);

        //}
        [HttpGet]
        public ActionResult IT_Register_Record(int? id)
        {
            //int userid = Convert.ToInt32(objCookie.getUserId());
           // IT_Register model1 = new IT_Register();
            var hl = model.IT_Register_GetAll_Record().Find(x => x.Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IT_Register_Edit_Record(IT_Register objModel)
        {
            IT_Register model = new IT_Register();
            if (mc.getPermission(Entry.ItAssest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.IT_Register_Update(objModel);
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

        public ActionResult IT_Register_Delete_Record(int id)
        {
         
            if (mc.getPermission(Entry.ItAssest, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.IT_Register_Delete(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
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
        public ActionResult ChangeCompany()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            return View();
        }
        [HttpPost]
        public ActionResult ChangeCompany(FormCollection form)
        {
            string compcode = form["ddlCompany"].ToString();
            if (objCookie.getCompCode() != compcode)
            {
                loginBLL.changeCoockieForCompany(compcode);
            }
            return RedirectToAction("Index");
        }
    }
}