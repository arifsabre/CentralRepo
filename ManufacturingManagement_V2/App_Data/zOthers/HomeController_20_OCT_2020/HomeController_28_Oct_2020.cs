using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ManufacturingManagement_V2.Models;
using Newtonsoft.Json;
namespace ManufacturingManagement_V2.Controllers
{
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
        public ActionResult Index(string deptt = "Home")
        {
            ViewBag.Dept = deptt;
            //
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            loginBLL.changeCoockieForDepartment(deptt);
            setViewData();
            //
            
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadMDI model = new LibFileUploadMDI();
            LibFileUploadBLL Lists = new LibFileUploadBLL();
            LibBLL List1 = new LibBLL();

            //ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            //checking department accessibility except home & elibrary
            //to define check for Maintenance & ITCELL in tbl_deptmenu
            if (!(deptt == "Home" || deptt == "ELibrary" || deptt == "Maintenance" || deptt == "ITCELL"))
            {
                if (objCookie.getDeptMenuList().ToLower().Contains(deptt.ToLower()) == false)
                {
                    return RedirectToAction("Index", new { deptt = "Home" });
                }
            }

            if (deptt == "ELibrary")
            {
                return RedirectToAction("Index_GetAllFile", "Library");
            }
            else if (deptt == "Maintenance")
            {
                return RedirectToAction("ComplaintDashboard", "Copplaint");
            }

            AlertsMdl amdl = new AlertsMdl();
            amdl = getAlertSummary();
            if (deptt == "Compliance")
            {
                //amdl.AlertDetail = getAlertDetail("compliance").AlertDetail;
                return RedirectToAction("Compliance_Dashboard", "DisplayDashboard");
            }
            else if (deptt == "Quality")
            {
                //amdl.AlertDetail = getAlertDetail("quality").AlertDetail;
                return RedirectToAction("Quality_Dashboard","DisplayDashboard");
            }
            

            //ModelState.Clear();
            //model.Item_List = Lists.LibGetAllFileDetail(luserid);
            //ViewBag.total = model.Item_List.Count;
            ////NotStarted
            //List<LibTask> modelObject = new List<LibTask> { };
            //modelObject = List1.GetTaskListByUserNotStarted(luserid);
            //ViewBag.nstarted = modelObject.Count.ToString();
            ////Inprogress
            //List<LibTask> modelObjectns = new List<LibTask> { };
            //LibBLL List2 = new LibBLL();
            //modelObjectns = List2.GetTaskListByUserInProgress(luserid);
            //ViewBag.inp = modelObjectns.Count.ToString();
            ////Completed
            //List<LibTask> modelObjectcomp = new List<LibTask> { };
            //LibBLL List3 = new LibBLL();
            //modelObjectcomp = List3.GetTaskListByUserCompleted(luserid);
            //ViewBag.totalcomp = modelObjectcomp.Count.ToString();
            ////TotalTaskByUserId
            //List<LibTask> modelObjecttotal = new List<LibTask> { };
            //LibBLL List4 = new LibBLL();
            //modelObjecttotal = List4.GetTaskListByUser(luserid);
            //ViewBag.totaltask = modelObjecttotal.Count.ToString();

            ////GetAllNotification
            //List<LibTask> modelObjectTotalNotification = new List<LibTask> { };
            //LibBLL List5 = new LibBLL();
            //modelObjectTotalNotification = List5.GetAllNotificationByUserId(luserid);
            //ViewBag.totalNotification = modelObjectTotalNotification.Count.ToString();

            ////GetAllNotification
            //List<LibTask> modelObjectAllNotification = new List<LibTask> { };
            //LibBLL List6 = new LibBLL();
            //modelObjectAllNotification = List5.GetAllNotification();
            //@ViewBag.AllNotice = modelObjectAllNotification.Count.ToString();

            ////set 2

            //string alt= "OK TTTT KKK";
            //ViewBag.AlertSummary ="< html >< body > " + alt + " </ body ></ html > ";

            return View(amdl);
     
        }

        [HttpGet]
        public ActionResult Quality_Dashboard()
        {
            //
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            //note: dept to be get after setting
            ViewData["mnuOpt"] = objCookie.getMenu();
            //
            AlertsMdl amdl = new AlertsMdl();
            amdl = getAlertSummary();
            amdl.AlertDetail = getAlertDetail("quality").AlertDetail;
            return View(amdl);

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

        [HttpGet]
        public ActionResult Index1()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL Lists = new LibBLL();

            modelObject = Lists.GetTaskListByUserNotStarted(luserid);


            ViewBag.nstarted = modelObject.Count.ToString();
            return View(modelObject);
        }

        #region dashboards

        //public ActionResult AdminDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult StoreDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult MktgDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult HRDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult IndentDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult ProductionDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    return View(getAlertSummary());
        //}

        //public ActionResult ComplianceDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    AlertsMdl amdl = new AlertsMdl();
        //    amdl = getAlertSummary();
        //    amdl.AlertDetail = getAlertDetail("compliance").AlertDetail;
        //    return View(amdl);
        //}

        //public ActionResult QuailDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    AlertsMdl amdl = new AlertsMdl();
        //    amdl = getAlertSummary();
        //    //amdl.AlertDetail = getAlertDetail("compliance").AlertDetail;
        //    return View(amdl);
        //}

        //public ActionResult AccountDashboard()
        //{
        //    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
        //    setViewData();
        //    ViewData["Dept"] = objCookie.getDepartment();
        //    AlertsMdl amdl = new AlertsMdl();
        //    amdl = getAlertSummary();
        //    //amdl.AlertDetail = getAlertDetail("compliance").AlertDetail;
        //    return View(amdl);
        //}

        #endregion

        #region partial views

        public PartialViewResult HomeIndexPanelPartial()
        {
            return PartialView();
        }

        public PartialViewResult AlertPartialSummary()
        {
            return PartialView();
        }

        public PartialViewResult AlertPartialCompliance()
        {
            return PartialView();
        }

        public PartialViewResult AlertPartialQuality()
        {
            return PartialView();
        }

        //public PartialViewResult pageHeader()
        //{
        //    return PartialView();
        //}

        //public PartialViewResult pageFooter()
        //{
        //    return PartialView();
        //}

        //[HttpPost]
        //public PartialViewResult PopupModalDialogPartial(string customerid)
        //{
        //    AlertsMdl amdl = new AlertsMdl();
        //    alertsBLL = new AlertsBLL();
        //    //return PartialView("AlertPartialCompliance", getAlertDetail());
        //    return PartialView("PopupModalDialogPartial", getAlertDetail());
        //}
        //public ActionResult _PartialTreeView()
        //{


        //    List<SiteMenu> all = new List<SiteMenu>();
        //    using (ErpConnection dc = new ErpConnection())
        //    {
        //        all = dc.SiteMenu.OrderBy(a => a.ParentMenuID).ToList();
        //    }
        //    return PartialView("_PartialTreeView", all);
        //}

        //public ActionResult _PartialTreeITAssest()
        //{


        //    List<SiteMenu_ITAssest> all = new List<SiteMenu_ITAssest>();
        //    using (ErpConnection dc = new ErpConnection())
        //    {
        //        all = dc.SiteMenu_ITAssest.OrderBy(a => a.ParentMenuID).ToList();
        //    }
        //    return PartialView("_PartialTreeITAssest", all);
        //}

        //public ActionResult _PartialSiteMenuMedicalTestTree()
        //{


        //    List<SiteMenu_MedicalTest> all = new List<SiteMenu_MedicalTest>();
        //    using (ErpConnection dc = new ErpConnection())
        //    {
        //        all = dc.SiteMenu_MedicalTest.OrderBy(a => a.ParentMenuID).ToList();
        //    }
        //    return PartialView("_PartialSiteMenuMedicalTestTree", all);
        //}

        public JsonResult GetEvents()
        {
            using (ErpConnection dc = new ErpConnection())
            {
                var events = dc.AAA_Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
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

        #region alerts object

        private AlertsMdl getAlertSummary()
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            amdl = alertsBLL.getAlertListForUser();
            return amdl;
        }

        private AlertsMdl getAlertDetail(string opt = "")
        {
            AlertsMdl amdl = new AlertsMdl();
            alertsBLL = new AlertsBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            //opt1
            if (opt == "compliance")
            {
                ds = alertsBLL.getWorkListAlert();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string info = "<table class='table table-bordered'>";//style='font-size:10pt;'
                    info += "<tr>";
                    info += "<td class='indexColumn' width='30px;' valign='top'><b>S.No.</b></td>";
                    info += "<td class='indexColumn' width='40px;' valign='top'><b>Company</b></td>";
                    info += "<td class='indexColumn' width='40px;' valign='top'><b>ComplianceOf</b></td>";
                    info += "<td class='indexColumn' width='50px;' valign='top'><b>Task Date</b></td>";
                    info += "<td class='indexColumn' width='auto;' valign='top'><b>Task Name</b></td>";
                    info += "<td class='indexColumn' width='auto;' valign='top'><b>Description</b></td>";
                    info += "</tr>";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        info += "<tr>";
                        info += "<td class='indexColumn' valign='top' style='text-align:center;background-color:" + ds.Tables[0].Rows[i]["RecColor"].ToString() + ";color:white;'>" + (i + 1).ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["compname"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["depname"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["recdt"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["taskname"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["workdesc"].ToString() + "</td>";
                        info += "</tr>";
                    }
                    info += "</table>";
                    amdl.AlertDetail = "<html><body>" + info + "</body></html>";
                }
            }
            //opt 2
            else if (opt == "quality")
            {
                //[100043]--Set 1: 30 Days calibration alert
                CalibrationBLL calBLL = new CalibrationBLL();
                ds = calBLL.getCalibrationAlertDataV2(0, 0, DateTime.Now);
                DateTime cdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                DateTime duedate = new DateTime();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string info = "<table class='table table-bordered'>";//style='font-size:10pt;'
                    info += "<tr>";
                    info += "<td class='indexColumn' width='20px;' valign='top'><b>S.No.</b></td>";
                    info += "<td class='indexColumn' width='20px;' valign='top'><b>Company</b></td>";
                    //info += "<td class='indexColumn' width='20px;' valign='top'><b>Rec Id</b></td>";
                    info += "<td class='indexColumn' width='auto;' valign='top'><b>Id&nbsp;No</b></td>";
                    info += "<td class='indexColumn' width='auto;' valign='top'><b>Range</b></td>";
                    info += "<td class='indexColumn' width='auto;' valign='top'><b>Location</b></td>";
                    info += "<td class='indexColumn' width='30px;' valign='top'><b>Calibration&nbsp;Done&nbsp;On</b></td>";
                    //info += "<td class='indexColumn' width='20px;' valign='top'><b>Certificate No</b></td>";
                    //info += "<td class='indexColumn' width='20px;' valign='top'><b>Certified By</b></td>";
                    info += "<td class='indexColumn' width='30px;' valign='top'><b>Next&nbsp;Calibration&nbsp;Due&nbsp;On</b></td>";
                    info += "</tr>";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        info += "<tr>";
                        duedate = Convert.ToDateTime(ds.Tables[0].Rows[i]["nextcalibdate"].ToString());
                        //if (duedate < cdate) line formatted
                        //{
                        //    info += "<tr style='text-align:center;color:white;background-color:gray;'>";//background-color:red;
                        //}
                        //else
                        //{
                        //    info += "<tr style='text-align:center;color:black;font-weight:bold;background-color:white;'>";//background-color:yellow;
                        //}
                        info += "<td class='indexColumn' valign='top'>" + (i + 1).ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["CmpName"].ToString() + "</td>";
                        //info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["RecId"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["IdNo"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["ImteRange"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["Location"].ToString() + "</td>";
                        info += "<td class='indexColumn' valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["calibdate"].ToString())) + "</td>";
                        //info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["CertificateNo"].ToString() + "</td>";
                        //info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[i]["CertifiedBy"].ToString() + "</td>";
                        if (duedate < cdate)//column formatted
                        {
                            info += "<td class='indexColumn' valign='top' style='color:white;background-color:gray;'>" + mc.getStringByDate(duedate) + "</td>";
                        }
                        else
                        {
                            info += "<td class='indexColumn' valign='top' style='color:black;background-color:yellow;'>" + mc.getStringByDate(duedate) + "</td>";
                        }
                        info += "</tr>";
                    }
                    info += "</table>";
                    amdl.AlertDetail = "<html><body>" + info + "</body></html>";
                }
            }
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

        #endregion

        #region others

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

        public ActionResult DisplayErpV1(string url = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["Dept"] = objCookie.getDepartment();//note

            string pms = "uid="+objCookie.getUserId();
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

      #region testing

        // GET: API Test
        public ActionResult TestingAPI(string grade="s")
        {
            //to check for post
            EmployeeMdl std = new EmployeeMdl();
            std.EmpName = "abc";
            std.FatherName = "xyz";
            string inputJson = (new JavaScriptSerializer()).Serialize(std);
            //---------------------------------

            IEnumerable<EmployeeMdl> students = null;

            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59772/");
                
                //HTTP GET without parameter
                //var responseTask = client.GetAsync("StudentAPI/GetStaffList");
                //HTTP GET with parameter(s)
                var responseTask = client.GetAsync(string.Format("api/StudentAPI/GetStaffList?etype={0}&mxtype={1}&grade={2}", "1", "99", grade));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //var readTask = result.Content.ReadAsAsync<IList<EmployeeMdl>>();
                    var readTask = result.Content.ReadAsStringAsync().Result;
                    //readTask.Wait();

                    //students = readTask.Result;
                    students = (new JavaScriptSerializer()).Deserialize<List<EmployeeMdl>>(readTask);
                }
                else //web api sent error response 
                {
                    //log response status here..

                    students = Enumerable.Empty<EmployeeMdl>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(students);
        }


        [HttpGet]
        public ActionResult TestingAPI_Post()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestingAPI_Post(EmployeeMdl student)
        {
            UnitMdl umdl = new UnitMdl();
            umdl.UnitName = "T K K K X";
            umdl.Unit = 100;

            var client = new System.Net.Http.HttpClient();

            string json = JsonConvert.SerializeObject(umdl, Formatting.Indented);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);

            var byteContent = new System.Net.Http.ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var httpResponce = client.PostAsync("http://localhost:59772/api/StudentAPI/SaveStaff", byteContent).Result;
            
            //using (var client = new System.Net.Http.HttpClient())
            //{
            //    //client.BaseAddress = new Uri("http://localhost:59772/api/StudentAPI");
            //    client.BaseAddress = new Uri("http://localhost:59772/api/StudentAPI/SaveStaff");

            //    //HTTP POST
            //    //var postTask = client.PostAsJsonAsync<StudentViewModel>("student", student);
            //    //System.Net.Http.HttpContent hc = null;

            //    string inputJson = (new JavaScriptSerializer()).Serialize(student);
            //    var httpContent = new System.Net.Http.StringContent(inputJson);
            //    //var postTask = client.PostAsync("StudentAPI/SaveStaff", httpContent);
            //    var stringContent = new System.Net.Http.HttpContent(inputJson, System.Text.Encoding.UTF8, "application/json");
            //    var postTask = client.PostAsync("StudentAPI/SaveStaff", stringContent);
            //    //postTask.Wait();

            //    var result = postTask.Result;//not occured
            //    if (result.IsSuccessStatusCode)
            //    {
            //        return RedirectToAction("Index");
            //    }
            //}

            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(student);
        }

       #endregion

    }
}
