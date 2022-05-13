using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class HRHomeBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        public List<EmployeeMdl> Employee_AllActiveList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ZZZ_AllActiveEmpList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            FatherName = reader["FatherName"].ToString(),
                            Grade = reader["Grade"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                            ContactNo = reader["ContactNo"].ToString(),
                            GrossSalary = Convert.ToDouble(reader["GrossSalary"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        public List<EmployeeMdl> Employee_PresentList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_Present", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            PresentDate = Convert.ToDateTime(reader["PresentDate"]),
                            //Grade = reader["Grade"].ToString(),
                            //BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            //JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                            //ContactNo = reader["ContactNo"].ToString(),
                            //GrossSalary = Convert.ToDouble(reader["GrossSalary"]),
                            //IsActive = Convert.ToBoolean(reader["IsActive"]),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        public List<EmployeeMdl> Employee_AbsentList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_Absent", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            AbsentDate = Convert.ToDateTime(reader["AbsentDate"]),
                            


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        //LateEmp
        public List<EmployeeMdl> Employee_LateList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_Late", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            EDate = Convert.ToDateTime(reader["EDate"]),
                            ETime = reader["ETime"].ToString(),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        //Birthday
        public List<EmployeeMdl> Employee_BirthList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ZZSP_EmployeeUpComming", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            // EDate = Convert.ToDateTime(reader["EDate"]),
                            Email = reader["Email"].ToString(),
                           BirthdayAfter = Convert.ToInt32(reader["BirthdayAfter"]),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        //Marriage Anniversary
        public List<EmployeeMdl> Employee_MarriageAnniversary()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_Aniversary", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            // EDate = Convert.ToDateTime(reader["EDate"]),
                            Email = reader["Email"].ToString(),
                            AnnivDate = Convert.ToDateTime(reader["AnnivDate"]),
                            BirthdayAfter = Convert.ToInt32(reader["AnniversaryAfter"]),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
       //MissingPunch
        public List<EmployeeMdl> Employee_MissingPunch()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_MissingPunch", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            EDate = Convert.ToDateTime(reader["EDate"]),
                            MobileNo = reader["MobileNo"].ToString(),
                             };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        //EmployeeRetirement
        public List<EmployeeMdl> Employee_Retirement()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ZZSP_EmployeeRetiredShortly", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            EmpId = reader["EmpId"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                            Retirementdate = Convert.ToDateTime(reader["Retirementdate"]),
                            retiredindays = Convert.ToInt32(reader["retiredindays"]),

                            // ETime = reader["ETime"].ToString(),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
        //AbsentSMSList
        public List<EmployeeMdl> Emp_AbsentSMSList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("EmpSendedSMS_Absent", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                           // EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            AbsentDate = Convert.ToDateTime(reader["AbsentDate"]),
                            SendDate = Convert.ToDateTime(reader["SendDate"]),
                            MobileNo = reader["MobileNo"].ToString(),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }


        //MissingSent SMSList
        public List<EmployeeMdl> Emp_MissingSentSMSList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[EmpSendedSMS_Missingpunch]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                            // EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            EDate = Convert.ToDateTime(reader["EDate"]),
                            SendDate = Convert.ToDateTime(reader["SendDate"]),
                            MobileNo = reader["MobileNo"].ToString(),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }


        //LateSMSList
        public List<EmployeeMdl> Emp_LateSMSList()
        {
            List<EmployeeMdl> tasklist = new List<EmployeeMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("EmpSendedSMS_LateSMS", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        EmployeeMdl task = new EmployeeMdl
                        {
                            // totaltask.TotalTask = Convert.ToInt32(reader["TotalTask"]);
                          //  EmpId = reader["EmpId"].ToString(),
                            //cmpname = reader["cmpname"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            EDate = Convert.ToDateTime(reader["EDate"]),
                            SendDate = Convert.ToDateTime(reader["SendDate"]),
                            MobileNo = reader["MobileNo"].ToString(),


                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }
       








    }
}