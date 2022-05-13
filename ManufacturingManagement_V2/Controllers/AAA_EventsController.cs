using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Controllers
{
    public class AAA_EventsController : Controller
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        private readonly EmployeeBLL empBLL = new EmployeeBLL();
        private void SetViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        //
        // GET: /Treeview/
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Biometric_Data) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            SetViewData();
            return View();
        }
        public JsonResult GetEvents()
        {
            using (ErpConnection dc = new ErpConnection())
            {
                var events = dc.AAA_Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        [HttpPost]
        public JsonResult SaveEvent(AAA_Events e)
        {
            var status = false;
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status } };
            }
            
            using (ErpConnection dc = new ErpConnection())
            {
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.AAA_Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.AAA_Events.Add(e);
                }
                dc.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status } };
            }

            using (ErpConnection dc = new ErpConnection())
            {
                var v = dc.AAA_Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.AAA_Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status } };
        }
    }
}
