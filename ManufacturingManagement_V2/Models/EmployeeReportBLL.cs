using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EmployeeReportBLL
    {
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region fetching objects
        //
        internal DataTable getEmployeeReportData(string grade, string depcode, int compcode,string emptype,string sorton,string sortorder, DateTime dtfrom, DateTime dtto, bool filterbydt, int categoryid, int qualid, int casteid, int agencyid, int locationid)
        {
            //[100088]
            string qualname = "";
            if (qualid > 0)
            {
                QualificationMdl qmdl = new QualificationMdl();
                QualificationBLL qualBLL = new QualificationBLL();
                qualBLL = new QualificationBLL();
                qmdl = qualBLL.searchObject(qualid);
                qualname = qmdl.Qualification;
            }
            DataSet ds = new DataSet();
            EmployeeBLL bllObject = new EmployeeBLL();
            ds = bllObject.getObjectData(grade,compcode);
            bllObject.setAdditionalColumns(ds);
            Message = "  ";//to prefix report name
            string rowfilter = "1=1";
            //filter
            if (emptype == "e")//existing
            {
                rowfilter += " and isactive = 1";
            }
            else if (emptype == "r")//resigned
            {
                rowfilter += " and isactive = 0";
            }
            if (depcode.Length > 0)
            {
                rowfilter += " and depcode = '" + depcode + "'";
            }
            if (filterbydt == true)
            {
                //rowfilter += " and (joiningdate >= '" + mc.getStringByDate(dtfrom) + "' and joiningdate <= '" + mc.getStringByDate(dtto) + "')";
                string dtf = dtfrom.Year.ToString() + "-" + dtfrom.Month.ToString() + "-" + dtfrom.Day.ToString();
                string dtt = dtto.Year.ToString() + "-" + dtto.Month.ToString() + "-" + dtto.Day.ToString();
                if (emptype == "r")//isactive=0, so leavingdate
                {
                    rowfilter += " and (leavingdate >= '" + dtf + "' and leavingdate <= '" + dtt + "')";
                }
                else//otherwise
                {
                    rowfilter += " and (joiningdate >= '" + dtf + "' and joiningdate <= '" + dtt + "')";
                }
            }
            if (categoryid > 0)
            {
                rowfilter += " and categoryid = " + categoryid + "";
            }
            if (qualid > 0)
            {
                rowfilter += " and qualid = " + qualid + "";
            }
            if (casteid > 0)
            {
                rowfilter += " and casteid = " + casteid + "";
            }
            if (agencyid > 0)
            {
                rowfilter += " and agencyid = " + agencyid + "";
            }
            if (locationid > 0)
            {
                rowfilter += " and locationid = " + locationid + "";
            }
            ds.Tables[0].DefaultView.RowFilter = rowfilter;
            //sorting
            if (sorton.ToLower() != "default")
            {
                ds.Tables[0].DefaultView.Sort = sorton + ' ' + sortorder;
            }
            DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            //filter string
            if (compcode > 0 && dtr.Rows.Count > 0)
            {
                Message += " Unit: " + dtr.Rows[0]["JoiningUnitName"].ToString() + ", ";
            }
            if (grade.Length > 0)
            {
                Message += " Grade: " + mc.getNameByKey(mc.getGrades(), "grade", grade, "gradename") + ", ";
            }
            if (depcode.Length > 0)
            {
                Message += " Department: " + mc.getNameByKey(UserBLL.Instance.getDepartmentData(), "depcode", depcode, "department") + ", ";
            }
            if (emptype == "e")
            {
                Message += " Existing Employee(s), ";
            }
            else if (emptype == "r")
            {
                Message += " Resigned Employee(s), ";
            }
            if (filterbydt == true)
            {
                if (emptype == "r")//isactive=0, so leavingdate
                {
                    Message += " Leaving Date from " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + ", ";
                }
                else//otherwise
                {
                    Message += " Joining Date from " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto) + ", ";
                }
            }
            if (categoryid > 0)
            {
                Message += " Category: " + mc.getNameByKey(mc.getEmpCategory(), "categoryid", categoryid.ToString(), "categoryname") + ", ";
            }
            if (qualid > 0)
            {
                Message += " Qualification: " + qualname + ", ";
            }
            if (casteid >0)
            {
                Message += " Caste: "+ mc.getNameByKey(EmployeeBLL.Instance.getEmployeeCasteData(), "casteid", casteid.ToString(), "castename")+"  ";
            }
            if (agencyid > 0)
            {
                Message += " Agency: " + mc.getNameByKey(EmployeeBLL.Instance.getAgencyData(), "agencyid", agencyid.ToString(), "agencyname") + "  ";
            }
            if (locationid > 0)
            {
                Message += " Location: " + mc.getNameByKey(EmployeeBLL.Instance.getLocationData(), "locationid", locationid.ToString(), "locationname") + "  ";
            }
            Message = Message.Substring(0, Message.Length - 2);
            return dtr;
        }
        //
        #endregion
        //
        internal DataSet getRptHeader(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_form_rptheader";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #region employee form report
        internal DataSet GetEmployeeTaxableReportHtml(int ccode, string finyear, int newempid, bool addsummary, double filterbyamt)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_taxable_report";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@addsummary", addsummary, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@filterbyamt", filterbyamt, DbType.Double));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getJoiningDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_joining_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPaymentDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_payment_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPersonalDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_personal_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getContactDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_contact_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getEmergencyDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_emergency_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getReferenceDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_reference_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBankDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_bank_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPFESIDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_pfesi_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getLeavingUpdationDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_leaving_updation_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getQualificationDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_qualification_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getExperienceDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_experience_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getFamilyDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_family_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getNomineeDetail(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_employee_nominee_detail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBirthdayAnniversaryReportHtml(int monthfrom, int monthto, string grade)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_birthday_anniversary_report";
            cmd.Parameters.Add(mc.getPObject("@monthfrom", monthfrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@monthto", monthto, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}