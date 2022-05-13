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
    public class ProfitNdLossRptBLL : DbContext
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
        #region private objects
        //
        /// <summary>
        /// Adds specified row from trial balance to
        /// Returning table
        /// </summary>
        /// <param name="dtRet"></param>
        /// <param name="drTrb"></param>
        private void addRowFromTrb(DataTable dtdebitcredit, DataRow drTrb, string amtfield)
        {
            DataRow dr = dtdebitcredit.NewRow();
            dr["accode"] = drTrb["accode"].ToString();
            dr["acdesc"] = drTrb["acdesc"].ToString();
            dr["amount"] = drTrb[amtfield].ToString();
            dr["rectype"] = drTrb["rectype"].ToString();
            dr["grcode"] = drTrb["grcode"].ToString();
            dr["actype"] = drTrb["actype"].ToString();
            dr["bsheet"] = drTrb["bsheet"].ToString();
            dr["lev"] = drTrb["lev"].ToString();
            dr["grdesc"] = drTrb["grdesc"].ToString();
            dr["markdel"] = false;
            dtdebitcredit.Rows.Add(dr);
        }
        //
        /// <summary>
        /// Returns datatable of debits/credit
        /// from trial balance table
        /// </summary>
        /// <returns></returns>
        private void processDebitCreditFile(DateTime vdate, DataTable dttrialbalance, DataTable dtdebitcredit, drCrType drcr, int ccode, string finyear)
        {
            //[100080]
            CompanyBLL compBLL = new CompanyBLL();
            FinYearMdl fymdl = new FinYearMdl();
            fymdl = compBLL.getDateRangeByFinancialYear(ccode, finyear);
            //
            TrialBalanceRptBLL objtrbll = new TrialBalanceRptBLL();
            //process trial balance table by trialbalancerptdal
            objtrbll.prepareTrialBalance(ccode, fymdl.FromDate, vdate, dttrialbalance, finyear);//[100072]
            dtdebitcredit.Rows.Clear();
            //
            if (drcr == drCrType.Dr)
            {
                for (int i = 0; i < dttrialbalance.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dttrialbalance.Rows[i]["dramt"].ToString()) > 0 && dttrialbalance.Rows[i]["actype"].ToString().ToLower() == "p" && Convert.ToBoolean(dttrialbalance.Rows[i]["bsheet"].ToString()) == true)
                    {
                        addRowFromTrb(dtdebitcredit, dttrialbalance.Rows[i], "dramt");
                    }
                }
            }
            else if (drcr == drCrType.Cr)
            {
                for (int i = 0; i < dttrialbalance.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dttrialbalance.Rows[i]["cramt"].ToString()) > 0 && dttrialbalance.Rows[i]["actype"].ToString().ToLower() == "p" && Convert.ToBoolean(dttrialbalance.Rows[i]["bsheet"].ToString()) == true)
                    {
                        addRowFromTrb(dtdebitcredit, dttrialbalance.Rows[i], "cramt");
                    }
                }
            }
        }
        //
        /// <summary>
        /// Returns datatable of debit or credit
        /// records having unique grcodes
        /// </summary>
        /// <param name="dtdrcr"></param>
        /// <returns></returns>
        private DataTable getDebitCreditGrCodes(DataTable dtdrcr)
        {
            //[100080]
            DataTable dtret = new DataTable();
            dtret.Columns.Add("grcode");
            dtret.Columns.Add("grdesc");
            for (int i = 0; i < dtdrcr.Rows.Count; i++)
            {
                if (mc.isDuplicateDataTableItem(dtret, "grcode", dtdrcr.Rows[i]["grcode"].ToString()) == false)
                {
                    dtret.Rows.Add(dtdrcr.Rows[i]["grcode"].ToString(), dtdrcr.Rows[i]["grdesc"].ToString());
                }
            }
            return dtret;
        }
        //
        /// <summary>
        /// Returns processed datatable of debit/credit
        /// from debit/credit files processd by trial balance
        /// </summary>
        /// <param name="dtgroups"></param>
        /// <param name="dtrecords"></param>
        /// <param name="phase"></param>
        /// <returns></returns>
        private DataTable getProfitLossFile(DataTable dtgroups, DataTable dtrecords, pnlPhase phase)
        {
            //[100080]
            //profit --dr records
            //loss   --cr records
            AccountBLL actbll=new AccountBLL();
            DataTable dtret = new DataTable();
            dtret.Columns.Add("acdesc");
            dtret.Columns.Add("amount");
            DataRow dr = dtret.NewRow();
            string vgrcode = string.Empty;
            if (phase == pnlPhase.Phase1)
            {
                //skip records having grcode={(22=sales revenue) or (24=direct expenses)}
                //and marks delete for records having grcode=0
                //to return (groupwise)
                for (int i = 0; i < dtrecords.Rows.Count; i++)
                {
                    //(a || b)=!(a && b)
                    if (!(dtrecords.Rows[i]["grcode"].ToString() == Convert.ToInt32(fAccount.SalesRevenue).ToString() && dtrecords.Rows[i]["grcode"].ToString() == Convert.ToInt32(fAccount.DirectExpenses).ToString()))
                    {
                        vgrcode = dtrecords.Rows[i]["grcode"].ToString();
                        while (true)
                        {
                            if (vgrcode == Convert.ToInt32(fAccount.SalesRevenue).ToString() || vgrcode == Convert.ToInt32(fAccount.DirectExpenses).ToString())
                            {
                                break;
                            }
                            else if (vgrcode == "0")
                            {
                                dtrecords.Rows[i]["markdel"] = true;
                                break;
                            }
                            else
                            {
                                vgrcode =actbll.getGroupCode(vgrcode);
                            }
                        }
                    }
                }//iteration around dr/cr file

                //to return groupwise records
                for (int i = 0; i < dtgroups.Rows.Count; i++)
                {
                    for (int j = 0; j < dtrecords.Rows.Count; j++)
                    {
                        if (Convert.ToBoolean(dtrecords.Rows[j]["markdel"].ToString()) != true && dtrecords.Rows[j]["grcode"].ToString() == dtgroups.Rows[i]["grcode"].ToString())
                        {
                            dr = dtret.NewRow();
                            dr["acdesc"] = dtrecords.Rows[j]["acdesc"].ToString();
                            dr["amount"] = dtrecords.Rows[j]["amount"].ToString();
                            dtret.Rows.Add(dr);
                        }
                    }
                }

            }//end phase p1
            else if (phase == pnlPhase.Phase2)
            {
                //discards records which are not marked to delete
                //(i.e. which has been used in phase 1) and returns 
                //remaining records (groupwise)
                for (int i = 0; i < dtgroups.Rows.Count; i++)
                {
                    for (int j = 0; j < dtrecords.Rows.Count; j++)
                    {
                        if (Convert.ToBoolean(dtrecords.Rows[j]["markdel"].ToString()) == true && dtrecords.Rows[j]["grcode"].ToString() == dtgroups.Rows[i]["grcode"].ToString())
                        {
                            dr = dtret.NewRow();
                            dr["acdesc"] = dtrecords.Rows[j]["acdesc"].ToString();
                            dr["amount"] = dtrecords.Rows[j]["amount"].ToString();
                            dtret.Rows.Add(dr);
                        }
                    }
                }

            }//end phase p2

            return dtret;
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareProfitNdLoss(DateTime vdate, DataTable dttrialbalance, DataTable dtdebit, DataTable dtcredit, DataTable dtpnl, int ccode, string finyear)
        {
            //[100080]
            //profit --dr records
            //loss   --cr records

            //prepare debit file for profit
            processDebitCreditFile(vdate, dttrialbalance, dtdebit, drCrType.Dr, ccode, finyear);
            //prepare credit file loss
            processDebitCreditFile(vdate, dttrialbalance, dtcredit, drCrType.Cr, ccode, finyear);

            //sorting dr cr records on grcode
            dtdebit.DefaultView.Sort = "grcode";//profit
            dtcredit.DefaultView.Sort = "grcode";//loss

            //getting records of debit grcodes
            DataTable dtDebitGr = getDebitCreditGrCodes(dtdebit);
            //getting records of credit grcodes
            DataTable dtCreditGr = getDebitCreditGrCodes(dtcredit);
            //

            //process debit credit files to prepare dtpnl for printing
            dtpnl.Rows.Clear();
            DataRow dr = dtpnl.NewRow();

            //processing for phase 1
            //get processed debit file for profit 
            DataTable dtprofit = getProfitLossFile(dtDebitGr, dtdebit, pnlPhase.Phase1);

            //get processed credit file for loss
            DataTable dtloss = getProfitLossFile(dtCreditGr, dtcredit, pnlPhase.Phase1);

            //phase 1 records
            int cnt = dtprofit.Rows.Count < dtloss.Rows.Count ? dtprofit.Rows.Count : dtloss.Rows.Count;
            for (int i = 0; i < cnt; i++)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = dtprofit.Rows[i]["acdesc"].ToString();
                dr["pfamount"] = dtprofit.Rows[i]["amount"].ToString();
                dr["loss"] = dtloss.Rows[i]["acdesc"].ToString();
                dr["lsamount"] = dtloss.Rows[i]["amount"].ToString();
                dtpnl.Rows.Add(dr);
            }
            if (dtprofit.Rows.Count != dtloss.Rows.Count)
            {
                if (dtprofit.Rows.Count > dtloss.Rows.Count)
                {
                    //add remaining credit/profit side records
                    for (int i = cnt; i < dtprofit.Rows.Count; i++)
                    {
                        dr = dtpnl.NewRow();
                        dr["profit"] = dtprofit.Rows[i]["acdesc"].ToString();
                        dr["pfamount"] = dtprofit.Rows[i]["amount"].ToString();
                        dr["loss"] = "";
                        dr["lsamount"] = "0";
                        dtpnl.Rows.Add(dr);
                    }
                }
                else//if debit records > credit records
                {
                    //add remaining dedit/loss side records
                    for (int i = cnt; i < dtloss.Rows.Count; i++)
                    {
                        dr = dtpnl.NewRow();
                        dr["profit"] = "";
                        dr["pfamount"] = "0";
                        dr["loss"] = dtloss.Rows[i]["acdesc"].ToString();
                        dr["lsamount"] = dtloss.Rows[i]["amount"].ToString();
                        dtpnl.Rows.Add(dr);
                    }
                }
            }
            //

            //to insert gross profit/loss as last record for phase 1
            //calculating sums
            double totalprofit = 0;
            double totalloss = 0;
            for (int i = 0; i < dtpnl.Rows.Count; i++)
            {
                totalprofit = totalprofit + Convert.ToDouble(dtpnl.Rows[i]["pfamount"].ToString());
                totalloss = totalloss + Convert.ToDouble(dtpnl.Rows[i]["lsamount"].ToString());
            }

            //add
            if (totalprofit > totalloss)//profit > loss (Dr < Cr)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = "";
                dr["pfamount"] = "0";
                dr["loss"] = "Gross Loss B/F from Balance Sheet";
                dr["lsamount"] = (totalprofit - totalloss).ToString("f2");
                dtpnl.Rows.Add(dr);
            }
            else if (totalloss > totalprofit)//loss > profit (Cr > Dr)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = "Gross Profit B/F from Balance Sheet";
                dr["pfamount"] = (totalloss - totalprofit).ToString("f2");
                dr["loss"] = "";
                dr["lsamount"] = "0";
                dtpnl.Rows.Add(dr);
            }

            //processing for phase 2
            //get processed debit file for profit 
            dtprofit = getProfitLossFile(dtDebitGr, dtdebit, pnlPhase.Phase2);

            //get processed credit file for loss
            dtloss = getProfitLossFile(dtCreditGr, dtcredit, pnlPhase.Phase2);

            //to insert gross profit/loss as first record for phase 2
            //calculating sums
            totalprofit = 0;
            totalloss = 0;
            for (int i = 0; i < dtprofit.Rows.Count; i++)
            {
                totalprofit = totalprofit + Convert.ToDouble(dtprofit.Rows[i]["amount"].ToString());
            }
            for (int i = 0; i < dtloss.Rows.Count; i++)
            {
                totalloss = totalloss + Convert.ToDouble(dtloss.Rows[i]["amount"].ToString());
            }

            //add
            if (totalprofit > totalloss)//profit > loss (Dr < Cr)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = "";
                dr["pfamount"] = "0";
                dr["loss"] = "Gross Loss B/F from Balance Sheet";
                dr["lsamount"] = (totalprofit - totalloss).ToString("f2");
                dtpnl.Rows.Add(dr);
            }
            else if (totalloss > totalprofit)//loss > profit (Cr > Dr)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = "Gross Profit B/F from Balance Sheet";
                dr["pfamount"] = (totalloss - totalprofit).ToString("f2");
                dr["loss"] = "";
                dr["lsamount"] = "0";
                dtpnl.Rows.Add(dr);
            }

            //phase 2 records
            cnt = dtprofit.Rows.Count < dtloss.Rows.Count ? dtprofit.Rows.Count : dtloss.Rows.Count;
            for (int i = 0; i < cnt; i++)
            {
                dr = dtpnl.NewRow();
                dr["profit"] = dtprofit.Rows[i]["acdesc"].ToString();
                dr["pfamount"] = dtprofit.Rows[i]["amount"].ToString();
                dr["loss"] = dtloss.Rows[i]["acdesc"].ToString();
                dr["lsamount"] = dtloss.Rows[i]["amount"].ToString();
                dtpnl.Rows.Add(dr);
            }
            if (dtprofit.Rows.Count != dtloss.Rows.Count)
            {
                if (dtprofit.Rows.Count > dtloss.Rows.Count)
                {
                    //add remaining credit/profit side records
                    for (int i = cnt; i < dtprofit.Rows.Count; i++)
                    {
                        dr = dtpnl.NewRow();
                        dr["profit"] = dtprofit.Rows[i]["acdesc"].ToString();
                        dr["pfamount"] = dtprofit.Rows[i]["amount"].ToString();
                        dr["loss"] = "";
                        dr["lsamount"] = "0";
                        dtpnl.Rows.Add(dr);
                    }
                }
                else//if debit records > credit records
                {
                    //add remaining dedit/loss side records
                    for (int i = cnt; i < dtloss.Rows.Count; i++)
                    {
                        dr = dtpnl.NewRow();
                        dr["profit"] = "";
                        dr["pfamount"] = "0";
                        dr["loss"] = dtloss.Rows[i]["acdesc"].ToString();
                        dr["lsamount"] = dtloss.Rows[i]["amount"].ToString();
                        dtpnl.Rows.Add(dr);
                    }
                }
            }
            //
        }
        //
        #endregion
        //
    }
}