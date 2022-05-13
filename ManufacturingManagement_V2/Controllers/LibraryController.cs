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
    public class LibraryController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        // GET: Library
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        readonly LibFileUploadBLL Lists = new LibFileUploadBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }


        public ActionResult Indexcategory()
        {
            if (mc.getPermission(Entry.Library, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            int luserid = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadMDI model1 = new LibFileUploadMDI();
            LibFileUploadBLL Lists = new LibFileUploadBLL();
            LibBLL List1 = new LibBLL();

            setViewData();
            ModelState.Clear();
            model1.Item_List = Lists.LibGetAllFileDetail();
            ViewBag.total = model1.Item_List.Count;
            //NotStarted
            List<LibTask> modelObject = new List<LibTask> { };
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

            //GetAllNotification
            List<LibTask> modelObjectTotalNotification = new List<LibTask> { };
            LibBLL List5 = new LibBLL();
            modelObjectTotalNotification = List5.GetAllNotificationByUserId(luserid);
            ViewBag.totalNotification = modelObjectTotalNotification.Count.ToString();

            //GetAllNotification
            List<LibTask> modelObjectAllNotification = new List<LibTask> { };
            LibBLL List6 = new LibBLL();
            modelObjectAllNotification = List5.GetAllNotification();
            @ViewBag.AllNotice = modelObjectAllNotification.Count.ToString();


            int luserid1 = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadMDI model = new LibFileUploadMDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            // model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory", "LibSubCategory", "LibSubCategoryId");
            ModelState.Clear();

            model.Item_List = Lists.LibGetAllFileDetail();
            ViewBag.total = model.Item_List.Count;
            return View(model);
        }
        [HttpPost]
        public JsonResult AjaxMethod(string type, int value)
        {
            LibFileUploadMDI model = new LibFileUploadMDI();
            switch (type)
            {
                case "LibCategoryId":
                    model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory WHERE LibCategoryId = " + value, "LibSubCategory", "LibSubCategoryId");
                    break;


            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult Indexcategory(int countryId)
        {
            LibFileUploadMDI model = new LibFileUploadMDI();
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory WHERE LibCategoryId=" + countryId, "LibSubCategory", "LibSubCategoryId");

            //model.AAA_Item_StockITCell = PopulateDropDown("SELECT Item_Id, Stock FROM AAA_Item_StockITCell  WHERE Item_Id = " + itemid, "Stock", "Item_Id");

            //model.AAA_Item_StockITCell = PopulateDropDown("SELECT Item_Id, Serial_No FROM AAA_Item_StockITCell  WHERE Item_Id = " + stateId, "Serial_No", "Item_Id");
            return View(model);
        }

        [HttpPost]
        public ActionResult Indexcategory1(LibFileUploadMDI model, List<HttpPostedFileBase> files)
        {
            //model.Status = true;
            // String FileExt = Path.GetExtension(files.FileName).ToUpper();
            foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null && file.ContentLength > 0)
                {

                    string fileName = Path.GetFileName(file.FileName);

                    //ViewBag.Message = "File already exists! Replacement Denied! File Name: " + file.FileName; //mm.Attachments.Add(new Attachment(attachment.InputStream, fileName));

                }
            }
            UploadFile(model);
            ViewBag.Message = "Data Save Successfully";
            ModelState.Clear();
            return RedirectToAction("Index_GetAllFile");
            //return View();

        }
        public void UploadFile(LibFileUploadMDI model)
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

                    LibFileUploadMDI Fd = new Models.LibFileUploadMDI();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }

        }

        private void SaveFileDetails(LibFileUploadMDI objDet)
        {

            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            DbConnection();
            con.Open();
            con.Execute("LibraryUploadRecordInsert", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();

        }
        private string SaveMailDetails(LibFileUploadMDI model)
        {

            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@LibCategoryId", model.LibCategoryId);
            Parm.Add("@LibSubCategoryId", model.LibSubCategoryId);
            Parm.Add("@Description", model.Description);
            Parm.Add("@UserId", objCookie.getUserId());
            //Parm.Add("@Status", model.Status == true ? 1 : 0);
            DbConnection();
            con.Open();
            con.Execute("LibraryRecordFileInsert", Parm, commandType: CommandType.StoredProcedure);
            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.LibraryRecordInsert, "RecordId");
            return recid;
        }
        [HttpGet]
        public ActionResult Edit_LibraryFileRecord(int? id)
        {
            var hl = Lists.LibGetAllFileDetail().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_LibraryFile(LibFileUploadMDI objModel)
        {
            try
            {
                int result = Lists.LibUpdateFile(objModel);
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

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult LibraryDeleteRecord(int id)
        {
            if (mc.getPermission(Entry.Library, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.LibraryDeleteFile(id);
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

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }
        //Deleteproc
        public ActionResult LibraryDeleteRecordProc(int id)
        {
            if (mc.getPermission(Entry.Library, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.LibDeleteProc(id);
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

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }

        //ViewFile
        public ActionResult Index_GetAllFile()
        {
            if (mc.getPermission(Entry.Library, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            LibFileUploadMDI model = new LibFileUploadMDI();
            ViewBag.GetCategoryList = new SelectList(Lists.LibCategoryListDropdownList(), "LibCategoryId", "LibCategory");
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory", "LibSubCategory", "LibSubCategoryId");
            return View(model);
        }
        //Download File
        public ActionResult Index_GetAllFileDownload()
        {
            if (mc.getPermission(Entry.LibraryDownload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            setViewData();
            // int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf;base64", FileById.FileName);
            // return File(stream, "application/pdf");
            // return "data:application/pdf;base64," + read["FileContent"];
            // ; base64," + read["FileContent"];

        }

        [HttpGet]
        public PartialViewResult FileDetails()
        {
            setViewData();

            // int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> DetList = GetFileList();

            return PartialView("FileDetails", DetList);


        }

        [HttpGet]
        public PartialViewResult FileDetailsDownload()
        {
            //setViewData();
            // int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> DetList = GetFileList();

            return PartialView("FileDetailsDownload", DetList);


        }

        private List<LibFileUploadMDI> GetFileList()
        {
            List<LibFileUploadMDI> DetList = new List<LibFileUploadMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<LibFileUploadMDI>(con, "LibraryGetAllFile", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public ActionResult Index_GetIndentNo()
        {

            setViewData();
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile1(int id)
        {
            List<LibFileUploadMDI> ObjFiles = GetFileList1();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }


        [HttpGet]
        public PartialViewResult FileDetails1()
        {
            setViewData();
            List<LibFileUploadMDI> DetList = GetFileList1();

            return PartialView("FileDetails1", DetList);


        }
        private List<LibFileUploadMDI> GetFileList1()
        {
            List<LibFileUploadMDI> DetList = new List<LibFileUploadMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<LibFileUploadMDI>(con, "LibraryGetAllFile1", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
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

        //public object DocumentRetrieve(Dictionary<string, string> jsonResult)
        //{
        //    string Id = jsonResult["FileName"];
        //    string constr = System.Configuration.ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        //    System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(constr);
        //    //searches for the PDF document from the Database
        //   // var query = "select FileContent from LibraryFileUploadDetails where DocumentName = '" + documentID + "'";
        //    var query = "select FileContent from LibraryFileUploadDetails";
        //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query);
        //    cmd.Connection = con;
        //    con.Open();
        //    System.Data.SqlClient.SqlDataReader read = cmd.ExecuteReader();
        //    read.Read();
        //    return "data:application/pdf;base64," + read["FileContent"];
        //}


        public FileContentResult GetFile(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            // string mimeType = ""; 
            string fileName = "";
            // int Id;
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            // const string connect = "Server=DESKTOP-N39I570;Database=manufacturingmgtdbv1;Trusted_Connection=True;";
            //[LibraryGetAllFile]
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM LibraryFileUploadDetails WHERE Id = @ID";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@ID", Id);
                conn.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    Id = Convert.ToInt32(rdr["Id"].ToString());
                    fileContent = (byte[])rdr["FileContent"];
                    //  mimeType = rdr["MimeType"].ToString();
                    fileName = rdr["FileName"].ToString();
                }
            }
            return File(fileContent, fileName);

        }

        //DownloadRequest

        public ActionResult LibraryDownloadRequest()
        {
            //if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.LibraryDownload, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}

            // int luserid = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadBLL Lists = new LibFileUploadBLL();
            setViewData();
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI();
            // model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            // ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            // ViewBag.OwnerList = new SelectList(Lists.LibraryGetOwnerNameList(), "userid", "username");
            //  ViewBag.Department = new SelectList(Lists.LibraryGetDepartmentNameList(), "DepCode", "Department");
            ModelState.Clear();

            model.Item_List = Lists.LibraryDownloadRequestList();
            ViewBag.totalRequest = model.Item_List.Count;
            model.DownloadReason = "NA";
            return View(model);
        }

        //add
        [HttpPost]
        public ActionResult LibraryDownloadRequestSave(LibFileUploadMDI empmodel)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.LibraryDownload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.LibraryDownloadRequestInsert(empmodel);
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

                return RedirectToAction("LibraryDownloadRequest");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult Edit_LibraryDownloadRecord(int? id)
        {
            var hl = Lists.LibraryDownloadRequestList().Find(x => x.ReqId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_LibraryDownloadRequest(LibFileUploadMDI objModel)
        {
            //if (mc.getPermission(Entry.MasterFileUpload, permissionType.Edit) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}
            try
            {
                int result = Lists.LibraryDownloadRequestUpdate(objModel);
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

                return RedirectToAction("LibraryDownloadRequest");
            }
            catch
            {
                throw;
            }
        }

        public ActionResult LibraryDownloadDeleteRecord(int id)
        {
            if (mc.getPermission(Entry.Library, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.LibraryDownloadDelete(id);
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

                return RedirectToAction("LibraryDownloadRequest");
            }
            catch
            {
                throw;
            }
        }


        public ActionResult LibraryDownloadRequest_Approved()
        {
            //if (mc.getPermission(Entry.LibraryDownload, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            //}

            LibFileUploadBLL Lists = new LibFileUploadBLL();
            setViewData();
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI();
            ModelState.Clear();
            model.Item_List = Lists.LibraryDownloadRequestApproved();
            // ViewBag.totalRequest = model.Item_List.Count;
            // model.DownloadReason = "NA";
            return View(model);
        }

        public ActionResult LibraryDownloadRequest_Reject()
        {
            LibFileUploadBLL Lists = new LibFileUploadBLL();
            setViewData();
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI();
            ModelState.Clear();
            model.Item_List = Lists.LibraryDownloadRequestReject();
            // ViewBag.totalRequest = model.Item_List.Count;
            // model.DownloadReason = "NA";
            return View(model);
        }


        [HttpPost]
        public ActionResult InsertLibCatgory(LibFileUploadMDI objModel)
        {

            try
            {
                int result = Lists.AddNewLibCategory(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Save Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult InsertLibSubCatgory(LibFileUploadMDI objModel)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.AddNewLibSubCategory(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Save Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }

        //CategoryUpdate
        [HttpGet]
        public ActionResult Edit_LibraryCategoryRecord(int? id)
        {
            var hl = Lists.LibGetAllFileDetail().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_LibraryCategory1(LibFileUploadMDI objModel)
        {
            try
            {
                int result = Lists.LibUpdateCategory1(objModel);
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

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }

        //Subcategory
        [HttpGet]
        public ActionResult Edit_LibrarySubCategoryRecord(int? id)
        {
            var hl = Lists.LibGetAllFileDetail().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_LibrarySubCategory(LibFileUploadMDI objModel)
        {
            try
            {
                int result = Lists.LibUpdateSubCategory(objModel);
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

                return RedirectToAction("Index_GetAllFile");
            }
            catch
            {
                throw;
            }
        }

    }
}