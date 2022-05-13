using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_MedicalTestBLL
    {
        internal string Message { get; set; }
        internal bool Result { get; set; }
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPConnection"].ToString());
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public List<AAA_MedicalTest> Get_EmpMedicalList_History()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;

            List<AAA_MedicalTest> HList = new List<AAA_MedicalTest>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_GetEmpMedicalTest_History", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_MedicalTest
                    {
                        MedicalId = Convert.ToInt32(rdr["MedicalId"]),
                        EmpId = rdr["EmpId"].ToString(),
                        EmpName = rdr["EmpName"].ToString(),
                        NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                        Age = Convert.ToInt32(rdr["Age"]),
                        Weight = Convert.ToDecimal(rdr["Weight"]),
                        Hemoglobin = rdr["Hemoglobin"].ToString(),
                        Pulse = rdr["Pulse"].ToString(),
                        Oxygen = rdr["Oxygen"].ToString(),
                        Sugar = rdr["Sugar"].ToString(),
                        TestDate = Convert.ToDateTime(rdr["TestDate"]),
                        Bp = rdr["Bp"].ToString(),
                        Bplow = rdr["Bplow"].ToString(),
                        BloodGroup = rdr["BloodGroup"].ToString(),
                        Height = rdr["Height"].ToString(),
                        EyeSight = rdr["EyeSight"].ToString(),
                        Allergies = rdr["Allergies"].ToString(),
                        HealthCondition = rdr["HealthCondition"].ToString(),
                        Remark = rdr["Remark"].ToString(),
                        cmpname = rdr["ShortName"].ToString(),
                        GradeName = rdr["GradeName"].ToString(),
                        // GradeName = rdr["GradeName"].ToString(),
                    });
                }
                return HList;
            }
        }
        public List<AAA_MedicalTest> GetEmpVaccineList()
        {
            List<AAA_MedicalTest> tasklist = new List<AAA_MedicalTest>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Medical_GeAllVaccineList", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@compcode", compcode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        AAA_MedicalTest task = new AAA_MedicalTest
                        {
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            NewEmpId = Convert.ToInt32(reader["NewEmpId"]),
                            EmpName = reader["EmpName"].ToString(),
                            EmpId = reader["EmpId"].ToString(),
                            VacType = reader["VacType"].ToString(),
                            VacId = Convert.ToInt32(reader["VacId"]),
                            Dose1Date = Convert.ToDateTime(reader["Dose1Date"]),
                            Dose1 = Convert.ToBoolean(reader["Dose1"]),
                            Dose2Date = Convert.ToDateTime(reader["Dose2Date"]),
                            Dose2 = Convert.ToBoolean(reader["Dose2"]),
                        };
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
        public List<AAA_MedicalTest> Get_EmpMedicalNewEMPId()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_Get_EMPName_To_Issue";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_MedicalTest> ls = new List<AAA_MedicalTest> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_MedicalTest
                {

                    NewEmpId = Convert.ToInt32(dr["NewEmpId"]),
                    EmpName = dr["EmpName"].ToString()
                });
            }
            return ls;
        }

        public List<AAA_MedicalTest> GetEmpListByCompcode(int compcode=0)
        {
            List<AAA_MedicalTest> tasklist = new List<AAA_MedicalTest>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Medical_GetEmpListByCompany]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Compcode", objCookie.getCompCode());
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        AAA_MedicalTest task = new AAA_MedicalTest
                        {
                            NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                            EmpName = rdr["EmpName"].ToString(),
                          
                        };

                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }
       public List<AAA_MedicalTest> Get_EmpMedicalVacType()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "Medical_GetVacType";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_MedicalTest> ls = new List<AAA_MedicalTest> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_MedicalTest
                {

                    VacId = Convert.ToInt32(dr["VacId"]),
                    VacType = dr["VacType"].ToString()
                });
            }
            return ls;
        }
        public int Update_Emp_Medical_Test(AAA_MedicalTest hlu)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_Update_EmpMedicalTest", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@MedicalId", hlu.MedicalId);
                com.Parameters.AddWithValue("@NewEmpId", hlu.NewEmpId);
                com.Parameters.AddWithValue("@Weight", hlu.Weight);
                com.Parameters.AddWithValue("@Hemoglobin", hlu.Hemoglobin);
                com.Parameters.AddWithValue("@Pulse", hlu.Pulse);
                com.Parameters.AddWithValue("@Oxygen", hlu.Oxygen);
                com.Parameters.AddWithValue("@Sugar", hlu.Sugar);
                com.Parameters.AddWithValue("@Bp", hlu.Bp);
                com.Parameters.AddWithValue("@Bplow", hlu.Bplow);
                com.Parameters.AddWithValue("@BloodGroup", hlu.BloodGroup);
                com.Parameters.AddWithValue("@Height", hlu.Height);
                com.Parameters.AddWithValue("@EyeSight", hlu.EyeSight);
                com.Parameters.AddWithValue("@Allergies", hlu.Allergies);
                com.Parameters.AddWithValue("@HealthCondition", hlu.HealthCondition);
                com.Parameters.AddWithValue("@Remark", hlu.Remark);
                com.Parameters.AddWithValue("@UpdateStatus", hlu.UpdateStatus);
                com.Parameters.AddWithValue("@Compcode", hlu.Compcode);
                com.Parameters.AddWithValue("@Grade", hlu.Grade);
                com.Parameters.AddWithValue("@UpdatedBy", objCookie.getUserId());
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Insert_NewEmp_Medical_TestRecord(AAA_MedicalTest hlu)
        {
            clsCookie objCookie = new clsCookie();
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_Insert_EmpMedicalTestNewRecord]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@NewEmpId", hlu.NewEmpId);
                //com.Parameters.AddWithValue("@Age", hlu.Age);
                com.Parameters.AddWithValue("@TestDate", hlu.TestDate);
                com.Parameters.AddWithValue("@Weight", hlu.Weight);
                com.Parameters.AddWithValue("@Hemoglobin", hlu.Hemoglobin);
                com.Parameters.AddWithValue("@Pulse", hlu.Pulse);
                com.Parameters.AddWithValue("@Oxygen", hlu.Oxygen);
                com.Parameters.AddWithValue("@Sugar", hlu.Sugar);
                com.Parameters.AddWithValue("@Bp", hlu.Bp);
                com.Parameters.AddWithValue("@Bplow", hlu.Bplow);
                com.Parameters.AddWithValue("@BloodGroup", hlu.BloodGroup);
                com.Parameters.AddWithValue("@Height", hlu.Height);
                com.Parameters.AddWithValue("@EyeSight", hlu.EyeSight);
                com.Parameters.AddWithValue("@Allergies", hlu.Allergies);
                com.Parameters.AddWithValue("@HealthCondition", hlu.HealthCondition);
                com.Parameters.AddWithValue("@Remark", hlu.Remark);
                com.Parameters.AddWithValue("@Compcode", hlu.Compcode);
                com.Parameters.AddWithValue("@Grade", hlu.Grade);
                com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int DeleteMedicalTest(int id)
        {

            SqlCommand cmd = new SqlCommand("DELETE FROM AAA_EmpMedicalTest WHERE MedicalId = @Item_Id", con)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Item_Id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
       public int DeleteVacRecord(int recordid)
        {
            int i;
            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[Medical_Vaccine_Delete]", conn))
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
        public int Vaccination_Update_Record(AAA_MedicalTest hlu)
        {
            clsMyClass mc = new clsMyClass();
            clsCookie objCookie = new clsCookie();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Medical_Vaccine_Update", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@RecordId", hlu.RecordId);
                com.Parameters.AddWithValue("@VacId", hlu.VacId);
               // com.Parameters.AddWithValue("@Dose1", hlu.Dose1);
                com.Parameters.AddWithValue("@Dose2", hlu.Dose2);
                com.Parameters.AddWithValue("@Dose2Date", hlu.Dose2Date);
                com.Parameters.AddWithValue("@UserId", objCookie.getUserId());
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public List<AAA_MedicalTest> GetEmpVaccineListByRecordId( int RecordId)
        {
            List<AAA_MedicalTest> tasklist = new List<AAA_MedicalTest>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Medical_GeAllVaccineListByRecordId", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", RecordId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        AAA_MedicalTest task = new AAA_MedicalTest
                        {
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            EmpId = reader["EmpId"].ToString(),
                            EmpName = reader["EmpName"].ToString(),
                            VacType = reader["VacType"].ToString(),
                            VacId = Convert.ToInt32(reader["VacId"]),
                            Dose1Date = Convert.ToDateTime(reader["Dose1Date"]),
                            Dose1 = Convert.ToBoolean(reader["Dose1"]),
                            Dose2Date = Convert.ToDateTime(reader["Dose2Date"]),
                            Dose2 = Convert.ToBoolean(reader["Dose2"]),
                        };
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }




        public List<AAA_MedicalTest> AllVaccineCertificate(int RecordId)
        {
            List<AAA_MedicalTest> tasklist = new List<AAA_MedicalTest>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Medical_GeAllVaccineCertificateAll", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@RecordId", RecordId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        AAA_MedicalTest task = new AAA_MedicalTest
                        {
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            Id = Convert.ToInt32(reader["Id"]),
                            FileName = reader["FileName"].ToString(),
                          

                        };
                        tasklist.Add(task);
                    }
                }
            }
            return tasklist;
        }













    }
}