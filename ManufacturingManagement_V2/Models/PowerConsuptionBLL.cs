using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PowerConsuptionBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();
        
        public List<PowerConsuptionMDI> PowerAllConsuptionList(int compcode = 0)
        {
            clsCookie objCookie = new clsCookie();
            List<PowerConsuptionMDI> tasklist = new List<PowerConsuptionMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("PowerConsuptionList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PowerConsuptionMDI task = new PowerConsuptionMDI();
                        {
                         task.Id= Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.ReadingDate = string.IsNullOrEmpty(reader["ReadingDate"].ToString()) ? "" : Convert.ToDateTime(reader["ReadingDate"]).ToShortDateString();
                       
                        task.MeterReadingKwh = Convert.ToDecimal(reader["MeterReadingKwh"]);
                        task.MeterReadingKvah = Convert.ToDecimal(reader["MeterReadingKvah"]);

                        task.DailyConKwh = Convert.ToInt32(reader["DailyConKwh"]);
                        task.DailyConKvah = Convert.ToInt32(reader["DailyConKvah"]);
                        task.MTDConKwh = Convert.ToInt32(reader["MTDConKwh"]);
                        task.MTDConKvah = Convert.ToInt32(reader["MTDConKvah"]);
                        task.PFDaily = Convert.ToDecimal(reader["PFDaily"]);
                        task.MTDPFDaily = Convert.ToDecimal(reader["MTDPFDaily"]);
                        task.MeterPFDaily = Convert.ToDecimal(reader["MeterPFDaily"]);
                        task.Demand = Convert.ToDecimal(reader["Demand"]);
                        task.Remark = reader["Remark"].ToString();
                        task.username = reader["username"].ToString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.UpdatedOn =  string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                            // ReadingDate = string.IsNullOrEmpty(reader["ReadingDate"].ToString()) ? "" : Convert.ToDateTime(reader["ReadingDate"]).ToShortDateString();

                        };

                    tasklist.Add(task);
                    }

                }
           }
            return tasklist;
        }

        //AllCompany
        public List<PowerConsuptionMDI> PowerConsuptionAllCompanyList()
        {
            clsCookie objCookie = new clsCookie();
            List<PowerConsuptionMDI> tasklist = new List<PowerConsuptionMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("PowerConsuptionListAllCompany", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PowerConsuptionMDI task = new PowerConsuptionMDI();
                        {
                            task.Id = Convert.ToInt32(reader["Id"]);
                            task.ShortName = reader["ShortName"].ToString();
                            task.ReadingDate = string.IsNullOrEmpty(reader["ReadingDate"].ToString()) ? "" : Convert.ToDateTime(reader["ReadingDate"]).ToShortDateString();

                            task.MeterReadingKwh = Convert.ToDecimal(reader["MeterReadingKwh"]);
                            task.MeterReadingKvah = Convert.ToDecimal(reader["MeterReadingKvah"]);

                            task.DailyConKwh = Convert.ToInt32(reader["DailyConKwh"]);
                            task.DailyConKvah = Convert.ToInt32(reader["DailyConKvah"]);
                            task.MTDConKwh = Convert.ToInt32(reader["MTDConKwh"]);
                            task.MTDConKvah = Convert.ToInt32(reader["MTDConKvah"]);
                            task.PFDaily = Convert.ToDecimal(reader["PFDaily"]);
                            task.MTDPFDaily = Convert.ToDecimal(reader["MTDPFDaily"]);
                            task.MeterPFDaily = Convert.ToDecimal(reader["MeterPFDaily"]);
                            task.Demand = Convert.ToDecimal(reader["Demand"]);
                            task.Remark = reader["Remark"].ToString();
                            task.username = reader["username"].ToString();
                            task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                            task.UpdatedBy = reader["UpdatedBy"].ToString();
                            task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                            };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public int AddNewCunsuptionEntry(PowerConsuptionMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("PowerConsuptionInsert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    cmd.Parameters.AddWithValue("@ReadingDate", libtask.ReadingDate);
                    cmd.Parameters.AddWithValue("@MeterReadingKwh", libtask.MeterReadingKwh);
                    cmd.Parameters.AddWithValue("@MeterReadingKvah", libtask.MeterReadingKvah);
                    cmd.Parameters.AddWithValue("@MeterPFDaily", libtask.MeterPFDaily);
                    cmd.Parameters.AddWithValue("@Demand", libtask.Demand);
                    cmd.Parameters.AddWithValue("@Remark", libtask.Remark);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }
        public int PowerUpdate_ConsuptionEntry(PowerConsuptionMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("PowerConsuptionUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", libtask.Id);
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    // cmd.Parameters.AddWithValue("@ReadingDate", libtask.ReadingDate);
                    // cmd.Parameters.AddWithValue("@ReadingTime", libtask.ReadingTime);
                    cmd.Parameters.AddWithValue("@MeterReadingKwh", libtask.MeterReadingKwh);
                    cmd.Parameters.AddWithValue("@MeterReadingKvah", libtask.MeterReadingKvah);
                    cmd.Parameters.AddWithValue("@MeterPFDaily", libtask.MeterPFDaily);
                    cmd.Parameters.AddWithValue("@Demand", libtask.Demand);
                    cmd.Parameters.AddWithValue("@Remark", libtask.Remark);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int PowerConsuptionDelete(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM PowerConsuptionMonitor WHERE Id = @Id", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                return i;
            }
        }
        //Company
        public int PowerCompanyUpdate(PowerConsuptionMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("PowerconCompanyUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", libtask.Id);
                    cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        //Email
        //public List<PowerConsuptionMDI> PowerSendEmailPF()
        //{
        //    List<PowerConsuptionMDI> tasklist = new List<PowerConsuptionMDI>();

        //    using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("PowerConsuptionList_EmailGreaterLessThan098", conn))
        //        {

        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            conn.Open();
        //           // cmd.Parameters.AddWithValue("@compcode", compcode);
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                PowerConsuptionMDI task = new PowerConsuptionMDI
        //                {
        //                    Id = Convert.ToInt32(reader["Id"]),
        //                    cmpname = reader["ShortName"].ToString(),
        //                    ReadingDate = Convert.ToDateTime(reader["ReadingDate"]),
        //                    ////ReadingTime = reader["ReadingTime"].ToString(),
        //                    //MeterReadingKwh = Convert.ToDecimal(reader["MeterReadingKwh"]),
        //                    //MeterReadingKvah = Convert.ToDecimal(reader["MeterReadingKvah"]),
        //                    DailyConKwh = Convert.ToInt32(reader["DailyConKwh"]),
        //                    DailyConKvah = Convert.ToInt32(reader["DailyConKvah"]),
        //                    //MTDConKwh = Convert.ToInt32(reader["MTDConKwh"]),
        //                    //MTDConKvah = Convert.ToInt32(reader["MTDConKvah"]),
        //                    PFDaily = Convert.ToDecimal(reader["PFDaily"]),
        //                    //MTDPFDaily = Convert.ToDecimal(reader["MTDPFDaily"]),
        //                    //MeterPFDaily = Convert.ToDecimal(reader["MeterPFDaily"]),
        //                   /// Demand = Convert.ToDecimal(reader["Demand"]),
        //                    Remark = reader["Remark"].ToString(),
        //                    //username = reader["username"].ToString(),
        //                    //CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
        //                };

        //                tasklist.Add(task);
        //            }
        //        }
        //    }
        //    return tasklist;
        //}
    }
}