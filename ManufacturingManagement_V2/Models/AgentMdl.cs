using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class AgentMdl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AgentId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }

        [Display(Name = "Service Partner")]
        public int SVPTypeId { get; set; }

        [Display(Name = "Service Partner")]
        public string SVPTypeName { get; set; }//d

        [Required(ErrorMessage = "Required!")]
        [Display(Name = "City")]
        public int CityId { get; set; }

        [Display(Name = "City")]
        public string CityName { get; set; }//d

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "EMail")]
        public string EMail { get; set; }

        [Display(Name = "Postal Address")]
        public string PostalAddress { get; set; }

        [Display(Name = "Railway")]
        public string Railway { get; set; }//d

        public string FlName { get; set; }//file upload
        public byte[] FileContent { get; set; }//file upload
    }
}