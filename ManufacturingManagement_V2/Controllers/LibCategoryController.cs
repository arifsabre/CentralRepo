using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class LibCategoryController : Controller
    {
        // GET: LibCategory
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        readonly LibFileUploadBLL Lists = new LibFileUploadBLL();
        //readonly ErpConnection categoryName = new ErpConnection();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
      
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //ViewBag.CategoryList = new SelectList(Lists.GetCategoryListDropdownList(), "LibCategoryId", "LibCategory");
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI
            {
                Item_List = Lists.GetAllLibCategory()
            };
            return View(model);
        }

       

        [HttpPost]
        public ActionResult InsertLibCatgory(LibFileUploadMDI objModel)
        {
         
            try
            {
                int result = Lists.AddNewLibCategory(objModel);
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

        [HttpGet]
        public ActionResult Edit_LibCategoryRecord(int? id)
        {
            var hl = Lists.GetAllLibCategory().Find(x => x.LibCategoryId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_LibCategory(LibFileUploadMDI objModel)
        {
            try
            {
                int result = Lists.Update_LibCategory(objModel);
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
    public ActionResult DeleteLibCategory(int id)
        {
            try
            {
                int result = Lists.DeleteLibCategory(id);
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


       
    }
}