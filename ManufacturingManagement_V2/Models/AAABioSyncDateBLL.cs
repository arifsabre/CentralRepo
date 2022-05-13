using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AAABioSyncDateBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
       
        //
        #region dml objects
        //
        internal void Update_BIO_Attendance(DateTime activedate)
        {
            Result = false;
            //if (mc.isValidDateToSetRecord(activedate) == false)
            //{
            //    Message = "[Date check failed] Old record modification is not allowed!";
            //    return;
            //}
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "ZZZ_USP_UpdateAttendanceByDate";
                cmd.Parameters.Add(mc.getPObject("@activedate", activedate, DbType.DateTime));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendance, mc.getStringByDate(activedate), "Staff Attendance Updated1");
                Result = true;
                Message = "Record(s) Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AAABioSyncDateBLL", "Update_BIO_Attendance", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        #region dml objects

        internal void Update_BIO_AttendanceFromAAAASMSSend(DateTime activedate)
        {
            Result = false;
            //if (mc.isValidDateToSetRecord(activedate) == false)
            //{
            //    Message = "[Date check failed] Old record modification is not allowed!";
            //    return;
            //}
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "[ZZZ_USP_UpdateAttendanceByDate_BioId_ForStaff_AAAA_SendSMS]";
                cmd.Parameters.Add(mc.getPObject("@activedate",activedate, DbType.DateTime));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendance, mc.getStringByDate(activedate), "Staff Attendance Updated2");
                Result = true;
                Message = "Record(s) Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AAABioSyncDateBLL", "Update_BIO_AttendanceFromAAAASMSSend", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        #endregion

        internal void Update_Attendance_Worker(DateTime activedate)
        {
            Result = false;
            //if (mc.isValidDateToSetRecord(activedate) == false)
            //{
            //    Message = "[Date check failed] Old record modification is not allowed!";
            //    return;
            //}
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "[ZZZ_USP_UpdateAttendanceByDate_BioId_ForWorker_AAAA_SendSMS]";
                cmd.Parameters.Add(mc.getPObject("@activedate", activedate, DbType.DateTime));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_attendance, mc.getStringByDate(activedate), "Worker Attendance Updated3");
                Result = true;
                Message = "Record(s) Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AAABioSyncDateBLL", "Update_BIO_Attendance", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }

        public List<rptOptionMdl> Get_BiometricLastUpdate()
        {
            List<rptOptionMdl> tasklist = new List<rptOptionMdl>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Emp_BiometricLastUpdate", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        rptOptionMdl task = new rptOptionMdl
                        {
                          
                            LastUpdated = reader["LastUpdated"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;


        }

        



    }
}