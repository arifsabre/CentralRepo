using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EmployeeMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "EMP Code")]
        public string EmpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "NewEMP Code")]
        public int NewEmpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Name")]
        public string EmpName { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Grade { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Joining Unit")]
        public int JoiningUnit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Working Unit")]
        public int WorkingUnit { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public string DepCode { get; set; }

        [Display(Name = "Job Description")]
        public string JobDesc { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Designation")]
        public int DesigId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Qualification")]
        public int QualId { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "HOD EMP Code")]
        public string HODEmpId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Group Joining Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime GroupJoiningDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        //[Range(3000, 10000000, ErrorMessage = "Basic must be between 3000 and 10000000")]
        [Display(Name = "Basic Rate")]
        public double BasicRate { get; set; }

        [Required(ErrorMessage = "Required!")]
        public double DA { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Conv.Allow.")]
        public double ConvAllowance { get; set; }

        [Required(ErrorMessage = "Required!")]
        public double HRA { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Med.Allow.")]
        public double MedicalAllowance { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Comp.Allow.")]
        public double CompAllowance { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "D.W. Allow.")]
        public double DWAllowance { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ESI Allow.")]
        public bool ESIApplicable { get; set; }

        [Display(Name = "ESI Number")]
        public string ESINumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PF Allow.")]
        public bool PFApplicable { get; set; }

        [Display(Name = "PF Number")]
        public string PFNumber { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Special Pay")]
        public double SpecialPay { get; set; }

        [Required(ErrorMessage = "Required!")]
        public double Others { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Gross Pay")]
        public double GrossSalary { get; set; }

        [Display(Name = "LIC Id")]
        public string LICId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Prev. Exp.")]
        public double PrevExp { get; set; }

        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Display(Name = "Spouse Name")]
        public string SpouseName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Spouse Age")]
        public int SpouseAge { get; set; }

        [Display(Name = "Corresp. Address")]
        public string CAddress { get; set; }

        [Display(Name = "Corresp. City")]
        public string CCity { get; set; }

        [Display(Name = "Permanent Address")]
        public string PAddress { get; set; }

        [Display(Name = "Permanent City")]
        public string PCity { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Gender { get; set; }

        [Display(Name = "Gender")]
        public string GenderName { get; set; }//d

        [Display(Name = "Emerg.Cont. Person1")]
        public string EmgContactPer { get; set; }

        [Display(Name = "Emerg. ContactNo1")]
        public string EmgContactNo { get; set; }

        [Display(Name = "Emerg.Cont. Person2")]
        public string EmgContactPer2 { get; set; }

        [Display(Name = "Emerg. ContactNo2")]
        public string EmgContactNo2 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Service Type")]
        public string ServiceType { get; set; }

        [Display(Name = "On-role Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime OnroleDate { get; set; }

        [Display(Name = "Leaving Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime LeavingDate { get; set; }

        [Display(Name = "Leaving Reason")]
        public string Reason { get; set; }

        public string Remarks { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Stop Deduction")]
        public bool StopDeduction { get; set; }

        [Display(Name = "Religion")]
        public string Religion { get; set; }

        [Display(Name = "Ident. Mark")]
        public string IdentMark { get; set; }

        [Display(Name = "Blood Group")]
        public string BGroup { get; set; }

        [Display(Name = "Father's Occupation")]
        public string FatherOccupation { get; set; }

        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Display(Name = "Mother's Occupation")]
        public string MotherOccupation { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatusName { get; set; }

        [Display(Name = "Landline No")]
        public string PhoneNo { get; set; }

        [Display(Name = "PAN No")]
        public string PanNo { get; set; }

        [Display(Name = "Aadhar No")]
        public string AadharNo { get; set; }

        [Display(Name = "Voter ID No")]
        public string VoterIdNo { get; set; }

        [Display(Name = "Passport No")]
        public string PassportNo { get; set; }

        [Display(Name = "D.L. No")]
        public string DrvLicNo { get; set; }

        [Display(Name = "Anniv. Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AnnivDate { get; set; }

        [Display(Name = "Spouse Occupation")]
        public string SpouseOccupation { get; set; }

        [Display(Name = "Spouse Qual.")]
        public string SpouseQual { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Branch Address")]
        public string BranchAddress { get; set; }

        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        public string NameOnPassbook { get; set; }

        [Display(Name = "IFSC Code")]
        public string IfscCode { get; set; }

        [Display(Name = "MICR Code")]
        public string MicrCode { get; set; }

        [Display(Name = "PF Nominee")]
        public string PFNominee { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "PF Nominee Age")]
        public int PFNomineeAge { get; set; }

        [Display(Name = "PF Nominee Relation")]
        public string PFNomineeRelation { get; set; }

        [Display(Name = "ESI Nominee")]
        public string ESINominee { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "ESI Nominee Age")]
        public int ESINomineeAge { get; set; }

        [Display(Name = "ESI Nominee Relation")]
        public string ESINomineeRelation { get; set; }

        [Display(Name = "UAN")]
        public string UAN { get; set; }

        [Display(Name = "Ref. Name1")]
        public string RefName1 { get; set; }

        [Display(Name = "Ref. ContactNo1")]
        public string RefContactNo1 { get; set; }

        [Display(Name = "Ref. Address1")]
        public string RefAddress1 { get; set; }

        [Display(Name = "Ref. Name2")]
        public string RefName2 { get; set; }

        [Display(Name = "Ref. ContactNo2")]
        public string RefContactNo2 { get; set; }

        [Display(Name = "Ref. Address2")]
        public string RefAddress2 { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Appl. Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AppDate { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        public int ReportingTo { get; set; }

        [Display(Name = "Agency")]
        public int AgencyId { get; set; }

        [Display(Name = "Agency")]
        public string AgencyName { get; set; }//d


        #region additional fields

        [Display(Name = "HOD Name")]
        public string HODEmpName { get; set; }
        
        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        [Display(Name = "Joining Unit")]
        public string JoiningUnitName { get; set; }

        [Display(Name = "Working Unit")]
        public string WorkingUnitName { get; set; }

        [Display(Name = "Department")]
        public string DepName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public string ServiceTypeName { get; set; }

        public string Designation { get; set; }

        public string Qualification { get; set; }

        public int Age { get; set; }

        [Display(Name = "Exp. At Prag")]
        public double PragExp { get; set; }

        [Display(Name = "Total Experience")]
        public double TotalExp { get; set; }
        
        //in case of transfer only
        [Display(Name = "Trans EmpId")]
        public int TransEmpId { get; set; }

        [Display(Name = "Trans EmpCode")]
        public string TransEmpCode { get; set; }

        [Display(Name = "Increment Amount")]
        public double IncAmount { get; set; }

        [Display(Name = "Village")]
        public string VillageName { get; set; }

        [Display(Name = "Police Station")]
        public string PoliceStation { get; set; }

        [Display(Name = "Sub-Division")]
        public string SubDivision { get; set; }

        [Display(Name = "Post Office")]
        public string PostOffice { get; set; }

        [Display(Name = "District")]
        public string District { get; set; }//not in use

        [Display(Name = "State")]
        public string StateName { get; set; }

        [Display(Name = "Caste")]
        public int CasteId { get; set; }

        [Display(Name = "Caste")]
        public string CasteName { get; set; }//d

        [Display(Name = "Extn.No")]
        public string PhoneExtNo { get; set; }

        [Display(Name = "DocUploaded")]
        public bool Doc { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "TDS Deduction")]
        public double TDSDeduction { get; set; }

        [Display(Name = "Referred By")]
        public string ReferredBy { get; set; }

        [Display(Name = "Updation Log")]
        public string UpdationLog { get; set; }

        [Display(Name = "Put 1 Here")]
        public int SendSMS { get; set; }

        [Display(Name = "Updation Log")]
        public string hdnUpdationLog { get; set; }//chk

        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Contract Valid Upto")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ContValidUpto { get; set; }

        public List<EmployeeMdl> GetempList { get; set; }

        public List<ExperienceDetailMdl> ExpDetail { get; set; }
        public List<QualDetailMdl> QualDetail { get; set; }
        public List<FamilyDetailMdl> FamilyDetail { get; set; }
        public List<PFNomineeMdl> PFNomineeDetail { get; set; }
        public List<EmployeeMdl> Item_List { get; set; }
        public DateTime PresentDate { get; set; }//chk
        public DateTime AbsentDate { get; set; }//chk
        public DateTime EDate { get; set; }//chk
        public string ETime { get; set; }//chk
        public int BirthdayAfter { get; set; }//chk
        public DateTime Retirementdate { get; set; }//chk
        public int retiredindays { get; set; }//chk

        public string cmpname { get; set; }//chk
        public bool SendSMSLate { get; set; }
        public bool SendSMSAbsent { get; set; }
        public bool SendSMSMissing { get; set; }

        public DateTime SendDate { get; set; }//chk
        public string MobileNo { get; set; }//chk

        #endregion

    }
}