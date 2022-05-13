using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class WorkListController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private WorkListBLL bllObject = new WorkListBLL();
        private CompanyBLL compBLL = new CompanyBLL();
        private GovtDepartmentBLL govtDepBLL = new GovtDepartmentBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int compcode = 0, string status = "p", string dtfrom = "", string dtto = "", int depid = 0, int topt = 0)
        {
            ViewBag.TaskOptList = new SelectList(bllObject.getComplianceGroupList(), "GroupId", "GroupName", topt);
            //
            List<CompanyMdl> cmdl = compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId()));
            CompanyMdl objmdl = new CompanyMdl();
            objmdl.CompCode = 0;
            objmdl.CmpName = "NA";
            cmdl.Add(objmdl);
            ViewBag.CompanyList = new SelectList(cmdl, "compcode", "cmpname", compcode);
            //
            List<GovtDepartmentMdl> govtmdl = govtDepBLL.getObjectList();
            GovtDepartmentMdl gvtmdl = new GovtDepartmentMdl();
            gvtmdl.DepId = 0;
            gvtmdl.DepName = "NA";
            govtmdl.Add(gvtmdl);
            ViewBag.DepartmentList = new SelectList(govtmdl, "depid", "depname", depid);
            //
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.status = status;
        }

        // GET:
        public ActionResult Index(int compcode = 0, string status = "p", string dtfrom = "", string dtto = "", int depid=0, int topt = 0)
        {
            List<EntryGroupMdl> compGroup = bllObject.getComplianceGroupList();
            if (compGroup.Count == 0)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            DateTime dt = new DateTime(2018, 10, 1);
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(dt); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddYears(1)); };
            if (topt == 0)
            {
                topt = compGroup[0].GroupId;
            }
            setViewObject(compcode,status,dtfrom,dtto,depid,topt);
            List<WorkListMdl> modelObject = new List<WorkListMdl> { };
            bool comp = false;
            if (status == "c") { comp = true; };
            modelObject = bllObject.getObjectList(comp,depid,dtfrom,dtto,topt,compcode);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int compcode = 0;
            if (form["ddlCompany"].ToString().Length > 0)
            {
                compcode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            int depid = 0;
            if (form["ddlDepartment"].ToString().Length > 0)
            {
                depid = Convert.ToInt32(form["ddlDepartment"].ToString());
            }
            int topt = 2015;
            if (form["ddlTOpt"].ToString().Length > 0)
            {
                topt = Convert.ToInt32(form["ddlTOpt"].ToString());
            }
            string status = form["ddlStatus"].ToString();
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { compcode = compcode, status = status, dtfrom=dtfrom,dtto=dtto,depid=depid,topt=topt });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["mnuOpt"] = objCookie.getMenu();
            setViewObject();
            ViewData["AddEdit"] = "Save";

            WorkListMdl modelObject = new WorkListMdl();
            modelObject.RecDT = DateTime.Now;
            modelObject.CompCode = Convert.ToInt32(objCookie.getCompCode());
            modelObject.DepId = 1;
            string pdesc = "P1: " + mc.getStringByDate(DateTime.Now.AddDays(10))+", ";
            pdesc += "P2: " + mc.getStringByDate(DateTime.Now.AddDays(20))+", ";
            pdesc += "P3: " + mc.getStringByDate(DateTime.Now.AddDays(30));
            ViewBag.PDesc = pdesc;
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
        public ActionResult CreateUpdate(WorkListMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {    
                List<EntryGroupMdl> compGroup = bllObject.getComplianceGroupList();
                if (compGroup.Count == 0)//no permission
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (bllObject.isValidToModifyWorkList(modelObject.RecId) == false)
                {
                    return View();
                }
                //edit permission is checked in dbprocedure also
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                if (ViewData["AddEdit"].ToString() == "Update")
                {
                    return RedirectToAction("Index", new { compcode = modelObject.CompCode });
                }
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        // GET: /CreateCompliance
        public ActionResult CreateCompliance(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["mnuOpt"] = objCookie.getMenu();

            WorkListMdl modelObject = new WorkListMdl();
            modelObject.RecDT = DateTime.Now;
            modelObject.CompDT = DateTime.Now;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                modelObject.CompDT = modelObject.RecDT;
            }
            return View(modelObject);
        }

        // POST: /CreateCompliance
        [HttpPost]
        public ActionResult CreateCompliance(WorkListMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["mnuOpt"] = objCookie.getMenu();

            if (ModelState.IsValid)
            {
                List<EntryGroupMdl> compGroup = bllObject.getComplianceGroupList();
                if (compGroup.Count == 0)//no permission
                {
                    ViewBag.Message = "Permission Denied for addition!";
                    return View();
                }
                if (bllObject.isValidToModifyWorkList(modelObject.RecId) == false)
                {
                    ViewBag.Message = "Permission Denied for modification!";
                    return View();
                }
                bllObject.insertCompliance(modelObject);
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index", new { compcode = modelObject.CompCode });
                }
                else
                {
                    ViewBag.Message = bllObject.Message;
                    return View();
                }
            }
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult updateCompleted(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            //permission checked in dbprocedure also
            if (bllObject.isValidToModifyWorkList(recid) == false)
            {
                ViewBag.Message = "Permission Denied for modification!";
                return new JsonResult { Data = new { status = false, message = bllObject.Message } };
            }
            bllObject = new WorkListBLL();
            bllObject.setWorkCompleted(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult updatePending(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            //permission checked in dbprocedure
            //permission checked in dbprocedure also
            if (bllObject.isValidToModifyWorkList(recid) == false)
            {
                ViewBag.Message = "Permission Denied for modification!";
                return new JsonResult { Data = new { status = false, message = bllObject.Message } };
            }
            bllObject = new WorkListBLL();
            bllObject.setWorkPending(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #region upload/download section

        //get
        public ActionResult UploadFile(string recid="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["mnuOpt"] = objCookie.getMenu();

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
        public ActionResult UploadFile(HttpPostedFileBase docfile, string recid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["mnuOpt"] = objCookie.getMenu(); 

            if (recid.Length == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            if (bllObject.isValidToModifyWorkList(Convert.ToInt32(recid)) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            ViewBag.recid = recid;
            Session["updrecid"] = recid;
            string dirpath = Server.MapPath("~/App_Data/ComplianceDocs/");
            string path = "";
            int cntr = 0;
            try
            {
                if (docfile!=null && docfile.ContentLength > 0)
                {
                    path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(recid+".pdf"));
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
                    bllObject.updateWorklistDocument(Convert.ToInt32(recid), true);
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
        public ActionResult ShowDocument(string recid)
        {
            if (recid.Length == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            if (bllObject.isValidToModifyWorkList(Convert.ToInt32(recid)) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            string path = Server.MapPath("~/App_Data/ComplianceDocs/");
            if (System.IO.File.Exists(path + recid + ".pdf") == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>File Not Uploaded!</h1></a>");
            }
            return File(path + recid + ".pdf", mc.getMimeType(recid + ".pdf"));
        }

        #endregion //upload/download section

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (bllObject.isValidToDeleteWorkList(recid)==false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new WorkListBLL();
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
