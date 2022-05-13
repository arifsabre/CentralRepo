using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class MasterDocumentNameController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        MasterDocumentNameBLL bllObject = new MasterDocumentNameBLL();

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
            ModelState.Clear();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "CmpName");
            MasterDocumentNameMdl objModel = new MasterDocumentNameMdl();
            objModel.MDocList = bllObject.getObjectList();
            return View(objModel);
        }

        public JsonResult EditRecord(int? id)
        {
            if (mc.getPermission(Entry.Document_Type_Master, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            var hl = bllObject.getObjectList().Find(x => x.DocumentId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateRecord(MasterDocumentNameMdl objModel)
        {
            if (mc.getPermission(Entry.Document_Type_Master, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.updateObject(objModel);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult InsertRecord(MasterDocumentNameMdl objModel)
        {
            if (mc.getPermission(Entry.Document_Type_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.insertObject(objModel);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteRecord(int id)
        {
            if (mc.getPermission(Entry.Document_Type_Master, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.deleteObject(id);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}

