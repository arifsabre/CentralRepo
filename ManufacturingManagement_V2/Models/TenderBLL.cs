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
    public class TenderBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        TenderDetailMdl ledgerObj = new TenderDetailMdl();
        TenderDetailBLL ledgerBLL = new TenderDetailBLL();
        public static TenderBLL Instance
        {
            get { return new TenderBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, TenderMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderNo", dbobject.TenderNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SerialNo", dbobject.SerialNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderType", dbobject.TenderType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RecDate", mc.getDateByStringDDMMYYYY(dbobject.RecDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@POType", dbobject.POType.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OpeningDate", mc.getDateByStringDDMMYYYY(dbobject.OpeningDate), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@OpeningTime", dbobject.OpeningTime.Trim(), DbType.Time));//note
            cmd.Parameters.Add(mc.getPObject("@QuotationNo", dbobject.QuotationNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@QuotationDate", dbobject.QuotationDate.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RailwayId", dbobject.RailwayId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@EstProduct", dbobject.EstProduct.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ProductDesc", dbobject.ProductDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DrgSpecChanged", dbobject.DrgSpecChanged.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvSchedule", dbobject.DelvSchedule.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ModeOfDisp", dbobject.ModeOfDisp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InspAuthority", dbobject.InspAuthority.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Clarification", dbobject.Clarification.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TenderStatus", dbobject.TenderStatus, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@altuser", objcoockie.getUserId(), DbType.Int32));//to be used in nitlist
            cmd.Parameters.Add(mc.getPObject("@TC1PmtTerms", dbobject.TC1PmtTerms.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsReTender", dbobject.IsReTender, DbType.String));
        }
        //
        private List<TenderMdl> createObjectList(DataSet ds)
        {
            List<TenderMdl> objlist = new List<TenderMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TenderMdl objmdl = new TenderMdl();
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TenderNo = dr["TenderNo"].ToString();
                if (dr.Table.Columns.Contains("SerialNo"))
                {
                    objmdl.SerialNo = dr["SerialNo"].ToString();
                }
                if (dr.Table.Columns.Contains("TCFileNo"))
                {
                    objmdl.TCFileNo = dr["TCFileNo"].ToString();
                }
                if (dr.Table.Columns.Contains("TenderType"))
                {
                    objmdl.TenderType = dr["TenderType"].ToString();
                }
                if (dr.Table.Columns.Contains("RecDate"))
                {
                    objmdl.RecDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["RecDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("POType"))
                {
                    objmdl.POType = dr["POType"].ToString();
                }
                if (dr.Table.Columns.Contains("OpeningDate"))
                {
                    objmdl.OpeningDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["OpeningDate"].ToString()));
                }
                if (dr.Table.Columns.Contains("OpeningTime"))
                {
                    objmdl.OpeningTime = dr["OpeningTime"].ToString();
                }
                if (dr.Table.Columns.Contains("QuotationNo"))
                {
                    objmdl.QuotationNo = dr["QuotationNo"].ToString();
                }
                if (dr.Table.Columns.Contains("QuotationDate"))
                {
                    objmdl.QuotationDate = dr["QuotationDate"].ToString();
                }
                if (dr.Table.Columns.Contains("QuotationDateStr"))
                {
                    objmdl.QuotationDateStr = dr["QuotationDateStr"].ToString();//d
                }
                if (dr.Table.Columns.Contains("Remarks"))
                {
                    objmdl.Remarks = dr["Remarks"].ToString();
                }
                if (dr.Table.Columns.Contains("RailwayId"))
                {
                    objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                }
                if (dr.Table.Columns.Contains("RailwayName"))
                {
                    objmdl.RailwayName = dr["RailwayName"].ToString();
                }
                if (dr.Table.Columns.Contains("EstProduct"))
                {
                    objmdl.EstProduct = dr["EstProduct"].ToString();
                }
                if (dr.Table.Columns.Contains("ProductDesc"))
                {
                    objmdl.ProductDesc = dr["ProductDesc"].ToString();
                }
                if (dr.Table.Columns.Contains("DrgSpecChanged"))
                {
                    objmdl.DrgSpecChanged = dr["DrgSpecChanged"].ToString();
                }
                if (dr.Table.Columns.Contains("DelvSchedule"))
                {
                    objmdl.DelvSchedule = dr["DelvSchedule"].ToString();
                }
                if (dr.Table.Columns.Contains("ModeOfDisp"))
                {
                    objmdl.ModeOfDisp = dr["ModeOfDisp"].ToString();
                }
                if (dr.Table.Columns.Contains("InspAuthority"))
                {
                    objmdl.InspAuthority = dr["InspAuthority"].ToString();
                }
                if (dr.Table.Columns.Contains("Clarification"))
                {
                    objmdl.Clarification = dr["Clarification"].ToString();
                }
                if (dr.Table.Columns.Contains("TenderStatus"))
                {
                    objmdl.TenderStatus = dr["TenderStatus"].ToString();
                }
                if (dr.Table.Columns.Contains("StatusDesc"))
                {
                    objmdl.StatusDesc = dr["StatusDesc"].ToString();
                }
                if (dr.Table.Columns.Contains("TC1PmtTerms"))
                {
                    objmdl.TC1PmtTerms = dr["TC1PmtTerms"].ToString();
                }
                if (dr.Table.Columns.Contains("IsReTender"))
                {
                    objmdl.IsReTender = Convert.ToBoolean(dr["IsReTender"].ToString());
                }
                if (dr.Table.Columns.Contains("PONumber"))
                {
                    objmdl.PONumber = dr["PONumber"].ToString();
                }
                if (dr.Table.Columns.Contains("PODateStr"))
                {
                    objmdl.PODateStr = dr["PODateStr"].ToString();//d
                }
                if (dr.Table.Columns.Contains("LoaNumber"))
                {
                    objmdl.LoaNumber = dr["LoaNumber"].ToString();
                }
                if (dr.Table.Columns.Contains("LoaDateStr"))
                {
                    objmdl.LoaDateStr = dr["LoaDateStr"].ToString();//d
                }
                if (dr.Table.Columns.Contains("ItemsQty"))
                {
                    objmdl.ItemsQty = dr["ItemsQty"].ToString();
                }
                if (dr.Table.Columns.Contains("Step1"))
                {
                    objmdl.Step1 = dr["Step1"].ToString();
                }
                if (dr.Table.Columns.Contains("Step2"))
                {
                    objmdl.Step2 = dr["Step2"].ToString();
                }
                if (dr.Table.Columns.Contains("Step3"))
                {
                    objmdl.Step3 = dr["Step3"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(TenderMdl dbobject)
        {
            Message = "";
            if (dbobject.TenderNo == null)
            {
                Message = "Empty Tender No!";
                return false;
            }
            if (dbobject.SerialNo == null)
            {
                dbobject.SerialNo = "";
            }
            if (dbobject.TenderType == null)
            {
                dbobject.TenderType = "";
            }
            if (dbobject.RecDate == null)
            {
                Message = "Receiving Date Invalid!";
                return false;
            }
            if (dbobject.POType == null)
            {
                dbobject.POType = "";
            }
            if (dbobject.OpeningDate == null)
            {
                Message = "Opening Date Invalid!";
                return false;
            }
            if (dbobject.OpeningDate == null)
            {
                Message = "Opening Time Invalid!";
                return false;
            }
            if (dbobject.QuotationNo == null)
            {
                dbobject.QuotationNo = "";
            }
            if (dbobject.QuotationDate == null)
            {
                Message = "Quotation Date Invalid!";
                return false;
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            if (dbobject.RailwayId == 0)
            {
                Message = "Railway not selected!";
                return false;
            }
            if (dbobject.EstProduct == null)
            {
                dbobject.EstProduct = "";
            }
            if (dbobject.ProductDesc == null)
            {
                dbobject.ProductDesc = "";
            }
            if (dbobject.DrgSpecChanged == null)
            {
                dbobject.DrgSpecChanged = "";
            }
            if (dbobject.DelvSchedule == null)
            {
                dbobject.DelvSchedule = "";
            }
            if (dbobject.ModeOfDisp == null)
            {
                dbobject.ModeOfDisp = "";
            }
            if (dbobject.InspAuthority == null)
            {
                dbobject.InspAuthority = "";
            }
            if (dbobject.Clarification == null)
            {
                dbobject.Clarification = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertTender(TenderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_insert_tbl_tender_v2";
                addCommandParameters(cmd, dbobject);
                //
                cmd.Parameters.Add("@TenderId", SqlDbType.Int);
                cmd.Parameters["@TenderId"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@RetMsg", SqlDbType.VarChar, 50);
                cmd.Parameters["@RetMsg"].Direction = ParameterDirection.Output;
                //
                cmd.ExecuteNonQuery();
                //
                int tenderid = Convert.ToInt32(cmd.Parameters["@TenderId"].Value.ToString());
                string retmsg = cmd.Parameters["@RetMsg"].Value.ToString();
                //
                if (tenderid == 0)
                {
                    Message = retmsg;
                    return;
                }
                //
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.saveTenderDetail(cmd, tenderid, dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_tender, tenderid.ToString(), "Inserted");
                dbobject.TenderId = tenderid;
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("TenderBLL", "insertTender", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateTender(TenderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_tender";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@tenderid", dbobject.TenderId, DbType.Int32));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.updateTenderDetail(cmd, dbobject.TenderId, dbobject.Ledgers[i]);
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_tender, dbobject.TenderId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_tender"))
                {
                    Message = "Duplicate Tender No entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("TenderDAL", "updateTender", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteTender(int tenderid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            Message = "";
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
                cmd.CommandText = "usp_delete_tbl_tender";//deletes tenderdetail also
                cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_tender, tenderid.ToString(), "Deleted");
                trn.Commit();
                Message = "Tender Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                string st = ex.Message;
                Message = mc.setErrorLog("TenderDAL", "deleteTender", "This record cannot be deleted!\n\rIt has been used further.");
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteTenderDetailRecord(int tenderid, int itemid)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tenderdetailrecord";
                cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
                cmd.ExecuteNonQuery();
                string delKey = "T." + tenderid + "/I." + itemid;
                mc.setEventLog(cmd, dbTables.tbl_tenderdetail, delKey, "Tender Item Deleted");
                Message = "Tender Item Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                if (st.ToLower().Contains("fk_tbl_etenderitem_tbl_tenderdetail"))
                {
                    Message = "This record cannot be deleted! It has been used in Step-2.";
                }
                else
                {
                    Message = mc.setErrorLog("TenderDAL", "deleteTenderDetailRecord", st);
                }
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
        internal DataSet getTenderSearchData(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_tender";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal TenderMdl searchTender(int tenderid)
        {
            DataSet ds = new DataSet();
            ds = getTenderSearchData(tenderid);
            TenderMdl objmdl = new TenderMdl();
            if (ds.Tables[0].Rows.Count > 0)
            {
                objmdl = createObjectList(ds)[0];
            }
            objmdl.Ledgers = ledgerBLL.getTenderDetailList(tenderid);
            return objmdl;
        }
        //
        internal DataSet getTenderData(DateTime dtfrom, DateTime dtto, bool filterbydt, int groupid, int itemid, int railwayid, string tenderstatus, int tenderid, bool isretender)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_tender_v2";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@filterbydt", filterbydt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@groupid", groupid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@tenderstatus", tenderstatus.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@isretender", isretender, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<TenderMdl> getObjectList(DateTime dtfrom, DateTime dtto, bool filterbydt, int groupid, int itemid, int railwayid, string tenderstatus, int tenderid, bool isretender)
        {
            DataSet ds = getTenderData(dtfrom, dtto, filterbydt, groupid, itemid, railwayid, tenderstatus, tenderid, isretender);
            return createObjectList(ds);
        }
        //
        internal DataSet getTenderListData(string rtype = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (rtype != "0")
            {
                cmd.CommandText = "usp_fill_tender_search_list_by_rtype";
                cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            }
            else
            {
                cmd.CommandText = "usp_fill_Tender_search_list";
            }
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal string getTenderInfoString(int tenderid)
        {
            if (tenderid == 0) { return ""; };
            TenderBLL tenderBll = new TenderBLL();
            TenderMdl tenderMdl = new TenderMdl();
            tenderMdl = tenderBll.searchTender(tenderid);
            string tinfo = "Tender No: " + tenderMdl.TenderNo;
            if (tenderMdl.TenderType.Length > 0)
            {
                tinfo += " " + tenderMdl.TenderType;
            }
            tinfo += ", Railway: " + tenderMdl.RailwayName;
            tinfo += ", Due On: " + tenderMdl.OpeningDate;
            tinfo += " at: " + tenderMdl.OpeningTime;
            if (tenderMdl.SerialNo.Length > 0)
            {
                tinfo += ", Serial No: " + tenderMdl.SerialNo;
            }
            return tinfo;
        }
        internal string getTenderInfoString(DataSet ds)
        {
            if (ds.Tables.Count == 0) { return ""; };
            if (ds.Tables[0].Rows.Count == 0) { return ""; };
            string tinfo = "Tender No: " + ds.Tables[0].Rows[0]["TenderNo"].ToString();
            if (ds.Tables[0].Rows[0]["TenderType"].ToString().Length > 0)
            {
                tinfo += " " + ds.Tables[0].Rows[0]["TenderType"].ToString();
            }
            tinfo += ", Railway: " + ds.Tables[0].Rows[0]["RailwayName"].ToString();
            tinfo += ", Due On: " + ds.Tables[0].Rows[0]["OpeningDate"].ToString();
            tinfo += " at: " + ds.Tables[0].Rows[0]["OpeningTime"].ToString();
            if (ds.Tables[0].Rows[0]["SerialNo"].ToString().Length > 0)
            {
                tinfo += ", Serial No: " + ds.Tables[0].Rows[0]["SerialNo"].ToString();
            }
            return tinfo;
        }
        internal DataSet getTenderInformation(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tenderinfo_v2";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getTenderItemInfo(int itemid, int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tender_item_info";
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getTenderStatusListData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_tenderstatus";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ObjectMdl> getTenderStatusList()
        {
            DataSet ds = getTenderStatusListData();
            List<ObjectMdl> objlist = new List<ObjectMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ObjectMdl objmdl = new ObjectMdl();
                objmdl.ObjectCode = dr["TenderStatus"].ToString();
                objmdl.ObjectName = dr["StatusDesc"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal double getCalculatedLoaAmount(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_calculated_loaamount";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            return Convert.ToDouble(mc.getFromDatabase(cmd));
        }
        //
        internal string getLoaDetail(int tenderid, string loanumber="", string loadate="", string aalco="")
        {
            string res = "AAL / Counter Offer :N/A";
            if (loanumber.Length > 0)
            {
                double lamt = getCalculatedLoaAmount(tenderid);
                if (aalco.ToLower() == "a")
                {
                    res = "AAL No: " + loanumber + ", Date: " + loadate;
                    res += ", AAL Amount: " + mc.getINRCFormat(lamt);
                }
                else if (aalco.ToLower() == "c")
                {
                    res = "Counter Offer No: " + loanumber + ", Date: " + loadate;
                    res += ", CO Amount: " + mc.getINRCFormat(lamt);
                }
            }
            return res;
        }
        //
        #endregion
        //
    }
}