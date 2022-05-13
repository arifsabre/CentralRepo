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
    public class AccountBLL : DbContext
    {
        //
        //internal DbSet<AccountMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters_AcMast(SqlCommand cmd, AccountMdl dbobject, recType rectype)
        {
            DataSet ds = getInfoToCreateAccount(dbobject.GrCode.ToString());
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@acdesc", dbobject.AcDesc.Trim(), DbType.String));//accode
            cmd.Parameters.Add(mc.getPObject("@rectype", Convert.ToChar(rectype).ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@cash", ds.Tables[0].Rows[0]["cash"].ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@grcode", dbobject.GrCode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@actype", ds.Tables[0].Rows[0]["actype"].ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@lev", Convert.ToInt32(ds.Tables[0].Rows[0]["lev"].ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@bsheet", dbobject.BSheet, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@modif", "1", DbType.String));
            cmd.Parameters.Add(mc.getPObject("@shortname", dbobject.ShortName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@aliasname", dbobject.AliasName.Trim(), DbType.String));
        }
        //
        private DataSet getInfoToCreateAccount(string grcode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_info_to_create_account";
            cmd.Parameters.Add(mc.getPObject("@grcode", grcode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        private void addCommandParameters_Address(SqlCommand cmd, AccountMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@contper", dbobject.Address.ContPer.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@hacdesc", dbobject.Address.HAcDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@address1", dbobject.Address.Address1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@address2", dbobject.Address.Address2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@address3", dbobject.Address.Address3.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@address4", dbobject.Address.Address4.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@tinno", dbobject.Address.TinNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@tindate", dbobject.Address.TinDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@phoneoff", dbobject.Address.PhoneOff.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@phoneresi", dbobject.Address.PhoneResi.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@faxno", dbobject.Address.FaxNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@mobileno", dbobject.Address.MobileNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@email", dbobject.Address.Email.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@areaid", dbobject.Address.AreaId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@crlimit", dbobject.Address.CrLimit, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@partydiscount", dbobject.Address.PartyDiscount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@crdays", dbobject.Address.CrDays, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@customertype", dbobject.Address.CustomerType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@cstno", dbobject.Address.CstNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@cstdate", dbobject.Address.CstDate, DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@categoryid", dbobject.Address.CategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RailwayId", mc.getForSqlIntString(dbobject.Address.RailwayId.ToString()), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@gstinno", dbobject.Address.GSTinNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateCode", dbobject.Address.StateCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateName", dbobject.Address.StateName.Trim(), DbType.String));
        }
        //
        private List<AccountMdl> createObjectList_Account(DataSet ds)
        {
            List<AccountMdl> objlist = new List<AccountMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AccountMdl objmdl = new AccountMdl();
                objmdl.RecId = Convert.ToInt32(dr["recid"].ToString());
                objmdl.CompCode = Convert.ToInt16(dr["compcode"].ToString());
                //objmdl.CmpName = dr["cmpname"].ToString();//d
                objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                objmdl.AcDesc = dr["acdesc"].ToString();
                objmdl.RecType = dr["rectype"].ToString();
                objmdl.Cash = dr["cash"].ToString();
                objmdl.GrCode = Convert.ToInt32(dr["grcode"].ToString());
                objmdl.GrDesc = dr["grdesc"].ToString();
                objmdl.AcType = dr["actype"].ToString();
                objmdl.Lev = Convert.ToInt32(dr["lev"].ToString());
                objmdl.BSheet = Convert.ToBoolean(dr["bsheet"].ToString());
                objmdl.Modif = Convert.ToBoolean(dr["modif"].ToString());
                objmdl.ShortName = dr["shortname"].ToString();
                objmdl.AliasName = dr["aliasname"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<AccountAddressMdl> createObjectList_Address(DataSet ds)
        {
            List<AccountAddressMdl> objlist = new List<AccountAddressMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AccountAddressMdl objmdl = new AccountAddressMdl();
                objmdl.RecId = Convert.ToInt32(dr["recid"].ToString());
                objmdl.HAcDesc = dr["hacdesc"].ToString();
                objmdl.ContPer = dr["contper"].ToString();
                objmdl.Address1 = dr["address1"].ToString();
                objmdl.Address2 = dr["address2"].ToString();
                objmdl.Address3 = dr["address3"].ToString();
                objmdl.Address4 = dr["address4"].ToString();
                objmdl.TinNo = dr["tinno"].ToString();
                objmdl.TinDate = Convert.ToDateTime(dr["tindate"].ToString());
                objmdl.PhoneOff = dr["phoneoff"].ToString();
                objmdl.PhoneResi = dr["phoneresi"].ToString();
                objmdl.FaxNo = dr["faxno"].ToString();
                objmdl.MobileNo = dr["mobileno"].ToString();
                objmdl.Email = dr["email"].ToString();
                objmdl.AreaId = Convert.ToInt32(dr["areaid"].ToString());
                objmdl.CrLimit = Convert.ToDouble(dr["crlimit"].ToString());
                objmdl.PartyDiscount = Convert.ToDouble(dr["partydiscount"].ToString());
                objmdl.CrDays = Convert.ToInt32(dr["crdays"].ToString());
                objmdl.CustomerType = dr["customertype"].ToString();
                objmdl.CstNo = dr["cstno"].ToString();
                objmdl.CstDate = Convert.ToDateTime(dr["cstdate"].ToString());
                objmdl.CategoryId = Convert.ToInt32(dr["categoryid"].ToString());
                objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                objmdl.Railway = dr["RailwayName"].ToString();
                objmdl.GSTinNo = dr["GSTinNo"].ToString();
                objmdl.StateCode = dr["StateCode"].ToString();
                objmdl.StateName = dr["StateName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isValidShortName(string shortname)
        {
            if (shortname == "")
            {
                Message = "Short name not entered!";
                return false;
            }
            if (shortname.Length != 3)
            {
                Message = "Short name must be of 3 characters!";
                return false;
            }
            return true;
        }
        //
        private bool CheckDuplicate_Insert(string acdesc)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_insert_tbl_acmast";
            cmd.Parameters.Add(mc.getPObject("@acdesc", acdesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate account name not allowed!";
            return false;
        }
        //
        private bool CheckDuplicate_Insert_ShortName(string shortname, string grcode)
        {
            if (grcode != Convert.ToInt64(fAccount.SundryCreditors).ToString() && shortname == "") { return true; };
            if (grcode == Convert.ToInt64(fAccount.SundryCreditors).ToString() && isValidShortName(shortname) == false) { return false; };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_insert_tbl_acmast_for_shortname";
            cmd.Parameters.Add(mc.getPObject("@shortname", shortname, DbType.String));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate short name entry not allowed!";
            return false;
        }
        //
        private bool CheckDuplicate_Update(string acdesc, string accode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_update_tbl_acmast";
            cmd.Parameters.Add(mc.getPObject("@acdesc", acdesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate account name not allowed!";
            return false;
        }
        //
        private bool CheckDuplicate_Update_ShortName(string shortname, string accode, string grcode)
        {
            if (grcode != Convert.ToInt64(fAccount.SundryCreditors).ToString() && shortname == "") { return true; };
            if (grcode == Convert.ToInt64(fAccount.SundryCreditors).ToString() && isValidShortName(shortname) == false) { return false; };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_update_tbl_acmast_for_shortname";
            cmd.Parameters.Add(mc.getPObject("@shortname", shortname, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate short name entry not allowed!";
            return false;
        }
        //
        private bool CheckDuplicate_AliasName(string aliasname, string accode)
        {
            if (aliasname == "") { return true; };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkduplicate_alias_tbl_acmast";
            cmd.Parameters.Add(mc.getPObject("@aliasname", aliasname, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate alias name entry is not allowed!";
            return false;
        }
        //
        private string getRecIdByAccode(int accode, int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recid_accode";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        private bool checkSetValidModel_Account(AccountMdl dbobject)
        {
            if (dbobject.GrCode == 0)
            {
                Message = "Group not selected!";
                return false;
            }
            if (dbobject.ShortName == null)
            {
                dbobject.ShortName = "";
            }
            if (dbobject.AliasName == null)
            {
                dbobject.AliasName = "";
            }
            if (dbobject.DrCr == null)
            {
                dbobject.DrCr = "d";
            }
            return true;
        }
        //
        private bool checkSetValidModel_Address(AccountMdl dbobject)
        {
            //note
            if (dbobject.Railway == null)
            {
                dbobject.RailwayId = 0;
            }
            dbobject.Address.RailwayId = dbobject.RailwayId;
            //cl--note

            if (dbobject.Address.ContPer == null)
            {
                dbobject.Address.ContPer = "";
            }
            if (dbobject.Address.HAcDesc == null)
            {
                dbobject.Address.HAcDesc = "";
            }
            if (dbobject.Address.Address1 == null)
            {
                dbobject.Address.Address1 = "";
            }
            if (dbobject.Address.Address2 == null)
            {
                dbobject.Address.Address2 = "";
            }
            if (dbobject.Address.Address3 == null)
            {
                dbobject.Address.Address3 = "";
            }
            if (dbobject.Address.Address4 == null)
            {
                dbobject.Address.Address4 = "";
            }
            if (dbobject.Address.TinNo == null)
            {
                dbobject.Address.TinNo = "";
            }
            if (mc.isValidDate(dbobject.Address.TinDate) == false)
            {
                dbobject.Address.TinDate = DateTime.Now;
            }
            if (dbobject.Address.PhoneOff == null)
            {
                dbobject.Address.PhoneOff = "";
            }
            if (dbobject.Address.PhoneResi == null)
            {
                dbobject.Address.PhoneResi = "";
            }
            if (dbobject.Address.FaxNo == null)
            {
                dbobject.Address.FaxNo = "";
            }
            if (dbobject.Address.MobileNo == null)
            {
                dbobject.Address.MobileNo = "";
            }
            if (dbobject.Address.Email == null)
            {
                dbobject.Address.Email = "";
            }
            if (dbobject.Address.AreaId == 0)
            {
                dbobject.Address.AreaId = 1;
            }
            if (dbobject.Address.CustomerType == null)
            {
                dbobject.Address.CustomerType = "x";
            }
            if (dbobject.Address.Address1 == null)
            {
                dbobject.Address.Address1 = "";
            }
            if (dbobject.Address.CstNo == null)
            {
                dbobject.Address.CstNo = "";
            }
            if (mc.isValidDate(dbobject.Address.CstDate) == false)
            {
                dbobject.Address.CstDate = DateTime.Now;
            }
            if (dbobject.Address.CategoryId == 0)
            {
                dbobject.Address.CategoryId = 1;
            }
            if (dbobject.Address.GSTinNo == null)
            {
                dbobject.Address.GSTinNo = "";
            }
            if (dbobject.Address.StateCode == null)
            {
                dbobject.Address.StateCode = "";
            }
            if (dbobject.Address.StateName == null)
            {
                dbobject.Address.StateName = "";
            }
            return true;
        }
        //
        private bool checkSetValidModel_Address(AccountAddressMdl dbobject)
        {
            //if (dbobject.NewEmpId == 0)
            //{
            //    Message = "Employee not selected!";
            //    return false;
            //}
            //if (mc.isValidDate(dbobject.AdvDate) == false)
            //{
            //    Message = "Invalid advance date!";
            //    return false;
            //}
            //if (mc.isSalaryLockedForUpdation(dbobject.AdvDate) == true)
            //{
            //    Message = "Salary for this month/year has been locked! So entry/updation is not allowed!";
            //    return false;
            //}
            return true;
        }
        //
        private bool ToTblAcTrans(SqlCommand cmd, int accode, double opbalance, string drcr, string optpost)
        {
            bool res = false;
            try
            {
                //for current finyear
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_actrans";
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@closing", "0", DbType.Double));
                if (drcr == "d")
                {
                    cmd.Parameters.Add(mc.getPObject("@yobdr", opbalance, DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@yobcr", "0", DbType.Double));
                }
                else if (drcr == "c")
                {
                    cmd.Parameters.Add(mc.getPObject("@yobdr", "0", DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@yobcr", opbalance, DbType.Double));
                }
                cmd.Parameters.Add(mc.getPObject("@drcr", drcr, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@balance", "0", DbType.String));
                cmd.ExecuteNonQuery();
                if (optpost == "actpost")
                {
                    if (performAccountPosting(cmd, accode, opbalance, drcr, postDirection.Forward) == false) { return false; };
                }
                //for other financial years
                //not required here, it is managed in voucher-posting procedures
                //DataSet ds = getAllFinYearsExceptCurrentFinYear(cmd);
                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    cmd.Parameters.Clear();
                //    cmd.CommandText = "usp_insert_tbl_actrans";
                //    cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                //    cmd.Parameters.Add(mc.getPObject("@finyear", ds.Tables[0].Rows[i]["finyear"].ToString(), DbType.String));
                //    cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                //    cmd.Parameters.Add(mc.getPObject("@closing", "0", DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@yobdr", "0", DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@yobcr", "0", DbType.Double));
                //    cmd.Parameters.Add(mc.getPObject("@drcr", "d", DbType.String));//dr by default
                //    cmd.Parameters.Add(mc.getPObject("@balance", "0", DbType.String));
                //    cmd.ExecuteNonQuery();
                //}
                res = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AccountBLL", "ToTblAcTrans", ex.Message);
            }
            return res;
        }
        //
        private void addAddressToList(DataSet ds)
        {
            AccountAddressMdl objadd = new AccountAddressMdl();
            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //{
            //    objadd = searchAccount(ds.Tables[0].Rows[i]["accode"].ToString());
            //    if (objadd.AcCode != null && objadd.Address1.Length != 0)
            //    {
            //        ds.Tables[0].Rows[i]["acdesc"] = ds.Tables[0].Rows[i]["acdesc"] + "  (" + objadd.Address1 + ")";
            //    }
            //}
        }
        //
        private DataSet getAllFinYearsExceptCurrentFinYear(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_all_finyears_except_current_finyear";
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            return ds;
        }
        //
        private bool CheckToDelete_Account(int accode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_check_to_delete_account";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "This group/account can not be deleted!";
            return false;
        }
        //
        private DataSet getAccountOpBalance(int accode, int ccode=0, string finyear="")
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            if (finyear == "") { finyear = objCookie.getFinYear(); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_account_opbalance";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        private DataSet getAccountOpBalance(SqlCommand cmd, int accode, int ccode= 0, string finyear = "")
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            if (finyear == "") { finyear = objCookie.getFinYear(); };
            DataSet ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_account_opbalance";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            return ds;
        }
        //
        private bool performAccountPosting(SqlCommand cmd, int accode, double amount, string drcr, postDirection postdir)
        {
            bool res = false;
            try
            {
                double amt = Convert.ToDouble(amount);
                cmd.Parameters.Clear();
                if (postdir == postDirection.Forward)
                {
                    if (drcr == "c") { amt = -amt; };
                }
                else if (postdir == postDirection.Reverse)
                {
                    if (drcr == "d") { amt = -amt; };
                }
                cmd.CommandText = "usp_account_posting";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@amount", amt.ToString(), DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AccountBLL", "doAccountPosting", ex.Message);
            }
            return res;
        }
        //
        private bool checkValidAccountForTinNo(string accode)
        {
            string ret = "";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tinno_for_account";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            ret = mc.getFromDatabase(cmd);
            if (ret == "") { return false; };
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void Insert_Record_AccountGroup(AccountMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel_Account(dbobject) == false) { return; };
            if (CheckDuplicate_Insert(dbobject.AcDesc) == false) { return; };
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
                cmd.CommandText = "usp_get_new_accode";
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                int accode = Convert.ToInt32(mc.getFromDatabase(cmd, conn));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_acmast";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                addCommandParameters_AcMast(cmd, dbobject, recType.Group);
                cmd.ExecuteNonQuery();
                if (ToTblAcTrans(cmd, accode, 0, "d", "") == false) { return; };
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_acmast, "recid");
                mc.setEventLog(cmd, dbTables.tbl_acmast, recid, "insert");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AccountBLL", "Insert_Record_AccountGroup", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void Insert_Record_AccountMaster(AccountMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel_Account(dbobject) == false) { return; };
            if (checkSetValidModel_Address(dbobject) == false) { return; };
            if (CheckDuplicate_Insert(dbobject.AcDesc) == false) { return; };
            if (CheckDuplicate_AliasName(dbobject.AliasName, "0") == false) { return; };
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
                //to account master
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_new_accode";
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                int accode = Convert.ToInt32(mc.getFromDatabase(cmd, conn));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_acmast";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                addCommandParameters_AcMast(cmd, dbobject, recType.Account);
                cmd.ExecuteNonQuery();
                //to address
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_address";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                addCommandParameters_Address(cmd, dbobject);
                cmd.ExecuteNonQuery();
                if (ToTblAcTrans(cmd, accode, dbobject.OpBalance, dbobject.DrCr, "actpost") == false) { return; };
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_acmast, "recid");
                mc.setEventLog(cmd, dbTables.tbl_acmast, recid, "insert");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AccountBLL", "Insert_Record_AccountMaster", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void Update_Record_AccountGroup(int accode, AccountMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel_Account(dbobject) == false) { return; };
            if (CheckDuplicate_Update(dbobject.AcDesc, accode.ToString()) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_acmast";
                addCommandParameters_AcMast(cmd, dbobject, recType.Group);
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.ExecuteNonQuery();
                string recid = getRecIdByAccode(accode);
                mc.setEventLog(cmd, dbTables.tbl_acmast, recid, "update");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AccountBLL", "Update_Record_AccountGroup", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void Update_Record_AccountMaster(int accode, AccountMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel_Account(dbobject) == false) { return; };
            if (checkSetValidModel_Address(dbobject) == false) { return; };
            if (CheckDuplicate_Update(dbobject.AcDesc, accode.ToString()) == false) { return; };
            if (CheckDuplicate_AliasName(dbobject.AliasName, accode.ToString()) == false) { return; };
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
                //reverse account posting
                DataSet ds = getAccountOpBalance(cmd, accode);
                if (performAccountPosting(cmd, accode, Convert.ToDouble(ds.Tables[0].Rows[0]["opening"].ToString()), ds.Tables[0].Rows[0]["drcr"].ToString(), postDirection.Reverse) == false) { return; };
                //
                //account master
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_acmast";
                addCommandParameters_AcMast(cmd, dbobject, recType.Account);
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.ExecuteNonQuery();
                //address
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_address";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                addCommandParameters_Address(cmd, dbobject);
                cmd.ExecuteNonQuery();
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_actrans_for_opbalance";
                if (dbobject.DrCr == "d")
                {
                    cmd.Parameters.Add(mc.getPObject("@yobdr", dbobject.OpBalance, DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@yobcr", "0", DbType.Double));
                }
                else if (dbobject.DrCr == "c")
                {
                    cmd.Parameters.Add(mc.getPObject("@yobdr", "0", DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@yobcr", dbobject.OpBalance, DbType.Double));
                }
                cmd.Parameters.Add(mc.getPObject("@drcr", dbobject.DrCr, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.ExecuteNonQuery();
                //
                if (performAccountPosting(cmd, accode, dbobject.OpBalance, dbobject.DrCr, postDirection.Forward) == false) { return; };
                //
                string recid = getRecIdByAccode(accode);
                mc.setEventLog(cmd, dbTables.tbl_acmast, recid, "update account");
                //
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AccountBLL", "Update_Record_AccountMaster", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void Delete_Account(int accode)
        {
            Result = false;
            if (CheckToDelete_Account(accode) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                string recid = getRecIdByAccode(accode);
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_account";
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_acmast, recid, "Account Deleted");
                trn.Commit();
                Message = "Account Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AccountBLL", "Delete_Account", ex.Message);
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
        internal double getCurrentBalance(string accode, int ccode=0, string finyear="")
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            if (finyear == "") { finyear = objCookie.getFinYear(); };
            string ret = "0";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_current_balance";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            ret = mc.getFromDatabase(cmd);
            if (ret == "") { ret = "0"; };
            double dblcbalance = Convert.ToDouble(ret);
            return dblcbalance;
        }
        //
        internal string getdrcr(string accode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_drcr";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //
        internal AccountMdl searchAccountGroup(int accode)
        {
            DataSet ds = new DataSet();
            AccountMdl dbobject = new AccountMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_acmast";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList_Account(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal AccountMdl searchAccountMaster(int accode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            AccountMdl dbobject = new AccountMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_acmast";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            //account master
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList_Account(ds)[0];
                }
            }
            //account details (if any)
            dbobject.Address = searchAccountAddress(accode,ccode);
            
            //note
            dbobject.RailwayId = dbobject.Address.RailwayId;
            dbobject.Railway = dbobject.Address.Railway;
            //cl--note

            //opening balance (if any)
            DataSet opinfo = getAccountOpBalance(accode);
            if (opinfo.Tables.Count > 0)
            {
                if (opinfo.Tables[0].Rows.Count > 0)
                {
                    dbobject.OpBalance = Math.Abs(Convert.ToDouble(opinfo.Tables[0].Rows[0]["opening"].ToString()));
                    dbobject.DrCr = opinfo.Tables[0].Rows[0]["drcr"].ToString();
                }
            }
            return dbobject;
        }
        //
        internal AccountAddressMdl searchAccountAddress(int accode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            AccountAddressMdl dbobject = new AccountAddressMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_address";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList_Address(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal DataSet getAccountListByRecType(recType rectype, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistbyrectype";//usp_fill_account_search_list
            cmd.Parameters.Add(mc.getPObject("@rectype", Convert.ToChar(rectype).ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            addAddressToList(ds);
            return ds;
        }
        //
        internal DataSet getAccountListByGroup(fAccount grcode, cashType cshtype, string rtype, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_getaccountlistbygroup_and_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_getaccountlistbygroup";//usp_fill_account_by_group
            }
            cmd.Parameters.Add(mc.getPObject("@grcode", Convert.ToInt64(grcode).ToString(), DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@cash", Convert.ToChar(cshtype).ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            addAddressToList(ds);
            return ds;
        }
        //
        internal DataSet getAccountListRailway(string railwayid, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistbyrailway";
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getAccountListForSundryDebtors(string rtype = "0", int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_getaccountlistforsundrydebtor_and_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_getaccountlistforsundrydebtor";
            }
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet fillAccountListByCashType(string cash, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistbycashtype";//usp_getaccounts_by_cashtype
            cmd.Parameters.Add(mc.getPObject("@cash", cash, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet fillAccountListForCashNdBank(int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistforcashndbank";//usp_getaccounts_of_cashndbank
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet fillAccountListByCategory(string categoryid, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistbycategory";//usp_fill_account_search_list_by_category
            cmd.Parameters.Add(mc.getPObject("@categoryid", categoryid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //

        internal DataSet getAccountWithGroupData(recType rectype, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getaccountlistwithgroup";//usp_get_accounts_with_group
            cmd.Parameters.Add(mc.getPObject("@rectype", Convert.ToChar(rectype).ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        internal List<AccountMdl> getAccountWithGroupList(recType rectype, int ccode=0)
        {
            DataSet ds = getAccountWithGroupData(rectype,ccode);
            List<AccountMdl> objlist = new List<AccountMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AccountMdl objmdl = new AccountMdl();
                objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                objmdl.AcDesc = dr["acdesc"].ToString();
                objmdl.GrCode = Convert.ToInt32(dr["grcode"].ToString());
                objmdl.GrDesc = dr["grdesc"].ToString();
                objmdl.Modif = Convert.ToBoolean(dr["modif"].ToString());
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //

        internal string getAccountName(string accode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_accountname";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        internal string getGroupCode(string accode, int ccode =0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_groupcode";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        internal string getCashTypeByAcCode(string accode, int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_cashtype";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        internal void performBalancePosting(SqlCommand cmd, string accode, string amount, int ccode, string finyear)
        {
            //[100072]
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_balance_posting";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@amount", amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        /// <summary>
        /// Returns shortname withn current transaction
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="accode"></param>
        /// <returns></returns>
        internal string getShortName(SqlCommand cmd, string accode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_shortname";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd, cmd.Connection);
        }
        //
        /// <summary>
        /// Returns shortname
        /// </summary>
        /// <param name="accode"></param>
        /// <returns></returns>
        internal string getShortName(string accode, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_shortname";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        internal string getAcCodeByAcctName(SqlCommand cmd, string acdesc, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_accode_by_acdesc";
            cmd.Parameters.Add(mc.getPObject("@acdesc", acdesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd, cmd.Connection);
        }
        //
        internal string getAcCodeByAliasName(SqlCommand cmd, string aliasname, int ccode=0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_accode_by_aliasname";
            cmd.Parameters.Add(mc.getPObject("@aliasname", aliasname, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd, cmd.Connection);
        }
        //
        internal string getCreditLimit(string accode, int ccode = 0)
        {
            if (ccode == 0) { ccode = Convert.ToInt16(objCookie.getCompCode()); };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_credit_limit";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            return mc.getFromDatabase(cmd);
        }
        //
        #endregion
        //
    }
}