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
    public class UserPermissionBLL : DbContext
    {
        //
        //internal DbSet<UserBLL> Users { get; set; }
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
            //cmd.Parameters.Add(mc.getPObject("@UserName", dbobject.UserName.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@Title", dbobject.Title.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@FullName", dbobject.FullName.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@PassW", ec.Encrypt(dbobject.PassW.Trim()), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@LoginType", dbobject.LoginType, DbType.Int16));
            //cmd.Parameters.Add(mc.getPObject("@department", dbobject.Department, DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@MobileNo", dbobject.MobileNo.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@IsActive", dbobject.IsActive, DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@EmpId", dbobject.EmpId, DbType.String));
        }
        //
        private UserPermissionMdl createObjectList(int userid,int compcode,DataSet ds)
        {
            List<Permission> uper = new List<Permission> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Permission per = new Permission();
                per.EntryId = Convert.ToInt32(dr["EntryId"].ToString());
                per.EntryName = dr["EntryName"].ToString();
                per.AddPer = Convert.ToBoolean(dr["addper"].ToString());
                per.EditPer = Convert.ToBoolean(dr["editper"].ToString());
                per.DeletePer = Convert.ToBoolean(dr["deleteper"].ToString());
                uper.Add(per);
            }
            UserPermissionMdl usersper = new UserPermissionMdl();
            usersper.UserId = userid;
            usersper.CompCode = compcode;
            usersper.Entries = uper;
            return usersper;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void saveUserPermission(UserPermissionMdl dbobject)
        {
            Result = false;
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
                for (int i = 0; i < dbobject.Entries.Count; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_save_userpermission";
                    cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.CompCode, DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@userid", dbobject.UserId, DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@entryid", dbobject.Entries[i].EntryId, DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@addper", dbobject.Entries[i].AddPer, DbType.Boolean));
                    cmd.Parameters.Add(mc.getPObject("@editper", dbobject.Entries[i].EditPer, DbType.Boolean));
                    cmd.Parameters.Add(mc.getPObject("@deleteper", dbobject.Entries[i].DeletePer, DbType.Boolean));
                    cmd.ExecuteNonQuery();
                }
                Result = true;
                Message = "Permission(s) Updated Successfully";
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("UserPermissionDAL", "saveUserPermission", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setDefaultUserPermission(int userid, int defaultcompcode)
        {
            Result = false;
            DataSet ds = CompanyBLL.Instance.getCompanyDataByUser(userid);
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(ds.Tables[0].Rows[i]["compcode"].ToString()) != defaultcompcode)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_set_default_userpermission";
                        cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
                        cmd.Parameters.Add(mc.getPObject("@compcode", ds.Tables[0].Rows[i]["compcode"].ToString(), DbType.Int16));
                        cmd.Parameters.Add(mc.getPObject("@defaultcompcode", defaultcompcode, DbType.Int16));
                        cmd.ExecuteNonQuery();
                    }
                }
                Result = true;
                Message = "Default Permission Updated Successfully";
                trn.Commit();
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("UserPermissionDAL", "setDefaultPermission", ex.Message);
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
        internal UserPermissionMdl getUsersByPermissionForAllEntriesList(int userid, int compcode)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("entryid");
            ds.Tables[0].Columns.Add("entryname");
            ds.Tables[0].Columns.Add("addper", typeof(System.Boolean));
            ds.Tables[0].Columns.Add("editper", typeof(System.Boolean));
            ds.Tables[0].Columns.Add("deleteper", typeof(System.Boolean));
            DataRow dr = ds.Tables[0].NewRow();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet dtper = new DataSet();
            foreach (Entry ent in Enum.GetValues(typeof(Entry)))
            {
                dr = ds.Tables[0].NewRow();
                dr["entryid"] = Convert.ToInt32(ent).ToString();
                dr["entryname"] = ent.ToString();
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_user_permission_for_all_actions";
                cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@entryid", Convert.ToInt32(ent).ToString(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                dtper = new DataSet();
                mc.fillFromDatabase(dtper, cmd);
                dr["addper"] = "false";
                dr["editper"] = "false";
                dr["deleteper"] = "false";
                if (dtper.Tables[0].Rows.Count > 0)
                {
                    dr["addper"] = dtper.Tables[0].Rows[0]["addper"].ToString();
                    dr["editper"] = dtper.Tables[0].Rows[0]["editper"].ToString();
                    dr["deleteper"] = dtper.Tables[0].Rows[0]["deleteper"].ToString();
                }
                ds.Tables[0].Rows.Add(dr);
            }
            return createObjectList(userid, compcode, ds);
        }
        //
        internal List<EntryGroupMdl> getEntryGroupList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_entrygroup";
            mc.fillFromDatabase(ds, cmd);
            List<EntryGroupMdl> eg = new List<EntryGroupMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EntryGroupMdl objmdl = new EntryGroupMdl();
                objmdl.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                objmdl.GroupName = dr["GroupName"].ToString();
                eg.Add(objmdl);
            }
            return eg;
        }
        //
        internal DataSet getEntryDetailData(int groupid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_entrydetail";
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EntryDetailMdl> getEntryDetailList(int groupid)
        {
            DataSet ds = new DataSet();
            ds = getEntryDetailData(groupid);
            List<EntryDetailMdl> ed = new List<EntryDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EntryDetailMdl objmdl = new EntryDetailMdl();
                objmdl.EntryId = Convert.ToInt32(dr["EntryId"].ToString());
                objmdl.EntryName = dr["EntryName"].ToString();
                ed.Add(objmdl);
            }
            return ed;
        }
        //
        #endregion
        //
    }
}