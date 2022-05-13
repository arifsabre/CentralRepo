using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    [Table("Emp_Shift")]
    public partial class Emp_Shift
    {
        [Key]
        [Required]
        public int ShiftId{ get; set; }
        [Required]
        public string ShiftName { get; set; }
        [Required]
        public DateTime ShiftStartDate { get; set; }


        [Required]
        public string StartTime { get; set; }
       

        [Required]
        public string EndTime { get; set; }
      

        [Required]
        public string LateStart { get; set; }

        [Required]
        public string LateEnd { get; set; }

        [Required]
        public int NewEmpId { get; set; }

        public string EmpName { get; set; }

        public int compcode { get; set; }

        public string cmpname { get; set; }

        public int UserId { get; set; }
 	public int BioId{ get; set; }

        public string CreatedBy { get; set; }


        public List<Emp_Shift> Item_List { get; set; }
        public List<Emp_Shift> Item_List1 { get; set; }

    }
}