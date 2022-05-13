using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionReportBLL : DbContext
    {
        internal string Message { get; set; }
        internal bool Result { get; set; }
       
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        
        internal DataSet GetProductionPlanReportHtml(int ppmonth, int ppyear, int ccode, int formatopt)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_production_plan_material_list";
            cmd.Parameters.Add(mc.getPObject("@ppmonth", ppmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ppyear", ppyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@formatopt", formatopt, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        
    }
}