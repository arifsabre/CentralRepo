using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Collections;

namespace ManufacturingManagement_V2.Models
{
    public class clsMyClass
    {
        string dtformat = "dmy";//or "mdy"
        public string minvaliddate = "01/01/1900";
        public DateTime minValDate = new DateTime(1900,1,1);

        /// <summary>
        /// used in place of -internal string strconn = System.Configuration.
        /// ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        /// </summary>
        internal DataTable getCrptLoginInfo()
        {
            DataTable dtinfo = new DataTable();
            dtinfo.Columns.Add("svrname");
            dtinfo.Columns.Add("dbname");
            dtinfo.Columns.Add("userid");
            dtinfo.Columns.Add("passw");
            DataRow dtr = dtinfo.NewRow();
            dtr["svrname"] = System.Configuration.ConfigurationManager.AppSettings["svrname"].ToString();
            dtr["dbname"] = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            dtr["userid"] = System.Configuration.ConfigurationManager.AppSettings["userid"].ToString();
            dtr["passw"] = System.Configuration.ConfigurationManager.AppSettings["passw"].ToString();
            dtinfo.Rows.Add(dtr);
            return dtinfo;
        }
        //
        internal string getEmailPassword()
        {
            clsEncryption enc = new clsEncryption();
            //if to be read from xml file by app software
            //DataSet ds = new DataSet();
            //string fileName = "tbllginfo.xml";
            //string fullPath = Path.GetFullPath(fileName);
            //if writing on consle
            //Console.WriteLine("GetFullPath('{0}') returns '{1}'", fileName, fullPath);
            //ds.ReadXml(fullPath);
            //return enc.Decrypt(ds.Tables[0].Rows[0]["EmailPassw"].ToString());
            //---------------------------------------
            return enc.Decrypt(System.Configuration.ConfigurationManager.AppSettings["EmailPassw"].ToString());
        }
        //
        internal string strconn
        {
            get
            {
                HttpContext hc = HttpContext.Current;
                //cons = "Provider=Microsoft.Jet.OLEDB.4.0;";
                //cons = cons + "Data Source=" + hc.Server.MapPath("App_Data/amcdb.mdb") + ";";
                //cons = cons + "jet oledb:database password=xgenglobalworld";
                DataTable lginfo = getCrptLoginInfo();
                string cons = @"data source=" + lginfo.Rows[0]["svrname"].ToString() + ";";
                cons = cons + @"initial catalog=" + lginfo.Rows[0]["dbname"].ToString() + ";";
                cons = cons + @"user id=" + lginfo.Rows[0]["userid"].ToString() + ";";
                cons = cons + @"password=" + lginfo.Rows[0]["passw"].ToString() + ";";
                cons = cons + @"Connect Timeout=200;";
                cons = cons + @"pooling='true';";
                cons = cons + @"Max Pool Size=200;";
                return cons;
            }
        }
        //
        #region sql objects
        //
        internal SqlParameter getPObject(string name, object value, DbType dbt)
        {
            SqlParameter sp = new SqlParameter();
            sp.IsNullable = true;
            sp.ParameterName = name;
            sp.Value = value;
            sp.DbType = dbt;
            return sp;
        }
        //
        public SqlParameter getSqlPObject(string name, object value, SqlDbType dbt)
        {
            SqlParameter sp = new SqlParameter();
            sp.IsNullable = true;
            sp.ParameterName = name;
            sp.Value = value;
            sp.SqlDbType = dbt;
            sp.Size = 2000000;//note - assumed max
            return sp;
        }
        //
        internal string getFromDatabase(SqlCommand cmd)
        {
            string ret = "";
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = strconn;
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                cmd.Connection = conn;
                var x = cmd.ExecuteScalar();
                if (x != null) { ret = x.ToString(); };
                if (conn != null) { conn.Close(); };
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return ret;
        }
        //
        internal string getFromDatabase(SqlCommand cmd, SqlConnection conn)
        {
            string ret = "";
            try
            {
                var x = cmd.ExecuteScalar();
                if (x != null) { ret = x.ToString(); };
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return ret;
        }
        //
        internal void fillFromDatabase(DataSet ds, SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strconn;
            ds.Clear();
            if (conn.State == ConnectionState.Closed) { conn.Open(); };
            cmd.Connection = conn;
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tbl");
            if (conn != null) { conn.Close(); };
        }
        //
        internal void fillFromDatabase(DataSet ds, SqlCommand cmd, SqlConnection conn)
        {
            ds.Clear();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            adp.Fill(ds, "tbl");
        }
        //
        internal void fillFromDatabase(DataTable dt, SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strconn;
            dt.Clear();
            if (conn.State == ConnectionState.Closed) { conn.Open(); };
            cmd.Connection = conn;
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            adp.Fill(dt);
            if (conn != null) { conn.Close(); };
        }
        //
        internal void fillFromDatabase(DataTable dt, SqlCommand cmd, SqlConnection conn)
        {
            dt.Clear();
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            adp.Fill(dt);
        }
        //
        public bool isDuplicateDataTableItem(DataTable dt, string colname, string colvalue)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][colname].ToString() == colvalue) { return true; };
            }
            return false;
        }
        //
        public string getNewId(SqlCommand cmd, dbTables tblname, string colname)
        {
            clsCookie objCookie = new clsCookie();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_getnewid";
            cmd.Parameters.Add(getPObject("@tblname", tblname.ToString(), DbType.String));
            cmd.Parameters.Add(getPObject("@colname", colname, DbType.String));
            cmd.Parameters.Add(getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            return getFromDatabase(cmd, cmd.Connection);
        }
        //
        public string getRecentIdentityValue(SqlCommand cmd, dbTables tblname, string colname)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recent_identity_value";
            cmd.Parameters.Add(getPObject("@tblname", tblname.ToString(), DbType.String));
            cmd.Parameters.Add(getPObject("@colname", colname, DbType.String));
            return getFromDatabase(cmd, cmd.Connection);
        }
        //
        public string getRecentIdentityValue(dbTables tblname, string colname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_recent_identity_value";
            cmd.Parameters.Add(getPObject("@tblname", tblname.ToString(), DbType.String));
            cmd.Parameters.Add(getPObject("@colname", colname, DbType.String));
            return getFromDatabase(cmd);
        }
        //
        public string getNewVNo(SqlCommand cmd, dbTables tblname, string vtype)
        {
            cmd.Parameters.Clear();
            clsCookie objCoockie = new clsCookie();
            if (tblname == dbTables.tbl_voucher)
            {
                cmd.CommandText = "usp_get_new_vno_for_voucher";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            if (tblname == dbTables.tbl_purchase)
            {
                cmd.CommandText = "usp_get_new_vno_for_purchase";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            if (tblname == dbTables.tbl_sale)
            {
                cmd.CommandText = "usp_get_new_vno_for_sale";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            else if (tblname == dbTables.tbl_dailywork)
            {
                cmd.CommandText = "usp_get_new_vno_for_dailywork";
            }
            cmd.Parameters.Add(getPObject("@compcode", objCoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(getPObject("@finyear", objCoockie.getFinYear(), DbType.String));
            return getFromDatabase(cmd, cmd.Connection);
        }
        //
        public string getNewVNo(dbTables tblname, string vtype)
        {
            clsCookie objCoockie = new clsCookie();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (tblname == dbTables.tbl_voucher)
            {
                cmd.CommandText = "usp_get_new_vno_for_voucher";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            if (tblname == dbTables.tbl_purchase)
            {
                cmd.CommandText = "usp_get_new_vno_for_purchase";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            if (tblname == dbTables.tbl_sale)
            {
                cmd.CommandText = "usp_get_new_vno_for_sale";
                cmd.Parameters.Add(getPObject("@vtype", vtype, DbType.String));
            }
            else if (tblname == dbTables.tbl_dailywork)
            {
                cmd.CommandText = "usp_get_new_vno_for_dailywork";
            }
            cmd.Parameters.Add(getPObject("@compcode", objCoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(getPObject("@finyear", objCoockie.getFinYear(), DbType.String));
            return getFromDatabase(cmd);
        }
        //
        public string getForSqlBitString(string bitvalue)
        {
            string bitstr = "0";
            if (bitvalue == "Yes" || bitvalue == "True")
            {
                bitstr = "1";
            }
            return bitstr;
        }
        //
        public string getForSqlIntString(string intvalue)
        {
            //string intstr = intvalue != 0 ? intvalue : null;
            string intstr = null;
            if (Convert.ToInt32(intvalue) != 0)
            {
                intstr = intvalue;
            }
            return intstr;
        }
        //
        public string getForSqlStrIdString(string strvalue)
        {
            //string intstr = intvalue != 0 ? intvalue : null;
            string strstr = null;
            if (strvalue != "0")
            {
                strstr = strvalue;
            }
            return strstr;
        }
        //
        public string getForSqlDateString(DateTime datevalue)
        {
            //string grdts = objbprc.GrDate != new DateTime(1900, 1, 1) ? objbprc.GrDate.ToShortDateString() : null;
            string datestr = null;
            if (datevalue != new DateTime(1900, 1, 1))
            {
                datestr = datevalue.ToShortDateString();
            }
            return datestr;
        }
        //
        public DataSet getDatasetByExcel(string path, string sheetname)
        {
            DataSet ds = new DataSet();
            string connString = "";//connStr to excel workbook
            if (System.IO.Path.GetExtension(path).ToLower() == ".xls")
            {
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (System.IO.Path.GetExtension(path).ToLower() == ".xlsx")
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            }
            System.Data.OleDb.OleDbConnection xlconn = new System.Data.OleDb.OleDbConnection(connString);
            try
            {
                if (xlconn.State == ConnectionState.Closed) { xlconn.Open(); };
                string query = "select * from [" + sheetname + "$]";
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, xlconn);
                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(cmd);
                da.Fill(ds, "tbl");
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            finally
            {
                if (xlconn != null) { xlconn.Close(); };
            }
            return ds;
        }
        //
        #endregion
        //
        #region user permission
        //
        internal bool getPermission(Entry entryid, permissionType ptype)
        {
            bool res = false;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (ptype == permissionType.Add)
            {
                cmd.CommandText = "usp_get_permission_for_add";
            }
            else if (ptype == permissionType.Edit)
            {
                cmd.CommandText = "usp_get_permission_for_edit";
            }
            else if (ptype == permissionType.Delete)
            {
                cmd.CommandText = "usp_get_permission_for_delete";
            }
            clsCookie objCookie = new clsCookie();
            cmd.Parameters.Add(getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(getPObject("@entryid", Convert.ToInt32(entryid).ToString(), DbType.Int32));
            cmd.Parameters.Add(getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            string ret = getFromDatabase(cmd);
            if (ret != "") { res = Convert.ToBoolean(ret); };
            return res;
        }
        //
        public bool isValidToDisplayReport()
        {
            HttpContext hc = HttpContext.Current;
            clsCookie objcookie = new clsCookie();
            bool per = true;
            if (hc.Session["xsid"] == null)
            {
                return false;//not called by programme
            }
            if (hc.Session["xsid"].ToString() != objcookie.getUserId())
            {
                hc.Session.Remove("xsid");
                return false; //trying un-fairly
            }
            hc.Session.Remove("xsid");
            return per;
        }
        //
        #endregion
        //
        #region collaboration permission
        //
        private bool getCollabPermissionByDB(int itemgroupid, int categoryid, collabPermissionType ptype)
        {
            clsCookie objCookie = new clsCookie();
            bool res = false;
            int userid = Convert.ToInt32(objCookie.getUserId());
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (ptype == collabPermissionType.View)
            {
                cmd.CommandText = "usp_get_collabpermission_for_view";
            }
            else if (ptype == collabPermissionType.Upload)
            {
                cmd.CommandText = "usp_get_collabpermission_for_upload";
            }
            else if (ptype == collabPermissionType.Download)
            {
                cmd.CommandText = "usp_get_collabpermission_for_download";
            }
            else if (ptype == collabPermissionType.Delete)
            {
                cmd.CommandText = "usp_get_collabpermission_for_delete";
            }
            cmd.Parameters.Add(getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(getPObject("@itemgroupid", itemgroupid, DbType.Int32));
            cmd.Parameters.Add(getPObject("@categoryid", categoryid, DbType.Int32));
            string ret = getFromDatabase(cmd);
            if (ret != "") { res = Convert.ToBoolean(ret); };
            return res;
        }
        //
        public bool getCollabPermission(int itemgroupid, int categoryid, collabPermissionType ptype, System.Web.UI.WebControls.Label lblinfo)
        {
            bool res = getCollabPermissionByDB(itemgroupid, categoryid, ptype);
            if (res == false)
            {
                lblinfo.Text = ptype.ToString() + " Permission is denied!";
            }
            return res;
        }
        //
        public bool getCollabPermission(int itemgroupid, int categoryid, collabPermissionType ptype)
        {
            bool res = getCollabPermissionByDB(itemgroupid, categoryid, ptype);
            return res;
        }
        //
        #endregion

        #region document permission [view / download]
        //
        public bool getDocumentPermissionToView(string filepath, int igroupid, int catgid)
        {
            bool result = false;
            if (filepath.ToLower().Contains("itemdocs\\"))
            {
                if (getPermission(Entry.Download_Docs_Item, permissionType.Add) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("stdspecs\\"))
            {
                if (getPermission(Entry.Download_Docs_StdSpecs, permissionType.Add) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("companydocs\\"))
            {
                result = true;
                //if (getPermission(Entry.Download_Company_Document, permissionType.Add) == true)
                //{
                //    result = true;
                //}
            }
            else if (filepath.ToLower().Contains("companypolicies\\"))
            {
                result = true;
                //if (getPermission(Entry.Download_Company_Document, permissionType.Add) == true)
                //{
                //    result = true;
                //}
            }
            else if (filepath.ToLower().Contains("tenderfile"))
            {
                if (getPermission(Entry.Download_Docs_Tender, permissionType.Add) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("casefile"))
            {
                if (getPermission(Entry.Download_Dispach_Document, permissionType.Add) == true)
                {
                    result = true;
                }
            }
            //collaboration files -for documentcontrol
            else if (filepath.ToLower().Contains("projectdocs\\"))
            {
                if (getCollabPermission(igroupid, catgid, collabPermissionType.View) == true)
                {
                    result = true;
                }
            }
            return result;
        }
        //
        public bool getDocumentPermissionToDownload(string filepath, int igroupid, int catgid)
        {
            bool result = false;
            if (filepath.ToLower().Contains("itemdocs\\"))
            {
                if (getPermission(Entry.Download_Docs_Item, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("stdspecs\\"))
            {
                if (getPermission(Entry.Download_Docs_StdSpecs, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("companydocs\\"))
            {
                if (getPermission(Entry.Download_Company_Document, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("companypolicies\\"))
            {
                if (getPermission(Entry.Download_Company_Document, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("tenderfile"))
            {
                if (getPermission(Entry.Download_Docs_Tender, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            else if (filepath.ToLower().Contains("casefile"))
            {
                if (getPermission(Entry.Download_Dispach_Document, permissionType.Edit) == true)
                {
                    result = true;
                }
            }
            //collaboration files -for documentcontrol
            else if (filepath.ToLower().Contains("projectdocs\\"))
            {
                if (getCollabPermission(igroupid, catgid, collabPermissionType.Download) == true)
                {
                    result = true;
                }
            }
            return result;
        }
        //
        #endregion
        //
        #region eventlog
        //
        public bool setEventLog(SqlCommand cmd, dbTables tblname, string pkval, string logdesc)
        {
            bool res = false;
            clsCookie objCookie = new clsCookie();
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_make_eventlog";
                cmd.Parameters.Add(getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(getPObject("@evdate", DateTime.Now.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(getPObject("@evtime", DateTime.Now.ToShortTimeString(), DbType.String));
                cmd.Parameters.Add(getPObject("@tblid", Convert.ToInt32(tblname).ToString(), DbType.Int32));
                cmd.Parameters.Add(getPObject("@pkval", pkval, DbType.String));
                cmd.Parameters.Add(getPObject("@logdesc", logdesc, DbType.String));
                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            return res;
        }
        //
        #endregion
        //
        #region errorlog
        //
        public string setErrorLog(string errpage, string errobject, string errmsg)
        {
            clsCookie objCookie = new clsCookie();
            string ErrOpt = "0";//ConfigurationManager.AppSettings["ErrOpt"].ToString();
            if (ErrOpt == "0") { return errmsg; };//--ToDisplay Otherwise insert in errortable
            string ret = errmsg;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strconn;
            errmsg = errmsg.Replace("'", "*");
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_error";
                cmd.Parameters.Add(getPObject("@errdate", DateTime.Now.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(getPObject("@errtime", DateTime.Now.ToShortTimeString(), DbType.String));
                cmd.Parameters.Add(getPObject("@errpage", errpage, DbType.String));
                cmd.Parameters.Add(getPObject("@errobject", errobject, DbType.String));
                cmd.Parameters.Add(getPObject("@errmsg", errmsg, DbType.String));
                cmd.Parameters.Add(getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                ret = "Action Failed!";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
            return ret;
        }
        //
        #endregion
        //
        #region helper objects
        //
        #region general
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns numbers in word
        /// upto 99 arab with 2 point decimal
        /// values
        /// </summary>
        /// <param name="snumber"></param>
        /// <returns></returns>
        public string getWordByNumericDouble(string snumber)
        {
            if (clsValidateInput.IsValidDouble(snumber) == false) { return ""; };
            double dbl = Convert.ToDouble(snumber);
            if (dbl == 0) { return "Zero"; };
            snumber = Math.Abs(dbl).ToString("f2");
            string fr = snumber.Substring(snumber.Length - 2, 2);
            string frv = getInWord(fr) + " Paise Only";
            snumber = snumber.Substring(0, snumber.Length - 3);
            if (snumber.Length > 11) { return "Amount greater than 99 arab!"; };
            snumber = StrDup("0", 11 - snumber.Length) + snumber;
            string ar = snumber[0].ToString() + snumber[1].ToString();
            string cr = snumber[2].ToString() + snumber[3].ToString();
            string lk = snumber[4].ToString() + snumber[5].ToString();
            string th = snumber[6].ToString() + snumber[7].ToString();
            string hd = snumber[8].ToString();
            string os = snumber[9].ToString() + snumber[10].ToString();
            string wd = "";
            if (Convert.ToInt64(ar) != 0)
            {
                wd = getInWord(ar) + " Arab ";
            }
            if (Convert.ToInt64(cr) != 0)
            {
                wd = wd + getInWord(cr) + " Crore ";
            }
            if (Convert.ToInt64(lk) != 0)
            {
                wd = wd + getInWord(lk) + " Lakh ";
            }
            if (Convert.ToInt64(th) != 0)
            {
                wd = wd + getInWord(th) + " Thousand ";
            }
            if (Convert.ToInt64(hd) != 0)
            {
                wd = wd + getInWord(hd) + " Hundred ";
            }
            if (Convert.ToInt64(os) != 0)
            {
                wd = wd + getInWord(os);
            }
            if (wd == "") { wd = "Zero"; };
            if (Convert.ToDouble(fr) > 0)
            {
                wd = "Rupees " + wd + " and " + frv;
            }
            else
            {
                wd = "Rupees " + wd + " Only";
            }
            return wd.Trim();
        }
        //
        public string StrDup(string str, int num)
        {
            string ret = string.Empty;
            for (int i = 0; i < num; i++)
            {
                ret = ret + str;
            }
            return ret;
        }
        //
        private string getInWord(string number)
        {
            int num = Convert.ToInt32(number);
            string ret = "Zero";
            string[] ones = new string[] {"One", "Two", "Three", "Four",
            "Five", "Six", "Seven", "Eight", "Nine", "Ten",
            "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen",
            "Sixteen", "Seventeen", "Eighteen", "Nineteen"};
            string[] tens = new string[] {"Twenty", "Thirty", "Forty",
            "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};
            if (Convert.ToInt32(num) > 0 && Convert.ToInt32(num) <= 19)
            {
                ret = ones[Convert.ToInt32(num) - 1];
            }
            else if (Convert.ToInt32(num) >= 20)
            {
                ret = tens[Convert.ToInt32(num) / 10 - 2];
                if (Convert.ToInt32(num) % 10 > 0)
                {
                    ret = ret + " " + ones[Convert.ToInt32(num) % 10 - 1];
                }
            }
            return ret;
        }
        //
        //--------------------------------------------------------------------------------
        public string getMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
        //
        public DataSet getLoginTypes()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("logintype");
            ds.Tables[0].Columns.Add("logintypename");
            ds.Tables[0].Rows.Add("0", "Admin");
            ds.Tables[0].Rows.Add("1","User");
            ds.Tables[0].Rows.Add("2", "HOD");
            ds.Tables[0].Rows.Add("3", "Director");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getLoginTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "User", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "HOD", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Director", Value = "3" },
                  new System.Web.UI.WebControls.ListItem { Text = "Admin", Value = "0" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getLoginTypeListIndex()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "5" },
                  new System.Web.UI.WebControls.ListItem { Text = "User", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "HOD", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Director", Value = "3" },
                  new System.Web.UI.WebControls.ListItem { Text = "Admin", Value = "0" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getMainMenueList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Report Group", Value = "92" },
                  new System.Web.UI.WebControls.ListItem { Text = "Master", Value = "10" },
                  new System.Web.UI.WebControls.ListItem { Text = "Tender", Value = "11" },
                  new System.Web.UI.WebControls.ListItem { Text = "Purchase Order", Value = "12" },
                  new System.Web.UI.WebControls.ListItem { Text = "Sales", Value = "13" },
                  new System.Web.UI.WebControls.ListItem { Text = "Receipt", Value = "14" },
                  new System.Web.UI.WebControls.ListItem { Text = "Document Files", Value = "15" },
                  new System.Web.UI.WebControls.ListItem { Text = "Collaboration", Value = "16" },
                  new System.Web.UI.WebControls.ListItem { Text = "Calibration", Value = "17" },
                  new System.Web.UI.WebControls.ListItem { Text = "Reports", Value = "18" },
                  new System.Web.UI.WebControls.ListItem { Text = "Dashboard", Value = "19" },
                  new System.Web.UI.WebControls.ListItem { Text = "Options", Value = "20" },
                  new System.Web.UI.WebControls.ListItem { Text = "Store", Value = "21" },
                  new System.Web.UI.WebControls.ListItem { Text = "HR", Value = "22" },
                  new System.Web.UI.WebControls.ListItem { Text = "Complaince", Value = "24" },
                  new System.Web.UI.WebControls.ListItem { Text = "Complaint", Value = "25" },
                  new System.Web.UI.WebControls.ListItem { Text = "Others P1", Value = "90" },
                  new System.Web.UI.WebControls.ListItem { Text = "Others P2", Value = "91" },
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getTitleList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Mr.", Value = "Mr." },
                  new System.Web.UI.WebControls.ListItem { Text = "Mrs.", Value = "Mrs." },
                  new System.Web.UI.WebControls.ListItem { Text = "Miss", Value = "Miss" }
            };
            return listItems;
        }
        //
        public DataSet getGenders()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("gender");
            ds.Tables[0].Columns.Add("gendername");
            ds.Tables[0].Rows.Add("m", "Male");
            ds.Tables[0].Rows.Add("f", "Female");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getGenderList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Male", Value = "m" },
                  new System.Web.UI.WebControls.ListItem { Text = "Female", Value = "f" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getCorpTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Draft", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Main", Value = "m" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getDispatchTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Sent", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Receipt", Value = "r" }
            };
            return listItems;
        }
        //
        public DataSet getMaritalStatus()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("mstatus");
            ds.Tables[0].Columns.Add("mstatusname");
            ds.Tables[0].Rows.Add("un", "Unmarried");
            ds.Tables[0].Rows.Add("mr", "Married");
            ds.Tables[0].Rows.Add("wd", "Widow");
            ds.Tables[0].Rows.Add("wr", "Widower");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getMaritalStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Unmarried", Value = "un" },
                  new System.Web.UI.WebControls.ListItem { Text = "Married", Value = "mr" },
                  new System.Web.UI.WebControls.ListItem { Text = "Widow", Value = "wd" },
                  new System.Web.UI.WebControls.ListItem { Text = "Widower", Value = "wr" }
            };
            return listItems;
        }
        //
        public DataSet getNomineeForOption()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("nomineefor");
            ds.Tables[0].Columns.Add("nomineeforname");
            ds.Tables[0].Rows.Add("b", "Both");
            ds.Tables[0].Rows.Add("p", "PF");
            ds.Tables[0].Rows.Add("g", "Gratuity");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getNomineeForList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Both", Value = "b" },
                  new System.Web.UI.WebControls.ListItem { Text = "PF", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Gratuity", Value = "g" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getMsgTypes()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Single", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Multiple", Value = "m" },
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "a" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getComplaintTypes()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Others/Grievance", Value = "2515" },
                  new System.Web.UI.WebControls.ListItem { Text = "HW", Value = "2510" },
                  new System.Web.UI.WebControls.ListItem { Text = "ERP", Value = "2505" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getQuailPriorityList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "1", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "2", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "3", Value = "3" },
                  new System.Web.UI.WebControls.ListItem { Text = "4", Value = "4" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getComplaintStatus()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Under-process", Value = "u" },
                  new System.Web.UI.WebControls.ListItem { Text = "Resolved", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "0" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getComplaintReplyStatus()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Under-process", Value = "u" },
                  new System.Web.UI.WebControls.ListItem { Text = "Resolved", Value = "r" },
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getFeedbackStatus()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "0" }, 
                new System.Web.UI.WebControls.ListItem { Text = "Reviewed", Value = "r" },
                new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
            };
            return listItems;
        }
        //
        public DataSet getReportFormats()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("format");
            ds.Tables[0].Columns.Add("ext");
            ds.Tables[0].Rows.Add("PDF", "pdf");
            ds.Tables[0].Rows.Add("Excel", "xls");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getReportFormatList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "PDF", Value = "PDF" },
                  new System.Web.UI.WebControls.ListItem { Text = "Excel", Value = "Excel" }
            };
            return listItems;
        }
        //
        public DataSet getSaleOrderTypes()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("sotype");
            ds.Tables[0].Columns.Add("sotypename");
            ds.Tables[0].Rows.Add("acct", "Accountable");
            ds.Tables[0].Rows.Add("nonacct", "Non-Accountable");
            ds.Tables[0].Rows.Add("all", "ALL");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getSaleOrderTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Accountable", Value = "acct" },
                  new System.Web.UI.WebControls.ListItem { Text = "Non-Accountable", Value = "nonacct" },
                  new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "all" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getBGStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Submitted", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Expired", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Received", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cancelled", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "To be Released", Value = "t" },
                  new System.Web.UI.WebControls.ListItem { Text = "Closed", Value = "d" }
            };
            return listItems;
        }
        //
        public void fillDropDownList(DataSet ds, System.Web.UI.WebControls.DropDownList ddl, string valuefield, string textfield)
        {
            ddl.Items.Clear();
            ddl.DataSource = ds.Tables[0];
            ddl.DataValueField = valuefield;
            ddl.DataTextField = textfield;
            if (ddl.Items.Count > 0) { ddl.SelectedIndex = 0; };
        }
        //
        public string getNameByKey(DataSet ds, string keyname, string keyvalue, string fieldname)
        {
            string ret = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i][keyname].ToString().ToLower() == keyvalue.ToLower())
                {
                    ret = ds.Tables[0].Rows[i][fieldname].ToString();
                    break;
                }
            }
            return ret;
        }
        //
        public string getTableName(string tblid)
        {
            string ret = "";
            foreach (dbTables dbt in Enum.GetValues(typeof(dbTables)))
            {
                if (Convert.ToInt32(dbt) == Convert.ToInt32(tblid))
                {
                    ret = dbt.ToString().Substring(4, dbt.ToString().Length - 4).ToUpper();
                    break;
                }
            }
            return ret;
        }
        //
        public string getINRCFormat(double dblvalue)
        {
            //old logic--works in limited scope
            ////dblvalue.ToString("#,##0.00") works in limited scope
            //System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-IN");
            //string amountString = string.Format(cultureInfo, "{0:C}", dblvalue);
            //return amountString.Split(' ')[1].Trim();//removing rupee symbol
            //-----------------------------------------------------------------
            string strdbl = dblvalue.ToString();
            if (strdbl.Contains(".") == false)
            {
                strdbl = strdbl + ".00";
            }
            string[] str = strdbl.Split('.');
            string num = str[0];
            string dec = "." + str[1];
            //
            if (num.Length <= 3)
            {
                return num + dec;
            }
            //
            string str2 = num.Substring(str[0].Length - 3);
            string str1 = num.Substring(0, str[0].Length - 3);
            string p = "";
            if (str1.Length % 2 != 0) 
            {
                str1 = "0" + str1;
            }
            int c = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (c == 2)
                {
                    p += ",";
                    c = 0;
                }
                p += str1[i].ToString();
                c += 1;
            }
            if (p[0].ToString() == "0")
            {
                p = p.Substring(1);
            }
            return p + "," + str2 + dec;
        }
        //
        #endregion
        //
        #region datetime
        //
        public bool isValidDate(DateTime datevalue)
        {
            if (datevalue == new DateTime(1900, 1, 1) || datevalue == new DateTime(0001, 1, 1))
                return false;
            else
                return true;
        }
        //
        public bool isValidDateForFinYear(string chkfinyr, DateTime vdate)
        {
            string finyr = "";
            int yr = vdate.Year;
            int mt = vdate.Month;
            if (mt >= 4 && mt <= 12)//apr-dec
            {
                finyr = yr.ToString().Substring(2, 2) + "-" + (yr + 1).ToString().Substring(2, 2);
            }
            else if (mt >= 1 && mt <= 3)//jan-march
            {
                finyr = (yr - 1).ToString().Substring(2, 2) + "-" + yr.ToString().Substring(2, 2);
            }
            if (chkfinyr == finyr)
            {
                return true;
            }
            return false;
        }
        public DateTime setToValidOptionalDate(DateTime datevalue)
        {
            if (datevalue == new DateTime(0001, 1, 1))
            {
                datevalue = new DateTime(1900, 1, 1);//sql server default storable
            }
            return datevalue;
        }
        //
        public bool isValidDateString(string datestr)
        {
            return isValidDate(getDateByString(datestr));
        }
        //
        public bool IsValidInteger(string str)
        {
            int d;
            if (int.TryParse(str, out d) == true) { return true; };
            return false;
        }
        //
        public string getTimeInWords(string datetime)
        {
            string[] str1 = datetime.Trim().Split(' ');
            string[] str2 = null;
            string timeinwords = "";
            if (str1.Length == 3)
            {
                str2 = str1[1].Trim().Split(':');
                if (str2.Length == 2)
                {
                    if (IsValidInteger(str2[0]) == true && IsValidInteger(str2[1]) == true)
                    {
                        timeinwords = getInWord(str2[0]);
                        if (Convert.ToDouble(str2[1]) > 0)
                        {
                            timeinwords += " " + getInWord(str2[1]);
                        }
                        timeinwords += " Hours";
                    }

                }
            }
            return timeinwords;
        }
        public DateTime getDateByString(string datestr)
        {
            if (datestr.Length != 10) { return new DateTime(1900, 1, 1); };
            string separator = "/";
            if (datestr.Contains("-"))
            {
                separator = "-";
            }
            else if (datestr.Contains("."))
            {
                separator = ".";
            }
            string MM = "";
            string dd = "";
            int j = 0;
            string yyyy = "";
            if (dtformat == "mdy")
            {
                j = datestr.IndexOf(separator);
                MM = datestr.Substring(0, j);
                datestr = datestr.Substring(j + 1, datestr.Length - MM.Length - 1);
                j = datestr.IndexOf(separator);
                dd = datestr.Substring(0, j);
                datestr = datestr.Substring(j + 1, datestr.Length - dd.Length - 1);
                j = datestr.IndexOf(separator);
                yyyy = datestr.Substring(j + 1, datestr.Length);
            }
            else if (dtformat == "dmy")
            {
                j = datestr.IndexOf(separator);
                dd = datestr.Substring(0, j);
                datestr = datestr.Substring(j + 1, datestr.Length - dd.Length - 1);
                j = datestr.IndexOf(separator);
                MM = datestr.Substring(0, j);
                datestr = datestr.Substring(j + 1, datestr.Length - MM.Length - 1);
                j = datestr.IndexOf(separator);
                yyyy = datestr.Substring(j + 1, datestr.Length);
            }
            DateTime dt = DateTime.Now;
            try
            {
                dt = new DateTime(Convert.ToInt32(yyyy), Convert.ToInt32(MM), Convert.ToInt32(dd));
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                dt = new DateTime(1900, 1, 1);
            }
            return dt;
        }
        //
        #region new
        //
        public DateTime getDateByStringDDMMYYYY(string datestr)
        {
            if (datestr.Length != 10) { return new DateTime(1900, 1, 1); };
            string separator = "/";
            if (datestr.Contains("-"))
            {
                separator = "-";
            }
            else if (datestr.Contains("."))
            {
                separator = ".";
            }
            string MM = "";
            string dd = "";
            int j = 0;
            string yyyy = "";
            j = datestr.IndexOf(separator);
            dd = datestr.Substring(0, j);
            datestr = datestr.Substring(j + 1, datestr.Length - dd.Length - 1);
            j = datestr.IndexOf(separator);
            MM = datestr.Substring(0, j);
            datestr = datestr.Substring(j + 1, datestr.Length - MM.Length - 1);
            j = datestr.IndexOf(separator);
            yyyy = datestr.Substring(j + 1, datestr.Length);
            DateTime dt = DateTime.Now;
            try
            {
                dt = new DateTime(Convert.ToInt32(yyyy), Convert.ToInt32(MM), Convert.ToInt32(dd));
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                dt = new DateTime(1900, 1, 1);
            }
            return dt;
        }
        //
        public string getStringByDateDDMMYYYY(DateTime datevalue)
        {
            string ret = "";
            if (datevalue.Day == 1 && datevalue.Month == 1 && datevalue.Year == 1900) { return ret; };
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            ret = string.Format("{0:dd/MM/yyyy}", datevalue);
            return ret;
        }
        //
        #endregion
        public string getStringByDate(DateTime datevalue)
        {
            string ret = "";
            if (datevalue.Day == 1 && datevalue.Month == 1 && datevalue.Year == 1900) { return ret; };
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            if (dtformat == "mdy")
            {
                ret = string.Format("{0:MM/dd/yyyy}", datevalue);
            }
            else if (dtformat == "dmy")
            {
                ret = string.Format("{0:dd/MM/yyyy}", datevalue);
            }
            return ret;
        }
        //
        public string getStringByDateToStore(string datevalue)
        {
            string ret = "";
            DateTime dtm = getDateByString(datevalue);
            if (dtm.ToShortDateString() == "01/01/1900") { return ret; };
            string MM = dtm.Month.ToString();
            string dd = dtm.Day.ToString();
            string yyyy = dtm.Year.ToString();
            if(MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if(dd.Length < 2)
            {
                dd = "0" + dd;
            }
            return yyyy + MM + dd;
        }
        //
        public string getStringByDateToStore(DateTime datevalue)
        {
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            if (MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if (dd.Length < 2)
            {
                dd = "0" + dd;
            }
            return yyyy + MM + dd;
        }
        //
        public DateTime getDateBySqlGenericString(string datevalue)
        {
            DateTime dt = new DateTime(1900, 1, 1);
            if (datevalue.Length != 10) { return dt; };
            string gstr = getStringByDateToStore(datevalue);
            if (gstr.Length != 8) { return dt; };//f=yyyyMMdd
            string yyyy = gstr.Substring(0, 4);
            string MM = gstr.Substring(4, 2);
            string dd = gstr.Substring(6, 2);
            try
            {
                dt = new DateTime(Convert.ToInt32(yyyy), Convert.ToInt32(MM), Convert.ToInt32(dd));
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                dt = new DateTime(1900, 1, 1);
            }
            return dt;
        }
        //
        public string getStringByDateForJavaScript(DateTime datevalue)
        {
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            if (MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if (dd.Length < 2)
            {
                dd = "0" + dd;
            }
            return yyyy +"-"+ MM +"-"+ dd;
        }
        //
        public string getStringByDateForReport(DateTime datevalue)
        {
            if (datevalue.Day == 1 && datevalue.Month == 1 && datevalue.Year == 1900) { return ""; };
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            return yyyy + "-" + MM + "-" + dd;//sql server detault format
        }
        //
        public string getCurrentDateStringForSql()
        {
            DateTime dtm = DateTime.Now;
            string MM = dtm.Month.ToString();
            string dd = dtm.Day.ToString();
            string yyyy = dtm.Year.ToString();
            if (MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if (dd.Length < 2)
            {
                dd = "0" + dd;
            }
            return yyyy + MM + dd;
        }
        //
        public string getDateTimeString(DateTime datevalue)
        {
            string MM = datevalue.Month.ToString();
            string dd = datevalue.Day.ToString();
            string yyyy = datevalue.Year.ToString();
            if (MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if (dd.Length < 2)
            {
                dd = "0" + dd;
            }
            return dd + "/" + MM + "/" + datevalue.Year.ToString() + " " + datevalue.ToShortTimeString();
        }
        //
        public bool isValidDateRange(string dtfrom, string dtto)
        {
            DateTime dtx = getDateByString(dtfrom);
            DateTime dty = getDateByString(dtto);
            if (isValidDate(dtx) == false)
            {
                return false;
            }
            if (isValidDate(dty) == false)
            {
                return false;
            }
            if (dtx > dty)
            {
                return false;
            }
            return true;
        }
        //
        public int DateDifference(DateTime dt1, DateTime dt2)
        {
            TimeSpan diffDate = dt1.Subtract(dt2);
            return diffDate.Days;
        }
        //
        public bool IsValidDouble(string str)
        {
            if (str.Length == 0) { return false; }
            double d;
            if (Double.TryParse(str, out d) == true) { return true; };
            return false;
        }
        //
        public bool isValidDateToSetRecord(DateTime evdate)
        {
            DateTime ctdate = DateTime.Now;
            int evMth = evdate.Month;
            int ctMth = ctdate.Month;
            if (evdate.Year > ctdate.Year)
            {
                return true;
            }
            if (evdate.Year < ctdate.Year)
            {
                ctMth = ctMth + (ctdate.Year - evdate.Year) * 12;
            }
            int dtdiff = ctMth - evMth;
            if (dtdiff > 1)
            {
                return false;
            }
            else if(dtdiff == 1)
            {
                if (ctdate.Day > 5)
                {
                    return false;
                }
            }
            return true;
        }
        //
        public string RightNChars(string str, int n)
        {
            return str.Substring(str.Length - n, n);
        }
        public string LeftNChars(string str, int n)
        {
            return str.Substring(0, n);
        }
        #endregion
        //
        #region store department
        /// <summary>
        /// 1 = company, 2 = itemtype, 3 = project,
        /// 4 = group, 5 = code
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>
        public string[] getElementsByPartNo(string partno)
        {
            //note: partnodesc 1,1,02,12,005=9d
            //d 1 = company
            //d 2 = itemtype
            //d 3+4 = project
            //d 5+6 = group
            //d 7+8+9 = code
            //1 = PP-B1, 2 = PP-U2
            //1 = RM, 9 = FI
            //cl-note--------------------------
            string[] arr = new string[] { "","","","","",""};//6
            if (partno.Length != 9) { return arr; };

            string cmp = partno.Substring(0, 1);
            if (cmp == "1")
            {
                cmp = "6";//erp compcode for PP-B1
            }
            else if (cmp == "2")
            {
                cmp = "11";//erp compcode for PP-U2
            }

            string itemtype = partno.Substring(1, 1);
            if (itemtype == "1")
            {
                itemtype = "rm";
            }
            else if (itemtype == "9")
            {
                itemtype = "fi";
            }

            arr[1] = cmp;
            arr[2] = itemtype;
            arr[3] = partno.Substring(2, 2);//project
            arr[4] = partno.Substring(4, 2);//group
            arr[5] = partno.Substring(6, 3);//code
            return arr;
        }
        //
        public DataSet getIndentStatus()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("indentstatus");
            ds.Tables[0].Columns.Add("indentstatusname");
            ds.Tables[0].Rows.Add("p", "Pending");
            ds.Tables[0].Rows.Add("h", "Approved (HOD)");
            ds.Tables[0].Rows.Add("c", "Cancelled (HOD)");
            ds.Tables[0].Rows.Add("a", "Approved (Admin)");
            ds.Tables[0].Rows.Add("r", "Rejected (Admin)");
            ds.Tables[0].Rows.Add("e", "Executed");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getIndentStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Approved (HOD)", Value = "h" },
                  new System.Web.UI.WebControls.ListItem { Text = "Approved (Admin)", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "Executed", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cancelled (HOD)", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "Rejected (Admin)", Value = "r" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getCurrencyList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Rs.", Value = "Rs." },
                  new System.Web.UI.WebControls.ListItem { Text = "USD", Value = "USD" },
                  new System.Web.UI.WebControls.ListItem { Text = "GBP", Value = "GBP" },
                  new System.Web.UI.WebControls.ListItem { Text = "EUR", Value = "EUR" },
                  new System.Web.UI.WebControls.ListItem { Text = "HKD", Value = "HKD" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getOfferTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "RFQ", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Budgetary", Value = "b" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getAalCoList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "AAL", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "CO", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "n" }
            };
            return listItems;
        }
        //
        /// <summary>
        /// Subset of tbl_vouchertype
        /// </summary>
        /// <returns></returns>
        public List<System.Web.UI.WebControls.ListItem> getAccountVoucherTypeList()
        {

            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Payment Voucher", Value = "pt" },
                  new System.Web.UI.WebControls.ListItem { Text = "Receipt Voucher", Value = "rt" },
                  new System.Web.UI.WebControls.ListItem { Text = "Journal Voucher", Value = "jv" },
                  new System.Web.UI.WebControls.ListItem { Text = "Contra Voucher", Value = "co" },
                  new System.Web.UI.WebControls.ListItem { Text = "Bill Receipt", Value = "bpr" },
                  new System.Web.UI.WebControls.ListItem { Text = "Bill Payment", Value = "vpt" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getDrCrList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Select", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cr", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "Dr", Value = "d" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getDrCrListForAccount()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "Dr", Value = "d" },
                new System.Web.UI.WebControls.ListItem { Text = "Cr", Value = "c" }
                  
            };
            return listItems;
        }
        //
        /// <summary>
        /// to be revised --refered from objectList.xlsx
        /// </summary>
        /// <returns></returns>
        public DataSet getStockVoucherTypes()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("vtype");
            ds.Tables[0].Columns.Add("vtypename");
            ds.Tables[0].Rows.Add("op", "Opening");
            ds.Tables[0].Rows.Add("pc", "Purchase");
            ds.Tables[0].Rows.Add("ip", "Indent Purchase");
            ds.Tables[0].Rows.Add("pp", "Indent PO Purchase");
            ds.Tables[0].Rows.Add("dr", "DRR");
            ds.Tables[0].Rows.Add("ii", "Issued To Indent");
            ds.Tables[0].Rows.Add("dm", "Damage (General)");
            ds.Tables[0].Rows.Add("dp", "Damage In Production");
            ds.Tables[0].Rows.Add("wp", "Issued In Production");
            ds.Tables[0].Rows.Add("sl", "Sale");
            ds.Tables[0].Rows.Add("mn", "Manufactured");
            ds.Tables[0].Rows.Add("jo", "Jobwork-Out");
            ds.Tables[0].Rows.Add("ji", "Jobwork-In");
            ds.Tables[0].Rows.Add("pr", "Purchase Return");
            ds.Tables[0].Rows.Add("rj", "Mfg. Rejection");
            ds.Tables[0].Rows.Add("ig", "Issue (General)");
            ds.Tables[0].Rows.Add("rg", "Return (General)");
            return ds;
        }
        //
        /// <summary>
        /// Subset of tbl_StockVTypeInfo
        /// </summary>
        /// <returns></returns>
        public List<System.Web.UI.WebControls.ListItem> getStockVoucherTypeList()
        {
            
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Opening", Value = "op" },
                  new System.Web.UI.WebControls.ListItem { Text = "Issue (General)", Value = "ig" },
                  new System.Web.UI.WebControls.ListItem { Text = "Return (General)", Value = "rg" },
                  //new System.Web.UI.WebControls.ListItem { Text = "Issue (Indent)", Value = "ii" },
                  //new System.Web.UI.WebControls.ListItem { Text = "Return (Indent)", Value = "ri" },
                  new System.Web.UI.WebControls.ListItem { Text = "Issue (Production)", Value = "wp" },
                  new System.Web.UI.WebControls.ListItem { Text = "Return (Production)", Value = "rp" },
                  new System.Web.UI.WebControls.ListItem { Text = "Damage (Production)", Value = "dp" },
                  new System.Web.UI.WebControls.ListItem { Text = "Damage (General)", Value = "dm" }
            };
            return listItems;
        }
        //
        /// <summary>
        /// Subset of tbl_StockVTypeInfo
        /// </summary>
        /// <returns></returns>
        public List<System.Web.UI.WebControls.ListItem> getPurchaseVoucherTypeList()
        {

            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Purchase", Value = "pc" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getItemOptionList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Stock List", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Vendor PO", Value = "1" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getCashCreditList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Credit", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cash", Value = "c" }
            };
            return listItems;
        }
        //

        public List<System.Web.UI.WebControls.ListItem> getSupplyTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "N/A" },
                  new System.Web.UI.WebControls.ListItem { Text = "Business to Business", Value = "B2B" },
                  new System.Web.UI.WebControls.ListItem { Text = "Business to Consumer", Value = "B2C" },
                  new System.Web.UI.WebControls.ListItem { Text = "To SEZ with Payment", Value = "SEZWP" },
                  new System.Web.UI.WebControls.ListItem { Text = "To SEZ without Payment", Value = "SEZWOP" },
                  new System.Web.UI.WebControls.ListItem { Text = "Export with Payment", Value = "EXPWP" },
                  new System.Web.UI.WebControls.ListItem { Text = "Export Without Payment", Value = "EXPWOP" },
                  new System.Web.UI.WebControls.ListItem { Text = "Deemed Export", Value = "DEXP" }
            };
            return listItems;
        }
        //

        public List<System.Web.UI.WebControls.ListItem> getDocumentTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "N/A" },
                  new System.Web.UI.WebControls.ListItem { Text = "Invoice", Value = "INV" },
                  new System.Web.UI.WebControls.ListItem { Text = "Credit Note", Value = "CRN" },
                  new System.Web.UI.WebControls.ListItem { Text = "Debit Note", Value = "DBN" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getJobworkStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "All Pending", Value = "allpending" },
                  new System.Web.UI.WebControls.ListItem { Text = "Partially Received", Value = "partialrec" },
                  new System.Web.UI.WebControls.ListItem { Text = "Total Received", Value = "totalrec" }
            };
            return listItems;
        }
        //
        public DataSet getItemTypes()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("ItemType");
            ds.Tables[0].Columns.Add("ItemTypeName");
            ds.Tables[0].Rows.Add("fi", "Finished");
            ds.Tables[0].Rows.Add("sa", "Sub Assembly");
            ds.Tables[0].Rows.Add("rm", "Raw Material");
            ds.Tables[0].Rows.Add("sc", "Scrap");
            ds.Tables[0].Rows.Add("pm", "Plant & Machinary/Capital Goods");
            ds.Tables[0].Rows.Add("md", "Moulds, Dies & Fixtures");
            ds.Tables[0].Rows.Add("sv", "Service");
            ds.Tables[0].Rows.Add("st", "Stationary");
            ds.Tables[0].Rows.Add("ca", "Computers & Accessories");
            ds.Tables[0].Rows.Add("co", "Consumables");
            ds.Tables[0].Rows.Add("tt", "Tools & Tackles");
            ds.Tables[0].Rows.Add("ot", "Others");
            ds.Tables[0].Rows.Add("sp", "Spares");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getItemTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "0" },  
                new System.Web.UI.WebControls.ListItem { Text = "Finished", Value = "fi" },
                new System.Web.UI.WebControls.ListItem { Text = "Sub Assembly", Value = "sa" },
                new System.Web.UI.WebControls.ListItem { Text = "Spares", Value = "sp" },
                new System.Web.UI.WebControls.ListItem { Text = "Raw Material", Value = "rm" },
                new System.Web.UI.WebControls.ListItem { Text = "Scrap", Value = "sc" },
                new System.Web.UI.WebControls.ListItem { Text = "Plant & Machinary/Capital Goods", Value = "pm" },
                new System.Web.UI.WebControls.ListItem { Text = "Moulds, Dies & Fixtures", Value = "md" },
                new System.Web.UI.WebControls.ListItem { Text = "Service", Value = "sv" },
                new System.Web.UI.WebControls.ListItem { Text = "Stationary", Value = "st" },
                new System.Web.UI.WebControls.ListItem { Text = "Computers & Accessories", Value = "ca" },
                new System.Web.UI.WebControls.ListItem { Text = "Consumables", Value = "co" },
                new System.Web.UI.WebControls.ListItem { Text = "Tools & Tackles", Value = "tt" },
                new System.Web.UI.WebControls.ListItem { Text = "Others", Value = "ot" }
            };
            return listItems;
        }
        //
        #endregion
        //
        #region HR department
        //
        public ArrayList getArrayByString(string liststr)
        {
            //get individual ids
            ArrayList arl = new ArrayList();
            string[] arlvals = liststr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arlvals.Length; i++)
            {
                if (arl.Contains(arlvals[i].ToString()) == false)
                {
                    arl.Add(arlvals[i].ToString());
                }
            }
            return arl;
        }
        //
        public bool isValidEmployeeId(string empid)
        {
            //getting active emp list
            DataSet ds = new DataSet();
            EmployeeBLL empbll = new EmployeeBLL();
            ds = empbll.getObjectData("", 0);
            ds.Tables[0].DefaultView.RowFilter = "isactive=true";
            DataTable dt = ds.Tables[0].DefaultView.ToTable();
            bool f = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["empid"].ToString().ToLower() == empid.ToLower())
                {
                    f = true;
                    break;
                }
            }
            return f;
        }
        //
        public bool isValidEmployeeList(ArrayList arlemp)
        {
            //getting active emp list
            DataSet ds = new DataSet();
            EmployeeBLL empbll = new EmployeeBLL();
            ds = empbll.getObjectData("", 0);
            ds.Tables[0].DefaultView.RowFilter = "isactive=true";
            DataTable dt = ds.Tables[0].DefaultView.ToTable();
            //checking valid emp codes
            bool f = false;
            for (int i = 0; i < arlemp.Count; i++)
            {
                f = false;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (arlemp[i].ToString().ToLower() == dt.Rows[j]["empid"].ToString().ToLower())
                    {
                        f = true;
                        break;
                    }
                }
            }
            return f;
        }
        //
        public DataSet getGrades()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("grade");
            ds.Tables[0].Columns.Add("gradename");
            ds.Tables[0].Rows.Add("w", "Worker");
            ds.Tables[0].Rows.Add("o", "Other than Worker");
            ds.Tables[0].Rows.Add("s", "Staff");
            ds.Tables[0].Rows.Add("m", "Manager");
            ds.Tables[0].Rows.Add("d", "Director");
            ds.Tables[0].Rows.Add("c", "Consultant");
            ds.Tables[0].Rows.Add("a", "Apprentice");
            ds.Tables[0].Rows.Add("l", "Contractual Labour");
            ds.Tables[0].Rows.Add("g", "Security Guard");
            ds.Tables[0].Rows.Add("r", "CSR");
            return ds;
        }
        //
        //not to be used further
        public List<System.Web.UI.WebControls.ListItem> getGradeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Worker", Value = "w" },
                  new System.Web.UI.WebControls.ListItem { Text = "Staff", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Manager", Value = "m" },
                  new System.Web.UI.WebControls.ListItem { Text = "Director", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Consultant", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "Apprentice", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "Contractual Labour", Value = "l" },
                  new System.Web.UI.WebControls.ListItem { Text = "Security Guard", Value = "g" },
                  new System.Web.UI.WebControls.ListItem { Text = "CSR", Value = "r" }
            };
            return listItems;
            //DataSet ds = getDepUnits();
            //List<System.Web.UI.WebControls.ListItem> listItems = new List<System.Web.UI.WebControls.ListItem> { };
            //System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    li = new System.Web.UI.WebControls.ListItem();
            //    li.Text = dr["unitname"].ToString();
            //    li.Value = (int)dr["unit"].ToString();
            //    listItems.Add(li);
            //}
        }
        //
        public DataSet getEmpCategory()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("categoryid");
            ds.Tables[0].Columns.Add("categoryname");
            ds.Tables[0].Rows.Add("1", "Skilled");
            ds.Tables[0].Rows.Add("2", "Semiskilled");
            ds.Tables[0].Rows.Add("3", "Unskilled");
            ds.Tables[0].Rows.Add("0", "ALL");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getEmpCategoryList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Skilled", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "Semiskilled", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Unskilled", Value = "3" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getSearchFieldList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "--Select--", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Party Name", Value = "pn" },
                  new System.Web.UI.WebControls.ListItem { Text = "Address", Value = "ca" },
                  new System.Web.UI.WebControls.ListItem { Text = "Contact Person", Value = "cp" },
                  new System.Web.UI.WebControls.ListItem { Text = "Subject", Value = "cs" },
                  new System.Web.UI.WebControls.ListItem { Text = "Reference", Value = "cr" },
                  new System.Web.UI.WebControls.ListItem { Text = "Keywords", Value = "kw" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getLetterSeriesList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "--Select--", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Draft", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Main", Value = "m" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getBGTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Against Security", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Against Performance", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "n" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getEmpCategoryListReport()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "ALL", Value = "0" },
                new System.Web.UI.WebControls.ListItem { Text = "Skilled", Value = "1" },
                new System.Web.UI.WebControls.ListItem { Text = "Semiskilled", Value = "2" },
                new System.Web.UI.WebControls.ListItem { Text = "Unskilled", Value = "3" }
            };
            return listItems;
        }
        //
        public DataSet getDesignation()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("desigid");
            ds.Tables[0].Columns.Add("designation");
            ds.Tables[0].Rows.Add("1", "Skilled");
            ds.Tables[0].Rows.Add("2", "Semiskilled");
            ds.Tables[0].Rows.Add("3", "Unskilled");
            ds.Tables[0].Rows.Add("4", "Unskilled Helper");
            ds.Tables[0].Rows.Add("5", "Clerk");
            ds.Tables[0].Rows.Add("6", "Driver");
            ds.Tables[0].Rows.Add("7", "Mat. Testing Ins");
            ds.Tables[0].Rows.Add("8", "MQ");
            ds.Tables[0].Rows.Add("9", "MP");
            ds.Tables[0].Rows.Add("10", "MEDP");
            ds.Tables[0].Rows.Add("11", "M.Finance");
            ds.Tables[0].Rows.Add("12", "Marketing");
            ds.Tables[0].Rows.Add("13", "Asst. Marketing");
            ds.Tables[0].Rows.Add("14", "Store Incharge");
            ds.Tables[0].Rows.Add("15", "GM");
            ds.Tables[0].Rows.Add("16", "Staff");
            ds.Tables[0].Rows.Add("17", "Helper");
            ds.Tables[0].Rows.Add("18", "MIS Manager");
            ds.Tables[0].Rows.Add("19", "7:30 Shift");
            ds.Tables[0].Rows.Add("20", "8:30 Shift");
            ds.Tables[0].Rows.Add("0", "Not defined");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getDesignationList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Skilled", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "Semiskilled", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Unskilled", Value = "3" },
                  new System.Web.UI.WebControls.ListItem { Text = "Unskilled Helper", Value = "4" },
                  new System.Web.UI.WebControls.ListItem { Text = "Clerk", Value = "5" },
                  new System.Web.UI.WebControls.ListItem { Text = "Driver", Value = "6" },
                  new System.Web.UI.WebControls.ListItem { Text = "Mat. Testing Ins", Value = "7" },
                  new System.Web.UI.WebControls.ListItem { Text = "MQ", Value = "8" },
                  new System.Web.UI.WebControls.ListItem { Text = "MP", Value = "9" },
                  new System.Web.UI.WebControls.ListItem { Text = "MEDP", Value = "10" },
                  new System.Web.UI.WebControls.ListItem { Text = "M.Finance", Value = "11" },
                  new System.Web.UI.WebControls.ListItem { Text = "Marketing", Value = "12" },
                  new System.Web.UI.WebControls.ListItem { Text = "Asst. Marketing", Value = "13" },
                  new System.Web.UI.WebControls.ListItem { Text = "Store Incharge", Value = "14" },
                  new System.Web.UI.WebControls.ListItem { Text = "GM", Value = "15" },
                  new System.Web.UI.WebControls.ListItem { Text = "Staff", Value = "16" },
                  new System.Web.UI.WebControls.ListItem { Text = "Helper", Value = "17" },
                  new System.Web.UI.WebControls.ListItem { Text = "HR Manager", Value = "21" },
                  new System.Web.UI.WebControls.ListItem { Text = "MIS Manager", Value = "18" },
                  new System.Web.UI.WebControls.ListItem { Text = "7:30 Shift", Value = "19" },
                  new System.Web.UI.WebControls.ListItem { Text = "8:30 Shift", Value = "20" },
                  new System.Web.UI.WebControls.ListItem { Text = "Not defined", Value = "0" }
            };
            return listItems;
        }
        //
        public DataSet getShift()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("shiftid");
            ds.Tables[0].Columns.Add("shiftname");
            ds.Tables[0].Rows.Add("d", "Day");
            ds.Tables[0].Rows.Add("n", "Night");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getShiftList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Day", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Night", Value = "n" }
            };
            return listItems;
        }
        //
        public DataSet getShiftName()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("DesigId");
            ds.Tables[0].Columns.Add("DesigName");
            ds.Tables[0].Rows.Add("19", "7:30 Shift");
            ds.Tables[0].Rows.Add("20", "8:30 Shift");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getShifName()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "7:30 Shift", Value = "19" },
                  new System.Web.UI.WebControls.ListItem { Text = "8:30 Shift", Value = "20" },
                  
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getEmployeeOpeningVTypes()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Opening", Value = "op" },
                  new System.Web.UI.WebControls.ListItem { Text = "Encashment", Value = "en" }
            };
            return listItems;
        }
        //
        public DataSet getServiceTypes()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("servicetype");
            ds.Tables[0].Columns.Add("servicetypename");
            ds.Tables[0].Rows.Add("n", "New");
            ds.Tables[0].Rows.Add("p", "Probation");
            ds.Tables[0].Rows.Add("o", "Off-role");
            ds.Tables[0].Rows.Add("r", "On-role");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getServiceTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "New", Value = "n" },
                  new System.Web.UI.WebControls.ListItem { Text = "Probation", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Off-role", Value = "o" },
                  new System.Web.UI.WebControls.ListItem { Text = "On-role", Value = "r" }
            };
            return listItems;
        }
        //
        public DataSet getMonths()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("monthid");
            ds.Tables[0].Columns.Add("monthname");
            ds.Tables[0].Rows.Add("1", "January");
            ds.Tables[0].Rows.Add("2", "February");
            ds.Tables[0].Rows.Add("3", "March");
            ds.Tables[0].Rows.Add("4", "April");
            ds.Tables[0].Rows.Add("5", "May");
            ds.Tables[0].Rows.Add("6", "June");
            ds.Tables[0].Rows.Add("7", "July");
            ds.Tables[0].Rows.Add("8", "August");
            ds.Tables[0].Rows.Add("9", "September");
            ds.Tables[0].Rows.Add("10", "October");
            ds.Tables[0].Rows.Add("11", "November");
            ds.Tables[0].Rows.Add("12", "December");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getMonthList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "January", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "February", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "March", Value = "3" },
                  new System.Web.UI.WebControls.ListItem { Text = "April", Value = "4" },
                  new System.Web.UI.WebControls.ListItem { Text = "May", Value = "5" },
                  new System.Web.UI.WebControls.ListItem { Text = "June", Value = "6" },
                  new System.Web.UI.WebControls.ListItem { Text = "July", Value = "7" },
                  new System.Web.UI.WebControls.ListItem { Text = "August", Value = "8" },
                  new System.Web.UI.WebControls.ListItem { Text = "September", Value = "9" },
                  new System.Web.UI.WebControls.ListItem { Text = "October", Value = "10" },
                  new System.Web.UI.WebControls.ListItem { Text = "November", Value = "11" },
                  new System.Web.UI.WebControls.ListItem { Text = "December", Value = "12" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getAttendanceTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  //this is automatic by biometric 
                  new System.Web.UI.WebControls.ListItem { Text = "Present", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Week-off day", Value = "wd" },
                  new System.Web.UI.WebControls.ListItem { Text = "Holiday", Value = "hld" }
            };
            return listItems;
        }
        //
        public DataSet getAttedanceDetailType()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("attvalue");
            ds.Tables[0].Columns.Add("attvaluename");
            ds.Tables[0].Rows.Add("inc", "Incentive");
            ds.Tables[0].Rows.Add("sl", "Short leave");
            ds.Tables[0].Rows.Add("lat", "Late attendance");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getgetAttedanceDetailTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Incentive", Value = "inc" },
                  new System.Web.UI.WebControls.ListItem { Text = "Short leave", Value = "sl" },
                  new System.Web.UI.WebControls.ListItem { Text = "Late attendance", Value = "lat" },
            };
            return listItems;
        }
        //
        public DataSet getComplianceHead()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("value");
            ds.Tables[0].Columns.Add("text");
            ds.Tables[0].Rows.Add("veh", "Vehicle");
            ds.Tables[0].Rows.Add("cact", "Companies Act");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getComplianceHeadList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Vehicle", Value = "veh" },
                  new System.Web.UI.WebControls.ListItem { Text = "Companies Act", Value = "cact" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getTravelTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Domestic", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "International", Value = "i" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getTravelModeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Rail", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Road", Value = "v" },
                  new System.Web.UI.WebControls.ListItem { Text = "Air", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "Sea", Value = "s" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getVisitStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Initiated", Value = "i" },
                  new System.Web.UI.WebControls.ListItem { Text = "Approved", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cancelled", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "f" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getBillStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "c" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getPendingCompStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "c" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getInvCtrlStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "a" }
            };
            return listItems;
        }
        //
        #endregion
        //
        #region Mktg-V2
        //
        public List<System.Web.UI.WebControls.ListItem> getInvCtrlRptList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Unloading", Value = "un" },
                  new System.Web.UI.WebControls.ListItem { Text = "Receipted Challan", Value = "rc" },
                  new System.Web.UI.WebControls.ListItem { Text = "Receipt Note", Value = "rn" },
                  new System.Web.UI.WebControls.ListItem { Text = "Bill Submission Part1", Value = "bsp1" },
                  new System.Web.UI.WebControls.ListItem { Text = "Bill Submission Part2", Value = "bsp2" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getMAReasonList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "N/A" },
                  new System.Web.UI.WebControls.ListItem { Text = "Bank Detail", Value = "Bank Detail" },
                  new System.Web.UI.WebControls.ListItem { Text = "GST", Value = "GST" },
                  new System.Web.UI.WebControls.ListItem { Text = "Item Description", Value = "Item Description" },
                  new System.Web.UI.WebControls.ListItem { Text = "Others", Value = "Others" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getYesNoList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Yes", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "No", Value = "0" }
            };
            return listItems;
        }
        //
        public DataSet getPOTypesRpt()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("potype");
            ds.Tables[0].Columns.Add("potypename");
            ds.Tables[0].Rows.Add("0", "All");
            ds.Tables[0].Rows.Add("x", "Railway, Private");
            ds.Tables[0].Rows.Add("t", "Railway");
            ds.Tables[0].Rows.Add("p", "Private");
            ds.Tables[0].Rows.Add("i", "Internal");
            ds.Tables[0].Rows.Add("w", "Warranty");
            ds.Tables[0].Rows.Add("n", "Non-PO");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getPOTypeRptList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Railway, Private", Value = "x" },
                  new System.Web.UI.WebControls.ListItem { Text = "Railway", Value = "t" },
                  new System.Web.UI.WebControls.ListItem { Text = "Private", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Internal", Value = "i" },
                  new System.Web.UI.WebControls.ListItem { Text = "Warranty", Value = "w" },
                  new System.Web.UI.WebControls.ListItem { Text = "Non-PO", Value = "n" }
            };
            return listItems;
        }
        //
        public DataSet getOrderStatus()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("status");
            ds.Tables[0].Columns.Add("statusname");
            ds.Tables[0].Rows.Add("0", "All");
            ds.Tables[0].Rows.Add("x", "Pending");
            ds.Tables[0].Rows.Add("i", "In Progress");
            ds.Tables[0].Rows.Add("p", "Partially Executed");
            ds.Tables[0].Rows.Add("e", "Executed");
            ds.Tables[0].Rows.Add("c", "Cancelled");
            ds.Tables[0].Rows.Add("h", "On-Hold");
            ds.Tables[0].Rows.Add("x1", "Lapsed DP");
            ds.Tables[0].Rows.Add("a", "Executed By Admin");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getOrderStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "x" },
                  new System.Web.UI.WebControls.ListItem { Text = "In Progress", Value = "i" },
                  new System.Web.UI.WebControls.ListItem { Text = "Partially Executed", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Executed", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cancelled", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "On-Hold", Value = "h" },
                  new System.Web.UI.WebControls.ListItem { Text = "Lapsed DP", Value = "x1" },
                  new System.Web.UI.WebControls.ListItem { Text = "Executed By Admin", Value = "a" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getTenderStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "Quoted", Value = "q" },
                  new System.Web.UI.WebControls.ListItem { Text = "Quoted-L1/L2", Value = "l" },
                  new System.Web.UI.WebControls.ListItem { Text = "Quoted-AAL/CO", Value = "a" },
                  new System.Web.UI.WebControls.ListItem { Text = "To be Quoted", Value = "t" },
                  new System.Web.UI.WebControls.ListItem { Text = "PO Received", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Not to be Quoted", Value = "n" }
            };
            return listItems;
        }
        //
        public DataSet getInvoiceModeRpt()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("invtype");
            ds.Tables[0].Columns.Add("invtypename");
            ds.Tables[0].Rows.Add("0", "All");
            ds.Tables[0].Rows.Add("exh", "Exhibition");
            ds.Tables[0].Rows.Add("gst", "GST");
            ds.Tables[0].Rows.Add("job", "Job Work");
            ds.Tables[0].Rows.Add("dbn", "Debit Note");
            ds.Tables[0].Rows.Add("cdn", "Credit Note");
            ds.Tables[0].Rows.Add("rnt", "Rent");
            ds.Tables[0].Rows.Add("rms", "Stock Transfer");
            ds.Tables[0].Rows.Add("sup", "Supplimentary");
            ds.Tables[0].Rows.Add("exp", "Export");
            ds.Tables[0].Rows.Add("eds", "ED/Sale Tax");
            ds.Tables[0].Rows.Add("slr", "Sale Return");
            return ds;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getInvoiceModeRptList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "0" },
                  new System.Web.UI.WebControls.ListItem { Text = "GST", Value = "gst" },
                  new System.Web.UI.WebControls.ListItem { Text = "Job Work", Value = "job" },
                  new System.Web.UI.WebControls.ListItem { Text = "Debit Note", Value = "dbn" },
                  new System.Web.UI.WebControls.ListItem { Text = "Credit Note", Value = "cdn" },
                  new System.Web.UI.WebControls.ListItem { Text = "Rent", Value = "rnt" },
                  new System.Web.UI.WebControls.ListItem { Text = "Stock Transfer", Value = "rms" },
                  new System.Web.UI.WebControls.ListItem { Text = "Supplimentary", Value = "sup" },
                  new System.Web.UI.WebControls.ListItem { Text = "Export", Value = "exp" },
                  new System.Web.UI.WebControls.ListItem { Text = "Exhibition", Value = "exh" },
                  new System.Web.UI.WebControls.ListItem { Text = "ED/Sale Tax", Value = "eds" },
                  new System.Web.UI.WebControls.ListItem { Text = "Sale Return", Value = "slr" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getPOSaleInvoiceModeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "GST", Value = "gst" },
                  new System.Web.UI.WebControls.ListItem { Text = "Supplimentary", Value = "sup" },
                  new System.Web.UI.WebControls.ListItem { Text = "Export", Value = "exp" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getDraftProformaOptList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Draft", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Proforma", Value = "p" }
            };
            return listItems;
        }
        //
        public List<System.Web.UI.WebControls.ListItem> getScrapTransferOptionList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Scrap Sale", Value = "s" },
                  new System.Web.UI.WebControls.ListItem { Text = "Rejection/Miscellaneous", Value = "r" },
                  new System.Web.UI.WebControls.ListItem { Text = "Stock Transfer", Value = "m" },
                  new System.Web.UI.WebControls.ListItem { Text = "Rent", Value = "n" },
                  new System.Web.UI.WebControls.ListItem { Text = "Job Work", Value = "j" },
                  new System.Web.UI.WebControls.ListItem { Text = "Debit Note", Value = "d" },
                  new System.Web.UI.WebControls.ListItem { Text = "Credit Note", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "Transfer (OLD)", Value = "t" },
                  new System.Web.UI.WebControls.ListItem { Text = "Exhibition", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Sale Return", Value = "x" }
            };
            return listItems;
        }
        //

        public string getInvoiceMode(string chinv)
        {
            string ret = "gst";
            if (chinv.ToLower() == "n")
            {
                ret = "rnt";//rent
            }
            else if (chinv.ToLower() == "m")
            {
                ret = "rms";//raw material
            }
            else if (chinv.ToLower() == "j")
            {
                ret = "job";//job-work
            }
            else if (chinv.ToLower() == "d")
            {
                ret = "dbn";//debit note
            }
            else if (chinv.ToLower() == "c")
            {
                ret = "cdn";//credit note
            }
            else if (chinv.ToLower() == "e")
            {
                ret = "exh";//exhibition
            }
            else if (chinv.ToLower() == "x")
            {
                ret = "slr";//sale return
            }
            return ret;
        }
        //
        #endregion
        //
        #endregion
        //
        #region API(s) Section
        //
        public string performAPICall_PNRServiceRequest(string pnrnumber)
        {
            //railway pnr status
            //pattern --http://api.railwayapi.com/pnr_status/pnr/<pnr no>/apikey/<apikey>/
            string url = "http://api.railwayapi.com/pnr_status/pnr/" + pnrnumber + "/apikey/khukf6690";
            string st = string.Empty;
            string results = "";
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                results = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                results = ex.Message;
            }
            return results;
        }
        //
        public string performAPICall_SendSMS(string receipient, string rcpmessage)
        {
            string url = "http://smsfortius.com/api/mt/SendSMS?user=prggroup&password=123123&senderid=PRAGIT&channel=trans&DCS=0&flashsms=0&number=" + receipient + "&route=2&text=" + rcpmessage + "";
            string st = string.Empty;
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
            catch (Exception ex)
            {
                st = ex.Message;
            }
            return st;
        }
        //
        public string performAPICall_GetLoggedSMS()
        {
            //getting sms
            //pattern1 --http://czarsindia.com/longcode/longcode.txt
            string url = "http://czarsindia.com/longcode/longcode.txt";
            string st = string.Empty;
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
            catch (Exception ex)
            {
                st = ex.Message;
            }
            return st;
        }
        //
        public string getOTP()
        {
            //char[] charArr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            int noofcharacters = 6;
            for (int i = 0; i < noofcharacters; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                    strrandom += charArr.GetValue(pos);
                else
                    i--;
            }
            return strrandom;
        }
        //
        public string getCaptcha()
        {
            //char[] charArr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            //char[] charArr = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            int noofcharacters = 4;
            for (int i = 0; i < noofcharacters; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                    strrandom += charArr.GetValue(pos);
                else
                    i--;
            }
            return strrandom;
        }
        //
        public string sendOTPtoEmail(string emailmsg, string emailto)
        {
            string res = "";
            try
            {
                MailMessage mail = new MailMessage();
                //comma separated multiple emails
                mail.To.Add(emailto);
                //mail.Bcc.Add("");
                mail.From = new MailAddress("erp@praggroup.com", "Prag ERP");
                mail.Subject = "PRAG ERP OTP Alert";
                emailmsg += "<br /><br />Regards:<br />Prag ERP";
                emailmsg += "<br /><br /><span style='color: red;'>Please make sure to <b>'Logout'</b> from <b>ERP System</b> after each and every login session for security reason.</span>";
                //message template
                mail.Body += " <html>";
                mail.Body += "<body>";
                mail.Body += emailmsg;
                mail.Body += "</body>";
                mail.Body += "</html>";
                mail.IsBodyHtml = true;
                //
                SmtpClient smtp = new SmtpClient();
                //Your SMTP Server Address
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;//465
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", getEmailPassword());
                smtp.EnableSsl = true;
                smtp.Send(mail);
                res = "1";
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        //
        public string SendAlertsOnEmail(int emailset, string subject, string msgcontent)
        {
            string res = "";
            MailMessage mail = new MailMessage();
            //
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_emailset_for_alert";
            cmd.Parameters.Add(getPObject("@eset", emailset, DbType.Int32));
            DataSet dsEmail = new DataSet();
            fillFromDatabase(dsEmail, cmd);
            //
            if (dsEmail.Tables.Count == 0)
            {
                res = "Email(s) not found!";
                return res;
            }
            if (dsEmail.Tables[0].Rows.Count == 0)
            {
                res = "Email(s) not found!";
                return res;
            }
            string addrs = "";
            for (int i = 0; i < dsEmail.Tables[0].Rows.Count; i++)
            {
                addrs += dsEmail.Tables[0].Rows[i]["email"].ToString() + ",";
            }
            addrs = addrs.Substring(0, addrs.Length - 1);
            try
            {
                mail.From = new MailAddress("erp@praggroup.com");
                mail.To.Add(addrs);
                mail.Subject = subject;
                mail.Body = msgcontent;
                mail.IsBodyHtml = true;
                //
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", getEmailPassword());
                smtp.EnableSsl = true;
                smtp.Send(mail);
                //
                res = "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        //
        public string SendEmailByERP(ArrayList arladdress, string subject, string msgcontent)
        {
            string res = "";
            MailMessage mail = new MailMessage();
            System.Collections.ArrayList arl = new System.Collections.ArrayList();
            if (arladdress.Count == 0)
            {
                res = "Email(s) not found!";
                return res;
            }
            string addrs = "";
            for (int i = 0; i < arladdress.Count; i++)
            {
                addrs += arladdress[i].ToString() + ",";
            }
            addrs = addrs.Substring(0, addrs.Length - 1);
            try
            {
                mail.From = new MailAddress("erp@praggroup.com");
                mail.To.Add(addrs);
                mail.Subject = subject;
                mail.Body = msgcontent;
                mail.IsBodyHtml = true;
                //
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("erp@praggroup.com", getEmailPassword());
                smtp.EnableSsl = true;
                smtp.Send(mail);
                //
                res = "Email Sent Successfully";
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        //
        #endregion
        //
    }
}