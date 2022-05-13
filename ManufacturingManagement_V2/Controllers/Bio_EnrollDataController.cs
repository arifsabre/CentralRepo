using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class Bio_EnrollDataController : Controller
    {
        // GET: Bio_EnrollData
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        BioEnroll_DataMDI biobll = new BioEnroll_DataMDI();
        BioEnroll_DataMDI model = new BioEnroll_DataMDI();
        private void SetViewData()
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
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            SetViewData();
            model.Item_List = biobll.GetAll_BioEnrolled();
            return View(model);
        }
       [HttpGet]
        public ActionResult BioEmpRolledMap(int? id)
        {
            //int userid = Convert.ToInt32(objCookie.getUserId());
            // IT_Register model1 = new IT_Register();
            var hl = biobll.GetAll_BioEnrolled().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult BioEmpRolledMap(BioEnroll_DataMDI objModel)
        {
            BioEnroll_DataMDI model = new BioEnroll_DataMDI();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MapBioIdToERP(objModel);
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

        [HttpGet]
        public ActionResult BioEmpNotRolledMap(int? id)
        {
            //int userid = Convert.ToInt32(objCookie.getUserId());
            // IT_Register model1 = new IT_Register();
            var hl = biobll.GetAll_NotMapBioId().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexMapBio()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            SetViewData();
            model.Item_List = biobll.GetAll_NotMapBioId();
            return View(model);
        }


        [HttpPost]
        public ActionResult BioEmpRolledMap1(BioEnroll_DataMDI objModel)
        {
            BioEnroll_DataMDI model = new BioEnroll_DataMDI();
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MapBioIdToERP(objModel);
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

                return RedirectToAction("IndexMapBio");
            }
            catch
            {
                throw;
            }
        }


    }
}