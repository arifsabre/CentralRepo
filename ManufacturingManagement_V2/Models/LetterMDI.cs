using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class LetterMDI
    {
        clsCookie objCookie = new clsCookie();
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Company Required")]
        [Display(Name = "Company")]
        public int compcode { get; set; }
        public string ReferenceLetter { get; set; }
        public string ReferenceNo { get; set; }
        public string SNO { get; set; }
        public string Remark { get; set; }
       


        [Required(ErrorMessage = "Department Required")]
        [Display(Name = "Department")]
        public int DepId { get; set; }
        public string FinYear { get; set; }

        public string DepName { get; set; }

        public string ShortName { get; set; }
        public string cmpname { get; set; }
       // public string ReferenceNo { get; set; }

        [Required(ErrorMessage = "Letter No Required")]
        [Display(Name = "Lettter No")]
        public string LetterNo { get; set; }

        [Required(ErrorMessage = "Subject  Required")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Keywords  Required")]
        [Display(Name = "Keywords")]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "SendTo Required")]
        [Display(Name = "Send To")]
        public string SendTo { get; set; }

        //[Required(ErrorMessage = "Send Date Required")]
        [Display(Name = "SendDate")]
        public string SendDate { get; set; }

        //[Required(ErrorMessage = "Letter Date Required")]
        //[Display(Name = "Letter Date")]
        public string LetterDate { get; set; }



        //[Required(ErrorMessage = "Sent Date Required")]
        [Display(Name = "SentDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SendDate1 { get; set; }

        //[Required(ErrorMessage = "Sent Date Required")]
        //[Display(Name = "SentDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LetterDate1 { get; set; }










        [Required(ErrorMessage = "SendVia  Required")]
        [Display(Name = "SendVia")]
        public string SendVia { get; set; }

       // [Required(ErrorMessage = "Tracking No  Required")]
       // [Display(Name = "TrackNo")]
        public string TrackNo { get; set; }
        public string TrackNoPost { get; set; }
        public string TrackNoHand { get; set; }
        public string Track_No { get; set; }
        public bool Email { get; set; }
        public bool Post { get; set; }
        public bool ByHand { get; set; }





       // [Required(ErrorMessage = "Id  Required")]
        [Display(Name = "Id")]
        public int Id { get; set; }

       // [Required(ErrorMessage = "FileName  Required")]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

       // [Required(ErrorMessage = "Attached File")]
        [DataType(DataType.Upload)]
        [Display(Name = "Attached File")]
        public List<HttpPostedFileBase> files { get; set; }

        public byte[] FileContent { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }

        
        public List<LetterMDI> Item_List { get; set; }
        public List<LetterMDI> Item_List2 { get; set; }
        public List<LetterMDI> Item_List1 { get; set; }
        public List<LetterMDI> Item_ListRef { get; set; }
        public List<LetterMDI> ShortNameList { get; set; }

        public int UserId { get; set; }
        public string OtherFile { get; set; }
        public List<LetterMDI> usersinfo { get; set; }
        //List
        
        public List<LetterMDI> GetAllLetterListByUserId(int id=0,string fin=null)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByUser", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   cmd.Parameters.AddWithValue("@CompCode", objCookie.getCompCode());
                   cmd.Parameters.AddWithValue("@FinYear", objCookie.getFinYear());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.FinYear = reader["finYear"].ToString();
                        task.DepId = Convert.ToInt32(reader["DepId"]);
                        task.DepName = reader["DepName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();

                        task.Email = Convert.ToBoolean(reader["Email"].ToString());
                        task.Post = Convert.ToBoolean(reader["Post"].ToString());
                        task.ByHand = Convert.ToBoolean(reader["ByHand"].ToString());
                        task.TrackNoPost = reader["TrackNoPost"].ToString();
                        task.TrackNoHand = reader["TrackNoHand"].ToString();
                        task.Track_No = reader["Track_No"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.ReferenceNo = reader["ReferenceNo"].ToString();
                        task.SNO = reader["SNO"].ToString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();

                        task.cmpname = reader["cmpname"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();
                        task.LetterDate = string.IsNullOrEmpty(reader["LetterDate"].ToString()) ? "" : Convert.ToDateTime(reader["LetterDate"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public List<LetterMDI> GetAllCompanyLetter(int compcode=0)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllCompany", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CompCode", objCookie.getCompCode());
                    //cmd.Parameters.AddWithValue("@FinYear", objCookie.getFinYear());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();
                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.FinYear = reader["finYear"].ToString();
                        task.DepId = Convert.ToInt32(reader["DepId"]);
                        task.DepName = reader["DepName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();

                        task.Email = Convert.ToBoolean(reader["Email"].ToString());
                        task.Post = Convert.ToBoolean(reader["Post"].ToString());
                        task.ByHand = Convert.ToBoolean(reader["ByHand"].ToString());
                        task.TrackNoPost = reader["TrackNoPost"].ToString();
                        task.TrackNoHand = reader["TrackNoHand"].ToString();
                        task.Track_No = reader["Track_No"].ToString();
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.SNO = reader["SNO"].ToString();
                        task.Remark = reader["Remark"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.cmpname = reader["cmpname"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();
                        task.LetterDate = string.IsNullOrEmpty(reader["LetterDate"].ToString()) ? "" : Convert.ToDateTime(reader["LetterDate"]).ToShortDateString();
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //CountTop1
        public List<LetterMDI> GetTop1Record()
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetTop1", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@CompCode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.ReferenceNo = reader["ReferenceNo"].ToString();
                        // task.Id = Convert.ToInt32(reader["Id"]);

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //CountTop1
        public List<LetterMDI> GetCompanyCount(int id=0,string finyear=null)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetCompanyCount1", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                     cmd.Parameters.AddWithValue("@CompCode", objCookie.getCompCode());
                    cmd.Parameters.AddWithValue("@FinYear", objCookie.getFinYear());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.ReferenceNo = reader["ReferenceNo"].ToString();
                       // task.ReferenceLetter = reader["ReferenceNo"].ToString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<LetterMDI>getDepartmentList()
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetDept", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI
                        {
                            DepId = Convert.ToInt32(reader["DepId"]),
                            DepName = reader["DepName"].ToString(),

                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //GetCompanyPrefix
        public List<LetterMDI> GetCompanyPrefix(int id=0)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetCompany", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode",compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.ReferenceNo = reader["ReferenceNo"].ToString();
                        // task.Id = Convert.ToInt32(reader["Id"]);

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        //ListAllFile
        public List<LetterMDI> GetAllLetterList()
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Letter_GetAllFile]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        //task.Remark = reader["Remark"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();
                        //task.LetterDate = string.IsNullOrEmpty(reader["LetterDate"].ToString()) ? "" : Convert.ToDateTime(reader["LetterDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //ByRecordId
        //List
        public List<LetterMDI> GetAllLetterListByRecordId(int id=0)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByFileId", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                         task.RecordId = Convert.ToInt32(reader["RecordId"]);
                         task.Id = Convert.ToInt32(reader["Id"]);
                         task.FileName = reader["FileName"].ToString();
                     

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListPI
        public List<LetterMDI> GetAllLetterListByCompanyPI(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByCompanyPI", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                       // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        //ListPI-2
        public List<LetterMDI> GetAllLetterListByCompanyPI2(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByCompanyPI2", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        //task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListPP
        public List<LetterMDI> GetAllLetterListByCompanyPP(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Letter_GetAllByCompanyPP]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                       // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListPP2
        public List<LetterMDI> GetAllLetterListByCompanyPP2(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByCompanyPP2", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                       // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListPR
        public List<LetterMDI> GetAllLetterListByCompanyPR(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByCompanyPR", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                       // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //ListGS
        public List<LetterMDI> GetAllLetterListByCompanyGS(int userid)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_GetAllByCompanyGS", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                       // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();
                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        //Update
        public int LetterUpdate(LetterMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@LetterNo", hld.LetterNo);
                    cmd.Parameters.AddWithValue("@Subject", hld.Subject);
                    cmd.Parameters.AddWithValue("@SendTo", hld.SendTo);
                    cmd.Parameters.AddWithValue("@Keywords", hld.Keywords);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@SendVia", hld.SendVia);

                    cmd.Parameters.AddWithValue("@TrackNo", hld.TrackNo);
                    cmd.Parameters.AddWithValue("@TrackNoPost", hld.TrackNoPost);
                    cmd.Parameters.AddWithValue("@TrackNoHand", hld.TrackNoHand);

                    cmd.Parameters.AddWithValue("@Email", hld.Email);
                    cmd.Parameters.AddWithValue("@Post", hld.Post);
                    cmd.Parameters.AddWithValue("@ByHand", hld.ByHand);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    //cmd.Parameters.AddWithValue("@finyear", hld.FinYear);
                    cmd.Parameters.AddWithValue("@DepId", hld.DepId);
                    cmd.Parameters.AddWithValue("@Remark", hld.Remark);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
       // LetterCompanyUpdate
        public int LetterCompanyUpdate(LetterMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_CompanyUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        // LetterSentDateUpdate
        public int LetterSentUpdate(LetterMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_SentdateUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@SendDate", hld.SendDate1);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int LetterDateUpdate(LetterMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_LetterDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", hld.RecordId);
                    cmd.Parameters.AddWithValue("@LetterDate", hld.LetterDate1);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //ByRecordIdView
        public List<LetterMDI> GetAllLetterListByRecordNo(int recordid = 0)
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Letter_GetAllByRecordId]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", recordid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                        task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        // task.Id = Convert.ToInt32(reader["Id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.Subject = reader["Subject"].ToString();
                        task.LetterNo = reader["LetterNo"].ToString();
                        task.SendTo = reader["SendTo"].ToString();
                        task.Keywords = reader["Keywords"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.CreatedBy = reader["CreatedBy"].ToString();
                        task.SendVia = reader["SendVia"].ToString();
                        task.TrackNo = reader["TrackNo"].ToString();

                        task.Email = Convert.ToBoolean(reader["Email"]);
                        task.Post = Convert.ToBoolean(reader["Post"]);
                        task.ByHand = Convert.ToBoolean(reader["ByHand"]);

                        task.TrackNoPost = reader["TrackNoPost"].ToString();
                        task.TrackNoHand = reader["TrackNoHand"].ToString();


                        task.UpdatedOn = string.IsNullOrEmpty(reader["UpdatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["UpdatedOn"]).ToShortDateString();
                        task.CreatedOn = string.IsNullOrEmpty(reader["CreatedOn"].ToString()) ? "" : Convert.ToDateTime(reader["CreatedOn"]).ToShortDateString();
                        task.SendDate = string.IsNullOrEmpty(reader["SendDate"].ToString()) ? "" : Convert.ToDateTime(reader["SendDate"]).ToShortDateString();

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        public int LetterDelete(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", id);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int LetterDeleteoOneFile(int id)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_DeleteOneFile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", id);
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public List<LetterMDI> GetAllFileNameListToRepalce()
        {
            List<LetterMDI> tasklist = new List<LetterMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Letter_getAllFileIndropdown", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LetterMDI task = new LetterMDI();

                       // task.RecordId = Convert.ToInt32(reader["RecordId"]);
                        task.Id = Convert.ToInt32(reader["Id"]);
                        task.FileName = reader["FileName"].ToString();


                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }



    }
}