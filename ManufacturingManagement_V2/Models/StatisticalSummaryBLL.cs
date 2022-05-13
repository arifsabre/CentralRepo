using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class StatisticalSummaryBLL : DbContext
    {
        //
        //internal DbSet<WorkListMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        internal StatisticalSummaryMdl getStatisticalSummaryData(DateTime vdate, int compcode)
        {
            DataSet ds = new DataSet();
            StatisticalSummaryMdl dbobject = new StatisticalSummaryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_statistical_summary_info";
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject.CompCode = Convert.ToInt32(ds.Tables[0].Rows[0]["compcode"].ToString());
                    dbobject.CompName = ds.Tables[0].Rows[0]["CompName"].ToString();
                    dbobject.FinYear = ds.Tables[0].Rows[0]["FinYear"].ToString();
                    dbobject.QuarterNo = ds.Tables[0].Rows[0]["QuarterNo"].ToString();
                    dbobject.MonthNo = ds.Tables[0].Rows[0]["MonthNo"].ToString();
                    dbobject.WeekNo = ds.Tables[0].Rows[0]["WeekNo"].ToString();
                    dbobject.AsOnDate = ds.Tables[0].Rows[0]["AsOnDate"].ToString();
                }
            }
            ds = new DataSet();
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_statistical_summary_detail";
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            List<StatisticalSummaryDetailMdl> ssd = new List<StatisticalSummaryDetailMdl> { };
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ssd = createSymmaryDetailList(ds);
                }
            }
            dbobject.SummaryList = ssd;
            return dbobject;
        }
        //
        internal List<CompanyMdl> getSectionList()
        {
            DataSet ds = new DataSet();
            StatisticalSummaryMdl dbobject = new StatisticalSummaryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_section_list";
            mc.fillFromDatabase(ds, cmd);
            List<CompanyMdl> objlist = new List<CompanyMdl> { };
            for (int i = 0; i < ds.Tables[0].Rows.Count;i++ )
            {
                CompanyMdl objmdl = new CompanyMdl();
                objmdl.ShortName = ds.Tables[0].Rows[i]["sccode"].ToString();
                objmdl.CmpName = ds.Tables[0].Rows[i]["scname"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getSegmentListData(string sccode)
        {
            DataSet ds = new DataSet();
            StatisticalSummaryMdl dbobject = new StatisticalSummaryMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_segment_list";
            cmd.Parameters.Add(mc.getPObject("@sccode", sccode, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CompanyMdl> getSegmentList(string sccode)
        {
            DataSet ds = getSegmentListData(sccode);
            CompanyMdl dbobject = new CompanyMdl();
            List<CompanyMdl> objlist = new List<CompanyMdl> { };
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                CompanyMdl objmdl = new CompanyMdl();
                objmdl.ShortName = ds.Tables[0].Rows[i]["segmentcode"].ToString();
                objmdl.CmpName = ds.Tables[0].Rows[i]["segmentname"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #region private objects
        //
        private List<StatisticalSummaryDetailMdl> createSymmaryDetailList(DataSet ds)
        {
            List<StatisticalSummaryDetailMdl> objlist = new List<StatisticalSummaryDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StatisticalSummaryDetailMdl objmdl = new StatisticalSummaryDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.SCCode = dr["SCCode"].ToString();
                objmdl.SCName = dr["SCName"].ToString();
                objmdl.SegmentCode = dr["SegmentCode"].ToString();
                objmdl.SegmentName = dr["SegmentName"].ToString();
                objmdl.SSWeek = Convert.ToDouble(dr["SSWeek"].ToString());
                objmdl.SSMonth = Convert.ToDouble(dr["SSMonth"].ToString());
                objmdl.SSQuarter = Convert.ToDouble(dr["SSQuarter"].ToString());
                objmdl.SSFinYear = Convert.ToDouble(dr["SSFinYear"].ToString());
                objmdl.Unit = dr["Unit"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}