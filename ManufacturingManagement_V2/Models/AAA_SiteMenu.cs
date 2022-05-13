using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_SiteMenu
    {
        public int MenuID { get; set; }

        [Required(ErrorMessage = "Menu Name Required")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Nav URL Required")]
        public string NavURL { get; set; }


        public int ParentMenuID { get; set; }

        [Required(ErrorMessage = "Parent Menu Required")]
        public string ParentMenuName { get; set; }
        public List<AAA_SiteMenu> Item_List { get; set; }
    }
}