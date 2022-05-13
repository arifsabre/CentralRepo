using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ManufacturingManagement_V2.Models
{
    public class LoginBLL
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        //
        clsEncryption ec = new clsEncryption();
        clsCookie objCoockie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, LoginMdl objlogin)
        {
            cmd.Parameters.Add(mc.getPObject("@username", objlogin.UserName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@passw", ec.Encrypt(objlogin.PassW), DbType.String));
        }
        //
        private void setLogin(string userid)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_SetUserLogin";
                cmd.Parameters.Add(mc.getPObject("@LgDateNdTime", DateTime.Now.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("LoginDAL", "SetLogin", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        internal bool isValidPassword(string passw, int luserid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_checkusertochangepassword";
            cmd.Parameters.Add(mc.getPObject("@passw", ec.Encrypt(passw), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@userid", luserid, DbType.Int32));//objCoockie.getUserId()
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == false)
            {
                Message = "Invalid Password!";
                return false;
            }
            return true;
        }
        //
        private bool performCheck()
        {
            //DateTime dtChk = new DateTime(2017, 10, 31);
            //DateTime dtNow = DateTime.Now;
            //if (dtNow > dtChk)
            //{
            //    return false;
            //}
            return true;
        }
        //
        private DataSet getFinYearDates(string compcode, string finyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getfromdatetodateforcompanyndfinyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal void getUserLoginInfo(LoginMdl objlogin)
        {
            Result = false;
            if (performCheck() == false) { return; };
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_GetUserLoginInfo";
            addCommandParameters(cmd, objlogin);
            DataSet ds = new DataSet();
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count == 0)
            {
                Message = "Invalid User Id or Password!";
                return;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                Message = "Invalid User Id or Password!";
                return;
            }
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["isactive"].ToString()) == false)
            {
                Message = "This User Id has been locked!<br/>Contact to system administrator.";
                return;
            }
            objlogin.UserId = Convert.ToInt32(ds.Tables[0].Rows[0]["userid"].ToString());
            objlogin.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
            objlogin.FullName = ds.Tables[0].Rows[0]["FullName"].ToString();
            objlogin.LoginType = Convert.ToInt32(ds.Tables[0].Rows[0]["logintype"].ToString());
            objlogin.Department = ds.Tables[0].Rows[0]["department"].ToString();
            objlogin.MobileNo = ds.Tables[0].Rows[0]["MobileNo"].ToString();
            objlogin.EMail = ds.Tables[0].Rows[0]["EMail"].ToString();
            Result = true;
        }
        //
        internal void performLogin(string userid, string compcode, string finyear, string otp, bool fromV1 = false, string depcode = "Home", string smenu = "")
        {
            Result = false;
            if (performCheck() == false) { return; };
            DataSet dsdates = getFinYearDates(compcode, finyear);
            DateTime dtfromdate = Convert.ToDateTime(dsdates.Tables[0].Rows[0]["fromdate"].ToString());
            DateTime dttodate = Convert.ToDateTime(dsdates.Tables[0].Rows[0]["todate"].ToString());
            DateTime dtdisptodate = DateTime.Now;
            if (Convert.ToDateTime(DateTime.Now.ToShortDateString()) > Convert.ToDateTime(dttodate.ToShortDateString()))
            {
                dtdisptodate = dttodate;
            }
            //get other variables by user info to create cookie
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_users";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            DataSet ds = new DataSet();
            mc.fillFromDatabase(ds, cmd);
            //getting dept-menu-list
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_department_menu_list_for_user";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int32));
            string deptmenulist = mc.getFromDatabase(cmd);
            //get company name
            CompanyBLL cmpbll = new CompanyBLL();
            string cmpname = mc.getNameByKey(cmpbll.getObjectData(), "compcode", compcode, "cmpname");

            //create user log
            if (fromV1 == false)
            {
                UserLogBLL ulbll = new UserLogBLL();
                ulbll.insertObject(userid, ds.Tables[0].Rows[0]["mobileno"].ToString(), otp);
            }
            //create coockie
            objCoockie.createCookie(userid, ds.Tables[0].Rows[0]["fullname"].ToString(), ds.Tables[0].Rows[0]["logintype"].ToString(), compcode, cmpname, finyear, dtfromdate, dttodate, dtdisptodate, depcode, smenu, deptmenulist);
            setLogin(userid);
            Result = true;
        }
        //
        #region coockie change management
        //
        internal void changeCoockieForCompany(string compcode)
        {
            clsEncryption ec = new clsEncryption();
            HttpContext hc = HttpContext.Current;
            CompanyBLL cmpbll = new CompanyBLL();
            string cmpname = mc.getNameByKey(cmpbll.getObjectData(), "compcode", compcode, "cmpname");
            string userid = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvuserid"].ToString());
            string username = hc.Request.Cookies["ckmfgmgtv2"]["cvusername"].ToString();
            string lgtype = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvlgtype"].ToString());
            string finyear = hc.Request.Cookies["ckmfgmgtv2"]["cvfinyear"].ToString();
            DateTime dtfromdate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvfromdate"].ToString());
            DateTime dttodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvtodate"].ToString());
            DateTime dtdisptodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvdisptodate"].ToString());
            string depcode = hc.Request.Cookies["ckmfgmgtv2"]["cvdepcode"].ToString();
            string smenu = hc.Request.Cookies["ckmfgmgtv2"]["cvsmenu"].ToString();
            string deptmenulist = hc.Request.Cookies["ckmfgmgtv2"]["cvdeptmenulist"].ToString();
            //removing existing values
            objCoockie.RemoveCookie(false);
            //creating changed for company
            objCoockie.createCookie(userid, username, lgtype, compcode, cmpname, finyear, dtfromdate, dttodate, dtdisptodate, depcode, smenu, deptmenulist);
        }
        //
        internal void changeCoockieForFinancialYear(string finyear)
        {
            clsEncryption ec = new clsEncryption();
            HttpContext hc = HttpContext.Current;
            string userid = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvuserid"].ToString());
            string username = hc.Request.Cookies["ckmfgmgtv2"]["cvusername"].ToString();
            string lgtype = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvlgtype"].ToString());
            string compcode = hc.Request.Cookies["ckmfgmgtv2"]["cvcompcode"].ToString();
            string compname = hc.Request.Cookies["ckmfgmgtv2"]["cvcmpname"].ToString();
            string depcode = hc.Request.Cookies["ckmfgmgtv2"]["cvdepcode"].ToString();
            string smenu = hc.Request.Cookies["ckmfgmgtv2"]["cvsmenu"].ToString();
            string deptmenulist = hc.Request.Cookies["ckmfgmgtv2"]["cvdeptmenulist"].ToString();
            DataSet dsdates = getFinYearDates(compcode, finyear);
            DateTime dtfromdate = Convert.ToDateTime(dsdates.Tables[0].Rows[0]["fromdate"].ToString());
            DateTime dttodate = Convert.ToDateTime(dsdates.Tables[0].Rows[0]["todate"].ToString());
            DateTime dtdisptodate = DateTime.Now;
            if (Convert.ToDateTime(DateTime.Now.ToShortDateString()) > Convert.ToDateTime(dttodate.ToShortDateString()))
            {
                dtdisptodate = dttodate;
            }
            //removing existing values
            objCoockie.RemoveCookie(false);
            //creating changed for company
            objCoockie.createCookie(userid, username, lgtype, compcode, compname, finyear, dtfromdate, dttodate, dtdisptodate, depcode, smenu, deptmenulist);
        }
        //
        internal void changeCoockieForDepartment(string depcode)
        {
            clsEncryption ec = new clsEncryption();
            HttpContext hc = HttpContext.Current;
            string compcode = hc.Request.Cookies["ckmfgmgtv2"]["cvcompcode"].ToString();
            string cmpname = hc.Request.Cookies["ckmfgmgtv2"]["cvcmpname"].ToString();
            string userid = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvuserid"].ToString());
            string username = hc.Request.Cookies["ckmfgmgtv2"]["cvusername"].ToString();
            string lgtype = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvlgtype"].ToString());
            string finyear = hc.Request.Cookies["ckmfgmgtv2"]["cvfinyear"].ToString();
            DateTime dtfromdate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvfromdate"].ToString());
            DateTime dttodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvtodate"].ToString());
            DateTime dtdisptodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvdisptodate"].ToString());
            string smenu = hc.Request.Cookies["ckmfgmgtv2"]["cvsmenu"].ToString();
            string deptmenulist = hc.Request.Cookies["ckmfgmgtv2"]["cvdeptmenulist"].ToString();
            //removing existing values
            objCoockie.RemoveCookie(false);
            //creating changed for company
            objCoockie.createCookie(userid, username, lgtype, compcode, cmpname, finyear, dtfromdate, dttodate, dtdisptodate, depcode, smenu, deptmenulist);
        }
        //
        internal void changeCoockieForSelectedMenu(string smenu)
        {
            clsEncryption ec = new clsEncryption();
            HttpContext hc = HttpContext.Current;
            string compcode = hc.Request.Cookies["ckmfgmgtv2"]["cvcompcode"].ToString();
            string cmpname = hc.Request.Cookies["ckmfgmgtv2"]["cvcmpname"].ToString();
            string userid = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvuserid"].ToString());
            string username = hc.Request.Cookies["ckmfgmgtv2"]["cvusername"].ToString();
            string depcode = hc.Request.Cookies["ckmfgmgtv2"]["cvdepcode"].ToString();
            string lgtype = ec.Decrypt(hc.Request.Cookies["ckmfgmgtv2"]["cvlgtype"].ToString());
            string finyear = hc.Request.Cookies["ckmfgmgtv2"]["cvfinyear"].ToString();
            DateTime dtfromdate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvfromdate"].ToString());
            DateTime dttodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvtodate"].ToString());
            DateTime dtdisptodate = Convert.ToDateTime(hc.Request.Cookies["ckmfgmgtv2"]["cvdisptodate"].ToString());
            string deptmenulist = hc.Request.Cookies["ckmfgmgtv2"]["cvdeptmenulist"].ToString();
            //removing existing values
            objCoockie.RemoveCookie(false);
            //creating changed for company
            objCoockie.createCookie(userid, username, lgtype, compcode, cmpname, finyear, dtfromdate, dttodate, dtdisptodate, depcode, smenu, deptmenulist);
        }
        //
        #endregion
        //
        internal bool isValidForPasswordPolicy(string passw)
        {
            if (passw.Contains(" "))
            {
                Message = "Blank spaces are not allowed!";
                return false;
            }
            if (passw.Length < 10)
            {
                Message = "Password must have minimum 10 characters!";
                return false;
            }
            if (passw.Length > 50)
            {
                Message = "Password length must not be greater than 50 characters!";
                return false;
            }
            //string PasswordPattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*0-9]{6,50}$";
            string PasswordPattern = @"^(?=.{10,}$)(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).*$";
            if (!Regex.IsMatch(passw, PasswordPattern))
            {
                Message = "The Password must have at least one upper case alphabet, one lower case alphabet, one numeric and one special character!";
                return false;
            }
            return true;
        }
        //
        internal void updatePassword(string oldpassw, string newpassw, int luserid)
        {
            Result = false;
            Message = "";

            //check password policy
            if (isValidForPasswordPolicy(newpassw) == false) { return; };
            //validity of existing password
            if (isValidPassword(oldpassw, luserid) == false) { return; };
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_ChangePassword";
                cmd.Parameters.Add(mc.getPObject("@passw", ec.Encrypt(newpassw.Trim()), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@userid", luserid, DbType.Int32));//objCoockie.getUserId()
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_users, luserid.ToString(), "Password Changed");//objCoockie.getUserId()
                trn.Commit();
                Result = true;
                Message = "Password Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("LoginDAL", "UpdatePassword", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void forgetPassword(string username, string empid)
        {
            Result = false;
            Message = "";
            if (username == null) { username = ""; };
            if (empid == null) { empid = ""; };
            if (username.Length == 0 && empid.Length == 0)
            {
                Message = "Either User Id or Employee Id must be entered!";
                return;
            }
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_email_to_send_password";
            cmd.Parameters.Add(mc.getPObject("@username", username, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@empid", empid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows[0]["username"].ToString().Length == 0)
            {
                Message = "Either valid User Name or Employee Id must be provided!";
                return;
            }
            else
            {
                System.Collections.ArrayList arl = new System.Collections.ArrayList();
                arl.Add(ds.Tables[0].Rows[0]["email"].ToString());
                string emailmsg = "";
                emailmsg = "<b>Dear "+ds.Tables[0].Rows[0]["fullname"].ToString()+",<br/>";
                emailmsg += "Kindly note your ERP UserId and Password as-<br/>";
                //
                string reportmatter = "<table border='1' cellpadding='4' cellspacing='0' style='font-size:10pt;'>";
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'><b>UserId</b></td>";
                reportmatter += "<td valign='top'>" + ds.Tables[0].Rows[0]["username"].ToString() + "</td>";
                reportmatter += "</tr>";
                reportmatter += "<tr>";
                reportmatter += "<td valign='top'><b>Password</b></td>";
                reportmatter += "<td valign='top'>" + ec.Decrypt(ds.Tables[0].Rows[0]["passw"].ToString()) + "</td>";
                reportmatter += "</tr>";
                reportmatter += "</table>";
                //
                emailmsg += reportmatter;
                emailmsg += "<br/>Note: Please update your password after receiving this email.";
                emailmsg += "<br/><br/>Regards:<br/>PRAG ERP System";
                string st = mc.SendEmailByERP(arl, "PRAG ERP USERID/PASSWORD", emailmsg);
                Message = st;
            }
        }
        //
    }
}