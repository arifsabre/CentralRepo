using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace ManufacturingManagement_V2.Models
{
    public class Emp_Shift_bll
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        private LoginBLL loginBLL = new LoginBLL();
        //GetShift
        public List<Emp_Shift> GetAllShift_ByCompany(int compcode)
        {
            List<Emp_Shift> tasklist = new List<Emp_Shift>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_Shift_GetEmpList_ByCompany]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Emp_Shift task = new Emp_Shift();
                        task.NewEmpId = Convert.ToInt32(reader["NewEmpId"]);
                        task.EmpName = reader["EmpName"].ToString();
                        task.ShiftId = Convert.ToInt32(reader["ShiftId"]);
                        task.ShiftName = reader["ShiftName"].ToString();
                        //task.ShiftStartDate = reader["hodname"].ToString();
                        task.StartTime = reader["StartTime"].ToString();
                        task.EndTime = reader["EndTime"].ToString();
                        tasklist.Add(task);
                    }
                }
            }
        return tasklist;
        }
        //GetShiftDetail
        public List<Emp_Shift>GetAllShiftDetail()
        {
            List<Emp_Shift> tasklist = new List<Emp_Shift>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_Shift_All_Detail]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Emp_Shift task = new Emp_Shift();
                        task.ShiftName = reader["ShiftName"].ToString();
                        task.StartTime = reader["StartTime"].ToString();
                        task.EndTime = reader["EndTime"].ToString();
                        task.LateStart = reader["LateStart"].ToString();
                        task.LateEnd = reader["LateEnd"].ToString();
                     
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }












        //GetShiftList
        public List<Emp_Shift> GetAllShift()
        {
            List<Emp_Shift> tasklist = new List<Emp_Shift>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_Shift_GetAll]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Emp_Shift task = new Emp_Shift();
                     
                        task.ShiftId = Convert.ToInt32(reader["ShiftId"]);
                        task.ShiftName = reader["ShiftName"].ToString();
                       tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        //Contractor_Complaint_UpdatetFirstStep
        public int UpdateShiftBy_Company(Emp_Shift hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_Shift_Update]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@NewEmpId", hld.NewEmpId);
                    cmd.Parameters.AddWithValue("@ShiftId", hld.ShiftId);
                    //cmd.Parameters.AddWithValue("@ShiftName", hld.ShiftName);
                    //cmd.Parameters.AddWithValue("@StartTime", hld.StartTime);
                    //cmd.Parameters.AddWithValue("@EndTime", hld.EndTime);
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

       
        public int Insert_Shift(Emp_Shift hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Emp_Shift_Insert]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ShiftName", hld.ShiftName);
                    cmd.Parameters.AddWithValue("@ShiftStartDate", hld.ShiftStartDate);
                    cmd.Parameters.AddWithValue("@StartTime", hld.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", hld.EndTime);
                    cmd.Parameters.AddWithValue("@LateStart", hld.LateStart);
                    cmd.Parameters.AddWithValue("@LateEnd", hld.LateEnd);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        internal DataSet getEmployeeSearchListBio()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "[dbo].[Emp_search_list_BioId]";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
    }
}