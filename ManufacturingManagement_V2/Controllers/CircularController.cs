using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CircularController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UserBLL bllUser = new UserBLL();
        private CircularBLL bllObject = new CircularBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string depcode = "0")
        {
            ViewBag.DepartmentList = new SelectList(bllUser.getDepartmentList(), "depcode", "department", depcode);
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.depcode = depcode;
        }

        // GET://not in use
        public ActionResult Index(string dtfrom = "", string dtto = "", string depcode = "0")
        {
            //if (mc.getPermission(Entry.Circular_View, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            //}
            setViewData();
            DateTime dt = objCookie.getFromDate();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(dt); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now); };
            setViewObject(dtfrom, dtto, depcode);
            List<CircularMdl> modelObject = new List<CircularMdl> { };
            modelObject = bllObject.getObjectList(mc.getDateByStringDDMMYYYY(dtfrom), mc.getDateByStringDDMMYYYY(dtto), depcode);
            return View(modelObject.ToList());
        }

        public ActionResult IndexNew(string dtfrom = "", string dtto = "", string depcode = "0")
        {
            //if (mc.getPermission(Entry.Circular_View, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            //}
            setViewData();
            ViewBag.baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            DateTime dt = objCookie.getFromDate();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(dt); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now); };
            setViewObject(dtfrom, dtto, depcode);
            bllObject = new CircularBLL();
            CircularMdl modelObject = new CircularMdl();
            //modelObject.CircularList = bllObject.getObjectList(mc.getDateByStringDDMMYYYY(dtfrom), mc.getDateByStringDDMMYYYY(dtto), depcode).Where(s => s.RptName.ToLower().Contains(rptname.ToLower())).ToList();
            modelObject.CircularList = bllObject.getObjectList(mc.getDateByStringDDMMYYYY(dtfrom), mc.getDateByStringDDMMYYYY(dtto), depcode).ToList();
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string depcode = form["ddlDepartment"].ToString();
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, depcode = depcode });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            CircularMdl modelObject = new CircularMdl();
            modelObject.CircularDate = DateTime.Now;
            modelObject.DepCode = "ofc";
            //modelObject.UserId = Convert.ToInt32(objCookie.getUserId());
            //modelObject.UserName = objCookie.getUserName();
            ViewBag.Status = "0";
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
        public ActionResult CreateUpdate(CircularMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            ViewBag.Status = "0";
            if (modelObject.RecId == 0)//add
            {
                if (mc.getPermission(Entry.Circular_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Circular_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                ViewBag.Status = "1";
            }
            return View(modelObject);
        }

        #region upload/download section

        //get
        public ActionResult UploadFile(int recid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.recid = recid;
            ViewBag.empname = "";
            ViewBag.Message = "";
            if (Session["updrecid"] != null)
            {
                ViewBag.recid = Session["updrecid"].ToString();
                Session.Remove("updrecid");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase docfile, int recid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (recid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            if (mc.getPermission(Entry.Circular_Entry, permissionType.Edit) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            ViewBag.recid = recid;
            Session["updrecid"] = recid;
            string dirpath = Server.MapPath("~/App_Data/CircularDocs/");
            string path = "";
            int cntr = 0;
            try
            {
                if (docfile != null && docfile.ContentLength > 0)
                {
                    path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(recid.ToString() + ".pdf"));
                    docfile.SaveAs(path);
                    cntr += 1;
                }
                ViewBag.Message = cntr.ToString() + " File(s) uploaded.";
                if (cntr == 0)
                {
                    ViewBag.Message = "No file selected to upload!";
                }
                if (cntr > 0)//note
                {
                    bllObject.updateCircularDocument(recid, true);
                }
                Session["updmsg"] = ViewBag.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                return View();
            }
            return RedirectToAction("UploadFile");
        }

        //get
        public ActionResult ShowDocument(int recid = 0)
        {
            if (recid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            //if (mc.getPermission(Entry.Circular_View, permissionType.Edit) == false)
            //{
            //    ViewBag.Message = "Permission Denied!";
            //    return View();
            //}
            string path = Server.MapPath("~/App_Data/CircularDocs/");
            if (System.IO.File.Exists(path + recid.ToString() + ".pdf") == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>File Not Uploaded!</h1></a>");
            }
            return File(path + recid + ".pdf", mc.getMimeType(recid.ToString() + ".pdf"));
        }

        #endregion //upload/download section

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Circular_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new CircularBLL();
            bllObject.deleteObject(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
