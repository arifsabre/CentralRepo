using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class RfqTenderBLL : DbContext
    {
        //
        //public DbSet<RfqTenderMdl> Orders { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        RfqTenderDetailMdl ledgerMdl = new RfqTenderDetailMdl();
        RfqTenderDetailBLL ledgerBLL = new RfqTenderDetailBLL();
        public static RfqTenderBLL Instance
        {
            get { return new RfqTenderBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, RfqTenderMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ReferenceNo", dbobject.ReferenceNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@OfferType", dbobject.OfferType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RfqDate", mc.getStringByDateToStore(dbobject.RfqDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@QuotationNo", dbobject.QuotationNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@QuotationDate", mc.getStringByDateToStore(dbobject.QuotationDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Remarks", dbobject.Remarks, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RailwayId", dbobject.RailwayId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@EstProduct", dbobject.EstProduct.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ProductDesc", dbobject.ProductDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DrgSpecChanged", dbobject.DrgSpecChanged, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ModeOfDisp", dbobject.ModeOfDisp.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InspAuthority", dbobject.InspAuthority.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TC1PmtTerms", dbobject.TC1PmtTerms.Trim(), DbType.String));
        }
        //
        private List<RfqTenderMdl> createObjectList(DataSet ds)
        {
            List<RfqTenderMdl> objlist = new List<RfqTenderMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                RfqTenderMdl objmdl = new RfqTenderMdl();
                objmdl.RfqId = Convert.ToInt32(dr["RfqId"].ToString());
                objmdl.ReferenceNo = dr["ReferenceNo"].ToString();
                objmdl.OfferType = dr["OfferType"].ToString();
                objmdl.OfferTypeName = dr["OfferTypeName"].ToString();//d
                objmdl.RfqDate = mc.getStringByDate(Convert.ToDateTime(dr["RfqDate"].ToString()));
                objmdl.QuotationNo = dr["QuotationNo"].ToString();
                objmdl.QuotationDate = mc.getStringByDate(Convert.ToDateTime(dr["QuotationDate"].ToString()));
                objmdl.Remarks = dr["Remarks"].ToString();
                objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                objmdl.RailwayName = dr["RailwayName"].ToString();//d
                objmdl.EstProduct = dr["EstProduct"].ToString();
                objmdl.ProductDesc = dr["ProductDesc"].ToString();
                objmdl.DrgSpecChanged = dr["DrgSpecChanged"].ToString();
                objmdl.ModeOfDisp = dr["ModeOfDisp"].ToString();
                objmdl.InspAuthority = dr["InspAuthority"].ToString();
                objmdl.TC1PmtTerms = dr["TC1PmtTerms"].ToString();
                if (dr.Table.Columns.Contains("Items"))
                {
                    objmdl.Items = dr["Items"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(RfqTenderMdl dbobject)
        {
            Message = "";
            if (dbobject.ReferenceNo == null)
            {
                Message = "Empty reference no!";
                return false;
            }
            if (dbobject.OfferType == null)
            {
                Message = "Empty offer type!";
                return false;
            }
            //if (mc.isValidDate(dbobject.RfqDate) == false)
            //{
            //    Message = "Invalid RFQ date!";
            //    return false;
            //}
            if (dbobject.QuotationNo == null)
            {
                dbobject.QuotationNo = "";
            }
            //if (mc.isValidDate(dbobject.QuotationDate) == false)
            //{
            //    Message = "Invalid Quotation date!";
            //    return false;
            //}
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
            if (dbobject.ModeOfDisp == null)
            {
                dbobject.ModeOfDisp = "";
            }
            if (dbobject.InspAuthority == null)
            {
                dbobject.InspAuthority = "";
            }
            if (dbobject.TC1PmtTerms == null)
            {
                dbobject.TC1PmtTerms = "";
            }
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].ItemId == 0)
                {
                    Message = "Invalid item entry not allowed!";
                    return false;
                }
                if (dbobject.Ledgers[i].ItemName == "")
                {
                    Message = "Invalid item name entered!";
                    return false;
                }
                if (dbobject.Ledgers[i].UnitName == "")
                {
                    Message = "Invalid unit entered!";
                    return false;
                }
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(RfqTenderMdl dbobject)
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
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_isfound_referenceno";
                cmd.Parameters.Add(mc.getPObject("@ReferenceNo", dbobject.ReferenceNo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
                if (Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection)) == true)
                {
                    Message = "Duplicate reference number entry not allowed!";
                    return;
                }
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_rfqtender";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.RfqId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_rfqtender, "rfqid"));
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd,dbobject.RfqId,dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_rfqtender, dbobject.RfqId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("RfqTenderBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(RfqTenderMdl dbobject)
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
                //updation
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_tbl_rfqtender";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@RfqId", dbobject.RfqId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                ledgerBLL.deleteRfqLedger(cmd, dbobject.RfqId);
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.RfqId, dbobject.Ledgers[i]);
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_rfqtender, dbobject.RfqId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("uk_tbl_rfqtender_m") == true)
                {
                    Message = "Duplicate reference number entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("RfqTenderBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #region file management
        internal void uploadRfqTenderFile(RfqTenderMdl dbobject)
        {
            //dbobject.FileContent = ConvertToByte(File);
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_save_rfqtender_file";
                cmd.Parameters.AddWithValue("@RfqId", dbobject.RfqId);
                cmd.Parameters.AddWithValue("@FlName", dbobject.FlName);
                cmd.Parameters.AddWithValue("@FileContent", dbobject.FileContent);
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_rfqtender, dbobject.RfqId.ToString(), "File Uploaded");
                Result = true;
                Message = "File Uploaded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("RfqTenderBLL", "uploadRfqTenderFile", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal byte[] getRfqTenderFile(int rfqid = 0)
        {
            System.Data.SqlClient.SqlDataReader dataReader;
            byte[] fileContent = null;
            string fileName = "";
            Message = "";
            string constr = System.Configuration.ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed) { conn.Open(); };
                    cmd.Connection = conn;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_rfqtender_file";
                    cmd.Parameters.Add(mc.getPObject("@rfqid", rfqid, DbType.Int32));
                    dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        dataReader.Read();
                        fileContent = (byte[])dataReader["FileContent"];
                        fileName = dataReader["FLName"].ToString();
                    }
                    if (conn != null) { conn.Close(); };
                }
                Message = fileName;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("RfqTenderBLL", "getRfqTenderFile", ex.Message);
            }
            return fileContent;
        }
        //
        #endregion file management
        internal void deleteObject(int rfqid)
        {
            Result = false;
            DataSet ds = new DataSet();
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
                cmd.CommandText = "usp_delete_tbl_rfqtender";//deletion with ledger
                cmd.Parameters.Add(mc.getPObject("@rfqid", rfqid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_rfqtender, rfqid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("RfqTenderBLL", "deleteObject", ex.Message);
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
        internal RfqTenderMdl searchObject(int rfqid)
        {
            DataSet ds = new DataSet();
            RfqTenderMdl dbobject = new RfqTenderMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_rfqtender";
            cmd.Parameters.Add(mc.getPObject("@rfqid", rfqid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getObjectList(rfqid);
            return dbobject;
        }
        //
        internal DataSet getObjectData(DateTime dtfrom, DateTime dtto, bool filterbydt, int itemid, string offertype)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_rfqtender";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@filterbydt", filterbydt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@offertype", offertype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<RfqTenderMdl> getObjectList(DateTime dtfrom, DateTime dtto, bool filterbydt, int itemid, string offertype)
        {
            DataSet ds = getObjectData(dtfrom, dtto, filterbydt, itemid, offertype);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}