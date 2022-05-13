using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MasterLibraryBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        

    public List<LibFileUploadMDI>MasterLibraryGetAllFileDetail(int userid)
    {
            clsCookie objCookie = new clsCookie();
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
        {
            using (SqlCommand cmd = new SqlCommand("MasterLibraryGetAllFileByUser", conn))
            {
                  
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                conn.Open();
                cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                        LibFileUploadMDI task = new LibFileUploadMDI();
                    {

                            task.RecordId = Convert.ToInt32(reader["RecordId"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.DepCode = reader["DepCode"].ToString();
                            task.DoccumentName = reader["DoccumentName"].ToString();
                            task.Location = reader["Location"].ToString();
                            task.Keyss = reader["Keyss"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                            task.Status = reader["Status"].ToString();
                            task.CreatedBy = reader["CreatedBy"].ToString();
                           task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                            task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                            task.IssueToName = reader["IssueToName"].ToString();
                            task.ReturnStatus = reader["ReturnStatus"].ToString();
                           // task.Type1 = reader["Type1"].ToString();

                        };

                    tasklist.Add(task);
                }

            }
        }
        return tasklist;
    }

        public List<LibFileUploadMDI> MasterLibraryGetAllFile()
        {
            clsCookie objCookie = new clsCookie();
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibraryGetAllFile", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI();
                        {

                            task.RecordId = Convert.ToInt32(reader["RecordId"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.DepCode = reader["DepCode"].ToString();
                            task.DoccumentName = reader["DoccumentName"].ToString();
                            task.Location = reader["Location"].ToString();
                            task.Keyss = reader["Keyss"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                            task.Status = reader["Status"].ToString();
                            task.CreatedBy = reader["CreatedBy"].ToString();
                            task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                            task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                            task.IssueToName = reader["IssueToName"].ToString();
                            task.ReturnStatus = reader["ReturnStatus"].ToString();
                            // task.Type1 = reader["Type1"].ToString();

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //AddRecord
        //UpdateMasterLibrary

        public int MasterLibraryAddRecord(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibraryMasterUploadInsert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@DepCode", libtask.DepCode);
                    cmd.Parameters.AddWithValue("@DoccumentName", libtask.DoccumentName);
                    cmd.Parameters.AddWithValue("@Type", libtask.Type);
                    cmd.Parameters.AddWithValue("@Keyss", libtask.Keyss);
                    cmd.Parameters.AddWithValue("@Location", libtask.Location);
                    cmd.Parameters.AddWithValue("@Status", libtask.Status);
                    cmd.Parameters.AddWithValue("@NewEmpId", libtask.NewEmpId);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                   // cmd.Parameters.AddWithValue("@Type1", libtask.Type1);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        //UpdateMasterLibrary

        public int MasterLibraryUpdate(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibraryUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                    cmd.Parameters.AddWithValue("@DoccumentName", libtask.DoccumentName);
                    cmd.Parameters.AddWithValue("@Keyss", libtask.Keyss);
                    cmd.Parameters.AddWithValue("@Location", libtask.Location);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                   // cmd.Parameters.AddWithValue("@Type1", libtask.Type1);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int MasterLibraryDeleteFile(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM MasterLibraryFileUpload WHERE RecordId = @RecordId", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@RecordId", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                return i;
            }
        }
        //IssueTo
        public int IssueToFile(LibFileUploadMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibrary-IssueTo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", libtask.RecordId);
                  //  cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@DoccumentName", libtask.DoccumentName);
                    cmd.Parameters.AddWithValue("@Location", libtask.Location);
                   // cmd.Parameters.AddWithValue("@Status", libtask.Status);
                    cmd.Parameters.AddWithValue("@IssueDate", libtask.IssueDate);
                    cmd.Parameters.AddWithValue("@UserId",libtask.userid);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //GetOwerList
        public List<LibFileUploadMDI> MasterLibraryGetOwnerList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "MasterLibraryGetOwerList";
            mc.fillFromDatabase(ds, cmd);
            List<LibFileUploadMDI> ls = new List<LibFileUploadMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new LibFileUploadMDI
                {
                    NewEmpId = Convert.ToInt32(dr["NewEmpId"]),
                    EmpName = dr["EmpName"].ToString()
                });
            }
            return ls;
        }

        //GetIssueTo

        public List<LibFileUploadMDI> MasterLibraryGetIssueTo()
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
        //IssueToList
        public List<LibFileUploadMDI> MasterLibraryGetIssueToListByUser(int userid)
        {
            clsCookie objCookie = new clsCookie();
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibraryGetAllFileByUserIssueTo", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI();
                        {

                            task.RecordId = Convert.ToInt32(reader["RecordId"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.DepCode = reader["DepCode"].ToString();
                            task.DoccumentName = reader["DoccumentName"].ToString();
                            task.Location = reader["Location"].ToString();
                            task.Keyss = reader["Keyss"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                            task.Status = reader["Status"].ToString();
                            task.CreatedBy = reader["CreatedBy"].ToString();
                            task.CreatedOn =   string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy =   reader["UpdatedBy"].ToString();
                            task.UpdatedOn =   string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                          //  task.IssueToName = reader["IssueToName"].ToString();
                            task.IssueDate =   string.IsNullOrEmpty(reader["IssueDate"].ToString()) ? "" : Convert.ToDateTime(reader["IssueDate"]).ToShortDateString();
                            task.FullName = reader["FullName"].ToString();
                           // task.Type1 = reader["Type1"].ToString();

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //RecieveBy
        public List<LibFileUploadMDI> FileRecieveByUser(int userid)
        {
            clsCookie objCookie = new clsCookie();
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibraryGetAllFileByUserRecieveTo", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI();
                        {

                            task.RecordId = Convert.ToInt32(reader["RecordId"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.DepCode = reader["DepCode"].ToString();
                            task.DoccumentName = reader["DoccumentName"].ToString();
                            task.Location = reader["Location"].ToString();
                            task.Keyss = reader["Keyss"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                            task.Status = reader["Status"].ToString();
                            task.CreatedBy = reader["CreatedBy"].ToString();
                            task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                            task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                           
                            task.IssueDate = string.IsNullOrEmpty(reader["IssueDate"].ToString()) ? "" : Convert.ToDateTime(reader["IssueDate"]).ToShortDateString();
                            task.FullName = reader["FullName"].ToString();
                            task.ReturnStatus = reader["ReturnStatus"].ToString();
                           // task.Type1 = reader["Type1"].ToString();

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        //CompanyUpdate
        public int MasterLibraryCompanyUpdate(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibrary_CompanyUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //returnUpdate
     
        public int MasterLibraryReturnTo(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibrary-ReturnTo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@DoccumentName", hld.DoccumentName);
                    cmd.Parameters.AddWithValue("@ReturnTo", hld.CreatedBy);
                    cmd.Parameters.AddWithValue("@ReturnDate", hld.ReturnDate).ToString();
                    cmd.Parameters.AddWithValue("@ReturnBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                   // cmd.Parameters.AddWithValue("@Id", hld.Id);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //returnConfirmation

        public int MasterLibraryReturnConfirmation(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibrary-Confirmtoreturn", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    //cmd.Parameters.AddWithValue("@Status", hld.Status);
                   // cmd.Parameters.AddWithValue("@ReturnStatus", hld.ReturnStatus);
                   // cmd.Parameters.AddWithValue("@DoccumentName", hld.DoccumentName);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    // cmd.Parameters.AddWithValue("@Id", hld.Id);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }


        //OwnerUpdate
        public int MasterLibraryOwnerUpdate(LibFileUploadMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibrary_OwnerUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //ReturnToConfirm
        public List<LibFileUploadMDI> FileReturnToconfirmByUser(int userid)
        {
            clsCookie objCookie = new clsCookie();
            List<LibFileUploadMDI> tasklist = new List<LibFileUploadMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MasterLibraryGetFile-ReturnConfirmationList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibFileUploadMDI task = new LibFileUploadMDI();
                        {

                            task.RecordId = Convert.ToInt32(reader["RecordId"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.DepCode = reader["DepCode"].ToString();
                            task.DoccumentName = reader["DoccumentName"].ToString();
                            task.Location = reader["Location"].ToString();
                            task.Keyss = reader["Keyss"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                            task.Status = reader["Status"].ToString();
                            task.CreatedBy = reader["CreatedBy"].ToString();
                            task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                            task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();

                          //  task.IssueDate = string.IsNullOrEmpty(reader["IssueDate"].ToString()) ? "" : Convert.ToDateTime(reader["IssueDate"]).ToShortDateString();
                           // task.IssueToName = reader["FullName"].ToString();
                            task.ReturnStatus = reader["ReturnStatus"].ToString();
                            //task.ReturnTo = reader["ReturnTo"].ToString();

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

    }
}