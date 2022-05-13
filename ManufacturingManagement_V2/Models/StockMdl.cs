using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class StockMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VType")]
        public string VType { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VNo")]
        public int VNo { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "VDate")]
        public string VDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Description")]
        public string RecDesc { get; set; }

        [Display(Name = "IndentId")]
        public int IndentId { get; set; }

        [Display(Name = "Indent No")]
        public string IndentNo { get; set; }//d

        [Display(Name = "Indent By")]
        public string IndentByName { get; set; }//d

        [Display(Name = "VType")]
        public string VTypeName { get; set; }//d

        public List<StockLedgerMdl> Ledgers { get; set; }
    }
}