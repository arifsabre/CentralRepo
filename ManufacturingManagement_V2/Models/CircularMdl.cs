using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CircularMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Circular Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CircularDate { get; set; }

        [Display(Name = "Circular Number")]
        public int CircularNo { get; set; }

        [Display(Name = "Department")]
        public string DepCode { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }//d

        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Display(Name = "From")]
        public string UserName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Subject On")]
        public string SubjectOn { get; set; }

        [Display(Name = "Document")]
        public bool Doc { get; set; }

        public List<CircularMdl> CircularList { get; set; }

    }
}