using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Controllers
{
    public class HRController : Controller
    {
        readonly string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ItemBLL itemBLL = new ItemBLL();
        private AlertsBLL alertsBLL = new AlertsBLL();
        private WorkListBLL workListBLL = new WorkListBLL();
        private UserPermissionBLL bllObject = new UserPermissionBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private CompanyBLL compBLL = new CompanyBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        // GET: HR
        public ActionResult Index()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadMDI model = new LibFileUploadMDI();
            LibFileUploadBLL Lists = new LibFileUploadBLL();
            LibBLL List1 = new LibBLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ModelState.Clear();
            model.Item_List = Lists.LibGetAllFileDetail();
            ViewBag.total = model.Item_List.Count;


            List<LibTask> modelObject = new List<LibTask> { };
            modelObject = List1.GetTaskListByUserNotStarted(luserid);
            ViewBag.nstarted = modelObject.Count.ToString();


            List<LibTask> modelObjectns = new List<LibTask> { };
            LibBLL List2 = new LibBLL();
            modelObjectns = List2.GetTaskListByUserInProgress(luserid);
            ViewBag.inp = modelObjectns.Count.ToString();


            List<LibTask> modelObjectcomp = new List<LibTask> { };
            LibBLL List3 = new LibBLL();
            modelObjectcomp = List3.GetTaskListByUserCompleted(luserid);
            ViewBag.totalcomp = modelObjectcomp.Count.ToString();


            List<LibTask> modelObjecttotal = new List<LibTask> { };
            LibBLL List4 = new LibBLL();
            modelObjecttotal = List4.GetTaskListByUser(luserid);
            ViewBag.totaltask = modelObjecttotal.Count.ToString();


            List<LibTask> modelObjectTotalNotification = new List<LibTask> { };
            LibBLL List5 = new LibBLL();
            modelObjectTotalNotification = List5.GetAllNotificationByUserId(luserid);
            ViewBag.totalNotification = modelObjectTotalNotification.Count.ToString();


            List<LibTask> modelObjectAllNotification = new List<LibTask> { };
            LibBLL List6 = new LibBLL();
            modelObjectAllNotification = List5.GetAllNotification();
            ViewBag.AllNotice = modelObjectAllNotification.Count.ToString();

            //TotalActive
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            activelist = emplist.Employee_AllActiveList();
            ViewBag.ActiveList = activelist.Count.ToString();

            //TotalPresent
            List<EmployeeMdl> present = new List<EmployeeMdl> { };
            HRHomeBLL p = new HRHomeBLL();
            present = p.Employee_PresentList();
            ViewBag.Present = present.Count.ToString();

            //Absent
            //TotalAbsent
            List<EmployeeMdl> absent = new List<EmployeeMdl> { };
            HRHomeBLL a = new HRHomeBLL();
            absent = a.Employee_AbsentList();
            ViewBag.absent = absent.Count.ToString();

            //Late
            List<EmployeeMdl> late = new List<EmployeeMdl> { };
            HRHomeBLL l = new HRHomeBLL();
            late = l.Employee_LateList();
            ViewBag.late = late.Count.ToString();

            //UpcommingBirthday
            List<EmployeeMdl> birth = new List<EmployeeMdl> { };
            HRHomeBLL bir = new HRHomeBLL();
            birth = bir.Employee_BirthList();
            ViewBag.Birthlist = birth.Count.ToString();

            //MissingPunch
            List<EmployeeMdl> missing = new List<EmployeeMdl> { };
            HRHomeBLL miss = new HRHomeBLL();
            missing = miss.Employee_MissingPunch();
            ViewBag.Missing = missing.Count.ToString();

            //Retirement
            List<EmployeeMdl> mm = new List<EmployeeMdl> { };
            HRHomeBLL tt = new HRHomeBLL();
            mm = tt.Employee_Retirement();
            ViewBag.Retirement = mm.Count.ToString();

            //Anniversasry

            List<EmployeeMdl> oo = new List<EmployeeMdl> { };
            HRHomeBLL anniver = new HRHomeBLL();
            oo = anniver.Employee_MarriageAnniversary();
            ViewBag.Anniversary = oo.Count.ToString();
            //sms
            List<EmployeeMdl> smsmodelObject = new List<EmployeeMdl> { };
            HRHomeBLL homeobj = new HRHomeBLL();

            smsmodelObject = homeobj.Emp_AbsentSMSList();
            ViewBag.AbsentSMSList = smsmodelObject.Count.ToString();
            //
            List<EmployeeMdl> smsmodelObject1 = new List<EmployeeMdl> { };
            HRHomeBLL homeobj1 = new HRHomeBLL();

            smsmodelObject1 = homeobj1.Emp_LateSMSList();
            ViewBag.Latesmslist = smsmodelObject1.Count.ToString();
            //
            List<EmployeeMdl> smsmodelObject2 = new List<EmployeeMdl> { };
            HRHomeBLL homeobj2 = new HRHomeBLL();

            smsmodelObject2 = homeobj2.Employee_MissingPunch();
            ViewBag.Missingsmslist = smsmodelObject2.Count.ToString();
            //
            //
            List<EmployeeMdl> smsmodelObject9 = new List<EmployeeMdl> { };
            HRHomeBLL homeobj9 = new HRHomeBLL();

            smsmodelObject9 = homeobj9.Emp_MissingSentSMSList();
            ViewBag.AllMissingsmslist = smsmodelObject9.Count.ToString();
            return View(model);
        }
        public ActionResult EmpList()
        {

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_AllActiveList();
            return View(model);

        }
        public ActionResult EmpPresent()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_PresentList();
            return View(model);

        }
        public ActionResult EmpAbsent()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_AbsentList();

            return View(model);


        }
        public ActionResult EmpAbsentSms()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            SendAbsent();
           return View();
        }
        [HttpPost]
        public void SendAbsent()
        {
            string empcode;
            string empname;
            string mobileno;
            DateTime absentdate;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_Absent_SMS]", con)
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
                Sendsms(mobileno);
                SMSAbsentSaveData(empcode, empname, mobileno, absentdate);

            }
        }
        private void SMSAbsentSaveData(string empcode, string empname, string mobileno, DateTime absentdate)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_AbsentSMS_Log]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@absentdate", absentdate));

                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {

                    }

                }
                catch (SqlException e)
                {
                    string s = e.Message;
                }
                finally
                {
                    con.Close();

                }
            }
        }
       public ActionResult EmpLate()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_LateList();

            return View(model);

        }
        public ActionResult EmpmissingSms()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            SendMissing();
            return View();


        }
        public ActionResult EmpLateSms()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            SendLate();
            SendSMSLateHO();
            //SendLate_pp2_14_00();
            //SendLate_pp2_6_00();
            Response.AddHeader("Refresh", "5");

            return View();
       }
        public ActionResult EmployeeCommingBirthday()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_BirthList();
            return View(model);

        }
        public ActionResult EmployeeCommingAnniversary()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_MarriageAnniversary();
            return View(model);

        }
        public ActionResult EmployeeMissingPunch()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_MissingPunch();
            return View(model);

        }
        public ActionResult EmployeeRetirement()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Employee_Retirement();
            return View(model);
        }
        public ActionResult AbsentSMS()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Emp_AbsentSMSList();
            return View(model);

        }
        public ActionResult SentLateSMS()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Emp_LateSMSList();
            return View(model);
        }
        public ActionResult SentMissingSMS()
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            EmployeeMdl model = new EmployeeMdl();
            List<EmployeeMdl> activelist = new List<EmployeeMdl> { };
            HRHomeBLL emplist = new HRHomeBLL();
            model.Item_List = emplist.Emp_MissingSentSMSList();
            return View(model);
        }
      [HttpPost]
       private void Sendsms(string mobileno)
        {
            DateTime yes = Yesterdaydate();
            string message = "Your In/Out Punch is Not Present on" + " " + String.Format("{0:d/M/yyyy}", yes) + "You have Been Marked Absent.Please Contact HR Deptt PRAG INDUSTRIES (INDIA) PVT LIMITED";

            using (var client = new WebClient())
            {
                // url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);
            }
        }
        static DateTime Yesterdaydate()
        {


            return DateTime.Today;
        }
        private void SendMissing()
        {

            string empcode;
            string empname;
            string mobileno;
            DateTime edate;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_Usp_SMS_Missing_Punch_ForSMS]", con)
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
                edate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EDate"].ToString());
                Sendmisspunch(mobileno);
                SMSMissingSaveData(empcode, empname, mobileno, edate);
            }
        }
        private void SMSMissingSaveData(string empcode, string empname, string mobileno, DateTime edate)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_MissingSMS_Log]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                try
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@edate", edate));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {

                    }

                }
                catch (SqlException e)
                {
                    string s = e.Message;
                }
                finally
                {
                    con.Close();

                }
            }
        }
        private void Sendmisspunch(string mobileno)
        {
            DateTime yes = YesterdaydateMissing();
            string message = "Your In/Out Punch is Not Present on" + " " + String.Format("{0:d/M/yyyy}", yes) + "You have Been Marked Absent.Please Contact HR Deptt PRAG INDUSTRIES(INDIA) PVT LIMITED";

           // Your In/ Out Punch is Not Present on 27 / 7 / 2021 You have Been Marked Absent.Please Contact HR Deptt PRAG INDUSTRIES(INDIA) PVT LIMITED
            //string result;
            using (var client = new WebClient())
            {
                //string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                //string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=8" + "&peid=1601100000000010017" + "&text=" + message;
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);
            }
        }
        static DateTime YesterdaydateMissing()
        {

            return DateTime.Today.AddDays(-1);
        }

        private void SendLate()
        {
            string empcode;
            string empname;
            string mobileno;
            DateTime edate;
            string etime;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_Usp_Get_Mobile_No_SMSLate_Staff]", con)
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
                edate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EDate"].ToString());
                etime = ds.Tables[0].Rows[i]["ETime"].ToString();
                SendLateSMS(mobileno, empname);
                Latesmssavedata(empcode, empname, mobileno, edate, etime);

            }
        }
        private void Latesmssavedata(string empcode, string empname, string mobileno, DateTime edate, string etime)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_LateSMS_Log]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@edate", edate));
                    cmd.Parameters.Add(new SqlParameter("@etime", etime));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {

                    }

                }
                catch (SqlException e)
                {
                    string s = e.Message;
                }
                finally
                {
                    con.Close();

                }
            }
        }
        private void SendLateSMS(string mobileno, string empname)
        {
            DateTime yes = Yesterdaydate2();
            string message = "You have Been Marked Late On " + " " + String.Format("{0:d/M/yyyy}", yes) + " " + "Kindly Improve Your Reporting Time-H.R.Deptt";
            using (var client = new WebClient())
            {

                //string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=8" + "&peid=1601100000000010017" + "&text=" + message;
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);
            }
        }
        static DateTime Yesterdaydate2()
        {

            return DateTime.Today;
        }
        private void SendSMSLateHO()
        {

            string empcode;
            string empname;
            string mobileno;
            DateTime edate;
            string etime;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();

                SqlCommand com = new SqlCommand("[dbo].[ZZZ_USP_LateArrival_ByDate_Staff_Late_HO]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Clear();
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = com;
                adp.Fill(ds, "tbl");
                con.Close();
            }

            int nrow; nrow = ds.Tables[0].Rows.Count;
            for (int i = 0; i < nrow; i++)
            {
                empcode = ds.Tables[0].Rows[i]["EmpCode"].ToString();
                empname = ds.Tables[0].Rows[i]["EmpName"].ToString();
                mobileno = ds.Tables[0].Rows[i]["MobileNo"].ToString();
                edate = Convert.ToDateTime(ds.Tables[0].Rows[i]["EDate"].ToString());
                etime = ds.Tables[0].Rows[i]["ETime"].ToString();
                SendsmsLateHO(mobileno, empname);
                Latesmssavedata1(empcode, empname, mobileno, edate, etime);

            }
        }
        private void Latesmssavedata1(string empcode, string empname, string mobileno, DateTime edate, string etime)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_LateSMS_Log]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                try
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@empcode", empcode));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    cmd.Parameters.Add(new SqlParameter("@mobileno", mobileno));
                    cmd.Parameters.Add(new SqlParameter("@edate", edate));
                    cmd.Parameters.Add(new SqlParameter("@etime", etime));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {

                    }

                }
                catch (SqlException e)
                {
                    string s = e.Message;
                }
                finally
                {
                    con.Close();

                }
            }
        }
        private void SendsmsLateHO(string mobileno, string empname)
        {
            DateTime yes = Yesterdaydate();
            string message = "You have Been Marked Late On " + " " + String.Format("{0:d/M/yyyy}", yes) + " " + "Kindly Improve Your Reporting Time-H.R.Deptt";
            using (var client = new WebClient())
            {
                // http://sms.skysoftware.in/api/sendmsg.php?user=PRAGHO&pass=PRAGHO&sender=PRAGHO&phone=" + mobileno + "&text=" + message + "&priority=ndnd&stype=normal", new NameValueCollection() 

                //string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=8" + "&peid=1601100000000010017" + "&text=" + message;
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&route=2" + "&text=" + message;
                string response = client.DownloadString(url);


            }
        }
      
       
      
    
    }
}







