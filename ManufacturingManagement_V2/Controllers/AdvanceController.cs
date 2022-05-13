using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AdvanceController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AdvanceBLL bllObject = new AdvanceBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom="",string dtto="",string empid="", string empname="")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.EmpId = empid;
            ViewBag.EmpName = empname;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", string empid = "", string empname = "")
        {
            if (mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Employee_Advance_Report) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom,dtto,empid, empname);
            List<AdvanceMdl> modelObject = new List<AdvanceMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, empid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string empid = form["hfEmpId"].ToString();
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, empid = empid, empname = empname });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.AdvanceEntry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.AdvanceEntry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            AdvanceMdl modelObject = new AdvanceMdl();
            modelObject.AdvDate = DateTime.Now;
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
        public ActionResult CreateUpdate(AdvanceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (modelObject.Remarks == null) { modelObject.Remarks = ""; };
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.AdvanceEntry, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.AdvanceEntry, permissionType.Edit) == false)
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
                    return View();
                }
            }
            return View(modelObject);
        }

        [AcceptVerbs(HttpVerbs.Post)]//with select event of autocomplete
        public JsonResult getAdvanceDeductionInfo(int newempid)
        {
            AdvanceBLL objbll = new AdvanceBLL();
            return new JsonResult { Data = new { advanceinfo = objbll.getAdvanceDeductionInfo(newempid) } };
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AdvanceMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.AdvanceEntry, permissionType.Delete) == false)
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

        //-------------------------------------------

        [HttpPost]
        public PartialViewResult QuailDialogPartial(int rno)
        {
            AdvanceMdl advmdl=new AdvanceMdl();
            advmdl.RecId = rno;
            ViewBag.QualPList = new SelectList(mc.getQuailPriorityList(), "Value", "Text");
            return PartialView("QuailDialogPartial",advmdl);
        }

        [HttpPost]
        public ActionResult FilterIndexQUAIL(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string empid = form["hfEmpId"].ToString();
            string empname = form["txtEmpName"].ToString();
            return RedirectToAction("TestQUAIL", new { dtfrom = dtfrom, dtto = dtto, empid = empid, empname = empname });
        }

        //get
        public ActionResult TestQUAIL(string dtfrom = "", string dtto = "", string empid = "", string empname = "")
        {
            if (mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Employee_Advance_Report) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            setViewObject(dtfrom, dtto, empid, empname);
            List<AdvanceMdl> modelObject = new List<AdvanceMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, empid);
            return View(modelObject.ToList());
        }

        //get
        public ActionResult TestQUAIL_NEW(string dtfrom = "", string dtto = "", string empid = "", string empname = "")
        {
            if (mc.getPermission(Entry.Employee_Advance_Report, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Employee_Advance_Report) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            setViewObject(dtfrom, dtto, empid, empname);
            List<AdvanceMdl> modelObject = new List<AdvanceMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto, empid);
            return View(modelObject.ToList());
        }

        //-------------------------------------------

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
