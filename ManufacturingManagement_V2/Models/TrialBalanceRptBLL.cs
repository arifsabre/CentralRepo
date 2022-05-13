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
    public class TrialBalanceRptBLL : DbContext
    {
        //
        //internal DbSet<AdvanceMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region fetching objects
        //
        internal void prepareTrialBalance(int ccode, DateTime fydtfrom, DateTime vdate, DataTable dtRpt, string finyear)
        {
            //[100072]//[100079]/[100080]
            BalancePostingBLL objbpostingbll = new BalancePostingBLL();
            DataTable dtTRB = objbpostingbll.getRevisedRecordsAfterBalancePosting(fydtfrom, vdate, ccode, finyear);
            dtRpt.Rows.Clear();
            DataRow dr = dtRpt.NewRow();
            double dbl = 0;
            for (int i = 0; i < dtTRB.Rows.Count; i++)
            {
                dr = dtRpt.NewRow();
                dr["accode"] = dtTRB.Rows[i]["accode"].ToString();
                dr["acdesc"] = dtTRB.Rows[i]["acdesc"].ToString();
                dr["cramt"] = "0.00";
                dr["dramt"] = "0.00";
                if (Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString()) < 0)
                {
                    dbl = Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString());
                    dr["cramt"] = Math.Abs(dbl).ToString();
                }
                else
                {
                    dr["dramt"] = dtTRB.Rows[i]["amount"].ToString();
                }
                //to be used in p/l & b/s reports
                dr["rectype"] = dtTRB.Rows[i]["rectype"].ToString();
                dr["grcode"] = dtTRB.Rows[i]["grcode"].ToString();
                dr["actype"] = dtTRB.Rows[i]["actype"].ToString();
                dr["bsheet"] = dtTRB.Rows[i]["bsheet"].ToString();
                dr["lev"] = dtTRB.Rows[i]["lev"].ToString();
                dr["grdesc"] = dtTRB.Rows[i]["grdesc"].ToString();
                //----
                if (Convert.ToDouble(dr["cramt"].ToString()) != 0 || Convert.ToDouble(dr["dramt"].ToString()) != 0)
                {
                    dtRpt.Rows.Add(dr);
                }
                //----
            }
        }
        //
        internal void prepareTrialBalanceBetweenDates(int ccode, DateTime fydtfrom, DateTime dtfrom, DateTime dtto, DataTable dtRpt, string finyear)
        {
            //[100072]//[100079]/[100080]
            BalancePostingBLL objbpostingbll = new BalancePostingBLL();
            //phase 1 for opening
            DataTable dtTRB = objbpostingbll.getRevisedRecordsAfterBalancePosting(fydtfrom, dtfrom.AddDays(-1), ccode, finyear);
            dtRpt.Rows.Clear();
            DataRow dr = dtRpt.NewRow();
            double dbl = 0;
            for (int i = 0; i < dtTRB.Rows.Count; i++)
            {
                dr = dtRpt.NewRow();
                dr["accode"] = dtTRB.Rows[i]["accode"].ToString();
                dr["acdesc"] = dtTRB.Rows[i]["acdesc"].ToString();
                dr["opcramt"] = "0.00";
                dr["opdramt"] = "0.00";
                if (Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString()) < 0)
                {
                    dbl = Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString());
                    dr["opcramt"] = Math.Abs(dbl).ToString();
                }
                else
                {
                    dr["opdramt"] = dtTRB.Rows[i]["amount"].ToString();
                }
                dr["pddramt"] = "0.00";
                dr["pdcramt"] = "0.00";
                dr["cldramt"] = "0.00";
                dr["clcramt"] = "0.00";
                dr["grcode"] = dtTRB.Rows[i]["grcode"].ToString();
                dr["rectype"] = dtTRB.Rows[i]["rectype"].ToString();
                dtRpt.Rows.Add(dr);
            }
            //
            //phase 2 for period
            dtTRB = new DataTable();
            dtTRB = objbpostingbll.getRevisedRecordsAfterBalancePosting(dtfrom, dtto, ccode, finyear);
            string accode = string.Empty;
            dbl = 0;
            for (int i = 0; i < dtTRB.Rows.Count; i++)
            {
                accode = dtTRB.Rows[i]["accode"].ToString();
                for (int j = 0; j < dtRpt.Rows.Count; j++)
                {
                    if (dtRpt.Rows[j]["accode"].ToString() == accode)
                    {
                        if (Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString()) < 0)
                        {
                            dbl = Convert.ToDouble(dtTRB.Rows[i]["amount"].ToString());
                            dtRpt.Rows[j]["pdcramt"] = Math.Abs(dbl).ToString();
                        }
                        else
                        {
                            dtRpt.Rows[j]["pddramt"] = dtTRB.Rows[i]["amount"].ToString();
                        }
                    }
                }
            }
            //
            //phase 3 for closing
            //updating cldramt,clcramt by dtRpt itself
            dbl = 0;
            for (int i = 0; i < dtRpt.Rows.Count; i++)
            {
                dbl = Convert.ToDouble(dtRpt.Rows[i]["opdramt"].ToString()) + Convert.ToDouble(dtRpt.Rows[i]["pddramt"].ToString()) - Convert.ToDouble(dtRpt.Rows[i]["opcramt"].ToString()) - Convert.ToDouble(dtRpt.Rows[i]["pdcramt"].ToString());
                if (dbl < 0)
                {
                    dtRpt.Rows[i]["clcramt"] = Math.Abs(dbl).ToString();
                }
                else
                {
                    dtRpt.Rows[i]["cldramt"] = dbl.ToString();
                }
            }
            //
        }
        //
        #endregion
        //
    }
}