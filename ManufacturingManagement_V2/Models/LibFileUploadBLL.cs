using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class LibFileUploadBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();

        public List<LibFileUploadMDI> LibCategoryListDropdownList()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Lib_Get_CategoryDropDownList", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            LibCategoryId = Convert.ToInt32(reader["LibCategoryId"]),
                            LibCategory = reader["LibCategory"].ToString(),

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<LibFileUploadMDI> LibSubCategoryListDropdownList()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Lib_Get_CategoryDropDownList", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            LibSubCategoryId = Convert.ToInt32(reader["LibSubCategoryId"]),
                            LibSubCategory = reader["LibSubCategory"].ToString(),

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
    
        public List<LibFileUploadMDI> Lib_Get_CompanyDropDownList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "LibCompanyNameDropdownlist";
            mc.fillFromDatabase(ds, cmd);
            List<LibFileUploadMDI> ls = new List<LibFileUploadMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new LibFileUploadMDI
                {
                    compcode = Convert.ToInt32(dr["Compcode"]),
                    cmpname = dr["cmpname"].ToString()
                });
            }
            return ls;
        }
     
        public List<LibFileUploadMDI> LibraryGetOwnerNameList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "LibraryMasterGetOwnerDropDown";
            mc.fillFromDatabase(ds, cmd);
            List<LibFileUploadMDI> ls = new List<LibFileUploadMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new LibFileUploadMDI
                {
                    userid = Convert.ToInt32(dr["userid"]),
                    FullName = dr["FullName"].ToString()
                });
            }
            return ls;
        }
        //
       


        public List<LibFileUploadMDI> LibraryGetDepartmentNameList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "LibraryMasterGetDepartmnetDropDown";
            mc.fillFromDatabase(ds, cmd);
            List<LibFileUploadMDI> ls = new List<LibFileUploadMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new LibFileUploadMDI
                {
                    DepCode = dr["DepCode"].ToString(),
                    Department = dr["Department"].ToString()
                });
            }
            return ls;
        }
       public List<LibFileUploadMDI> LibGetAllFileDetail()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryGetAllFileDetail", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            Id = Convert.ToInt32(reader["Id"]),
                            LibCategory = reader["LibCategory"].ToString(),
                            LibSubCategory = reader["LibSubCategory"].ToString(),
                            Description = reader["Description"].ToString(),
                            FileName = reader["FileName"].ToString(),
                            username = reader["username"].ToString(),
                           

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<LibFileUploadMDI> LibraryMasterGetAllFileDetail(int userid = 0)
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryMasterGetAllFileByUser", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            cmpname = reader["cmpname"].ToString(),
                            DepCode = reader["DepCode"].ToString(),
                            LibCategory = reader["LibCategory"].ToString(),
                            LibSubCategory = reader["LibSubCategory"].ToString(),
                            //Purpose = reader["Purpose"].ToString(),
                            DuccumentName = reader["DuccumentName"].ToString(),
                            FileName = reader["FileName"].ToString(),
                            username = reader["username"].ToString(),
                            };

                        tasklist.Add(task);
                    }
                   
                }
            }
            return tasklist;
        }
        public int LibUpdateFile(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryFileUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                    cmd.Parameters.AddWithValue("@Description", libtask.Description);
                    //cmd.Parameters.AddWithValue("@LibCategoryId", libtask.LibCategoryId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //UpdateCategory
        public int LibUpdateCategory1(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryFileUpdateCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                   // cmd.Parameters.AddWithValue("@LibCategoryId", libtask.LibCategoryId);
                    cmd.Parameters.AddWithValue("@LibCategoryId",libtask.LibCategoryId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //UpdateSubcategory
        //UpdateCategory
        public int LibUpdateSubCategory(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryFileUpdateSubcategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                    cmd.Parameters.AddWithValue("@LibSubCategoryId", libtask.LibSubCategoryId);
                    //cmd.Parameters.AddWithValue("@LibCategoryId", libtask.LibCategoryId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //DeleteByProcedure
        public int LibDeleteProc(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryFileDeleteByRecordId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id",id);
                  //  cmd.Parameters.AddWithValue("@LibSubCategoryId", libtask.LibSubCategoryId);
                    //cmd.Parameters.AddWithValue("@LibCategoryId", libtask.LibCategoryId);
                   // cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }













        public int LibraryMasterUpdateFile(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryMasterUploadUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                    //cmd.Parameters.AddWithValue("@Purpose", libtask.Purpose);
                    cmd.Parameters.AddWithValue("@DuccumentName", libtask.DuccumentName);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int LibraryMasterDeleteFile(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM LibraryRecordInsert WHERE Id = @Id", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }






        public int LibraryDeleteFile(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM LibraryRecordInsert WHERE RecordId = @RecordId", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@RecordId", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }

        //Category
        public List<LibFileUploadMDI> GetAllLibCategory()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetAllCategory", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            LibCategoryId = Convert.ToInt32(reader["LibCategoryId"]),
                            LibCategory = reader["LibCategory"].ToString(),
                            //CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            //UpdatedOn = Convert.ToDateTime(reader["UpdatedOn"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }





        public int AddNewLibCategory(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibInsertCategory", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@LibCategory", libtask.LibCategory);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        public int Update_LibCategory(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibUpdateCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@LibCategoryId", hld.LibCategoryId);
                    cmd.Parameters.AddWithValue("@LibCategory", hld.LibCategory);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int DeleteLibCategory(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM LibraryCategory WHERE LibCategoryId = @LibCategoryId", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@LibCategoryId", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                return i;
            }
        }

        //SubCategory

        public List<LibFileUploadMDI> GetAllSubLibCategory()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetAllSubCategory", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            LibSubCategoryId = Convert.ToInt32(reader["LibSubCategoryId"]),
                            LibCategory = reader["LibCategory"].ToString(),
                            LibSubCategory = reader["LibSubCategory"].ToString(),
                           

                            //CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            //UpdatedOn = Convert.ToDateTime(reader["UpdatedOn"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int AddNewLibSubCategory(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibInsertSubCategory", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@LibSubCategory", libtask.LibSubCategory);
                    cmd.Parameters.AddWithValue("@LibCategoryId", libtask.LibCategoryId);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        public int Update_LibSubCategory(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibUpdateSubCategory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@LibSubCategoryId", hld.LibSubCategoryId);
                    cmd.Parameters.AddWithValue("@LibSubCategory", hld.LibSubCategory);
                    cmd.Parameters.AddWithValue("@LibCategoryId", hld.LibCategoryId);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int DeleteLibSubCategory(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM LibrarySubCategory WHERE LibSubCategoryId = @LibSubCategoryId", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@LibSubCategoryId", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }



        public List<LibFileUploadMDI> GetCategoryListDropdownList()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetAllCategoryDropDown", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            LibSubCategoryId = Convert.ToInt32(reader["LibCategoryId"]),
                            LibSubCategory = reader["LibCategory"].ToString(),

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //downloadRequestList
        public List<LibFileUploadMDI> LibraryDownloadRequestList()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryDownloadReq", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            ReqId = Convert.ToInt32(reader["ReqId"]),
                           //RecordId = Convert.ToInt32(reader["RecordId"]),
                            DownloadReason = reader["DownloadReason"].ToString(),
                            Approved = reader["Approved"].ToString(),
                            RequestOn = Convert.ToDateTime(reader["RequestOn"]),
                            username = reader["username"].ToString(),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //downloadRequestApproved
        public List<LibFileUploadMDI> LibraryDownloadRequestApproved()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryDownloadReq_Approved", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            ReqId = Convert.ToInt32(reader["ReqId"]),
                            //RecordId = Convert.ToInt32(reader["RecordId"]),
                            DownloadReason = reader["DownloadReason"].ToString(),
                            Approved = reader["Approved"].ToString(),
                            RequestOn = Convert.ToDateTime(reader["RequestOn"]),
                            username = reader["username"].ToString(),
                            //Approvedby = reader["Approvedby"].ToString(),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //downloadRequestReject
        public List<LibFileUploadMDI> LibraryDownloadRequestReject()
        {
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryDownloadReq_Rejected", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI
                        {
                            ReqId = Convert.ToInt32(reader["ReqId"]),
                            //RecordId = Convert.ToInt32(reader["RecordId"]),
                            DownloadReason = reader["DownloadReason"].ToString(),
                            Approved = reader["Approved"].ToString(),
                            RequestOn = Convert.ToDateTime(reader["RequestOn"]),
                            username = reader["username"].ToString(),
                            //Approvedby = reader["Approvedby"].ToString(),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //add
        public int LibraryDownloadRequestInsert(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryDowbloadRequestInsert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                    cmd.Parameters.AddWithValue("@DownloadReason", libtask.DownloadReason);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        //Update
        public int LibraryDownloadRequestUpdate(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryDowbloadRequestUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ReqId", libtask.ReqId);
                    cmd.Parameters.AddWithValue("@DownloadReason", libtask.DownloadReason);
                    //cmd.Parameters.AddWithValue("@Description", libtask.Description);
                    cmd.Parameters.AddWithValue("@Approved", libtask.Approved);
                    cmd.Parameters.AddWithValue("@ApprovedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //delete
        public int LibraryDownloadDelete(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM LibraryReqDownload WHERE ReqId = @ReqId", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@ReqId", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }






    }
}