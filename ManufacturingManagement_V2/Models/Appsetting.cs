using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class Appsetting
    {
        public static string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;

        }
    }
}