using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    [Table("tbl_users")]
    public partial class tbl_users
    {
        [Key]
        public int userid { get; set; }
        public string EMail { get; set; }
        public string username { get; set; }
        public string FullName { get; set; }
    }
}