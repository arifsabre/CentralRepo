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
    public class LibraryMasterUploadController : Controller
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
        MasterLibraryBLL Masterlibobj = new MasterLibraryBLL();
        readonly LibFileUploadBLL Lists = new LibFileUploadBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult LibraryDaishboard()
        {

            return View();
        }


        public ActionResult Indexcategory()
        {
            // if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };

            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            int luserid = Convert.ToInt32(objCookie.getUserId());
            // LibFileUploadBLL Lists = new LibFileUploadBLL();
            setViewData();
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI();
            //  model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.OwnerList = new SelectList(Masterlibobj.MasterLibraryGetOwnerList(), "NewEmpId", "EmpName");
            ViewBag.Department = new SelectList(Lists.LibraryGetDepartmentNameList(), "DepCode", "Department");
            ViewBag.IssueTo = new SelectList(Masterlibobj.MasterLibraryGetIssueTo(), "userid", "FullName");
            //  ModelState.Clear();
            model.DoccumentName = " ";
            model.Location = " ";
            model.Keyss = " ";
            model.Type1 = " ";

            model.Item_List = Masterlibobj.MasterLibraryGetAllFileDetail(luserid);
            ViewBag.totalDoc = model.Item_List.Count.ToString(); ;

            //ReturnToConfirm
            LibFileUploadMDI modelr = new LibFileUploadMDI();
            int useridr = Convert.ToInt32(objCookie.getUserId());
            modelr.Item_List = Masterlibobj.FileReturnToconfirmByUser(useridr);
            ViewBag.totalReturn = modelr.Item_List.Count.ToString();

            //issued
            LibFileUploadMDI modelObject2 = new LibFileUploadMDI { };
            MasterLibraryBLL List2 = new MasterLibraryBLL();
            int userid2 = Convert.ToInt32(objCookie.getUserId());
            modelObject2.Item_List = List2.MasterLibraryGetIssueToListByUser(userid2);
            ViewBag.TotalIssued = modelObject2.Item_List.Count.ToString();
            //recieved
            LibFileUploadMDI model2 = new LibFileUploadMDI();
            model2.Item_List = Masterlibobj.FileRecieveByUser(luserid);
            ViewBag.totalRecieved = model2.Item_List.Count.ToString();
            ////Letter
            //LetterMDI modelObject1 = new LetterMDI { };
            //LetterMDI List1 = new LetterMDI();
            //int userid = Convert.ToInt32(objCookie.getUserId());
            //modelObject1.Item_List = List1.GetAllLetterListByUserId(luserid);
            //ViewBag.TotalLetter = modelObject1.Item_List.Count.ToString();

            ////PITotal
            //LetterMDI modelpi = new LetterMDI { };
            //modelpi.Item_List = modelpi.GetAllLetterListByCompanyPI();
            //ViewBag.PI = modelpi.Item_List.Count.ToString();

            return View(model);
        }

        [HttpGet]
        public PartialViewResult AllPhysical()
        {
            LibFileUploadMDI model = new LibFileUploadMDI();
            MasterLibraryBLL modelobj = new MasterLibraryBLL();
            // model1.Item_List2 = model1.GetAllLetterList();
            if (mc.getPermission(Entry.MasterFileDownload, permissionType.Add) == false)
            {
                // Redirecttoaction("ff");  // return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
                //return RedirectToAction("Index");
                return PartialView("_edd");
            }
            else
            {
                // LetterMDI model = new LetterMDI();
                setViewData();

                model.Item_List1 = modelobj.MasterLibraryGetAllFile();
                return PartialView("_AllPhysicalFiles", model);
            }
        }


        [HttpPost]
        public ActionResult Indexcategory(LibFileUploadMDI model)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {


                int result = Masterlibobj.MasterLibraryAddRecord(model);
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

                return RedirectToAction("Indexcategory");
            }
            catch
            {
                throw;
            }


        }

        //public void UploadFile(LibFileUploadMDI model)
        //{
        //    // String FileExt = Path.GetExtension(files.FileName).ToUpper();


        //    //save mail details
        //    var recordId = SaveMailDetails(model);

        // save attachements
        //foreach (HttpPostedFileBase file in model.files)
        //{

        //    if (file != null)
        //    {
        //        Stream str = file.InputStream;
        //        BinaryReader Br = new BinaryReader(str);
        //        Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

        //        LibFileUploadMDI Fd = new Models.LibFileUploadMDI();
        //        Fd.FileName = file.FileName;
        //        Fd.FileContent = FileDet;
        //        Fd.RecordId = Convert.ToInt32(recordId);
        //       // SaveFileDetails(Fd);
        //    }
        //}

        // }

        //private void SaveFileDetails(LibFileUploadMDI objDet)
        //{

        //    DynamicParameters Parm = new DynamicParameters();
        //    Parm.Add("@RecordId", objDet.RecordId);
        //    Parm.Add("@FileName", objDet.FileName);
        //    Parm.Add("@FileContent", objDet.FileContent);
        //    DbConnection();
        //    con.Open();
        //    con.Execute("LibraryMasterUploadFileInsert", Parm, commandType: System.Data.CommandType.StoredProcedure);
        //    con.Close();

        //}
        //private string SaveMailDetails(LibFileUploadMDI model)
        //{

        //    clsMyClass mc = new clsMyClass();
        //    DynamicParameters Parm = new DynamicParameters();
        //    Parm.Add("@Compcode", model.compcode);
        //    Parm.Add("@DepCode", model.DepCode);
        //    Parm.Add("@DoccumentName", model.DoccumentName);
        //    Parm.Add("@Type", model.Type);
        //    Parm.Add("@Location", model.Location);
        //    Parm.Add("@Keyss", model.Keyss);
        //    Parm.Add("@Status", model.Status);
        //    Parm.Add("@OwnerId", model.OwnerId);
        //   Parm.Add("@UserId", objCookie.getUserId());
        //    Parm.Add("@CreatedBy", objCookie.getUserName());
        //    DbConnection();
        //    con.Open();
        //    con.Execute("LibraryMasterUploadInsert", Parm, commandType: CommandType.StoredProcedure);
        //    // procedure to get recently added recordid
        //    SqlCommand cmd = new SqlCommand
        //    {
        //        CommandType = CommandType.StoredProcedure,
        //        Connection = con
        //    };
        //    string recid = mc.getRecentIdentityValue(cmd, dbTables.MasterLibraryFileUpload, "RecordId");
        //    return recid;
        //}

        [HttpGet]
        public ActionResult Edit_LibraryMasterFileRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Masterlibobj.MasterLibraryGetAllFileDetail(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_LibraryMasterFile(LibFileUploadMDI objModel)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Masterlibobj.MasterLibraryUpdate(objModel);
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

                return RedirectToAction("Indexcategory");
            }
            catch
            {
                throw;
            }
        }

        public ActionResult LibraryMasterDeleteRecord(int id)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            try
            {
                int result = Masterlibobj.MasterLibraryDeleteFile(id);
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

                return RedirectToAction("Indexcategory");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetDetailsMasterRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Masterlibobj.MasterLibraryGetAllFileDetail(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }


        //ViewOnlyFile
        public ActionResult Index_GetAllMasterFile()
        {
            if (mc.getPermission(Entry.MasterFileDownload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();
        }
        //Download MasterFile
        public ActionResult Index_GetAllMasterFileDownload()
        {
            if (mc.getPermission(Entry.MasterFileDownload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            return View();
        }
        [HttpGet]
        public FileResult DownLoadMasterFile(int id)
        {

            int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> ObjFiles = MasterGetFileList(luserid);

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf;base64", FileById.FileName);


        }

        [HttpGet]
        public PartialViewResult MasterFileDetails()
        {

            int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> DetList = MasterGetFileList(luserid);

            return PartialView("MasterFileDetails", DetList);


        }

        [HttpGet]
        public PartialViewResult MasterFileDetailsDownload()
        {

            int luserid = Convert.ToInt32(objCookie.getUserId());
            List<LibFileUploadMDI> DetList = MasterGetFileList(luserid);

            return PartialView("MasterFileDetailsDownload", DetList);
        }
        private List<LibFileUploadMDI> MasterGetFileList(int userid)
        {
            List<LibFileUploadMDI> DetList = new List<LibFileUploadMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<LibFileUploadMDI>(con, "MasterLibraryGetAllFileByUser", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        public ActionResult Index_GetIndentNo()
        {

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
            List<LibFileUploadMDI> DetList = GetFileList1();

            return PartialView("FileDetails1", DetList);
        }

        private List<LibFileUploadMDI> GetFileList1()
        {
            List<LibFileUploadMDI> DetList = new List<LibFileUploadMDI>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<LibFileUploadMDI>(con, "LibraryMasterGetAllFileByUser1", commandType: CommandType.StoredProcedure).ToList();
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


        public FileContentResult GetFileMaster(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM LibraryMasterFileUploadDetails WHERE Id = @ID";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@ID", Id);
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
        //IssueTo

        [HttpGet]
        public ActionResult IssueTo_Record(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            var hl = Masterlibobj.MasterLibraryGetAllFileDetail(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult IssueTo(LibFileUploadMDI objModel)
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Masterlibobj.IssueToFile(objModel);
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

                return RedirectToAction("Indexcategory");
            }
            catch
            {
                throw;
            }
        }

        //GetIssuedListbyUser
        public ActionResult GetissuedListByUser()
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            ModelState.Clear();
            LibFileUploadMDI model = new LibFileUploadMDI();
            int userid2 = Convert.ToInt32(objCookie.getUserId());
            model.Item_List = Masterlibobj.MasterLibraryGetIssueToListByUser(userid2);
            ViewBag.totalIssued = model.Item_List.Count.ToString();
            //ReturnToConfirm
            LibFileUploadMDI modelr = new LibFileUploadMDI();
            int useridr = Convert.ToInt32(objCookie.getUserId());
            modelr.Item_List = Masterlibobj.FileReturnToconfirmByUser(useridr);
            ViewBag.totalReturn = modelr.Item_List.Count.ToString();
            //           
            LibFileUploadMDI model2 = new LibFileUploadMDI();
            int userid3 = Convert.ToInt32(objCookie.getUserId());
            model2.Item_List = Masterlibobj.FileRecieveByUser(userid3);
            ViewBag.totalRecieved = model2.Item_List.Count.ToString();
            //
            LibFileUploadMDI model1 = new LibFileUploadMDI();
            int userid4 = Convert.ToInt32(objCookie.getUserId());
            model1.Item_List = Masterlibobj.MasterLibraryGetAllFileDetail(userid4);
            ViewBag.totalDoc = model1.Item_List.Count.ToString(); ;


            //Letter
            LetterMDI modelObject1 = new LetterMDI { };
            LetterMDI List1 = new LetterMDI();
            int userid5 = Convert.ToInt32(objCookie.getUserId());
            modelObject1.Item_List = List1.GetAllLetterListByUserId();
            ViewBag.TotalLetter = modelObject1.Item_List.Count.ToString();

            return View(model);
        }

        //GetRecieveListByUser
        public ActionResult GetRecievedListByUser()
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            ViewBag.IssueTo = new SelectList(Masterlibobj.MasterLibraryGetIssueTo(), "userid", "FullName");

            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            ModelState.Clear();

            //ReturnToConfirm
            LibFileUploadMDI modelr = new LibFileUploadMDI();
            int useridr = Convert.ToInt32(objCookie.getUserId());
            modelr.Item_List = Masterlibobj.FileReturnToconfirmByUser(useridr);
            ViewBag.totalReturn = modelr.Item_List.Count.ToString();
            //issued
            LibFileUploadMDI model2 = new LibFileUploadMDI();
            int userid5 = Convert.ToInt32(objCookie.getUserId());
            model2.Item_List = Masterlibobj.MasterLibraryGetIssueToListByUser(userid5);
            ViewBag.totalIssued = model2.Item_List.Count.ToString();

            //
            LibFileUploadMDI model = new LibFileUploadMDI();
            model.Item_List = Masterlibobj.FileRecieveByUser(luserid);
            ViewBag.totalRecieved = model.Item_List.Count.ToString();

            //
            LibFileUploadMDI model1 = new LibFileUploadMDI();
            int useridt = Convert.ToInt32(objCookie.getUserId());
            model1.Item_List = Masterlibobj.MasterLibraryGetAllFileDetail(useridt);
            ViewBag.totalDoc = model1.Item_List.Count.ToString(); ;


            ////Letter
            //LetterMDI modelObject1 = new LetterMDI { };
            //LetterMDI List1 = new LetterMDI();
            //int userid = Convert.ToInt32(objCookie.getUserId());
            //modelObject1.Item_List = List1.GetAllLetterListByUserId();
            //ViewBag.TotalLetter = modelObject1.Item_List.Count.ToString();

            return View(model);
        }

        //ReturnUpdate
        [HttpGet]
        public ActionResult Edit_ReturnRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            // LetterMDI model = new LetterMDI();
            MasterLibraryBLL Masterlibobj = new MasterLibraryBLL();
            var hl = Masterlibobj.FileRecieveByUser(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_Return(LibFileUploadMDI objModel)
        {
            MasterLibraryBLL model = new MasterLibraryBLL();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MasterLibraryReturnTo(objModel);
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

                return RedirectToAction("GetRecievedListByUser");
            }
            catch
            {
                throw;
            }
        }

        //ReturnConfirmation
        [HttpGet]
        public ActionResult Edit_ReturmConfirmationRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            // LetterMDI model = new LetterMDI();
            MasterLibraryBLL Masterlibobj = new MasterLibraryBLL();
            var hl = Masterlibobj.FileReturnToconfirmByUser(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_ReturnConfirmation(LibFileUploadMDI objModel)
        {
            MasterLibraryBLL model = new MasterLibraryBLL();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MasterLibraryReturnConfirmation(objModel);
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

                return RedirectToAction("Indexcategory");
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
            int luserid = Convert.ToInt32(objCookie.getUserId());
            // LetterMDI model = new LetterMDI();
            MasterLibraryBLL Masterlibobj = new MasterLibraryBLL();
            var hl = Masterlibobj.MasterLibraryGetAllFileDetail(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_Company(LibFileUploadMDI objModel)
        {
            MasterLibraryBLL model = new MasterLibraryBLL();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MasterLibraryCompanyUpdate(objModel);
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

                return RedirectToAction("IndexCategory");
            }
            catch
            {
                throw;
            }
        }

        //UpdateOwner
        [HttpGet]
        public ActionResult Edit_OwnerRecord(int? id)
        {
            int luserid = Convert.ToInt32(objCookie.getUserId());
            // LetterMDI model = new LetterMDI();
            MasterLibraryBLL Masterlibobj = new MasterLibraryBLL();
            var hl = Masterlibobj.MasterLibraryGetAllFileDetail(luserid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_Owner(LibFileUploadMDI objModel)
        {
            MasterLibraryBLL model = new MasterLibraryBLL();
            if (mc.getPermission(Entry.MasterFile, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.MasterLibraryOwnerUpdate(objModel);
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

                return RedirectToAction("IndexCategory");
            }
            catch
            {
                throw;
            }
        }
        //ReturnToConfirmList
        public ActionResult GetReturnTOConfirmListByUser()
        {
            if (mc.getPermission(Entry.MasterFile, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            // ViewBag.IssueTo = new SelectList(Masterlibobj.MasterLibraryGetIssueTo(), "userid", "FullName");

            int luserid = Convert.ToInt32(objCookie.getUserId());
            setViewData();
            // ModelState.Clear();

            //ReturnToConfirm
            LibFileUploadMDI modelr = new LibFileUploadMDI();
            int useridr = Convert.ToInt32(objCookie.getUserId());
            modelr.Item_List = Masterlibobj.FileReturnToconfirmByUser(useridr);
            ViewBag.totalReturn = modelr.Item_List.Count.ToString();


            //issued
            LibFileUploadMDI model2 = new LibFileUploadMDI();
            int userid5 = Convert.ToInt32(objCookie.getUserId());
            model2.Item_List = Masterlibobj.MasterLibraryGetIssueToListByUser(userid5);
            ViewBag.totalIssued = model2.Item_List.Count.ToString();

            //
            LibFileUploadMDI model = new LibFileUploadMDI();
            model.Item_List = Masterlibobj.FileRecieveByUser(luserid);
            ViewBag.totalRecieved = model.Item_List.Count.ToString();

            //
            LibFileUploadMDI model1 = new LibFileUploadMDI();
            model1.Item_List = Masterlibobj.MasterLibraryGetAllFileDetail(luserid);
            ViewBag.totalDoc = model1.Item_List.Count.ToString(); ;


            //Letter
            LetterMDI modelObject1 = new LetterMDI { };
            LetterMDI List1 = new LetterMDI();
            int userid = Convert.ToInt32(objCookie.getUserId());
            modelObject1.Item_List = List1.GetAllLetterListByUserId();
            ViewBag.TotalLetter = modelObject1.Item_List.Count.ToString();

            return View(modelr);
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

    }
}