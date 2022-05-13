using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class DocumentFileController : Controller
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

        private void setViewObject(int categoryid = 0, int subcategoryid=0, int ccode = 0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname", ccode);
            List<DocumentCategoryMdl> dcmdl = bllObject.getDocumentCategoryData(33);
            DocumentCategoryMdl objmdl = new DocumentCategoryMdl();

            List<DocSubCategoryMdl> dsubmdl = docSubCatgBLL.getObjectList();
            DocSubCategoryMdl objdsubmdl = new DocSubCategoryMdl();

            objmdl.CategoryId = 0;
            objmdl.CategoryName = "All Documents";
            dcmdl.Add(objmdl);

            objdsubmdl.SubCategoryId = 0;
            objdsubmdl.SubCategoryName = "All";
            dsubmdl.Add(objdsubmdl);

            ViewBag.CatgeoryList = new SelectList(bllObject.getDocumentCategoryData(33), "categoryid", "categoryname", categoryid);
            ViewBag.SubCatgeoryList = new SelectList(docSubCatgBLL.getObjectList(), "subcategoryid", "subcategoryname", subcategoryid);
            
            var cname = dcmdl.Find(c => c.CategoryId == categoryid);
            var subcname = dsubmdl.Find(c => c.SubCategoryId == subcategoryid);

            ViewBag.CName = cname.CategoryName;
            ViewBag.SubCName = subcname.SubCategoryName;
        }

        [HttpPost]
        public JsonResult getISODocsLinkDetail(int ccode)
        {
            List<DocumentCategoryMdl> dcmdl = bllObject.getDocumentCategoryData(33);
            string result = "<table>";
            result += "<tr>";
            result += "<td style='width:2px;'><b>1.</b></td>";
            result += "<td><a style='color:black;' href='../DocumentFile/IndexView?categoryid=0&ccode=" + ccode.ToString() + "'>All Documents</a></td>";
            result += "</tr>";
            for (int i = 0; i < dcmdl.Count; i++)
            {
                result += "<tr>";
                result += "<td style='width:2px;'><b>" + (i + 2) + ".</b></td>";
                result += "<td><a style='color:black;' href='../DocumentFile/IndexView?categoryid=" + dcmdl[i].CategoryId.ToString() + "&ccode=" + ccode.ToString() + "'>" + dcmdl[i].CategoryName + "</a></td>";
                result += "</tr>";
            }
            result += "</table>";
            return new JsonResult { Data = new { result = result } };
        }

        public ActionResult Index(int categoryid = 0, int subcategoryid=0, int ccode = 0)
        {
            if (mc.getPermission(Entry.Upload_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (ccode == 0) { ccode = Convert.ToInt32(objCookie.getCompCode()); };
            setViewObject(categoryid, subcategoryid, ccode);
            List<DocumentFileMdl> modelObject = new List<DocumentFileMdl> { };
            modelObject = bllObject.getObjectList(33, categoryid, subcategoryid, ccode);
            return View(modelObject.ToList());
        }

        public ActionResult IndexView(int categoryid = 0, int subcategoryid = 0, int ccode = 0)
        {
            if (mc.getPermission(Entry.Download_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (ccode == 0) { ccode = Convert.ToInt32(objCookie.getCompCode()); };
            setViewObject(categoryid, subcategoryid, ccode);
            List<DocumentFileMdl> modelObject = new List<DocumentFileMdl> { };
            modelObject = bllObject.getObjectList(33, categoryid, subcategoryid, ccode);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int catgid = 0;
            int subcatgid = 0;
            int ccode = 0;
            if (form["ddlCategory"].ToString().Length > 0)
            {
                catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            }
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            return RedirectToAction("Index", new { categoryid = catgid, subcategoryid = subcatgid, ccode = ccode });
        }

        [HttpPost]
        public ActionResult FilterIndexView(FormCollection form)
        {
            int catgid = 0;
            int subcatgid = 0;
            int ccode = 0;
            if (form["ddlCategory"].ToString().Length > 0)
            {
                catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            }
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            return RedirectToAction("IndexView", new { categoryid = catgid, subcategoryid = subcatgid, ccode = ccode });
        }

        #region view document

        //get
        public ActionResult ShowDocument(string fileName, string categoryid, string subcategoryid, string ccode)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getDocumentPermissionToView("CompanyDocs\\",0,0);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getDocumentPermissionToDownload("CompanyDocs\\", 0, 0);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "DocumentFile/getDocumentFile";
                string reportpms = "fileName=" + fileName + "";
                reportpms += "&categoryid=" + categoryid + "";
                reportpms += "&subcategoryid=" + subcategoryid + "";
                reportpms += "&ccode=" + ccode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDocumentFile", new { fileName = fileName, categoryid = categoryid, subcategoryid = subcategoryid, ccode = ccode });
        }

        //get
        public FileResult getDocumentFile(string fileName, string categoryid, string subcategoryid, string ccode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            string path = Server.MapPath("~/App_Data/CompanyDocs/"+ccode+"/");
            if (fileName.Length == 0 || categoryid.Length == 0)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            path = Server.MapPath("~/App_Data/CompanyDocs/" + ccode+"/" + categoryid + "/" + subcategoryid + "/");
            return File(path + fileName, mc.getMimeType(fileName));
        }

        #endregion

        #region upload
        //get
        public ActionResult UploadFile(int ccode=0, int catgid=0, int subcatgid=0, string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (mc.getPermission(Entry.Upload_Company_Document, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewObject(catgid, subcatgid, ccode);
            ViewBag.Message = msg;
            ViewBag.ccode = ccode;
            ViewBag.catgid = catgid;
            ViewBag.subcatgid = subcatgid;
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
            int ccode = 0;
            string filedesc = "";
            if (form["ddlCategory"].ToString().Length > 0)
            {
                catgid = Convert.ToInt32(form["ddlCategory"].ToString());
            }
            if (form["ddlSubCategory"].ToString().Length > 0)
            {
                subcatgid = Convert.ToInt32(form["ddlSubCategory"].ToString());
            }
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            if (form["filedesc"].ToString().Length > 0)
            {
                filedesc = form["filedesc"].ToString();
            }
            string dirpath = Server.MapPath("~/App_Data/CompanyDocs/" + ccode + "/" + catgid + "/"+ subcatgid + "/");
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
                        modelObject.DocumentTypeId = 33;
                        modelObject.CategoryId = catgid;
                        modelObject.SubCategoryId = subcatgid;
                        modelObject.ItemId = 0;
                        modelObject.DocFileName = file.FileName;
                        modelObject.FileDesc = filedesc;
                        modelObject.ModifyUser = 0;
                        modelObject.CCode = ccode;
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
            return RedirectToAction("UploadFile", new { ccode = ccode, catgid = catgid, msg = ViewBag.Message });
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
            string ccode = "0";
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
                ccode = modelObject.CCode.ToString();
                fileName = modelObject.DocFileName;
                categoryid = modelObject.CategoryId.ToString();
                subcategoryid = modelObject.SubCategoryId.ToString();
            }
            //
            bllObject.deleteObject(id);
            if (bllObject.Result == true)
            {
                string path = Server.MapPath("~/App_Data/CompanyDocs/" + ccode + "/" + categoryid + "/" + subcategoryid + "/");
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
