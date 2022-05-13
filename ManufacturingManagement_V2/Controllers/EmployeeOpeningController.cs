using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class EmployeeOpeningController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private EmployeeOpeningBLL bllObject = new EmployeeOpeningBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", int newempid = 0, string empname = "", string vtype = "op")
        {
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.VTypeList = new SelectList(mc.getEmployeeOpeningVTypes(), "Value", "Text",vtype);
            ViewBag.NewEmpId = newempid;
            ViewBag.EmpName = empname;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", int newempid = 0, string empname = "")
        {
            if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            setViewObject(dtfrom, dtto, newempid, empname);
            List<EmployeeOpeningMdl> modelObject = new List<EmployeeOpeningMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, newempid);
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
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            setViewObject();
            EmployeeOpeningMdl modelObject = new EmployeeOpeningMdl();
            modelObject.VDate = DateTime.Now;
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
        public ActionResult CreateUpdate(EmployeeOpeningMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                ViewBag.Message = bllObject.Message;
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            EmployeeOpeningMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.EmployeeOpeningEntry, permissionType.Delete) == false)
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
