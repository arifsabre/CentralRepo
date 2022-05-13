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
    public class InvoiceControlBLL : DbContext
    {
        //
        //internal DbSet<InvoiceControlMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, InvoiceControlMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@IsUnloaded", dbobject.IsUnloaded, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UnloadingDate", dbobject.UnloadingDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@IsRCReceived", dbobject.IsRCReceived, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RCRecDate", dbobject.RCRecDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@IsRNoteReceived", dbobject.IsRNoteReceived, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RNoteDate", dbobject.RNoteDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@IsBillSubmitted", dbobject.IsBillSubmitted, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BillSubmitDate", dbobject.BillSubmitDate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@IsBillSubmittedP2", dbobject.IsBillSubmittedP2, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@BillSubmitDateP2", dbobject.BillSubmitDateP2.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@RCInfo", dbobject.RCInfo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@salerecid", dbobject.SaleRecId, DbType.Int32));
        }
        //
        private List<InvoiceControlMdl> createObjectList(DataSet ds)
        {
            List<InvoiceControlMdl> objlist = new List<InvoiceControlMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                InvoiceControlMdl objmdl = new InvoiceControlMdl();
                objmdl.SaleRecId = Convert.ToInt32(dr["SaleRecId"].ToString());
                objmdl.IsUnloaded = Convert.ToBoolean(dr["IsUnloaded"].ToString());
                objmdl.UnloadingDate = Convert.ToDateTime(dr["UnloadingDate"].ToString());
                objmdl.IsRCReceived = Convert.ToBoolean(dr["IsRCReceived"].ToString());
                objmdl.RCRecDate = Convert.ToDateTime(dr["RCRecDate"].ToString());
                objmdl.IsRNoteReceived = Convert.ToBoolean(dr["IsRNoteReceived"].ToString());
                objmdl.RNoteDate = Convert.ToDateTime(dr["RNoteDate"].ToString());
                objmdl.IsBillSubmitted = Convert.ToBoolean(dr["IsBillSubmitted"].ToString());
                objmdl.BillSubmitDate = Convert.ToDateTime(dr["BillSubmitDate"].ToString());
                objmdl.IsBillSubmittedP2 = Convert.ToBoolean(dr["IsBillSubmittedP2"].ToString());
                objmdl.BillSubmitDateP2 = Convert.ToDateTime(dr["BillSubmitDateP2"].ToString());
                objmdl.GrNo = dr["GrNo"].ToString();//d
                objmdl.RCInfo = dr["RCInfo"].ToString();
                objmdl.BillNo = dr["BillNo"].ToString();
                objmdl.VDate = Convert.ToDateTime(dr["VDate"].ToString());
                objmdl.BillPerP1 = Convert.ToDouble(dr["BillPerP1"].ToString());//d
                objmdl.BillPerP2 = Convert.ToDouble(dr["BillPerP2"].ToString());//d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(InvoiceControlMdl dbobject)
        {
            if (mc.isValidDate(dbobject.UnloadingDate) == false)
            {
                Message = "Invalid unloading date!";
                return false;
            }
            if (mc.isValidDate(dbobject.RCRecDate) == false)
            {
                Message = "Invalid RC receiving date!";
                return false;
            }
            if (mc.isValidDate(dbobject.RNoteDate) == false)
            {
                Message = "Invalid R Note receiving date!";
                return false;
            }
            if (mc.isValidDate(dbobject.BillSubmitDate) == false)
            {
                Message = "Invalid bill submit date!";
                return false;
            }
            if (mc.isValidDate(dbobject.BillSubmitDateP2) == false)
            {
                Message = "Invalid bill submit date P2!";
                return false;
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updateInvoiceControl(InvoiceControlMdl dbobject)
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
                cmd.CommandText = "usp_update_invoice_control";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), "Control Updated");
                trn.Commit();
                Result = true;
                Message = "Invoice Control Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("InvoiceControlBLL", "updateInvoiceControl", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        #region file management
        internal void uploadInvoiceQRCodeFile(InvoiceControlMdl dbobject)
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
                cmd.CommandText = "usp_save_invoice_qrcode_file";
                cmd.Parameters.AddWithValue("@SaleRecId", dbobject.SaleRecId);
                cmd.Parameters.AddWithValue("@FlName", dbobject.FlName);
                cmd.Parameters.AddWithValue("@FileContent", dbobject.FileContent);
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_sale, dbobject.SaleRecId.ToString(), "QR Code Uploaded");
                Result = true;
                Message = "QR Code Uploaded Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("InvoiceControlBLL", "uploadInvoiceQRCodeFile", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        internal byte[] getInvoiceQRCodeFile(int salerecid = 0)
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
                    cmd.CommandText = "usp_get_invoice_qrcode_file";
                    cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
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
                Message = mc.setErrorLog("InvoiceControlBLL", "getInvoiceQRCodeFile", ex.Message);
            }
            return fileContent;
        }
        //
        #endregion file management
        //
        #endregion
        //
        #region fetching objects
        //
        internal DataSet getSaleInformation(int salerecid)
        {
            DataSet ds = new DataSet();
            InvoiceControlMdl dbobject = new InvoiceControlMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_sale";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal InvoiceControlMdl searchObject(int salerecid)
        {
            DataSet ds = new DataSet();
            ds = getSaleInformation(salerecid);
            InvoiceControlMdl dbobject = new InvoiceControlMdl();
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
        internal DataSet getObjectData(DateTime dtfrom, DateTime dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_sale";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<InvoiceControlMdl> getObjectList(DateTime dtfrom, DateTime dtto)
        {
            DataSet ds = getObjectData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        internal DataSet InvoiceControlReportHtml(string potype, DateTime dtfrom, DateTime dtto, string rptfor, string rptopt, int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_invoice_control_report_html";
            cmd.Parameters.Add(mc.getPObject("@potype", potype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rptfor", rptfor, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rptopt", rptopt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}