using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class BioSyncDateController : Controller
    {
        readonly string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        private ItemBLL itemBLL = new ItemBLL();
        private AlertsBLL alertsBLL = new AlertsBLL();
        private WorkListBLL workListBLL = new WorkListBLL();
        private UserPermissionBLL bllObject = new UserPermissionBLL();
        private LoginBLL loginBLL = new LoginBLL();
      
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        AAABioSyncDateBLL activedate = new AAABioSyncDateBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult Index(string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOpt = new rptOptionMdl();
            rptOpt.ReportDate = DateTime.Now;
            ViewBag.Message = msg;
            rptOpt.Item_List = activedate.Get_BiometricLastUpdate();
            return View(rptOpt);
        }
        [HttpPost]
        public ActionResult UpdateBIOAttendanceBydate(rptOptionMdl objModel)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            
            
            activedate.Update_BIO_Attendance(objModel.ReportDate);
            string msg = activedate.Message + "Attendance For All Employees Date: " + mc.getStringByDate(objModel.ReportDate);
            ViewBag.Message = msg;
            return RedirectToAction("Index", new { msg = msg });
        }
      
        [HttpPost]
        public ActionResult UpdateBIOAttendanceBydateAAAASendSMS(rptOptionMdl objModel)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
           
            activedate.Update_BIO_AttendanceFromAAAASMSSend(objModel.ReportDate);
            string msg1 = activedate.Message + " Attendance For Staff Only Date: " + mc.getStringByDate(objModel.ReportDate);
            ViewBag.Message1 = msg1;
            return RedirectToAction("Index", new { msg = msg1 });
        }

        [HttpPost]
        public ActionResult UpdateAttendance_Worker(rptOptionMdl objModel)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            string msg1 = activedate.Message + "Mark Absent & Present : " + mc.getStringByDate(objModel.ReportDate);

            SendmailWorkerA();
            SendmailWorkerP();
            ViewBag.absent = msg1;
            return RedirectToAction("Index",new { msg = msg1 });
           
        }

        [HttpPost]
        public ActionResult AbsentPresentLog()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            SendmailWorkerA();
            SendmailWorkerP();
            // ViewBag.absent = "Data Has Been Converted";
            return RedirectToAction("AbsentPresentLog");
        }
        private void SendmailWorkerA()
        {
            string empcode;
            string empname;
            string mobileno;
            DateTime absentdate;
            Int32 compcode;
            //string etime;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[Emp_Absent_SMS_WorkerA]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Clear();
                SqlDataAdapter adp = new SqlDataAdapter
                {
                    SelectCommand = com
                };
                adp.Fill(ds, "tbl");
                con.Close();
            }

            int nrow; nrow = ds.Tables[0].Rows.Count;
            for (int i = 0; i < nrow; i++)
            {
                empcode = ds.Tables[0].Rows[i]["EmpCode"].ToString();
                empname = ds.Tables[0].Rows[i]["EmpName"].ToString();
                mobileno = ds.Tables[0].Rows[i]["MobileNo"].ToString();
                absentdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AbsentDate"].ToString());
                compcode = Convert.ToInt32(ds.Tables[0].Rows[i]["Compcode"].ToString());
                SMSAbsentSaveDataWorkerA(empcode, empname, mobileno, absentdate, compcode);
                //WriteToFile("SMS SENT To:" + mobileno + " " +empname +" " + "On" + "  " + DateTime.Now.ToString());
            }
        }
        private void SMSAbsentSaveDataWorkerA(string empcode, string empname, string mobileno, DateTime absentdate, Int32 compcode)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_AbsentSMS_LogA]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@absentdate", absentdate));
                    cmd.Parameters.Add(new SqlParameter("@Compcode", compcode));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    //  Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }
        private void SendmailWorkerP()
        {
            string empcode;
            string empname;
            string mobileno;
            DateTime absentdate;
            Int32 compcode;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[Emp_Absent_SMS_WorkerP]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Clear();
                SqlDataAdapter adp = new SqlDataAdapter
                {
                    SelectCommand = com
                };
                adp.Fill(ds, "tbl");
                con.Close();
            }

            int nrow; nrow = ds.Tables[0].Rows.Count;
            for (int i = 0; i < nrow; i++)
            {
                empcode = ds.Tables[0].Rows[i]["EmpCode"].ToString();
                empname = ds.Tables[0].Rows[i]["EmpName"].ToString();
                mobileno = ds.Tables[0].Rows[i]["MobileNo"].ToString();
                absentdate = Convert.ToDateTime(ds.Tables[0].Rows[i]["AbsentDate"].ToString());
                compcode = Convert.ToInt32(ds.Tables[0].Rows[i]["Compcode"].ToString());
                SMSAbsentSaveDataWorkerP(empcode, empname, mobileno, absentdate, compcode);

            }
        }
        private void SMSAbsentSaveDataWorkerP(string empcode, string empname, string mobileno, DateTime absentdate, int compcode)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_AbsentSMS_LogP]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@absentdate", absentdate));
                    cmd.Parameters.Add(new SqlParameter("@Compcode", compcode));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    //  Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }




    }
}
