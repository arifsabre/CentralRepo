using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    //go to zOthers for previous reference
    public class HomeController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ItemBLL itemBLL = new ItemBLL();
        private AlertsBLL alertsBLL = new AlertsBLL();
        private WorkListBLL workListBLL = new WorkListBLL();
        private UserPermissionBLL bllObject = new UserPermissionBLL();
        private LoginBLL loginBLL = new LoginBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            //note: dept to be get after setting
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Index(string deptt = "Home", string url = "")
        {
            ViewBag.Dept = deptt;
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            loginBLL.changeCoockieForDepartment(deptt);
            setViewData();

            //to set messages, notifications and tasks values and calculation
            //and return it by json call to commonlayout page for display on each link

            //checking department accessibility except home, DocumentControl & maintenance
            if (!(deptt == "Home" || deptt == "DocumentControl" || deptt == "Maintenance"))
            {
                if (objCookie.getDeptMenuList().ToLower().Contains(deptt.ToLower()) == false)
                {
                    return RedirectToAction("Index", new { deptt = "Home" });
                }
            }
            //
            if (deptt == "Marketing")
            {
                return RedirectToAction("TenderAlert", "DisplayDashboard");
            }
            else if (deptt == "DocumentControl")
            {
                return RedirectToAction("DocumentControl_Dashboard", "DisplayDashboard");
            }
            else if (deptt == "Compliance")
            {
                return RedirectToAction("Compliance_Dashboard", "DisplayDashboard");
            }
            else if (deptt == "Quality")
            {
                return RedirectToAction("Quality_Dashboard","DisplayDashboard");
            }
            else if (deptt == "Maintenance")
            {
                if (url.Length > 0)
                {
                    return RedirectToAction(url, "ContractorIndent");
                }
                return RedirectToAction("Index_ComplaintInsert", "ContractorIndent");
            }
            else if (deptt == "ITCELL")
            {
                return RedirectToAction("Index", "IT_Register");
            }
            else if (deptt == "CRM")
            {
                return RedirectToAction("Index", "C_CRM");
            }
            else if (deptt == "Accounts")
            {
                if (objCookie.getDeptMenuList().ToLower().Contains(deptt.ToLower()) == false)
                {
                    return RedirectToAction("Index", new { deptt = "Accounts" });
                }
            }
            AlertsMdl amdl = new AlertsMdl();
            amdl = getAlertSummary();
            return View(amdl);
     
        }

        public ActionResult DisplayErpV1(string url = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["Dept"] = objCookie.getDepartment();//note

            string pms = "uid=" + objCookie.getUserId();
            pms += "&uname=" + objCookie.getUserName();
            pms += "&lgt=" + objCookie.getLoginType();
            pms += "&ccode=" + objCookie.getCompCode();
            pms += "&cmp=" + objCookie.getCmpName();
            pms += "&fy=" + objCookie.getFinYear();
            pms += "&pname=" + url;

            string erpv1url = System.Configuration.ConfigurationManager.AppSettings["erpv1url"].ToString();
            erpv1url += "Dashboard/GetFromErpV2.aspx?" + pms;
            ViewBag.Src = erpv1url += "#toolbar=0&navpanes=0&scrollbar=0";
            return View();
        }

        public ActionResult DisplayErpV1Report(string reporturl = "", string reportpms="", string entryid="", string rptname="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["Dept"] = objCookie.getDepartment();//note

            string pms = "uid=" + objCookie.getUserId();
            pms += "&uname=" + objCookie.getUserName();
            pms += "&lgt=" + objCookie.getLoginType();
            pms += "&ccode=" + objCookie.getCompCode();
            pms += "&cmp=" + objCookie.getCmpName();
            pms += "&fy=" + objCookie.getFinYear();
            //
            pms += "&reporturl=" + reporturl;
            pms += "&reportpms=" + reportpms;//with * separator
            pms += "&entryid=" + entryid;
            pms += "&rptname=" + rptname;
            //
            string erpv1url = System.Configuration.ConfigurationManager.AppSettings["erpv1url"].ToString();
            erpv1url += "Dashboard/DisplayControlledReportForV2.aspx?" + pms;
            ViewBag.Src = erpv1url;//+= "#toolbar=0&navpanes=0&scrollbar=0";
            return View();
        }

        public ActionResult DisplayErpV1File(string url = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["Dept"] = objCookie.getDepartment();//note

            string pms = "uid=" + objCookie.getUserId();
            pms += "&uname=" + objCookie.getUserName();
            pms += "&lgt=" + objCookie.getLoginType();
            pms += "&ccode=" + objCookie.getCompCode();
            pms += "&cmp=" + objCookie.getCmpName();
            pms += "&fy=" + objCookie.getFinYear();
            pms += "&pname=" + url;

            string erpv1url = System.Configuration.ConfigurationManager.AppSettings["erpv1url"].ToString();
            erpv1url += "Dashboard/GetFromErpV2.aspx?" + pms;
            ViewBag.Src = erpv1url;
            return View();
        }

        [HttpPost]
        public JsonResult setMenuOpt(string smenu)
        {
            loginBLL.changeCoockieForSelectedMenu(smenu);
            return new JsonResult { Data = new { status = 1, message = "OK" } };
        }

        [HttpPost]
        public JsonResult isValidToGetDeptMenu(string deptt = "")
        {
            bool status = false;
            if (objCookie.getDeptMenuList().ToLower().Contains(deptt.ToLower()))
            {
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult getDeptMenuList(string id = "")
        {
            string mnulist = objCookie.getDeptMenuList();
            return new JsonResult { Data = new { mnulist = mnulist } };
        }

        public JsonResult GetEvents()
        {
            using (ErpConnection dc = new ErpConnection())
            {
                var events = dc.AAA_Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        #region partial views

        public PartialViewResult HomeIndexPanelPartial()
        {
            return PartialView();
        }

        #endregion

        #region menu partial views

        public PartialViewResult Menu_DeptDropDown()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Home()
        {
            return PartialView();
        }
        
        public PartialViewResult Menu_Account()
        {
            return PartialView();
        }
        
        public PartialViewResult Menu_Admin()
        {
            return PartialView();
        }
        
        public PartialViewResult Menu_HR()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Compliance()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Marketing()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Quality()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Production()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Stores()
        {
            return PartialView();
        }

        public PartialViewResult Menu_ITCell()
        {
            return PartialView();
        }

        public PartialViewResult Menu_ELibrary()
        {
            return PartialView();
        }

        public PartialViewResult Menu_Maintenance()
        {
            return PartialView();
        }

        public PartialViewResult Menu_CRM()
        {
            return PartialView();
        }

        //not in use
        public PartialViewResult Menu_Module()
        {
            return PartialView();
        }

        //not in use
        public PartialViewResult Menu_Indent()
        {
            return PartialView();
        }

        //not in use
        public PartialViewResult Menu_Quail()
        {
            return PartialView();
        }

        #endregion

        private AlertsMdl getAlertSummary()
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            amdl = alertsBLL.getAlertListForUser();
            return amdl;
        }

        public string DashboardForX1(string strvalue = "")
        {
            //OK & working in V1 by performAPICall_GeneralPage()
            string[] pms = strvalue.Split('/');
            string uid = pms[0];
            string ccode = pms[1];
            string fy = pms[2];
            loginBLL.performLogin(uid, ccode, fy, "123", true, "Marketing");//note
            //
            AlertsMdl amdl= new AlertsMdl();
            amdl = getAlertSummary();
            //
            string result = "<table>";
            if (amdl.isBirthdayAlert == true)
            {
                result += "<tr>";
                result += "<td colspan='5'><div style='color:black;'><marquee>" + amdl.BirthdayAnnivMsg + "</marquee></div></td>";
                result += "</tr>";
            }
            for (int i = 0; i < amdl.AlertList.Count; i++)
            {
                result += "<tr>";
                result += "<td>" + amdl.AlertList[i].alertfor + "</td>";
                result += "<td>:</td>";
                result += "<td style='color:black;'>" + amdl.AlertList[i].result + "</td>";
                result += "<td>&nbsp;&nbsp;&nbsp;</td>";
                result += "<td>" + amdl.AlertList[i].srcurl.Replace("..", "../Dashboard/GoToErpV2.aspx?pa=") + "</td>";
                result += "</tr>";
            }
            result += "</table>";
            return result;
        }

        //to display controlled reports/documents in read only format
        public ActionResult IndexIFrameRpt(string reporturl, string reportpms)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            string hurlwithpms = baseurl + reporturl + "?" + reportpms;
            ViewBag.Src = hurlwithpms += "#toolbar=0&navpanes=0&scrollbar=0";
            HttpContext hc = System.Web.HttpContext.Current;
            int x = hc.Request.Browser.ScreenPixelsWidth;
            int y = hc.Request.Browser.ScreenPixelsHeight + 110;
            ViewBag.Style = "width:100%;height:" + y.ToString() + "px;";
            return View();
        }

        public ActionResult DisplayErpExplorer(string url = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            Session["xsid"] = objCookie.getUserId();
            ViewData["Dept"] = objCookie.getDepartment();//note

            string filename = "PRAG_ERP_EXPLORER.pdf";
            string reporturl = "Home/AnnexureHelp";
            string reportpms = "filename=" + filename + "";
            string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            string hurlwithpms = baseurl + reporturl + "?" + reportpms;
            
            ViewBag.Src = hurlwithpms += "#toolbar=0&navpanes=0&scrollbar=0";
            return View();
        }

        #region others

        //POST: /getItemRate -used in purchase/store/order
        [HttpPost]
        public JsonResult getItemRate(int itemid)
        {
            ItemMdl stimdl = new ItemMdl();
            stimdl = itemBLL.searchObject(itemid);
            return new JsonResult { Data = new { rate = stimdl.PurchaseRate,unit=stimdl.Unit } };
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Date()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        #endregion

        #region user manual

        public ActionResult HelpContent_General()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult HelpContent_Marketing()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult HelpContent_Production()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult HelpContent_Stores()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult HelpContent_HR()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult HelpContent_Quality()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        //get
        public ActionResult AnnexureHelp(string fileName)
        {
            Session["xsid"] = objCookie.getUserId();
            return RedirectToAction("getAnnexureFile", new { filename=fileName});
        }

        //get
        public ActionResult getAnnexureFile(string fileName)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            string path = Server.MapPath("~/App_Data/FileTrf/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return File(path + fileName, mc.getMimeType(fileName));
        }

        #endregion

    }
}
