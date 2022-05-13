using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CompanyPolicyController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();
        private DocumentFileBLL bllObject = new DocumentFileBLL();
        private DocSubCategoryBLL docSubCatgBLL = new DocSubCategoryBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int categoryid = 0, int subcategoryid=0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.CatgeoryList = new SelectList(bllObject.getDocumentCategoryData(44), "categoryid", "categoryname", categoryid);
            ViewBag.SubCatgeoryList = new SelectList(docSubCatgBLL.getObjectList(), "subcategoryid", "subcategoryname", subcategoryid);
        }

        public ActionResult Index(int categoryid = 0, int subcategoryid = 0)
        {
            if (mc.getPermission(Entry.Upload_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(categoryid,subcategoryid);
            List<DocumentFileMdl> modelObject = new List<DocumentFileMdl> { };
            modelObject = bllObject.getObjectList(44, categoryid, subcategoryid, 0);//note ccode=0=all companies
            return View(modelObject.ToList());
        }

        public ActionResult IndexView(int categoryid = 0, int subcategoryid = 0)
        {
            if (mc.getPermission(Entry.Download_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(categoryid,subcategoryid);
            List<DocumentFileMdl> modelObject = new List<DocumentFileMdl> { };
            modelObject = bllObject.getObjectList(44, categoryid, subcategoryid, 0);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int catgid = 0;
            //if (form["ddlCategory"].ToString().Length > 0)
            //{
            //    catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            //}
            int subcatgid = 0;
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            return RedirectToAction("Index", new { categoryid = catgid, subcategoryid = subcatgid});
        }

        [HttpPost]
        public ActionResult FilterIndexView(FormCollection form)
        {
            int catgid = 0;
            //if (form["ddlCategory"].ToString().Length > 0)
            //{
            //    catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            //}
            int subcatgid = 0;
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            return RedirectToAction("IndexView", new { categoryid = catgid, subcategoryid = subcatgid});
        }

        #region view document

        //get
        public ActionResult ShowDocument(string fileName, string categoryid, int subcategoryid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getDocumentPermissionToView("CompanyPolicies\\", 0, 0);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getDocumentPermissionToDownload("CompanyPolicies\\", 0, 0);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "CompanyPolicy/getDocumentFile";
                string reportpms = "fileName=" + fileName + "";
                reportpms += "&categoryid=" + categoryid + "";
                reportpms += "&subcategoryid=" + subcategoryid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDocumentFile", new { fileName = fileName, categoryid = categoryid, subcategoryid = subcategoryid });
        }

        //get
        public FileResult getDocumentFile(string fileName, string categoryid, string subcategoryid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string path = Server.MapPath("~/App_Data/CompanyPolicies/");
            if (fileName.Length == 0 || categoryid.Length == 0)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            path = Server.MapPath("~/App_Data/CompanyPolicies/" + categoryid + "/" + subcategoryid + "/");
            return File(path + fileName, mc.getMimeType(fileName));
        }

        #endregion

        #region upload
        //get
        public ActionResult UploadFile(int catgid=0, int subcatgid=0, string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (mc.getPermission(Entry.Upload_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewObject(catgid);
            ViewBag.Message = msg;
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase[] docfiles, FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (mc.getPermission(Entry.UploadEmployeeDocument, permissionType.Add) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            int catgid = 0;
            int subcatgid = 0;
            string filedesc = "";
            if (form["ddlCategory"].ToString().Length > 0)
            {
                catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            }
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            if (form["filedesc"].ToString().Length > 0)
            {
                filedesc = form["filedesc"].ToString();
            }
            string dirpath = Server.MapPath("~/App_Data/CompanyPolicies/" + catgid + "/" + subcatgid + "/");
            System.IO.DirectoryInfo empdirinfo = new System.IO.DirectoryInfo(dirpath);
            if (!empdirinfo.Exists)
            {
                empdirinfo.Create();
            }
            string path = "";
            int cntr = 0;
            bool flg = true;
            try
            {
                foreach (HttpPostedFileBase file in docfiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(file.FileName));
                        //to database
                        DocumentFileMdl modelObject = new DocumentFileMdl();
                        modelObject.DocumentTypeId = 44;
                        modelObject.CategoryId = catgid;
                        modelObject.SubCategoryId = subcatgid;
                        modelObject.ItemId = 0;
                        modelObject.DocFileName = file.FileName;
                        modelObject.FileDesc = filedesc;
                        modelObject.ModifyUser = 0;
                        modelObject.CCode = 0;//note
                        bllObject.insertObject(modelObject);
                        if (bllObject.Result == true)
                        {
                            file.SaveAs(path);
                            cntr += 1;
                        }
                        else
                        {
                            flg = false;
                        }
                        //
                    }
                }
                ViewBag.Message = cntr.ToString() + " File(s) uploaded.";
                if (cntr == 0)
                {
                    ViewBag.Message = "No file selected to upload!";
                }
                if (flg == false)
                {
                    ViewBag.Message = bllObject.Message;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
            }
            return RedirectToAction("UploadFile", new { catgid = catgid, msg = ViewBag.Message });
        }
        #endregion

        //get
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Upload_Company_Document, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            string fileName = "";
            string categoryid = "";
            string subcategoryid = "";
            DocumentFileMdl modelObject = new DocumentFileMdl();
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                fileName = modelObject.DocFileName;
                categoryid = modelObject.CategoryId.ToString();
                subcategoryid = modelObject.SubCategoryId.ToString();
            }
            //
            bllObject.deleteObject(id);
            if (bllObject.Result == true)
            {
                string path = Server.MapPath("~/App_Data/CompanyPolicies/" + categoryid + "/" + subcategoryid+"/");
                if (System.IO.File.Exists(path + fileName))
                {
                    System.IO.File.Delete(path + fileName);
                }
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
