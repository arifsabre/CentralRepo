using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace ManufacturingManagement_V2.Models
{
    public class NitListBLL : DbContext
    {
        //
        //internal DbSet<NitListMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, NitListMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@TenderNo", dbobject.TenderNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DeptRailway", dbobject.DeptRailway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderTitle", dbobject.TenderTitle.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ItemDesc", dbobject.ItemDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderStatus", dbobject.TenderStatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UploadingDT", dbobject.UploadingDT.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ClosingDT", dbobject.ClosingDT.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DueDays", dbobject.DueDays.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EntryDT", dbobject.EntryDT.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@PrcToCS", dbobject.PrcToCS, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NotOurItem", dbobject.NotOurItem, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AlertGenerated", dbobject.AlertGenerated, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ProcessedBy", dbobject.ProcessedBy, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AlertBy", dbobject.AlertBy, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ProcessDT", dbobject.ProcessDT.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", mc.getForSqlIntString(dbobject.CompCode), DbType.Int16));
        }
        //
        private void addCommandParametersForProcess(SqlCommand cmd, NitListMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@PrcToCS", dbobject.PrcToCS, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NotOurItem", dbobject.NotOurItem, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ProcessDT", DateTime.Now, DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@Remarks", "", DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ShortName", dbobject.ShortName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ProcessedBy", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TenderNo", dbobject.TenderNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ClosingDT", dbobject.ClosingDT.Trim(), DbType.String));
        }
        //
        private List<NitListMdl> createObjectList(DataSet ds)
        {
            List<NitListMdl> objlist = new List<NitListMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                NitListMdl objmdl = new NitListMdl();
                objmdl.TenderNo = dr["TenderNo"].ToString();
                objmdl.DeptRailway = dr["DeptRailway"].ToString();
                objmdl.TenderTitle = dr["TenderTitle"].ToString();
                objmdl.ItemDesc = dr["ItemDesc"].ToString();
                objmdl.TenderStatus = dr["TenderStatus"].ToString();
                objmdl.UploadingDT = dr["UploadingDT"].ToString();
                objmdl.ClosingDT = dr["ClosingDT"].ToString();
                objmdl.DueDays = dr["DueDays"].ToString();
                if (dr.Table.Columns.Contains("EntryDT"))
                {
                    objmdl.EntryDT = Convert.ToDateTime(dr["EntryDT"].ToString());
                }
                if (dr.Table.Columns.Contains("PrcToCS"))
                {
                    objmdl.PrcToCS = dr["PrcToCS"].ToString();
                    objmdl.isPrcToCS = Convert.ToBoolean(dr["PrcToCS"].ToString());
                }
                if (dr.Table.Columns.Contains("NotOurItem"))
                {
                    objmdl.NotOurItem = dr["NotOurItem"].ToString();
                    objmdl.isNotOurItem = Convert.ToBoolean(dr["NotOurItem"].ToString());
                }
                if (dr.Table.Columns.Contains("AlertGenerated"))
                {
                    objmdl.AlertGenerated = dr["AlertGenerated"].ToString();
                    objmdl.isAlertGenerated = Convert.ToBoolean(dr["AlertGenerated"].ToString());
                }
                if (dr.Table.Columns.Contains("ProcessedBy"))
                {
                    objmdl.ProcessedBy = dr["ProcessedBy"].ToString();
                }
                if (dr.Table.Columns.Contains("AlertBy"))
                {
                    objmdl.AlertBy = dr["AlertBy"].ToString();
                }
                if (dr.Table.Columns.Contains("ProcessDT"))
                {
                    objmdl.ProcessDT = Convert.ToDateTime(dr["ProcessDT"].ToString());
                }
                if (dr.Table.Columns.Contains("Remarks"))
                {
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (dr.Table.Columns.Contains("CompCode"))
                {
                    objmdl.CompCode = dr["CompCode"].ToString();
                }
                if (dr.Table.Columns.Contains("ShortName"))
                {
                    objmdl.ShortName = dr["ShortName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("ProcessedByName"))
                {
                    objmdl.ProcessedByName = dr["ProcessedByName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("DispUser"))
                {
                    objmdl.DispUser = dr["DispUser"].ToString();//d
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void prepareNITList(NitListMdl dbobject)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                int rcount = 0;
                bool fnd = false;
                DataSet ds = getTenderRecords(cmd,dbobject);
                //
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //
                    NitListMdl objmdl = new NitListMdl();
                    objmdl.TenderNo = ds.Tables[0].Rows[i]["TenderNo"].ToString();
                    objmdl.DeptRailway = ds.Tables[0].Rows[i]["DeptRailway"].ToString();
                    objmdl.TenderTitle = ds.Tables[0].Rows[i]["TenderTitle"].ToString();
                    objmdl.ItemDesc = ds.Tables[0].Rows[i]["ItemDesc"].ToString();
                    objmdl.TenderStatus = ds.Tables[0].Rows[i]["TenderStatus"].ToString();
                    objmdl.UploadingDT = ds.Tables[0].Rows[i]["UploadingDT"].ToString();
                    objmdl.ClosingDT = ds.Tables[0].Rows[i]["ClosingDT"].ToString();
                    objmdl.DueDays = ds.Tables[0].Rows[i]["DueDays"].ToString();
                    objmdl.EntryDT = DateTime.Now;
                    objmdl.PrcToCS = "0";
                    objmdl.NotOurItem = "0";
                    objmdl.AlertGenerated = "0";
                    objmdl.ProcessedBy = objCookie.getUserId();
                    objmdl.AlertBy = objCookie.getUserId();
                    objmdl.ProcessDT = DateTime.Now;
                    objmdl.Remarks = "";
                    objmdl.CompCode = "0";
                    //
                    fnd = false;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_isfound_tender_in_nitlist";
                    cmd.Parameters.Add(mc.getPObject("@TenderNo", objmdl.TenderNo, DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@ClosingDT", objmdl.ClosingDT, DbType.String));
                    fnd = Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection));
                    if (fnd == false)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "usp_insert_tbl_nitlist";
                        addCommandParameters(cmd, objmdl);
                        cmd.ExecuteNonQuery();
                        rcount += 1;
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_nitlist, mc.getStringByDate(DateTime.Now), "NIT List Upload");
                dbobject.TenderList = createObjectList(ds);
                //
                trn.Commit();
                Result = true;
                Message = "NIT List Updated Successfully with " + rcount.ToString() + " Tenders Inserted.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("prepareNITList", "prepareNITList", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        private bool chkValidation(NitListMdl dbobject)
        {
            if (dbobject.ShortName == null)
            {
                dbobject.ShortName = "";
            }
            if (dbobject.PrcToCS.ToLower() == "true" && dbobject.NotOurItem.ToLower() == "true")
            {
                Message = "Invalid Processing!";
                return false;
            }
            if (dbobject.PrcToCS.ToLower() == "false" && dbobject.NotOurItem.ToLower() == "false")
            {
                Message = "Processing option not selected!";
                return false;
            }
            return true;
        }
        //
        internal void updateNITListForProcessing(NitListMdl dbobject)
        {
            Result = false;
            if (chkValidation(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                if (Convert.ToBoolean(dbobject.NotOurItem) == true || Convert.ToBoolean(dbobject.PrcToCS) == true)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_update_nitlist_for_processing";
                    addCommandParametersForProcess(cmd, dbobject);
                    cmd.ExecuteNonQuery();
                }
                Result = true;
                trn.Commit();
                Message = "Record Processed Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("NITListDAL", "updateNITListForProcessing", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateProcessingStatus(NitListMdl dbobject)
        {
            Result = false;
            if (chkValidation(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_processing_status";
                addCommandParametersForProcess(cmd, dbobject);
                cmd.ExecuteNonQuery();
                Result = true;
                trn.Commit();
                Message = "Processing Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("NITListDAL", "updateProcessingStatus", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataSet getNITListToProcess(DateTime uploadingdt)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_nitlist_to_process";
            cmd.Parameters.Add(mc.getPObject("@uploadingdt", uploadingdt.ToShortDateString(), DbType.DateTime));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<NitListMdl> getToProcessNITList(DateTime uploadingdt)
        {
            DataSet ds = getNITListToProcess(uploadingdt);
            return createObjectList(ds);
        }
        //
        internal DataSet getDeptRailwaysExclusionListForNITGeneric()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_deptrailways_exclusion_list_for_nit_generic";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal ArrayList getDeptRailwaysExclusionListForNITGeneric(SqlCommand cmd)
        {
            ArrayList arl = new ArrayList();
            DataSet ds = new DataSet();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_deptrailways_exclusion_list_for_nit_generic";
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                arl.Add(ds.Tables[0].Rows[i]["deptrailway"].ToString());
            }
            return arl;
        }
        //
        internal DataSet getDeptRailwaysExclusionListForNIT()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_deptrailways_exclusion_list_for_nit";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal ArrayList getDeptRailwaysExclusionListForNIT(SqlCommand cmd)
        {
            ArrayList arl = new ArrayList();
            DataSet ds = new DataSet();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_deptrailways_exclusion_list_for_nit";
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                arl.Add(ds.Tables[0].Rows[i]["deptrailway"].ToString());
            }
            return arl;
        }
        //
        internal DataSet getTenderTitlesExclusionListForNIT()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tendertitles_exclusion_list_for_nit";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal ArrayList getTenderTitlesExclusionListForNIT(SqlCommand cmd)
        {
            ArrayList arl = new ArrayList();
            DataSet ds = new DataSet();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tendertitles_exclusion_list_for_nit";
            mc.fillFromDatabase(ds, cmd, cmd.Connection);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                arl.Add(ds.Tables[0].Rows[i]["tendertitle"].ToString());
            }
            return arl;
        }
        //
        internal NitListMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            NitListMdl dbobject = new NitListMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_worklist";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            return dbobject;
        }
        //
        internal DataSet getNITData(DateTime dtfrom, DateTime dtto, string filterby, string filteropt, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_nitlist_v2";//usp_get_tbl_nitlist
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@filterby", filterby, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@filteropt", filteropt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<NitListMdl> getNITList(DateTime dtfrom, DateTime dtto, string filterby, string filteropt, int ccode)
        {
            DataSet ds = getNITData(dtfrom, dtto, filterby, filteropt, ccode);
            return createObjectList(ds);
        }
        //
        #endregion
        //
        #region page content processing by regular expression
        private DataSet getTenderRecords(SqlCommand cmd, NitListMdl dbobject)
        {
            DataSet ds = ConvertHTMLTablesToDataSet(dbobject.PageContent);
            string tblsearch = "deptt./rly. unit";
            string tblName = "Table11";//11 for old
            bool flgTbl = false;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Rows.Count > 0)
                {
                    if (ds.Tables[i].Columns.Count > 0)
                    {
                        if (ds.Tables[i].Rows[0][0].ToString().ToLower().Trim() == tblsearch)
                        {
                            tblName = ds.Tables[i].TableName;
                            flgTbl = true;
                            break;
                        }
                    }
                }
            }
            //
            DataSet dsR = new DataSet();
            if (flgTbl == false)
            {
                return dsR;
            }
            ArrayList arlgendiv = new ArrayList();
            ArrayList arldept = new ArrayList();
            ArrayList arltitle = new ArrayList();
            arlgendiv = getDeptRailwaysExclusionListForNITGeneric(cmd);
            arldept = getDeptRailwaysExclusionListForNIT(cmd);
            arltitle = getTenderTitlesExclusionListForNIT(cmd);
            //
            dsR.Tables.Add();
            dsR.Tables[0].Columns.Add("DeptRailway");//0
            dsR.Tables[0].Columns.Add("TenderNo");//1
            dsR.Tables[0].Columns.Add("TenderTitle");//2
            dsR.Tables[0].Columns.Add("TenderStatus");//3
            dsR.Tables[0].Columns.Add("UploadingDT");//4
            dsR.Tables[0].Columns.Add("ClosingDT");//5
            dsR.Tables[0].Columns.Add("DueDays");//6
            dsR.Tables[0].Columns.Add("ItemDesc");//7
            DataRow dr = dsR.Tables[0].NewRow();
            bool f = true;
            string[] urltno = new string[] { "", "" };
            string url = "";
            string tno = "";
            string deptrly = "";
            string title = "";
            string duedays = "";
            for (int i = 1; i < ds.Tables[tblName].Rows.Count; i++)
            {
                //column 1 is tenderno with url
                urltno = getURLndTenderNo(ds.Tables[tblName].Rows[i][1].ToString());
                url = urltno[0];
                tno = urltno[1];
                //deptrailway = colindx 0
                deptrly = ds.Tables[tblName].Rows[i][0].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Trim();
                //tendertitle = colindx 2
                title = ds.Tables[tblName].Rows[i][2].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Replace("<span class = \"dataText\">", "").Replace("</span>", "").Trim();
                //duedays = colindx 6
                duedays = ds.Tables[tblName].Rows[i][6].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Trim();
                f = true;
                //generic divisions [chk contains]
                for (int j = 0; j < arlgendiv.Count; j++)
                {
                    if (deptrly.ToLower().Contains(arlgendiv[j].ToString().ToLower()))
                    {
                        f = false;
                        break;
                    }
                }
                //specific division [chk equals]
                for (int j = 0; j < arldept.Count; j++)
                {
                    if (deptrly.ToLower().Equals(arldept[j].ToString().ToLower()))
                    {
                        f = false;
                        break;
                    }
                }
                //title [chk value found as word]
                for (int j = 0; j < arltitle.Count; j++)
                {
                    if (title.ToLower().Equals(arltitle[j].ToString().ToLower()) ||
                    title.ToLower().StartsWith(arltitle[j].ToString().ToLower() + " ") ||
                    title.ToLower().EndsWith(" " + arltitle[j].ToString().ToLower()) ||
                    title.ToLower().Contains(" " + arltitle[j].ToString().ToLower() + " "))
                    {
                        f = false;
                        break;
                    }
                }
                if (duedays.ToLower() == "lapsed")
                {
                    f = false;
                }
                if (f == true)
                {
                    dr = dsR.Tables[0].NewRow();
                    dr["DeptRailway"] = deptrly;//0 = DeptRailway
                    dr["TenderNo"] = tno;//retrieved from tno with url by column 1
                    dr["TenderTitle"] = title;//2 = TenderTitle
                    dr["TenderStatus"] = ds.Tables[tblName].Rows[i][3].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Trim();//3 = TenderStatus
                    dr["UploadingDT"] = ds.Tables[tblName].Rows[i][4].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Trim();//4 = UploadingDT
                    dr["ClosingDT"] = ds.Tables[tblName].Rows[i][5].ToString().Replace("\t", "").Replace("&nbsp;", "").Replace("\r\n", "").Trim();//5 = DueDate as ClosingDT
                    dr["DueDays"] = duedays;//6 = DueDays
                    dr["ItemDesc"] = "";//url;//7 = Itemdesc
                    dsR.Tables[0].Rows.Add(dr);
                }
            }
            return dsR;
        }
        //
        private DataSet ConvertHTMLTablesToDataSet(string HTML)
        {
            // Declarations 
            DataSet ds = new DataSet();
            DataTable dt = null;
            DataRow dr = null;
            string TableExpression = "<table[^>]*>(.*?)</table>";
            string HeaderExpression = "<th[^>]*>(.*?)</th>";
            string RowExpression = "<tr[^>]*>(.*?)</tr>";
            string ColumnExpression = "<td[^>]*>(.*?)</td>";
            bool HeadersExist = false;
            int iCurrentColumn = 0;
            int iCurrentRow = 0;
            // Get a match for all the tables in the HTML 
            MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // Loop through each table element 
            foreach (Match Table in Tables)
            {
                // Reset the current row counter and the header flag 
                iCurrentRow = 0;
                HeadersExist = false;
                // Add a new table to the DataSet 
                dt = new DataTable();
                //Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names) 
                if (Table.Value.Contains("<th"))
                {
                    // Set the HeadersExist flag 
                    HeadersExist = true;
                    // Get a match for all the rows in the table 
                    MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    // Loop through each header element 
                    foreach (Match Header in Headers)
                    {
                        dt.Columns.Add(Header.Groups[1].ToString());
                    }
                }
                else
                {
                    for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                    {
                        dt.Columns.Add("Column " + iColumns);
                    }
                }
                //Get a match for all the rows in the table 
                MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                // Loop through each row element 
                foreach (Match Row in Rows)
                {
                    // Only loop through the row if it isn't a header row 
                    if (!(iCurrentRow == 0 && HeadersExist))
                    {
                        // Create a new row and reset the current column counter 
                        dr = dt.NewRow();
                        iCurrentColumn = 0;
                        // Get a match for all the columns in the row 
                        MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        try
                        {
                            // Loop through each column element 
                            foreach (Match Column in Columns)
                            {
                                // Add the value to the DataRow 
                                dr[iCurrentColumn] = Column.Groups[1].ToString().Replace("&nbsp;", "");
                                // Increase the current column  
                                iCurrentColumn++;
                            }
                        }
                        catch (Exception ex)
                        {
                            string st = ex.Message;
                        }
                        // Add the DataRow to the DataTable 
                        dt.Rows.Add(dr);
                    }
                    // Increase the current row counter 
                    iCurrentRow++;
                }
                // Add the DataTable to the DataSet 
                ds.Tables.Add(dt);
            }
            return ds;
        }
        //
        private string[] getURLndTenderNo(string colstr)
        {
            string[] retstr = new string[] { "", "" };
            //getting url string long
            //int p = colstr.IndexOf("http");//old
            int p = colstr.ToLower().IndexOf("onclick");//onclick
            if (p == -1)
            {
                p = colstr.ToLower().IndexOf("span style");
            }
            int q = colstr.IndexOf(">");
            string url = colstr.Substring(p, q - p);
            //getting url string short
            q = url.IndexOf("'");
            if (q == -1)
            {
                q = url.Replace("\"", "'").IndexOf("'");
            }
            url = colstr.Substring(p, q);
            //getting tenderno
            p = colstr.IndexOf(">");
            q = colstr.LastIndexOf("<");
            string tno = colstr.Substring(p + 1, q - p - 1);
            retstr[0] = url;
            retstr[1] = tno;
            return retstr;
        }
        //
        #endregion
        //
    }
}