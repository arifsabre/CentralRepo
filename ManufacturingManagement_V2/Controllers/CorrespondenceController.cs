using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class CorrespondenceController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private CompanyBLL compBLL = new CompanyBLL();
        private UserBLL bllUser = new UserBLL();
        private MasterDocumentNameBLL mdocBLL = new MasterDocumentNameBLL();
        private CorrespondenceBLL bllObject = new CorrespondenceBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int ccode = 0, string finyr = "0", string depcode = "0", bool filterbydt = true, string dtfrom = "", string dtto = "", string searchfield = "0", string searchtext = "", string series = "d", int documentid=0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", ccode);
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear", finyr);
            ViewBag.DepartmentList = new SelectList(bllUser.getDepartmentList(), "depcode", "department", depcode);
            ViewBag.MDocList = new SelectList(mdocBLL.getObjectList(ccode), "documentid", "documentname", documentid);
            ViewBag.SearchFieldList = new SelectList(mc.getSearchFieldList(), "Value", "Text", searchfield);
            ViewBag.LetterSeriesList = new SelectList(mc.getLetterSeriesList(), "Value", "Text", series);
            ViewBag.Series = series;
            ViewBag.FilterByDT = filterbydt;
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.SearchFor = searchfield;
            ViewBag.SearchText = searchtext;
        }

        // GET: /
        public ActionResult Index(int ccode = 0, string finyr = "0", string depcode = "0", bool filterbydt = true, string dtfrom = "", string dtto = "", string searchfield = "0", string searchtext = "", string series = "0", int documentid=0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note: open to all
            //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            //
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(ccode, finyr, depcode, filterbydt, dtfrom, dtto, searchfield, searchtext, series, documentid);
            List<CorrespondenceMdl> modelObject = new List<CorrespondenceMdl> { };
            modelObject = bllObject.getObjectList(ccode, finyr, filterbydt, mc.getDateByString(dtfrom), mc.getDateByString(dtto), series, depcode, documentid, searchfield, searchtext);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string finyr = form["ddlFinYear"].ToString();
            string depcode = form["ddlDepartment"].ToString();
            string series = form["ddlSeries"].ToString();
            string searchfield = form["ddlSearchField"].ToString();
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string searchtext = form["txtSearchText"].ToString();
            //bool incdraft = (form["IncDraft"] ?? "").Equals("true", StringComparison.CurrentCultureIgnoreCase);
            bool filterbydt = false;
            if (form["chkFilterByDT"] != null)
            {
                filterbydt = true;
            }
            int ccode = 0;
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            int mdocid = 0;
            if (form["ddlMDocList"].ToString().Length > 0)
            {
                mdocid = Convert.ToInt32(form["ddlMDocList"].ToString());
            }
            return RedirectToAction("Index", new { ccode = ccode, finyr = finyr, depcode = depcode, filterbydt = filterbydt, dtfrom = dtfrom, dtto = dtto, searchfield = searchfield, searchtext = searchtext, series = series, documentid = mdocid });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //note: open to all
            //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            //
            setViewData();
            setViewObject(Convert.ToInt32(objCookie.getCompCode()),objCookie.getFinYear(),"ofc",true);
            CorrespondenceMdl modelObject = new CorrespondenceMdl();
            modelObject.LetterDT = DateTime.Now;
            modelObject.LetterNo = "New";
            modelObject.Series = "x";
            modelObject.FinYear = objCookie.getFinYear();
            ViewBag.Message = msg;
            if (id != 0)
            {
                if (bllObject.isValidToModifyLetter(id) == false)
                {
                    return RedirectToAction("Index");
                }
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
        public ActionResult CreateUpdate(CorrespondenceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject(modelObject.CompCode,modelObject.FinYear,modelObject.DepCode,true);
            //if (ModelState.IsValid) { }
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add
            {
                //note: open to all
                //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Add) == false)
                //{
                //    return View();
                //}
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                //note: open to all but ownself only
                //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Edit) == false)
                //{
                //    return View();
                //}
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("CreateUpdate", new { id = modelObject.RecId, msg = bllObject.Message });
            }
            modelObject.LetterNo = bllObject.getLetterNoById(modelObject.RecId);
            //not required modelObject.DocumentURL = "<a target='_new' href=" + modelObject.DocumentLink + ">Go_To_Document</a>'";
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult convertDraftCorrespondenceToMain(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (bllObject.isValidToModifyLetter(recid) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new CorrespondenceBLL();
            bllObject.ConvertDraftCorrespondenceToMain(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, recid = recid } };
        }

        [HttpPost]
        public ActionResult goToDocument(string url)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            return RedirectToAction(url);
        }

        [HttpPost]
        public JsonResult deleteCorrespondence(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (bllObject.isValidToModifyLetter(recid) == false)//chk in bll also
            {
                ViewBag.Message = "Permission Denied!";
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new CorrespondenceBLL();
            bllObject.deleteCorrespondence(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, recid = recid } };
        }

        #region upload/download attachments

        //get
        public ActionResult UploadFile(int id = 0, string letterno="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.recid = id;
            ViewBag.letterno = letterno;
            ViewBag.Message = "";
            if (Session["recid"] != null)
            {
                ViewBag.recid = Session["recid"].ToString();
                Session.Remove("recid");
            }
            if (Session["letterno"] != null)
            {
                ViewBag.letterno = Session["letterno"].ToString();
                Session.Remove("letterno");
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
        public ActionResult UploadFile(HttpPostedFileBase[] docfiles, int recid=0, string letterno="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (recid == 0)
            {
                ViewBag.Message = "Letter number not selected!";
                return View();
            }
            if (bllObject.isValidToModifyLetter(recid) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            //note: open to all
            //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Add) == false)
            //{
            //    ViewBag.Message = "Permission Denied!";
            //    return View();
            //}
            //
            ViewBag.recid = recid;
            Session["recid"] = recid;
            Session["letterno"] = letterno;
            //Session["updempname"] = empname;
            string empdirpath = Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/");
            System.IO.DirectoryInfo empdirinfo = new System.IO.DirectoryInfo(empdirpath);
            if (!empdirinfo.Exists)
            {
                empdirinfo.Create();
            }
            string path = "";
            int cntr = 0;
            string filemsg = "";
            try
            {
                foreach (HttpPostedFileBase file in docfiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        path = System.IO.Path.Combine(empdirpath, System.IO.Path.GetFileName(file.FileName));
                        if (System.IO.File.Exists(path))
                        {
                            if (bllObject.getCorrespondenceSeries(recid).ToLower() != "d")//not draft
                            {
                                if (objCookie.getLoginType() != 0)//not admin
                                {
                                    ViewBag.Message = "File already exists! Replacement Denied! File Name: " + file.FileName;
                                    return View();
                                }
                            }
                        }
                        file.SaveAs(path);
                        cntr += 1;
                        filemsg += file.FileName + ", ";
                    }
                }
                string fmsg = cntr.ToString() + " File(s) uploaded- " + filemsg;
                ViewBag.Message = fmsg.Substring(0,fmsg.Length-2);
                if (cntr == 0)
                {
                    ViewBag.Message = "No file selected to upload!";
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
        public ActionResult Downloads(string recid = "", string letterno = "", string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.recid = recid;
            ViewBag.letterno = letterno;
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/"));
            List<string> items = new List<string>();
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }
            }
            ViewBag.Message = msg;
            return View(items);
        }

        // POST: /Downloads
        [HttpPost]
        public ActionResult FilterDownload(FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string recid = form["hfRecId"].ToString();
            string letterno = form["txtLetterNo"].ToString();
            return RedirectToAction("Downloads", new { recid = recid, letterno = letterno });
        }

        //get
        public FileResult Download(string fileName, string recid)
        {
            string path = Server.MapPath("~/App_Data/CorrspLetters/");
            int rid = 0;
            if (recid.Length > 0) { rid = Convert.ToInt32(recid); };
            if (bllObject.isValidToModifyLetter(rid) == false)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            //note: open to all
            //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Edit) == false || recid.Length == 0)
            //{
            //    fileName = "Permission.png";
            //    return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            //}
            path = Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        //get
        public FileResult ShowDocument(string fileName, string recid)
        {
            string path = Server.MapPath("~/App_Data/CorrspLetters/");
            int rid = 0;
            if (recid.Length > 0) { rid = Convert.ToInt32(recid); };
            if (bllObject.isValidToModifyLetter(rid) == false)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            //note: open to all
            //if (mc.getPermission(Entry.Correspondence_Entry, permissionType.Edit) == false || recid.Length == 0)
            //{
            //    fileName = "Permission.png";
            //    return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            //}
            path = Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/");
            return File(path + fileName, mc.getMimeType(fileName));
        }

        public ActionResult RemoveDocument(string fileName, string recid, string letterno)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            string path = Server.MapPath("~/App_Data/CorrspLetters/");
            int rid = 0;
            if (recid.Length > 0) { rid = Convert.ToInt32(recid); };
            if (bllObject.getCorrespondenceSeries(rid).ToLower() != "d")//not draft
            {
                if (objCookie.getLoginType() != 0)//not admin
                {
                    fileName = "Permission.png";
                    return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
                }
            }
            if (bllObject.isValidToModifyLetter(rid) == false)
            {
                return RedirectToAction("Downloads", new { recid = recid, letterno = letterno, msg = "Permission denied to remove attachment!" });
            }
            path = Server.MapPath("~/App_Data/CorrspLetters/" + recid + "/");
            if (System.IO.File.Exists(path + fileName))
            {
                System.IO.File.Delete(path + fileName);
            }
            return RedirectToAction("Downloads", new { recid = recid, letterno = letterno });
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
