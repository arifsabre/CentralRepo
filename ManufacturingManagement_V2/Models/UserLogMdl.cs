using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UserLogMdl
    {
        public int RecId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LoginAt { get; set; }
        public string OtpAt { get; set; }
        public string OTP { get; set; }
        public bool IsLogout { get; set; }
        public DateTime LogoutAt { get; set; }
    }
}