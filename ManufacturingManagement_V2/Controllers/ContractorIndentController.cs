using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ManufacturingManagement_V2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
namespace ManufacturingManagement_V2.Controllers
{
    public class ContractorIndentController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        //
        readonly AAA_ITAssestCell_BLL ITRegisterobj = new AAA_ITAssestCell_BLL();
        readonly ComplaintBLLL ComplaintObj = new ComplaintBLLL();
        readonly ContractorIndentMDI contobj = new ContractorIndentMDI();
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
       [HttpPost]
        public ActionResult Edit_Status(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorStatusUpdate(objModel);
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
        public ActionResult Edit_Category(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorCategoryUpdate(objModel);
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
        public ActionResult Edit_Priority(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorPriorityUpdate(objModel);
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
        public ActionResult IndentDeleteRecord(int id)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.IndentDelete(id);
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

                return RedirectToAction("Index_ComplaintApprovalHOD_Sec");
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult Index(ContractorIndentMDI model, List<HttpPostedFileBase> files)
        {
            foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                }
            }
            UploadFile(model);
            // SendmailForHOD();
            ViewBag.Message = "Data Save Successfully";
            ModelState.Clear();


            return RedirectToAction("GetReferenceNo");
            // return View();

        }
        public ActionResult GetReferenceNo()
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = contobj.GetReferenceNo();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }
        public void UploadFile(ContractorIndentMDI model)
        {

            // String FileExt = Path.GetExtension(files.FileName).ToUpper();


            //save mail details
            var recordId = SaveMailDetails(model);

            // save attachements
            foreach (HttpPostedFileBase file in model.files)
            {

                if (file != null)
                {
                    Stream str = file.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                    ContractorIndentMDI Fd = new Models.ContractorIndentMDI();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }
        }
        private void SaveFileDetails(ContractorIndentMDI objDet)
        {
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            DbConnection();
            con.Open();
            con.Execute("ContactorUploadFileInsert", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        private string SaveMailDetails(ContractorIndentMDI model)
        {

            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", model.RecordId);
            Parm.Add("@compcode", model.compcode);
            // Parm.Add("@Location", model.Location);
            Parm.Add("@Category", model.Category);
            //Parm.Add("@Remark", model.Remark);
            //Parm.Add("@HODRemark", model.HODRemark);
            //Parm.Add("@FinalRemark", model.FinalRemark);
            //Parm.Add("@Contractor", model.Contractor);
            //Parm.Add("@WorkDetail", model.WorkDetail);

            //Parm.Add("@EstimateAmount", model.EstimateAmount);
            //Parm.Add("@hodid", model.hodid);
            //Parm.Add("@Status", model.Status);
            //Parm.Add("@Priority", model.Priority);
            //Parm.Add("@WorkStartDate", model.WorkStartDate);
            //Parm.Add("@CompletionDate", model.CompletionDate);
            //Parm.Add("@UpdatedBy", objCookie.getUserName());
            Parm.Add("@FinalApprovalBY", objCookie.getUserName());
            DbConnection();
            con.Open();
            con.Execute("Contractor_Store_Sec_Insert", Parm, commandType: CommandType.StoredProcedure);
            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.ContractorIndent, "RecordId");
            return recid;

        }
        public ActionResult Index_GetFile()
        {

            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {

           // int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<ContractorIndentMDI> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }
        [HttpGet]
        public PartialViewResult FileDetails()
        {

            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<ContractorIndentMDI> DetList = GetFileList();

            return PartialView("FileDetails", DetList);


        }
        private List<ContractorIndentMDI> GetFileList()
        {
            // int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<ContractorIndentMDI> DetList = new List<ContractorIndentMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<ContractorIndentMDI>(con, "ContractorGetFileQuatation", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public ActionResult Index_GeIndentNo()
        {
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile1(int id)
        {
            //int compcode = Convert.ToInt32(objCookie.getCompCode());

            List<ContractorIndentMDI> ObjFiles = GetFileList1();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }
        [HttpGet]
        public PartialViewResult FileDetails1()
        {

            int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<ContractorIndentMDI> DetList = GetFileList1();
            return PartialView("FileDetails1", DetList);
        }
        private List<ContractorIndentMDI> GetFileList1()
        {
            List<ContractorIndentMDI> DetList = new List<ContractorIndentMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<ContractorIndentMDI>(con, "[ContractorGetFileDetails1]", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public FileContentResult GetQutation(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            //bool DOC;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id as Id,FileContent,FileName FROM ContractorFileUploadDetails WHERE RecordId = @RecordId";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@RecordId",Id);
                //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                conn.Open();
                rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Read();
                    Id = Convert.ToInt32(rdr["Id"].ToString());
                    fileContent = (byte[])rdr["FileContent"];
                    fileName = rdr["FileName"].ToString();
                    //DOC = Convert.ToBoolean(rdr["DOC"].ToString());
                }
            }
            return File(fileContent, fileName);
        }
      ///Email Section
        public void SendmailForHOD()
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("erp@praggroup.com")
            };
            ContractorIndentMDI bll = new ContractorIndentMDI { };
            mail.To.Add(bll.getEmailAddress());
            mail.Subject = ("PRAG ERP New Task Notification");
            mail.IsBodyHtml = true;
            mail.Body = CreateHtmlBody2();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", mc.getEmailPassword());
            SmtpServer.EnableSsl = true;
            //int messageSize = 0;
            SmtpServer.Send(mail);
            mail.Attachments.Clear();
            //messageSize = 0;
        }
        public string CreateHtmlBody2()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("<html><head> <b>PRAG ERP Alert - Only For Test Purpose</b><br/>" +
            // " As on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "<br/><br/></head><body>");
            stringBuilder.Append("Dear Sir,<br/>New Task Has Been Created  and Pending For Approval with following Details<br/>");
            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Task_Number");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Company");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Category");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Work_Detail");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Created_By");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Created_On");
            stringBuilder.Append("</td>");
            ContractorIndentMDI gg = new ContractorIndentMDI();
            StringBuilder reportmatter = new StringBuilder();
            var tablst = gg.GetHODApprovalForMail();
            for (int i = 0; i < tablst.Count; i++)
            {
                reportmatter.Append("<tr>");
                reportmatter.Append("<tr>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                //  reportmatter.Append("<td valign='top'>" + (i + 1).ToString() + "</td>");//sno
                reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].cmpname.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Category.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].WorkDetail.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].CreatedBy.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].CreatedOn.ToString() + "</td>");
                reportmatter.Append("</tr>");
            }
            reportmatter.Append("</table>");
            reportmatter.Append("</br>");
            stringBuilder.Append(reportmatter);
            stringBuilder.Append("<br/>Regards:<br/>PRAG Enterprise Resource Planning");
            stringBuilder.Append("</body></html>");
            return stringBuilder.ToString();
        }
        //Email Section Other
        public void SendmailForOther()
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("erp@praggroup.com")
            };
            ContractorIndentMDI bll = new ContractorIndentMDI { };
            mail.To.Add(bll.getEmailAddressofOther());
            mail.Subject = ("PRAG ERP Task Assign Notification");
            mail.IsBodyHtml = true;
            mail.Body = CreateHtmlBody3();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", mc.getEmailPassword());
            SmtpServer.EnableSsl = true;
            //int messageSize = 0;
            SmtpServer.Send(mail);
            mail.Attachments.Clear();
            //messageSize = 0;
        }
        public string CreateHtmlBody3()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("<html><head> <b>PRAG ERP Alert - Only For Test Purpose</b><br/>" +
            // " As on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "<br/><br/></head><body>");
            stringBuilder.Append("Dear,<br/>New Task Has Been Assign  and Pending  From Management Approval/Rejection with following Details<br/>");
            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Task_Number");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Company");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Category");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Work_Detail");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>HOD_Status");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Management_Status");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Assign_To");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Assign_On");
            stringBuilder.Append("</td>");
            ContractorIndentMDI gg = new ContractorIndentMDI();
            StringBuilder reportmatter = new StringBuilder();
            var tablst = gg.GetHODApprovalForMail_Other();
            for (int i = 0; i < tablst.Count; i++)
            {
                reportmatter.Append("<tr>");
                reportmatter.Append("<tr>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                //  reportmatter.Append("<td valign='top'>" + (i + 1).ToString() + "</td>");//sno
                reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].cmpname.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Category.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].WorkDetail.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].HODApproval.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].FinalApproval.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].FullName.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].HODApprovalDate.ToString() + "</td>");
                reportmatter.Append("</tr>");
            }
            reportmatter.Append("</table>");
            reportmatter.Append("</br>");
            stringBuilder.Append(reportmatter);
            stringBuilder.Append("<br/>Regards:<br/>PRAG Enterprise Resource Planning");
            stringBuilder.Append("</body></html>");
            return stringBuilder.ToString();
        }

        //Email Section StatusChanged
        public void SendmailForStatusChanged()
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("erp@praggroup.com")
            };
            ContractorIndentMDI bll = new ContractorIndentMDI { };
            mail.To.Add(bll.getEmailAddressofStatusChanged());
            mail.Subject = ("PRAG ERP Task Status  Notification");
            mail.IsBodyHtml = true;
            mail.Body = CreateHtmlBody4();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", mc.getEmailPassword());
            SmtpServer.EnableSsl = true;
            //int messageSize = 0;
            SmtpServer.Send(mail);
            mail.Attachments.Clear();
            //messageSize = 0;
        }
        public string CreateHtmlBody4()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("<html><head> <b>PRAG ERP Alert - Only For Test Purpose</b><br/>" +
            // " As on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "<br/><br/></head><body>");
            stringBuilder.Append("Dear,<br/>Task Status Has been  Updated  with following Details<br/>");
            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Task_Number");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Company");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>HOD Name");
            stringBuilder.Append("</td>");


            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Category");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Work_Detail");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Status");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Updated_By");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Updated_On");
            stringBuilder.Append("</td>");
                       
            ContractorIndentMDI gg = new ContractorIndentMDI();
            StringBuilder reportmatter = new StringBuilder();
            var tablst = gg.GetEmailChanedStatus();
            for (int i = 0; i < tablst.Count; i++)
            {
                reportmatter.Append("<tr>");
                reportmatter.Append("<tr>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                // reportmatter.Append("<td valign='top'>" + (i + 1).ToString() + "</td>");//sno
                reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].cmpname.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].hodname.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Category.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].WorkDetail.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Status.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].UpdatedBy.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].UpdatedOn.ToString() + "</td>");
               // reportmatter.Append("<td valign='top'>" + tablst[i].HODApprovalDate.ToString() + "</td>");
                reportmatter.Append("</tr>");
            }
            reportmatter.Append("</table>");
            reportmatter.Append("</br>");
            stringBuilder.Append(reportmatter);
            stringBuilder.Append("<br/>Regards:<br/>PRAG Enterprise Resource Planning");
            stringBuilder.Append("</body></html>");
            return stringBuilder.ToString();
        }

        //Email Section RatingChanged
        public void SendmailForRatingChanged()
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("erp@praggroup.com")
            };
            ContractorIndentMDI bll = new ContractorIndentMDI { };
            mail.To.Add(bll.getEmailAddressofRatingChanged());
            mail.Subject = ("PRAG ERP Task Work Rating Notification");
            mail.IsBodyHtml = true;
            mail.Body = CreateHtmlBody5();
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", mc.getEmailPassword());
            SmtpServer.EnableSsl = true;
            //int messageSize = 0;
            SmtpServer.Send(mail);
            mail.Attachments.Clear();
            //messageSize = 0;
        }
        public string CreateHtmlBody5()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.Append("<html><head> <b>PRAG ERP Alert - Only For Test Purpose</b><br/>" +
            // " As on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + "<br/><br/></head><body>");
            stringBuilder.Append("Dear,<br/>Task Work Rating Has been  Updated  with following Details<br/>");
            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Task_Number");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Company");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Assign_To");
            stringBuilder.Append("</td>");


            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Category");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Work_Detail");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Status");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Work_Rating");
            stringBuilder.Append("</td>");

            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Updated_By");
            stringBuilder.Append("</td>");


            stringBuilder.Append("<td>");
            stringBuilder.Append("<b>Updated_On");
            stringBuilder.Append("</td>");

            ContractorIndentMDI gg = new ContractorIndentMDI();
            StringBuilder reportmatter = new StringBuilder();
            var tablst = gg.GetEmailChanedRating();
            for (int i = 0; i < tablst.Count; i++)
            {
                reportmatter.Append("<tr>");
                reportmatter.Append("<tr>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                //  reportmatter.Append("<td valign='top'>" + (i + 1).ToString() + "</td>");//sno
                reportmatter.Append("<td valign='top'>" + tablst[i].RecordId.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].cmpname.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].FullName.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Category.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].WorkDetail.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].Status.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].WorkRating.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].UpdatedBy.ToString() + "</td>");
                reportmatter.Append("<td valign='top'>" + tablst[i].UpdatedOn.ToString() + "</td>");
                // reportmatter.Append("<td valign='top'>" + tablst[i].HODApprovalDate.ToString() + "</td>");
                reportmatter.Append("</tr>");
            }
            reportmatter.Append("</table>");
            reportmatter.Append("</br>");
            stringBuilder.Append(reportmatter);
            stringBuilder.Append("<br/>Regards:<br/>PRAG Enterprise Resource Planning");
            stringBuilder.Append("</body></html>");
            return stringBuilder.ToString();
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
            return RedirectToAction("Index_ComplaintInsert");
        }
        //Complaint User 
        [HttpGet]
        public ActionResult Index_ComplaintInsert()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int userid = Convert.ToInt32(objCookie.getUserId());
            //if (mc.getPermission(Entry.Civil, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            setViewData();
            ContractorIndentMDI model = new ContractorIndentMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.HodList = new SelectList(ComplaintObj.CoplaintHODListByComapny(), "hodid", "hodname");
            model.Location = " ";
            model.WorkDetail = " ";
            //model.Priority = " ";
            //model.Status = " ";
            model.Remark = " ";
            model.Item_List = model.GetAllCivilUser_Section(userid);
            //HODApproval
            ContractorIndentMDI modelObject = new ContractorIndentMDI { };
            ContractorIndentMDI List = new ContractorIndentMDI();
            modelObject.Item_List = List.GetHOD_SectionByCompany(compcode);
            ViewBag.HODApproval = modelObject.Item_List.Count.ToString();

            //StoreApproval
            ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            ContractorIndentMDI List1 = new ContractorIndentMDI();
            modelObject1.Item_List = List1.GetAllStore_Section();
            ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();


            //UserSecApproval
            ContractorIndentMDI modelObjectE = new ContractorIndentMDI { };
            ContractorIndentMDI ListE = new ContractorIndentMDI();
            modelObjectE.Item_List = ListE.GetAllCivilUser_Section(userid);
            ViewBag.UserSec = modelObjectE.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjectc = new ContractorIndentMDI { };
            ContractorIndentMDI Listc = new ContractorIndentMDI();
            modelObjectc.Item_List = Listc.GetAllCivil_Section();
            ViewBag.CivilSec = modelObjectc.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjecte = new ContractorIndentMDI { };
            ContractorIndentMDI Liste = new ContractorIndentMDI();
            modelObjecte.Item_List = Liste.GetAllElectrical_Section();
            ViewBag.Electrical = modelObjecte.Item_List.Count.ToString();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index_ComplaintInsert(ContractorIndentMDI empmodel)
        {
            //if (mc.getPermission(Entry.Civil, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            setViewData();
            try
            {
                int result = contobj.Contractor_Complaint_Insert(empmodel);
                SendmailForHOD();
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

                return RedirectToAction("GetReferenceNo");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult Update_User_ComplaintRecord1(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivilUser_Section(userid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Update_User_ComplaintRecord(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivilUser_Section(userid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_FirstStep(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            
            try
            {
                int result = model.Contractor_Complaint_UpdatetFirstStep(objModel);
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

                return RedirectToAction("Index_ComplaintInsert");
            }
            catch
            {
                throw;
            }
        }

        //Complaint HOD Step
        [HttpGet]
        public ActionResult ViewHodSecRecord(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivilUser_Section(userid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult updateHodApprovalRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetHOD_SectionByCompany(compcode).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Index_ComplaintApprovalHOD_Sec()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int userid = Convert.ToInt32(objCookie.getUserId());
         
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ContractorIndentMDI model = new ContractorIndentMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.HodList = new SelectList(ComplaintObj.CoplaintHODListByComapny(), "hodid", "hodname");
            // model.Location = " ";
            //model.WorkDetail = " ";
            model.HODRemark = " ";
            model.Item_List = model.GetHOD_SectionByCompany(compcode);
            // ViewBag.HODApproval = model.Item_List.Count.ToString();

            //HODApproval
            ContractorIndentMDI modelObjectt = new ContractorIndentMDI { };
            ContractorIndentMDI Listt = new ContractorIndentMDI();
            modelObjectt.Item_List = Listt.GetHOD_SectionByCompany(compcode);
            ViewBag.HODApproval = modelObjectt.Item_List.Count.ToString();

            //StoreApproval
            ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            ContractorIndentMDI List1 = new ContractorIndentMDI();
            modelObject1.Item_List = List1.GetAllStore_Section();
            ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();


            //UserSecApproval
            ContractorIndentMDI modelObjectE = new ContractorIndentMDI { };
            ContractorIndentMDI ListE = new ContractorIndentMDI();
            modelObjectE.Item_List = ListE.GetAllCivilUser_Section(userid);
            ViewBag.UserSec = modelObjectE.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjectc = new ContractorIndentMDI { };
            ContractorIndentMDI Listc = new ContractorIndentMDI();
            modelObjectc.Item_List = Listc.GetAllCivil_Section();
            ViewBag.CivilSec = modelObjectc.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjecte = new ContractorIndentMDI { };
            ContractorIndentMDI Liste = new ContractorIndentMDI();
            modelObjecte.Item_List = Liste.GetAllElectrical_Section();
            ViewBag.Electrical = modelObjecte.Item_List.Count.ToString();
            return View(model);
        
        }
       
       [HttpPost]
        public ActionResult Index_ComplaintApprovalHOD_Sec(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorApprovalHOD(objModel);
                SendmailForOther();

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

                return RedirectToAction("Index_ComplaintApprovalHOD_Sec");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult Update_Rating_Record(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetHOD_SectionByCompany().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update_Rating(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorWorkRatingUpdate(objModel);
                SendmailForRatingChanged();
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

                return RedirectToAction("Index_ComplaintApprovalHOD_Sec");
            }
            catch
            {
                throw;
            }
        }

        //Store Section
        [HttpGet]
        public ActionResult Index_ComplaintStore_Sec()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int userid = Convert.ToInt32(objCookie.getUserId());
            if (mc.getPermission(Entry.CivilIndentFinal, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ContractorIndentMDI model = new ContractorIndentMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.HodList = new SelectList(ComplaintObj.CoplaintHODListByComapny(), "hodid", "hodname");

            model.FinalRemark = " ";
            model.Contractor = "Self";
            model.Item_List = model.GetAllStore_Section();
           // ViewBag.FinalApproval=model.Item_List.Count.ToString();

            //UserSection
            ContractorIndentMDI modelObjectu = new ContractorIndentMDI { };
            ContractorIndentMDI Listu = new ContractorIndentMDI();
            modelObjectu.Item_List = Listu.GetAllCivilUser_Section(userid);
            ViewBag.UserSec = modelObjectu.Item_List.Count.ToString();
            //HODApproval
            ContractorIndentMDI modelObject = new ContractorIndentMDI { };
            ContractorIndentMDI List = new ContractorIndentMDI();
            modelObject.Item_List = List.GetHOD_SectionByCompany(compcode);
            ViewBag.HODApproval = modelObject.Item_List.Count.ToString();

            //StoreApproval
            ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            ContractorIndentMDI List1 = new ContractorIndentMDI();
            modelObject1.Item_List = List1.GetAllStore_Section();
            ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();

            //Civil
            ContractorIndentMDI modelObjectE = new ContractorIndentMDI { };
            ContractorIndentMDI ListE = new ContractorIndentMDI();
            modelObjectE.Item_List = ListE.GetAllCivil_Section();
            ViewBag.CivilSec = modelObjectE.Item_List.Count.ToString();

            //ElectricalApproval
            ContractorIndentMDI modelObjectc = new ContractorIndentMDI { };
            ContractorIndentMDI Listc = new ContractorIndentMDI();
            modelObjectc.Item_List = Listc.GetAllElectrical_Section();
            ViewBag.Electrical = modelObjectc.Item_List.Count.ToString();


            return View(model);
          
        }
        [HttpGet]
        public ActionResult Update_ComplaintStore_SecRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllStore_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index_ComplaintStore_Sec(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentFinal, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorStoreApproval(objModel);
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

                return RedirectToAction("Index_ComplaintStore_Sec");
            }
            catch
            {
                throw;
            }

        }
        public ActionResult Update_Store_ComplaintStatus(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllStore_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update_Store_ComplaintStatus(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.CivilIndentHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorUpdateStatus(objModel);
                SendmailForStatusChanged();
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

                return RedirectToAction("Index_ComplaintStore_Sec");
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult QuatationUploadFile(HttpPostedFileBase postedFile, ContractorIndentMDI hld)
        {
            if (mc.getPermission(Entry.CivilIndentFinal, permissionType.Edit) == false)
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
                // string query = "INSERT INTO ContractorFileUploadDetails VALUES (@FileName,@FileContent,@RecordId)";
                using (SqlCommand cmd = new SqlCommand("Contractor_Quataion_Upload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@Doc", hld.Doc);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                   }
            }
            // string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(constr))
            //{
            //    string query = "INSERT INTO ContractorFileUploadDetails VALUES (@FileName,@FileContent,@RecordId)";
            //    using (SqlCommand cmd = new SqlCommand(query))
            //    {
            //        cmd.Connection = con;
            //        cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
            //        //cmd.Parameters.AddWithValue("@FileContent", postedFile.ContentType);
            //        cmd.Parameters.AddWithValue("@FileContent", bytes);
            //        cmd.Parameters.AddWithValue("@RecordId", model.RecordId);
            //        con.Open();
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //    }
            //}
            return RedirectToAction("Index_ComplaintInsert");
           // return View(model);
        }
        //CivilSection
        [HttpGet]
        public ActionResult CivilUpdateStatusRecord1(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivil_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Index_ComplaintCivil_Sec()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int userid = Convert.ToInt32(objCookie.getUserId());
            if (mc.getPermission(Entry.Civil, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ContractorIndentMDI model = new ContractorIndentMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.HodList = new SelectList(ComplaintObj.CoplaintHODListByComapny(), "hodid", "hodname");

            model.FinalRemark = " ";
            model.Contractor = "Self";
            model.Item_List = model.GetAllCivil_Section();
            // ViewBag.CivilSec = model.Item_List.Count.ToString();

            //HODApproval
            ContractorIndentMDI modelObject = new ContractorIndentMDI { };
            ContractorIndentMDI List = new ContractorIndentMDI();
            modelObject.Item_List = List.GetHOD_SectionByCompany(compcode);
            ViewBag.HODApproval = modelObject.Item_List.Count.ToString();

            //StoreApproval
            ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            ContractorIndentMDI List1 = new ContractorIndentMDI();
            modelObject1.Item_List = List1.GetAllStore_Section();
            ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();


            //UserSecApproval
            ContractorIndentMDI modelObjectE = new ContractorIndentMDI { };
            ContractorIndentMDI ListE = new ContractorIndentMDI();
            modelObjectE.Item_List = ListE.GetAllCivilUser_Section(userid);
            ViewBag.UserSec = modelObjectE.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjectc = new ContractorIndentMDI { };
            ContractorIndentMDI Listc = new ContractorIndentMDI();
            modelObjectc.Item_List = Listc.GetAllCivil_Section();
            ViewBag.CivilSec = modelObjectc.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjecte = new ContractorIndentMDI { };
            ContractorIndentMDI Liste = new ContractorIndentMDI();
            modelObjecte.Item_List = Liste.GetAllElectrical_Section();
            ViewBag.Electrical = modelObjecte.Item_List.Count.ToString();
            return View(model);
        }
        [HttpGet]
        public ActionResult Index_ComplaintCivil_Sec_Record(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivil_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Index_ComplaintCivil_Sec_View(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivil_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index_ComplaintCivil_Sec(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.Civil, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorStoreApproval(objModel);
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

                return RedirectToAction("Index_ComplaintCivil_Sec");
            }
            catch
            {
                throw;
            }

        }
        [HttpPost]
        public ActionResult Update_Civil_ComplaintStatus(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.Civil, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorUpdateStatus(objModel);
                SendmailForStatusChanged();
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

                return RedirectToAction("Index_ComplaintCivil_Sec");
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult QuatationUploadCivilFile(HttpPostedFileBase postedFile, ContractorIndentMDI hld)
        {
            if (mc.getPermission(Entry.Civil, permissionType.Edit) == false)
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
                using (SqlCommand cmd = new SqlCommand("Contractor_Quataion_Upload", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                   //  conn.Close();
                }
            }
            //using (SqlConnection con = new SqlConnection(constr))
            //{
            //    string query = "INSERT INTO ContractorFileUploadDetails VALUES (@FileName,@FileContent,@RecordId)";
            //    using (SqlCommand cmd = new SqlCommand(query))
            //    {
            //        cmd.Connection = con;
            //        cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
            //        //cmd.Parameters.AddWithValue("@FileContent", postedFile.ContentType);
            //        cmd.Parameters.AddWithValue("@FileContent", bytes);
            //        cmd.Parameters.AddWithValue("@RecordId", model.RecordId);

            //        con.Open();
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //    }
            //}
            return RedirectToAction("Index_ComplaintCivil_Sec");
            // return View(model);
        }
        [HttpGet]
        public ActionResult QuatationUploadCivilFile_Record(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllCivil_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        //Electrical Section
        public ActionResult Index_ComplaintElectrical_Sec()
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            int userid = Convert.ToInt32(objCookie.getUserId());

            if (mc.getPermission(Entry.Coplaint, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ContractorIndentMDI model = new ContractorIndentMDI();
            ViewBag.LocationList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.HodList = new SelectList(ComplaintObj.CoplaintHODListByComapny(), "hodid", "hodname");

            model.FinalRemark = " ";
            model.Contractor = "Self";
            model.Item_List = model.GetAllElectrical_Section();
   
            //HODApproval
            ContractorIndentMDI modelObject = new ContractorIndentMDI { };
            ContractorIndentMDI List = new ContractorIndentMDI();
            modelObject.Item_List = List.GetHOD_SectionByCompany(compcode);
            ViewBag.HODApproval = modelObject.Item_List.Count.ToString();
           
            //StoreApproval
            ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            ContractorIndentMDI List1 = new ContractorIndentMDI();
            modelObject1.Item_List = List1.GetAllStore_Section();
            ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();


            //UserSecApproval
            ContractorIndentMDI modelObjectE = new ContractorIndentMDI { };
            ContractorIndentMDI ListE = new ContractorIndentMDI();
            modelObjectE.Item_List = ListE.GetAllCivilUser_Section(userid);
            ViewBag.UserSec = modelObjectE.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjectc = new ContractorIndentMDI { };
            ContractorIndentMDI Listc = new ContractorIndentMDI();
            modelObjectc.Item_List = Listc.GetAllCivil_Section();
            ViewBag.CivilSec = modelObjectc.Item_List.Count.ToString();

            //CivilApproval
            ContractorIndentMDI modelObjecte = new ContractorIndentMDI { };
            ContractorIndentMDI Liste = new ContractorIndentMDI();
            modelObjecte.Item_List = Liste.GetAllElectrical_Section();
            ViewBag.Electrical = modelObjecte.Item_List.Count.ToString();


            return View(model);

        }
        [HttpGet]
        public ActionResult Index_ComplaintElectrical_SecRecord(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllElectrical_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index_ComplaintElectrical_Sec(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.Coplaint, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorStoreApproval(objModel);
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

                return RedirectToAction("Index_ComplaintElectrical_Sec");
            }
            catch
            {
                throw;
            }

        }
        [HttpPost]
        public ActionResult Update_Electrical_ComplaintStatus(ContractorIndentMDI objModel)
        {
            ContractorIndentMDI model = new ContractorIndentMDI();
            if (mc.getPermission(Entry.Coplaint, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.ContractorUpdateStatus(objModel);
                SendmailForStatusChanged();
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

                return RedirectToAction("Index_ComplaintElectrical_Sec");
            }
            catch
            {
                throw;
            }
        }

        public ActionResult ElectricalUpdateStatusRecord1(int? id)
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllElectrical_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult QuatationUploadElectricalFile(HttpPostedFileBase postedFile, ContractorIndentMDI hld)
        {
            if (mc.getPermission(Entry.Coplaint, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
           //string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                // string query = "INSERT INTO ContractorFileUploadDetails VALUES (@FileName,@FileContent,@RecordId)";
                using (SqlCommand cmd = new SqlCommand("Contractor_Quataion_Upload",conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                   cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent",bytes);
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                   //cmd.Parameters.AddWithValue("@Doc", hld.Doc);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                   // con.Close();
                }
            }
            return RedirectToAction("Index_ComplaintElectrical_Sec");
            // return View(model);
        }
        [HttpGet]
        public ActionResult FileStore_Electrical_ComplaintRecord(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            ContractorIndentMDI model = new ContractorIndentMDI();
            var hl = model.GetAllElectrical_Section().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
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
