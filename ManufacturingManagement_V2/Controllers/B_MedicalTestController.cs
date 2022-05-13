using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Data;
namespace ManufacturingManagement_V2.Controllers
{
    public class B_MedicalTestController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ToString();
            con = new SqlConnection(constr);
        }
        AAA_MedicalTestBLL Lists = new AAA_MedicalTestBLL();
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        private void SetViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult History()
        {

            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
           
            AAA_MedicalTest model = new AAA_MedicalTest();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();
            //model.tbl_employee = PopulateDropDown("SELECT NewEmpId, EmpName FROM tbl_employee  where Isactive=1 order by EmpName", "EmpName", "NewEmpid");
            ViewBag.EMPName = new SelectList(Lists.Get_EmpMedicalNewEMPId(), "NewEmpId", "EmpName");
            ModelState.Clear();
            model.UpdateStatus = 1;
            model.List = Lists.Get_EmpMedicalList_History();
            return View(model);
        }
        public ActionResult UpdateMedicalIndex()
        {
            AAA_MedicalTestBLL objlist = new AAA_MedicalTestBLL();
            AAA_MedicalTest model = new AAA_MedicalTest();
            AAA_MedicalTest modelv = new AAA_MedicalTest();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            ViewBag.VacType = new SelectList(objlist.Get_EmpMedicalVacType(), "VacId", "VacType");
            model.UpdateStatus = 1;
            model.List = Lists.Get_EmpMedicalList_History();
            model.Item_List = objlist.GetEmpVaccineList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Update_Medical_Record(int? id)
        {


            var hl = Lists.Get_EmpMedicalList_History().Find(x => x.MedicalId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update_MedicalTest(AAA_MedicalTest objModel)
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.Update_Emp_Medical_Test(objModel);
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

                return RedirectToAction("UpdateMedicalIndex");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult AddNewRecord()
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            SetViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            AAA_MedicalTest model = new AAA_MedicalTest();
            model.BloodGroup = "A+";
            model.Bp = "0";
            model.Pulse = "0";
            model.EyeSight = "0";
            model.Height = "0";
            model.Hemoglobin = "0";
            model.Remark = "NA";
            model.Oxygen = "0";
            model.Sugar = "0";
            model.Bp = "0";
            model.Bplow = "0";
            model.Allergies = "NO";
            model.HealthCondition = "Ok";
            return View(model);
        }
        [HttpPost]
        public ActionResult AddNewRecord(AAA_MedicalTest empmodel)
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = Lists.Insert_NewEmp_Medical_TestRecord(empmodel);
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

                return RedirectToAction("UpdateMedicalIndex");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult DeleteMedicalTestRecord(int id)
        {
            try
            {
                int result = Lists.DeleteMedicalTest(id);
                if (result == 1)
                {
                    ViewBag.Message = "Record Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("History");
            }
            catch
            {
                throw;
            }
        }
       
        //Vaccine
        [HttpGet]
        public ActionResult IndexVaccine()
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            int compcode = Convert.ToInt32(objCookie.getCompCode());
            AAA_MedicalTestBLL obllist = new AAA_MedicalTestBLL();
            AAA_MedicalTestBLL obllist1 = new AAA_MedicalTestBLL();
            AAA_MedicalTestBLL obllist2 = new AAA_MedicalTestBLL();
            C_ComplaintBLL obllistc = new C_ComplaintBLL();
            AAA_MedicalTest model = new AAA_MedicalTest();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();
            ViewBag.VacType = new SelectList(obllist1.Get_EmpMedicalVacType(), "VacId", "VacType");
            //ViewBag.EmpList = new SelectList(obllistc.ComplaintAssignTo(), "NewEmpId", "EmpName");
            ViewBag.EMPName = new SelectList(obllist2.GetEmpListByCompcode(compcode), "NewEmpId", "EmpName");
            model.Item_List = obllist.GetEmpVaccineList();
            //model.Item_List = obllist.GetEmpVaccineListByRecordId();
            return View(model);
        }
        [HttpPost]
        public ActionResult IndexVaccine(AAA_MedicalTest model, List<HttpPostedFileBase> files)
        {
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
            return RedirectToAction("UpdateMedicalIndex");
            // return View();

        }
        [HttpPost]
        public ActionResult IndexVaccine1(AAA_MedicalTest model, List<HttpPostedFileBase> files)
        {
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
            return RedirectToAction("IndexVaccine");
            // return View();

        }
        public void UploadFile(AAA_MedicalTest model)
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
                    AAA_MedicalTest Fd = new Models.AAA_MedicalTest();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.RecordId = Convert.ToInt32(recordId);
                    SaveFileDetails(Fd);
                }
            }
        }
        private void SaveFileDetails(AAA_MedicalTest objDet)
        {
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@RecordId", objDet.RecordId);
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            DbConnection();
            con.Open();
            con.Execute("Medical_Vaccine_InsertFile", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        private string SaveMailDetails(AAA_MedicalTest model)
        {

            clsMyClass mc = new clsMyClass();
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@NewEmpId", model.NewEmpId);
            Parm.Add("@VacId", model.VacId);
            Parm.Add("@Dose1Date", model.Dose1Date);
            Parm.Add("@Dose2Date", model.Dose2Date);
            Parm.Add("@Dose1", model.Dose1);
            Parm.Add("@Dose2", model.Dose2);
            Parm.Add("@UserId", objCookie.getUserId());
            DbConnection();
            con.Open();
            con.Execute("Medical_Vaccine_Insert", Parm, commandType: CommandType.StoredProcedure);

            // procedure to get recently added recordid
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = con
            };
            string recid = mc.getRecentIdentityValue(cmd, dbTables.AAA_EmpMedicalTest_Vaccine, "RecordId");
            return recid;

        }
       [HttpGet]
        public PartialViewResult GetEmpVaccineList()
        {

           // int userid = Convert.ToInt32(objCookie.getUserId());
            AAA_MedicalTest model = new AAA_MedicalTest();
            AAA_MedicalTestBLL obllist = new AAA_MedicalTestBLL();
            SetViewData();
            model.Item_List = obllist.GetEmpVaccineList();
            return PartialView("_Vaccine", model);
        }
        //getFile
        public FileContentResult GetQutation(int Id)
        {
            SqlDataReader rdr; byte[] fileContent = null;
            string fileName = "";
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                var qry = "SELECT Id,FileContent,FileName FROM AAA_EmpMedicalTest_File WHERE Id = @Id";
                var cmd = new SqlCommand(qry, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
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
        public ActionResult DeleteVaccine(int id)
        {
            AAA_MedicalTestBLL bll = new AAA_MedicalTestBLL();
            if(mc.getPermission(Entry.MedicalTest, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            try
            {
                int result = bll.DeleteVacRecord(id);
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

                return RedirectToAction("IndexVaccine");
            }
            catch
            {
                throw;
            }
        }

        //update Second Dose
        [HttpPost]
        public ActionResult UploadSecondVaccinedCertificate(HttpPostedFileBase postedFile, AAA_MedicalTest hld)
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Add) == false)
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
                using (SqlCommand cmd = new SqlCommand("Medical_Vaccine_InsertFile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileName", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@FileContent", bytes);
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@Doc", hld.Doc);
                   // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    // con.Close();
                }
            }
            return RedirectToAction("IndexVaccine");
            // return View(model);
        }
        [HttpGet]
        public ActionResult UploadSecondCertificatetRecord(int? id)
        {
            int userid = Convert.ToInt32(objCookie.getUserId());
            AAA_MedicalTestBLL model = new AAA_MedicalTestBLL();
            var hl = model.GetEmpVaccineList().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }
        //Update Second Dose Record
        public JsonResult Edit_VanSecordDose_Record(int? id)
        {

            AAA_MedicalTestBLL model = new AAA_MedicalTestBLL();
            var hl = model.GetEmpVaccineList().Find(x => x.RecordId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_VaccineRecord(AAA_MedicalTest objModel)
        {
            if (mc.getPermission(Entry.MedicalTest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
           try
            {
                int result = Lists.Vaccination_Update_Record(objModel);
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

                return RedirectToAction("IndexVaccine");
            }
            catch
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult IndexCertificate(int id)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();
            if (mc.getPermission(Entry.MedicalTest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
           AAA_MedicalTestBLL obllist = new AAA_MedicalTestBLL();
           AAA_MedicalTest model = new AAA_MedicalTest();
            model.Item_List = obllist.AllVaccineCertificate(id);
          
            return View(model);
        }


    }
}


    
