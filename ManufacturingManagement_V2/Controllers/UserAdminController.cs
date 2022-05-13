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
    public class UserAdminController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UserBLL bllObject = new UserBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int logintype = 5, string department = "", string sorton = "un")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.TitleList = new SelectList(mc.getTitleList(), "Value", "Text");
            ViewBag.LoginTypeList = new SelectList(mc.getLoginTypeList(), "Value", "Text");
            ViewBag.LoginTypeListIndex = new SelectList(mc.getLoginTypeListIndex(), "Value", "Text", logintype);
            ViewBag.DepartmentList = new SelectList(bllObject.getDepartmentList(), "depcode", "department",department);
        }

        public ActionResult Index(int logintype = 5, string department = "", string sorton = "un", bool isactive = true)
        {
            if (mc.getPermission(Entry.User_Admin, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(logintype,department,sorton);
            ViewBag.sorton = sorton;
            ViewBag.lgt = logintype;
            ViewBag.dep = department;
            ViewBag.IsActive = isactive;
            List<UserMdl> modelObject = new List<UserMdl> { };
            modelObject = bllObject.getObjectList();
            if (logintype != 5)
            {
                modelObject = bllObject.getObjectList().Where(x => x.LoginType == logintype).ToList();
            }
            if (department.Length > 0)
            {
                modelObject = bllObject.getObjectList().Where(x => x.Department == department).ToList();
            }
            modelObject = bllObject.getObjectList().Where(x => x.IsActive.Equals(isactive)).ToList();
            if (sorton == "un")
            {
                modelObject = modelObject.OrderBy(x => x.FullName).ToList();
            }
            else if (sorton == "lt")
            {
                modelObject = modelObject.OrderBy(x => x.LoginName).ToList();
            }
            else if (sorton == "dp")
            {
                modelObject = modelObject.OrderBy(x => x.Department).ToList();
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int logintype = -1;
            if (form["ddlLoginType"].ToString().Length > 0)
            {
                logintype = Convert.ToInt32(form["ddlLoginType"].ToString());
            }
            bool isactive = false;
            if (form["chkIsActive"] != null)
            {
                isactive = true;
            }
            string department = form["ddlDepartment"].ToString();
            string sorton = form["ddlSortOn"].ToString();
            return RedirectToAction("Index", new { logintype = logintype, department = department, sorton = sorton, isactive = isactive });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.User_Admin, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            UserMdl modelObject = new UserMdl();
            modelObject.AllCompanies = bllObject.getAllCompanyList();
            modelObject.UserCompanies = bllObject.getUserCompanyList(0);
            modelObject.SelectedCompanies = modelObject.UserCompanies.Select(x => x.CompCode).ToArray();
            modelObject.Title = "Mr.";//note
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
        public ActionResult CreateUpdate(UserMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.UserId == 0)//add mode
                {
                    if (mc.getPermission(Entry.User_Admin, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.User_Admin, permissionType.Edit) == false)
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
            }
            //s-note
            modelObject.AllCompanies = bllObject.getAllCompanyList();
            modelObject.UserCompanies = bllObject.getUserCompanyList(modelObject.UserId);
            modelObject.SelectedCompanies = modelObject.UserCompanies.Select(x => x.CompCode).ToArray();
            //e-note
            return View(modelObject);
        }

        #region update profile
        // GET: /UpdateProfile
        public ActionResult UpdateProfile()
        {
            //allowed only to logged-in user or redirected 
            //from login to update profile
            if (objCookie.checkSessionState() == false && Session["userid"] == null) 
            {
                return RedirectToAction("LoginUser", "Login");
            }
            int luserid = 0;
            if (objCookie.checkSessionState() == true)
            {
                luserid = Convert.ToInt32(objCookie.getUserId());
                setViewData();
            }
            else if (Session["userid"] != null)
            {
                luserid = Convert.ToInt32(Session["userid"].ToString());
                ViewData["UserName"] = Session["svloginuser"].ToString();
                ViewData["CompName"] = "PRAG GROUP";
                ViewData["FinYear"] = DateTime.Now.Year.ToString();
                ViewData["Dept"] = "Home";
                ViewData["mnuOpt"] = "x";
            }
            setViewObject();
            UserMdl modelObject = new UserMdl();
            modelObject.Title = "Mr.";//note
            if (luserid != 0)
            {
                modelObject = bllObject.searchObject(luserid);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            return View(modelObject);
        }

        // POST: /UpdateProfile
        [HttpPost]
        public ActionResult UpdateProfile(UserMdl modelObject)
        {
            //allowed only to logged-in user or redirected 
            //from login to update profile
            if (objCookie.checkSessionState() == false && Session["userid"] == null)
            {
                return RedirectToAction("LoginUser", "Login");
            }
            int luserid = 0;
            if (objCookie.checkSessionState() == true)
            {
                luserid = Convert.ToInt32(objCookie.getUserId());
                setViewData();
            }
            else if (Session["userid"] != null)
            {
                luserid = Convert.ToInt32(Session["userid"].ToString());
                ViewData["UserName"] = Session["svloginuser"].ToString();
                ViewData["CompName"] = "PRAG GROUP";
                ViewData["FinYear"] = DateTime.Now.Year.ToString();
                ViewData["Dept"] = "Home";
                ViewData["mnuOpt"] = "x";
            }
            setViewObject();
            modelObject.UserId = luserid;
            bllObject.updateUserProfile(modelObject);
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                return RedirectToAction("UpdateConfirmation");
            }
            return View(modelObject);
        }

        // GET: /UpdateProfile
        public ActionResult UpdateConfirmation()
        {
            ViewData["UserName"] = Session["svloginuser"].ToString();
            ViewData["CompName"] = "PRAG GROUP";
            ViewData["FinYear"] = DateTime.Now.Year.ToString();
            ViewData["Dept"] = "Home";
            ViewData["mnuOpt"] = "x";
            return View();
        }

        #endregion

        [HttpGet]
        public ActionResult UserDetail()
        {
            if (objCookie.checkSessionState() == false)
            {
                return RedirectToAction("LoginUser", "Login");
            }
            int luserid = 0;
            if (objCookie.checkSessionState() == true)
            {
                luserid = Convert.ToInt32(objCookie.getUserId());
                setViewData();
            }
            setViewData();
            UserMdl uMdl = new UserMdl();
            uMdl = bllObject.searchObject(luserid);
            EmployeeMdl modelObject = new EmployeeMdl();
            if (uMdl.EmpId != 0)
            {
                EmployeeBLL empBll = new EmployeeBLL();
                modelObject = empBll.searchEmployeeByNewEmpId(uMdl.EmpId);
                if (modelObject.NewEmpId > 0)
                {
                    QualDetailBLL qDetBll = new QualDetailBLL();
                    ExperienceDetailBLL expDetBll = new ExperienceDetailBLL();
                    FamilyDetailBLL familyDetBll = new FamilyDetailBLL();
                    PFNomineeBLL pfNomineeDetBll = new PFNomineeBLL();
                    modelObject.QualDetail = qDetBll.getObjectList(uMdl.EmpId);
                    modelObject.ExpDetail = expDetBll.getObjectList(uMdl.EmpId);
                    modelObject.FamilyDetail = familyDetBll.getObjectList(uMdl.EmpId);
                    modelObject.PFNomineeDetail = pfNomineeDetBll.getObjectList(uMdl.EmpId);
                }
            }
            return View(modelObject);
        }

        [HttpGet]
        public ActionResult UserReport()
        {
            if (objCookie.checkSessionState() == false)
            {
                return RedirectToAction("LoginUser", "Login");
            }
            int luserid = 0;
            if (objCookie.checkSessionState() == true)
            {
                luserid = Convert.ToInt32(objCookie.getUserId());
                setViewData();
            }
            setViewData();
            UserMdl uMdl = new UserMdl();
            uMdl = bllObject.searchObject(luserid);
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateFrom = new DateTime(DateTime.Now.Year,1,1);
            rptOption.DateTo = DateTime.Now;
            rptOption.NewEmpId = uMdl.EmpId;
            DataSet ds = new DataSet();
            AttendanceReportBLL attRptBll = new AttendanceReportBLL();
            ds = attRptBll.getRemainingLeavesData(DateTime.Now.Month, DateTime.Now.Year, uMdl.EmpId);
            ViewBag.RemCL = 0;
            ViewBag.RemPL = 0;
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.RemCL = Convert.ToDouble(ds.Tables[0].Rows[0]["RemainingCL"].ToString());
                    ViewBag.RemPL = Convert.ToDouble(ds.Tables[0].Rows[0]["RemainingPL"].ToString());
                }
            }
            return View(rptOption);
        }

        [HttpGet]
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.User_Admin, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            UserMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.User_Admin, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}