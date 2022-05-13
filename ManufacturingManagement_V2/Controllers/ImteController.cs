using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ImteController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();
        private UserBLL bllUser = new UserBLL();
        private IMTETypeBLL imteTypeBLL = new IMTETypeBLL();
        private ImteBLL bllObject = new ImteBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int imtetypeid = 0, string location = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.ImteTypeList = new SelectList(imteTypeBLL.getObjectList(), "imtetypeid", "imtetypename", imtetypeid);
            ViewBag.LocationList = new SelectList(bllObject.getImteLocationList(imtetypeid), "location", "location", location);
            //ViewBag.SearchText = searchtext;
        }

        // GET: /
        public ActionResult Index(int imtetypeid = 0, string location = "")
        {
            if (mc.getPermission(Entry.Imte_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(imtetypeid, location);
            List<ImteMdl> modelObject = new List<ImteMdl> { };
            modelObject = bllObject.getObjectList(imtetypeid).Where(s => s.Location.ToLower().Contains(location.ToLower())).ToList();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int imtetypeid = 0;
            if (form["ddlImteType"].ToString().Length > 0)
            {
                imtetypeid = Convert.ToInt32(form["ddlImteType"].ToString());
            }
            string location = form["ddlLocation"].ToString();
            return RedirectToAction("Index", new { imtetypeid = imtetypeid, location = location });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Imte_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(0,"");
            ImteMdl modelObject = new ImteMdl();
            modelObject.InUseStatus = "In Use";
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

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ImteMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject(modelObject.ImteTypeId, modelObject.Location);
            //if (ModelState.IsValid) { }
            ViewBag.Message = "Permission Denied!";
            if (modelObject.ImteId == 0)//add
            {
                if (mc.getPermission(Entry.Imte_Master, permissionType.Add) == false)
                {
                    return View();
                }
                modelObject.IsInUse = true;//note
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Imte_Master, permissionType.Edit) == false)
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

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Imte_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ImteMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Imte_Master, permissionType.Delete) == false)
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
