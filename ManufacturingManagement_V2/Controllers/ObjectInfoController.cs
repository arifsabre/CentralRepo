using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ObjectInfoController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ObjectInfoBLL bllObject = new ObjectInfoBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index(int groupid = 0, string rptname="", string sorton = "obj")
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname", groupid);
            ViewBag.RptName = rptname;
            ViewBag.SortOn = sorton;
            List<ObjectInfoMdl> modelObject = new List<ObjectInfoMdl> { };
            modelObject = bllObject.getObjectList(groupid,sorton).Where(s => s.RptName.ToLower().Contains(rptname.ToLower())).ToList();
            return View(modelObject.ToList());
        }

        #region testNew
        public ActionResult IndexNew(int groupid = 0, string rptname = "", string sorton = "obj")
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ModelState.Clear();
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname", groupid);
            ViewBag.RptName = rptname;
            ViewBag.SortOn = sorton;
            ObjectInfoMdl modelObject = new ObjectInfoMdl();
            modelObject.ObjectInfoList = bllObject.getObjectList(groupid, sorton).Where(s => s.RptName.ToLower().Contains(rptname.ToLower())).ToList();
            return View(modelObject);
        }

        [HttpGet]
        public ActionResult EditRecord(int id=0)
        {
            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            //ContractorIndentMDI model = new ContractorIndentMDI();
            //var hl = null; //model.GetAllCivilIndentDetail(compcode).Find(x => x.RecordId.Equals(id));
            ////return Json(hl, JsonRequestBehavior.AllowGet);
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname");
            ObjectInfoMdl modelObject = new ObjectInfoMdl();
            var hl = bllObject.searchObject(id);
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateRecord(ObjectInfoMdl modelObject)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.updateObject(modelObject);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("IndexNew");
        }

        [HttpPost]
        public ActionResult InsertRecord(ObjectInfoMdl modelObject)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.insertObject(modelObject);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("IndexNew");
        }

        public ActionResult DeleteRecord(int id)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            bllObject.deleteObject(id);
            ViewBag.Message = bllObject.Message;//if (bllObject.Result == true)
            ModelState.Clear();
            return RedirectToAction("IndexNew");
        }

        [HttpGet]
        public ActionResult TestNew(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                //return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname");
            ObjectInfoMdl modelObject = new ObjectInfoMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    //return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            //return PartialView(modelObject);
            return View(modelObject);
        }

        [HttpGet]
        public ActionResult ActionResponse(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                //return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname");
            ObjectInfoMdl modelObject = new ObjectInfoMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    //return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return PartialView(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult TestNew(ObjectInfoMdl modelObject)
        {
            //if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1")
            {
                ViewBag.Message = "Permission Denied!";
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            if (ModelState.IsValid)
            {
                if (modelObject.ObjectId == 0)//add mode
                {
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    //ViewBag.Result = "1";
                    //return RedirectToAction("Index");
                }
                ViewBag.Message = bllObject.Message;
            }
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname", modelObject.EntryId);
            //ViewBag.Result = bllObject.Result;
            //return PartialView(modelObject);
            return View(modelObject);
        }

        public ActionResult DetailsNew(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ObjectInfoMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAdmin = "0";
            if (objCookie.getLoginType() == 0)
            {
                ViewBag.IsAdmin = "1";
            }
            return View(modelObject);
        }

        #endregion

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string sorton = form["ddlSortOn"].ToString();
            int groupid = 0;
            if (form["ddlGroup"].ToString().Length > 0)
            {
                groupid = Convert.ToInt32(form["ddlGroup"].ToString());
            }
            string rptname = form["txtRptName"].ToString();
            return RedirectToAction("Index", new { groupid = groupid, rptname = rptname, sorton = sorton });
        }

        public ActionResult IndexList(int groupid = 0, string rptname = "", string sorton = "obj")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
            //
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupListForUsers(), "groupid", "groupname", groupid);
            ViewBag.RptName = rptname;
            ViewBag.SortOn = sorton;
            List<ObjectInfoMdl> modelObject = new List<ObjectInfoMdl> { };
            modelObject = bllObject.getObjectListForUsers(groupid, sorton).Where(s => s.RptName.ToLower().Contains(rptname.ToLower())).ToList();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexList(FormCollection form)
        {
            string sorton = form["ddlSortOn"].ToString();
            int groupid = 0;
            if (form["ddlGroup"].ToString().Length > 0)
            {
                groupid = Convert.ToInt32(form["ddlGroup"].ToString());
            }
            string rptname = form["txtRptName"].ToString();
            return RedirectToAction("IndexList", new { groupid = groupid, rptname = rptname, sorton = sorton });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname");
            ObjectInfoMdl modelObject = new ObjectInfoMdl();
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
        public ActionResult CreateUpdate(ObjectInfoMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1")
            {
                ViewBag.Message = "Permission Denied!";
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            if (ModelState.IsValid)
            {
                if (modelObject.ObjectId == 0)//add mode
                {
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Message = bllObject.Message;
            }
            ViewBag.ObjGroupList = new SelectList(bllObject.getObjectGroupList(), "groupid", "groupname", modelObject.EntryId);
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ObjectInfoMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAdmin = "0";
            if (objCookie.getLoginType() == 0)
            {
                ViewBag.IsAdmin = "1";
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (objCookie.getUserId() != "1")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}