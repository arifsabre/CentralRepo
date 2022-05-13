using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MachineMaintenanceBLL
    {
        clsCookie objCookie = new clsCookie();
        readonly clsMyClass mc = new clsMyClass();
        readonly CompanyBLL compBLL = new CompanyBLL();

        public List<MachineMaintenanceMDI> GetMaintenanceTaskByCompany(int compcode = 0)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineMaintenanceList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CompCode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI();

                        task.id = Convert.ToInt32(reader["id"]);
                        task.compcode = Convert.ToInt32(reader["compcode"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.scheduleid = Convert.ToInt32(reader["scheduleid"]);
                        task.schedulename = reader["schedulename"].ToString();
                        task.machineid = Convert.ToInt32(reader["machineid"]);
                        task.machinename = reader["machinename"].ToString();
                        task.NewEmpId = Convert.ToInt32(reader["NewEmpId"]);
                        task.EmpName = reader["EmpName"].ToString();
                        task.userid = Convert.ToInt32(reader["userid"]);
                        task.FullName = reader["FullName"].ToString();

                        task.checkpointid = Convert.ToInt32(reader["checkpointid"]);
                        task.checkdetail = reader["checkdetail"].ToString();
                        task.detailofworkdone = reader["detailofworkdone"].ToString();
                        task.condition = reader["condition"].ToString();
                        task.duedate = reader["duedate"].ToString();
                        task.assignee = reader["Assignee"].ToString();
                        task.UpdatedOn = reader["UpdatedOn"].ToString();
                        task.CreatedOn = reader["CreatedOn"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.Doc = Convert.ToInt32(reader["Doc"]);
                        //task.Active = Convert.ToBoolean(reader["Active"].ToString());
                        task.NextDueDate = reader["NextDueDate"].ToString();
                        // NotUpdatedSinceDays = reader["NotUpdatedSinceDays"].ToString()
                        task.NotUpdatedSinceDays = Convert.ToInt32(reader["NotUpdatedSinceDays"]);
                        task.ExternalMan = reader["ExternalMan"].ToString();
                        tasklist.Add(task);
                    }


                }
            }

            return tasklist;
        }

        public int AddMaintenanceTask(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ManchineMaintenance_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    cmd.Parameters.AddWithValue("@machineid",libtask.machineid);
                    cmd.Parameters.AddWithValue("@scheduleid",libtask.scheduleid);
                    cmd.Parameters.AddWithValue("@checkpointid",libtask.checkpointid);
                    cmd.Parameters.AddWithValue("@NewEmpId", libtask.NewEmpId);
                    cmd.Parameters.AddWithValue("@userid",libtask.userid);
                   // cmd.Parameters.AddWithValue("@detailofworkdone",libtask.detailofworkdone);
                   // cmd.Parameters.AddWithValue("@condition", libtask.condition);
                    cmd.Parameters.AddWithValue("@duedate", libtask.duedate);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@ExternalMan", libtask.ExternalMan);



                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }
        //
        public int MaintenanceTaskUpdate(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ManchineMaintenance_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", libtask.id);
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    cmd.Parameters.AddWithValue("@machinename", libtask.machinename);
                    cmd.Parameters.AddWithValue("@NewEmpId", libtask.NewEmpId);
                    cmd.Parameters.AddWithValue("@EmpName", libtask.EmpName);
                    cmd.Parameters.AddWithValue("@FullName", libtask.FullName);
                    cmd.Parameters.AddWithValue("@detailofworkdone", libtask.detailofworkdone);
                    cmd.Parameters.AddWithValue("@condition", libtask.condition);
                    cmd.Parameters.AddWithValue("@schedule", libtask.schedulename);
                    cmd.Parameters.AddWithValue("@duedate", libtask.duedate);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);
                    cmd.Parameters.AddWithValue("@checkpointid", libtask.checkpointid);
                    cmd.Parameters.AddWithValue("@scheduleid", libtask.scheduleid);
                    cmd.Parameters.AddWithValue("@userid", libtask.userid);
                    cmd.Parameters.AddWithValue("@ExternalMan", libtask.ExternalMan);
                    //cmd.Parameters.AddWithValue("@Active", libtask.Active);

                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        ////DefineNextSchedule

       

        //
        internal List<MachineMaintenanceMDI> MachineList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "Machine_GetMachineName";
            mc.fillFromDatabase(ds, cmd);
            List<MachineMaintenanceMDI> machine = new List<MachineMaintenanceMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                machine.Add(new MachineMaintenanceMDI
                {
                    machineid = Convert.ToInt32(dr["machineid"].ToString()),
                    machinename = dr["machinename"].ToString()
                });
            }
            return machine;
        }
//
        internal List<MachineMaintenanceMDI> MachineCheckPoint()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "Machine_GetMachineCheckPoint";
            mc.fillFromDatabase(ds, cmd);
            List<MachineMaintenanceMDI> checkpoint = new List<MachineMaintenanceMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                checkpoint.Add(new MachineMaintenanceMDI
                {
                    checkpointid = Convert.ToInt32(dr["checkpointid"].ToString()),
                    checkdetail = dr["checkdetail"].ToString()
                });
            }
            return checkpoint;
        }
       
        //MasterMachine
        public List<MachineMaintenanceMDI> MachineSchedule()
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Machine_GetMachineSchedule", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            scheduleid = Convert.ToInt32(reader["scheduleid"]),
                            schedulename = reader["schedulename"].ToString(),
                                 
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }



        //Responsibility
        internal DataSet MachineResponsibility(int workingunit)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "MachineResponsibility";
            cmd.Parameters.Add(mc.getPObject("@WorkingUnit", workingunit, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
       internal List<MachineMaintenanceMDI> getResplistbyCompany(int workingunit)
        {
            DataSet ds = MachineResponsibility(workingunit);
            List<MachineMaintenanceMDI> companies = new List<MachineMaintenanceMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new MachineMaintenanceMDI
                {
                    NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString()),
                    EmpName = dr["EmpName"].ToString()
                });
            }
            return companies;
        }
      //MasterMachine
        public List<MachineMaintenanceMDI> GetMachineNameList(int compcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_List", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            machineid = Convert.ToInt32(reader["machineid"]),
                            ShortName = reader["ShortName"].ToString(),
                            machinename = reader["machinename"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString(),

                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<MachineMaintenanceMDI> GetMachineNameByCompany(int compcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_ListByCompany", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            machineid = Convert.ToInt32(reader["machineid"]),
                            //ShortName = reader["ShortName"].ToString(),
                            machinename = reader["machinename"].ToString(),
                            //CreatedBy = reader["CreatedBy"].ToString(),
                            //CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }

        public List<MachineMaintenanceMDI> GetCheckPointByCompany(int compcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("CheckPoint_ListByCompany", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            checkpointid = Convert.ToInt32(reader["checkpointid"]),
                            checkdetail = reader["checkdetail"].ToString(),
                           
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }


        //
        public int InsertMasterMachine(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                     cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    // cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@machinename", libtask.machinename);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        //
        //
        public int UpdateMasterMachine(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_Update", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);
                   // cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                  //  cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@machinename", libtask.machinename);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        //checkpoint
        public List<MachineMaintenanceMDI> GetMachineCheckPointList(int compcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineCheckPointList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                     cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                           checkpointid = Convert.ToInt32(reader["checkpointid"]),
                           ShortName = reader["ShortName"].ToString(),
                           machinename = reader["machinename"].ToString(),
                            machineid = Convert.ToInt32(reader["machineid"]),
                            checkdetail = reader["checkdetail"].ToString(),
                           scheduleid = Convert.ToInt32(reader["scheduleid"]),
                           schedulename = reader["schedulename"].ToString(),
                           CreatedBy = reader["CreatedBy"].ToString(),
                           CreatedOn = reader["CreatedOn"].ToString(),

                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        //
        public int InsertMasterCheckPoint(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineCheckPoint_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    //cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@checkdetail", libtask.checkdetail);
                    cmd.Parameters.AddWithValue("@scheduleid", libtask.scheduleid);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

        //
        //

        public int UpdateMasterCheckPoint(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineCheckPoint_Update", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();

                    cmd.Parameters.AddWithValue("@checkpointid", libtask.checkpointid);
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);

                   // cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@checkdetail", libtask.checkdetail);
                    cmd.Parameters.AddWithValue("@scheduleid", libtask.scheduleid);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }

      //MasterSchedule
        public List<MachineMaintenanceMDI> GetMachineScheduleList()
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineSchedule_List", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            scheduleid = Convert.ToInt32(reader["scheduleid"]),
                            schedulename = reader["schedulename"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString(),

                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        //breakDown


        public List<MachineMaintenanceMDI> GetBreakDownList(int comcode)
        {
           
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ManchineBreakDown_List", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            breakid = Convert.ToInt32(reader["breakid"]),
                            ShortName = reader["ShortName"].ToString(),
                            machinename = reader["machinename"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            detailofworkdone = reader["detailofworkdone"].ToString(),
                            breakreason = reader["breakreason"].ToString(),
                            breakdate = reader["breakdate"].ToString(),
                            MachineUp_Date = reader["MachineUp_Date"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString(),
                            UpdatedBy = reader["UpdatedBy"].ToString(),
                            UpdatedOn =  reader["UpdatedOn"].ToString(),
                            UpTime = reader["UpTime"].ToString(),
                            BreakTime = reader["BreakTime"].ToString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<MachineMaintenanceMDI> GetBreakDownHistory(int comcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Machine_DownHistorybyCompany", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            breakid = Convert.ToInt32(reader["breakid"]),
                            machineid = Convert.ToInt32(reader["machineid"]),
                            machinename = reader["machinename"].ToString(),
                            ShortName = reader["ShortName"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            detailofworkdone = reader["detailofworkdone"].ToString(),
                            breakreason = reader["breakreason"].ToString(),
                            breakdate = reader["breakdate"].ToString(),
                            MachineUp_Date = reader["MachineUp_Date"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString(),
                            UpdatedBy = reader["UpdatedBy"].ToString(),
                            UpdatedOn = reader["UpdatedOn"].ToString(),
                            UpTime = reader["UpTime"].ToString(),
                            BreakTime = reader["BreakTime"].ToString(),
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
       //
        public int AddBreakTask(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ManchineBreakDown_Insert", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    
                    cmd.Parameters.AddWithValue("@compcode", libtask.compcode);
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);
                    cmd.Parameters.AddWithValue("@NewEmpId", libtask.NewEmpId);
                    cmd.Parameters.AddWithValue("@UserId", libtask.userid);
                   // cmd.Parameters.AddWithValue("@detailofworkdone", libtask.detailofworkdone);
                   // cmd.Parameters.AddWithValue("@breakreason", libtask.breakreason);
                    cmd.Parameters.AddWithValue("@breakdate", libtask.breakdate);
                    cmd.Parameters.AddWithValue("@BreakTime", libtask.BreakTime);
                    // cmd.Parameters.AddWithValue("@MachineUp_Date", libtask.upagain);
                    cmd.Parameters.AddWithValue("@CreatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
            }
            return i;
        }
        //
        public int UpdateBreakdowntask(MachineMaintenanceMDI libtask)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ManchineBreakDown_update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                    conn.Open();
                    cmd.Parameters.AddWithValue("@breakid", libtask.breakid);
                    //cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    cmd.Parameters.AddWithValue("@machineid", libtask.machineid);
                    cmd.Parameters.AddWithValue("@machinename", libtask.machinename);
                    cmd.Parameters.AddWithValue("@ShortName", libtask.ShortName);
                    cmd.Parameters.AddWithValue("@EmpName", libtask.EmpName);
                    cmd.Parameters.AddWithValue("@FullName", libtask.FullName);
                    cmd.Parameters.AddWithValue("@detailofworkdone", libtask.detailofworkdone);
                    cmd.Parameters.AddWithValue("@breakreason", libtask.breakreason);
                   // cmd.Parameters.AddWithValue("@breakdate", libtask.breakdate);
                    cmd.Parameters.AddWithValue("@MachineUp_Date", libtask.MachineUp_Date);
                    cmd.Parameters.AddWithValue("@UpTime", libtask.UpTime);
                    cmd.Parameters.AddWithValue("@BreakTime", libtask.BreakTime);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public List<MachineMaintenanceMDI> GetUserNameByCompany(int compcode)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineGetCheckingResponsibility", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                     cmd.Parameters.AddWithValue("@compcode", compcode);
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
        //deleteMaintenanceTask
        public int DeleteMaintenanaceTask(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM MachineMaintenance WHERE id = @id", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }
        //DeleteMachine
        public int DeleteMachine(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM MachineName WHERE machineid = @machineid", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@machineid", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
               // conn.Close();
                return i;
            }
        }

        //DeleteChecklist
    
        public int DeleteCheckPoint(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM MachineCheckPoint WHERE checkpointid = @checkpointid", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@checkpointid", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                //conn.Close();
                return i;
            }
        }
        //DeleteBreakdown
        public int DeleteBreakdown(int id)
        {
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {


                SqlCommand cmd = new SqlCommand("DELETE FROM MachineBreakdown WHERE breakid = @breakid", conn)
                {
                    CommandType = CommandType.Text
                };
                cmd.Parameters.AddWithValue("@breakid", id);
                conn.Open();
                int i = cmd.ExecuteNonQuery();
              //  conn.Close();
                return i;
            }
        }

    
        public int ChangeCompany(MachineMaintenanceMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_CompanyUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", hld.id);
                    cmd.Parameters.AddWithValue("@compcode", hld.compcode);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }
        public int ChangeResponsibility(MachineMaintenanceMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_ResponsibilityUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", hld.id);
                    cmd.Parameters.AddWithValue("@responsibility", hld.responsibility);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

        public int ChangeCheckResponsibility(MachineMaintenanceMDI hld)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineName_ResponsibilityUpdate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", hld.id);
                    cmd.Parameters.AddWithValue("@responsibility", hld.responsibility);
                    cmd.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserName());
                    i = cmd.ExecuteNonQuery();
                }
                return i;
            }
        }

      
        public List<MachineMaintenanceMDI> GetAll_ScheduleTaskByCompany(int compcode = 0)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("MachineMaintenanceSchedule_SMS", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@CompCode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI
                        {
                            id = Convert.ToInt32(reader["id"]),
                            compcode = Convert.ToInt32(reader["compcode"]),
                            ShortName = reader["ShortName"].ToString(),
                            scheduleid = Convert.ToInt32(reader["scheduleid"]),
                            schedulename = reader["schedulename"].ToString(),
                            machineid = Convert.ToInt32(reader["machineid"]),
                            machinename = reader["machinename"].ToString(),
                            NewEmpId = Convert.ToInt32(reader["NewEmpId"]),
                            EmpName = reader["EmpName"].ToString(),
                            userid = Convert.ToInt32(reader["userid"]),
                            FullName = reader["FullName"].ToString(),

                            checkpointid = Convert.ToInt32(reader["checkpointid"]),
                            checkdetail = reader["checkdetail"].ToString(),
                            detailofworkdone = reader["detailofworkdone"].ToString(),
                            condition = reader["condition"].ToString(),
                            duedate = reader["duedate"].ToString(),
                            assignee = reader["Assignee"].ToString(),
                            UpdatedOn = reader["UpdatedOn"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString(),
                            UpdatedBy = reader["UpdatedBy"].ToString(),
                            Doc = Convert.ToInt32(reader["Doc"]),
                            NextDueDate = reader["NextDueDate"].ToString(),
                           //Active = Convert.ToBoolean(reader["Active"]),
                            //NotUpdatedSinceDays = Convert.ToInt32(reader["NotUpdatedSinceDays"]),
                            //ExternalMan = reader["ExternalMan"].ToString(),

                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<MachineMaintenanceMDI> GetAllServiceReport(int id = 0)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Machine_GetAll_ServiceReport", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI();

                       task.id = Convert.ToInt32(reader["id"]);
                       task.FileId = Convert.ToInt32(reader["FileId"]);
                       task.FileName = reader["FileName"].ToString();
                        task.FileContent = (byte[])reader["FileContent"];
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<MachineMaintenanceMDI> GetHistoryById(int Id = 0)
        {
            List<MachineMaintenanceMDI> tasklist = new List<MachineMaintenanceMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Machine_Maintenance_AuditHistory", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id",Id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MachineMaintenanceMDI task = new MachineMaintenanceMDI();

                        task.id = Convert.ToInt32(reader["id"]);
                        task.ShortName = reader["ShortName"].ToString();
                        task.machinename = reader["MachineName"].ToString();
                        task.EmpName = reader["EmpName"].ToString();
                        task.FullName = reader["FullName"].ToString();
                        task.condition = reader["condition"].ToString();
                        task.detailofworkdone = reader["detailofworkdone"].ToString();
                        task.duedate = reader["duedate"].ToString();
                        task.schedulename = reader["schedulename"].ToString();
                        task.UpdatedBy = reader["UpdatedBy"].ToString();
                        task.UpdatedOn = reader["UpdatedOn"].ToString();
                        task.ExternalMan = reader["ExternalMan"].ToString();


                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }


    }
}