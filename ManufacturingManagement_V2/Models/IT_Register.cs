using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class IT_Register
    {
        clsCookie objCookie = new clsCookie();
        public int Id { get; set; }
        public string EmpName { get; set; }
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Company?")]
        [Display(Name ="Company")]
        public int compcode { get; set; }
        public string cmpname { get; set; }

        [Required(ErrorMessage = "Location?")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Assign To?")]
        [Display(Name = "Assign To")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "Uses?")]
        [Display(Name = "Uses")]
        public string Uses { get; set; }

        [Required(ErrorMessage = "DeviceName?")]
        [Display(Name = "DeviceName")]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "SerialNumber?")]
        [Display(Name = "SerialNumber")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "OSVersion?")]
        [Display(Name = "OSVersion")]
        public string OSVersion { get; set; }

        [Required(ErrorMessage = "LabelName?")]
        [Display(Name = "LabelName")]
        public string LabelName { get; set; }

        [Required(ErrorMessage = "Remark?")]
        [Display(Name = "Remark")]
        public string Remark { get; set; }

        public int  UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public List<IT_Register> Item_List { get; set; }
        public List<SelectListItem> tbl_Company { get; set; }


        //Function List

        public List<IT_Register> IT_Register_GetAll_Record()
        {
            List<IT_Register> tasklist = new List<IT_Register>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[IT_Register_GetAllRecord]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@userid", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                    
                        IT_Register task = new IT_Register();
                        task.Id = Convert.ToInt32(reader["Id"]);
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.cmpname = reader["cmpname"].ToString();
                        task.ShortName = reader["ShortName"].ToString();
                        task.Location = reader["Location"].ToString();
                        task.NewEmpId = Convert.ToInt32(reader["NewEmpId"]);
                        task.EmpName = reader["EmpName"].ToString();
                        task.Uses = reader["Uses"].ToString();
                        task.DeviceName = reader["DeviceName"].ToString();
                        task.SerialNumber = reader["SerialNumber"].ToString();
                        task.OSVersion = reader["OSVersion"].ToString();
                        task.LabelName =  reader["LabelName"].ToString();
                        task.Remark = reader["Remark"].ToString();
                        //task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        //task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        //// task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        //task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        //task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        //task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        //task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        //task.Remark = reader["Remark"].ToString();
                        //task.CreatedBy = reader["CreatedBy"].ToString();
                        //task.UpdatedBy = reader["UpdatedBy"].ToString();
                        //task.HODApproval = reader["HODApproval"].ToString();
                        //task.HODApprovalBy = reader["HODApprovalBy"].ToString();
                        //task.FinalApproval = reader["FinalApproval"].ToString();
                        //task.FinalApprovalBY = reader["FinalApprovalBY"].ToString();
                        //task.Status = reader["Status"].ToString();
                        //task.Priority = reader["Priority"].ToString();
                        //task.FinalRemark = reader["FinalRemark"].ToString();
                        //task.HODRemark = reader["HODRemark"].ToString();
                        //// task.AssignTo = Convert.ToInt32(reader["AssignTo"]);
                        //task.WorkRating = reader["WorkRating"].ToString();
                        //task.FullName = reader["FullName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int IT_Register_Insert(IT_Register hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[IT_Register_Insert]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@Location", hld.Location);
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@Uses", hld.Uses);
                    cmd.Parameters.AddWithValue("@DeviceName", hld.DeviceName);
                    cmd.Parameters.AddWithValue("@SerialNumber", hld.SerialNumber);
                    cmd.Parameters.AddWithValue("@OSVersion", hld.OSVersion);
                    cmd.Parameters.AddWithValue("@LabelName", hld.LabelName);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int IT_Register_Update(IT_Register hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[IT_Register_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", hld.Id);
                     cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@Location", hld.Location);
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@Uses", hld.Uses);
                    cmd.Parameters.AddWithValue("@DeviceName", hld.DeviceName);
                    cmd.Parameters.AddWithValue("@SerialNumber", hld.SerialNumber);
                    cmd.Parameters.AddWithValue("@OSVersion", hld.OSVersion);
                    cmd.Parameters.AddWithValue("@LabelName", hld.LabelName);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int IT_Register_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[IT_Register_Delete]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", id);
                    //cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    //cmd.Parameters.AddWithValue("@Category", hld.Category);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
    }
}