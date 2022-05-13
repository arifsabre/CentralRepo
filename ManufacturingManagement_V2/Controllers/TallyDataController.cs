using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class TallyDataController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        TallyDataBLL objBll = new TallyDataBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        #region bom-definition

        //get
        public ActionResult UploadFileBOM()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "";
            if (Session["updmsgt"] != null)
            {
                ViewBag.Message = Session["updmsgt"].ToString();
                Session.Remove("updmsgt");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFileBOM(HttpPostedFileBase file)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            bool res = false;
            string msg = "";
            if (file != null && file.ContentLength > 0)
                try
                {
                    string dirpath = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/BOM/");
                    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dirpath);
                    if (!dirinfo.Exists)
                    {
                        dirinfo.Create();
                    }
                    string path = System.IO.Path.Combine(dirpath, mc.getStringByDateForReport(DateTime.Now) + "-" + System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    try
                    {
                        DataSet ds = mc.getDatasetByExcel(path, "BOM");
                        objBll = new TallyDataBLL();
                        objBll.GenerateBOMByExcel(ds);
                        res = objBll.Result;
                        msg = objBll.Message;
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if (res == true)
                    {
                        ViewBag.Message = "File uploaded successfully. " + msg;
                    }
                    else
                    {
                        ViewBag.Message = "Error in File Upload. " + msg;
                    }
                    Session["updmsgt"] = ViewBag.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View();
                }
            else
            {
                ViewBag.Message = "No file selected to upload.";
                return View();
            }
            return RedirectToAction("UploadFileBOM");
        }

        //get
        public ActionResult DownloadsBOM()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/BOM/"));
            List<string> items = new List<string>();
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }
            }
            return View(items);
        }

        //get
        public FileResult DownloadBOM(string fileName)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                string err = Server.MapPath("~/App_Data/EmpDocs/");
                fileName = "Permission.png";
                return File(err + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            //
            string path = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/BOM/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        #endregion bom-definition

        #region tally-drr

        //get
        public ActionResult UploadFileDRR()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "";
            if (Session["updmsgt"] != null)
            {
                ViewBag.Message = Session["updmsgt"].ToString();
                Session.Remove("updmsgt");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFileDRR(HttpPostedFileBase file)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            bool res = false;
            string msg = "";
            if (file != null && file.ContentLength > 0)
                try
                {
                    string dirpath = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/DRR/");
                    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dirpath);
                    if (!dirinfo.Exists)
                    {
                        dirinfo.Create();
                    }
                    string path = System.IO.Path.Combine(dirpath, mc.getStringByDateForReport(DateTime.Now) + "-" + System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    try
                    {
                        DataSet ds = mc.getDatasetByExcel(path, "DRR");
                        objBll= new TallyDataBLL();
                        objBll.SaveTallyDrrByExcel(ds);
                        res = objBll.Result;
                        msg = objBll.Message;
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if (res == true)
                    {
                        ViewBag.Message = "File uploaded. " + msg;
                    }
                    else
                    {
                        ViewBag.Message = "Error in File Upload. " + msg;
                    }
                    Session["updmsgt"] = ViewBag.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View();
                }
            else
            {
                ViewBag.Message = "No file selected to upload.";
                return View();
            }
            return RedirectToAction("UploadFileDRR");
        }

        //get
        public ActionResult DownloadsDRR()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/DRR/"));
            List<string> items = new List<string>();
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }
            }
            return View(items);
        }

        //get
        public FileResult DownloadDRR(string fileName)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                string err = Server.MapPath("~/App_Data/EmpDocs/");
                fileName = "Permission.png";
                return File(err + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            //
            string path = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/DRR/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        #endregion tally-drr

        #region item-opening

        //get
        public ActionResult UploadFileItemOpening()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "";
            if (Session["updmsgt"] != null)
            {
                ViewBag.Message = Session["updmsgt"].ToString();
                Session.Remove("updmsgt");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFileItemOpening(HttpPostedFileBase file)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            bool res = false;
            string msg = "";
            if (file != null && file.ContentLength > 0)
                try
                {
                    string dirpath = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/ItemOpening/");
                    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(dirpath);
                    if (!dirinfo.Exists)
                    {
                        dirinfo.Create();
                    }
                    string path = System.IO.Path.Combine(dirpath, mc.getStringByDateForReport(DateTime.Now) + "-" + System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    try
                    {
                        DataSet ds = mc.getDatasetByExcel(path, "ItemOpening");
                        objBll = new TallyDataBLL();
                        objBll.GenerateItemOpeningByExcel(ds);
                        res = objBll.Result;
                        msg = objBll.Message;
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                    if (res == true)
                    {
                        ViewBag.Message = "File uploaded. " + msg;
                    }
                    else
                    {
                        ViewBag.Message = "Error in File Upload. " + msg;
                    }
                    Session["updmsgt"] = ViewBag.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View();
                }
            else
            {
                ViewBag.Message = "No file selected to upload.";
                return View();
            }
            return RedirectToAction("UploadFileItemOpening");
        }

        //get
        public ActionResult DownloadsItemOpening()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/ItemOpening/"));
            List<string> items = new List<string>();
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }
            }
            return View(items);
        }

        //get
        public FileResult DownloadItemOpening(string fileName)
        {
            if (mc.getPermission(Entry.UploadTallyData, permissionType.Add) == false)
            {
                string err = Server.MapPath("~/App_Data/EmpDocs/");
                fileName = "Permission.png";
                return File(err + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            //
            string path = Server.MapPath("~/App_Data/TallyFile/" + objCookie.getCompCode() + "/ItemOpening/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        #endregion item-opening

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
