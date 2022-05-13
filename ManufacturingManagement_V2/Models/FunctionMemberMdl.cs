using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class FunctionMemberMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "FctnId")]
        public int FctnId { get; set; }

        [Display(Name = "Function")]
        public string FctnName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Display(Name = "Member")]
        public string FnMember { get; set; }//d

        [Display(Name = "EmpId")]
        public int NewEmpId { get; set; }//d

    }
}