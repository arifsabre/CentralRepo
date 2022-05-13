using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class Trainning_Insert
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        [Key]
        public int RecordId { get; set; }
        public int Id { get; set; }
             
        public int compcode { get; set; }
        public string cmpname { get; set; }
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Topic Required")]
        [Display(Name = "Topic Name")]
        public int TopicId { get; set; }

        [Required(ErrorMessage = "Category Required")]
        [Display(Name = "Category Name")]
        public int LibCategoryId { get; set; }
        public string LibCategory { get; set; }

        [Required(ErrorMessage = "TotalPerson Required")]
        [Display(Name = "Total Person")]
        public int TotalPerson { get; set; }




        [Required(ErrorMessage = "Topic Required")]
        [Display(Name = "Topic Name")]
        public string Trainning_Topic { get; set; }

        [Required(ErrorMessage = "Trainner Required")]
        [Display(Name = "Trainner Name")]
        public string Trainner { get; set; }

        [Required(ErrorMessage = "Trainee Required")]
        [Display(Name = "Trainee Name")]
        public int NewEmpId { get; set; }


        [Required(ErrorMessage = "Trainee Name Required")]
        [Display(Name = "Trainee Name")]
        public string EmpName { get; set; }


        [Required(ErrorMessage = "Trainning Date Required")]
        [Display(Name = "Trainning Date")]
        public string TrainningDate { get; set; }


        [Required(ErrorMessage = "Evalution Date Required")]
        [Display(Name = "Evalution Date")]
        public string EvalutionDate { get; set; }

        [Required(ErrorMessage = "Evalution Rating Required")]
        [Display(Name = "Evalution Rating")]
        public string EvalutionRating { get; set; }

        public string CreatedBy { get; set; }
        public int UserId { get; set; }

        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }

        [Required(ErrorMessage = "FileName  Required")]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached File")]

        [Key]
        public List<HttpPostedFileBase> files { get; set; }

        public byte[] FileContent { get; set; }

        public List<Trainning_Insert> Item_List { get; set; }
        public List<Trainning_Insert> Item_List2 { get; set; }
        public List<Trainning_Insert> Item_List3 { get; set; }
        public List<Trainning_Insert> Item_List4 { get; set; }

        public string LibSubCategory { get; set; }

        [Required(ErrorMessage = "SubCategory is Required:")]
        public int LibSubCategoryId { get; set; }

        public List<Trainning_Insert> Orders { get; set; }
         
        public Trainning_Insert()
        {
            this.LibraryCategory = new List<SelectListItem>();
            this.LibrarySubCategory = new List<SelectListItem>();

        }
        public List<SelectListItem> LibraryCategory { get; set; }
        public List<SelectListItem> LibrarySubCategory { get; set; }
        //FuntionList
       internal DataSet getEmpDataByEmp(int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "Trainning__Get_TraineeList";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<Trainning_Insert> getEmpListByCompany(int compcode)
        {
            DataSet ds = getEmpDataByEmp(compcode);
            List<Trainning_Insert> companies = new List<Trainning_Insert> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new Trainning_Insert
                {
                    NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString()),
                    EmpName = dr["EmpName"].ToString()
                });
            }
            return companies;
        }


        //GetOwerList
        public List<Trainning_Insert> Trainning_GetTopic()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "Trainning_GetAllTopic";
            mc.fillFromDatabase(ds, cmd);
            List<Trainning_Insert> ls = new List<Trainning_Insert> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new Trainning_Insert
                {
                    TopicId = Convert.ToInt32(dr["TopicId"]),
                    Trainning_Topic = dr["Trainning_Topic"].ToString()
                });
            }
            return ls;
        }

        //ListAllFile
        public List<Trainning_Insert> GetAllTrainningList()
        {
            List<Trainning_Insert> tasklist = new List<Trainning_Insert>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("TrainningGetAll", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   //cmd.Parameters.AddWithValue("@TopicId", topicid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Trainning_Insert task = new Trainning_Insert();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.TopicId = Convert.ToInt32(reader["TopicId"]);
                        task.ShortName = reader["ShortName"].ToString();
                       
                        task.Trainning_Topic = reader["Trainning_Topic"].ToString();
                        task.Trainner = reader["Trainner"].ToString();
                        task.EmpName = reader["EmpName"].ToString();
                        task.TrainningDate = string.IsNullOrEmpty(reader["TrainningDate"].ToString()) ? "" : Convert.ToDateTime(reader["TrainningDate"]).ToShortDateString();
                        //task.EvalutionDate = Convert.ToDateTime(reader["EvalutionDate"]);
                        // task.EvalutionRating = reader["EvalutionRating"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        // task.FileName = reader["FileName"].ToString();
                        //task.OtherFile = reader["OtherFile"].ToString();
                        // task.SendDate = Convert.ToDateTime(reader["SendDate"]);
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                        //task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListByTopicId
        public List<Trainning_Insert> GetAllTrainningListByTopicId(int topicid=0)
        {
            List<Trainning_Insert> tasklist = new List<Trainning_Insert>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[TrainningGetAllByTopic]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TopicId", topicid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Trainning_Insert task = new Trainning_Insert();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.TopicId = Convert.ToInt32(reader["TopicId"]);
                        task.LibCategory = reader["LibCategory"].ToString();
                        task.LibSubCategory = reader["LibSubCategory"].ToString();
                        task.ShortName = reader["ShortName"].ToString();
                        // task.Id = Convert.ToInt32(reader["Id"]);
                        task.Trainning_Topic = reader["Trainning_Topic"].ToString();
                        task.Trainner = reader["Trainner"].ToString();
                        task.EmpName = reader["EmpName"].ToString();
                        task.TrainningDate = string.IsNullOrEmpty(reader["TrainningDate"].ToString()) ? "" : Convert.ToDateTime(reader["TrainningDate"]).ToShortDateString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                        //task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<Trainning_Insert> GetFileByRecordId(int id = 0)
        {
            List<Trainning_Insert> tasklist = new List<Trainning_Insert>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("TrainningGetAllBYFileId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Trainning_Insert task = new Trainning_Insert();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.Id = Convert.ToInt32(reader["Id"]);
                        task.FileName = reader["FileName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        public int Trainning_Insert_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Trainning_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", id);


                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Trainning_GroupDelete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Trainning_GroupDelete]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", id);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        // TrainningUpdate

        public int Trainning_TopicUpdate(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[TrainningInsert_TopicUpdateT]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@TopicId", hld.TopicId);

                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int Trainning_DateUpdate(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("TrainningInsert_TrainningDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@TrainningDate", hld.TrainningDate);
                    // cmd.Parameters.AddWithValue("@Trainner", hld.Trainner);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Trainning_TrainnerUpdate(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[TrainningInsert_TrainnerUpdate]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@TrainningDate", hld.TrainningDate);
                    cmd.Parameters.AddWithValue("@Trainner", hld.Trainner);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int Trainning_TrainneeUpdate(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[TrainningInsert_TrainneeUpdate]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@TrainningDate", hld.TrainningDate);
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Trainning_TopicSave(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Trainning_TopicSave", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@TrainningDate", hld.TrainningDate);
                    cmd.Parameters.AddWithValue("@Trainning_Topic", hld.Trainning_Topic);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Training_DetailsSave(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Trainning_Save", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@LibCategoryId", hld.LibCategoryId);
                    cmd.Parameters.AddWithValue("@TopicId", hld.TopicId);
                    cmd.Parameters.AddWithValue("@Trainner", hld.Trainner);
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@TrainningDate", hld.TrainningDate);
                    cmd.Parameters.AddWithValue("@LibSubCategoryId", hld.LibSubCategoryId);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        public List<Trainning_Insert> GetTrainningTopic()
        {
            List<Trainning_Insert> tasklist = new List<Trainning_Insert>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Trainning_GetTopicList]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Trainning_Insert task = new Trainning_Insert();
                        task.Trainning_Topic = reader["Trainning_Topic"].ToString();
                        task.TopicId = Convert.ToInt32(reader["TopicId"]);
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int Trainning_Deletetopic(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Trainning_Deletetopic", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TopicId", id);


                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Trainning_TopicMasterUpdate(Trainning_Insert hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[TrainningInsert_TopicUpdate]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TopicId", hld.TopicId);
                    cmd.Parameters.AddWithValue("@Trainning_Topic", hld.Trainning_Topic);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //ListAllFile
        public List<Trainning_Insert> GetAllTrainningGroup()
        {
            List<Trainning_Insert> tasklist = new List<Trainning_Insert>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Trainning_AllGroup]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Trainning_Insert task = new Trainning_Insert();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.TopicId = Convert.ToInt32(reader["TopicId"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.LibCategory = reader["LibCategory"].ToString();
                        task.LibSubCategory = reader["LibSubCategory"].ToString();
                        task.Trainning_Topic = reader["Trainning_Topic"].ToString();
                        task.Trainner = reader["Trainner"].ToString();
                        task.TrainningDate = string.IsNullOrEmpty(reader["TrainningDate"].ToString()) ? "" : Convert.ToDateTime(reader["TrainningDate"]).ToShortDateString();
                        //task.EvalutionDate = Convert.ToDateTime(reader["EvalutionDate"]);
                        // task.EvalutionRating = reader["EvalutionRating"].ToString();
                        task.TotalPerson = Convert.ToInt32(reader["TotalPerson"]);
                        task.CreatedBy = reader["CreatedBy"].ToString();
                       task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = Convert.ToDateTime(reader["CreatedOn"]);
                        //task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }






















    }

 
 

   
}