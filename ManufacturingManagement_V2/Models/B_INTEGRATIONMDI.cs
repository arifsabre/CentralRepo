using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
  // [Table("AAASMSTEMP")]
    public class B_INTEGRATIONMDI
    {
        //[Key]
       // [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
       
        public int RowId { get; set; }
        public string EmpName { get; set; }
        public string ECode { get; set; }
        public string EmpId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy:HH:mm:tt}",
        ApplyFormatInEditMode = true)]
        public DateTime EDate { get; set; }
     
        public string ETime { get; set; }
        public int MCNo { get; set; }
        public string SendData { get; set; }

        public int DeviceId { get; set; }
        public string DeviceStatus { get; set; }
        public string ReaderReplyTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy:HH:mm:tt}",
        ApplyFormatInEditMode = true)]
        public DateTime ReaderPingdateTime { get; set; }

        public string DeviceLocation { get; set; }
        public string DeviceIP { get; set; }
        public string status1 { get; set; }
        public List<B_INTEGRATIONMDI> Item_List { get; set; }
        public List<B_INTEGRATIONMDI> Item_Lis1 { get; set; }

    }
}