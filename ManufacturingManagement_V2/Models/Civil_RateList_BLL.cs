using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace ManufacturingManagement_V2.Models
{
    public class Civil_RateList_BLL
    {
        internal string Message { get; set; }
        internal bool Result { get; set; }
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPConnection"].ToString());
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
 
        public List<CiviRateMDI> GetAllCivil_RateList()
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_GetAllRate]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        task.Category = reader["Category"].ToString();
                        task.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]);
                        task.SubCategory = reader["SubCategory"].ToString();
                        task.Description = reader["Description"].ToString();
                        task.UnitId = Convert.ToInt32(reader["UnitId"]);
                        task.Unit = reader["Unit"].ToString();
                        task.Rate = Convert.ToDecimal(reader["Rate"]);
                        task.Amount = Convert.ToDecimal(reader["Amount"]);
                        task.Remark = reader["Remark"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<CiviRateMDI> GetCivil_RateByChapterName(int id=0)
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilRate_Index]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubCategoryId",id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        task.Category = reader["Category"].ToString();
                        task.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]);
                        task.SubCategory = reader["SubCategory"].ToString();
                        task.Description = reader["Description"].ToString();
                        task.UnitId = Convert.ToInt32(reader["UnitId"]);
                        task.Unit = reader["Unit"].ToString();
                        task.Rate = Convert.ToDecimal(reader["Rate"]);
                        task.Amount = Convert.ToDecimal(reader["Amount"]);
                        task.Remark = reader["Remark"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<CiviRateMDI> GetAllChapterList()
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilRate_ChapterList]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.Category = reader["Category"].ToString();
                        task.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]);
                        task.SubCategory = reader["SubCategory"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int CivilRate_Insert(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("CivilRate_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CategoryId", hld.CategoryId);
                    cmd.Parameters.AddWithValue("@SubCategoryId", hld.SubCategoryId);
                    cmd.Parameters.AddWithValue("@Description", hld.Description);
                    cmd.Parameters.AddWithValue("@Rate", hld.Rate);
                   // cmd.Parameters.AddWithValue("@Amount", hld.Amount);
                    cmd.Parameters.AddWithValue("@UnitId", hld.UnitId);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        public int CivilRate_Update(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilRate_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    // cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@CategoryId", hld.CategoryId);
                    // cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@SubCategoryId", hld.SubCategoryId);
                    cmd.Parameters.AddWithValue("@Description", hld.Description);
                    cmd.Parameters.AddWithValue("@Rate", hld.Rate);
                    cmd.Parameters.AddWithValue("@Amount", hld.Amount);
                    cmd.Parameters.AddWithValue("@UnitId", hld.UnitId);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserId());
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int CivilRate_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilRate_Delete]", conn))
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

        public int Category_Insert(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilCategory_Insert]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Category", hld.Category);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int Category_Update(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilCategory_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CategoryId", hld.CategoryId);
                    cmd.Parameters.AddWithValue("@Category", hld.Category);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserId());
                   i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int Category_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[CivilCategory_Delete]", conn))
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


        public int SubCategory_Insert(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_SubCategory_Insert]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CategoryId", hld.CategoryId);
                    cmd.Parameters.AddWithValue("@SubCategory", hld.SubCategory);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int SubCategory_Update(CiviRateMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_SubCategory_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@SubCategoryId", hld.SubCategoryId);
                    cmd.Parameters.AddWithValue("@SubCategory", hld.SubCategory);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int SubCategory_Delete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_SubCategory_Delete]", conn))
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



        public List<CiviRateMDI> GetAllCategoryList()
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_GetAllCategory]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        task.Category = reader["Category"].ToString();
                       
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        public List<CiviRateMDI> GetAllSubCategoryList()
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Civil_GetAllSubCategory]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]);
                        task.SubCategory = reader["SubCategory"].ToString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        public List<CiviRateMDI> GetAllUnitListList()
        {
            List<CiviRateMDI> tasklist = new List<CiviRateMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Civil_GetAllUnit", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CiviRateMDI task = new CiviRateMDI();
                        task.UnitId = Convert.ToInt32(reader["UnitId"]);
                        task.Unit = reader["Unit"].ToString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

    }
}