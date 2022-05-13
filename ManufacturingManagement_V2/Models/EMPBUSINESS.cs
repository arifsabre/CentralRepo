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
    //public class EMPBUSINESS
    //{
        public class EmployeeBusinessLayer
        {
            public IEnumerable<B_INTEGRATIONMDI> employees
           {
              get
               {
                  clsMyClass mc = new clsMyClass();
                  string connectionString = mc.strconn;// ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;

                    List<B_INTEGRATIONMDI> employees = new List<B_INTEGRATIONMDI>();

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                    SqlCommand cmd = new SqlCommand("[ZZZ_BIO_INTEGRATION]", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            B_INTEGRATIONMDI employee = new B_INTEGRATIONMDI();
                            employee.EmpName = rdr["empname"].ToString();
                            employee.RowId = Convert.ToInt32(rdr["RowId"]);
                            employee.ECode = rdr["ECode"].ToString();
                            employee.EDate = Convert.ToDateTime(rdr["EDate"]);
                            employee.ETime = rdr["ETIME"].ToString();
                            employee.MCNo = Convert.ToInt32(rdr["MCNo"]);
                            employees.Add(employee);
                        }
                    }
                  return employees;
                }
            }

        public IEnumerable<B_INTEGRATIONMDI> GetAllNetworkStatus
        {
            get
            {
                clsMyClass mc = new clsMyClass();
                string connectionString = mc.strconn;// ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;

                List<B_INTEGRATIONMDI> Status = new List<B_INTEGRATIONMDI>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[Network_GetNetworkStatus]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
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
                        Status.Add(status);

                    }
                }
                return Status;
            }
        }
    }
    
     }

