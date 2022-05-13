using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    [Table("AAA_NetworkStatusLog")]
    public class AAA_NetworkStatusLog
    {
        [Key]
        public Int64 PingId { get; set; }
        public string DeviceStatus { get; set; }
        public string ReaderReplyTime { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy:HH:mm:tt}",
        //ApplyFormatInEditMode = true)]
        public DateTime ReaderPingdateTime { get; set; }
        public Int64 DeviceLocation { get; set; }
        public string DeviceIP { get; set; }
        public string Status1 { get; set; }

    }
}