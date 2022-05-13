using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class UpdateLOABLL : DbContext
    {
        //
        //internal DbSet<ProductionPlanMdl> productions { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static UpdateLOABLL Instance
        {
            get { return new UpdateLOABLL(); }
        }
        //
        #region private objects
        //
        private List<UpdateLOAMdl> createObjectList(DataSet ds)
        {
            List<UpdateLOAMdl> objlist = new List<UpdateLOAMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                UpdateLOAMdl objmdl = new UpdateLOAMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.TenderId = Convert.ToInt32(dr["tenderid"].ToString());
                objmdl.TenderNo = dr["tenderno"].ToString();
                objmdl.ItemCode = dr["ItemCode"].ToString();
                objmdl.ShortName = dr["ShortName"].ToString();
                objmdl.ConsigneeName = dr["ConsigneeName"].ToString();
                objmdl.OfferedQty = Convert.ToDouble(dr["OfferedQty"].ToString());
                objmdl.BasicRate = Convert.ToDouble(dr["BasicRate"].ToString());
                objmdl.SaleTaxPer = Convert.ToDouble(dr["SaleTaxPer"].ToString());
                objmdl.LoaQty = Convert.ToDouble(dr["loaqty"].ToString());
                objmdl.LoaRate = Convert.ToDouble(dr["LoaRate"].ToString());
                objmdl.LoaAmt = Convert.ToDouble(dr["LoaAmt"].ToString());
                objmdl.SdBgAmount = Convert.ToDouble(dr["SdBgAmount"].ToString());
                objmdl.AalCo = dr["AalCo"].ToString();
                objmdl.DelvSchedule = dr["DelvSchedule"].ToString();
                objmdl.LoaDelvSchedule = dr["LoaDelvSchedule"].ToString();
                objmdl.TCFileNo = dr["TCFileNo"].ToString();
                objmdl.AalCoName = dr["AalCoName"].ToString();//d
                objmdl.LoaNumber = dr["LoaNumber"].ToString();
                objmdl.LoaDate = Convert.ToDateTime(dr["Loadate"].ToString());
                objmdl.LoaDateStr = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(dr["Loadate"].ToString()));
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool checkSetValidModel(UpdateLOAMdl dbobject)
        {
            Message = "";
            if (mc.isValidDate(dbobject.LoaDate) == false)
            {
                Message = "Invalid LOA date!";
                return false;
            }
            if (dbobject.TCFileNo == null)
            {
                dbobject.TCFileNo = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updateLoaQtyRate(UpdateLOAMdl dbobject)
        {
            Result = false;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_loaqty_and_rate";
                cmd.Parameters.Add(mc.getPObject("@LoaQty", dbobject.LoaQty, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@LoaRate", dbobject.LoaRate, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_tender, dbobject.TenderId.ToString(), "LOA Qty/Rate Updated");
                Result = true;
                Message = "LOA Qty/Rate Updated";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UpdateLOABLL", "updateLoaQtyRate", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateLoaInformation(UpdateLOAMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            //
            if (dbobject.TCFileNo.Trim().Length > 0)
            {
                if (dbobject.TCFileNo.Trim().Length < 6)
                {
                    Message = "Invalid case file format!";
                    return;
                }
                string l6 = dbobject.TCFileNo.Trim();
                l6 = l6.Substring(l6.Length - 6);
                if (mc.IsValidInteger(l6.Remove(2, 1)) == false)
                {
                    Message = "Invalid case file format!";
                    return;
                }
            }
            //
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_loa_information";
                cmd.Parameters.Add(mc.getPObject("@aalco", dbobject.AalCo, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@LoaNumber", dbobject.LoaNumber.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@LoaDate", dbobject.LoaDate.ToShortDateString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@SdBgAmount", dbobject.SdBgAmount, DbType.Double));
                cmd.Parameters.Add(mc.getPObject("@LoaDelvSchedule", dbobject.LoaDelvSchedule.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@TCFileNo", dbobject.TCFileNo.Trim(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@tenderid", dbobject.TenderId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_tender, dbobject.TenderId.ToString(), "LOA Information Updated");
                Result = true;
                Message = "LOA Information Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("UpdateLOABLL", "updateLoaInformation", ex.Message);
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
        internal DataSet getObjectData(int tenderid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_records_to_update_loa_qty_rate";
            cmd.Parameters.Add(mc.getPObject("@tenderid", tenderid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<UpdateLOAMdl> getObjectList(int tenderid)
        {
            DataSet ds = getObjectData(tenderid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}