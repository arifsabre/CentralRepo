using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace ManufacturingManagement_V2.Models
{
    public class CompanyBLL : DbContext
    {
        //
        internal DbSet<CompanyMdl> Companies { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static CompanyBLL Instance
        {
            get { return new CompanyBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, CompanyMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CmpName", dbobject.CmpName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address1", dbobject.Address1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address2", dbobject.Address2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address3", dbobject.Address3.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TinNo", dbobject.TinNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TinDate", dbobject.TinDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@PhoneOff", dbobject.PhoneOff.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PhoneResi", dbobject.PhoneResi.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FaxNo", dbobject.FaxNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MobileNo", dbobject.MobileNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ContPer", dbobject.ContPer.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EMail", dbobject.EMail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Website", dbobject.Website.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer1", dbobject.Footer1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer2", dbobject.Footer2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer3", dbobject.Footer3.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer4", dbobject.Footer4.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer5", dbobject.Footer5.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer6", dbobject.Footer6.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer7", dbobject.Footer7.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer8", dbobject.Footer8.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer9", dbobject.Footer9.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Footer10", dbobject.Footer10.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CIN", dbobject.CIN, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CerEccNo", dbobject.CerEccNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CstNo", dbobject.CstNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CstDate", dbobject.CstDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@ShortName", dbobject.ShortName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdCmpCode", dbobject.TdCmpCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PanNo", dbobject.PanNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DispIndex", dbobject.DispIndex, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@GSTinNo", dbobject.GSTinNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@GSTinDate", dbobject.GSTinDate.ToShortDateString(), DbType.Date));
            cmd.Parameters.Add(mc.getPObject("@StateCode", dbobject.StateCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BankName", dbobject.BankName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AccountNo", dbobject.AccountNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IfscCode", dbobject.IfscCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmployerAddress", dbobject.EmployerAddress, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ManagerName", dbobject.ManagerName, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ManagerAddress", dbobject.ManagerAddress, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFNo", dbobject.PFNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ESICNo", dbobject.ESICNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FRegNo", dbobject.FRegNo, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SalaryAccountNo", dbobject.SalaryAccountNo.Trim(), DbType.String));
        }
        //
        private void addCommandParametersCompYear(SqlCommand cmd, int compcode, DateTime fromdate, string finyear, string longfinyear)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@fromdate", fromdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@todate", fromdate.AddYears(1).AddDays(-1).ToShortDateString(), DbType.Date));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@longfinyear", longfinyear, DbType.String));
        }
        //
        private List<CompanyMdl> createObjectList(DataSet ds)
        {
            List<CompanyMdl> companies = new List<CompanyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CompanyMdl objmdl = new CompanyMdl();
                objmdl.CompCode = Convert.ToInt16(dr["CompCode"].ToString());
                if (dr.Table.Columns.Contains("cmpname"))
                {
                    objmdl.CmpName = dr["cmpname"].ToString();
                }
                if (dr.Table.Columns.Contains("address1"))
                {
                    objmdl.Address1 = dr["address1"].ToString();
                }
                if (dr.Table.Columns.Contains("address2"))
                {
                    objmdl.Address2 = dr["address2"].ToString();
                }
                if (dr.Table.Columns.Contains("address3"))
                {
                    objmdl.Address3 = dr["address3"].ToString();
                }
                if (dr.Table.Columns.Contains("tinno"))
                {
                    objmdl.TinNo = dr["tinno"].ToString();
                }
                if (dr.Table.Columns.Contains("tindate"))
                {
                    objmdl.TinDate = Convert.ToDateTime(dr["tindate"].ToString());
                }
                if (dr.Table.Columns.Contains("phoneoff"))
                {
                    objmdl.PhoneOff = dr["phoneoff"].ToString();
                }
                if (dr.Table.Columns.Contains("phoneresi"))
                {
                    objmdl.PhoneResi = dr["phoneresi"].ToString();
                }
                if (dr.Table.Columns.Contains("faxno"))
                {
                    objmdl.FaxNo = dr["faxno"].ToString();
                }
                if (dr.Table.Columns.Contains("mobileno"))
                {
                    objmdl.MobileNo = dr["mobileno"].ToString();
                }
                if (dr.Table.Columns.Contains("contper"))
                {
                    objmdl.ContPer = dr["contper"].ToString();
                }
                if (dr.Table.Columns.Contains("email"))
                {
                    objmdl.EMail = dr["email"].ToString();
                }
                if (dr.Table.Columns.Contains("website"))
                {
                    objmdl.Website = dr["website"].ToString();
                }
                if (dr.Table.Columns.Contains("footer1"))
                {
                    objmdl.Footer1 = dr["footer1"].ToString();
                }
                if (dr.Table.Columns.Contains("footer2"))
                {
                    objmdl.Footer2 = dr["footer2"].ToString();
                }
                if (dr.Table.Columns.Contains("footer3"))
                {
                    objmdl.Footer3 = dr["footer3"].ToString();
                }
                if (dr.Table.Columns.Contains("footer4"))
                {
                    objmdl.Footer4 = dr["footer4"].ToString();
                }
                if (dr.Table.Columns.Contains("footer5"))
                {
                    objmdl.Footer5 = dr["footer5"].ToString();
                }
                if (dr.Table.Columns.Contains("footer6"))
                {
                    objmdl.Footer6 = dr["footer6"].ToString();
                }
                if (dr.Table.Columns.Contains("footer7"))
                {
                    objmdl.Footer7 = dr["footer7"].ToString();
                }
                if (dr.Table.Columns.Contains("footer8"))
                {
                    objmdl.Footer8 = dr["footer8"].ToString();
                }
                if (dr.Table.Columns.Contains("footer9"))
                {
                    objmdl.Footer9 = dr["footer9"].ToString();
                }
                if (dr.Table.Columns.Contains("footer10"))
                {
                    objmdl.Footer10 = dr["footer10"].ToString();
                }
                if (dr.Table.Columns.Contains("CIN"))
                {
                    objmdl.CIN = dr["CIN"].ToString();
                }
                if (dr.Table.Columns.Contains("CerEccNo"))
                {
                    objmdl.CerEccNo = dr["CerEccNo"].ToString();
                }
                if (dr.Table.Columns.Contains("CstNo"))
                {
                    objmdl.CstNo = dr["CstNo"].ToString();
                }
                if (dr.Table.Columns.Contains("CstDate"))
                {
                    objmdl.CstDate = Convert.ToDateTime(dr["CstDate"].ToString());
                }
                if (dr.Table.Columns.Contains("ShortName"))
                {
                    objmdl.ShortName = dr["ShortName"].ToString();
                }
                if (dr.Table.Columns.Contains("TdCmpCode"))
                {
                    objmdl.TdCmpCode = dr["TdCmpCode"].ToString();
                }
                if (dr.Table.Columns.Contains("PanNo"))
                {
                    objmdl.PanNo = dr["PanNo"].ToString();
                }
                if (dr.Table.Columns.Contains("DispIndex"))
                {
                    objmdl.DispIndex = dr["DispIndex"].ToString();
                }
                if (dr.Table.Columns.Contains("gstinno"))
                {
                    objmdl.GSTinNo = dr["gstinno"].ToString();
                }
                if (dr.Table.Columns.Contains("gstindate"))
                {
                    objmdl.GSTinDate = Convert.ToDateTime(dr["gstindate"].ToString());
                }
                if (dr.Table.Columns.Contains("StateCode"))
                {
                    objmdl.StateCode = dr["StateCode"].ToString();
                }
                if (dr.Table.Columns.Contains("BankName"))
                {
                    objmdl.BankName = dr["BankName"].ToString();
                }
                if (dr.Table.Columns.Contains("AccountNo"))
                {
                    objmdl.AccountNo = dr["AccountNo"].ToString();
                }
                if (dr.Table.Columns.Contains("IfscCode"))
                {
                    objmdl.IfscCode = dr["IfscCode"].ToString();
                }
                if (dr.Table.Columns.Contains("EmployerAddress"))
                {
                    objmdl.EmployerAddress = dr["EmployerAddress"].ToString();
                }
                if (dr.Table.Columns.Contains("ManagerName"))
                {
                    objmdl.ManagerName = dr["ManagerName"].ToString();
                }
                if (dr.Table.Columns.Contains("ManagerAddress"))
                {
                    objmdl.ManagerAddress = dr["ManagerAddress"].ToString();
                }
                if (dr.Table.Columns.Contains("PFNo"))
                {
                    objmdl.PFNo = dr["PFNo"].ToString();
                }
                if (dr.Table.Columns.Contains("ESICNo"))
                {
                    objmdl.ESICNo = dr["ESICNo"].ToString();
                }
                if (dr.Table.Columns.Contains("FRegNo"))
                {
                    objmdl.FRegNo = dr["FRegNo"].ToString();
                }
                if (dr.Table.Columns.Contains("SalaryAccountNo"))
                {
                    objmdl.SalaryAccountNo = dr["SalaryAccountNo"].ToString();
                }
                companies.Add(objmdl);
            }
            return companies;
        }
        //
        private bool isFoundShortName(string shortname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_shortname";
            cmd.Parameters.Add(mc.getPObject("@shortname", shortname, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate shortname not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkDuplicateInsert(string cmpname)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_chkdup_insert_tbl_company";
            cmd.Parameters.Add(mc.getPObject("@cmpname", cmpname, DbType.String));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Duplicate company not allowed!";
            return false;
        }
        //
        internal DataSet getFinYearNdLongFinYear(DateTime fromdate)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("finyear");
            ds.Tables[0].Columns.Add("longfinyear");
            DateTime todate = fromdate.AddYears(1).AddDays(-1);
            string yearf = fromdate.Year.ToString();
            string yeart = todate.Year.ToString();
            string finyear = yearf.Substring(2, 2) + "-" + yeart.Substring(2, 2);
            string longfinyear = yearf + "-" + yeart;
            ds.Tables[0].Rows.Add(finyear, longfinyear);
            return ds;
        }
        //
        private bool checkSetValidModel(CompanyMdl dbobject)
        {
            if (dbobject.TinNo == null)
            {
                dbobject.TinNo = "";
            }
            if (mc.isValidDate(dbobject.TinDate) == false)
            {
                dbobject.TinDate = DateTime.Now;
            }
            if (dbobject.CstNo == null)
            {
                dbobject.CstNo = "";
            }
            if (mc.isValidDate(dbobject.CstDate) == false)
            {
                dbobject.CstDate = DateTime.Now;
            }
            if (dbobject.PhoneOff == null)
            {
                dbobject.PhoneOff = "";
            }
            if (dbobject.PhoneResi == null)
            {
                dbobject.PhoneResi = "";
            }
            if (dbobject.FaxNo == null)
            {
                dbobject.FaxNo = "";
            }
            if (dbobject.MobileNo == null)
            {
                dbobject.MobileNo = "";
            }
            if (dbobject.ContPer == null)
            {
                dbobject.ContPer = "";
            }
            if (dbobject.EMail == null)
            {
                dbobject.EMail = "";
            }
            if (dbobject.Website == null)
            {
                dbobject.Website = "";
            }
            if (dbobject.Address2 == null)
            {
                dbobject.Address2 = "";
            }
            if (dbobject.Address3 == null)
            {
                dbobject.Address3 = "";
            }
            if (dbobject.Footer1 == null)
            {
                dbobject.Footer1 = "";
            }
            if (dbobject.Footer2 == null)
            {
                dbobject.Footer2 = "";
            }
            if (dbobject.Footer3 == null)
            {
                dbobject.Footer3 = "";
            }
            if (dbobject.Footer4 == null)
            {
                dbobject.Footer4 = "";
            }
            if (dbobject.Footer5 == null)
            {
                dbobject.Footer5 = "";
            }
            if (dbobject.Footer6 == null)
            {
                dbobject.Footer6 = "";
            }
            if (dbobject.Footer7 == null)
            {
                dbobject.Footer7 = "";
            }
            if (dbobject.Footer8 == null)
            {
                dbobject.Footer8 = "";
            }
            if (dbobject.Footer9 == null)
            {
                dbobject.Footer9 = "";
            }
            if (dbobject.Footer10 == null)
            {
                dbobject.Footer10 = "";
            }
            if (dbobject.CIN == null)
            {
                dbobject.CIN = "";
            }
            if (dbobject.CerEccNo == null)
            {
                dbobject.CerEccNo = "";
            }
            if (dbobject.BankName == null)
            {
                dbobject.BankName = "";
            }
            if (dbobject.IfscCode == null)
            {
                dbobject.IfscCode = "";
            }
            if (dbobject.AccountNo == null)
            {
                dbobject.AccountNo = "";
            }
            if (dbobject.EmployerAddress == null)
            {
                dbobject.EmployerAddress = "";
            }
            if (dbobject.ManagerName == null)
            {
                dbobject.ManagerName = "";
            }
            if (dbobject.ManagerAddress == null)
            {
                dbobject.ManagerAddress = "";
            }
            if (dbobject.PFNo == null)
            {
                dbobject.PFNo = "";
            }
            if (dbobject.ESICNo == null)
            {
                dbobject.ESICNo = "";
            }
            if (dbobject.FRegNo == null)
            {
                dbobject.FRegNo = "";
            }
            if (dbobject.TdCmpCode == null)
            {
                dbobject.TdCmpCode = "";
            }
            if (dbobject.PanNo == null)
            {
                dbobject.PanNo = "";
            }
            if (dbobject.SalaryAccountNo == null)
            {
                dbobject.SalaryAccountNo = "";
            }
            if (mc.isValidDate(dbobject.GSTinDate) == false)
            {
                dbobject.GSTinDate = DateTime.Now;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertCompany(CompanyMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (checkDuplicateInsert(dbobject.CmpName) == false) { return; };
            if (isFoundShortName(dbobject.ShortName) == true) { return; };
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
                //
                //to tbl_company
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_company";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                //to tbl_compyear
                DataSet ds = getFinYearNdLongFinYear(objCookie.getFromDate());
                //
                int compcode = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_company, "compcode"));
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_compyear";
                addCommandParametersCompYear(cmd, compcode, objCookie.getFromDate(), ds.Tables[0].Rows[0]["finyear"].ToString(), ds.Tables[0].Rows[0]["longfinyear"].ToString());
                cmd.ExecuteNonQuery();
                //
                //to tbl_swconfig  --it is not in use yet
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_swconfig";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@remarks", "", DbType.String));
                cmd.ExecuteNonQuery();
                //
                //to create fixed account heads
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_create_fixed_accounts";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", ds.Tables[0].Rows[0]["finyear"].ToString(), DbType.String));
                cmd.ExecuteNonQuery();
                //to state
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_state";
                cmd.Parameters.Add(mc.getPObject("@stateid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@statename", "UP", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.ExecuteNonQuery();
                //to city
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_city";
                cmd.Parameters.Add(mc.getPObject("@cityid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@cityname", "Lucknow", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@stateid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.ExecuteNonQuery();
                //to area
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_area";
                cmd.Parameters.Add(mc.getPObject("@areaid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@areaname", "LKO", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@cityid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.ExecuteNonQuery();
                //to category
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_category";
                cmd.Parameters.Add(mc.getPObject("@categoryid", "1", DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@category", "NA", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.ExecuteNonQuery();
                //
                trn.Commit();
                Result = true;
                Message = "Company Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CompanyBLL", "insertCompany", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateCompany(CompanyMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_company";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.CompCode, DbType.Int16));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_company") == true)
                {
                    Message = "Duplicate company name not allowed!";
                }
                else if (ex.Message.Contains("uk_tbl_shortname_company") == true)
                {
                    Message = "Duplicate short name not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("CompanyBLL", "updateCompany", ex.Message);
                }
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
        internal CompanyMdl searchObject(int compcode)
        {
            DataSet ds = new DataSet();
            CompanyMdl dbobject = new CompanyMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_company";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
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
        //
        internal DataSet getObjectData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_display_tbl_company_erpv2";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CompanyMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal DataSet getCompanyDataByUser(int userid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_company_by_userid_erpv2";
            cmd.Parameters.Add(mc.getPObject("@userid", userid, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CompanyMdl> getCompanyListByUser(int userid)
        {
            DataSet ds = getCompanyDataByUser(userid);
            List<CompanyMdl> companies = new List<CompanyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new CompanyMdl
                {
                    CompCode = Convert.ToInt32(dr["compcode"].ToString()),
                    CmpName = dr["cmpname"].ToString()
                });
            }
            return companies;
        }
        //
        internal List<CompanyMdl> getCompanyListOther()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_usp_get_companyOR";
            mc.fillFromDatabase(ds, cmd);
            List<CompanyMdl> companies = new List<CompanyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                companies.Add(new CompanyMdl
                {
                    CompCode = Convert.ToInt32(dr["compcode"].ToString()),
                    CmpName = dr["cmpname"].ToString()
                });
            }
            return companies;
        }
        //
        internal List<FinYearMdl> getFinancialYear()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_financialyear";
            mc.fillFromDatabase(ds, cmd);
            List<FinYearMdl> finyears = new List<FinYearMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                finyears.Add(new FinYearMdl
                {
                    FinYear = dr["finyear"].ToString()
                });
            }
            return finyears;
        }
        //
        #endregion
        //
        #region financial year objects
        //
        internal FinYearMdl getLatestFinancialYear(int compcode)
        {
            DataSet ds=new DataSet();
            FinYearMdl obj = new FinYearMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_latest_financialyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.CompCode = Convert.ToInt32(ds.Tables[0].Rows[0]["compcode"].ToString());
                obj.FromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["fromdate"].ToString());
                obj.ToDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["todate"].ToString());
                obj.FinYear = ds.Tables[0].Rows[0]["finyear"].ToString();
                obj.LongFinYear = ds.Tables[0].Rows[0]["longfinyear"].ToString();
            }
            else
            {
                obj.FinYear = "x";
            }
            return obj;
        }
        //
        internal FinYearMdl getPreviousFinancialYear(string compcode)
        {
            DataSet ds = new DataSet();
            FinYearMdl obj = new FinYearMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_prevoius_financialyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.CompCode = Convert.ToInt32(ds.Tables[0].Rows[0]["compcode"].ToString());
                obj.FromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["fromdate"].ToString());
                obj.ToDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["todate"].ToString());
                obj.FinYear = ds.Tables[0].Rows[0]["finyear"].ToString();
                obj.LongFinYear = ds.Tables[0].Rows[0]["longfinyear"].ToString();
            }
            else
            {
                obj.FinYear = "x";
            }
            return obj;
        }
        //
        internal FinYearMdl getDateRangeByFinancialYear(int compcode, string finyear)
        {
            DataSet ds = new DataSet();
            FinYearMdl obj = new FinYearMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_date_range_by_finyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            obj.FromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["fromdate"].ToString());
            obj.ToDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["todate"].ToString());
            return obj;
        }
        //
        internal void installNewFinancialYear(int compcode, DateTime fromdate)
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
                DataSet ds = getFinYearNdLongFinYear(fromdate);
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_install_new_financial_year";
                addCommandParametersCompYear(cmd, compcode, fromdate, ds.Tables[0].Rows[0]["finyear"].ToString(), ds.Tables[0].Rows[0]["longfinyear"].ToString());
                cmd.ExecuteNonQuery();
                trn.Commit();
                Result = true;
                Message = "New Financial Year " + ds.Tables[0].Rows[0]["finyear"].ToString() + " Installed Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CompanyDAL", "installNewFinancialYear", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        private bool checkDeleteFinYear(int compcode, string finyear)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_check_delete_financialyear";
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            bool result = Convert.ToBoolean(mc.getFromDatabase(cmd));
            if (result == true) { return true; };
            Message = "Entries are available for financial year " + finyear + "\n\rSo It cannot be deleted.";
            return false;
        }
        //
        internal void deleteFinancialYear(int compcode, string finyear)
        {
            Result = false;
            Message = "";
            if (compcode.ToString() == objCookie.getCompCode() && finyear == objCookie.getFinYear())
            {
                Message = "Selected Financial Year cannot be deleted!";
                return;
            }
            if (checkDeleteFinYear(compcode, finyear) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_financial_year";
                cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
                cmd.ExecuteNonQuery();
                Result = true;
                Message = "Financial Year " + finyear + " Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CompanyDAL", "deleteFinancialYear", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
    }
}