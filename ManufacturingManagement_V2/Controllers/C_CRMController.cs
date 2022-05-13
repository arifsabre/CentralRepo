using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Controllers
{
    public class C_CRMController : Controller
    {
        // GET: C_CRM
        // GET: C_Fitting
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UserBLL bllUser = new UserBLL();
        CompanyBLL compBLL = new CompanyBLL();
        string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        private C_Fitting_ItemBLL bllObjectcomp = new C_Fitting_ItemBLL();
        private C_ComplaintBLL bllObject = new C_ComplaintBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        //public ActionResult MessageBox(string msg = "")
        //{
        //    ViewBag.Message = msg;
        //    //Response.AddHeader("Refresh", "5");
        //    return View();
        //}
        public ActionResult GetComplaintNo()
        {
            C_ComplaintMDI model = new C_ComplaintMDI();
            C_ComplaintBLL   bllObjectcomp1  = new C_ComplaintBLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = bllObjectcomp1.GetComplaintNo();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }
        public ActionResult ComplaintAssign()
        {
            C_ComplaintMDI model = new C_ComplaintMDI();
            C_ComplaintBLL bllObjectcomp1 = new C_ComplaintBLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = bllObjectcomp1.ComplaintAssignTo();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }
        public ActionResult ComplaintClosed()
        {
            C_ComplaintMDI model = new C_ComplaintMDI();
            C_ComplaintBLL bllObjectcomp1 = new C_ComplaintBLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = bllObjectcomp1.ComplaintClosed();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }
        private void setViewObject()
        {
            AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.ItemList = new SelectList(bllObjectcomp.getItemList(compcode), "ItemId", "ItemName");
            ViewBag.PragSRNO = new SelectList(bllObject.getPragSRNoWithWaranty(compcode), "srno", "pragno");
            ViewBag.AssignTo = new SelectList(Lists.get_EMPName_To_Issue(), "NewEmpId", "EmpName");
        }

     

        public ActionResult Index(int id = 0)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            }
            setViewObject();
            setViewData();
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            modelObject.Item_List = bllObject.GetAll_ComplaintsByCompany(compcode);
            return View(modelObject);
        }

        public ActionResult CheckWarantyIndex(int id = 0)
        {
            setViewData();
            C_Fitting_ItemMDI modelObject = new C_Fitting_ItemMDI();
            modelObject.Item_List = bllObjectcomp.GetAll_FittingItem_Details();
            return View(modelObject);
        }
        // GET: /CreateUpdate
        [HttpGet]
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            modelObject.dateofcomplaintrecieved = DateTime.Now;
            modelObject.customername = " ";
            modelObject.recievedby = " ";
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
        [HttpPost]
        public ActionResult CreateUpdate(C_ComplaintMDI modelObject)
        {
            //C_ComplaintMDI modelObject = new C_ComplaintMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";

            if (modelObject.referenceno == 0)//add
            {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
                Sendmail();
            }
            else
            {
              
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
               return RedirectToAction("GetComplaintNo", "C_CRM", new { msg = bllObject.Message });
              //   return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }
        // GET: /CreateUpdate
[HttpGet]
public ActionResult CreateUpdate1(int id = 0)
{
    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
    setViewObject();
    ViewData["AddEdit"] = "Save";
    C_ComplaintMDI modelObject = new C_ComplaintMDI();
    modelObject.dateofcomplaintrecieved = DateTime.Now;
    modelObject.customername = " ";
    modelObject.recievedby = " ";
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
[HttpPost]
public ActionResult CreateUpdate1(C_ComplaintMDI modelObject)
{
    //C_ComplaintMDI modelObject = new C_ComplaintMDI();
    if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Assign, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
    setViewObject();
    ViewBag.Message = "Permission Denied!";

    if (modelObject.referenceno == 0)//add
    {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
    }
    else
    {
        ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Assign, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
                SendmailAssignTo();
            }
    if (bllObject.Result == true)
    {
        return RedirectToAction("ComplaintAssign", "C_CRM", new { msg = bllObject.Message });
        //   return RedirectToAction("Index");
    }
    ViewBag.Message = bllObject.Message;
    return View(modelObject);
}
 // GET: /ClosedComplaint
[HttpGet]
public ActionResult ClosedComplaint(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Assign, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            //modelObject.dateofcomplaintrecieved = DateTime.Now;
            modelObject.attendeddate = DateTime.Now;
            modelObject.dateofclosed = DateTime.Now;
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
[HttpPost]
public ActionResult ClosedComplaint(C_ComplaintMDI modelObject)
        {
            //C_ComplaintMDI modelObject = new C_ComplaintMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Assign, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";

            if (modelObject.referenceno == 0)//add
            {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Assign, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.closedObject(modelObject);
               
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("ComplaintClosed", "C_CRM", new { msg = bllObject.Message });
                //   return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
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
      [HttpPost]
        public JsonResult DeleteCompt(int referenceno)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new C_ComplaintBLL();
            bllObject.deleteComplaint(referenceno);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }


        private void Sendmail()
        {

            int referenceno;
            string mobile1;
            string customername;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[c_complaint_SMS_NewComplain]", con)
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
                referenceno = Convert.ToInt32(ds.Tables[0].Rows[i]["referenceno"].ToString());
                mobile1 = ds.Tables[0].Rows[i]["mobile1"].ToString();
                customername = ds.Tables[0].Rows[i]["customername"].ToString();
                // mobile = ds.Tables[0].Rows[i]["Mobile"].ToString();
                //mobile1 = ds.Tables[0].Rows[i]["Mobile1"].ToString();
                //mobile2 = ds.Tables[0].Rows[i]["Mobile2"].ToString();
                //etime = ds.Tables[0].Rows[i]["ETime"].ToString();
                SendsmsNewCompalint(referenceno, mobile1);
                savedatanewcomplaint(referenceno, mobile1, customername);
                // WriteToFile("SMS SENT To:" + mobile  + "On" + "  " + DateTime.Now.ToString());
            }
        }
        private void savedatanewcomplaint(int referenceno, string mobile1, string customername)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[c_Compalint_Sent_SMSLog_Insert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@referenceno", referenceno));
                    cmd.Parameters.Add(new SqlParameter("@mobile1", mobile1));
                    cmd.Parameters.Add(new SqlParameter("@customername", customername));
                    //cmd.Parameters.Add(new SqlParameter("@mobile", mobile));
                    //cmd.Parameters.Add(new SqlParameter("@mobile1", mobile1));
                    //cmd.Parameters.Add(new SqlParameter("@mobile2", mobile2));
                    //cmd.Parameters.Add(new SqlParameter("@etime", etime));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    string k = e.Message;
                    //  Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }
        private void SendsmsNewCompalint(int referenceno, string mobile1)
        {
            DateTime yes = Yesterdaydate();
            string message = "Your Complaint has been Registered Successfully and  Your's Complaint Number is - " + " " + referenceno + " " + "Prag IT Deptt";
            using (var client = new System.Net.WebClient())
            {
                // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
               // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile1 + "&route=8" + "&peid=1601100000000010017" + "&text="
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile1 + "&route=2" + "&text=" + message;
                //http://smsfortius.com/api/mt/SendSMS?user=demo&password=demo123&senderid=WEBSMS&channel=Promo&DCS=0&flashsms=0&number=91989xxxxxxx,91999xxxxxxx&text=test message&route=##
                string response = client.DownloadString(url);

            }
        }
        static DateTime Yesterdaydate()
        {

            return DateTime.Now;
        }
        //SendAssign

        private void SendmailAssignTo()
        {

            int referenceno;
            string mobile2;
            string empname;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[c_complaint_SMSAssignTo_NewComplain]", con)
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
                referenceno = Convert.ToInt32(ds.Tables[0].Rows[i]["referenceno"].ToString());
                mobile2 = ds.Tables[0].Rows[i]["mobile2"].ToString();
                empname = ds.Tables[0].Rows[i]["empname"].ToString();
                // mobile = ds.Tables[0].Rows[i]["Mobile"].ToString();
                //mobile1 = ds.Tables[0].Rows[i]["Mobile1"].ToString();
                //mobile2 = ds.Tables[0].Rows[i]["Mobile2"].ToString();
                //etime = ds.Tables[0].Rows[i]["ETime"].ToString();
                SendsmsNewCompalintAssign(referenceno, mobile2,empname);
                savedatanewcomplaintAssign(referenceno, mobile2, empname);
                // WriteToFile("SMS SENT To:" + mobile  + "On" + "  " + DateTime.Now.ToString());
            }
        }
        private void savedatanewcomplaintAssign(int referenceno, string mobile2, string empname)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[c_Compalint_Sent_SMSLogAssign_Insert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@referenceno", referenceno));
                    cmd.Parameters.Add(new SqlParameter("@mobile2", mobile2));
                    cmd.Parameters.Add(new SqlParameter("@empname", empname));
                    //cmd.Parameters.Add(new SqlParameter("@mobile", mobile));
                    //cmd.Parameters.Add(new SqlParameter("@mobile1", mobile1));
                    //cmd.Parameters.Add(new SqlParameter("@mobile2", mobile2));
                    //cmd.Parameters.Add(new SqlParameter("@etime", etime));
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException e)
                {
                    string k = e.Message;
                    //  Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }
        private void SendsmsNewCompalintAssign(int referenceno, string mobile2,string empname)
        {
            DateTime yes = Yesterdaydate();
            string message = "Complaint Number -"  + referenceno +" has been Assign To - " + " " + empname + " " + "Prag IT Deptt";
            using (var client = new System.Net.WebClient())
            {
                // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile1 + "&route=8" + "&peid=1601100000000010017" + "&text="
                string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile2 + "&route=2" + "&text=" + message;
                //http://smsfortius.com/api/mt/SendSMS?user=demo&password=demo123&senderid=WEBSMS&channel=Promo&DCS=0&flashsms=0&number=91989xxxxxxx,91999xxxxxxx&text=test message&route=##
                string response = client.DownloadString(url);

            }
        }
       
        
        //ShedName
        public ActionResult GetShedSavedDetail()
        {
            C_ComplaintMDI model = new C_ComplaintMDI();
            C_ComplaintBLL bllObjectcomp1 = new C_ComplaintBLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = bllObjectcomp1.GetTop1ShedSaved();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }
        public ActionResult IndexShedName(int id = 0)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            }
            setViewObject();
            setViewData();
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            modelObject.Item_List = bllObject.GetAllShedList();
            return View(modelObject);
        }
        [HttpGet]
        public ActionResult Createshed(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            modelObject.StartDate = DateTime.Now;
            modelObject.EndDate = DateTime.Now;
            //modelObject.customername = " ";
            //modelObject.recievedby = " ";
            if (id != 0)
            {
                modelObject = bllObject.searchShedObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }
        [HttpPost]
        public ActionResult Createshed(C_ComplaintMDI modelObject)
        {
            //C_ComplaintMDI modelObject = new C_ComplaintMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            //setViewObject();
            ViewBag.Message = "Permission Denied!";

            if (modelObject.ShedId == 0)//add
            {
                //if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                //{
                //    return View();
                //}
                bllObject.insertShedObject(modelObject);
                
            }
            else
            {

                ViewData["AddEdit"] = "Update";
                //if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
                //{
                //    return View();
                //}
                bllObject.updateShedObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("GetShedSavedDetail", "C_CRM", new { msg = bllObject.Message });
                //   return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }
        [HttpGet]
        public ActionResult UpdateShed(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_ComplaintMDI modelObject = new C_ComplaintMDI();
            modelObject.StartDate = DateTime.Now;
            modelObject.EndDate = DateTime.Now;
            //modelObject.customername = " ";
            //modelObject.recievedby = " ";
            if (id != 0)
            {
                modelObject = bllObject.searchShedObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }
        [HttpPost]
        public ActionResult UpdateShed(C_ComplaintMDI modelObject)
        {
            //C_ComplaintMDI modelObject = new C_ComplaintMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";

            if (modelObject.ShedId == 0)//add
            {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertShedObject(modelObject);

            }
            else
            {

                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateShedObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("GetShedSavedDetail", "C_CRM", new { msg = bllObject.Message });
                //   return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult DeleteShed(int ShedId)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new C_ComplaintBLL();
            bllObject.deleteshed(ShedId);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }
    }
}
