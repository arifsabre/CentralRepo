using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Models
{
   
        public class BioLive
        {
      
        public List<B_INTEGRATIONMDI> employees()
        {
            List<B_INTEGRATIONMDI> tasklist = new List<B_INTEGRATIONMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ZZZ_BIO_INTEGRATION", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    {

                
                    while (reader.Read())
                    {
                        B_INTEGRATIONMDI task = new B_INTEGRATIONMDI();

                        task.RowId = Convert.ToInt32(reader["RowId"]);
                        task.ECode = reader["ECode"].ToString();
                            task.EmpId = reader["EmpId"].ToString();
                            task.EmpName = reader["EmpName"].ToString();
                        task.EDate = Convert.ToDateTime(reader["EDate"]);
                        task.ETime = reader["ETime"].ToString();
                        task.MCNo = Convert.ToInt32(reader["MCNo"]);
                        task.SendData = reader["SendData"].ToString();
                            tasklist.Add(task);
                    }
                    }
                }
            }
            return tasklist;
        }

        public List<B_INTEGRATIONMDI> GetAllNetworkStatus()
        {
            List<B_INTEGRATIONMDI> tasklist = new List<B_INTEGRATIONMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Network_GetNetworkStatus", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        B_INTEGRATIONMDI status = new B_INTEGRATIONMDI();
                        status.DeviceId = Convert.ToInt32(rdr["DeviceId"]);
                        status.DeviceLocation = rdr["DeviceLocation"].ToString();
                        status.DeviceIP = rdr["DeviceIP"].ToString();
                        status.ReaderPingdateTime = Convert.ToDateTime(rdr["ReaderPingdateTime"]);
                        status.ReaderReplyTime = rdr["ReaderReplyTime"].ToString();
                        status.DeviceStatus = rdr["DeviceStatus"].ToString();
                        status.status1 = rdr["status1"].ToString();
                       // Status.Add(status);



                        tasklist.Add(status);
                    }

                }
            }
            return tasklist;
        }


        
    }
    
     }

