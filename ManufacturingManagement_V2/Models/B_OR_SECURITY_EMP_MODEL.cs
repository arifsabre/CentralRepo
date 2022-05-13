using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class B_OR_SECURITY_EMP_MODEL
    {

        public int ECode { get; set; }

        public string EmpName { get; set; }

        public string compcode { get; set; }

        public bool IsActive { get; set; }

        public List<B_OR_SECURITY_EMP_MODEL> EmpORList { get; set; }
    }
}