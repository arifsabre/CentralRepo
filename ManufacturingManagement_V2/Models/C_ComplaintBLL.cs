using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class C_ComplaintBLL
    {
        //
        //internal DbSet<CorrespondenceMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static C_ComplaintBLL Instance
        {
            get { return new C_ComplaintBLL(); }
        }
        private void insertcomplaint(SqlCommand cmd, C_ComplaintMDI dbobject)
        {
          
            cmd.Parameters.Add(mc.getPObject("@srno", dbobject.srno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dateofcomplaintrecieved", dbobject.dateofcomplaintrecieved.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@customername", dbobject.customername.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@detail", dbobject.detail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@recievedby", dbobject.recievedby.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@priority", dbobject.priority.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@status", dbobject.status.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobile1", dbobject.mobile1.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@mobile2", dbobject.mobile2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@createdby", objCookie.getUserName().Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.newempid, DbType.String));



        }
        private void updatecomplaint(SqlCommand cmd, C_ComplaintMDI dbobject)
        {
           // cmd.Parameters.Add(mc.getPObject("@referenceno", dbobject.referenceno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@srno", dbobject.srno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dateofcomplaintrecieved", dbobject.dateofcomplaintrecieved.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@customername", dbobject.customername, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@detail", dbobject.detail, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@recievedby", dbobject.recievedby, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@priority", dbobject.priority, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@status", dbobject.status, DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@attendeddate", dbobject.attendeddate.ToShortDateString(), DbType.DateTime));
            //cmd.Parameters.Add(mc.getPObject("@observation", dbobject.observation.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@rootcause", dbobject.rootcause.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@actiontaken", dbobject.actiontaken.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@detailofitemreplaced", dbobject.detailofitemreplaced.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobile1", dbobject.mobile1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobile2", dbobject.mobile2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@userid1", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@updatedby", objCookie.getUserName(), DbType.String));
       }
        private void Closedcomplaint(SqlCommand cmd, C_ComplaintMDI dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@srno", dbobject.srno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dateofcomplaintrecieved", dbobject.dateofcomplaintrecieved.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@customername", dbobject.customername, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@detail", dbobject.detail, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@recievedby", dbobject.recievedby, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@priority", dbobject.priority, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@status", dbobject.status, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@attendeddate", dbobject.attendeddate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dateofclosed", dbobject.dateofclosed.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@observation", dbobject.observation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rootcause", dbobject.rootcause.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@actiontaken", dbobject.actiontaken.Trim(), DbType.String));
           // cmd.Parameters.Add(mc.getPObject("@detailofitemreplaced", dbobject.detailofitemreplaced.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobile1", dbobject.mobile1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobile2", dbobject.mobile2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@userid1", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@updatedby", objCookie.getUserName(), DbType.String));
        }
        internal void closedObject(C_ComplaintMDI dbobject)
        {
            Result = false;

            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_complaint_Closed";
                Closedcomplaint(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@referenceno", dbobject.referenceno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_complaint, dbobject.referenceno.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_ComplaintBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        private List<C_ComplaintMDI> createObjectList(DataSet ds)
        {
            List<C_ComplaintMDI> listObj = new List<C_ComplaintMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_ComplaintMDI objmdl = new C_ComplaintMDI();
                objmdl.referenceno = Convert.ToInt32(dr["referenceno"].ToString());
                objmdl.srno = Convert.ToInt32(dr["srno"].ToString());
                objmdl.pragno = dr["pragno"].ToString();//d
                objmdl.compcode = Convert.ToInt32(dr["compcode"].ToString());
                objmdl.cmpname = dr["cmpname"].ToString();//d
                objmdl.shortname = dr["shortname"].ToString();//d
                objmdl.itemid = Convert.ToInt32(dr["itemid"].ToString());
                objmdl.itemname = dr["itemname"].ToString();
                objmdl.dateofcomplaintrecieved = Convert.ToDateTime(dr["dateofcomplaintrecieved"].ToString());
                objmdl.customername = dr["customername"].ToString();//d
                objmdl.recievedby = dr["recievedby"].ToString();//d
                objmdl.detail = dr["detail"].ToString();//d
                objmdl.priority = dr["priority"].ToString();//d
                objmdl.status = dr["status"].ToString();//d
                objmdl.attendeddate = Convert.ToDateTime(dr["attendeddate"].ToString());
                objmdl.dateofclosed = Convert.ToDateTime(dr["dateofclosed"].ToString());
                objmdl.observation = dr["observation"].ToString();
                objmdl.rootcause = dr["rootcause"].ToString();
                objmdl.detailofitemreplaced = dr["detailofitemreplaced"].ToString();
                objmdl.newempid = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpName = dr["EmpName"].ToString();//d
                objmdl.mobile1 = dr["mobile1"].ToString();//d
                objmdl.mobile2 = dr["mobile2"].ToString();//d
                //objmdl.createdby = dr["createdby"].ToString();
                //objmdl.createdon = Convert.ToDateTime(dr["createdon"].ToString());
                //objmdl.updatedby = objCookie.getUserName();
                //objmdl.updatedon = Convert.ToDateTime(dr["updatedon"].ToString());
                //objmdl.userid = Convert.ToInt32(dr["userid"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }
        public List<C_ComplaintMDI> GetAll_ComplaintsByCompany(int compcode)
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_complaint_getByCompId]", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        C_ComplaintMDI objmdl = new C_ComplaintMDI();
                        objmdl.referenceno = Convert.ToInt32(dr["referenceno"].ToString());
                        objmdl.pragsrno = dr["pragno"].ToString();//d
                        objmdl.compcode = Convert.ToInt32(dr["compcode"].ToString());
                        objmdl.cmpname = dr["cmpname"].ToString();//d
                        objmdl.shortname = dr["shortname"].ToString();//d
                        objmdl.itemid = Convert.ToInt32(dr["itemid"].ToString());
                        objmdl.itemname = dr["itemname"].ToString();
                        objmdl.dateofcomplaintrecieved = Convert.ToDateTime(dr["dateofcomplaintrecieved"].ToString());
                        objmdl.detail = dr["detail"].ToString();
                        objmdl.customername = dr["customername"].ToString();
                        objmdl.recievedby = dr["recievedby"].ToString();//d
                        objmdl.status = dr["status"].ToString();//d
                        objmdl.EmpName = dr["EmpName"].ToString();//d
                                                                //objmdl.createdby = dr["createdby"].ToString();
                                                                //objmdl.createdon = Convert.ToDateTime(dr["createdon"].ToString());
                        tasklist.Add(objmdl);
                    }
                }
            }
            return tasklist;
        }
        private bool checkSetValidModel(C_ComplaintMDI dbobject)
        {
            //if (dbobject.pragno == null)
            //{
            //    dbobject.pragno = "";
            //}
            //if (dbobject.cmpname == null)
            //{
            //    dbobject.cmpname = "";
            //}
            //if (dbobject.itemname == null)
            //{
            //    dbobject.itemname = "";
            //}
            //if (dbobject.invoiceno == null)
            //{
            //    dbobject.invoiceno = "";
            //}
            //if (dbobject.shedname == null)
            //{
            //    dbobject.shedname = "";
            //}
           return true;
        }
        internal void insertObject(C_ComplaintMDI dbobject)
        {

            Result = false;
            // if (checkSetValidModel(dbobject) == false) { return; };
            //if (checkSetValidModel(dbobject) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_complaint_Insert";
                insertcomplaint(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.referenceno = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.c_complaint, "referenceno"));
                mc.setEventLog(cmd, dbTables.c_complaint, dbobject.referenceno.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("C_ComplaintBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        internal void updateObject(C_ComplaintMDI dbobject)
        {
            Result = false;

            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_complaint_Update";
                updatecomplaint(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@referenceno", dbobject.referenceno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_complaint, dbobject.referenceno.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_ComplaintBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        internal C_ComplaintMDI searchObject(int referenceno)
        {
            DataSet ds = new DataSet();
            C_ComplaintMDI dbobject = new C_ComplaintMDI();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "[c_complaint_getByRefrence]";
            cmd.Parameters.Add(mc.getPObject("@referenceno", referenceno, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            return dbobject;
        }
        internal void deleteComplaint (int referenceno)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_complaint_Delete";
                cmd.Parameters.Add(mc.getPObject("@referenceno", referenceno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_complaint, referenceno.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_ComplaintBLL", "deleteObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        public List<C_ComplaintMDI> GetComplaintNo()
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_complaint_getComplaintNo]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        C_ComplaintMDI task = new C_ComplaintMDI
                        {
                            referenceno = Convert.ToInt32(reader["referenceno"]),
                            pragno = reader["pragno"].ToString(),
                    };
                    tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<C_ComplaintMDI> ComplaintAssignTo()
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_complaint_AssignToEngineer]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        C_ComplaintMDI task = new C_ComplaintMDI
                        {
                            referenceno = Convert.ToInt32(reader["referenceno"]),
                            EmpName = reader["EmpName"].ToString(),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<C_ComplaintMDI> ComplaintClosed()
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_complaintDone]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        C_ComplaintMDI task = new C_ComplaintMDI
                        {
                            referenceno = Convert.ToInt32(reader["referenceno"]),
                            dateofclosed = Convert.ToDateTime(reader["dateofclosed"]),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }
        internal List<C_Fitting_ItemMDI> getItemListWithWaranty(int compcode = 0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
            cmd.CommandText = "c_GetItem_Waranty";
            mc.fillFromDatabase(ds, cmd);
            List<C_Fitting_ItemMDI> depmdl = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.itemid = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.itemname = dr["ItemName"].ToString();
                depmdl.Add(objmdl);
            }
            return depmdl;
        }
        internal List<C_Fitting_ItemMDI> getPragSRNoWithWaranty(int compcode = 0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
            cmd.CommandText = "c_GetPragSrno_Waranty";
            mc.fillFromDatabase(ds, cmd);
            List<C_Fitting_ItemMDI> depmdl = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.srno = Convert.ToInt32(dr["srno"].ToString());
                objmdl.pragno = dr["pragno"].ToString();
                depmdl.Add(objmdl);
            }
            return depmdl;
        }
        //ShedName
        private void insertShedTraning(SqlCommand cmd, C_ComplaintMDI dbobject)
        {

            cmd.Parameters.Add(mc.getPObject("@ShedName", dbobject.ShedName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Trainner", dbobject.Tranner, DbType.String));
           
            cmd.Parameters.Add(mc.getPObject("@StartDate", dbobject.StartDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@EndDate", dbobject.EndDate.ToShortDateString(), DbType.DateTime));

            cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CreatedBy", objCookie.getUserName().Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.newempid, DbType.String));
        }
        private void updateShedTraning(SqlCommand cmd, C_ComplaintMDI dbobject)
        {
            // cmd.Parameters.Add(mc.getPObject("@referenceno", dbobject.referenceno, DbType.Int32));
            //cmd.Parameters.Add(mc.getPObject("@ShecId", dbobject.ShecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ShedName", dbobject.ShedName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Trainner", dbobject.Tranner, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StartDate", dbobject.StartDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@EndDate", dbobject.EndDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@UpdatedBy", objCookie.getUserName(), DbType.String));
        }
        private List<C_ComplaintMDI> ShedObjectList(DataSet ds)
        {
            List<C_ComplaintMDI> listObj = new List<C_ComplaintMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_ComplaintMDI objmdl = new C_ComplaintMDI();
                objmdl.ShedId = Convert.ToInt32(dr["ShedId"].ToString());
                objmdl.ShedName = dr["ShedName"].ToString();//d
                objmdl.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                objmdl.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                objmdl.Tranner = dr["Trainner"].ToString();//d
                objmdl.createdby = dr["CreatedBy"].ToString();
                objmdl.createdon = Convert.ToDateTime(dr["CreatedOn"].ToString());
                objmdl.updatedby = objCookie.getUserName();
                objmdl.updatedon = Convert.ToDateTime(dr["UpdatedOn"].ToString());
                objmdl.userid = Convert.ToInt32(dr["UserId"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }
        internal void insertShedObject(C_ComplaintMDI dbobject)
        {

            Result = false;
            // if (checkSetValidModel(dbobject) == false) { return; };
            //if (checkSetValidModel(dbobject) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_ShedTraining_Insert";
                insertShedTraning(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.ShedId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.c_Shed_Traning, "ShedId"));
                mc.setEventLog(cmd, dbTables.c_Shed_Traning, dbobject.ShedId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("C_ComplaintBLL", "insertShedObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        internal void updateShedObject(C_ComplaintMDI dbobject)
        {
            Result = false;

            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_ShedTraining_Update";
                updateShedTraning(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@ShedId", dbobject.ShedId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_Shed_Traning, dbobject.ShedId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_ComplaintBLL", "updateShedObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        internal C_ComplaintMDI searchShedObject(int ShedId)
        {
            DataSet ds = new DataSet();
            C_ComplaintMDI dbobject = new C_ComplaintMDI();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "[c_ShedTraining_GetByShedId]";
            cmd.Parameters.Add(mc.getPObject("@ShedId", ShedId, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = ShedObjectList(ds)[0];
                }
            }
            return dbobject;
        }

        public List<C_ComplaintMDI> GetTop1ShedSaved()
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_ShedTraining_GetTop1All]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        C_ComplaintMDI task = new C_ComplaintMDI
                        {
                           
                            ShedName = reader["ShedName"].ToString(),
                            Tranner = reader["Trainner"].ToString(),
                            StartDate = Convert.ToDateTime(reader["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(reader["EndDate"].ToString()),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        public List<C_ComplaintMDI> GetAllShedList()
        {
            List<C_ComplaintMDI> tasklist = new List<C_ComplaintMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("[c_ShedTraining_GetAll]", conn))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    //  cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        C_ComplaintMDI task = new C_ComplaintMDI
                        {
                            ShedId = Convert.ToInt32(reader["ShedId"].ToString()),
                            ShedName = reader["ShedName"].ToString(),
                            Tranner = reader["Trainner"].ToString(),
                            StartDate = Convert.ToDateTime(reader["StartDate"].ToString()),
                            EndDate = Convert.ToDateTime(reader["EndDate"].ToString()),
                        };
                        tasklist.Add(task);
                    }

                }
            }
            return tasklist;
        }

        internal void deleteshed(int ShedId)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "c_ShedTraining_Delete";
                cmd.Parameters.Add(mc.getPObject("@ShedId", ShedId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_Shed_Traning, ShedId.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_ComplaintBLL", "deleteshed", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
    }
}