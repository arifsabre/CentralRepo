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
    public class AmcWorkBLL : DbContext
    {
        //
        //public DbSet<AmcWorkMdl> Orders { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        AmcDetailMdl ledgerMdl = new AmcDetailMdl();
        AmcDetailBLL ledgerBLL = new AmcDetailBLL();
        public static AmcWorkBLL Instance
        {
            get { return new AmcWorkBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, AmcWorkMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            //to set by dbp--cmd.Parameters.Add(mc.getPObject("@ChallanNo", dbobject.ChallanNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@POrderId", dbobject.POrderId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ConsigneeId", dbobject.ConsigneeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(dbobject.VDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrpMode", dbobject.TrpMode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TrpDetail", dbobject.TrpDetail.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Through", dbobject.Through.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@NoOfPckg", dbobject.NoOfPckg.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@InvNote", dbobject.InvNote.ToString(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SubTotal", dbobject.SubTotal, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@VATAmount", dbobject.VatAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@SATAmount", dbobject.SatAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CSTAmount", dbobject.CstAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@NetAmount", dbobject.NetAmount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@RevChgTaxAmount", dbobject.RevChgTaxAmount, DbType.Double));
        }
        //
        private List<AmcWorkMdl> createObjectList(DataSet ds)
        {
            List<AmcWorkMdl> objlist = new List<AmcWorkMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AmcWorkMdl objmdl = new AmcWorkMdl();
                objmdl.AmcId = Convert.ToInt32(dr["AmcId"].ToString());
                objmdl.ChallanNo = Convert.ToInt32(dr["ChallanNo"].ToString());
                objmdl.ChallanNoStr = dr["ChallanNoStr"].ToString();//d
                objmdl.POrderId = Convert.ToInt32(dr["POrderId"].ToString());
                objmdl.PONumber = dr["PONumber"].ToString();//d
                objmdl.ConsigneeId = Convert.ToInt32(dr["ConsigneeId"].ToString());
                objmdl.ConsigneeName = dr["ConsigneeName"].ToString();//d
                objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                objmdl.TrpMode = dr["TrpMode"].ToString();
                objmdl.TrpDetail = dr["TrpDetail"].ToString();
                objmdl.Through = dr["Through"].ToString();
                objmdl.NoOfPckg = dr["NoOfPckg"].ToString();
                objmdl.InvNote = dr["InvNote"].ToString();
                objmdl.SubTotal = Convert.ToDouble(dr["SubTotal"].ToString());
                objmdl.VatAmount = Convert.ToDouble(dr["VatAmount"].ToString());
                objmdl.SatAmount = Convert.ToDouble(dr["SatAmount"].ToString());
                objmdl.CstAmount = Convert.ToDouble(dr["CstAmount"].ToString());
                objmdl.NetAmount = Convert.ToDouble(dr["NetAmount"].ToString());
                objmdl.RevChgTaxAmount = Convert.ToDouble(dr["RevChgTaxAmount"].ToString());
                if (dr.Table.Columns.Contains("Items"))
                {
                    objmdl.Items = dr["Items"].ToString();
                }
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(AmcWorkMdl dbobject)
        {
            Message = "";
            if (dbobject.POrderId == 0)
            {
                Message = "PO Number not selected!";
                return false;
            }
            if (dbobject.ConsigneeId == 0)
            {
                Message = "Consignee not selected!";
                return false;
            }
            //if (mc.isValidDate(dbobject.VDate) == false)
            //{
            //    Message = "Invalid RFQ date!";
            //    return false;
            //}
            if (dbobject.TrpMode == null)
            {
                dbobject.TrpMode = "";
            }
            if (dbobject.TrpDetail == null)
            {
                dbobject.TrpDetail = "";
            }
            if (dbobject.Through == null)
            {
                dbobject.Through = "";
            }
            if (dbobject.NoOfPckg == null)
            {
                dbobject.NoOfPckg = "";
            }
            if (dbobject.InvNote == null)
            {
                dbobject.InvNote = "";
            }
            for (int i = 0; i < dbobject.Ledgers.Count; i++)
            {
                if (dbobject.Ledgers[i].ItemId == 0)
                {
                    Message = "Invalid item entry not allowed!";
                    return false;
                }
                if (dbobject.Ledgers[i].ItemDesc == "")
                {
                    Message = "Item description not entered!";
                    return false;
                }
                if (dbobject.Ledgers[i].UnitName == "")
                {
                    Message = "Item unit not entered!";
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
        internal void insertObject(AmcWorkMdl dbobject)
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
                
                //not required here
                //cmd.Parameters.Clear();
                //cmd.CommandText = "usp_isfound_challanno";
                //cmd.Parameters.Add(mc.getPObject("@challanno", dbobject.ChallanNo, DbType.Int32));
                //cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
                //cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
                //if (Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection)) == true)
                //{
                //    Message = "Duplicate challan number entry is not allowed!";
                //    return;
                //}
                
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_amc";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.AmcId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_amc, "amcid"));
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd,dbobject.AmcId,dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_amc, dbobject.AmcId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AmcWorkBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(AmcWorkMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_amc";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@amcid", dbobject.AmcId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                ledgerBLL.deleteAmcDetailLedger(cmd, dbobject.AmcId);
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.AmcId, dbobject.Ledgers[i]);
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_amc, dbobject.AmcId.ToString(), "Updated");
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                //if (ex.Message.Contains("uk_tbl_amc") == true)
                //{
                //    Message = "Duplicate challan number entry is not allowed!";
                //}
                //else
                //{
                   Message = mc.setErrorLog("AmcWorkBLL", "updateObject", ex.Message);
                //}
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #region file management
        internal void uploadAmcDocumentFile(AmcWorkMdl dbobject)
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
                cmd.CommandText = "usp_save_amc_file";
                cmd.Parameters.AddWithValue("@amcid", dbobject.AmcId);
                cmd.Parameters.AddWithValue("@FlName", dbobject.FlName);
                cmd.Parameters.AddWithValue("@FileContent", dbobject.FileContent);
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_amc, dbobject.AmcId.ToString(), "File Uploaded");
                Result = true;
                Message = "File Uploaded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("AmcWorkBLL", "uploadAmcDocumentFile", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal byte[] getAmcDocumentFile(int amcid = 0)
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
                    cmd.CommandText = "usp_get_amc_file";
                    cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
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
                Message = mc.setErrorLog("AmcWorkBLL", "getAmcDocumentFile", ex.Message);
            }
            return fileContent;
        }
        //
        #endregion file management
        internal void deleteObject(int amcid)
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
                cmd.CommandText = "usp_delete_tbl_amc";//deletion with ledger
                cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_amc, amcid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("AmcWorkBLL", "deleteObject", ex.Message);
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
        internal AmcWorkMdl searchObject(int amcid)
        {
            DataSet ds = new DataSet();
            AmcWorkMdl dbobject = new AmcWorkMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_amc";
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            dbobject.Ledgers = ledgerBLL.getObjectList(amcid);
            return dbobject;
        }
        //
        internal DataSet getObjectData(DateTime dtfrom, DateTime dtto, bool filterbydt, int itemid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_amc";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@filterbydt", filterbydt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPOConsigneeData(int porderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_consignee_list_by_purchase_order";
            cmd.Parameters.Add(mc.getPObject("@porderid", porderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UnitMdl> getPOConsigneeList(int porderid)
        {
            DataSet ds = getPOConsigneeData(porderid);
            List<UnitMdl> units = new List<UnitMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UnitMdl objmdl = new UnitMdl();
                objmdl.Unit = Convert.ToInt32(dr["ConsigneeId"].ToString());
                objmdl.UnitName = dr["ConsigneeName"].ToString();
                units.Add(objmdl);
            }
            return units;
        }
        //
        internal List<AmcWorkMdl> getObjectList(DateTime dtfrom, DateTime dtto, bool filterbydt, int itemid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, filterbydt, itemid);
            return createObjectList(ds);
        }
        //
        internal DataSet GetAmcReportHtml(int amcid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_amcwork_report";
            cmd.Parameters.Add(mc.getPObject("@amcid", amcid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}