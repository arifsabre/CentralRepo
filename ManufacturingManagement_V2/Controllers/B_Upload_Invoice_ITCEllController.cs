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

namespace ManufacturingManagement_V2.Controllers
{
    public class B_Upload_Invoice_ITCEllController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        //
        // GET: /B_Upload_Invoice_ITCEll/
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();

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
        public ActionResult Index()
        {
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            ModelState.Clear();
            model.Invoice_No = "NA";
            model.OrderNo = "NA";
            model.ItemQty =1;
            model.ItemDescription ="NA";
            model.BillNo = "NA";
       
           return View(model);
 }
 [HttpPost]
        public ActionResult Index(AAA_ITAssestCell_MDI model, List<HttpPostedFileBase> attachments)
        {
            model.Status = true;
            
                foreach (HttpPostedFileBase attachment in model.files)
                {
                    if (attachment != null)
                    {
                        string fileName = Path.GetFileName(attachment.FileName);
                       // mm.Attachments.Add(new Attachment(attachment.InputStream, fileName));
                    }
                }
            UploadFile(model);
            ViewBag.Message = "Data Save Successfully";
            ModelState.Clear();
            return View();
        }
        public void UploadFile(AAA_ITAssestCell_MDI model)
        {

            //String FileExt = Path.GetExtension(model.files.FileName).ToUpper(); 

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

                    AAA_ITAssestCell_MDI Fd = new Models.AAA_ITAssestCell_MDI();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }

        }

        private void SaveFileDetails(AAA_ITAssestCell_MDI objDet)
        {

            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            DbConnection();
            con.Open();
            con.Execute("AddUploadDetails", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();

        }
    private string SaveMailDetails(AAA_ITAssestCell_MDI model)
        {

            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@Invoice_No", model.Invoice_No);
            Parm.Add("@OrderNo", model.OrderNo);
            Parm.Add("@ItemQty", model.ItemQty);
            Parm.Add("@ItemDescription", model.ItemDescription);
            Parm.Add("@Remark", model.Remark);
            Parm.Add("@BillNo", model.BillNo);
            //Parm.Add("@Status", model.Status == true ? 1 : 0);
            DbConnection();
            con.Open();
            con.Execute("AAA_Sp_AddInvoice", Parm, commandType: CommandType.StoredProcedure);

            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.AAA_InvoiceSave_Detail_ITCell, "RecordId");
            return recid;
        }



public ActionResult Index_GetFile()  
            {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();  
            } 





[HttpGet]  
            public FileResult DownLoadFile(int id)  
            {  
      
      
                List<AAA_ITAssestCell_MDI> ObjFiles = GetFileList();  
      
                var FileById = (from FC in ObjFiles  
                                where FC.Id.Equals(id)  
                                select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();  
      
                return File(FileById.FileContent, "application/pdf", FileById.FileName);  
      
            }  
           
     
           
            [HttpGet]  
            public PartialViewResult FileDetails()  
            {  
                List<AAA_ITAssestCell_MDI> DetList = GetFileList();  
      
                return PartialView("FileDetails", DetList);  
      
      
            }  
            private List<AAA_ITAssestCell_MDI> GetFileList()  
            {  
                List<AAA_ITAssestCell_MDI> DetList = new List<AAA_ITAssestCell_MDI>();  
      
                DbConnection();  
                con.Open();
                DetList = SqlMapper.Query<AAA_ITAssestCell_MDI>(con, "ZZZZ_GetFileDetails", commandType: CommandType.StoredProcedure).ToList();  
                con.Close();  
                return DetList;  
            }  

   }
}
