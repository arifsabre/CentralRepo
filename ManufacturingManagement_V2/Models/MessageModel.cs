using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data;

namespace ManufacturingManagement_V2.Models
{
        public class MessageModel
        {
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsEncryption ec = new clsEncryption();
        clsCookie objCookie = new clsCookie();

        public int RecordId { get; set; }
        [Required(ErrorMessage = "Email  Required")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "erp@praggroup.com";
        //= "erp@praggroup.com";

        [Required(ErrorMessage = "Subject  Required")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message  Required")]
        [Display(Name = "Message:")]
        public string Detail { get; set; }
        public int  UserId { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Company Name  Required")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        public string 
        ShortName { get; set; }
        public string cmpname { get; set; }

        [Required(ErrorMessage = "SendTo Email  Required")]
        [Display(Name = "SendTo")]
        public string SendTo { get; set; }


        [Required(ErrorMessage = "Sender Password  Required")]
        [Display(Name = "Password")]
        public string Password { get; set; } = "PragQ4@Klx$";
        // = "pmtKlxR@96%$";

        [Required(ErrorMessage = "Id  Required")]
        [Display(Name = "Id")]

        public int Id { get; set; }
       //[Required(ErrorMessage = "FileName  Required")]
        [Display(Name = "FileName")]
        public string FileName { get; set; }
       //[Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached File")]
        public List<HttpPostedFileBase> files { get; set; }
        public List<MessageModel> Item_List { get; set; }
        public List<MessageModel> Item_List1 { get; set; }
        public List<MessageModel> Item_List3 { get; set; }
        public byte[] FileContent { get; set; }
        //[Required(ErrorMessage = "Sent Date Required")]
        [Display(Name = "SendTo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SendDate { get; set; }
        public string SendDate1 { get; set; }

      
        public string EMailFrom { get; set; }
        public List<SelectListItem> Emails { get; set; }

        public string UpdatedOn { get; set; }
        public string CreatedOn { get; set; }
        public string ReferenceNo { get; set; }

        public string FullNameRec { get; set; }

        [Required(ErrorMessage = "Select Onbehalf of  Required?")]
        public string FullName { get; set; }

        public string EmailAddress { get; set; }



        public bool EditPer { get; set; }

        public string MemoNo { get; set; }
        
        public List<MessageModel> GetAllMailSent()
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetAllSentMail", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.CompCode = Convert.ToInt32(reader["CompCode"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.cmpname = reader["cmpname"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.SendDate= Convert.ToDateTime(reader["SendDate"]);
                        task.Detail = reader["Detail"].ToString();
                        //task.SendDate = reader["Keywords"].ToString();
                        task.UserName = reader["UserName"].ToString();
                        task.EMailFrom = reader["Email"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate1 = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();
                        task.MemoNo = reader["MemoNo"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public List<MessageModel> GetAttachmentByRecordId(int id = 0)
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetAllSentMailByRecordId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.Id = Convert.ToInt32(reader["Id"]);
                        task.FileName = reader["FileName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public List<MessageModel> Gettop1()
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetGetTop1", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.CompCode = Convert.ToInt32(reader["CompCode"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.cmpname = reader["cmpname"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.SendDate = Convert.ToDateTime(reader["SendDate"]);
                        // task.SendDate = reader["Keywords"].ToString();
                        task.UserName = reader["UserName"].ToString();
                        task.EMailFrom = reader["Email"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        //task.SendVia = reader["SendVia"].ToString();
                       
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public List<MessageModel> GetEmailList()
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_EmailList", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();
                        task.UserId = Convert.ToInt32(reader["userid"]);
                        task.UserName = reader["username"].ToString();
                       // task.EditPer = Convert.ToBoolean(reader["EditPer"]);
                        task.EmailAddress = reader["Email"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public string MemoGetEmail()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Memo_Email_Sent]", conn))
                {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // con.Open();
                    // SqlCommand com = new SqlCommand("usp_get_emailset_for_alert", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    //com.Parameters.AddWithValue("@eset", emailset);
                    SqlDataAdapter adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(ds, "tbl");
                }
                string addrs = "";
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            addrs += ds.Tables[0].Rows[i]["Email"].ToString() + ",";
                        }
                        addrs = addrs.Substring(0, addrs.Length - 1);
                    }
                }
                return addrs;
            }

        }
        public List<MessageModel> MemoMailSentDetail()
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_SentDetail", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MessageModel task = new MessageModel
                        {
                            RecordId = Convert.ToInt32(rdr["RecordId"]),
                            Subject = rdr["Subject"].ToString(),
                            Detail = rdr["Detail"].ToString(),
                            FullNameRec = rdr["FullNameRec"].ToString(),
                            FullName = rdr["FullName"].ToString(),
                            //CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]).ToShortDateString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }


        public List<MessageModel> GetMailDetail(int recordid=0)
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetAllSentMailByRecordId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", recordid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.CompCode = Convert.ToInt32(reader["CompCode"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.cmpname = reader["cmpname"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        //task.SendDate = Convert.ToDateTime(reader["SendDate"]);
                        // task.SendDate = reader["Keywords"].ToString();
                        task.UserName = reader["UserName"].ToString();
                        task.EMailFrom = reader["Email"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        

                    
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate1 = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public int DeleteMemoRecord(int recordid)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Memo_Delete]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Recordid", recordid);

                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    //cmd.Parameters.AddWithValue("@Category", hld.Category);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public List<MessageModel> MemoTop1Record()
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetTop1", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();

                        task.ReferenceNo = reader["ReferenceNo"].ToString();
                        // task.Id = Convert.ToInt32(reader["Id"]);

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
       
        [HttpPost]
        public int UpdateEmailAddress(MessageModel hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                for (i = 0; i <hld.Item_List.Count; i++)
                {
                using (SqlCommand cmd = new SqlCommand("[Memo_UpdateEmail]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", hld.UserId);
                    cmd.Parameters.AddWithValue("@EditPer", hld.EditPer);
                   //cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                }
                return i;
            }
        }

        public List<MessageModel> GetFullUserName(int recordid = 0)
        {
            List<MessageModel> tasklist = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_GetToPrintByRecordId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", recordid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MessageModel task = new MessageModel();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        //task.UserName = reader["username"].ToString();
                        //// task.EditPer = Convert.ToBoolean(reader["EditPer"]);
                        task.FullNameRec = reader["FullNameRec"].ToString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<MachineMaintenanceMDI> GetAllUserName()
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Memo_AllUserNameList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            userid = Convert.ToInt32(reader["userid"]),
                            FullName = reader["FullName"].ToString(),


                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }




    }
}