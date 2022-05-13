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
    public class VoucherDetailBLL : DbContext
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
        private void addCommandParameters(SqlCommand cmd, string vtype, string vno, DateTime vdate, string accode, VoucherDetailMdl objvoucher)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@vdate", vdate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@accontra", objvoucher.AcContra, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@amount", objvoucher.Amount, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@drcr", objvoucher.DrCr, DbType.String));
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void Insert_Record(string vtype, string vno, DateTime vdate, string accode, VoucherDetailMdl[] objvoucher)
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
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_voucherdetailforaccount";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < objvoucher.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_voucherdetail";
                    addCommandParameters(cmd, vtype, vno, vdate, accode, objvoucher[i]);
                    cmd.ExecuteNonQuery();
                }
                trn.Commit();
                objvoucher[0].VNo = vno;
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDetailDAL", "Insert_Record", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void Update_Record(string vtype, string vno, DateTime vdate, string accode, VoucherDetailMdl[] objvoucher)
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
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_voucherdetailforaccount";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.ExecuteNonQuery();
                for (int i = 0; i < objvoucher.Length; i++)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_insert_tbl_voucherdetail";
                    addCommandParameters(cmd, vtype, vno, vdate, accode, objvoucher[i]);
                    cmd.ExecuteNonQuery();
                }
                trn.Commit();
                objvoucher[0].VNo = vno;
                Result = true;
                Message = "Record  Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDetailDAL", "Update_Record", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteVoucherDetail(string vtype, string vno)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_voucherdetail";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Message = "Record Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDetailDAL", "deleteVoucherDetail", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteVoucherDetailForAccount(string vtype, string vno, string accode)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            SqlTransaction trn = null;
            Message = "";
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                trn = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Transaction = trn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_voucherdetailforaccount";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
                cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
                cmd.ExecuteNonQuery();
                trn.Commit();
                Message = "Record Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDetailDAL", "deleteVoucherDetailForAccount", ex.Message);
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
        internal DataSet getVoucherDetail(string vtype, string vno, string accode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_voucherdetail";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objCookie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}