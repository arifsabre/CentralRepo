using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ManufacturingManagement_V2.Models
{
    public class UserBLL : DbContext
    {
        //
        internal DbSet<UserBLL> Users { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsEncryption ec = new clsEncryption();
        clsCookie objCookie = new clsCookie();
        public static UserBLL Instance
        {
            get { return new UserBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, UserMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@UserName", dbobject.UserName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Title", dbobject.Title.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FullName", dbobject.FullName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PassW", ec.Encrypt(dbobject.PassW.Trim()), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LoginType", dbobject.LoginType, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@department", dbobject.Department, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MobileNo", dbobject.MobileNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsActive", dbobject.IsActive, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmpId", mc.getForSqlIntString(dbobject.EmpId.ToString()), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@HODUserId", dbobject.HODUserId, DbType.Int32));
        }
        //
        private List<UserMdl> createObjectList(DataSet ds)
        {
            clsEncryption enc = new clsEncryption();
            List<UserMdl> users = new List<UserMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UserMdl objmdl = new UserMdl();
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                if (dr.Table.Columns.Contains("username"))
                {
                    objmdl.UserName = dr["username"].ToString();
                }
                if (dr.Table.Columns.Contains("passw"))
                {
                    objmdl.PassW = enc.Decrypt(dr["passw"].ToString());
                }
                if (dr.Table.Columns.Contains("loginType"))
                {
                    objmdl.LoginType = Convert.ToInt16(dr["LoginType"].ToString());
                    objmdl.LoginName = mc.getNameByKey(mc.getLoginTypes(), "logintype", dr["logintype"].ToString(), "logintypename");
                }
                if (dr.Table.Columns.Contains("Department"))
                {
                    objmdl.Department = dr["Department"].ToString();
                }
                if (dr.Table.Columns.Contains("MobileNo"))
                {
                    objmdl.MobileNo = dr["MobileNo"].ToString();
                }
                if (dr.Table.Columns.Contains("IsActive"))
                {
                    objmdl.IsActive = Convert.ToBoolean(dr["isactive"].ToString());
                }
                if (dr.Table.Columns.Contains("EMail"))
                {
                    objmdl.EMail = dr["EMail"].ToString();
                }
                if (dr.Table.Columns.Contains("FullName"))
                {
                    objmdl.FullName = dr["FullName"].ToString();
                }
                if (dr.Table.Columns.Contains("Title"))
                {
                    objmdl.Title = dr["Title"].ToString();
                }
                if (dr.Table.Columns.Contains("EmpId"))
                {
                    objmdl.EmpId = Convert.ToInt32(dr["EmpId"].ToString());
                }
                if (dr.Table.Columns.Contains("EmpName"))
                {
                    objmdl.EmpName = dr["EmpName"].ToString();
                }
                if (dr.Table.Columns.Contains("HODUserID"))
                {
                    objmdl.HODUserId = Convert.ToInt32(dr["HODUserId"].ToString());
                }
                if (dr.Table.Columns.Contains("HODUserName"))
                {
                    objmdl.HODUserName = dr["HODUserName"].ToString();
                }
                users.Add(objmdl);
            }
            return users;
        }
        //
        private bool isAlreadyFound(string username)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_insert_tbl_users";
            cmd.Parameters.Add(mc.getPObject("@UserName", username, DbType.String));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate user not allowed!";
            return false;
        }
        //
        private bool isFoundFullName(string fullname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_users_fullname";
            cmd.Parameters.Add(mc.getPObject("@fullname", fullname, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate full name not allowed!";
                return true;
            }
            return false;
        }
        //
        private void saveToCompUsers(SqlCommand cmd, UserMdl dbobject)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_compusers";
            cmd.Parameters.Add(mc.getPObject("@userid", dbobject.UserId, DbType.Int32));
            cmd.ExecuteNonQuery();
            for (int i = 0; i < dbobject.SelectedCompanies.ToArray().Length; i++)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_compusers";
                cmd.Parameters.Add(mc.getPObject("@userid", dbobject.UserId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.SelectedCompanies[i].ToString(), DbType.Int16));
                cmd.ExecuteNonQuery();
            }
        }
        //
        private bool isFoundMobileNo(string MobileNo,int userid)
        {
            if (MobileNo.Length == 0) { return false; };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_users_mobileno";
            cmd.Parameters.Add(mc.getPObject("@MobileNo", MobileNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == false) { return false; };
            Message = "Duplicate contact no not allowed!";
            return true;
        }
        //
        private bool checkSetValidModel(UserMdl dbobject)
        {
            //note: 1=admin
            if (dbobject.HODUserId == 0)
            {
                dbobject.HODUserId = 1;
            }
            if (dbobject.MobileNo == null)
            {
                dbobject.MobileNo = "";
            }
            if (dbobject.MobileNo.Length > 0 && dbobject.MobileNo.Length != 10)
            {
                Message = "Invalid mobile no! It must of 10 digits.";
                return false;
            }
            if (dbobject.EMail == null)
            {
                dbobject.EMail = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal DataSet getCompCodesByUserId(int userid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_compcodes_by_userid";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal void insertObject(UserMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.UserName) == false) { return; };
            if (isFoundFullName(dbobject.FullName) == true) { return; };
            if (isFoundMobileNo(dbobject.MobileNo,0) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_users";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@LgDateNdTime", DateTime.Now.ToString(), DbType.DateTime));
                cmd.ExecuteNonQuery();
                dbobject.UserId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_users, "userid"));
                saveToCompUsers(cmd, dbobject);
                trn.Commit();
                Result = true;
                Message = "User Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("UserAdminDAL", "insertUser", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(UserMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isFoundMobileNo(dbobject.MobileNo, dbobject.UserId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_users";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@userid", dbobject.UserId, DbType.Int32));
                cmd.ExecuteNonQuery();
                saveToCompUsers(cmd, dbobject);
                trn.Commit();
                Result = true;
                Message = "User Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_users") == true)
                {
                    Message = "Duplicate user not allowed!";
                }
                else if (ex.Message.Contains("uk_fullname_tbl_users") == true)
                {
                    Message = "Duplicate full name not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("UserAdminDAL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateUserProfile(UserMdl dbobject)
        {
            Result = false;
            if (dbobject.UserName == null)
            {
                Message = "User Id not entered!";
                return;
            }
            if (dbobject.UserName.Length < 8)
            {
                Message = "User Id must have minimum 8 characters!";
                return;
            }
            if (dbobject.FullName == null)
            {
                Message = "User name not entered!";
                return;
            }
            if (dbobject.MobileNo == null)
            {
                Message = "Mobile number not entered!";
                return;
            }
            if (dbobject.EMail == null)
            {
                Message = "Email not entered!";
                return;
            }
            if (dbobject.MobileNo.Length > 0 && dbobject.MobileNo.Length != 10)
            {
                Message = "Invalid mobile no! It must of 10 digits.";
                return;
            }
            if ((dbobject.UserName + dbobject.MobileNo + dbobject.EMail).Contains(" ") == true)
            {
                Message = "Blank spaces are not allowed!";
                return;
            }
            if (isFoundMobileNo(dbobject.MobileNo, dbobject.UserId) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_user_profile";
                cmd.Parameters.Add(mc.getPObject("@UserName", dbobject.UserName.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@Title", dbobject.Title.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@FullName", dbobject.FullName.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@MobileNo", dbobject.MobileNo.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@userid", dbobject.UserId, DbType.Int32));//objCookie.getUserId()
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_users, dbobject.UserId.ToString(),"Profile Updated");//objCookie.getUserId()
                trn.Commit();
                Result = true;
                Message = "User Profile Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_users") == true)
                {
                    Message = "Duplicate user not allowed!";
                }
                else if (ex.Message.Contains("uk_fullname_tbl_users") == true)
                {
                    Message = "Duplicate full name not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("UserAdminDAL", "updateProfile", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void lockUserId(string userid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_lockuserid";
                cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Message = "UserId Locked";
                Result = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserAdminDAL", "lockUserId", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int userid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_users";
                cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Message = "User Deleted";
                Result = true;
            }
            catch
            {
                Message = mc.setErrorLog("UserAdminDAL", "deleteUser", "This record cannot be deleted!\n\rIt has been used further.");
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal List<UserCompany> getAllCompanyList()
        {
            DataSet allcmp = CompanyBLL.Instance.getObjectData();
            List<UserCompany> allcmplist = new List<UserCompany> { };
            foreach (DataRow dr in allcmp.Tables[0].Rows)
            {
                UserCompany objmdl = new UserCompany();
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.Company = dr["CmpName"].ToString();
                allcmplist.Add(objmdl);
            }
            return allcmplist;
        }
        //
        internal List<UserCompany> getUserCompanyList(int userid)
        {
            DataSet usercmp = getCompCodesByUserId(userid);
            List<UserCompany> usercmplist = new List<UserCompany> { };
            foreach (DataRow dr in usercmp.Tables[0].Rows)
            {
                UserCompany objmdl = new UserCompany();
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.Company = "ABC";
                usercmplist.Add(objmdl);
            }
            return usercmplist;
        }
        //
        internal UserMdl searchObject(int userid)
        {
            DataSet ds = new DataSet();
            UserMdl dbobject = new UserMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_users";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.AllCompanies = getAllCompanyList();
            dbobject.UserCompanies = getUserCompanyList(userid);
            //set selected companies
            dbobject.SelectedCompanies = dbobject.UserCompanies.Select(x => x.CompCode).ToArray();
            return dbobject;
        }
        //
        internal DataSet getObjectData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_display_tbl_users";//"usp_get_tbl_users";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UserMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal DataSet getUsersByPermissionData(int entryid,int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_users_by_permission";
            cmd.Parameters.Add(mc.getPObject("@entryid", entryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UserMdl> getUsersByPermission(int entryid,int compcode)
        {
            DataSet ds = getUsersByPermissionData(entryid,compcode);
            return createObjectList(ds);
        }
        //
        internal DataSet getUsersList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_fill_user_search_list";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getHODUserList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_fill_hod_user_search_list";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        private DataSet getLogfRecordInfo(SqlCommand cmd, string tblid, string pkval)
        {
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getLogfRecordInfo";
            cmd.Parameters.Add(mc.getPObject("@tblid", tblid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@pkval", pkval, DbType.String));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            return ds;
        }
        //
        internal string udfGetLatestModifyInfo(dbTables tblname, string pkval)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_latest_modify_info";//dbp by v1
            cmd.Parameters.Add(mc.getPObject("@tblid", Convert.ToInt32(tblname).ToString(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@pkval", pkval, DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //
        internal DataSet getEventLogData(DateTime dtfrom, DateTime dtto, string userid, string compcode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_eventlog";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            DataSet ds = new DataSet();
            mc.fillFromDatabase(ds, cmd);
            DataSet dsrecinfo = new DataSet();
            //iteration within same/single open connection
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dsrecinfo = new DataSet();
                        ds.Tables[0].Rows[i]["tblname"] = mc.getTableName(ds.Tables[0].Rows[i]["tblid"].ToString());
                        dsrecinfo = getLogfRecordInfo(cmd, ds.Tables[0].Rows[i]["tblid"].ToString(), ds.Tables[0].Rows[i]["pkval"].ToString());
                        ds.Tables[0].Rows[i]["pkname"] = dsrecinfo.Tables[0].Rows[0]["pkname"].ToString();
                        ds.Tables[0].Rows[i]["recinfo"] = dsrecinfo.Tables[0].Rows[0]["recinfo"].ToString();
                        ds.Tables[0].Rows[i]["compcode"] = dsrecinfo.Tables[0].Rows[0]["compcode"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                Message = mc.setErrorLog("UserBLL", "getEventLogData", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            //
            return ds;
        }
        //
        internal void deleteEventLog(string eventid)
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
                cmd.CommandText = "usp_delete_tbl_eventlog";
                cmd.Parameters.Add(mc.getPObject("@eventid", eventid, DbType.Int32));
                cmd.ExecuteNonQuery();
                Result = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UserAdminDAL", "deleteEventLog", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal DataSet getDepartmentData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_department";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        internal List<DepartmentMdl> getDepartmentList()
        {
            DataSet ds = getDepartmentData();
            List<DepartmentMdl> depmdl = new List<DepartmentMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepartmentMdl objmdl = new DepartmentMdl();
                objmdl.DepCode = dr["DepCode"].ToString();
                objmdl.Department = dr["Department"].ToString();
                depmdl.Add(objmdl);
            }
            return depmdl;
        }
        //
        internal List<EntryInfoMdl> getEntryInfoList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_entryinfo";
            mc.fillFromDatabase(ds, cmd);
            List<EntryInfoMdl> objlist = new List<EntryInfoMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EntryInfoMdl objmdl = new EntryInfoMdl();
                objmdl.TblId = Convert.ToInt32(dr["TblId"].ToString());
                objmdl.TblName = dr["TblName"].ToString();
                objmdl.PKField = dr["PKField"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getUserTrainingList(string schdno, string trnstatus)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            //cmd.CommandText = "usp_get_user_training_list";
            cmd.CommandText = "usp_get_user_training_list_html";
            cmd.Parameters.Add(mc.getPObject("@schdno", schdno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@trnstatus", trnstatus, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}