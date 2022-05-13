using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class QuailFunctionMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Function ID")]
        public int FctnId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Function Name")]
        public string FctnName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Function Leader")]
        public int LdUserId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Function Leader")]
        public string FnLeader { get; set; }//d
    }
}