using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class MasterDocumentNameMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CompCode { get; set; }

        [Display(Name = "Company")]
        public string CmpName { get; set; }//d

        public List<MasterDocumentNameMdl> MDocList { get; set; }

    }
}