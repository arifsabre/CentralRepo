using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class StudentAPIController : ApiController
    {
        public IEnumerable<EmployeeMdl> GetStaffList(string etype = "0", string mxtype = "1", string grade = "s")
        {
            //Connection();
            //List<StaffModel> StaffData = new List<StaffModel>();
            //SqlCommand cmd = new SqlCommand("spGetStaffList", conn);
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //conn.Open();
            //SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    StaffModel staff = new StaffModel();
            //    staff.StaffId = Convert.ToInt32(reader["StaffId"]);
            //    staff.FirstName = reader["FirstName"].ToString();
            //    staff.LastName = reader["LastName"].ToString();
            //    staff.Mobile = reader["Mobile"].ToString();
            //    staff.Email = reader["Email"].ToString();
            //    staff.Address = reader["Address"].ToString();
            //    StaffData.Add(staff);
            //}
            //conn.Close();
            //return StaffData;

            //tokens reading
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            //and so on ...
            //

            clsMyClass mc = new clsMyClass();
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_directory";
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            List<EmployeeMdl> employees = new List<EmployeeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeMdl objmdl = new EmployeeMdl();
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.ContactNo = dr["ContactNo"].ToString();
                objmdl.PhoneExtNo = dr["PhoneExtNo"].ToString();
                employees.Add(objmdl);
            }
            return employees;
        }

        //public IEnumerable<EInvoiceSaleMdl> GetInvoiceResult()
        public IEnumerable<string> GetInvoiceResult()
        {
            //tokens redaing
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            string salerecid = "0";
            if (Request.Headers.Contains("salerecid"))
            {
                salerecid = Request.Headers.GetValues("salerecid").First();
            }
            //and so on ...
            //
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            List<EInvoiceSaleMdl> listInv = new List<EInvoiceSaleMdl> { };
            EInvoiceSaleMdl slMdl = new EInvoiceSaleMdl();
            slMdl = slBll.getEInvoiceSaleObject(Convert.ToInt32(salerecid));
            listInv.Add(slMdl);
            string[] strV = { "value1", "value2" };
            return strV;
            //return listInv;

        }


        public Response SaveStaff(EmployeeMdl staff)
        {
            //tokens redaing
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            //and so on ...
            //

            Response response = new Response();
            response.Message = "OK";
            response.Status = 1;
            //try
            //{
            //    if (string.IsNullOrEmpty(staff.EmpName))
            //    {
            //        response.Message = "FirstName  is Mandatory";
            //        response.Status = 0;

            //    }
            //    else if (string.IsNullOrEmpty(staff.FatherName))
            //    {
            //        response.Message = "LastName  is Mandatory";
            //        response.Status = 0;
            //    }
            //    else if (string.IsNullOrEmpty(staff.ContactNo))
            //    {
            //        response.Message = "Mobile is Mandatory";
            //        response.Status = 0;
            //    }
            //    else if (string.IsNullOrEmpty(staff.Email))
            //    {
            //        response.Message = "Email is Mandatory";
            //        response.Status = 0;
            //    }
            //    else if (string.IsNullOrEmpty(staff.CAddress))
            //    {
            //        response.Message = "Address  is Mandatory";
            //        response.Status = 0;
            //    }
            //    else
            //    {
            //        Connection();
            //        SqlCommand com = new SqlCommand("spAddStaff", conn);
            //        com.CommandType = System.Data.CommandType.StoredProcedure;
            //        com.Parameters.AddWithValue("FirstName", staff.EmpName);
            //        com.Parameters.AddWithValue("LastName", staff.FatherName);
            //        com.Parameters.AddWithValue("Mobile", staff.ContactNo);
            //        com.Parameters.AddWithValue("Email", staff.Email);
            //        com.Parameters.AddWithValue("Address", staff.CAddress);
            //        conn.Open();
            //        int i = com.ExecuteNonQuery();
            //        conn.Close();
            //        if (i >= 1)
            //        {
            //            response.Message = "staff Save Successfully";
            //            response.Status = 1;
            //        }
            //        else
            //        {
            //            response.Message = "Failed";
            //            response.Status = 0;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    response.Message = ex.Message;
            //    response.Status = 0;
            //}
            return response;
        }
    }
}