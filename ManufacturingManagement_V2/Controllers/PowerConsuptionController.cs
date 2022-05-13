using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class PowerConsuptionController : Controller
    {
        readonly string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        readonly PowerConsuptionBLL modelbll = new PowerConsuptionBLL();
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        // GET: PowerConsuption
        public ActionResult Index()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            setViewData();
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ModelState.Clear();
            model.Item_List = modelbll.PowerAllConsuptionList(compcode);
            model.Remark = "NA";
            return View(model);
        }
        // GET: PowerConsuptionAllCompanyList
        public ActionResult AllCompany()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            ModelState.Clear();
            model.Item_List = modelbll.PowerConsuptionAllCompanyList();
            return View(model);
        }

        // GET: PowerConsuption/Details/5
        [HttpGet]
        public ActionResult Edit_Record(int id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = modelbll.PowerAllConsuptionList(compcode).Find(x => x.Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        // POST: PowerConsuption/Create
        [HttpPost]
        public ActionResult AddEntry(PowerConsuptionMDI Cons)
        {
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            {
                try
                {
                    int result = modelbll.AddNewCunsuptionEntry(Cons);
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

        [HttpPost]
        public ActionResult Edit(PowerConsuptionMDI objModel)
        {
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = modelbll.PowerUpdate_ConsuptionEntry(objModel);
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
        public ActionResult Delete_Entry(int id)
        {
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = modelbll.PowerConsuptionDelete(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        //UpdateCompany
        [HttpGet]
        public ActionResult Edit_CompanyRecord(int? id)
        {
            int comcode = Convert.ToInt32(objCookie.getCompCode());
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            PowerConsuptionBLL modelbll1 = new PowerConsuptionBLL();
            var hl = modelbll1.PowerAllConsuptionList(comcode).Find(x => x.Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_Company(PowerConsuptionMDI objModel)
        {
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            PowerConsuptionBLL modelbll1 = new PowerConsuptionBLL();
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = modelbll1.PowerCompanyUpdate(objModel);
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
        //View Details
        [HttpGet]
        public ActionResult ViewPowerFactorRecord(int? id)
        {
            int comcode = Convert.ToInt32(objCookie.getCompCode());
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            PowerConsuptionBLL modelbll1 = new PowerConsuptionBLL();
            var hl = modelbll1.PowerAllConsuptionList(comcode).Find(x => x.Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ViewPowerFactor(PowerConsuptionMDI objModel)
        {
            PowerConsuptionMDI model = new PowerConsuptionMDI();
            PowerConsuptionBLL modelbll1 = new PowerConsuptionBLL();
            if (mc.getPermission(Entry.PowerConsuption, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = modelbll1.PowerCompanyUpdate(objModel);
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
            return RedirectToAction("Index", "Home");
        }


    }
}
