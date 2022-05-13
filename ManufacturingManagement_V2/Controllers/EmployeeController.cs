using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class EmployeeController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private EmployeeBLL bllObject = new EmployeeBLL();
        private CompanyBLL compBLL = new CompanyBLL();
        private QualificationBLL qualBLL = new QualificationBLL();
        private ExperienceDetailBLL expDetBLL = new ExperienceDetailBLL();
        private QualDetailBLL qualDetBLL = new QualDetailBLL();
        private FamilyDetailBLL familyDetBLL = new FamilyDetailBLL();
        private PFNomineeBLL pfNomineeDetBLL = new PFNomineeBLL();
        private UserBLL bllUser = new UserBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string grade="", int joiningunit=0)
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.TitleList = new SelectList(mc.getTitleList(), "Value", "Text");
            ViewBag.GradeList = new SelectList(bllObject.getEmployeeGradeList(), "ObjectCode", "ObjectName", grade);
            ViewBag.LocationList = new SelectList(bllObject.getLocationList(), "ObjectId", "ObjectName");
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname",joiningunit);
            ViewBag.QualificationList = new SelectList(qualBLL.getObjectList(), "qualid", "qualification");
            ViewBag.EmpCategoryList = new SelectList(mc.getEmpCategoryList(), "Value", "Text");
            ViewBag.DesignationList = new SelectList(mc.getDesignationList(), "Value", "Text");
            ViewBag.GenderList = new SelectList(mc.getGenderList(), "Value", "Text");
            ViewBag.MStatusList = new SelectList(mc.getMaritalStatusList(), "Value", "Text");
            ViewBag.DepartmentList = new SelectList(bllUser.getDepartmentList(), "depcode", "department");
            ViewBag.ServiceTypeList = new SelectList(mc.getServiceTypeList(), "Value", "Text");
            ViewBag.CasteList = new SelectList(bllObject.getCasteList(), "ObjectId", "ObjectName");
            ViewBag.AgencyList = new SelectList(bllObject.getAgencyList(), "ObjectId", "ObjectName");
        }
        
        // GET: /
        public ActionResult Index(string grade = "", int joiningunit = 0, bool incimage = false, bool isactive = true)
        {
            if (mc.getPermission(Entry.Employee_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            setViewObject(grade,joiningunit);
            List<EmployeeMdl> modelObject = new List<EmployeeMdl> { };
            modelObject = bllObject.getObjectList(grade, joiningunit).Where(s => s.IsActive.Equals(isactive)).ToList();
            ViewBag.IncImage = incimage;
            ViewBag.IsActive = isactive;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            bool incimage = false;
            if (form["chkImage"] != null)
            {
                incimage = true;
            }
            bool isactive = false;
            if (form["chkIsActive"] != null)
            {
                isactive = true;
            }
            string grade = form["ddlGrade"].ToString();
            int ju = 0;
            if (form["ddlJoiningUnit"].ToString().Length > 0)
            {
                ju = Convert.ToInt32(form["ddlJoiningUnit"].ToString());
            }
            return RedirectToAction("Index", new { grade = grade, joiningunit = ju, incimage = incimage, isactive = isactive });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            setViewObject();
            EmployeeMdl modelObject = new EmployeeMdl();
            modelObject.EmpId = "New Id";
            modelObject.Title = "Mr.";//note
            modelObject.ContValidUpto = new DateTime(1900, 1, 1);
            if (id != 0)
            {
                modelObject = bllObject.searchEmployeeByNewEmpId(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                modelObject.hdnUpdationLog = modelObject.UpdationLog;
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(EmployeeMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //if (ModelState.IsValid) { }//note
            ViewBag.Message = "Permission Denied!";
            if (modelObject.NewEmpId == 0)//add mode
            {
                if (mc.getPermission(Entry.EmployeeMaster, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
                modelObject.EmpId = "New Id";
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = bllObject.Message;
                return View(@modelObject);
            }
        }

        [HttpPost]
        public JsonResult getNewEmpCode(int compcode, string grade)
        {
            int x = bllObject.getEmpLastNo(grade,compcode);
            return new JsonResult { Data = new { empid = x } };
        }

        #region upload/download section

        //get
        public ActionResult UploadFile()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.newempid = "";
            ViewBag.empname = "";
            ViewBag.Message = "";
            if (Session["updempid"] != null)
            {
                ViewBag.newempid = Session["updempid"].ToString();
                Session.Remove("updempid");
            }
            if (Session["updempname"] != null)
            {
                ViewBag.empname = Session["updempname"].ToString();
                Session.Remove("updempname");
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
        public ActionResult UploadFile(HttpPostedFileBase[] docfiles, string newempid,string empname)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (mc.getPermission(Entry.UploadEmployeeDocument, permissionType.Add) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            if (newempid.Length == 0)
            {
                ViewBag.Message = "Employee not selected!";
                return View();
            }
            ViewBag.newempid = newempid;
            ViewBag.empname = empname;
            Session["updempid"] = newempid;
            Session["updempname"] = empname;
            string empdirpath = Server.MapPath("~/App_Data/EmpDocs/" + newempid + "/");
            System.IO.DirectoryInfo empdirinfo = new System.IO.DirectoryInfo(empdirpath);
            if (!empdirinfo.Exists)
            {
                empdirinfo.Create();
            }
            string path = "";
            int cntr = 0;
            try
            {
                foreach (HttpPostedFileBase file in docfiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        path = System.IO.Path.Combine(empdirpath, System.IO.Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        cntr += 1;
                    }
                }
                ViewBag.Message = cntr.ToString() + " File(s) uploaded.";
                if (cntr == 0)
                {
                    ViewBag.Message = "No file selected to upload!";
                }
                if (cntr > 0)//note
                {
                    bllObject.updateEmployeeDocument(Convert.ToInt32(newempid),true);
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
        public ActionResult Downloads(string newempid = "", string empname = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.newempid = newempid;
            ViewBag.empname = empname;
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/EmpDocs/" + newempid + "/"));
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

        // POST: /Downloads
        [HttpPost]
        public ActionResult FilterDownload(FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            string newempid = form["hfNewEmpId"].ToString();
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("Downloads", new { newempid = newempid, empname = empname });
        }

        //get
        public FileResult Download(string fileName, string newempid)
        {
            string path = Server.MapPath("~/App_Data/EmpDocs/");
            if (mc.getPermission(Entry.DownloadEmployeeDocument, permissionType.Add) == false || newempid.Length == 0)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            path = Server.MapPath("~/App_Data/EmpDocs/" + newempid + "/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }
        
        //get
        public FileResult ShowDocument(string fileName, string newempid)
        {
            string path = Server.MapPath("~/App_Data/EmpDocs/");
            if (mc.getPermission(Entry.DownloadEmployeeDocument, permissionType.Add) == false || newempid.Length == 0)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            path = Server.MapPath("~/App_Data/EmpDocs/" + newempid + "/");
            return File(path + fileName, mc.getMimeType(fileName));
        }

        //get
        public FileResult ShowEmpImage(string newempid)
        {
            string fileName = "";
            string path = Server.MapPath("~/App_Data/EmpDocs/");
            if (mc.getPermission(Entry.DownloadEmployeeDocument, permissionType.Add) == false || newempid.Length == 0)
            {
                fileName = "Permission.png";
                return File(path + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            EmployeeMdl empmdl = new EmployeeMdl();
            empmdl = bllObject.searchEmployeeByNewEmpId(Convert.ToInt32(newempid));
            fileName = empmdl.EmpId + ".jpg";
            path = Server.MapPath("~/App_Data/EmpDocs/" + newempid + "/");
            return File(path + fileName, mc.getMimeType(fileName));
        }

        #endregion //upload/download section

        #region directory

        [HttpGet]
        public ActionResult EmployeeDirectory(string grade = "", string empname = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (grade.Length == 0) { grade = "s"; };
            ViewBag.grade = grade;
            ViewBag.empname = empname;
            List<EmployeeMdl> modelObject = new List<EmployeeMdl> { };
            modelObject = bllObject.getDirectoryList(grade).Where(s => s.EmpName.ToLower().Contains(empname.ToLower())).ToList();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexDirectory(FormCollection form)
        {
            string grade = form["ddlGrade"].ToString();
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("EmployeeDirectory", new { grade = grade, empname = empname });
        }

        // GET: /UpdateDirectory
        public ActionResult UpdateDirectory(string msg = "")
        {
            if (mc.getPermission(Entry.DirectoryUpdation, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.Message = msg;
            return View();
        }

        // POST: /UpdateDirectory
        [HttpPost]
        public ActionResult UpdateDirectory(EmployeeMdl empMdl)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.DirectoryUpdation, permissionType.Edit) == false)
            {
                return View(empMdl);
            }
            bllObject.updateDirectory(empMdl);
            return RedirectToAction("UpdateDirectory", new { msg = bllObject.Message });
        }

        [AcceptVerbs(HttpVerbs.Post)]//with select event of autocomplete
        public JsonResult getDirectoryInfo(int newempid)
        {
            EmployeeBLL objbll = new EmployeeBLL();
            EmployeeMdl objmdl = new EmployeeMdl();
            objmdl = objbll.getDirectoryInfo(newempid);
            return new JsonResult { Data = new { phoneextno = objmdl.PhoneExtNo, contactno=objmdl.ContactNo } };
        }

        #endregion

        #region update sms status

        public ActionResult IndexUpdateSMS()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ModelState.Clear();
            EmployeeBLL objbll = new EmployeeBLL();
            EmployeeMdl model = new EmployeeMdl();
            model.GetempList = objbll.Get_UpdateList();
            return View(model);
        }
        
        public JsonResult EditStatusSMSRecord(int? id)
        {
            EmployeeBLL objbll = new EmployeeBLL();
            var hl = objbll.Get_UpdateList().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Update_SMSStatus(EmployeeMdl objModel)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bllObject = new EmployeeBLL();
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return View(objModel);
            }
            EmployeeBLL objbll = new EmployeeBLL();
            try
            {
               objbll.updateStatusSMS(objModel);
                if (objbll.Result == true)
                {
                    ViewBag.Message = "SMS Status  Updated Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }
                return RedirectToAction("IndexUpdateSMS");
            }
            catch
            {
                throw;
            }
        }

        #endregion

        [HttpPost]
        public ActionResult SearchEmployee(FormCollection form)
        {
            string newempid = form["hfNewEmpId"].ToString();
            if (newempid == "0")
            {
                string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
                baseurl += "Employee/Details";
                return Content("<a href='" + baseurl + "'><h1>Invalid ItemCode entered!</h1></a>");
            }
            return RedirectToAction("Details", new { id = newempid });
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Employee_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            EmployeeMdl modelObject = bllObject.searchEmployeeByNewEmpId(id);
            modelObject.QualDetail = qualDetBLL.getObjectList(id);
            modelObject.ExpDetail = expDetBLL.getObjectList(id);
            modelObject.FamilyDetail = familyDetBLL.getObjectList(id);
            modelObject.PFNomineeDetail = pfNomineeDetBLL.getObjectList(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            ViewBag.ToEdit = "0";
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == true)
            {
                ViewBag.ToEdit = "1";
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject = new EmployeeBLL();
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
