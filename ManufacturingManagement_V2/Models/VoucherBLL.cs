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
    public class VoucherBLL : DbContext
    {
        //
        //internal DbSet<AccountMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcookie = new clsCookie();
        BillOsBLL objBillOsBLL = new BillOsBLL();
        //
        #region private objects
        //
        internal bool isFoundVNo(string vtype, string vno)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_check_exists_voucher_vno";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        private DataTable getContraDrCrAccounts(VoucherMdl objvoucher)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("contradr");//first dr account to use with all cr accounts
            dt.Columns.Add("contracr");//first cr account to use with all dr accounts
            int cdr = 0;
            int ccr = 0;
            //getting contra dr
            for (int i = 0; i < objvoucher.Info.Count; i++)
            {
                if (objvoucher.Info[i].DrCr == "d")
                {
                    cdr = objvoucher.Info[i].AcCode;
                    break;
                }
            }
            //getting contra cr
            for (int i = 0; i < objvoucher.Info.Count; i++)
            {
                if (objvoucher.Info[i].DrCr == "c")
                {
                    ccr = objvoucher.Info[i].AcCode;
                    break;
                }
            }
            dt.Rows.Add(cdr, ccr);
            return dt;
        }
        //
        private bool toTblVoucher(SqlCommand cmd, string vtype, string vno, VoucherMdl objvoucher)
        {
            bool res = false;
            Message = "";
            try
            {
                DataTable dt = getContraDrCrAccounts(objvoucher);
                if (dt.Rows[0]["contradr"].ToString() == "")
                {
                    Message = "No Dr entry found!";
                    return false;
                }
                if (dt.Rows[0]["contracr"].ToString() == "")
                {
                    Message = "No Cr entry found!";
                    return false;
                }
                //
                for (int i = 0; i < objvoucher.Info.Count; i++)
                {
                    if (objvoucher.Info[i].Narration == null)
                    {
                        objvoucher.Info[i].Narration = "";
                    }
                    cmd.Parameters.Clear();
                    //instead of usp_insert_tbl_voucher
                    cmd.CommandText = "usp_insert_voucher_entry";
                    cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@vtype", vtype.Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@vno", vno.Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(objvoucher.VDate), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@accode", objvoucher.Info[i].AcCode, DbType.Int64));
                    if (objvoucher.Info[i].DrCr == "d")
                    {
                        cmd.Parameters.Add(mc.getPObject("@accontra", dt.Rows[0]["contracr"].ToString(), DbType.Int64));
                        cmd.Parameters.Add(mc.getPObject("@dramt", objvoucher.Info[i].DrAmount, DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@cramt", "0", DbType.Double));
                    }
                    else
                    {
                        cmd.Parameters.Add(mc.getPObject("@accontra", dt.Rows[0]["contradr"].ToString(), DbType.Int64));
                        cmd.Parameters.Add(mc.getPObject("@dramt", "0", DbType.Double));
                        cmd.Parameters.Add(mc.getPObject("@cramt", objvoucher.Info[i].CrAmount, DbType.Double));
                    }
                    cmd.Parameters.Add(mc.getPObject("@drcr", objvoucher.Info[i].DrCr.Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@narration", objvoucher.Info[i].Narration.Trim(), DbType.String));
                    //note
                    cmd.Parameters.Add(mc.getPObject("@BDate", mc.getStringByDateToStore(objvoucher.VDate), DbType.String));
                    //VNoLink default null by dbp
                    cmd.ExecuteNonQuery();
                }
                //
                res = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VoucherDAL", "toTblVoucher", ex.Message);
            }
            return res;
        }
        //
        private bool toTblBillOS(SqlCommand cmd, VoucherMdl objvoucher)
        {
            bool res = false;
            Message = "";
            if (objvoucher.BillOsInfo == null) { return true; };
            try
            {
                for (int i = 0; i < objvoucher.BillOsInfo.Count; i++)
                {
                    if (objvoucher.BillOsInfo[i].DrAmount + objvoucher.BillOsInfo[i].CrAmount != 0)
                    {
                        //to billos
                        BillOsMdl billos = new BillOsMdl();
                        billos.VType = objvoucher.VType;
                        billos.VNo = objvoucher.VNo;
                        billos.VDate = objvoucher.VDate;
                        billos.AcCode = objvoucher.BillOsInfo[i].AcCode;
                        billos.BillType = "b";
                        billos.BillNo = objvoucher.BillOsInfo[i].BillNo;
                        billos.BillAmount = 0;
                        billos.BillDate = objvoucher.VDate;
                        billos.DueDate = objvoucher.VDate;
                        if (objvoucher.VType == "bpr")
                        {
                            //get excess amount, if any
                            //double excessamt = 0;
                            //for (int k = 0; k < objvoucher.Info.Count; k++)
                            //{
                            //    if (objvoucher.Info[k].AcCode == 67)//67=excess amount
                            //    {
                            //        excessamt = objvoucher.Info[k].DrAmount;
                            //    }
                            //}
                            //
                            billos.DrAmount = 0;
                            billos.CrAmount = objvoucher.BillOsInfo[i].CrAmount;
                            billos.DrCr = "c";
                            billos.Narration = "Payment Received";
                        }
                        else if (objvoucher.VType == "vpt")
                        {
                            billos.DrAmount = objvoucher.BillOsInfo[i].DrAmount;
                            billos.CrAmount = 0;
                            billos.DrCr = "d";
                            billos.Narration = "Bill Paid";
                        }
                        objBillOsBLL.insertBillOS(cmd, billos);
                    }
                }
                //
                res = true;
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("VoucherDAL", "toTblBillOS", ex.Message);
            }
            return res;
        }
        //
        private List<VoucherMdl> getVoucherObjectList(DataSet ds)
        {
            List<VoucherMdl> objlist = new List<VoucherMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                VoucherMdl objmdl = new VoucherMdl();
                objmdl.CCode = Convert.ToInt16(dr["compcode"].ToString());
                objmdl.FinYR = dr["finyear"].ToString();
                objmdl.VType = dr["vtype"].ToString();
                objmdl.VNo = dr["vno"].ToString();
                objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                objmdl.Info = getVoucherInfoList(ds, objmdl.VNo);
                //
                objmdl.BillAcCode = 0;
                objmdl.BillAcDesc = "";
                objmdl.VendorId = 0;
                objmdl.PayingAuthId = 0;
                //
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<VoucherInfoMdl> getVoucherInfoList(DataSet ds, string vno)
        {
            List<VoucherInfoMdl> objlist = new List<VoucherInfoMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["VNo"].ToString() == vno)
                {
                    VoucherInfoMdl objmdl = new VoucherInfoMdl();
                    objmdl.VoucherId = dr["voucherid"].ToString();
                    objmdl.VNo = vno;
                    objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                    objmdl.AcContra = Convert.ToInt32(dr["AcContra"].ToString());
                    objmdl.DrAmount = Convert.ToDouble(dr["dramt"].ToString());
                    objmdl.CrAmount = Convert.ToDouble(dr["cramt"].ToString());
                    objmdl.DrCr = dr["drcr"].ToString();
                    objmdl.DrCrName = dr["drcrname"].ToString();//d
                    objmdl.Narration = dr["Narration"].ToString();
                    objmdl.BDate = mc.getStringByDate(Convert.ToDateTime(dr["BDate"].ToString()));
                    objmdl.AcDesc = dr["AcDesc"].ToString();
                    objlist.Add(objmdl);
                }
            }
            return objlist;
        }
        //
        private List<VoucherMdl> getVoucherObjectDisplayList(DataSet ds)
        {
            List<VoucherMdl> objlist = new List<VoucherMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                VoucherMdl objmdl = new VoucherMdl();
                objmdl.VNo = dr["vno"].ToString();
                if (dr.Table.Columns.Contains("PKValStr"))
                {
                    objmdl.PKValStr = dr["PKValStr"].ToString();
                }
                objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                objmdl.Info = getVoucherInfoDisplayList(ds, objmdl.VNo);
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private List<VoucherInfoMdl> getVoucherInfoDisplayList(DataSet ds, string vno)
        {
            List<VoucherInfoMdl> objlist = new List<VoucherInfoMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["VNo"].ToString() == vno)
                {
                    VoucherInfoMdl objmdl = new VoucherInfoMdl();
                    objmdl.VNo = vno;
                    objmdl.AcDesc = dr["acdesc"].ToString();
                    objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                    objmdl.Amount = Convert.ToDouble(dr["amount"].ToString());
                    if (dr.Table.Columns.Contains("billno"))
                    {
                        objmdl.dispBillNo = dr["billno"].ToString();
                    }
                    objlist.Add(objmdl);
                }
            }
            return objlist;
        }
        //
        private bool isValidPartySupplier(string vtype, VoucherMdl objvoucher)
        {
            Message = "";
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (vtype.ToLower() == "bpr")
            {
                cmd.CommandText = "usp_get_account_for_payingauthority";
                cmd.Parameters.Add(mc.getPObject("@payingauthid", objvoucher.PayingAuthId, DbType.Int32));
            }
            else if (vtype.ToLower() == "vpt")
            {
                cmd.CommandText = "usp_get_account_for_vendor";
                cmd.Parameters.Add(mc.getPObject("@vendorid", objvoucher.VendorId, DbType.Int32));
            }
            else
            {
                return true;
            }
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            if (objvoucher.BillAcCode == Convert.ToInt32(ds.Tables[0].Rows[0]["accode"].ToString()))
            {
                return true;
            }
            Message = "Invalid Account!";
            return false;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void InsertVoucher(string vtype, VoucherMdl objvoucher, BillOsMdl billos = null)
        {
            Result = false;
            if (isValidPartySupplier(vtype,objvoucher) == false) { return; };
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
                objvoucher.VType = vtype;
                objvoucher.VNo = mc.getNewVNo(cmd, dbTables.tbl_voucher, vtype);
                //
                if (toTblVoucher(cmd, vtype, objvoucher.VNo, objvoucher) == false) { return; };
                if (toTblBillOS(cmd, objvoucher) == false) { return; };
                //
                string ldesc = objcookie.getCompCode() + "/" + objcookie.getFinYear() + "/" + vtype.ToUpper() + "/" + objvoucher.VNo;
                mc.setEventLog(cmd, dbTables.tbl_voucher, ldesc, "Inserted");
                trn.Commit();
                Result = true;
                Message = "Voucher Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDAL", "InsertVoucher", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void UpdateVoucher(string vtype, string vno, VoucherMdl objvoucher)
        {
            Result = false;
            if (isValidPartySupplier(vtype, objvoucher) == false) { return; };
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
                objvoucher.VType = vtype;
                objvoucher.VNo = vno;
                //
                deleteVoucher(cmd, vtype, vno);
                if (toTblVoucher(cmd, vtype, vno, objvoucher) == false) { return; };
                objBillOsBLL.deleteBillOS(cmd, vtype, vno);
                if (toTblBillOS(cmd, objvoucher) == false) { return; };
                //
                string ldesc = objcookie.getCompCode() + "/" + objcookie.getFinYear() + "/" + vtype.ToUpper() + "/" + vno;
                mc.setEventLog(cmd, dbTables.tbl_voucher, ldesc, "Updated");
                trn.Commit();
                objvoucher.VNo = vno;
                Result = true;
                Message = "Voucher Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDAL", "UpdateVoucher", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteVoucher(string vtype, string vno)
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
                cmd.CommandText = "usp_delete_tbl_voucher";
                cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
                cmd.ExecuteNonQuery();
                string ldesc = objcookie.getCompCode() + "/" + objcookie.getFinYear() + "/" + vtype.ToUpper() + "/" + vno;
                mc.setEventLog(cmd, dbTables.tbl_voucher, ldesc, "Deleted");
                trn.Commit();
                Message = "Record Deleted";
                Result = true;
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("VoucherDAL", "Delete_Record", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteVoucher(SqlCommand cmd, string vtype, string vno)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_delete_tbl_voucher";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            cmd.ExecuteNonQuery();
        }
        //
        #endregion
        //
        #region fetching objects
        //
        private void addBillAccountCode(VoucherMdl objvoucher)
        {
            DataSet ds = new DataSet();
            if (!(objvoucher.VType.ToLower() == "bpr" || objvoucher.VType.ToLower() == "vpt"))
            {
                return;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_bill_adjustment_info";
            cmd.Parameters.Add(mc.getPObject("@vtype", objvoucher.VType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", objvoucher.VNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            objvoucher.BillAcCode = Convert.ToInt32(ds.Tables[0].Rows[0]["billaccode"].ToString());
            objvoucher.BillAcDesc = ds.Tables[0].Rows[0]["billacdesc"].ToString();
            objvoucher.PayingAuthId = Convert.ToInt32(ds.Tables[0].Rows[0]["PayingAuthId"].ToString());
            objvoucher.VendorId = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"].ToString());
        }
        //
        internal VoucherMdl SearchVoucher(string vtype, string vno)
        {
            DataSet ds = getVoucherInfo(vtype, vno);
            VoucherMdl objvoucher = new VoucherMdl();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    objvoucher = getVoucherObjectList(ds)[0];
                }
            }
            addBillAccountCode(objvoucher);
            objvoucher.BillOsInfo = objBillOsBLL.getBillOSList(vtype,vno);
            return objvoucher;
        }
        //
        internal DataSet getVoucherInfo(string vtype, string vno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_voucher";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getVoucherData(string vtype, DateTime dtfrom, DateTime dtto)
        {
            DataSet dsvno = new DataSet();
            DataSet dsvinfo = new DataSet();
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("vno");
            ds.Tables[0].Columns.Add("vdate");
            ds.Tables[0].Columns.Add("accode");
            ds.Tables[0].Columns.Add("acdesc");
            ds.Tables[0].Columns.Add("amount");
            ds.Tables[0].Columns.Add("pkvalstr");
            ds.Tables[0].Columns.Add("billno");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_voucher";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            mc.fillFromDatabase(dsvno, cmd);
            DataRow dr = ds.Tables[0].NewRow();
            ArrayList arlVNo = new ArrayList();
            for (int i = 0; i < dsvno.Tables[0].Rows.Count; i++)
            {
                if (arlVNo.Contains(dsvno.Tables[0].Rows[i]["vno"].ToString()) == false)
                {
                    arlVNo.Add(dsvno.Tables[0].Rows[i]["vno"].ToString());
                    dr = ds.Tables[0].NewRow();
                    dr["vno"] = dsvno.Tables[0].Rows[i]["vno"].ToString();
                    dr["pkvalstr"] = dsvno.Tables[0].Rows[i]["pkvalstr"].ToString();
                    dr["vdate"] = dsvno.Tables[0].Rows[i]["vdate"].ToString();
                    dr["accode"] = dsvno.Tables[0].Rows[i]["accode"].ToString();
                    dr["acdesc"] = dsvno.Tables[0].Rows[i]["acdesc"].ToString();
                    dr["amount"] = dsvno.Tables[0].Rows[i]["amount"].ToString();
                    if (dsvno.Tables[0].Columns.Contains("billno"))
                    {
                        dr["billno"] = dsvno.Tables[0].Rows[i]["billno"].ToString();
                    }
                    ds.Tables[0].Rows.Add(dr);
                }
            }
            return ds;
        }
        //
        internal List<VoucherMdl> getVoucherDataList(string vtype, DateTime dtfrom, DateTime dtto)
        {
            DataSet ds = getVoucherData(vtype,dtfrom,dtto);
            return getVoucherObjectDisplayList(ds);
        }
        //
        internal string getVoucherAmount(string vtype, string vno)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_voucheramount";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@vno", vno, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcookie.getFinYear(), DbType.String));
            return mc.getFromDatabase(cmd);
        }
        //--used in VE_BillReceipt
        internal DataSet getBillsToReceiptData(int payingAuthId)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pending_bills_to_receipt";
            cmd.Parameters.Add(mc.getPObject("@payingAuthId", payingAuthId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //--used in VE_BillPayment
        internal DataSet getBillsForPaymentData(int vendorid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_pending_bills_for_payment";
            cmd.Parameters.Add(mc.getPObject("@vendorid", vendorid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getBillHeadsData()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_billhead";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<AccountMdl> getAccountHeadsToReceivePayment()
        {
            List<AccountMdl> objlist = new List<AccountMdl> { };
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_account_heads_to_receive_payment";
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AccountMdl objmdl = new AccountMdl();
                objmdl.AcCode = Convert.ToInt32(dr["accode"].ToString());
                objmdl.AcDesc = dr["acdesc"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal int getSaleRecIdByBillNo(string billno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_salerecid_by_billno";
            cmd.Parameters.Add(mc.getPObject("@billno", billno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcookie.getCompCode(), DbType.Int16));
            return Convert.ToInt32(mc.getFromDatabase(cmd));
        }
        //
        internal DataSet getBillInfoBySaleRecId(int salerecid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_bill_info_by_salerecid";
            cmd.Parameters.Add(mc.getPObject("@salerecid", salerecid, DbType.Int32));
            mc.fillFromDatabase(ds,cmd);
            return ds;
        }
        //
        internal DataSet getPayingAuthorityDetail(int payingauthid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_PayingAuthority";
            cmd.Parameters.Add(mc.getPObject("@payingauthid", payingauthid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getEmployeeAcCode(int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_employee_accode";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}