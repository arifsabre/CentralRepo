using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Controllers
{
    public class LibTaskController : Controller
    {
      //
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        LibBLL Lists = new LibBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult Index()
        {
            LibTask modell = new LibTask();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            ViewBag.TaskStatus = new SelectList(Lists.LibgetTaskStatusDropDown(), "SId", "Status");
           ViewBag.TaskPriority = new SelectList(Lists.LibgetTaskPriorityDropDown(), "PId", "Priority");
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LibTask model = new LibTask
            {
              
                TaskList = Lists.GetTaskListByUser(luserid)
            };
            model.Task = " ";
           // model.SId = 2;
            ViewBag.total = model.TaskList.Count;


            //NotStarted
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL List1 = new LibBLL();
            modelObject = List1.GetTaskListByUserNotStarted(luserid);
            ViewBag.nstarted = modelObject.Count.ToString();
            //Inprogress
            List<LibTask> modelObjectns = new List<LibTask> { };
            LibBLL List2 = new LibBLL();
            modelObjectns = List2.GetTaskListByUserInProgress(luserid);
            ViewBag.inp = modelObjectns.Count.ToString();
            //Completed
            List<LibTask> modelObjectcomp = new List<LibTask> { };
            LibBLL List3 = new LibBLL();
            modelObjectcomp = List3.GetTaskListByUserCompleted(luserid);
            ViewBag.totalcomp = modelObjectcomp.Count.ToString();
            //TotalTaskByUserId
            List<LibTask> modelObjecttotal = new List<LibTask> { };
            LibBLL List4 = new LibBLL();
            modelObjecttotal = List4.GetTaskListByUser(luserid);
            ViewBag.totaltask = modelObjecttotal.Count.ToString();

            return View(model);
        }
        public ActionResult GetTaskListNotStarted()
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

        public ActionResult GetTaskListInProgress()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL Lists = new LibBLL();

            modelObject = Lists.GetTaskListByUserInProgress(luserid);


            ViewBag.total = modelObject.Count.ToString();
            return View(modelObject);
        }


        public ActionResult GetTotalTaskCompleted(int userid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL Lists = new LibBLL();

            modelObject = Lists.GetTaskListByUserCompleted(luserid);


            ViewBag.total = modelObject.Count.ToString();
            return View(modelObject);
        }

        public ActionResult GetTotalTask(int userid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL Lists = new LibBLL();

            modelObject = Lists.GetTotalTaskByUser(luserid);


            ViewBag.total = modelObject.Count.ToString();
            return View(modelObject);
        }


        public ActionResult GetAllTaskList(int userid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            List<LibTask> modelObject = new List<LibTask> { };
            LibBLL Lists = new LibBLL();

            modelObject = Lists.GetTaskListByUser(luserid);
            ViewBag.total = modelObject.Count.ToString();
            return View(modelObject);
        }

        public ActionResult GetAllTaskListWithoutUser(int userid = 0)
        {
            //LibTask model = new LibTask();
            int luserid = Convert.ToInt32(objCookie.getUserId());
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.TaskStatus = new SelectList(Lists.LibgetTaskStatusDropDown(), "SId", "Status");
            ViewBag.TaskPriority = new SelectList(Lists.LibgetTaskPriorityDropDown(), "PId", "Priority");
            //int luserid = Convert.ToInt32(objCookie.getUserId());
            LibTask model = new LibTask
            {
                Item_List = Lists.GetAllTaskList(luserid)
            };
            // ViewBag.total = model.TaskList.Count;
            return View(model);
        }



        [HttpPost]
        public ActionResult InsertNewTask(LibTask objModel)
        {
            LibTask model = new LibTask();
            try
            {
                int result = Lists.AddNewTask(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record inserted Successfully";

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

        //details
        [HttpGet]
        public ActionResult Edit_TaskListRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Lists.GetTaskListByUser(luserid).Find(x => x.TaskId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_TasklList(LibTask objModel)
        {
            try
            {
                int result = Lists.Update_TaskList(objModel);
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
        //Status
        [HttpGet]
        public ActionResult Edit_TaskStatusRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Lists.GetTaskListByUser(luserid).Find(x => x.TaskId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_TaskStatus(LibTask objModel)
        {
            try
            {
                int result = Lists.Update_TaskStatus(objModel);
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
        //Priority
        [HttpGet]
        public ActionResult Edit_TaskPriorityRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Lists.GetTaskListByUser(luserid).Find(x => x.TaskId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_TaskPriority(LibTask objModel)
        {
            try
            {
                int result = Lists.Update_TaskPriority(objModel);
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
        //TaskDate
        [HttpGet]
        public ActionResult Edit_TaskDateRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Lists.GetTaskListByUser(luserid).Find(x => x.TaskId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_TaskDate(LibTask objModel)
        {
            try
            {
                int result = Lists.Update_TaskDate(objModel);
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

//deleteTask
        public ActionResult DeleteTask(int id)
        {
            try
            {
                int result = Lists.DeleteTaskItem(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
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

        public ActionResult GetTaskListByUser(int userid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            LibBLL Lists = new LibBLL();
            LibTask model = new LibTask
            {
                //Status = 0,
                //TaskStatus = "Completed",
                TaskList = Lists.GetTaskListByUser(luserid)
            };
            return View(model);
        }

        public ActionResult GetNotificationListByUser(int userid = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            LibBLL Lists = new LibBLL();
            LibTask model = new LibTask
            {
                //Status = 0,
                //TaskStatus = "Completed",
                TaskList = Lists.GetAllNotificationByUserId(luserid)
            };
            return View(model);
        }


        public ActionResult GetAllNotificationList()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            LibBLL Lists = new LibBLL();
            LibTask model = new LibTask
            {
                TaskList = Lists.GetAllNotification()
            };
            return View(model);
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

    }
}