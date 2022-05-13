using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace ManufacturingManagement_V2.Models
{
    public class LoginMdl
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required User Name?")]
        public string UserName { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Required Password?")]
        public string PassW { get; set; }
        public string OldPassW { get; set; }
        public string NewPassW { get; set; }
        public string ConPassW { get; set; }
        public string Department { get; set; }
        public int LoginType { get; set; }
        public string MobileNo { get; set; }
        public string EMail { get; set; }
        public string OTP { get; set; }
        public bool IsAdmin { get; set; }
    }

}