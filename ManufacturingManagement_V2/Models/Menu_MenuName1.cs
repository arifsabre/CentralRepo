using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    [Table("Menu_MenuName")]
    public class Menu_MenuName1
    {
        [Key]
        public int MenuId { get; set; }

        [Required(ErrorMessage = "Menu Name Required")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Nav URL Required")]
        public string NavUrl { get; set; }

        public int ParentMenuId { get; set; }


        [Required(ErrorMessage = "Parent Menu Required")]
        public string ParentMenuName { get; set; }
        public List<Menu_MenuName1> Item_List { get; set; }
    }
}