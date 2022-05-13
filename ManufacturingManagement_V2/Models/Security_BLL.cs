using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class Security_BLL
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();

        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ECode")]
        public int ECode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "EmpName")]
        public string EmpName { get; set; }

        //[Required(ErrorMessage = "Required!")]
        //[Display(Name = "Calibration Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        //public DateTime CalibDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Isactive")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "compcode")]
        public int compcode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "JoiningUnit")]
        public int JoiningUnit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "cmpname")]
        public string cmpname { get; set; }
        public List<Security_BLL> Item_List { get; set; }
       
        //Function
        public List<Security_BLL> Security_Get_All_EmpList()
        {
            List<Security_BLL> tasklist = new List<Security_BLL>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_GetAllList", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Security_BLL task = new Security_BLL();
                        task.ECode = Convert.ToInt32(reader["ECode"]);
                        task.EmpName = reader["EmpName"].ToString();
                        task.cmpname = reader["cmpname"].ToString();
                        task.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        //task.EstimateAmount = Convert.ToDecimal(reader["EstimateAmount"]);
                        //task.CompletionDate = string.IsNullOrEmpty(reader["CompletionDate"].ToString()) ? "" : Convert.ToDateTime(reader["CompletionDate"]).ToShortDateString();
                        //task.WorkStartDate = string.IsNullOrEmpty(reader["WorkStartDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkStartDate"]).ToShortDateString();
                        //task.WorkComplatedDate = string.IsNullOrEmpty(reader["WorkComplatedDate"].ToString()) ? "" : Convert.ToDateTime(reader["WorkComplatedDate"]).ToShortDateString();
                        //task.FinalApprovalDate = string.IsNullOrEmpty(reader["FinalApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["FinalApprovalDate"]).ToShortDateString();
                        //task.HODApprovalDate = string.IsNullOrEmpty(reader["HODApprovalDate"].ToString()) ? "" : Convert.ToDateTime(reader["HODApprovalDate"]).ToShortDateString();
                        //task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        //task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<Security_BLL> Security_Get_All_CompanyOR()
        {
            List<Security_BLL> tasklist = new List<Security_BLL>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_Get_CompanyOR", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Security_BLL task = new Security_BLL();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.cmpname = reader["cmpname"].ToString();
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }

        public List<Security_BLL> Security_Get_AllEmpList()
        {
            List<Security_BLL> tasklist = new List<Security_BLL>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_GetEMPList", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Security_BLL task = new Security_BLL(); 
                        task.ECode = Convert.ToInt32(reader["ECode"]);
                        task.EmpName = reader["EmpName"].ToString();
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }

        //insert
        public int Security_Insert(Security_BLL hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@EmpName", hld.EmpName);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                   
                   
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Update
        public int Security_Update(Security_BLL hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ECode", hld.ECode);
                   // cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@EmpName", hld.EmpName);
                   // cmd.Parameters.AddWithValue("@IsActive", hld.IsActive);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
       public int Security_Update_Company(Security_BLL hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_Update_Company", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ECode", hld.ECode);
                    // cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    // cmd.Parameters.AddWithValue("@IsActive", hld.IsActive);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        //Delete
        public int Security_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_Delete_Emp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ECode", id);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        public List<Security_BLL> Security_GetEmpId()
        {
            List<Security_BLL> tasklist = new List<Security_BLL>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Security_GetEmpId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Security_BLL task = new Security_BLL();
                        task.ECode = Convert.ToInt32(reader["ECode"]);
                                    
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }


    }
}