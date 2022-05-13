using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;


namespace ManufacturingManagement_V2.Controllers
{
    public class B_Live_DataController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        BioLive bll = new BioLive();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = dsbDept.HR.ToString();
        }
      
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            B_INTEGRATIONMDI model = new B_INTEGRATIONMDI();
            model.Item_List = bll.employees();
        
            return View(model);
        }
        public ActionResult GetAllNetworkStatusLog()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            B_INTEGRATIONMDI model = new B_INTEGRATIONMDI();
            model.Item_List = bll.GetAllNetworkStatus();
            return View(model);
        }
        public ActionResult DisplayGrid()
        {
            B_INTEGRATIONMDI model = new B_INTEGRATIONMDI();
            model.Item_List = bll.GetAllNetworkStatus();
            //Response.AddHeader("Refresh", "5");
            return View(model);
        }
        public ActionResult PieChart()
        {
            ErpConnection entities = new ErpConnection();
            return Json(entities.AAA_NetworkStatusLog.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}
