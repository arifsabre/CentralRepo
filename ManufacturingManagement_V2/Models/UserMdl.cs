using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingManagement_V2.Models
{
    public class UserMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required!")]
        //[StringLength(10, MinimumLength = 3, ErrorMessage = "Between 3 to 10")]
        //[RegularExpression(@"(\s)+",ErrorMessage = "Blank space!")]
        [Display(Name = "User Id")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "User Name")]
        public string FullName { get; set; }
                
        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Password")]
        public string PassW { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Login Type")]
        public int LoginType { get; set; }

        [Display(Name = "Login Type")]
        public string LoginName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        public string Department { get; set; }

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        public string EMail { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "EmployeeId")]
        public int EmpId { get; set; }

        [Display(Name = "Employee")]
        public string EmpName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        public int HODUserId { get; set; }

        //[Required(ErrorMessage = "Required!")]
        [Display(Name = "HOD")]
        public string HODUserName { get; set; }//d

        public List<UserCompany> AllCompanies { get; set; }
        public List<UserCompany> UserCompanies { get; set; }
        public int[] SelectedCompanies { get; set; }

    }

    public class UserCompany
    {
        public int CompCode { get; set; }
        public string Company { get; set; }//d

    }
}