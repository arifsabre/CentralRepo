using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ErpConnection : DbContext
    {
        public ErpConnection()
            : base("name=ErpConnection")
        {

        }
        public DbSet<AAA_Events> AAA_Events { get; set; }
        public DbSet<AAA_NetworkStatusLog> AAA_NetworkStatusLog { get; set; }
        public DbSet<tbl_users> tbl_users { get; set; }
       
    }
}