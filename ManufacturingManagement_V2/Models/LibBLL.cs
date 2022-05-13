using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace ManufacturingManagement_V2.Models
{
    public class LibBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();
        readonly EmployeeBLL empBLL = new EmployeeBLL();

        public  List<LibTask> GetTotalTaskByUser(int  userid=0)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetTaskCountByUserIdTotal", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask totaltask = new LibTask
                        {
                            TotalTask = Convert.ToInt32(reader["TotalTask"])
                        };
                      tasklist.Add(totaltask);
                    }

                }
            }
            return tasklist;
        }
        public List<LibTask> GetAllTaskList(int userid=0)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetAllTask", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            Task = reader["Task"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            Status = reader["Status"].ToString(),
                            Priority = reader["Priority"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            UpdatedBy = reader["UpdatedBy"].ToString(),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            UpdatedOn = Convert.ToDateTime(reader["TaskDate"]),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public  List<LibTask> GetTaskListByUser(int userid=0)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetTaskByUserId", conn))
                {
                  
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            Task = reader["Task"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            Status = reader["Status"].ToString(),
                            Priority = reader["Priority"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            UpdatedBy = reader["UpdatedBy"].ToString(),
                            CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                            UpdatedOn = Convert.ToDateTime(reader["TaskDate"]),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }

      public List<LibTask> GetTaskListByUserCompleted(int userid)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetTaskByUserIdCompleted", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            Task = reader["Task"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            Status = reader["Status"].ToString(),
                            Priority = reader["Priority"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


        public List<LibTask> GetTaskListByUserNotStarted(int userid)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetTaskByUserIdNotStared", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            Task = reader["Task"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            Status = reader["Status"].ToString(),
                            Priority = reader["Priority"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<LibTask> GetTaskListByUserInProgress(int userid)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibGetTaskByUserIdInprogress", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            TaskId = Convert.ToInt32(reader["TaskId"]),
                            Task = reader["Task"].ToString(),
                            TaskDate = Convert.ToDateTime(reader["TaskDate"]),
                            Status = reader["Status"].ToString(),
                            Priority = reader["Priority"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<LibTask> GetAllNotificationByUserId(int userid)
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("NotificationUserId", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            //objmdl.NoticeDT = Convert.ToDateTime(dr["NoticeDt"].ToString());//d
                        NoticeDT = Convert.ToDateTime(reader["NoticeDt"].ToString()),//d
                        NoticeMsg = reader["NoticeMsg"].ToString(),
                            UserName = reader["username"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }



        public List<LibTask> GetAllNotification()
        {
            List<LibTask> tasklist = new List<LibTask>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Notice_GetAllNoticeList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@UserId", userid);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LibTask task = new LibTask
                        {
                            //objmdl.NoticeDT = Convert.ToDateTime(dr["NoticeDt"].ToString());//d
                            NoticeDT = Convert.ToDateTime(reader["NoticeDt"].ToString()),//d
                            NoticeMsg = reader["NoticeMsg"].ToString(),
                            UserName = reader["username"].ToString(),
                        };

                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
      public int AddNewTask(LibTask libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibTaskInsert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Task", libtask.Task);
                     cmd.Parameters.AddWithValue("@TaskDate", libtask.TaskDate);
                    cmd.Parameters.AddWithValue("@PId", libtask.PId);
                    cmd.Parameters.AddWithValue("@SId", libtask.SId);
                    cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i =  cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        public int Update_TaskList(LibTask hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibTaskUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TaskId", hld.TaskId);
                    cmd.Parameters.AddWithValue("@Task", hld.Task);
                    //cmd.Parameters.AddWithValue("@TaskDate", hld.TaskDate);
                    //cmd.Parameters.AddWithValue("@PId", hld.PId);
                   // cmd.Parameters.AddWithValue("@SId", hld.SId);
                   cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //status
        public int Update_TaskStatus(LibTask hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibTaskStatusUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TaskId", hld.TaskId);
                    cmd.Parameters.AddWithValue("@SId", hld.SId);
                    //cmd.Parameters.AddWithValue("@TaskDate", hld.TaskDate);
                    //cmd.Parameters.AddWithValue("@PId", hld.PId);
                    // cmd.Parameters.AddWithValue("@SId", hld.SId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //priority
       
        public int Update_TaskPriority(LibTask hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibTaskPriorityUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TaskId", hld.TaskId);
                    cmd.Parameters.AddWithValue("@PId", hld.PId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        //Taskdate
        public int Update_TaskDate(LibTask hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("LibTaskTaskDateUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@TaskId", hld.TaskId);
                    cmd.Parameters.AddWithValue("@TaskDate", hld.TaskDate);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int DeleteTaskItem(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {

           
                SqlCommand cmd = new SqlCommand("DELETE FROM LibTask WHERE TaskId = @TaskId", conn)
                {
                    CommandType = CommandType.Text
                };
            cmd.Parameters.AddWithValue("@TaskId", id);
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return i;
            }
        }



        internal List<LibTask> LibgetTaskStatusDropDown()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "LibGetTaskStatus";
            mc.fillFromDatabase(ds, cmd);
            List<LibTask> companies = new List<LibTask> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new LibTask
                {
                    SId = Convert.ToInt32(dr["SId"].ToString()),
                    Status = dr["Status"].ToString()
                });
            }
            return companies;
        }

        internal List<LibTask> LibgetTaskPriorityDropDown()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "LibGetTaskPriority";
            mc.fillFromDatabase(ds, cmd);
            List<LibTask> companies = new List<LibTask> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new LibTask
                {
                    PId = Convert.ToInt32(dr["PId"].ToString()),
                    Priority = dr["Priority"].ToString()
                });
            }
            return companies;
        }










    }
}