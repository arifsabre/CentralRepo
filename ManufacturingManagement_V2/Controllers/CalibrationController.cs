using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CalibrationController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private IMTETypeBLL imteTypeBLL = new IMTETypeBLL();
        private ImteBLL imteBLL = new ImteBLL();
        private CalibrationBLL bllObject = new CalibrationBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int imtetypeid = 0, int imteid = 0, string dtfrom = "", string dtto = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.ImteTypeList = new SelectList(imteTypeBLL.getObjectList(), "imtetypeid", "imtetypename", imtetypeid);
            ViewBag.ImteList = new SelectList(imteBLL.getObjectList(imtetypeid), "imteid", "idno", imteid);
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        // GET: /
        public ActionResult Index(int imtetypeid = 0, int imteid = 0, string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(imtetypeid, imteid, dtfrom, dtto);
            List<CalibrationMdl> modelObject = new List<CalibrationMdl> { };
            modelObject = bllObject.getObjectList(imtetypeid,imteid,mc.getDateByString(dtfrom),mc.getDateByString(dtto));
            return View(modelObject.ToList().OrderBy(m => m.ImteTypeName));
        }

        // GET: / to check lte css
        public ActionResult IndexTest(int imtetypeid = 0, int imteid = 0, string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(imtetypeid, imteid, dtfrom, dtto);
            List<CalibrationMdl> modelObject = new List<CalibrationMdl> { };
            modelObject = bllObject.getObjectList(imtetypeid, imteid, mc.getDateByString(dtfrom), mc.getDateByString(dtto));
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            int imtetypeid = 0;
            if (form["ddlImteType"].ToString().Length > 0)
            {
                imtetypeid = Convert.ToInt32(form["ddlImteType"].ToString());
            }
            int imteid = 0;
            if (form["ddlImte"].ToString().Length > 0)
            {
                imteid = Convert.ToInt32(form["ddlImte"].ToString());
            }
            return RedirectToAction("Index", new { imtetypeid = imtetypeid, imteid = imteid, dtfrom = dtfrom, dtto = dtto });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            CalibrationMdl modelObject = new CalibrationMdl();
            modelObject.CalibDate = DateTime.Now;
            modelObject.NextCalibDate = DateTime.Now;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        //POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(CalibrationMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }
            ViewBag.Message = "Permission Denied!";
            modelObject.IsTested = false;//note
            if (modelObject.RecId == 0)//add
            {
                if (mc.getPermission(Entry.Calibration_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Calibration_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        // GET: /MarkDeletion
        public ActionResult MarkDeletion()
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            CalibrationMdl modelObject = new CalibrationMdl();
            modelObject.CalibDate = DateTime.Now;
            modelObject.NextCalibDate = DateTime.Now;
            return View(modelObject);
        }

        //POST: MarkDeletion
        [HttpPost]
        public ActionResult MarkDeletion(CalibrationMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }
            ViewBag.Message = "Permission Denied!";
            modelObject.CertifiedBy = "Deleted";//certificateno for remarks
            modelObject.IsTested = true;//note
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Delete) == false)
            {
                return View();
            }
            bllObject.insertCalibrationForDamage(modelObject);
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            CalibrationMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Calibration_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            if (bllObject.Result == false)
            {
                return Content(bllObject.Message);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
