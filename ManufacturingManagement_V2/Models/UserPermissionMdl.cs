using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingManagement_V2.Models
{
    public class UserPermissionMdl
    {
        [Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string UserName { get; set; }//d

        public string FullName { get; set; }//d

        public int CompCode { get; set; }

        public string CmpName { get; set; }//d

        public List<Permission> Entries { get; set; }

    }

    public class Permission
    {
        public int EntryId { get; set; }

        public string EntryName { get; set; }//d

        public bool AddPer { get; set; }

        public bool EditPer { get; set; }

        public bool DeletePer { get; set; }
    }
}