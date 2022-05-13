using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class IndentMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "IndentId")]
        public int IndentId { get; set; }

        [Display(Name = " Indent No")]
        public int VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        //ApplyFormatInEditMode = true)]
        public string VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Indent By")]
        public int IndentBy { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Indent By")]
        public string IndentByName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "HOD")]
        public int HODUserId { get; set; }

        [Display(Name = "HOD")]
        public string HODUserName { get; set; }//d

        [Display(Name = "Indent Value")]
        public double IndentValue { get; set; }//d

        [Display(Name = "Company")]
        public int CompCode { get; set; }//d

        [Display(Name = "Company")]//shortname of company
        public string ShortName { get; set; }//d

        [Display(Name = "Fin. Year")]
        public string FinYear { get; set; }//d

        [Display(Name = "StkRecid")]
        public int StkRecId { get; set; }//d

        [Display(Name = "StrIndentNo")]
        public string StrIndentNo { get; set; }

        public List<IndentLedgerMdl> Ledgers { get; set; }
    }
}