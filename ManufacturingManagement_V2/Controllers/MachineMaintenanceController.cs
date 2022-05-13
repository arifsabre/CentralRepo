using Dapper;
using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Controllers
{
    public class MachineMaintenanceController : Controller
    {
        // GET: MachineMaintenance
        public SqlConnection con;
        public string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
        MachineMaintenanceBLL model_bll = new MachineMaintenanceBLL();
        readonly LibFileUploadBLL Listowner = new LibFileUploadBLL();
        readonly string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        //MachineMaster
        public ActionResult IndexMachineName()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            ModelState.Clear();
            // model.machinename = "Enter";
            model.Item_List = model_bll.GetMachineNameList(compcode);
            return View(model);

        }
        //
        [HttpGet]
        public ActionResult Edit_MasterMachineRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMachineNameList(compcode).Find(x => x.machineid.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_MasterMachine(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.UpdateMasterMachine(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("IndexMachineName");

            }
            catch
            {
                throw;
            }

        }
        //
        [HttpPost]
        public ActionResult AddMasterMachine(MachineMaintenanceMDI empmodel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.InsertMasterMachine(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("index");
            }
            catch
            {
                throw;
            }
        }
        //MasterCheckPoint
        public ActionResult IndexMachineCheckpint()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.MachineList = new SelectList(model_bll.GetMachineNameByCompany(compcode), "machineid", "machinename");
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            ModelState.Clear();
            // model.machinename = "Enter";
            model.Item_List = model_bll.GetMachineCheckPointList(compcode);
            ViewBag.ScheduleList = new SelectList(model_bll.MachineSchedule(), "scheduleid", "schedulename");

            return View(model);

        }
        //
        [HttpGet]
        public ActionResult Edit_MasterCheckPointRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMachineCheckPointList(compcode).Find(x => x.checkpointid.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_MasterCheckPoint(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.UpdateMasterCheckPoint(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("IndexMachineCheckpint");

            }
            catch
            {
                throw;
            }

        }
        //
        [HttpPost]
        public ActionResult AddMasterCheckpoint(MachineMaintenanceMDI empmodel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.InsertMasterCheckPoint(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("index");
            }
            catch
            {
                throw;
            }
        }
        //
       public ActionResult IndexMachineScheduleList()
        {
            // int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            ModelState.Clear();
            // model.machinename = "Enter";
            model.Item_List = model_bll.GetMachineScheduleList();
            return View(model);
        }

        //breakdown
        public ActionResult IndexBreakDownList()
        {
             int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.ResponsibilityList = new SelectList(model_bll.getResplistbyCompany(Convert.ToInt32(objCookie.getCompCode())), "NewEmpId", "EmpName", objCookie.getCompCode());
            ViewBag.CheckResponsibilityList = new SelectList(model_bll.GetUserNameByCompany(compcode), "userid", "FullName");
            ViewBag.MachineList = new SelectList(model_bll.GetMachineNameByCompany(compcode), "machineid", "machinename");
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            ModelState.Clear();
            model.detailofworkdone = " ";
            model.breakreason = " ";
            model.Item_List1 = model_bll.GetBreakDownList(compcode);
            model.Item_List3 = model_bll.GetBreakDownHistory(compcode);


            return View(model);
        }
        //
        [HttpPost]
        public ActionResult AddBreakDown(MachineMaintenanceMDI empmodel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
           
          

                try
            {
               

               
                int result = model_bll.AddBreakTask(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("IndexBreakDownList");
            }
            catch
            {
                throw;
            }
        
    }
    //
        [HttpGet]
        public ActionResult Edit_BreakDownRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetBreakDownList(compcode).Find(x => x.breakid.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Detail_BreakDownRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetBreakDownList(compcode).Find(x => x.breakid.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateBreakDown(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.UpdateBreakdowntask(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("IndexBreakDownList");

            }
            catch
            {
                throw;
            }

        }
        //DailySchedule
        public ActionResult DailyScheduleTask()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            ModelState.Clear();
            // model.machinename = "Enter";
            model.Item_List = model_bll.GetAll_ScheduleTaskByCompany(compcode);
            ViewBag.TotalTask = model.Item_List.Count.ToString();
          
           
            return View(model);

        }
      
        public ActionResult Index()
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int luserid = Convert.ToInt32(objCookie.getUserId());
            AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
            MachineMaintenanceBLL List1 = new MachineMaintenanceBLL();
           
            ModelState.Clear();
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            model.ExternalMan = " Not Required";
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.MachineList = new SelectList(model_bll.GetMachineNameByCompany(compcode), "machineid", "MachineName");
            ViewBag.Checkpoint = new SelectList(model_bll.GetCheckPointByCompany(compcode), "checkpointid", "checkdetail");
            ViewBag.ScheduleList = new SelectList(model_bll.MachineSchedule(), "scheduleid", "schedulename");
            ViewBag.ResponsibilityList = new SelectList(model_bll.getResplistbyCompany(Convert.ToInt32(objCookie.getCompCode())), "NewEmpId", "EmpName", objCookie.getCompCode());
            ViewBag.CheckResponsibilityList = new SelectList(model_bll.GetUserNameByCompany(compcode), "userid", "FullName");
            model.Item_List = List1.GetMaintenanceTaskByCompany(compcode);
            ViewBag.totalMaster = model.Item_List.Count;
      
            MachineMaintenanceMDI z = new MachineMaintenanceMDI();
            MachineMaintenanceBLL x = new MachineMaintenanceBLL();
            int compcode8 = Convert.ToInt32(objCookie.getCompCode());
            z.Item_List = x.GetAll_ScheduleTaskByCompany(compcode8);
     
            return View(model);
        }

        //gethistory
        [HttpGet]
        public ActionResult HistoryDetails(int id)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //int userid = Convert.ToInt32(objCookie.getUserId());
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            MachineMaintenanceBLL machinebll = new MachineMaintenanceBLL();
            setViewData();
            model.HistoryItem_List = machinebll.GetHistoryById(id);
            //return PartialView("_Details", model);
            return View(model);

        }
     [HttpPost]
        public ActionResult AddMaintenance(MachineMaintenanceMDI empmodel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.AddMaintenanceTask(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
        //
        [HttpGet]
        public ActionResult EditMaintenanceRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetSchedule(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult UpdateCompanyRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateCompany(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.ChangeCompany(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");

            }
            catch
            {
                throw;
            }

        }
  
        [HttpGet]
        public ActionResult UpdateResponsibilityRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateResponsibility(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.ChangeResponsibility(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");

            }
            catch
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult UpdateCheckingResponsibilityRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult UpdateCheckResponsibility(MachineMaintenanceMDI objModel)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = model_bll.ChangeCheckResponsibility(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult EditMaintenance(MachineMaintenanceMDI objModel)
        {
          
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
          
            try
            {
                int result = model_bll.MaintenanceTaskUpdate(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");

            }
            catch
            {
                throw;
            }

        }
        private static List<SelectListItem> PopulateDropDown(string query, string textColumn, string valueColumn)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr[textColumn].ToString(),
                                Value = sdr[valueColumn].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        //deleteMaintenanceTask
        public ActionResult Delete_Task(int id)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            try
            {
                int result = model_bll.DeleteMaintenanaceTask(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
        //DeleteMachine
        public ActionResult Delete_Machine(int id)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            try
            {
                int result = model_bll.DeleteMachine(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("IndexMachineName");
            }
            catch
            {
                throw;
            }
        }
        //DeleteCheckPoint
        public ActionResult Delete_CheckPoint(int id)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            try
            {
                int result = model_bll.DeleteCheckPoint(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("IndexMachineCheckpint");
            }
            catch
            {
                throw;
            }
        }
        //DeleteBreakdown
        public ActionResult Delete_Breakdown(int id)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            try
            {
                int result = model_bll.DeleteBreakdown(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("IndexBreakDownList");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult ChangeCompany()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            return View();
        }
        [HttpPost]
        public ActionResult ChangeCompany(FormCollection form)
        {
            string compcode = form["ddlCompany"].ToString();
            if (objCookie.getCompCode() != compcode)
            {
                loginBLL.changeCoockieForCompany(compcode);
            }
            return RedirectToAction("Index");
        }
        public ActionResult IndexMachineMaintenanceHistoryReport()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.MachineList = new SelectList(model_bll.GetMachineNameByCompany(compcode), "machineid", "machinename");
            ViewBag.ScheduleList = new SelectList(model_bll.MachineSchedule(), "scheduleid", "schedulename");
            rptOptionMdl rptOption = new rptOptionMdl();
            //rptOption.DateTo = DateTime.Now;
            //rptOption.DateFrom = DateTime.Now;
            return View(rptOption);
        }
        private void setLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
        {
            DataTable lginfo = mc.getCrptLoginInfo();
            CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
            CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();
            CrystalDecisions.Shared.ConnectionInfo crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
            crConnectionInfo.ServerName = lginfo.Rows[0]["svrname"].ToString();
            crConnectionInfo.DatabaseName = lginfo.Rows[0]["dbname"].ToString();
            crConnectionInfo.UserID = lginfo.Rows[0]["userid"].ToString();
            crConnectionInfo.Password = lginfo.Rows[0]["passw"].ToString();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables = rptDoc.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }
        [HttpPost]
        public ActionResult Machine_Maintenance_History(rptOptionMdl rptOption)
        {
            //[100136]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.MachineMaintenance, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.MachineMaintenance, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                //int compcode = Convert.ToInt32(objCookie.getCompCode());
                string reporturl = "MachineMaintenance/Machine_Maintenance_History_File";
                string reportpms = "compcode=" + rptOption.CompCode + "";
               // string reportpms = "compcode=" + compcode + "";
                // reportpms += "dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                // reportpms += "compcode=" + rptOption.CompCode + "";
                reportpms += "machineid=" + rptOption.machineid + "";
                reportpms += "scheduleid=" + rptOption.scheduleid + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Machine_Maintenance_History_File", new { CompCode = rptOption.CompCode, scheduleid = rptOption.scheduleid, machineid = rptOption.machineid });
        }
        public ActionResult Machine_Maintenance_History_File(int CompCode, int machineid,int scheduleid)
        {
            //[100136]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "Machine_MaintenanceHistory.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            // cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
            //ZZZ_USP_GET_FirstIn_Lastout_MissingWithHours
            rptDoc.SetParameterValue("@compcode", CompCode);
            rptDoc.SetParameterValue("@machineid", machineid);
            rptDoc.SetParameterValue("@scheduleid", scheduleid);
            // rptDoc.SetParameterValue("@gradecode", schedileid);

            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();

            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            // return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }
        public ActionResult Machine_ServiceFile(HttpPostedFileBase postedFile, MachineMaintenanceMDI hld)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                // string query = "INSERT INTO ContractorFileUploadDetails VALUES (@FileName,@FileContent,@RecordId)";
                using (SqlCommand cmd = new SqlCommand("Machine_ServiceFile_Upload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                    cmd.Parameters.AddWithValue("@id", hld.id);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //  conn.Close();
                }
            }
          
            return RedirectToAction("Index");
            // return View(model);
        }
        [HttpGet]
        public ActionResult Machine_ServiceFile_Record(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            var hl = model_bll.GetMaintenanceTaskByCompany(compcode).Find(x => x.id.Equals(id));
            //var hl = model.GetAllCivil_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexServiceFile(int id=0)
        {
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //int userid = Convert.ToInt32(objCookie.getUserId());
            MachineMaintenanceMDI model = new MachineMaintenanceMDI();
            setViewData();
            model.Item_List2 = model_bll.GetAllServiceReport(id);
            //return PartialView("_Details", model);
            return View(model);
        }
        public FileContentResult GetServiceReport(int id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT FileId,FileContent,FileName FROM Machine_ServiceFile WHERE FileId = @id";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@id", id);
                //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                conn.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    id = Convert.ToInt32(rdr["FileId"].ToString());
                    fileContent = (byte[])rdr["FileContent"];
                    fileName = rdr["FileName"].ToString();
                }
            }
            return File(fileContent, fileName);
        }

        private void SendMaintenance()
        {
            int id; string shortname; string schedulename;  string machinename; string username; string nextduedate; string mobile;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[MachineMaintenanceList_Pending]", con)
                {

                    CommandType = CommandType.StoredProcedure
                };
               //com.Parameters.AddWithValue("@CompCode", compcode);
                //com.Parameters.Clear();
                SqlDataAdapter adp = new SqlDataAdapter
                {
                    SelectCommand = com
                };
                adp.Fill(ds, "tbl");
                con.Close();
            }
            int nrow;
            nrow = ds.Tables[0].Rows.Count;
            for (int i = 0; i < nrow; i++)
            {
                id = Convert.ToInt32(ds.Tables[0].Rows[i]["id"].ToString());
                shortname = ds.Tables[0].Rows[i]["ShortName"].ToString();
                schedulename = ds.Tables[0].Rows[i]["schedulename"].ToString();
                machinename = ds.Tables[0].Rows[i]["machinename"].ToString();
                username = ds.Tables[0].Rows[i]["username"].ToString();
                nextduedate = ds.Tables[0].Rows[i]["NextDueDate"].ToString();
                mobile = ds.Tables[0].Rows[i]["MobileNo"].ToString();
               CallApi(schedulename, machinename, nextduedate, mobile);
               MaintenanceInsertLog(id,shortname,schedulename,machinename,username, nextduedate, mobile);
               
            }
        }
        private void MaintenanceInsertLog( int id, string shortname, string schedulename,  string machinename, string username, string nextduedate, string mobile)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Machine_SMSLog_Insert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id",id));
                    cmd.Parameters.Add(new SqlParameter("@ShortName",shortname));
                    cmd.Parameters.Add(new SqlParameter("@schedulename",schedulename));
                    cmd.Parameters.Add(new SqlParameter("@machinename",machinename));
                    cmd.Parameters.Add(new SqlParameter("@username", username));
                    cmd.Parameters.Add(new SqlParameter("@NextDueDate",nextduedate));
                    cmd.Parameters.Add(new SqlParameter("@MobileNo",mobile));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }
        private void CallApi(string schedulename, string machinename,string nextduedate, string mobile)
        {
            // DateTime yes = Yesterdaydate();
            //Kind Attention Please, Maintenance of the {#var#} Machine duedate was {#var#} and Machine Schedule {#var#} is not Updated Regularly on the Erp System.Prag Industries India Pvt. Ltd
            string message = "Kind Attention Please, Maintenance of the " + machinename + " " + "Machine duedate was" + " " + nextduedate + " " + "and Machine Schedule" + " " + schedulename + " " + "is not Updated Regularly on the Erp System.Prag Industries India Pvt. Ltd";
            using (var client = new System.Net.WebClient())
            {
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);
            }
        }
      
        [HttpGet]
        public ActionResult MachinePendingMaintenance()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.MachineMaintenance, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
           // ViewBag.msg = "Sent Message";
            //SendMaintenance();
            return View();
        }
        [HttpPost]
        public ActionResult MachinePendingMaintenancePost()
        {
            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            //if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            //setViewData();
            SendMaintenance();
            ViewBag.msg = "Sent Message";
            return RedirectToAction("MachinePendingMaintenance");
        }
       
        
        [HttpPost]
        public ActionResult MachinePendingMaintenanceCompanywise()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            //if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.MachineMaintenance, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            //setViewData();
            SendMaintenancebycompany(compcode);
            ViewBag.msg = "Sent Message";
            return RedirectToAction("MachinePendingMaintenance");
        }
        private void SendMaintenancebycompany(int compcode=0)
        {
            int id; string shortname; string schedulename; string machinename; string username; string nextduedate; string mobile;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[MachineMaintenanceList_PendingByCompany]", con)
                {

                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@CompCode", compcode);
                //com.Parameters.Clear();
                SqlDataAdapter adp = new SqlDataAdapter
                {
                    SelectCommand = com
                };
                adp.Fill(ds, "tbl");
                con.Close();
            }
            int nrow;
            nrow = ds.Tables[0].Rows.Count;
            for (int i = 0; i < nrow; i++)
            {
                id = Convert.ToInt32(ds.Tables[0].Rows[i]["id"].ToString());
                shortname = ds.Tables[0].Rows[i]["ShortName"].ToString();
                schedulename = ds.Tables[0].Rows[i]["schedulename"].ToString();
                machinename = ds.Tables[0].Rows[i]["machinename"].ToString();
                username = ds.Tables[0].Rows[i]["username"].ToString();
                nextduedate = ds.Tables[0].Rows[i]["NextDueDate"].ToString();
                mobile = ds.Tables[0].Rows[i]["MobileNo"].ToString();
                CallApibycompany(schedulename, machinename, nextduedate, mobile);
                MaintenanceInsertLogbycompany(id, shortname, schedulename, machinename, username, nextduedate, mobile);
            }
        }
        private void MaintenanceInsertLogbycompany(int id, string shortname, string schedulename, string machinename, string username, string nextduedate, string mobile)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Machine_SMSLog_Insert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.Parameters.Add(new SqlParameter("@ShortName", shortname));
                    cmd.Parameters.Add(new SqlParameter("@schedulename", schedulename));
                    cmd.Parameters.Add(new SqlParameter("@machinename", machinename));
                    cmd.Parameters.Add(new SqlParameter("@username", username));
                    cmd.Parameters.Add(new SqlParameter("@NextDueDate", nextduedate));
                    cmd.Parameters.Add(new SqlParameter("@MobileNo", mobile));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }
        private void CallApibycompany(string schedulename, string machinename, string nextduedate, string mobile)
        {
            // DateTime yes = Yesterdaydate();
            //Kind Attention Please, Maintenance of the {#var#} Machine duedate was {#var#} and Machine Schedule {#var#} is not Updated Regularly on the Erp System.Prag Industries India Pvt. Ltd
            string message = "Kind Attention Please, Maintenance of the " + machinename + " " + "Machine duedate was" + " " + nextduedate + " " + "and Machine Schedule" + " " + schedulename + " " + "is not Updated Regularly on the Erp System.Prag Industries India Pvt. Ltd";
            using (var client = new System.Net.WebClient())
            {
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);
            }
        }

    

    }
}

