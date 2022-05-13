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
    public class TallyDataBLL : DbContext
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
        internal void SaveTallyDrrByExcel(DataSet ds)
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
                
                DateTime vdate = new DateTime();
                vdate = mc.getDateByString(ds.Tables[0].Rows[0]["vdate"].ToString().Substring(0, 10));
                if (mc.isValidDateForFinYear(objCookie.getFinYear(), vdate) == false)
                {
                    Message = "Invalid date for selected financial year!";
                    return;
                }
                //
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_is_found_tallydrr_for_date";
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@VDate", vdate.ToString(), DbType.DateTime));
                if(Convert.ToBoolean(mc.getFromDatabase(cmd, cmd.Connection))==true)
                {
                    Message = "Entry found for this date. Duplicate entry for same date is not allowed!";
                    return;
                }
                //
                string cashcredit = "";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //validation
                    if (ds.Tables[0].Rows[i]["vchtype"].ToString().Length == 0)
                    {
                        Message = "Voucher type not entered! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (ds.Tables[0].Rows[i]["cashcredit"].ToString().Length == 0)
                    {
                        Message = "Cash/credit not entered! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (ds.Tables[0].Rows[i]["cashcredit"].ToString().Trim().ToLower() == "cash")
                    {
                        cashcredit = "c";
                    }
                    else if (ds.Tables[0].Rows[i]["cashcredit"].ToString().Trim().ToLower() == "credit")
                    {
                        cashcredit = "r";
                    }
                    else
                    {
                        Message = "Invalid cash/credit option entered! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (ds.Tables[0].Rows[i]["vendor"].ToString().Length == 0)
                    {
                        Message = "Vendor not entered! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                    
                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["othercharges"].ToString()) == false)
                    {
                        Message = "Invalid other charges! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (ds.Tables[0].Rows[i]["itemcode"].ToString().Length==0)
                    {
                        Message = "Itemcode not entered! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["qty"].ToString()) == false)
                    {
                        Message = "Invalid quantity! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["rate"].ToString()) == false)
                    {
                        Message = "Invalid rate! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["discperunit"].ToString()) == false)
                    {
                        Message = "Invalid discount/unit! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["sgstper"].ToString()) == false)
                    {
                        Message = "Invalid SGST%! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["cgstper"].ToString()) == false)
                    {
                        Message = "Invalid CGST%! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["igstper"].ToString()) == false)
                    {
                        Message = "Invalid IGST%! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["freightrate"].ToString()) == false)
                    {
                        Message = "Invalid freight rate! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    //to file
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_save_tallydrr";
                    cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@vchtype", ds.Tables[0].Rows[i]["vchtype"].ToString(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@cashcredit", cashcredit, DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@vendor", ds.Tables[0].Rows[i]["vendor"].ToString(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@VchNo", ds.Tables[0].Rows[i]["vchno"].ToString(), DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@VDate", vdate.ToString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@othercharges", ds.Tables[0].Rows[i]["othercharges"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@Remarks", ds.Tables[0].Rows[i]["remarks"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@ItemCode", ds.Tables[0].Rows[i]["itemcode"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@Qty", ds.Tables[0].Rows[i]["qty"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@UnitName", ds.Tables[0].Rows[i]["unitname"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@Rate", ds.Tables[0].Rows[i]["rate"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@discperunit", ds.Tables[0].Rows[i]["discperunit"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@sgstper", ds.Tables[0].Rows[i]["sgstper"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@cgstper", ds.Tables[0].Rows[i]["cgstper"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@igstper", ds.Tables[0].Rows[i]["igstper"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@freightrate", ds.Tables[0].Rows[i]["freightrate"].ToString(), DbType.Double));
                    //
                    cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 150);
                    cmd.Parameters["@Msg"].Direction = ParameterDirection.Output;
                    //
                    cmd.ExecuteNonQuery();
                    string msg = cmd.Parameters["@Msg"].Value.ToString();
                    if (msg != "1")
                    {
                        Message = msg + " [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                }
                //
                //sending to purchase
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_send_tallydrr_to_purchase";//with voucher posting
                cmd.Parameters.Add(mc.getPObject("@VDate", vdate.ToString(), DbType.DateTime));
                cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_tallydrr, objCookie.getCompCode()+"/"+objCookie.getFinYear(), "Saved");
                trn.Commit();
                Result = true;
                Message = ds.Tables[0].Rows.Count.ToString() + " Record(s) Saved Successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("TallyDataBLL", "SaveTallyDrrByExcel", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void GenerateBOMByExcel(DataSet ds)
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
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["rmqty"].ToString()) == false)
                    {
                        Message = "Invalid RM quantity! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["wasteqty"].ToString()) == false)
                    {
                        Message = "Invalid waste qty! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                    
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_generate_bom_by_excel";
                    cmd.Parameters.Add(mc.getPObject("@FgItemCode", ds.Tables[0].Rows[i]["FgItemCode"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@RmItemCode", ds.Tables[0].Rows[i]["RmItemCode"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@RmQty", ds.Tables[0].Rows[i]["RmQty"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@WasteQty", ds.Tables[0].Rows[i]["WasteQty"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@Remarks", ds.Tables[0].Rows[i]["Remarks"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@FgUnitName", ds.Tables[0].Rows[i]["FgUnitName"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@RmUnitName", ds.Tables[0].Rows[i]["RmUnitName"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                    //
                    cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 150);
                    cmd.Parameters["@Msg"].Direction = ParameterDirection.Output;
                    //
                    cmd.ExecuteNonQuery();
                    string msg = cmd.Parameters["@Msg"].Value.ToString();
                    if (msg != "1")
                    {
                        Message = msg + " [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                }
                mc.setEventLog(cmd, dbTables.tbl_bom, mc.getStringByDate(DateTime.Now), "BOM Generation");
                trn.Commit();
                Result = true;
                Message = "BOM generated successfully! " + ds.Tables[0].Rows.Count.ToString() + " Row(s) Inserted.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("TallyDataBLL", "GenerateBOMByExcel", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void GenerateItemOpeningByExcel(DataSet ds)
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
                cmd.CommandText = "usp_get_new_stock_vno";
                cmd.Parameters.Add(mc.getPObject("@vtype", "op", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                string vno = mc.getFromDatabase(cmd, cmd.Connection);

                DateTime vdate = new DateTime();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    vdate = mc.getDateByString(ds.Tables[0].Rows[i]["vdate"].ToString().Substring(0, 10));
                    if (mc.isValidDateForFinYear(objCookie.getFinYear(), vdate) == false)
                    {
                        Message = "Invalid date for selected financial year! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["qty"].ToString()) == false)
                    {
                        Message = "Invalid quantity! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                    if (mc.IsValidDouble(ds.Tables[0].Rows[i]["rate"].ToString()) == false)
                    {
                        Message = "Invalid rate! [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }

                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_generate_itemopening_by_excel";
                    cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                    cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@VNo", vno, DbType.Int32));
                    cmd.Parameters.Add(mc.getPObject("@VDate", vdate.ToString(), DbType.DateTime));
                    cmd.Parameters.Add(mc.getPObject("@ItemCode", ds.Tables[0].Rows[i]["itemcode"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@Qty", ds.Tables[0].Rows[i]["qty"].ToString(), DbType.Double));
                    cmd.Parameters.Add(mc.getPObject("@UnitName", ds.Tables[0].Rows[i]["unitname"].ToString().Trim(), DbType.String));
                    cmd.Parameters.Add(mc.getPObject("@Rate", ds.Tables[0].Rows[i]["rate"].ToString(), DbType.Double));
                    //
                    cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 150);
                    cmd.Parameters["@Msg"].Direction = ParameterDirection.Output;
                    //
                    cmd.ExecuteNonQuery();
                    string msg = cmd.Parameters["@Msg"].Value.ToString();
                    if (msg != "1")
                    {
                        Message = msg + " [Row No: " + (i + 1).ToString() + "].";
                        return;
                    }
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_stock";
                cmd.Parameters.Add(mc.getPObject("@VType", "op", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@VNo", vno, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(vdate), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@RecDesc", "By Excel from Tally", DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objCookie.getFinYear(), DbType.String));
                cmd.Parameters.Add(mc.getPObject("@IndentId", mc.getForSqlIntString("0"), DbType.String));
                cmd.ExecuteNonQuery();

                mc.setEventLog(cmd, dbTables.tbl_state, objCookie.getCompCode() + "/" + objCookie.getFinYear(), "Saved");
                trn.Commit();
                Result = true;
                Message = ds.Tables[0].Rows.Count.ToString() + " Record(s) Saved Successfully.";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("TallyDataBLL", "GenerateItemOpeningByExcel", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
    }
}