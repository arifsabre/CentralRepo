using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Net.Mail;
using ManufacturingManagement_V2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using Dapper;
using System.Threading;
using System.Text;

namespace ManufacturingManagement_V2.Controllers
{
    public class MessageController : Controller
    {
        // GET: /Message/
        private SqlConnection con;
        private string constr;
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        MessageModel bll = new MessageModel();
        string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        [HttpGet]
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Memo, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            int userid = Convert.ToInt32(objCookie.getUserId());
            MessageModel model = new MessageModel();
            model.Item_List = model.GetAllMailSent();
           // model.Item_List1 = model.GetEmailList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Memo, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            MessageModel modelObject = new MessageModel();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.AllUserList = new SelectList(bll.GetAllUserName(), "userid", "FullName");
            modelObject.Item_List1 = modelObject.GetEmailList();
            return View(modelObject);
        }
        [HttpGet]
        public ActionResult MailList()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            MessageModel modelObject = new MessageModel();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            modelObject.Item_List1 = modelObject.GetEmailList();

            return View(modelObject);
        }
        [HttpPost]
        public ActionResult Index(MessageModel model, List<HttpPostedFileBase> attachments)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Memo, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
           setViewData();
           UploadFilemail(model);
           Sendmail(model.SendTo);
           UploadFile(model);
         
            //Thread.Sleep(20000);//for 20second
            //SendmailRecipient(model);
           
            return RedirectToAction("Index_Top1Record");
        }

        public void UploadFilemail(MessageModel model)
        {
            using (MailMessage mm = new MailMessage(model.Email, model.SendTo))
            {
               
                string to = model.SendTo;
                string[] Multiple = to.Split(',');
                mm.Subject = model.Subject;
                mm.Body = model.Detail;
                foreach (string multiple_email in Multiple)
                {
                    mm.To.Add(new MailAddress(multiple_email));
                }

                foreach (HttpPostedFileBase attachment in model.files)
                {
                    if (attachment != null)
                    {
                        string fileName = Path.GetFileName(attachment.FileName);
                        mm.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                    }
                }

                
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(model.Email, model.Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
              
            }
        }
       public void UploadFile(MessageModel model)
        {
           var recordId = SaveMailDetails(model);
           foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null)
                {
                    Stream str = file.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                    MessageModel Fd = new Models.MessageModel();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }
        }
        private void SaveFileDetails(MessageModel objDet)
        {

            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            Parm.Add("@RecordId", objDet.RecordId);
            DbConnection();
            con.Open();
            con.Execute("Memo_FileInsert", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        private string SaveMailDetails(MessageModel model)
        {

            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@CompCode", model.CompCode);
            // Parm.Add("@RecordId", model.RecordId);
            Parm.Add("@Subject", model.Subject);
            Parm.Add("@SendTo", model.SendTo);
            Parm.Add("@Detail", model.Detail);
            Parm.Add("@UserId", objCookie.getUserId());
            Parm.Add("@FullName", model.FullName);
            DbConnection();
            con.Open();
            con.Execute("Memo_Insert", Parm, commandType: CommandType.StoredProcedure);
            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.Memo, "RecordId");
            return recid;
        }
        [HttpGet]
        public ActionResult Search(string term)
        {
            using (ErpConnection db = new ErpConnection())
            {
                return Json(db.tbl_users.Where(p => p.FullName.Contains(term)).Select(p => p.FullName).ToList(), JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult SearchFullName(string term)
        {
            using (ErpConnection db = new ErpConnection())
            {
                return Json(db.tbl_users.Where(p => p.FullName.Contains(term)).Select(p => p.FullName).ToList(), JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {

            // int userid = Convert.ToInt32(objCookie.getUserId());
            List<MessageModel> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }
        [HttpGet]
        public PartialViewResult FileDetails()
        {

            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<MessageModel> DetList = GetFileList();

            return PartialView("FileDetails", DetList);
        }
        private List<MessageModel> GetFileList()
        {
            // int userid = Convert.ToInt32(objCookie.getUserId());
            List<MessageModel> DetList = new List<MessageModel>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<MessageModel>(con, "Memo_GetAllFileDownload", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public ActionResult Index_Top1Record()
        {
            setViewData();
            MessageModel model = new MessageModel();
            model.Item_List1 = model.Gettop1();
            //return PartialView("_Details", model);
            return View(model);
        }
        public FileContentResult ViewAttachedFile(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM Memo_FileDetails WHERE Id = @ID";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@ID", Id);
                //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                conn.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    Id = Convert.ToInt32(rdr["Id"].ToString());
                    fileContent = (byte[])rdr["FileContent"];
                    fileName = rdr["FileName"].ToString();
                }
            }
            return File(fileContent, fileName);
        }
        public ActionResult Details(int id)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.Memo, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }

            //int userid = Convert.ToInt32(objCookie.getUserId());
            MessageModel model = new MessageModel();
            setViewData();
            model.Item_List1 = model.GetAttachmentByRecordId(id);
          
            //return PartialView("_Details", model);
            return View(model);
        }
        [HttpGet]
        public ActionResult MemoRecord_V(int? id)
        {
           // int userid = Convert.ToInt32(objCookie.getUserId());
            MessageModel model = new MessageModel();
            var hl = model.GetAllMailSent().Find(x => x.RecordId.Equals(id));
            //model.Item_List3 = model.GetFullUserName(recordid);
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteMemo(int id)
        {
            MessageModel bll = new MessageModel();
            if (mc.getPermission(Entry.Memo, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = bll.DeleteMemoRecord(id);
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
        public ActionResult SelecMail()
        {
            if (mc.getPermission(Entry.Memo, permissionType.Add) == false)
            {
                //return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a><br/>[" + Convert.ToInt32(Entry.User_Permission) + "]");
                return Content("<a href='#' onclick='javascript:window.history.back();'><h1>Permission Denied!</h1></a><br/>[" + Convert.ToInt32(Entry.User_Permission) + "]");
            }
            setViewData();
            MessageModel modelObject = new MessageModel { };
            modelObject.Item_List = modelObject.GetEmailList();
           return View(modelObject);
        }
         [HttpPost]
        public ActionResult Memo_Update_Email(MessageModel objModel)
        {
            if (mc.getPermission(Entry.Memo, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = bll.UpdateEmailAddress(objModel);
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

                return RedirectToAction("Create");
            }
            catch
            {
                throw;
            }
        }
        private void Sendmail(string sendTo)
        {
            string FullName;
            int RecordId;
            string mobile;
            string username;
            string FullNameRec;
            string Email;

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[Memo_Mobile_FromMails]", con)
                {

                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@SendTo", sendTo);

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
                RecordId = Convert.ToInt32(ds.Tables[0].Rows[i]["RecordId"].ToString());
                mobile = ds.Tables[0].Rows[i]["MobileNo"].ToString();
                username = ds.Tables[0].Rows[i]["username"].ToString();
                FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                FullNameRec = ds.Tables[0].Rows[i]["FullNameRec"].ToString();
                Email = ds.Tables[0].Rows[i]["Email"].ToString();
                SendsmsMemo(RecordId, mobile, FullName);
                savedMemmoSentDetails(mobile, username, RecordId, FullName,FullNameRec,Email);
                // WriteToFile("SMS SENT To:" + mobile  + "On" + "  " + DateTime.Now.ToString());
            }
        }
        private void savedMemmoSentDetails(string mobile, string username, int RecordId, string FullName,string FullNameRec,string Email)
        {
            using (SqlConnection con = new SqlConnection(mycon))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[Memo_SMSLog_Insert]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RecordId", RecordId));
                    cmd.Parameters.Add(new SqlParameter("@FullName", FullName));
                    cmd.Parameters.Add(new SqlParameter("@MobileNo", mobile));
                    cmd.Parameters.Add(new SqlParameter("@UserName", username));
                    cmd.Parameters.Add(new SqlParameter("@FullNameRec", FullNameRec));
                    cmd.Parameters.Add(new SqlParameter("@Email",Email));

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
        private void SendsmsMemo(int RecordId, string mobile, string FullName)
        {
            //{#var#} has been sent Memo Number {#var#} Dated {#var#} Prag Industries India Pvt Ltd

           // {#var#} has sent you Memo Number {#var#} Dated {#var#} through ERP system. Kindly check the same. Prag Industries India Pvt Ltd
            DateTime yes = Yesterdaydate();
                //string message = FullName+ " " + "has been sent Memo Number" +" " +RecordId + " " +"Dated" + " " +yes +" "+ "Prag Industries India Pvt Ltd";
                string message = FullName + " " + "has sent you Memo Number" + " " + RecordId + " " + "Dated" + " " + yes + " " + "through ERP system. Kindly check the same. Prag Industries India Pvt Ltd";
                using (var client = new System.Net.WebClient())
                {
                    // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobileno + "&text=" + message + "&route=02";
                    // string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile1 + "&route=8" + "&peid=1601100000000010017" + "&text="
                    string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + mobile + "&route=2" + "&text=" + message;
                    //http://smsfortius.com/api/mt/SendSMS?user=demo&password=demo123&senderid=WEBSMS&channel=Promo&DCS=0&flashsms=0&number=91989xxxxxxx,91999xxxxxxx&text=test message&route=##
                    string response = client.DownloadString(url);
               }
            }
       static DateTime Yesterdaydate()
            {

            return DateTime.Now;
                //}
            }

       [HttpGet]
        public ActionResult ReciepientList()
        {
            setViewData();
            MessageModel model = new MessageModel();
            model.Item_List = model.GetAllMailSent();
           // model.Item_List3 = model.GetFullUserName(int id);
            //return PartialView("_Details", model);
            return View(model);
        }

        private void SetLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
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
        public ActionResult Memo_PrintMail(MessageModel rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.ItAssest, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Memo, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "Message/Memo_PrintMailFile";
               string reportpms = "&RecordId=" + rptOption.RecordId + "";
                // reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                //reportpms += "&compcode=" + rptOption.CompCode + "";
                //reportpms += "&grade=" + rptOption.Grade + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Memo_PrintMailFile", new { recordid = rptOption.RecordId });
        }

        public ActionResult Memo_PrintMailFile( int recordid)
        {
            //by usp_get_indent_report
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "MemoPrintDetail.rpt"));//TestXCrystalReport1
            SetLoginInfo(rptDoc);
            //dbp --ZZZ_USP_GET_FirstIn_Lastout--ZZZ_USP_GET_FirstIn_Lastout_Night
            rptDoc.SetParameterValue("@RecordId", recordid);
           // rptDoc.SetParameterValue("@to", dtto);
            //rptDoc.SetParameterValue("@compcode", compcode);
            //rptDoc.SetParameterValue("@gradecode", grade);
            //
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

        public ActionResult Memo_UploadFile(HttpPostedFileBase postedFile, LetterMDI hld)
        {
            if (mc.getPermission(Entry.Memo, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {

                using (SqlCommand cmd = new SqlCommand("Memo_FileInsert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);

                    //cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");

        }
        [HttpGet]
        public ActionResult Memo_UploadRecord(int? id)
        {
            if (mc.getPermission(Entry.Memo, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //int luserid = Convert.ToInt32(objCookie.getUserId());
            MessageModel model = new MessageModel();
            var hl = model.GetAllMailSent().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        ///Email Section
        public void SendmailRecipient(MessageModel model)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("erp@praggroup.com")
            };
            foreach (HttpPostedFileBase attachment in model.files)
            {
                if (attachment != null)
                {
                    string fileName = Path.GetFileName(attachment.FileName);
                    mail.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                }
            }
            MessageModel bll = new MessageModel { };
            mail.To.Add(bll.MemoGetEmail());
            mail.Subject = ("PRAG Memorandam");
            mail.IsBodyHtml = true;
            mail.Body = SendmailRecipienthtml();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", mc.getEmailPassword());
            SmtpServer.EnableSsl = true;
            //int messageSize = 0;
            SmtpServer.Send(mail);
           
            //mail.Attachments.Clear();
            //messageSize = 0;
        }
        public string SendmailRecipienthtml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("<html><head> <b>PRAG ERP Alert - Only For Test Purpose</b><br/>" +
            // " As on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "<br/><br/></head><body>");
            stringBuilder.Append("Dear,<br/>Prag Memorandam<br/>");
            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>MemoNo");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>SubJect");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Message");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>SendTo");
            stringBuilder.Append("</td>");

            //stringBuilder.Append("<td>");
            //stringBuilder.Append("<b>Send By");
            //stringBuilder.Append("</td>");
            MessageModel gg = new MessageModel();
            StringBuilder reportmatter = new StringBuilder();
            var tablst = gg.MemoMailSentDetail();
            for (int i = 0; i < tablst.Count; i++)
            {
                reportmatter.Append("<tr>");
                reportmatter.Append("<tr>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                //  reportmatter.Append("<td valign='top'>" + (i + 1).ToString() + "</td>");//sno
                reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Subject.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Detail.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].FullNameRec.ToString() + "</td>");
               //reportmatter.Append("<td valign='top'>" + tablst[i].FullName.ToString() + "</td>");
                //reportmatter.Append("<td valign='top'>" + tablst[i].CreatedOn.ToString() + "</td>");
                reportmatter.Append("</tr>");
            }
            reportmatter.Append("</table>");
            reportmatter.Append("</br>");
            stringBuilder.Append(reportmatter);
            stringBuilder.Append("<br/>Regards:<br/>For More Detail Please Check on the ERP under Doccument Control->InterOffice Memo");
            stringBuilder.Append("</body></html>");
            return stringBuilder.ToString();
        }
    }
}

      

    


