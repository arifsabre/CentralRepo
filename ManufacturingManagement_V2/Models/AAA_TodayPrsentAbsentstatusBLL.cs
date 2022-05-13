using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_TodayPrsentAbsentstatusBLL
    {
        public List<AAA_TodayPrsentAbsentstatusMDI> Todays_Attendance_Status()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_TodayPrsentAbsentstatusMDI> Status = new List<AAA_TodayPrsentAbsentstatusMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_GET_PresentAbsentCount", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    Status.Add(new AAA_TodayPrsentAbsentstatusMDI
                    {
                        TotalPresent = Convert.ToInt32(rdr["TotalPresent"]),
                        TotalAbsent = Convert.ToInt32(rdr["TotalAbsent"]),
                        TotalEmployee = Convert.ToInt32(rdr["TotalEmployee"]),
                    });
                }
                return Status;
            }
        }
        public List<AAA_TodayPrsentAbsentstatusMDI> Ani_ZZSP_Ani()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_TodayPrsentAbsentstatusMDI> Anivar = new List<AAA_TodayPrsentAbsentstatusMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZSP_Ani", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    Anivar.Add(new AAA_TodayPrsentAbsentstatusMDI
                    {
                        EmpId = rdr["TotalPresent"].ToString(),
                        EmpName = rdr["EmpName"].ToString(),
                        FatherName = rdr["FatherName"].ToString(),
                        AnnivDate = Convert.ToDateTime(rdr["AnnivDate"]),
                        UpcomingAniversary = Convert.ToInt32(rdr["UpcomingAniversary"]),
                    });
                }
                return Anivar;
            }
        }


        public List<AAA_TodayPrsentAbsentstatusMDI> ZZSP_EmployeeUpCommingg()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_TodayPrsentAbsentstatusMDI> Birthdayy = new List<AAA_TodayPrsentAbsentstatusMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZSP_EmployeeUpComming", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    Birthdayy.Add(new AAA_TodayPrsentAbsentstatusMDI
                    {
                        EmpId = rdr["EmpId"].ToString(),
                        EmpName = rdr["EmpName"].ToString(),
                        FatherName = rdr["FatherName"].ToString(),
                       // Birthday = Convert.ToDateTime(rdr["BirthDate"]),
                        birthdayafter = Convert.ToInt32(rdr["birthdayafter"]),
                         });
                }
                return Birthdayy;
            }
        }

        public List<AAA_TodayPrsentAbsentstatusMDI> ZZSP_EmployeeRetiredShortly()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_TodayPrsentAbsentstatusMDI> Retirement = new List<AAA_TodayPrsentAbsentstatusMDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZSP_EmployeeRetiredShortly", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    Retirement.Add(new AAA_TodayPrsentAbsentstatusMDI
                    {
                        EmpId = rdr["EmpId"].ToString(),
                        EmpName = rdr["EmpName"].ToString(),
                        FatherName = rdr["FatherName"].ToString(),
                        JoiningDate = Convert.ToDateTime(rdr["BirthDate"]),
                        Retirementdate = Convert.ToDateTime(rdr["Retirementdate"]),
                        retiredindays = Convert.ToInt32(rdr["retiredindays"]),
                  });
                }
                return Retirement;
            }
        }


    }
}