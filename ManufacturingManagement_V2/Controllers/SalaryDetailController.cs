using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class SalaryDetailController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private SalaryDetailBLL bllObject = new SalaryDetailBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", int newempid = 0, string empname = "")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.NewEmpId = newempid;
            ViewBag.EmpName = empname;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", int newempid = 0, string empname = "")
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(dtfrom, dtto, newempid, empname);
            List<SalaryDetailMdl> modelObject = new List<SalaryDetailMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, newempid);
            if (objCookie.getLoginType() == 0)//admin
            {
                ViewBag.IsAdmin = "1";
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            int newempid = Convert.ToInt32(form["hfNewEmpId"].ToString());
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, newempid = newempid, empname = empname });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, bool addnew = false)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            SalaryDetailMdl modelObject = new SalaryDetailMdl();
            modelObject.IncDate = DateTime.Now;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            //note to add as new record
            if (addnew)
            {
                modelObject.RecId = 0;
                ViewData["AddEdit"] = "Save";
            }
            //
            return View(modelObject);
        }

        public ActionResult AddNewDetail(int newempid = 0)
        {
            if (mc.getPermission(Entry.EmployeeMaster, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            int recid = bllObject.getRecentSalaryDetailRecId(newempid);
            return RedirectToAction("CreateUpdate", new {id=recid, addnew =true });
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(SalaryDetailMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.SalaryDetailUpdation, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertSalaryDetail(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.SalaryDetailUpdation, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateSalaryDetail(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.SalaryDetailUpdation, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
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
