using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Models
{
    public class BioEnroll_DataMDI
    {
        public int Id { get; set; }
        public string EmpId { get; set; }

        public string EmpName { get; set; }
        public int NewEmpId { get; set; }
        public int EnrollId { get; set; }
        public int Thump { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Biometric No")]
        public int BioId { get; set; }

        public DateTime UpdatedOn { get; set; }
        public List<BioEnroll_DataMDI> Item_List { get; set; }
        public List<BioEnroll_DataMDI> GetAll_BioEnrolled()
        {
            List<BioEnroll_DataMDI> tasklist = new List<BioEnroll_DataMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_GetBioE_EnrollData]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@userid", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        BioEnroll_DataMDI task = new BioEnroll_DataMDI();
                        task.EmpId = reader["EmpId"].ToString();
                        task.EmpName = reader["EmpName"].ToString();
                        task.NewEmpId = Convert.ToInt32(reader["NewEmpId"]);
                        task.BioId = Convert.ToInt32(reader["BioId"]);
                        task.Thump = Convert.ToInt32(reader["Thump"]);
                        task.UpdatedOn = Convert.ToDateTime(reader["UpdatedOn"]);


                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<BioEnroll_DataMDI> GetAll_NotMapBioId()
        {
            List<BioEnroll_DataMDI> tasklist = new List<BioEnroll_DataMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_GetBio_NotMapWithBio]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@userid", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        BioEnroll_DataMDI task = new BioEnroll_DataMDI();
                        task.EmpId = reader["EmpId"].ToString();
                        task.EmpName = reader["EmpName"].ToString();
                        task.NewEmpId = Convert.ToInt32(reader["NewEmpId"]);
                        task.BioId = Convert.ToInt32(reader["BioId"]);
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int MapBioIdToERP(BioEnroll_DataMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_MapBioId_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NewEmpId", libtask.NewEmpId);
                   cmd.Parameters.AddWithValue("@BioId", libtask.BioId);
                  i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

    }
}