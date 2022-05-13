using Dapper;
using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Controllers
{
    public class LetterController : Controller
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
        readonly LetterMDI letterobj = new LetterMDI();
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
        [HttpGet]
        public ActionResult Index(int id=0)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
           // int userid = Convert.ToInt32(objCookie.getUserId());
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            string finyear = Convert.ToString(objCookie.getFinYear());
            string finyear1 = Convert.ToString(objCookie.getFinYear());
            LetterMDI model = new LetterMDI();
            setViewData();
      
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.GetAllFileNameList = new SelectList(letterobj.GetAllFileNameListToRepalce(), "Id", "FileName");
            ViewBag.Department = new SelectList(letterobj.getDepartmentList(), "DepId", "DepName");
            model.Item_List = model.GetAllLetterListByUserId(compcode,finyear1);
            model.Item_ListRef = model.GetCompanyCount(compcode,finyear1);
            ViewBag.TotalLetter = model.Item_List.Count;
            model.RecordId = id;
            model.Item_List1 = model.GetAllLetterListByRecordId(id);
            return View(model);
        }

        [HttpGet]
        public ActionResult IndexGetAllCompany(int id=0)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int userid = Convert.ToInt32(objCookie.getUserId());
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            string finyear = Convert.ToString(objCookie.getFinYear());
            string finyear1 = Convert.ToString(objCookie.getFinYear());
            LetterMDI model = new LetterMDI();
            setViewData();

            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.GetAllFileNameList = new SelectList(letterobj.GetAllFileNameListToRepalce(), "Id", "FileName");
            ViewBag.Department = new SelectList(letterobj.getDepartmentList(), "DepId", "DepName");
            model.Item_List = model.GetAllLetterListByUserId(compcode, finyear1);
            model.Item_ListRef = model.GetAllCompanyLetter(compcode);
            ViewBag.TotalLetter = model.Item_List.Count;
            model.RecordId = id;
            model.Item_List1 = model.GetAllLetterListByRecordId(id);
            return View(model);
        }
        [HttpGet]
        public PartialViewResult GetFileDetails()
        {

           int userid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            setViewData();
            model.Item_List2 = model.GetAllLetterListByCompanyPI(userid);
            return PartialView("_Letter", model);
        }
      //ListAllFile
        //[HttpGet]
        //public ActionResult IndexListAllFile()
        //{
        //    if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
        //    {
        //        return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
        //    }
        //    setViewData();
        //    LetterMDI model = new LetterMDI();
        //     setViewData();
        
        //    model.Item_List2 = model.GetAllLetterList();
          
        //    return View(model);
        //}
        //Ajax controle
        // Return all students
        //[HttpGet]
        //public PartialViewResult All()
        //{
        //    LetterMDI model = new LetterMDI();
        //    // model1.Item_List2 = model1.GetAllLetterList();
        //    if (mc.getPermission(Entry.MasterFileDownload, permissionType.Add) == false)
        //    {
        //        // Redirecttoaction("ff");  // return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
        //        //return RedirectToAction("Index");
        //        return PartialView("_edd");
        //    }
        //    else
        //    {
        //        // LetterMDI model = new LetterMDI();
        //        setViewData();

        //        model.Item_List2 = model.GetAllLetterList();
        //        ViewBag.TotalLetter1 = model.Item_List2.Count.ToString();
        //        return PartialView("_Letter", model);
        //    }
        //}
       //public ActionResult ff()
       // {
       //     // ViewBag.Msg = ViewBag.Message; // Assigned value : "Hi, Dot Net Tricks"
       //     return View();
       // }
       [HttpPost]
        public ActionResult Index(LetterMDI model, List<HttpPostedFileBase> files)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                }
            }
            UploadFile(model);

            ViewBag.Message = "Data Save Successfully";
            ModelState.Clear();
            return RedirectToAction("Index");
            // return View();

        }
        [HttpGet]
        public ActionResult Edit_LetterRecord(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int userid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public ActionResult Edit_LetterRecord1(int? id)
        //{
        //    if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
        //    {
        //        return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
        //    }
        //    setViewData();
        //    int Id;
        //    Id= 0;
        //    int userid = Convert.ToInt32(objCookie.getUserId());
        //    LetterMDI model = new LetterMDI();
        //    var hl = model.GetAllLetterListByRecordId(Id).Find(x => x.RecordId.Equals(id));
        //    return Json(hl, JsonRequestBehavior.AllowGet);
        //}
        //get details
        [HttpGet]
        public ActionResult Details(int id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //int userid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            setViewData();
            model.Item_List1 = model.GetAllLetterListByRecordId(id);
            //return PartialView("_Details", model);
            return View(model);
    
        }
        [HttpPost]
        public ActionResult Letter_UploadOtherFile(HttpPostedFileBase postedFile, LetterMDI hld)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
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

                using (SqlCommand cmd = new SqlCommand("LetterFile_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
           
        }
        [HttpGet]
        public ActionResult Letter_UploadOtherRecord(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //int luserid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Letter_UpdateFileName(HttpPostedFileBase postedFile, LetterMDI hld)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
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

                using (SqlCommand cmd = new SqlCommand("LetterFile_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", hld.Id);
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                   
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Edit_Letter(LetterMDI objModel)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterUpdate(objModel);

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
               // return View();
                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
      //UpdateCompany
        [HttpGet]
        public ActionResult Edit_CompanyRecord(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_Company(LetterMDI objModel)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterCompanyUpdate(objModel);
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
        //DateUpdate
        //UpdateCompany
        [HttpGet]
        public ActionResult Edit_SenddateRecord(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_SendDate(LetterMDI objModel)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterSentUpdate(objModel);
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
        public ActionResult LetterDeleteRecord(int id)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterDelete(id);
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
                //return RedirectToAction("Details", new { id = 0 });
                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        public ActionResult LetterDeleteFle(int id)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterDeleteoOneFile(id);
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

               // return RedirectToAction("Details", new { id = 0 });
                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
        public void UploadFile(LetterMDI model)
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
                    LetterMDI Fd = new Models.LetterMDI();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }
        }
        private void SaveFileDetails(LetterMDI objDet)
        {
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            Parm.Add("@CreatedOn", DateTime.Now);
            DbConnection();
            con.Open();
            con.Execute("LetterFile_Insert", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        private string SaveMailDetails(LetterMDI model)
        {
            int id = Convert.ToInt32(objCookie.getCompCode());
            string fin = Convert.ToString(objCookie.getFinYear());
            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            
            Parm.Add("@CompCode",objCookie.getCompCode());
            Parm.Add("@FinYear", objCookie.getFinYear());
            Parm.Add("@DepId", model.DepId);
            Parm.Add("@LetterNo", model.LetterNo);
           
           //Parm.Add("@ReferenceNo",model.ReferenceNo);
            Parm.Add("@Subject", model.Subject);
            Parm.Add("@SendTo", model.SendTo);
            Parm.Add("@SendDate", model.SendDate);
            Parm.Add("@SendDate", model.SendDate);
            Parm.Add("@Keywords", model.Keywords);

            Parm.Add("@SendVia", model.SendVia);
            Parm.Add("@TrackNo", model.TrackNo);
            Parm.Add("@TrackNoPost", model.TrackNoPost);
            Parm.Add("@TrackNoHand", model.TrackNoHand);
            Parm.Add("@CreatedBy", objCookie.getUserName());
            Parm.Add("@UserId", objCookie.getUserId());
            Parm.Add("@Email", model.Email);
            Parm.Add("@Post", model.Post);
            Parm.Add("@ByHand", model.ByHand);
            Parm.Add("@LetterDate", model.LetterDate);
            Parm.Add("@Remark", model.Remark);

            DbConnection();
            con.Open();
            con.Execute("Letter_Insert", Parm, commandType: CommandType.StoredProcedure);

            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.Letter, "RecordId");
            return recid;
        }
        public ActionResult Index_GetFile()
        {

            if (mc.getPermission(Entry.MasterFileDownload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
           
            // int userid = Convert.ToInt32(objCookie.getUserId());
            List<LetterMDI> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }
        [HttpGet]
        public PartialViewResult FileDetails()
        {

            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            List<LetterMDI> DetList = GetFileList();

            return PartialView("FileDetails", DetList);


        }
        private List<LetterMDI> GetFileList()
        {
            // int userid = Convert.ToInt32(objCookie.getUserId());
            List<LetterMDI> DetList = new List<LetterMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<LetterMDI>(con, "Letter_GetAllByFileIdDownload", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public ActionResult Index_GeIndentNo()
        {
            return View();
        }
         public FileContentResult GetFileView(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM LetterFile WHERE Id = @ID";
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
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult RecordDetail(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int userid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecordDetailArchive(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int userid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllCompanyLetter().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit_LetterDateRecord(int? id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LetterMDI model = new LetterMDI();
            var hl = model.GetAllLetterListByUserId().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_LetterDate(LetterMDI objModel)
        {
            LetterMDI model = new LetterMDI();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.LetterDateUpdate(objModel);
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

    }
}