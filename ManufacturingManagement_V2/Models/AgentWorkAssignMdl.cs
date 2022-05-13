using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AgentWorkAssignMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RecId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int AgentId { get; set; }//d

        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }//d

        [Display(Name = "Service Partner")]
        public string SVPTypeName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Work Group")]
        public string WorkId { get; set; }

        [Display(Name = "Work Group")]
        public string WorkName { get; set; }

        public int TenderId { get; set; }
        public int POrderId { get; set; }
        public int SaleRecId { get; set; }

        [Display(Name = "Work Detail")]
        public string WorkDetail { get; set; }

        [Display(Name = "Due Days")]
        public int DueDays { get; set; }

        [Display(Name = "Document No")]
        public string DocNumber { get; set; }

        [Display(Name = "Document Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime DocDate { get; set; }

        [Display(Name = "Assign Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AssignDate { get; set; }

        [Display(Name = "Inique IdNo")]
        public string UniqueIdNo { get; set; }

        [Display(Name = "Acknowledgement Status")]
        public string AckStatus { get; set; }

        [Display(Name = "Acknowledgement Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime AckDate { get; set; }

        [Display(Name = "Completion Status")]
        public string CompStatus { get; set; }

        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime CompDate { get; set; }

        [Display(Name = "Bill Status")]
        public string BillStatus { get; set; }

        [Display(Name = "Required Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        public DateTime ReqCompDate { get; set; }

        [Display(Name = "Delayed Days")]
        public int DelayDays { get; set; }

        [Display(Name = "Work Status")]
        public string WorkStatus { get; set; }

    }
}