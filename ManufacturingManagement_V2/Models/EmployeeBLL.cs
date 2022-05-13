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
    public class EmployeeBLL : DbContext
    {
        //
        //internal DbSet<EmployeeMdl> Employees { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static EmployeeBLL Instance
        {
            get { return new EmployeeBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, EmployeeMdl dbobject)
        {
            //cmd.Parameters.Add(mc.getPObject("@WorkingUnit", dbobject.WorkingUnit, DbType.Int16));
            //cmd.Parameters.Add(mc.getPObject("@HODEmpId", dbobject.HODEmpId.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@ServiceType", dbobject.ServiceType, DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@OnroleDate", dbobject.OnroleDate.ToShortDateString(), DbType.DateTime));
            //cmd.Parameters.Add(mc.getPObject("@StopDeduction", dbobject.StopDeduction, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmpId", dbobject.EmpId.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Title", dbobject.Title.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmpName", dbobject.EmpName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Grade", dbobject.Grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@JoiningUnit", dbobject.JoiningUnit, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@DepCode", dbobject.DepCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@JobDesc", dbobject.JobDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CategoryId", dbobject.CategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DesigId", dbobject.DesigId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@QualId", dbobject.QualId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@JoiningDate", dbobject.JoiningDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@BasicRate", dbobject.BasicRate, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DA", dbobject.DA, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ConvAllowance", dbobject.ConvAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@HRA", dbobject.HRA, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@MedicalAllowance", dbobject.MedicalAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompAllowance", dbobject.CompAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@DWAllowance", dbobject.DWAllowance, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ESIApplicable", dbobject.ESIApplicable, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ESINumber", dbobject.ESINumber.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFApplicable", dbobject.PFApplicable, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFNumber", dbobject.PFNumber.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SpecialPay", dbobject.SpecialPay, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Others", dbobject.Others, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@GrossSalary", dbobject.GrossSalary, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@LICId", dbobject.LICId, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PrevExp", dbobject.PrevExp, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@FatherName", dbobject.FatherName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SpouseName", dbobject.SpouseName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SpouseAge", dbobject.SpouseAge, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CAddress", dbobject.CAddress.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CCity", dbobject.CCity.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PAddress", dbobject.PAddress.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PCity", dbobject.PCity.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ContactNo", dbobject.ContactNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BirthDate", dbobject.BirthDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Gender", dbobject.Gender, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmgContactNo", dbobject.EmgContactNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmgContactPer", dbobject.EmgContactPer.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LeavingDate", dbobject.LeavingDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Reason", dbobject.Reason.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsActive", dbobject.IsActive, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DeductionAmt", "0", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@TotalDeduction", "0", DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@Religion", dbobject.Religion.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IdentMark", dbobject.IdentMark.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BGroup", dbobject.BGroup.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FatherOccupation", dbobject.FatherOccupation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MotherName", dbobject.MotherName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MotherOccupation", dbobject.MotherOccupation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MaritalStatus", dbobject.MaritalStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PhoneNo", dbobject.PhoneNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PanNo", dbobject.PanNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AadharNo", dbobject.AadharNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VoterIdNo", dbobject.VoterIdNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PassportNo", dbobject.PassportNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DrvLicNo", dbobject.DrvLicNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AnnivDate", dbobject.AnnivDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@SpouseOccupation", dbobject.SpouseOccupation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SpouseQual", dbobject.SpouseQual.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BankName", dbobject.BankName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BranchAddress", dbobject.BranchAddress.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AccountNo", dbobject.AccountNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NameOnPassbook", dbobject.NameOnPassbook.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IfscCode", dbobject.IfscCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MicrCode", dbobject.MicrCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFNominee", dbobject.PFNominee.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PFNomineeAge", dbobject.PFNomineeAge, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PFNomineeRelation", dbobject.PFNomineeRelation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ESINominee", dbobject.ESINominee.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ESINomineeAge", dbobject.ESINomineeAge, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ESINomineeRelation", dbobject.ESINomineeRelation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UAN", dbobject.UAN.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefName1", dbobject.RefName1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefContactNo1", dbobject.RefContactNo1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefAddress1", dbobject.RefAddress1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefName2", dbobject.RefName2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefContactNo2", dbobject.RefContactNo2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RefAddress2", dbobject.RefAddress2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmgContactNo2", dbobject.EmgContactNo2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmgContactPer2", dbobject.EmgContactPer2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AppDate", dbobject.AppDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Email", dbobject.Email.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IncAmount", dbobject.IncAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VillageName", dbobject.VillageName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PoliceStation", dbobject.PoliceStation.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SubDivision", dbobject.SubDivision.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PostOffice", dbobject.PostOffice.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@District", dbobject.District.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateName", dbobject.StateName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PhoneExtNo", dbobject.PhoneExtNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TDSDeduction", dbobject.TDSDeduction, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ReferredBy", dbobject.ReferredBy.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UpdationLog", dbobject.UpdationLog.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TransferDate", dbobject.TransferDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CasteId", dbobject.CasteId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@GroupJoiningDate", dbobject.GroupJoiningDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@LocationId", dbobject.LocationId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ContValidUpto", dbobject.ContValidUpto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@AgencyId", dbobject.AgencyId, DbType.Int32));
        }
        //
        internal void setAdditionalColumns(DataSet ds)
        {
            if (ds.Tables.Count == 0) { return; };
            //add columns
            ds.Tables[0].Columns.Add("GradeName");
            ds.Tables[0].Columns.Add("CategoryName");
            ds.Tables[0].Columns.Add("Designation");
            ds.Tables[0].Columns.Add("GenderName");
            ds.Tables[0].Columns.Add("MaritalStatusName");
            ds.Tables[0].Columns.Add("DepName");
            ds.Tables[0].Columns.Add("ServiceTypeName");
            ds.Tables[0].Columns.Add("TotalExp");
            //set column values in ds
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ds.Tables[0].Rows[i]["GradeName"] = mc.getNameByKey(mc.getGrades(), "grade", ds.Tables[0].Rows[i]["Grade"].ToString(), "gradename");
                ds.Tables[0].Rows[i]["CategoryName"] = mc.getNameByKey(mc.getEmpCategory(), "categoryid", ds.Tables[0].Rows[i]["categoryId"].ToString(), "categoryname");
                ds.Tables[0].Rows[i]["Designation"] = mc.getNameByKey(mc.getDesignation(), "desigid", ds.Tables[0].Rows[i]["DesigId"].ToString(), "designation");
                ds.Tables[0].Rows[i]["GenderName"] = mc.getNameByKey(mc.getGenders(), "gender", ds.Tables[0].Rows[i]["gender"].ToString(), "gendername");
                ds.Tables[0].Rows[i]["MaritalStatusName"] = mc.getNameByKey(mc.getMaritalStatus(), "mstatus", ds.Tables[0].Rows[i]["maritalstatus"].ToString(), "mstatusname");
                ds.Tables[0].Rows[i]["ServiceTypeName"] = mc.getNameByKey(mc.getServiceTypes(), "servicetype", ds.Tables[0].Rows[i]["servicetype"].ToString(), "servicetypename");
                ds.Tables[0].Rows[i]["TotalExp"] = Convert.ToDouble(ds.Tables[0].Rows[i]["PragExp"].ToString()) + Convert.ToDouble(ds.Tables[0].Rows[i]["PrevExp"].ToString());
            }
        }
        //
        private List<EmployeeMdl> createObjectList(DataSet ds)
        {
            setAdditionalColumns(ds);
            List<EmployeeMdl> employees = new List<EmployeeMdl> { };
            //DataTable dtr = ds.Tables[0].DefaultView.ToTable();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeMdl objmdl = new EmployeeMdl();
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();
                if (dr.Table.Columns.Contains("Title"))
                {
                    objmdl.Title = dr["Title"].ToString();
                }
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.Grade = dr["Grade"].ToString();
                objmdl.JoiningUnit = Convert.ToInt32(dr["JoiningUnit"].ToString());
                objmdl.WorkingUnit = Convert.ToInt32(dr["WorkingUnit"].ToString());
                objmdl.DepCode = dr["DepCode"].ToString();
                objmdl.JobDesc = dr["JobDesc"].ToString();
                objmdl.CategoryId = Convert.ToInt32(dr["CategoryId"].ToString());
                objmdl.DesigId = Convert.ToInt32(dr["DesigId"].ToString());
                objmdl.QualId = Convert.ToInt32(dr["QualId"].ToString());
                objmdl.HODEmpId = dr["HODEmpId"].ToString();
                objmdl.JoiningDate = Convert.ToDateTime(dr["JoiningDate"].ToString());
                objmdl.BasicRate = Convert.ToDouble(dr["BasicRate"].ToString());
                objmdl.DA = Convert.ToDouble(dr["DA"].ToString());
                objmdl.ConvAllowance = Convert.ToDouble(dr["ConvAllowance"].ToString());
                objmdl.HRA = Convert.ToDouble(dr["HRA"].ToString());
                objmdl.MedicalAllowance = Convert.ToDouble(dr["MedicalAllowance"].ToString());
                objmdl.CompAllowance = Convert.ToDouble(dr["CompAllowance"].ToString());
                objmdl.DWAllowance = Convert.ToDouble(dr["DWAllowance"].ToString());
                objmdl.ESIApplicable = Convert.ToBoolean(dr["ESIApplicable"].ToString());
                objmdl.ESINumber = dr["ESINumber"].ToString();
                objmdl.PFApplicable = Convert.ToBoolean(dr["PFApplicable"].ToString());
                objmdl.PFNumber = dr["PFNumber"].ToString();
                objmdl.SpecialPay = Convert.ToDouble(dr["SpecialPay"].ToString());
                objmdl.Others = Convert.ToDouble(dr["Others"].ToString());
                objmdl.GrossSalary = Convert.ToDouble(dr["GrossSalary"].ToString());
                objmdl.LICId = dr["LICId"].ToString();
                objmdl.PrevExp = Convert.ToDouble(dr["PrevExp"].ToString());
                objmdl.FatherName = dr["FatherName"].ToString();
                objmdl.SpouseName = dr["SpouseName"].ToString();
                objmdl.SpouseAge = Convert.ToInt32(dr["SpouseAge"].ToString());
                objmdl.CAddress = dr["CAddress"].ToString();
                objmdl.PAddress = dr["PAddress"].ToString();
                objmdl.ContactNo = dr["ContactNo"].ToString();
                objmdl.BirthDate = Convert.ToDateTime(dr["BirthDate"].ToString());
                objmdl.Gender = dr["Gender"].ToString();
                objmdl.EmgContactNo = dr["EmgContactNo"].ToString();
                objmdl.EmgContactPer = dr["EmgContactPer"].ToString();
                objmdl.ServiceType = dr["ServiceType"].ToString();
                objmdl.OnroleDate = Convert.ToDateTime(dr["OnroleDate"].ToString());
                objmdl.LeavingDate = Convert.ToDateTime(dr["LeavingDate"].ToString());
                objmdl.Reason = dr["Reason"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                objmdl.CCity = dr["CCity"].ToString();
                objmdl.PCity = dr["PCity"].ToString();
                objmdl.StopDeduction = Convert.ToBoolean(dr["StopDeduction"].ToString());
                objmdl.Religion = dr["Religion"].ToString();
                objmdl.IdentMark = dr["IdentMark"].ToString();
                objmdl.BGroup = dr["BGroup"].ToString();
                objmdl.FatherOccupation = dr["FatherOccupation"].ToString();
                objmdl.MotherName = dr["MotherName"].ToString();
                objmdl.MotherOccupation = dr["MotherOccupation"].ToString();
                objmdl.MaritalStatus = dr["MaritalStatus"].ToString();
                objmdl.PhoneNo = dr["PhoneNo"].ToString();
                objmdl.PanNo = dr["PanNo"].ToString();
                objmdl.AadharNo = dr["AadharNo"].ToString();
                objmdl.VoterIdNo = dr["VoterIdNo"].ToString();
                objmdl.PassportNo = dr["PassportNo"].ToString();
                objmdl.DrvLicNo = dr["DrvLicNo"].ToString();
                objmdl.AnnivDate = Convert.ToDateTime(dr["AnnivDate"].ToString());
                objmdl.SpouseOccupation = dr["SpouseOccupation"].ToString();
                objmdl.SpouseQual = dr["SpouseQual"].ToString();
                objmdl.BankName = dr["BankName"].ToString();
                objmdl.BranchAddress = dr["BranchAddress"].ToString();
                objmdl.AccountNo = dr["AccountNo"].ToString();
                objmdl.NameOnPassbook = dr["NameOnPassbook"].ToString();
                objmdl.IfscCode = dr["IfscCode"].ToString();
                objmdl.MicrCode = dr["MicrCode"].ToString();
                objmdl.PFNominee = dr["PFNominee"].ToString();
                objmdl.PFNomineeAge = Convert.ToInt32(dr["PFNomineeAge"].ToString());
                objmdl.PFNomineeRelation = dr["PFNomineeRelation"].ToString();
                objmdl.ESINominee = dr["ESINominee"].ToString();
                objmdl.ESINomineeAge = Convert.ToInt32(dr["ESINomineeAge"].ToString());
                objmdl.ESINomineeRelation = dr["ESINomineeRelation"].ToString();
                objmdl.UAN = dr["UAN"].ToString();
                objmdl.RefName1 = dr["RefName1"].ToString();
                objmdl.RefContactNo1 = dr["RefContactNo1"].ToString();
                objmdl.RefAddress1 = dr["RefAddress1"].ToString();
                objmdl.RefName2 = dr["RefName2"].ToString();
                objmdl.RefContactNo2 = dr["RefContactNo2"].ToString();
                objmdl.RefAddress2 = dr["RefAddress2"].ToString();
                objmdl.EmgContactNo2 = dr["EmgContactNo2"].ToString();
                objmdl.EmgContactPer2 = dr["EmgContactPer2"].ToString();
                objmdl.AppDate = Convert.ToDateTime(dr["AppDate"].ToString());
                objmdl.Email = dr["Email"].ToString();
                objmdl.IncAmount = Convert.ToDouble(dr["IncAmount"].ToString());
                if (dr.Table.Columns.Contains("Doc"))
                {
                    objmdl.Doc = Convert.ToBoolean(dr["Doc"].ToString());
                }
                //additional columns
                objmdl.HODEmpName = dr["HODEmpName"].ToString();
                objmdl.GradeName = dr["GradeName"].ToString();
                objmdl.JoiningUnitName = dr["JoiningUnitName"].ToString();
                objmdl.WorkingUnitName = dr["WorkingUnitName"].ToString();
                objmdl.DepName = dr["Department"].ToString();
                objmdl.CategoryName = dr["CategoryName"].ToString();
                objmdl.Designation = dr["Designation"].ToString();
                objmdl.Qualification = dr["Qualification"].ToString();
                objmdl.ServiceTypeName = dr["ServiceTypeName"].ToString();
                objmdl.GenderName = dr["GenderName"].ToString();
                objmdl.MaritalStatusName = dr["MaritalStatusName"].ToString();
                objmdl.Age = Convert.ToInt32(dr["Age"].ToString());
                objmdl.PragExp = Convert.ToDouble(dr["PragExp"].ToString());
                objmdl.TotalExp = Convert.ToDouble(dr["TotalExp"].ToString());
                objmdl.VillageName = dr["VillageName"].ToString();
                objmdl.PoliceStation = dr["PoliceStation"].ToString();
                objmdl.SubDivision = dr["SubDivision"].ToString();
                objmdl.PostOffice = dr["PostOffice"].ToString();
                objmdl.District = dr["District"].ToString();
                objmdl.StateName = dr["StateName"].ToString();
                objmdl.PhoneExtNo = dr["PhoneExtNo"].ToString();
                objmdl.TDSDeduction = Convert.ToDouble(dr["TDSDeduction"].ToString());
                objmdl.ReferredBy = dr["ReferredBy"].ToString();
                objmdl.UpdationLog = dr["UpdationLog"].ToString();
                objmdl.TransferDate = Convert.ToDateTime(dr["TransferDate"].ToString());
                objmdl.CasteId = Convert.ToInt32(dr["CasteId"].ToString());
                objmdl.CasteName = dr["CasteName"].ToString();
                objmdl.GroupJoiningDate = Convert.ToDateTime(dr["GroupJoiningDate"].ToString());
                if (dr.Table.Columns.Contains("LocationId"))
                {
                    objmdl.LocationId = Convert.ToInt32(dr["LocationId"].ToString());
                }
                if (dr.Table.Columns.Contains("LocationName"))
                {
                    objmdl.LocationName = dr["LocationName"].ToString();
                }
                if (dr.Table.Columns.Contains("ContValidUpto"))
                {
                    objmdl.ContValidUpto = Convert.ToDateTime(dr["ContValidUpto"].ToString());
                }
                if (dr.Table.Columns.Contains("AgencyId"))
                {
                    objmdl.AgencyId = Convert.ToInt32(dr["AgencyId"].ToString());
                }
                if (dr.Table.Columns.Contains("AgencyName"))
                {
                    objmdl.AgencyName = dr["AgencyName"].ToString();
                }
                employees.Add(objmdl);
            }
            return employees;
        }
        //
        internal bool isFoundEmpCode(string empid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_employee";
            cmd.Parameters.Add(mc.getPObject("@empid", empid, DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        #endregion
        //
        #region dml objects
        //
        private bool checkSetValidModel(EmployeeMdl dbobject)
        {
            Message = "";
            if (dbobject.EmpId == null)
            {
                dbobject.EmpId = "";//note
            }
            if (dbobject.HODEmpId == null)
            {
                dbobject.HODEmpId = "";//note
            }
            if (!(dbobject.CategoryId==1 || dbobject.CategoryId==2 || dbobject.CategoryId==3))
            {
                Message = "Invalid category selected!";
                return false;
            }
            if (dbobject.EmpId.Length > 0)
            {
                if (dbobject.EmpId.Contains('_') == false)
                {
                    Message = "Invalid EmpId pattern!";
                    return false;
                }
                string x1 = dbobject.EmpId.Split('_')[1];
                if (x1.Length < 2)
                {
                    Message = "Invalid EmpId pattern!";
                    return false;
                }
                string xs = x1.Substring(0, 1);
                if (mc.IsValidInteger(xs) == true)
                {
                    Message = "Invalid EmpId pattern!";
                    return false;
                }
                string x2 = x1.Substring(1, x1.Length-1);
                if (mc.IsValidInteger(x2) == false)
                {
                    Message = "Invalid EmpId pattern!";
                    return false;
                }
            }
            if (mc.isValidDate(dbobject.JoiningDate) == false)
            {
                Message = "Invalid joining date";
                return false;
            }
            if (mc.isValidDate(dbobject.TransferDate) == false)
            {
                Message = "Invalid transfer date";
                return false;
            }
            if (mc.isValidDate(dbobject.GroupJoiningDate) == false)
            {
                Message = "Invalid group joining date";
                return false;
            }
            if (mc.isValidDate(dbobject.AppDate) == false)
            {
                dbobject.AppDate = dbobject.JoiningDate;
            }
            if (dbobject.CategoryId == 0)
            {
                Message = "Invalid category selected!";
                return false;
            }
            if (dbobject.UpdationLog == null)
            {
                dbobject.UpdationLog = "";
            }
            DateTime dtm1 = new DateTime(1900,1,1);
            DateTime dtm2 = new DateTime(1, 1, 1);
            if (dbobject.IsActive == true && !(dbobject.LeavingDate == dtm1 || dbobject.LeavingDate == dtm2))
            {
                Message = "Leaving date has been entered. Employee cannot be set to active!";
                return false;
            }
            dbobject.OnroleDate = mc.setToValidOptionalDate(dbobject.OnroleDate);
            dbobject.LeavingDate = mc.setToValidOptionalDate(dbobject.LeavingDate);
            dbobject.AnnivDate = mc.setToValidOptionalDate(dbobject.AnnivDate);
            dbobject.BirthDate = mc.setToValidOptionalDate(dbobject.BirthDate);
            //provision for null values
            if (dbobject.ESINumber == null)
            {
                dbobject.ESINumber = "";
            }
            if (dbobject.PFNumber == null)
            {
                dbobject.PFNumber = "";
            }
            if (dbobject.LICId == null)
            {
                dbobject.LICId = "";
            }
            if (dbobject.FatherName == null)
            {
                dbobject.FatherName = "";
            }
            if (dbobject.SpouseName == null)
            {
                dbobject.SpouseName = "";
            }
            if (dbobject.CAddress == null)
            {
                dbobject.CAddress = "";
            }
            if (dbobject.PAddress == null)
            {
                dbobject.PAddress = "";
            }
            if (dbobject.ContactNo == null)
            {
                dbobject.ContactNo = "";
            }
            if (dbobject.EmgContactNo == null)
            {
                dbobject.EmgContactNo = "";
            }
            if (dbobject.EmgContactPer == null)
            {
                dbobject.EmgContactPer = "";
            }
            if (dbobject.Reason == null)
            {
                dbobject.Reason = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.CCity == null)
            {
                dbobject.CCity = "";
            }
            if (dbobject.PCity == null)
            {
                dbobject.PCity = "";
            }
            if (dbobject.DepCode == null)
            {
                dbobject.DepCode = "";
            }
            if (dbobject.JobDesc == null)
            {
                dbobject.JobDesc = "";
            }
            if (dbobject.Religion == null)
            {
                dbobject.Religion = "";
            }
            if (dbobject.IdentMark == null)
            {
                dbobject.IdentMark = "";
            }
            if (dbobject.BGroup == null)
            {
                dbobject.BGroup = "";
            }
            if (dbobject.FatherOccupation == null)
            {
                dbobject.FatherOccupation = "";
            }
            if (dbobject.MotherName == null)
            {
                dbobject.MotherName = "";
            }
            if (dbobject.MotherOccupation == null)
            {
                dbobject.MotherOccupation = "";
            }
            if (dbobject.MaritalStatus == null)
            {
                dbobject.MaritalStatus = "";
            }
            if (dbobject.PhoneNo == null)
            {
                dbobject.PhoneNo = "";
            }
            if (dbobject.PanNo == null)
            {
                dbobject.PanNo = "";
            }
            if (dbobject.AadharNo == null)
            {
                dbobject.AadharNo = "";
            }
            if (dbobject.VoterIdNo == null)
            {
                dbobject.VoterIdNo = "";
            }
            if (dbobject.PassportNo == null)
            {
                dbobject.PassportNo = "";
            }
            if (dbobject.DrvLicNo == null)
            {
                dbobject.DrvLicNo = "";
            }
            if (dbobject.SpouseOccupation == null)
            {
                dbobject.SpouseOccupation = "";
            }
            if (dbobject.SpouseQual == null)
            {
                dbobject.SpouseQual = "";
            }
            if (dbobject.BankName == null)
            {
                dbobject.BankName = "";
            }
            if (dbobject.BranchAddress == null)
            {
                dbobject.BranchAddress = "";
            }
            if (dbobject.NameOnPassbook == null)
            {
                dbobject.NameOnPassbook = "";
            }
            if (dbobject.IfscCode == null)
            {
                dbobject.IfscCode = "";
            }
            if (dbobject.IfscCode.ToString().Length > 0)
            {
                if (clsValidateInput.IsValidIFSCode(dbobject.IfscCode) == false)
                {
                    Message = "Invalid IFS Code entered!";
                    return false;
                }
            }
            if (dbobject.MicrCode == null)
            {
                dbobject.MicrCode = "";
            }
            if (dbobject.PFNominee == null)
            {
                dbobject.PFNominee = "";
            }
            if (dbobject.PFNomineeRelation == null)
            {
                dbobject.PFNomineeRelation = "";
            }
            if (dbobject.ESINominee == null)
            {
                dbobject.ESINominee = "";
            }
            if (dbobject.ESINomineeRelation == null)
            {
                dbobject.ESINomineeRelation = "";
            }
            if (dbobject.UAN == null)
            {
                dbobject.UAN = "";
            }
            if (dbobject.RefName1 == null)
            {
                dbobject.RefName1 = "";
            }
            if (dbobject.RefContactNo1 == null)
            {
                dbobject.RefContactNo1 = "";
            }
            if (dbobject.RefAddress1 == null)
            {
                dbobject.RefAddress1 = "";
            }
            if (dbobject.RefName2 == null)
            {
                dbobject.RefName2 = "";
            }
            if (dbobject.RefContactNo2 == null)
            {
                dbobject.RefContactNo2 = "";
            }
            if (dbobject.RefAddress2 == null)
            {
                dbobject.RefAddress2 = "";
            }
            if (dbobject.EmgContactNo2 == null)
            {
                dbobject.EmgContactNo2 = "";
            }
            if (dbobject.EmgContactPer2 == null)
            {
                dbobject.EmgContactPer2 = "";
            }
            if (dbobject.AccountNo == null)
            {
                dbobject.AccountNo = "";
            }
            if (dbobject.Email == null)
            {
                dbobject.Email = "";
            }
            if (dbobject.VillageName == null)
            {
                dbobject.VillageName = "";
            }
            if (dbobject.PoliceStation == null)
            {
                dbobject.PoliceStation = "";
            }
            if (dbobject.SubDivision == null)
            {
                dbobject.SubDivision = "";
            }
            if (dbobject.PostOffice == null)
            {
                dbobject.PostOffice = "";
            }
            if (dbobject.District == null)
            {
                dbobject.District = "";
            }
            if (dbobject.StateName == null)
            {
                dbobject.StateName = "";
            }
            if (dbobject.PhoneExtNo == null)
            {
                dbobject.PhoneExtNo = "";
            }
            if (dbobject.ReferredBy == null)
            {
                dbobject.ReferredBy = "";
            }
            return true;
        }
        //
        internal void insertObject(EmployeeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (mc.getPermission(Models.Entry.DirectorsInfo, permissionType.Add) == false && dbobject.Grade == "d")
            {
                dbobject.Reason = "Unauthorised access!";
                return;
            }
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
                cmd.CommandText = "usp_insert_tbl_employee";//with account
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string newempid = mc.getRecentIdentityValue(cmd, dbTables.tbl_employee, "newempid");
                mc.setEventLog(cmd, dbTables.tbl_employee, newempid, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("EmployeeBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateEmployeeDocument(int newempid, bool doc)
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
                cmd.CommandText = "usp_update_employee_document";
                cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@doc", doc, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, newempid.ToString(), "Document Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("EmployeeBLL", "updateEmployeeDocument", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(EmployeeMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (dbobject.UpdationLog.Length == 0)
            {
                Message = "Updation Log not entered!";
                return;
            }
            if (dbobject.hdnUpdationLog == null)
            {
                dbobject.hdnUpdationLog = "";
            }
            if (dbobject.UpdationLog.Trim().ToLower() == dbobject.hdnUpdationLog.Trim().ToLower())
            {
                Message = "Updation Log not changed!";
                return;
            }
            //
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
                cmd.CommandText = "usp_update_tbl_employee";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.NewEmpId, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, dbobject.NewEmpId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("pk_tbl_employee") == true)
                {
                    Message = "Duplicate EMP Code not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("EmployeeBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateDirectory(EmployeeMdl dbobject)
        {
            Result = false;
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return;
            }
            if (dbobject.PhoneExtNo == null)
            {
                dbobject.PhoneExtNo = "";
            }
            if (dbobject.ContactNo == null)
            {
                dbobject.ContactNo = "";
            }
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
                cmd.CommandText = "usp_update_employee_directory";
                cmd.Parameters.Add(mc.getPObject("@ContactNo", dbobject.ContactNo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@PhoneExtNo", dbobject.PhoneExtNo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@newempid", dbobject.NewEmpId, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, dbobject.NewEmpId.ToString(), "Directory Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("EmployeeBLL", "updateDirectory", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int id)
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
                //set by procedures by admin control
                //cmd.CommandText = "usp_delete_tbl_employee";
                cmd.Parameters.Add(mc.getPObject("@newempid", id, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, id.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                Message = "This record cannot be deleted!";
                mc.setErrorLog("EmployeeBLL", "deleteObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateStatusSMS(EmployeeMdl empmdl)
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
                cmd.CommandText = "ZZZ_UpDate_SMS_Status";
                cmd.Parameters.Add(mc.getPObject("@newempid", empmdl.NewEmpId, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@sendsms", empmdl.SendSMS, DbType.String));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_employee, empmdl.NewEmpId.ToString(), "SMS Status Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                mc.setErrorLog("EmployeeBLL", "updateStatusSMS", ex.Message);
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
        internal EmployeeMdl searchObject(string id)
        {
            DataSet ds = new DataSet();
            EmployeeMdl dbobject = new EmployeeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_employee";
            cmd.Parameters.Add(mc.getPObject("@empid", id, DbType.String));
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
        internal EmployeeMdl searchEmployeeByNewEmpId(int newempid)
        {
            DataSet ds = new DataSet();
            EmployeeMdl dbobject = new EmployeeMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_employee_newempid";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.String));
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
        internal DataSet getObjectData(string grade="", int joiningunit=0)
        {
            //[100088]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_employee";
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@joiningunit", joiningunit, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EmployeeMdl> getObjectList(string grade, int joiningunit)
        {
            DataSet ds = getObjectData(grade,joiningunit);
            return createObjectList(ds);
        }
        //
        internal DataSet getEmployeeSearchList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_search_list";
            mc.fillFromDatabase(ds, cmd);
            //List<EmployeeMdl> employees = new List<EmployeeMdl> { };
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    EmployeeMdl objmdl = new EmployeeMdl();
            //    objmdl.EmpId = dr["EmpId"].ToString();
            //    objmdl.EmpName = dr["EmpName"].ToString();
            //    employees.Add(objmdl);
            //}
            //return employees;
            return ds;
        }
        //
        internal DataSet getEmployeeSearchListNew()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_search_list_new";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getEmployeeSearchListInactive()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_search_list_inactive";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal string getEmployeeGrade(string empid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_grade";
            cmd.Parameters.Add(mc.getPObject("@empid", empid, DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //
        internal string getEmployeeGradeNewEmpId(int newempid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_grade_newempid";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        /// <summary>
        /// Employee --not worker grade
        /// </summary>
        /// <returns></returns>
        internal DataSet getStaffList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_staff_list";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<EmployeeMdl> getDirectoryList(string grade)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_directory";
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            List<EmployeeMdl> employees = new List<EmployeeMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EmployeeMdl objmdl = new EmployeeMdl();
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpName = dr["EmpName"].ToString();
                objmdl.ContactNo = dr["ContactNo"].ToString();
                objmdl.PhoneExtNo = dr["PhoneExtNo"].ToString();
                employees.Add(objmdl);
            }
            return employees;
        }
        //
        internal EmployeeMdl getDirectoryInfo(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_directory_info";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            EmployeeMdl employee = new EmployeeMdl();
            employee.NewEmpId = Convert.ToInt32(ds.Tables[0].Rows[0]["NewEmpId"].ToString());
            employee.EmpName = ds.Tables[0].Rows[0]["EmpName"].ToString();
            employee.ContactNo = ds.Tables[0].Rows[0]["ContactNo"].ToString();
            employee.PhoneExtNo = ds.Tables[0].Rows[0]["PhoneExtNo"].ToString();
            return employee;
        }
        //
        internal int getEmpLastNo(string grade,int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_emp_lastno";
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@JoiningUnit", compcode, DbType.Int16));
            return Convert.ToInt32(mc.getFromDatabase(cmd));
        }
        //
        public List<EmployeeMdl> Get_UpdateList()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<EmployeeMdl> HList = new List<EmployeeMdl>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_GetEmpStatus]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new EmployeeMdl
                    {
                        NewEmpId = Convert.ToInt32(rdr["NewEmpId"]),
                        // By_User = Convert.ToInt32(rdr["By_User"]),
                        EmpName = rdr["EmpName"].ToString(),
                        SendSMS = Convert.ToInt32(rdr["SendSMS"]),
                    });
                }
                return HList;
            }
        }
        //
        internal DataSet getEmployeeBirthdayData(int monthfrom, int monthto, string grade)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_birthday_report";
            cmd.Parameters.Add(mc.getPObject("@monthfrom", monthfrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@monthto", monthto, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getEmployeeAnniversaryData(int monthfrom, int monthto, string grade)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_anniversary_report";
            cmd.Parameters.Add(mc.getPObject("@monthfrom", monthfrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@monthto", monthto, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@grade", grade, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getEmployeeCasteData()
        {
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_casteList";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getCasteList()
        {
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            DataSet ds = getEmployeeCasteData();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectId = Convert.ToInt32(dr["CasteId"].ToString());
                objmdl.ObjectName = dr["CasteName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getEmployeeGradeData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_employeegrade";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getEmployeeGradeList()
        {
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            DataSet ds = getEmployeeGradeData();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectCode = dr["GradeCode"].ToString();
                objmdl.ObjectName = dr["GradeName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getLocationData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_location";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getLocationList()
        {
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            DataSet ds = getLocationData();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectId = Convert.ToInt32(dr["LocationId"].ToString());
                objmdl.ObjectName = dr["LocationName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getAgencyData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_agency";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getAgencyList()
        {
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            DataSet ds = getAgencyData();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectId = Convert.ToInt32(dr["AgencyId"].ToString());
                objmdl.ObjectName = dr["AgencyName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}