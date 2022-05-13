using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class DocumentFileMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Company")]
        public int CCode { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "DocumentType")]
        public int DocumentTypeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }

        [Display(Name = "Sub Category")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "File Name")]
        public string DocFileName { get; set; }

        [Display(Name = "Description")]
        public string FileDesc { get; set; }

        [Display(Name = "ModifyUser")]
        public int ModifyUser { get; set; }

        [Display(Name = "ModifyDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ModifyDate { get; set; }

    }
}