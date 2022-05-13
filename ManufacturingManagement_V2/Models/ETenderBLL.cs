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
    public class ETenderBLL : DbContext
    {
        //
        //public DbSet<SaleMdl> purchases { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        public static ETenderBLL Instance
        {
            get { return new ETenderBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ETenderMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@TenderId", dbobject.TenderId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@TDocCost", dbobject.TDocCost.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdcmpDDNo", dbobject.TdcmpDDNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdcmpDDDate", dbobject.TdcmpDDDate.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TdcmpExmp", dbobject.TdcmpExmp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TDocCostAmt", dbobject.TDocCostAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@EMDCost", dbobject.EMDCost.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmdcDDNo", dbobject.EmdcDDNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmdcDDDate", dbobject.EmdcDDDate.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmdcExmp", dbobject.EmdcExmp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@EmdCostAmt", dbobject.EmdCostAmt, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@ElgCriteria", dbobject.ElgCriteria.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ElgCrtRemarks", dbobject.ElgCrtRemarks.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC1ValidFrom", dbobject.TC1ValidFrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TC1FORDest", dbobject.TC1FORDest.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC1ModeOfDisp", dbobject.TC1ModeOfDisp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC1Insp", dbobject.TC1Insp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC1DelvPeriod", dbobject.TC1DelvPeriod.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC2ValidFrom", dbobject.TC2ValidFrom, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@TC2FORDest", dbobject.TC2FORDest.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC2ModeOfDisp", dbobject.TC2ModeOfDisp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC2Insp", dbobject.TC2Insp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC2DelvPeriod", dbobject.TC2DelvPeriod.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@TC2PmtTerms", dbobject.TC2PmtTerms.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DelvSchedule", dbobject.DelvSchedule.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FormDRequired", dbobject.FormDRequired.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PerfStmt", dbobject.PerfStmt.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DeviationReq", dbobject.DeviationReq.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@AttachedDocs", dbobject.AttachedDocs.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UploadedBy", dbobject.UploadedBy.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FormatFilledBy", dbobject.FormatFilledBy.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ApprovedBy", dbobject.ApprovedBy.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks.Trim(), DbType.String));
            //to update in tender
            cmd.Parameters.Add(mc.getPObject("@TC1PmtTerms", dbobject.TC1PmtTerms.Trim(), DbType.String));
        }
        //
        private List<ETenderMdl> createObjectList(DataSet ds)
        {
            List<ETenderMdl> objlist = new List<ETenderMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ETenderMdl objmdl = new ETenderMdl();
                objmdl.TenderId = Convert.ToInt32(dr["TenderId"].ToString());
                objmdl.TDocCost = dr["TDocCost"].ToString();
                objmdl.TdcmpDDNo = dr["TdcmpDDNo"].ToString();
                objmdl.TdcmpDDDate = dr["TdcmpDDDate"].ToString();
                objmdl.TdcmpExmp = dr["TdcmpExmp"].ToString();
                objmdl.TDocCostAmt = Convert.ToDouble(dr["TDocCostAmt"].ToString());
                objmdl.EMDCost = dr["EMDCost"].ToString();
                objmdl.EmdcDDNo = dr["EmdcDDNo"].ToString();
                objmdl.EmdcDDDate = dr["EmdcDDDate"].ToString();
                objmdl.EmdcExmp = dr["EmdcExmp"].ToString();
                objmdl.EmdCostAmt = Convert.ToDouble(dr["EmdCostAmt"].ToString());
                objmdl.ElgCriteria = dr["ElgCriteria"].ToString();
                objmdl.ElgCrtRemarks = dr["ElgCrtRemarks"].ToString();
                objmdl.TC1ValidFrom = Convert.ToInt32(dr["TC1ValidFrom"].ToString());
                objmdl.TC1FORDest = dr["TC1FORDest"].ToString();
                objmdl.TC1ModeOfDisp = dr["TC1ModeOfDisp"].ToString();
                objmdl.TC1Insp = dr["TC1Insp"].ToString();
                objmdl.TC1DelvPeriod = dr["TC1DelvPeriod"].ToString();
                objmdl.TC1PmtTerms = dr["TC1PmtTerms"].ToString();
                objmdl.TC2ValidFrom = Convert.ToInt32(dr["TC2ValidFrom"].ToString());
                objmdl.TC2FORDest = dr["TC2FORDest"].ToString();
                objmdl.TC2ModeOfDisp = dr["TC2ModeOfDisp"].ToString();
                objmdl.TC2Insp = dr["TC2Insp"].ToString();
                objmdl.TC2DelvPeriod = Convert.ToDateTime(dr["TC2DelvPeriod"].ToString());
                objmdl.TC2PmtTerms = dr["TC2PmtTerms"].ToString();
                objmdl.DelvSchedule = dr["DelvSchedule"].ToString();
                objmdl.FormDRequired = dr["FormDRequired"].ToString();
                objmdl.PerfStmt = dr["PerfStmt"].ToString();
                objmdl.DeviationReq = dr["DeviationReq"].ToString();
                objmdl.AttachedDocs = dr["AttachedDocs"].ToString();
                objmdl.UploadedBy = dr["UploadedBy"].ToString();
                objmdl.FormatFilledBy = dr["FormatFilledBy"].ToString();
                objmdl.ApprovedBy = dr["ApprovedBy"].ToString();
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.TenderNo = dr["TenderNo"].ToString();//d
                objmdl.TenderType = dr["TenderType"].ToString();//d
                objmdl.RailwayName = dr["RailwayName"].ToString();//d
                objmdl.OpeningDate = Convert.ToDateTime(dr["OpeningDate"].ToString());//d
                objmdl.OpeningTime = dr["OpeningTime"].ToString();//d
                objmdl.SerialNo = dr["SerialNo"].ToString();//d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(ETenderMdl dbobject)
        {
            Message = "";
            if (dbobject.TenderId == 0)
            {
                Message = "Empty Tender No!";
                return false;
            }
            if (mc.isValidDate(dbobject.TC2DelvPeriod) == false)
            {
                Message = "Invalid delivery period date!";
                return false;
            }
            if (dbobject.TDocCost == null)
            {
                dbobject.TDocCost = "";
            }
            if (dbobject.TdcmpDDNo == null)
            {
                dbobject.TdcmpDDNo = "";
            }
            if (dbobject.TdcmpDDDate == null)
            {
                dbobject.TdcmpDDDate = "";
            }
            if (dbobject.TdcmpExmp == null)
            {
                dbobject.TdcmpExmp = "";
            }
            if (dbobject.EMDCost == null)
            {
                dbobject.EMDCost = "";
            }
            if (dbobject.EmdcDDNo == null)
            {
                dbobject.EmdcDDNo = "";
            }
            if (dbobject.EmdcDDDate == null)
            {
                dbobject.EmdcDDDate = "";
            }
            if (dbobject.EmdcExmp == null)
            {
                dbobject.EmdcExmp = "";
            }
            if (dbobject.ElgCriteria == null)
            {
                dbobject.ElgCriteria = "";
            }
            if (dbobject.ElgCrtRemarks == null)
            {
                dbobject.ElgCrtRemarks = "";
            }
            if (dbobject.TC1FORDest == null)
            {
                dbobject.TC1FORDest = "";
            }
            if (dbobject.TC1ModeOfDisp == null)
            {
                dbobject.TC1ModeOfDisp = "";
            }
            if (dbobject.TC1Insp == null)
            {
                dbobject.TC1Insp = "";
            }
            if (dbobject.TC1DelvPeriod == null)
            {
                dbobject.TC1DelvPeriod = "";
            }
            if (dbobject.TC2FORDest == null)
            {
                dbobject.TC2FORDest = "";
            }
            if (dbobject.TC2ModeOfDisp == null)
            {
                dbobject.TC2ModeOfDisp = "";
            }
            if (dbobject.TC2Insp == null)
            {
                dbobject.TC2Insp = "";
            }
            if (dbobject.TC2PmtTerms == null)
            {
                dbobject.TC2PmtTerms = "";
            }
            if (dbobject.DelvSchedule == null)
            {
                dbobject.DelvSchedule = "";
            }
            if (dbobject.FormDRequired == null)
            {
                dbobject.FormDRequired = "";
            }
            if (dbobject.PerfStmt == null)
            {
                dbobject.PerfStmt = "";
            }
            if (dbobject.DeviationReq == null)
            {
                dbobject.DeviationReq = "";
            }
            if (dbobject.AttachedDocs == null)
            {
                dbobject.AttachedDocs = "";
            }
            if (dbobject.UploadedBy == null)
            {
                dbobject.UploadedBy = "";
            }
            if (dbobject.FormatFilledBy == null)
            {
                dbobject.FormatFilledBy = "";
            }
            if (dbobject.ApprovedBy == null)
            {
                dbobject.ApprovedBy = "";
            }
            if (dbobject.Remarks == null)
            {
                dbobject.Remarks = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertETender(ETenderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_etender";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_etender, dbobject.TenderId.ToString(), "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("pk_tbl_etender"))
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderBLL", "insertETender", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateETender(ETenderMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_etender";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_etender, dbobject.TenderId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ETenderBLL", "updateETender", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteETender(int tenderid)
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
                cmd.CommandText = "usp_delete_tbl_etender";
                cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_etender, tenderid.ToString(), "Deleted");
                trn.Commit();
                Message = "E-Tender Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                if (st.ToLower().Contains("fk_tbl_etenderitem_tbl_etender"))
                {
                    Message = "This record cannot be deleted! It has been used in Step-2.";
                }
                else
                {
                    Message = mc.setErrorLog("ETenderBLL", "deleteETender", st);
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
        internal ETenderMdl searchETender(int tenderid)
        {
            DataSet ds = new DataSet();
            ETenderMdl objpo = new ETenderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_etender";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objpo = createObjectList(ds)[0];
            }
            return objpo;
        }
        //
        #endregion
        //
    }
}