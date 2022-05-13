using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_OrganisationChartController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        //
        // GET: /B_OrganisationChart/
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult AjaxMethod()
        {
            List<object> chartData = new List<object>();
            //string query = "select NewEmpId,EmpName,Grade from tbl_employee where Grade= 'd' and IsActive=1 ";
            string query = "SELECT tbl_employee.NewEmpId,tbl_employee.EmpName, AAAGrade.GradeName,tbl_employee.ReportingTo FROM tbl_employee INNER JOIN tbl_department ON tbl_employee.DepCode = tbl_department.DepCode INNER JOIN AAAGrade ON tbl_employee.Grade = AAAGrade.GradeCode where  IsActive = 1 and NewEmpId in(243,245,210,212,213,214,215,216,217,220,232,229,231,234,244,247,413,422,2528,2545,2547,2606,330,256,274,275,276,277,278,285,289,319,322,345,346,498,2527,2536,2537,2549,2613,2592,20,57,58,108,9,22,53,34,73,10,31,52,63,74,79,82,83,84,85,87,86,88,90,89,93,95,97,98,102,104,109,110,121,129,500,501,1525,1526,2544,425,468,428,466,2593,2602,385,387,401,404,405,409,411,426,441,442,430,467,443,469,445,446,447,448,449,450,476,477,473,475,478,481,482,1516,1523,2530,2531,2532,2534,2539,2541,2563,2564,2566,2567,2568,148,162,171,163,169,178,177,181,182,185,198,200,206,497,2535,2538,2594)";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                //SqlCommand cmd = new SqlCommand("OrgchartList", con);
                // cmd.CommandType = CommandType.Text;
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chartData.Add(new object[]
                            {
                              sdr["NewEmpId"],sdr["EmpName"], sdr["GradeName"] , sdr["ReportingTo"]
                            });
                        }
                    }
                    con.Close();
                }
            }
            return Json(chartData);
        }

        AAA_SendSMS_BLL Lists = new AAA_SendSMS_BLL();
        AAA_SendSMS_BLL BList = new AAA_SendSMS_BLL();

        public ActionResult ReportingTo()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            //  if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Reportingto = new SelectList(BList.Get_ReportingToEmployee(), "NewEmpId", "Empname");
            ModelState.Clear();
            EmployeeMdl model = new EmployeeMdl
            {
                GetempList = Lists.Get_ReportingToUpdate()
            };
            return View(model);
        }

        public JsonResult EditReportingTo_Record(int? id)
        {

            var hl = Lists.Get_ReportingToUpdate().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult EditReportingTo(EmployeeMdl objModel)
        {
            try
            {
                int result = Lists.UpdateReportingTo(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Updated Successfully";

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


    }
}
