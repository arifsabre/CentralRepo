using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EventLogBLL : DbContext
    {
        //
        //internal DbSet<EventLogMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static CityBLL Instance
        {
            get { return new CityBLL(); }
        }
        //
        #region private objects
        //
        private List<EventLogMdl> createObjectList(DataSet ds)
        {
            List<EventLogMdl> objlist = new List<EventLogMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                EventLogMdl objmdl = new EventLogMdl();
                objmdl.EventId = Convert.ToInt32(dr["EventId"].ToString());
                objmdl.TblId = Convert.ToInt32(dr["tblid"].ToString());
                objmdl.PKVal = dr["PKVal"].ToString();
                objmdl.EVDate = mc.getStringByDate(Convert.ToDateTime(dr["evdate"].ToString()));
                objmdl.EVTime = dr["EVTime"].ToString();
                objmdl.LogDesc = dr["LogDesc"].ToString();
                objmdl.FullName = dr["FullName"].ToString();
                objmdl.TblName = dr["TblName"].ToString();
                objmdl.PKField = dr["PKField"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal List<EventLogMdl> getEventLogDetail(int tblid, string pkval)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_event_log_detail";
            cmd.Parameters.Add(mc.getPObject("@tblid", tblid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@pkval", pkval, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}