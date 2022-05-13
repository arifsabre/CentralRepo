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
    public class TrainningController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        //
        //readonly Trainning_Insert traininmodel = new Trainning_Insert();
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private Trainning_Insert trainning = new Trainning_Insert();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
       [HttpGet]
        public ActionResult Details(int id)
        {
            //int userid = Convert.ToInt32(objCookie.getUserId());
            if (mc.getPermission(Entry.Training, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Trainning_Insert model1 = new Trainning_Insert();
            Trainning_Insert model2 = new Trainning_Insert();
            Trainning_Insert TopicList = new Trainning_Insert();
            Trainning_Insert model = new Trainning_Insert();

            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory", "LibSubCategory", "LibSubCategoryId");
            ViewBag.Topic = new SelectList(model1.Trainning_GetTopic(), "TopicId", "Trainning_Topic");
            ViewBag.EMPName = new SelectList(TopicList.getEmpListByCompany(Convert.ToInt32(objCookie.getCompCode())), "NewEmpId", "EmpName", objCookie.getCompCode());
            setViewData();
            model.Item_List2 = model.GetAllTrainningListByTopicId(id);
            model.Item_List = model.GetTrainningTopic();
            //return PartialView("_Details", model);
            return View(model);

        }
        [HttpPost]
        public ActionResult Details(Trainning_Insert objModel)
        {
            setViewData();
            if (mc.getPermission(Entry.Training, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Trainning_Insert model = new Trainning_Insert();
            try
            {
                int result = model.Training_DetailsSave(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record inserted Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult DetailsByTopicId(int topicid)
        {
            //int userid = Convert.ToInt32(objCookie.getUserId());
            if (mc.getPermission(Entry.Training, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            Trainning_Insert model = new Trainning_Insert();
            setViewData();
            model.Item_List2 = model.GetFileByRecordId(topicid);
            //return PartialView("_Details", model);
            return View(model);

        }
        public ActionResult TrainningDeleteRecord(int id)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_Insert_Delete(id);
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

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult Trainning_Group_Delete(int id)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_GroupDelete(id);
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

                return RedirectToAction("IndexTrainningGroup");
            }
            catch
            {
                throw;
            }
        }
        //UpdateTrainning
        [HttpGet]
        public ActionResult Edit_TrainningDateRecord(int? id)
        {
           // int luserid = Convert.ToInt32(objCookie.getUserId());
            Trainning_Insert model = new Trainning_Insert();
            var hl = model.GetAllTrainningGroup().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_TrainningDate(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_DateUpdate(objModel);
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

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        //UpdateTrainning
        [HttpGet]
        public ActionResult Edit_TrainningTopicRecord(int? id)
        {
            int topicid;
            topicid = 2;
            // int luserid = Convert.ToInt32(objCookie.getUserId());
            Trainning_Insert model = new Trainning_Insert();
            var hl = model.GetAllTrainningListByTopicId(topicid).Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_TrainningTopic(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_TopicUpdate(objModel);
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

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        //UpdateTrainner
        [HttpGet]
        public ActionResult Edit_TrainnerRecord(int? id)
        {
            // int luserid = Convert.ToInt32(objCookie.getUserId());
            Trainning_Insert model = new Trainning_Insert();
            var hl = model.GetAllTrainningGroup().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_Trainner(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_TrainnerUpdate(objModel);
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

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        //UpdateTrainner
       
        [HttpPost]
        public ActionResult Edit_Trainneee(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_TrainneeUpdate(objModel);
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

                return RedirectToAction("Details");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult IndexTopicList()
        {
            AAA_MedicalTestBLL Lists = new AAA_MedicalTestBLL();
            // int userid = Convert.ToInt32(objCookie.getUserId());
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //ViewBag.Topic = new SelectList(model.Trainning_GetTopic(), "TopicId", "Trainning_Topic");
            //ViewBag.EMPName = new SelectList(Lists.Get_EmpMedicalNewEMPId(), "NewEmpId", "EmpName");
            model.Item_List2 = model.GetTrainningTopic();
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertTrainningTopic(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            try
            {
                int result = model.Trainning_TopicSave(objModel);
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
                return RedirectToAction("IndexTrainningGroup");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult TrainningDeleteTopic(int id)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_Deletetopic(id);
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

                return RedirectToAction("IndexTrainningGroup");
            }
            catch
            {
                throw;
            }
        }
        //UpdateTrainner
        [HttpGet]
        public ActionResult Edit_TopicMasterRecord(int? id)
        {
            // int luserid = Convert.ToInt32(objCookie.getUserId());
            Trainning_Insert model = new Trainning_Insert();
            var hl = model.GetTrainningTopic().Find(x => x.TopicId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit_TopicMaster(Trainning_Insert objModel)
        {
            Trainning_Insert model = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = model.Trainning_TopicMasterUpdate(objModel);
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

                return RedirectToAction("IndexTrainningGroup");
            }
            catch
            {
                throw;
            }
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
            return RedirectToAction("Details");
        }
       //Trainning Group
        [HttpGet]
        public ActionResult IndexTrainningGroup()
        {
          
            Trainning_Insert model = new Trainning_Insert();
            Trainning_Insert TopicList = new Trainning_Insert();
            if (mc.getPermission(Entry.Training, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.Topic = new SelectList(model.Trainning_GetTopic(), "TopicId", "Trainning_Topic");
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory", "LibSubCategory", "LibSubCategoryId");
            ViewBag.EMPName = new SelectList(TopicList.getEmpListByCompany(Convert.ToInt32(objCookie.getCompCode())), "NewEmpId", "EmpName", objCookie.getCompCode());
            model.Item_List2 = model.GetAllTrainningGroup();
            model.Item_List4 = model.Trainning_GetTopic();
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
        public ActionResult IndexTrainningGroup(int countryId)
        {
            LibFileUploadMDI model = new LibFileUploadMDI();
            model.LibraryCategory = PopulateDropDown("SELECT LibCategoryId, LibCategory FROM LibraryCategory", "LibCategory", "LibCategoryId");
            model.LibrarySubCategory = PopulateDropDown("SELECT LibSubCategoryId, LibSubCategory FROM LibrarySubCategory WHERE LibCategoryId=" + countryId, "LibSubCategory", "LibSubCategoryId");
            return View(model);
        }

        [HttpPost]
        public ActionResult IndexTrainningGroup1(Trainning_Insert model, List<HttpPostedFileBase> files)
        {
            foreach (HttpPostedFileBase file in model.files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                }
            }
            UploadFile1(model);
            ViewBag.Message = "Data Save Successfully";
            ModelState.Clear();
            return RedirectToAction("IndexTrainningGroup");
            // return View();
        }

         public void UploadFile1(Trainning_Insert model)
        {

            // String FileExt = Path.GetExtension(files.FileName).ToUpper();


            //save mail details
            var recordId = SaveMailDetails1(model);

            // save attachements
            foreach (HttpPostedFileBase file in model.files)
            {

                if (file != null)
                {
                    Stream str = file.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                    Trainning_Insert Fd = new Trainning_Insert
                    {
                        FileName = file.FileName,
                        FileContent = FileDet,
                        RecordId = Convert.ToInt32(recordId)
                    };
                    SaveFileDetails1(Fd);
                }
            }
        }
        private void SaveFileDetails1(Trainning_Insert objDet)
        {
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            DbConnection();
            con.Open();
            con.Execute("Trainning_FileInsert", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        private string SaveMailDetails1(Trainning_Insert model)
        {
            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            // Parm.Add("@RecordId", model.RecordId);
            Parm.Add("@compcode", model.compcode);
            Parm.Add("@TopicId", model.TopicId);
            Parm.Add("@LibCategoryId", model.LibCategoryId);
            Parm.Add("@Trainner", model.Trainner);
            Parm.Add("@TrainningDate", model.TrainningDate);
            Parm.Add("@TotalPerson", model.TotalPerson);
            Parm.Add("@CreatedBy", objCookie.getUserName());
            Parm.Add("@UserId", objCookie.getUserId());
            Parm.Add("@LibSubCategoryId", model.LibSubCategoryId);
            DbConnection();
            con.Open();
            con.Execute("Trainning_GroupSave", Parm, commandType: CommandType.StoredProcedure);
            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.Trainning_Group, "RecordId");
            return recid;
        }
        //getFile
        public FileContentResult GetQutation(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM Trainning_File WHERE RecordId = @RecordId";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@RecordId", Id);
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