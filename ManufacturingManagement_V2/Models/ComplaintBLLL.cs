using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ManufacturingManagement_V2.Models
{
    public class ComplaintBLLL
    {

        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPConnection"].ToString());
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();

        public List<ComplaintMDI> CoplaintGet_AllCoplaint()
        {
            
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[CoplaintGetComplaintNo]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                       });
                }
                return HList;
            }
        }
        public List<ComplaintMDI> CoplaintGet_AssignToDetail()
        {

            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[CoplaintGetAssignToDetail]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                             Reference = Convert.ToInt32(rdr["Reference"]),
                             EmpName = rdr["EmpName"].ToString(),
                             //CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                             UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),
                        //Category = rdr["Item_Name"].ToString(),
                        // Description = rdr["Serial_No"].ToString(),
                        //Status = rdr["Serial_No"].ToString(),
                        //Priority = rdr["Serial_No"].ToString(),
                        //Reark = rdr["Serial_No"].ToString(),
                         username = rdr["username"].ToString(),



                    });
                }
                return HList;
            }
        }

        //Display on Indexpage
        public List<ComplaintMDI> CoplaintGet_All( int compcode)
        {
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintGetAllCop", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                        cmpname = rdr["cmpname"].ToString(),
                        Category = rdr["Category"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Status = rdr["Status"].ToString(),
                       // EmpName = rdr["EmpName"].ToString(),
                        Priority = rdr["Priority"].ToString(),
                        Reark = rdr["Reark"].ToString(),
                        username = rdr["username"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                        Subject = rdr["Subject"].ToString(),
                        hodname = rdr["hodname"].ToString(),

                        // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),

                    });
                }
                return HList;
            }
        }
        public List<ComplaintMDI> CoplaintGetRecordForAssignTo(int compcode)
        {
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
              
                SqlCommand com = new SqlCommand("CoplaintGetAllAssignTo", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                        cmpname = rdr["cmpname"].ToString(),
                        Category = rdr["Category"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Status = rdr["Status"].ToString(),
                       
                        Priority = rdr["Priority"].ToString(),
                        Reark = rdr["Reark"].ToString(),
                        username = rdr["username"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                        Subject = rdr["Subject"].ToString(),
                        hodname = rdr["hodname"].ToString(),

                        // UpdatedBy = Convert.ToInt32(rdr["UpdatedBy"]),
                        // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),

                    });
                }
                return HList;
            }
        }


        public List<ComplaintMDI> CoplaintGetRecordForClosed( int compcode)
        {

            string cs = mc.strconn;
            clsCookie objCookie = new clsCookie();
            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintGetAllClosedTo", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                        cmpname = rdr["cmpname"].ToString(),
                        Category = rdr["Category"].ToString(),
                        hodname = rdr["hodname"].ToString(),
                        HODApproved = rdr["HODApproved"].ToString(),
                        FinalApproved = rdr["FinalApproved"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Status = rdr["Status"].ToString(),
                        Priority = rdr["Priority"].ToString(),
                        Reark = rdr["Reark"].ToString(),
                        ApprovedBy = rdr["ApprovedBy"].ToString(),
                        UpdatedBy = rdr["UpdatedBy"].ToString(),
                        username = rdr["username"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                        

                    });
                }
                return HList;
            }
        }
       public int InsertNewCoplaint(ComplaintMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintInsert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Subject", hld.Subject);
                com.Parameters.AddWithValue("@Compcode", hld.Compcode);
                com.Parameters.AddWithValue("@hodid", hld.hodid);
                com.Parameters.AddWithValue("@CatId", hld.CatId);
                com.Parameters.AddWithValue("@Description", hld.Description);
                com.Parameters.AddWithValue("@PID", hld.PID);
                com.Parameters.AddWithValue("@SID", hld.SID);
                com.Parameters.AddWithValue("@Reark", hld.Reark);
              // com.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                com.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());


                i = com.ExecuteNonQuery();
            }

            return i;

        }

        public int CoplaintUpdate(ComplaintMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintUpdate", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@Reference", hld.Reference);
                com.Parameters.AddWithValue("@Compcode", hld.Compcode);
                com.Parameters.AddWithValue("@Subject", hld.Subject);
                com.Parameters.AddWithValue("@hodid", hld.hodid);
                com.Parameters.AddWithValue("@CatId", hld.CatId);
                com.Parameters.AddWithValue("@Description", hld.Description);
                com.Parameters.AddWithValue("@PID", hld.PID);
                com.Parameters.AddWithValue("@SId", hld.SID);
                com.Parameters.AddWithValue("@Reark", hld.Reark);
              //  com.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                //com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                com.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                i = com.ExecuteNonQuery();
            }
            return i;
        }


        public int CoplaintAssignTo(ComplaintMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
               // com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlCommand com = new SqlCommand("CoplaintUpdateForAssignTo", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                com.Parameters.AddWithValue("@Reference", hld.Reference);
                // com.Parameters.AddWithValue("@Compcode", hld.Compcode);
                // com.Parameters.AddWithValue("@CatId", hld.CatId);

                com.Parameters.AddWithValue("@Subject", hld.Subject);
                com.Parameters.AddWithValue("@Description", hld.Description);
               // com.Parameters.AddWithValue("@PID", hld.PID);
                com.Parameters.AddWithValue("@SId", hld.SID);
                com.Parameters.AddWithValue("@Reark", hld.Reark);
               // com.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                //com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                com.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int CoplaintClosedTo(ComplaintMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[CoplaintUpdateForClosedTo]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
              
                com.Parameters.AddWithValue("@Reference", hld.Reference);
            
                com.Parameters.AddWithValue("@Description", hld.Description);
             
                com.Parameters.AddWithValue("@SId", hld.SID);
                com.Parameters.AddWithValue("@Reark", hld.Reark);
              
                com.Parameters.AddWithValue("@ClosedBy", objCookie.getUserName());
                i = com.ExecuteNonQuery();
            }
            return i;
        }


        public int DeleteCoplaint(int id)
        {

            SqlCommand cmd = new SqlCommand("DELETE FROM Coplaint WHERE Reference = @Reference", con)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Reference", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
        //dashboard
        public List<ComplaintMDI> ComplaintGetNotStarted(int compcode)
        {
            List<ComplaintMDI> tasklist = new List<ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ComplantNotStared", conn))
                {

                   cmd.CommandType = System.Data.CommandType.StoredProcedure;
                   conn.Open();
                   cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ComplaintMDI task = new ComplaintMDI
                        {
                            Reference = Convert.ToInt32(rdr["Reference"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            Priority = rdr["Priority"].ToString(),
                            Status = rdr["Status"].ToString(),
                            Description = rdr["Description"].ToString(),
                            Reark = rdr["Reark"].ToString(),
                           // EmpName = rdr["EmpName"].ToString(),
                            username = rdr["username"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                            hodname = rdr["hodname"].ToString(),
                            Subject = rdr["Subject"].ToString(),
                            // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        public List<ComplaintMDI> ComplaintGetInprogress()
        {
            List<ComplaintMDI> tasklist = new List<ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ComplantInProgress", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ComplaintMDI task = new ComplaintMDI
                        {
                            Reference = Convert.ToInt32(rdr["Reference"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            Priority = rdr["Priority"].ToString(),
                            Status = rdr["Status"].ToString(),
                            Description = rdr["Description"].ToString(),
                            Reark = rdr["Reark"].ToString(),
                            EmpName = rdr["EmpName"].ToString(),
                            username = rdr["username"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                            // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

      
        public List<ComplaintMDI> ComplaintGetCompleted()
        {
            List<ComplaintMDI> tasklist = new List<ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ComplantCompleted", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ComplaintMDI task = new ComplaintMDI
                        {
                            Reference = Convert.ToInt32(rdr["Reference"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            Priority = rdr["Priority"].ToString(),
                            Status = rdr["Status"].ToString(),
                            Description = rdr["Description"].ToString(),
                            Reark = rdr["Reark"].ToString(),
                            EmpName = rdr["EmpName"].ToString(),
                            username = rdr["username"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                            UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<ComplaintMDI> ComplaintGetClosed()
        {
            List<ComplaintMDI> tasklist = new List<ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ComplantClosed", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ComplaintMDI task = new ComplaintMDI
                        {
                            Reference = Convert.ToInt32(rdr["Reference"]),
                            cmpname = rdr["cmpname"].ToString(),
                            Category = rdr["Category"].ToString(),
                            Priority = rdr["Priority"].ToString(),
                            Status = rdr["Status"].ToString(),
                            Description = rdr["Description"].ToString(),
                            Reark = rdr["Reark"].ToString(),
                             EmpName = rdr["EmpName"].ToString(),
                            username = rdr["username"].ToString(),
                            CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                             UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),
                            ClosedDate = Convert.ToDateTime(rdr["ClosedDate"]),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //HODList
        public List<ComplaintMDI> CoplaintHODListByComapny()
        {
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand com = new SqlCommand("[Contractor_GetHODList]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
               // com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        hodid = Convert.ToInt32(rdr["hodid"]),
                        hodname = rdr["hodname"].ToString(),
                       
                    });
                }
                return HList;
            }
        }

        //Display on Approval Index Page
        public List<ComplaintMDI>ApprovalGet_All()
        {
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintGetAllCop_FinalApproval", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                        cmpname = rdr["cmpname"].ToString(),
                        Category = rdr["Category"].ToString(),
                        Subject = rdr["Subject"].ToString(),
                        hodname = rdr["hodname"].ToString(),
                        HODApproved = rdr["HODApproved"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Status = rdr["Status"].ToString(),
                        Priority = rdr["Priority"].ToString(),
                        Reark = rdr["Reark"].ToString(),
                        //username = rdr["username"].ToString(),
                        UpdatedBy = rdr["UpdatedBy"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"])

                       

                        // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),

                    });
                }
                return HList;
            }
        }
        //ApprovalUpdate
        public int ApprovalUpdate(ComplaintMDI hld)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintApprovalUpdate", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@Reference", hld.Reference);
               // com.Parameters.AddWithValue("@Compcode", hld.Compcode);
               com.Parameters.AddWithValue("@Subject", hld.Subject);
               // com.Parameters.AddWithValue("@hodid", hld.hodid);
               // com.Parameters.AddWithValue("@CatId", hld.CatId);
                com.Parameters.AddWithValue("@Description", hld.Description);
               // com.Parameters.AddWithValue("@PID", hld.PID);
                com.Parameters.AddWithValue("@SId", hld.SID);
                com.Parameters.AddWithValue("@Reark", hld.Reark);
                //  com.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                //com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                com.Parameters.AddWithValue("@ApprovedBy", objCookie.getUserName());
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        ////Display on BothApprovalApproval Index Page
        public List<ComplaintMDI> ApprovalGet_AllApprovedTaskk()
        {
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;

            List<ComplaintMDI> HList = new List<ComplaintMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("CoplaintGetAllCop_FinalApprovalList", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                //com.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new ComplaintMDI
                    {
                        Reference = Convert.ToInt32(rdr["Reference"]),
                        cmpname = rdr["cmpname"].ToString(),
                        Category = rdr["Category"].ToString(),
                        Subject = rdr["Subject"].ToString(),
                        hodname = rdr["hodname"].ToString(),
                        HODApproved = rdr["HodApproved"].ToString(),
                        FinalApproved = rdr["FinalApproved"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Status = rdr["Status"].ToString(),
                      
                        Priority = rdr["Priority"].ToString(),
                        Reark = rdr["Reark"].ToString(),
                       username = rdr["username"].ToString(),
                        UpdatedBy = rdr["UpdatedBy"].ToString(),
                        ApprovedBy = rdr["ApprovedBy"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"])



                        // UpdatedOn2 = Convert.ToDateTime(rdr["UpdatedOn2"]),

                    });
                }
                return HList;
            }
        }
    }
}