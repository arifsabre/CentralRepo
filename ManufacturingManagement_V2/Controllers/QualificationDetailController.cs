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
    public class QualificationDetailController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private QualDetailBLL bllObject = new QualDetailBLL();
        private QualificationBLL qualBLL = new QualificationBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject()
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.QualificationList = new SelectList(qualBLL.getObjectList(), "qualid", "qualification");
        }

        // GET: /
        public ActionResult Index(int newempid = 0, string empname = "")
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<QualDetailMdl> modelObject = new List<QualDetailMdl> { };
            modelObject = bllObject.getObjectList(newempid);
            ViewBag.NewEmpId = newempid;
            ViewBag.EmpName = empname;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int newempid = Convert.ToInt32(form["hfNewEmpId"].ToString());
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("Index", new { newempid = newempid, empname = empname });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string empid = "", int newempid = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            QualDetailMdl modelObject = new QualDetailMdl();
            modelObject.EmpId = empid;
            modelObject.NewEmpId = newempid;
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
        public ActionResult CreateUpdate(QualDetailMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.EmployeeMaster, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
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
                    if (modelObject.RecId == 0)//add mode
                    {
                        return RedirectToAction("CreateUpdate", new { empid = modelObject.EmpId, newempid = modelObject.NewEmpId });
                    }
                    else//edit mode
                    {
                        return RedirectToAction("Index", new { empid = modelObject.EmpId, newempid = modelObject.NewEmpId });
                    }
                }
                else
                {
                    ViewBag.Message = bllObject.Message;
                    return View();
                }
            }
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            QualDetailMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Delete) == false)
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