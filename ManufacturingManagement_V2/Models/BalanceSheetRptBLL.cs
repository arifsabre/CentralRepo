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
    public class BalanceSheetRptBLL : DbContext
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
        private void setBSheetOption(int ccode)
        {
            //[100079]
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_set_bsheet_option";
                cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("BillOSRptDAL", "setBSheetOption", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        /// <summary>
        /// Returns datatable of debits/credit
        /// from trial balance table
        /// </summary>
        /// <returns></returns>
        private void processDebitCreditFile(DateTime vdate, DataTable dttrialbalance, DataTable dtdebitcredit, drCrType drcr, int ccode, string finyear)
        {
            //[100079]
            CompanyBLL compBLL = new CompanyBLL();
            FinYearMdl fymdl = new FinYearMdl();
            fymdl = compBLL.getDateRangeByFinancialYear(ccode, finyear);
            //
            //--temp work
            setBSheetOption(ccode);//to decide applicability
            //
            //process trial balance table by trialbalancerptdal
            TrialBalanceRptBLL objtrb = new TrialBalanceRptBLL();
            objtrb.prepareTrialBalance(ccode, fymdl.FromDate, vdate, dttrialbalance, finyear);//[100072]
            dtdebitcredit.Rows.Clear();
            //
            if (drcr == drCrType.Dr)
            {
                for (int i = 0; i < dttrialbalance.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dttrialbalance.Rows[i]["dramt"].ToString()) > 0 && dttrialbalance.Rows[i]["actype"].ToString().ToLower() != "p" && Convert.ToBoolean(dttrialbalance.Rows[i]["bsheet"].ToString()) == true)
                    {
                        addRowFromTrb(dtdebitcredit, dttrialbalance.Rows[i], "dramt");
                    }
                }
            }
            else if (drcr == drCrType.Cr)
            {
                for (int i = 0; i < dttrialbalance.Rows.Count; i++)
                {
                    if (Convert.ToDouble(dttrialbalance.Rows[i]["cramt"].ToString()) > 0 && dttrialbalance.Rows[i]["actype"].ToString().ToLower() != "p" && Convert.ToBoolean(dttrialbalance.Rows[i]["bsheet"].ToString()) == true)
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
            //[100079]
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
        /// <returns></returns>
        private DataTable getLiabilityAssetFile(DataTable dtgroups, DataTable dtrecords)
        {
            //[100079]
            //liability --cr records
            //asset --dr records
            DataTable dtret = new DataTable();
            dtret.Columns.Add("rectype");
            dtret.Columns.Add("acdesc");
            dtret.Columns.Add("amount");
            string group = string.Empty;
            double dblamt = 0;
            DataRow dr = dtret.NewRow();
            for (int i = 0; i < dtgroups.Rows.Count; i++)
            {
                dblamt = 0;
                group = dtgroups.Rows[i]["grcode"].ToString();
                //to determine that amount for group is to be printed or not
                //if true then individual amount has to be printed under this group
                //else if false then print amount with the group
                bool famt = false;
                //to determine that the group is topmost group and
                //it has to be printed as first record
                bool tmg = dtgroups.Rows[i]["grcode"].ToString() == "0" ? true : false;
                for (int j = 0; j < dtrecords.Rows.Count; j++)
                {
                    if (dtrecords.Rows[j]["grcode"].ToString() == group)
                    {
                        famt = Convert.ToBoolean(dtrecords.Rows[j]["bsheet"].ToString());
                        if (famt == true && tmg == false) { break; };
                        dblamt = dblamt + Convert.ToDouble(dtrecords.Rows[j]["amount"].ToString());
                    }
                }
                //entering groups info as first record
                dr = dtret.NewRow();
                dr["rectype"] = "g";//note: to print in Bold Font at CRPT
                dr["acdesc"] = dtgroups.Rows[i]["grdesc"].ToString();
                if ((tmg == false && famt == false) || (tmg == false && famt == true))//case00or01
                {
                    dr["amount"] = "0";
                }
                else if ((tmg == true && famt == false) || (tmg == true && famt == true))//case10or11
                {
                    dr["amount"] = dblamt.ToString("f2");
                }
                dtret.Rows.Add(dr);
                //entering remaining records
                if (famt == true && tmg == false)
                {
                    for (int j = 0; j < dtrecords.Rows.Count; j++)
                    {
                        if (dtrecords.Rows[j]["grcode"].ToString() == group)
                        {
                            dr = dtret.NewRow();
                            //note: to print in Regular Font at CRPT
                            dr["rectype"] = "a";
                            dr["acdesc"] = dtrecords.Rows[j]["acdesc"].ToString();
                            dr["amount"] = dtrecords.Rows[j]["amount"].ToString();
                            dtret.Rows.Add(dr);
                        }
                    }
                }
            }
            return dtret;
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal void prepareBalanceSheet(DateTime vdate, DataTable dttrialbalance, DataTable dtdebit, DataTable dtcredit, DataTable dtblsheet, int ccode, string finyear)
        {
            //[100079]
            //liability --cr records
            //asset     --dr records

            //prepare debit file for asset
            processDebitCreditFile(vdate, dttrialbalance, dtdebit, drCrType.Dr, ccode, finyear);
            //prepare credit file for liability
            processDebitCreditFile(vdate, dttrialbalance, dtcredit, drCrType.Cr, ccode, finyear);

            //sorting dr cr records on grcode
            dtdebit.DefaultView.Sort = "grcode";
            dtcredit.DefaultView.Sort = "grcode";

            //getting records of debit grcodes
            DataTable dtDebitGr = getDebitCreditGrCodes(dtdebit);
            //getting records of credit grcodes
            DataTable dtCreditGr = getDebitCreditGrCodes(dtcredit);
            //

            //get processed credit file for liability
            DataTable dtPrcCredit = getLiabilityAssetFile(dtCreditGr, dtcredit);

            //get processed debit file for asset
            DataTable dtPrcDebit = getLiabilityAssetFile(dtDebitGr, dtdebit);

            //process debit credit files to prepare dtblsheet for printing
            dtblsheet.Rows.Clear();

            DataRow dr = dtblsheet.NewRow();
            int cnt = dtPrcCredit.Rows.Count < dtPrcDebit.Rows.Count ? dtPrcCredit.Rows.Count : dtPrcDebit.Rows.Count;
            for (int i = 0; i < cnt; i++)
            {
                dr = dtblsheet.NewRow();
                dr["librectype"] = dtPrcCredit.Rows[i]["rectype"].ToString();
                dr["liability"] = dtPrcCredit.Rows[i]["acdesc"].ToString();
                dr["libamount"] = dtPrcCredit.Rows[i]["amount"].ToString();
                dr["astrectype"] = dtPrcDebit.Rows[i]["rectype"].ToString();
                dr["asset"] = dtPrcDebit.Rows[i]["acdesc"].ToString();
                dr["astamount"] = dtPrcDebit.Rows[i]["amount"].ToString();
                dtblsheet.Rows.Add(dr);
            }
            if (dtPrcCredit.Rows.Count != dtPrcDebit.Rows.Count)
            {
                if (dtPrcCredit.Rows.Count > dtPrcDebit.Rows.Count)
                {
                    //add remaining credit/liability side records
                    for (int i = cnt; i < dtPrcCredit.Rows.Count; i++)
                    {
                        dr = dtblsheet.NewRow();
                        dr["librectype"] = dtPrcCredit.Rows[i]["rectype"].ToString();
                        dr["liability"] = dtPrcCredit.Rows[i]["acdesc"].ToString();
                        dr["libamount"] = dtPrcCredit.Rows[i]["amount"].ToString();
                        dr["asset"] = "";
                        dr["astamount"] = "0";
                        dtblsheet.Rows.Add(dr);
                    }
                }
                else//if debit records > credit records
                {
                    //add remaining dedit/asset side records
                    for (int i = cnt; i < dtPrcDebit.Rows.Count; i++)
                    {
                        dr = dtblsheet.NewRow();
                        dr["liability"] = "";
                        dr["libamount"] = "0";
                        dr["astrectype"] = dtPrcDebit.Rows[i]["rectype"].ToString();
                        dr["asset"] = dtPrcDebit.Rows[i]["acdesc"].ToString();
                        dr["astamount"] = dtPrcDebit.Rows[i]["amount"].ToString();
                        dtblsheet.Rows.Add(dr);
                    }
                }
            }
            //

            double totalliability = 0;
            double totalasset = 0;
            for (int i = 0; i < dtblsheet.Rows.Count; i++)
            {
                totalliability = totalliability + Convert.ToDouble(dtblsheet.Rows[i]["libamount"].ToString());
                totalasset = totalasset + Convert.ToDouble(dtblsheet.Rows[i]["astamount"].ToString());
            }

            //add as last record of balance sheet
            if (totalliability > totalasset)//Cr>Dr
            {
                dr = dtblsheet.NewRow();
                dr["liability"] = "";
                dr["libamount"] = "0";
                dr["asset"] = "Net Loss B/F from Profit & Loss Account";
                dr["astamount"] = (totalliability - totalasset).ToString("f2");
                dtblsheet.Rows.Add(dr);
            }
            else if (totalasset > totalliability)//Dr>Cr
            {
                dr = dtblsheet.NewRow();
                dr["liability"] = "Net Profit B/F from Profit & Loss Account";
                dr["libamount"] = (totalasset - totalliability).ToString("f2");
                dr["asset"] = "";
                dr["astamount"] = "0";
                dtblsheet.Rows.Add(dr);
            }
            //
        }
        //
        #endregion
        //
    }
}