using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ManufacturingManagement_V2.Models
{
    /// <summary>
    /// Summary description for clsMyClass
    /// </summary>
    public class clsCookie
    {
        //
        public string strconn;
        public string Message;
        public bool Result;
        string ckvar = "ckmfgmgtv2";
        //
        public bool checkForAdmin()
        {
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] == null) { return false; };
            if (getLoginType() != 0) { return false; };
            return true;
        }
        //
        public bool checkSessionState()
        {
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] == null) { return false; };
            return true;
        }
        //
        public void createCookie(string userid, string username, string logintype, string compcode, string cmpname, string finyear, DateTime fromdate, DateTime todate, DateTime disptodate, string depcode, string smenu, string deptmenulist)
        {
            HttpContext hc = HttpContext.Current;
            HttpCookie hcookie = new HttpCookie(ckvar);
            clsEncryption ec = new clsEncryption();
            hcookie["cvuserid"] = ec.Encrypt(userid);
            hcookie["cvusername"] = username;
            hcookie["cvlgtype"] = ec.Encrypt(logintype);
            hcookie["cvcompcode"] = compcode;
            hcookie["cvcmpname"] = cmpname;
            hcookie["cvfinyear"] = finyear;
            hcookie["cvfromdate"] = fromdate.ToShortDateString();
            hcookie["cvtodate"] = todate.ToShortDateString();
            hcookie["cvdisptodate"] = disptodate.ToShortDateString();
            hcookie["cvdepcode"] = depcode;
            hcookie["cvsmenu"] = smenu;
            hcookie["cvdeptmenulist"] = deptmenulist;
            hc.Response.Cookies[ckvar].Expires = DateTime.Now.AddHours(10.00);
            //hc.Response.Cookies[ckvar].Expires = DateTime.Now.AddSeconds(10.00);
            hc.Response.Cookies.Add(hcookie);
        }
        //
        public void RemoveCookie(bool forlogout)
        {
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] != null)
            {
                var c = new HttpCookie(ckvar);
                c.Expires = DateTime.Now.AddHours(-10.00);
                //c.Expires = DateTime.Now.AddSeconds(-10.00);
                hc.Response.Cookies.Add(c);
            }
            //logouts from v1 also
            if (forlogout == true)
            {
                if (hc.Request.Cookies["ckmfgmgtv1"] != null)
                {
                    var c = new HttpCookie("ckmfgmgtv1");
                    c.Expires = DateTime.Now.AddHours(-10.00);
                    //c.Expires = DateTime.Now.AddSeconds(-10.00);
                    hc.Response.Cookies.Add(c);
                }
            }
        }
        //
        public void setCookie(string ckname, string ckvalue)
        {
            HttpContext hc = HttpContext.Current;
            HttpCookie hcookie = hc.Request.Cookies[ckvar];
            hcookie[ckname] = ckvalue;
        }
        //
        public string getUserId()
        {
            string userid = "0";
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] == null) { return "0"; };
            userid = hc.Request.Cookies[ckvar]["cvuserid"].ToString();
            clsEncryption ec = new clsEncryption();
            return ec.Decrypt(userid);
        }
        //
        public string getCoockieValueForErpV1(string keyName)
        {
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] == null) { return "0"; };
            if (keyName == "cvuserid" || keyName == "cvlgtype")
            {
                string vl= hc.Request.Cookies[ckvar][keyName].ToString();
                clsEncryption ec = new clsEncryption();
                return ec.Decrypt(vl);
            }
            return hc.Request.Cookies[ckvar][keyName].ToString();
        }
        //
        public string getUserName()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvusername"].ToString();
        }
        //
        public int getLoginType()
        {
            string LoginType;
            HttpContext hc = HttpContext.Current;
            LoginType = hc.Request.Cookies[ckvar]["cvlgtype"].ToString();
            clsEncryption ec = new clsEncryption();
            return Convert.ToInt32(ec.Decrypt(LoginType));
        }
        //
        public string getCompCode()
        {
            HttpContext hc = HttpContext.Current;
            if (hc.Request.Cookies[ckvar] == null) { return "0"; };
            return hc.Request.Cookies[ckvar]["cvcompcode"].ToString();
        }
        //
        public string getCmpName()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvcmpname"].ToString();
        }
        //
        public string getFinYear()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvfinyear"].ToString();
        }
        //
        public DateTime getFromDate()
        {
            HttpContext hc = HttpContext.Current;
            return Convert.ToDateTime(hc.Request.Cookies[ckvar]["cvfromdate"].ToString());
        }
        //
        public DateTime getToDate()
        {
            HttpContext hc = HttpContext.Current;
            return Convert.ToDateTime(hc.Request.Cookies[ckvar]["cvtodate"].ToString());
        }
        //
        public DateTime getDispToDate()
        {
            HttpContext hc = HttpContext.Current;
            return Convert.ToDateTime(hc.Request.Cookies[ckvar]["cvdisptodate"].ToString());
        }
        //
        public string getDepartment()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvdepcode"].ToString();
        }
        //
        public string getMenu()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvsmenu"].ToString();
        }
        //
        public string getDeptMenuList()
        {
            HttpContext hc = HttpContext.Current;
            return hc.Request.Cookies[ckvar]["cvdeptmenulist"].ToString();
        }
        //
        public string getUserLoginInfo()
        {
            //sets userid, username, logintype, loginname, compcode and cmpname 
            //to coockie to use frequently throughout the application
            //clsMyClass mc = new clsMyClass();
            //string info = getCmpName();
            //info += ", Fin.Year: " + getFinYear();
            //info += ", User: " + getUserName();
            string info = getUserName();
            //info += " (" + mc.getNameByKey(mc.getLoginTypes(), "logintype", getLoginType().ToString(), "logintypename") + ")";
            return info;
        }
        //
    }
}