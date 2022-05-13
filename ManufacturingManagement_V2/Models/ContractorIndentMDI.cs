using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
namespace ManufacturingManagement_V2.Models
{
    public class ContractorIndentMDI
    {
        // private object objCookie;
        clsCookie objCookie = new clsCookie();
        //[Required(ErrorMessage = "Location")]
        //[Display(Name = "Enter Work Location")]
        public string Location { get; set; }

        public int compcode { get; set; }
        [Required(ErrorMessage = "Company")]
        [Display(Name = "Company")]
        public string cmpname { get; set; }

        public string email { get; set; }
        public int Doc { get; set; }

        [Required(ErrorMessage = "WorkDetail")]
        [Display(Name = "Work Detail")]
        public string WorkDetail { get; set; }

        [Required(ErrorMessage = "Reason")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Contractor")]
        public string Contractor { get; set; }

        [Required(ErrorMessage = "Estimate Amount")]
        [Display(Name = "Estimated Amount")]
        public decimal EstimateAmount { get; set; }

        [Required(ErrorMessage = "Remark?")]
        [Display(Name = "Task Creater Remark")]
        public string Remark { get; set; }

        public string WorkRating { get; set; }

        [Required(ErrorMessage = "Work Completed Date Required")]
        [Display(Name = "Work Completed Date")]
        [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string WorkComplatedDate { get; set; }

        [Required(ErrorMessage = "Work Start Date Required")]
        [Display(Name = "Work Start Date")]
        [DataType(DataType.Date)]
        //  [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string WorkStartDate { get; set; }


        [Required(ErrorMessage = "Expected Completion Date Required")]
        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode =  true)]
        //public Nullable<System.DateTime>
        public string CompletionDate { get; set; }
        // public string CompletionDateStr { get; set; }

        public int UserId { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Select Approve or Reject")]
        [Display(Name = "Select Approve or Reject")]
        public string HODApproval { get; set; }

        public string HODApprovalBy { get; set; }
        public string HODApprovalDate { get; set; }
        public string HODRemark { get; set; }

        public int AssignTo { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Select Approve or Reject")]
        [Display(Name = "Select Approve or Reject")]
        public string FinalApproval { get; set; }

        public string FinalApprovalBY { get; set; }
        public string FinalApprovalDate { get; set; }
        public string FinalRemark { get; set; }

        [Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached Quotation")]
        public List<HttpPostedFileBase> files { get; set; }

        public int Id { get; set; }

        [Display(Name = "TaskNo")]
        public int RecordId { get; set; }
        [Required(ErrorMessage = "Attached Quotation?")]
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public bool Status1 { get; set; }
        [Required(ErrorMessage = "Select Category")]
        public int WId { get; set; }
        [Required(ErrorMessage = "WorkCategory?")]
        [Display(Name = "WorkCategory")]
        public string WorkCategory { get; set; }
        public string Category { get; set; }
        [Required(ErrorMessage = "Category is Required:")]
        public int CatId { get; set; }
        [Required(ErrorMessage = "Priority is Required:")]
        [Display(Name = "Priority")]
        public string Priority { get; set; }
        public int PID { get; set; }
        [Required(ErrorMessage = "Status is Required:")]
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Status is Required:")]
        public int SID { get; set; }
        [Required(ErrorMessage = "Status is Required:")]
        public string hodname { get; set; }
        [Required(ErrorMessage = "Status is Required:")]
        public int hodid { get; set; }
        public List<ContractorIndentMDI> Item_List { get; set; }
        public ContractorIndentMDI()
        {
            this.CopCategory = new List<SelectListItem>();
            this.CopStatus = new List<SelectListItem>();
            this.tbl_employee = new List<SelectListItem>();
            this.tbl_company = new List<SelectListItem>();
            this.CopPrority = new List<SelectListItem>();

        }
        public List<SelectListItem> CopCategory { get; set; }
        public List<SelectListItem> CopStatus { get; set; }
        public List<SelectListItem> tbl_employee { get; set; }
        public List<SelectListItem> tbl_company { get; set; }
        public List<SelectListItem> CopPrority { get; set; }
        public List<ContractorIndentMDI> GetReferenceNo()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[ContractorGetReferenceNo]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI
                        {
                        RecordId = Convert.ToInt32(reader["RecordId"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public int IndentCivilUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ContractorindentUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@FinalApproval", hld.FinalApproval);
                    cmd.Parameters.AddWithValue("@FinalRemark", hld.FinalRemark);
                    cmd.Parameters.AddWithValue("@FinalApprovalBY", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        ////hodApprovalUpdate
        public int IndentCivilHODUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ContractorHodApprovalUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@HODApproval", hld.HODApproval);
                    cmd.Parameters.AddWithValue("@HODRemark", hld.HODRemark);
                    //cmd.Parameters.AddWithValue("@Priority", hld.Priority);
                    //cmd.Parameters.AddWithValue("@WorkStartDate", hld.WorkStartDate);
                    //cmd.Parameters.AddWithValue("@WorkComplatedDate", hld.WorkComplatedDate);
                    // cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@HODApprovalBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int IndentDelete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_Delete_Task", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", id);
                    //cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    //cmd.Parameters.AddWithValue("@Category", hld.Category);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
      
        //Statusupdate
        public int ContractorStatusUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ContractorUpdate_Status", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@Status", hld.Status);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Categoryupdate
        public int ContractorCategoryUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ContractorUpdate_Category", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@Category", hld.Category);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //Priorityyupdate
        public int ContractorPriorityUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ContractorUpdate_Priority", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@Priority", hld.Priority);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Contractor_Complaint_InsertFirstStep
        public List<ContractorIndentMDI> GetAllCivilUser_Section(int userid)
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_GetComplaint_User_Sec", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@userid", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.hodname = reader["hodname"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.Category = reader["Category"].ToString();
                        task.Contractor = reader["Contractor"].ToString();
                        task.WorkDetail = reader["WorkDetail"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        // task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.HODApproval = reader["HODApproval"].ToString();
                        task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        task.FinalApproval = reader["FinalApproval"].ToString();
                        task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        task.Status = reader["Status"].ToString();
                        task.Priority = reader["Priority"].ToString();
                        task.FinalRemark = reader["FinalRemark"].ToString();
                        task.HODRemark = reader["HODRemark"].ToString();
                       // task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public int Contractor_Complaint_Insert(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_Complaint_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@Location", hld.Location);
                    cmd.Parameters.AddWithValue("@Category", hld.Category);
                    cmd.Parameters.AddWithValue("@WorkDetail", hld.WorkDetail);
                    cmd.Parameters.AddWithValue("@hodid", hld.hodid);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Contractor_Complaint_UpdatetFirstStep
        public int Contractor_Complaint_UpdatetFirstStep(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Complaint_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@Location", hld.Location);
                    cmd.Parameters.AddWithValue("@WorkDetail", hld.WorkDetail);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //ApprovalUpdateHOD
        public List<ContractorIndentMDI> GetHOD_SectionByCompany(int compcode = 0)
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_GetComplaint_HOD_Sec", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.hodname = reader["hodname"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.Category = reader["Category"].ToString();
                        task.Contractor = reader["Contractor"].ToString();
                        task.WorkDetail = reader["WorkDetail"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.HODApproval = reader["HODApproval"].ToString();
                        task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        task.FinalApproval = reader["FinalApproval"].ToString();
                        task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        task.Status = reader["Status"].ToString();
                        task.Priority = reader["Priority"].ToString();
                        task.FinalRemark = reader["FinalRemark"].ToString();
                        task.HODRemark = reader["HODRemark"].ToString();
                       // task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        task.FullName = reader["FullName"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.Doc = Convert.ToInt32(reader["Doc"]);

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public int ContractorApprovalHOD(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_HODApprovalSec]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@Priority", hld.Priority);
                    cmd.Parameters.AddWithValue("@HODRemark", hld.HODRemark);
                    cmd.Parameters.AddWithValue("@HODApproval", hld.HODApproval);
                    cmd.Parameters.AddWithValue("@AssignTo", hld.AssignTo);
                    cmd.Parameters.AddWithValue("@Category", hld.Category);
                    // cmd.Parameters.AddWithValue("@WorkRating", hld.WorkRating);
                    cmd.Parameters.AddWithValue("@HODApprovalBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public List<ContractorIndentMDI> GetAllStore_Section()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_GetComplaint_Store_Sec]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.hodname = reader["hodname"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.Category = reader["Category"].ToString();
                        task.Contractor = reader["Contractor"].ToString();
                        task.WorkDetail = reader["WorkDetail"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.HODApproval = reader["HODApproval"].ToString();
                        task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        task.FinalApproval = reader["FinalApproval"].ToString();
                        task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        task.Status = reader["Status"].ToString();
                        task.Priority = reader["Priority"].ToString();
                        task.FinalRemark = reader["FinalRemark"].ToString();
                        task.HODRemark = reader["HODRemark"].ToString();
                       // task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        task.Doc = Convert.ToInt32(reader["Doc"]);
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ContractorStoreApproval
        public int ContractorStoreApproval(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Store_Sec_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@Contractor", hld.Contractor);
                    cmd.Parameters.AddWithValue("@EstimateAmount", hld.EstimateAmount);
                    cmd.Parameters.AddWithValue("@WorkStartDate", hld.WorkStartDate);
                    cmd.Parameters.AddWithValue("@CompletionDate", hld.CompletionDate);
                    cmd.Parameters.AddWithValue("@FinalApproval", hld.FinalApproval);
                    cmd.Parameters.AddWithValue("@FinalApprovalBY", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@FinalRemark", hld.FinalRemark);
                    cmd.Parameters.AddWithValue("@Status", hld.Status);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //UserUpdateStatus
        public int ContractorUpdateStatus(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_UserStatusSecUpdate]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@Status", hld.Status);
                    //cmd.Parameters.AddWithValue("@HODRemark", hld.HODRemark);
                    //cmd.Parameters.AddWithValue("@HODApproval", hld.HODApproval);
                    //cmd.Parameters.AddWithValue("@AssignTo", hld.AssignTo);
                    //cmd.Parameters.AddWithValue("@Category", hld.Category);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //WorkRating update
        public int ContractorWorkRatingUpdate(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[ContractorUpdate_WorkRating]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@WorkRating", hld.WorkRating);
                    cmd.Parameters.AddWithValue("@Status", hld.Status);
                    cmd.Parameters.AddWithValue("@HODRemark", hld.HODRemark);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Civil Section
        public List<ContractorIndentMDI> GetAllCivil_Section()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_GetComplaint_Civil_Sec]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.hodname = reader["hodname"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.Category = reader["Category"].ToString();
                        task.Contractor = reader["Contractor"].ToString();
                        task.WorkDetail = reader["WorkDetail"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.HODApproval = reader["HODApproval"].ToString();
                        task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        task.FinalApproval = reader["FinalApproval"].ToString();
                        task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        task.Status = reader["Status"].ToString();
                        task.Priority = reader["Priority"].ToString();
                        task.FinalRemark = reader["FinalRemark"].ToString();
                        task.HODRemark = reader["HODRemark"].ToString();
                       // task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        task.Doc = Convert.ToInt32(reader["Doc"]);
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //Electrical Section
        public List<ContractorIndentMDI> GetAllElectrical_Section()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_GetComplaint_Electrical_Sec]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.hodname = reader["hodname"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.Category = reader["Category"].ToString();
                        task.Contractor = reader["Contractor"].ToString();
                        task.WorkDetail = reader["WorkDetail"].ToString();
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.HODApproval = reader["HODApproval"].ToString();
                        task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        task.FinalApproval = reader["FinalApproval"].ToString();
                        task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        task.Status = reader["Status"].ToString();
                        task.Priority = reader["Priority"].ToString();
                        task.FinalRemark = reader["FinalRemark"].ToString();
                        task.HODRemark = reader["HODRemark"].ToString();
                        task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        task.WorkRating = reader["WorkRating"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        task.Doc = Convert.ToInt32(reader["Doc"]);



                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int Contractor_QuatationUpload(ContractorIndentMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Quataion_Upload]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@FileName", hld.FileName);
                                   
                    cmd.Parameters.AddWithValue("@FileContent", hld.FileContent);
                   // cmd.Parameters.AddWithValue("@Doc",hld.Doc);
                    cmd.Parameters.AddWithValue("@UpdatedBy",objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //alert
        public string getEmailAddress()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Select_Mail]", conn))
                {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // con.Open();
                    // SqlCommand com = new SqlCommand("usp_get_emailset_for_alert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    //com.Parameters.AddWithValue("@eset", emailset);
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds, "tbl");
                }
                string addrs = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            addrs += ds.Tables[0].Rows[i]["email"].ToString() + ",";
                        }
                        addrs = addrs.Substring(0, addrs.Length - 1);
                    }
                }
                return addrs;
            }

        }
        public string getEmailAddressofOther()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_MailHodApprovalForCivilElecandStore_Email]", conn))
                {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // con.Open();
                    // SqlCommand com = new SqlCommand("usp_get_emailset_for_alert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    //com.Parameters.AddWithValue("@eset", emailset);
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds, "tbl");
                }
                string addrs = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            addrs += ds.Tables[0].Rows[i]["email"].ToString() + ",";
                        }
                        addrs = addrs.Substring(0, addrs.Length - 1);
                    }
                }
                return addrs;
            }

        }
    
        public string getEmailAddressofStatusChanged()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Mail_Work_Status_Changed]", conn))
                {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // con.Open();
                    // SqlCommand com = new SqlCommand("usp_get_emailset_for_alert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    //com.Parameters.AddWithValue("@eset", emailset);
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds, "tbl");
                }
                string addrs = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            addrs += ds.Tables[0].Rows[i]["email"].ToString() + ",";
                        }
                        addrs = addrs.Substring(0, addrs.Length - 1);
                    }
                }
                return addrs;
            }

        }
        public List<ContractorIndentMDI> GetHODApprovalForMail()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_MailHodApproval", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI
                        {
                            RecordId = Convert.ToInt32(rdr["RecordId"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            WorkDetail = rdr["WorkDetail"].ToString(),
                            CreatedBy = rdr["CreatedBy"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]).ToShortDateString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
    
        public List<ContractorIndentMDI> GetHODApprovalForMail_Other()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Contractor_MailHodApprovalForCivilElecandStore", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI
                        {
                            RecordId = Convert.ToInt32(rdr["RecordId"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            WorkDetail = rdr["WorkDetail"].ToString(),
                            HODApproval = rdr["HODApproval"].ToString(),
                            FinalApproval = rdr["FinalApproval"].ToString(),
                            HODApprovalBy = rdr["HODApprovalBy"].ToString(),
                            FullName = rdr["FullName"].ToString(),
                            HODApprovalDate = Convert.ToDateTime(rdr["HODApprovalDate"]).ToShortDateString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<ContractorIndentMDI> GetEmailChanedStatus()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Mail_Work_Status_Changed]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI
                        {
                            RecordId = Convert.ToInt32(rdr["RecordId"]),
                            cmpname = rdr["cmpname"].ToString(),
                            hodname = rdr["hodname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            WorkDetail = rdr["WorkDetail"].ToString(),
                            Status = rdr["Status"].ToString(),
                            UpdatedBy = rdr["UpdatedBy"].ToString(),
                            UpdatedOn = rdr["UpdatedOn"].ToString(),
                           // HODApprovalDate = Convert.ToDateTime(rdr["HODApprovalDate"]).ToShortDateString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }

        public List<ContractorIndentMDI> GetEmailChanedRating()
        {
            List<ContractorIndentMDI> tasklist = new List<ContractorIndentMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Mail_Work_Rating_Changed]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ContractorIndentMDI task = new ContractorIndentMDI
                        {
                            RecordId = Convert.ToInt32(rdr["RecordId"]),
                            cmpname = rdr["cmpname"].ToString(),
                            FullName = rdr["FullName"].ToString(),
                            Category = rdr["Category"].ToString(),
                            WorkDetail = rdr["WorkDetail"].ToString(),
                            Status = rdr["Status"].ToString(),
                            WorkRating = rdr["WorkRating"].ToString(),
                            UpdatedBy = rdr["UpdatedBy"].ToString(),
                            UpdatedOn = rdr["UpdatedOn"].ToString(),
                            // HODApprovalDate = Convert.ToDateTime(rdr["HODApprovalDate"]).ToShortDateString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public string getEmailAddressofRatingChanged()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Contractor_Mail_Work_Rating_Changed]", conn))
                {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // con.Open();
                    // SqlCommand com = new SqlCommand("usp_get_emailset_for_alert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    //com.Parameters.AddWithValue("@eset", emailset);
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds, "tbl");
                }
                string addrs = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            addrs += ds.Tables[0].Rows[i]["email"].ToString() + ",";
                        }
                        addrs = addrs.Substring(0, addrs.Length - 1);
                    }
                }
                return addrs;
            }

        }



    }
}