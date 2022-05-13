using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CorrespondenceMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public string CmpName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Financial Year")]
        public string FinYear { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public string DepCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Department")]
        public string Department { get; set; }//d

        [Display(Name = "DocumentId")]
        public int DocumentId { get; set; }

        [Display(Name = "Master Doc. Name")]
        public string DocumentName { get; set; }//d

        public string Series { get; set; }

        [Display(Name = "CorpNo")]
        public int CorpNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Letter Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime LetterDT { get; set; }

        [Display(Name = "Party")]
        public string PartyName { get; set; }

        [Display(Name = "Address")]
        public string CorpAddress { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Subject")]
        public string CorpSubject { get; set; }

        [Display(Name = "Reference")]
        public string CorpReference { get; set; }

        [Display(Name = "Keywords")]
        public string Keywords { get; set; }

        [Display(Name = "Document Link")]
        public string DocumentLink { get; set; }

        [Display(Name = "Document Link")]
        [System.Web.Mvc.AllowHtml]
        public string DocumentURL { get; set; }//d

        [Display(Name = "Letter No")]
        public string LetterNo { get; set; }//d

        [Display(Name = "UserId")]
        public int UserId { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }//d

    }

}