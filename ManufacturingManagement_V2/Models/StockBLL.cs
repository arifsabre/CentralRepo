
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
    public class StockBLL : DbContext
    {
        //
        //public DbSet<StockMdl> Stocks { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objcoockie = new clsCookie();
        StockLedgerMdl ledgerObj = new StockLedgerMdl();
        StockLedgerBLL ledgerBLL = new StockLedgerBLL();
        public static StockBLL Instance
        {
            get { return new StockBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, StockMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@VType", dbobject.VType, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@VNo", dbobject.VNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@VDate", mc.getStringByDateToStore(dbobject.VDate), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@RecDesc", dbobject.RecDesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IndentId", mc.getForSqlIntString(dbobject.IndentId.ToString()), DbType.String));
        }
        //
        private List<StockMdl> createObjectList(DataSet ds)
        {
            List<StockMdl> stocks = new List<StockMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StockMdl objmdl = new StockMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.VType = dr["VType"].ToString();
                objmdl.VNo = Convert.ToInt32(dr["VNo"].ToString());
                objmdl.VDate = mc.getStringByDate(Convert.ToDateTime(dr["VDate"].ToString()));
                if (dr.Table.Columns.Contains("IndentId"))
                {
                    objmdl.IndentId = Convert.ToInt32(dr["IndentId"].ToString());
                }
                if (dr.Table.Columns.Contains("IndentNo"))
                {
                    objmdl.IndentNo = dr["IndentNo"].ToString();//d
                }
                if (dr.Table.Columns.Contains("IndentByName"))
                {
                    objmdl.IndentByName = dr["IndentByName"].ToString();//d
                }
                if (dr.Table.Columns.Contains("RecDesc"))
                {
                    objmdl.RecDesc = dr["RecDesc"].ToString();
                }
                if (dr.Table.Columns.Contains("VTypeName"))
                {
                    objmdl.VTypeName = dr["VTypeName"].ToString();//d
                }
                stocks.Add(objmdl);
            }
            return stocks;
        }
        //
        private bool checkSetValidModel(StockMdl dbobject)
        {
            Message = "";
            //if (mc.isValidDate(dbobject.VDate) == false)
            //{
            //    Message = "Invalid date";
            //    return false;
            //}
            //if (dbobject.VendorId == 0)
            //{
            //    Message = "Vendor not selected!";
            //    return false;
            //}
            if (dbobject.RecDesc == null)
            {
                dbobject.RecDesc = "";
            }
            dbobject.Ledgers = ledgerBLL.setStockLedgerValues(dbobject.Ledgers);
            return true;
        }
        //
        private DataSet getVTypeVNoByRecId(int recid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_vtypevno_for_stock";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(StockMdl dbobject)
        {
            Result = false;
            if (dbobject.VType.ToLower() == "ii")
            {
                Message = "Invalid voucher type selected!";
                return;
            }
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
                cmd.CommandText = "usp_get_new_stock_vno";
                cmd.Parameters.Add(mc.getPObject("@vtype", dbobject.VType, DbType.String));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objcoockie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@FinYear", objcoockie.getFinYear(), DbType.String));
                dbobject.VNo = Convert.ToInt32(mc.getFromDatabase(cmd, cmd.Connection));
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_stock";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_stock, "recid");
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, mc.getDateByString(dbobject.VDate), dbobject.Ledgers[i]);
                }
                mc.setEventLog(cmd, dbTables.tbl_stock, recid, "Inserted " + dbobject.VType);
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("StockBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(StockMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_update_tbl_stock";//deletes store ledger also
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                //getting and setting original vtype,vno so that it cannot be changed in store/ledger
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_get_vtypevno_for_stock";
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                mc.fillFromDatabase(ds, cmd, cmd.Connection);
                dbobject.VType = ds.Tables[0].Rows[0]["vtype"].ToString();
                dbobject.VNo = Convert.ToInt32(ds.Tables[0].Rows[0]["vno"].ToString());
                //getting indent ledger items in array
                ArrayList arlitems = new ArrayList();
                //chk for items that it is from indent(if indent found)
                if (dbobject.IndentId != 0)
                {
                    ds = new DataSet();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_get_tbl_indentledger";
                    cmd.Parameters.Add(mc.getPObject("@indentid", dbobject.IndentId, DbType.Int32));
                    mc.fillFromDatabase(ds, cmd,cmd.Connection);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (arlitems.Contains(ds.Tables[0].Rows[i]["itemid"].ToString()) == false)
                        {
                            arlitems.Add(ds.Tables[0].Rows[i]["itemid"].ToString());
                        }
                    }
                    for (int i = 0; i < dbobject.Ledgers.Count; i++)
                    {
                        if (arlitems.Contains(dbobject.Ledgers[i].ItemId.ToString()) == false)
                        {
                            Message = "Invalid indent item : " + dbobject.Ledgers[i].ItemDesc;
                            return;
                        }
                    }
                }
                //
                ledgerBLL.deleteStockLedger(cmd,dbobject.VType,dbobject.VNo);
                for (int i = 0; i < dbobject.Ledgers.Count; i++)
                {
                    ledgerBLL.insertObject(cmd, dbobject.VType, dbobject.VNo, mc.getDateByString(dbobject.VDate), dbobject.Ledgers[i]);
                }
                //
                if (dbobject.IndentId != 0)
                {
                    //updation of issuedqty=sum(qty) by stockledger
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_update_indentledger_by_stockledger";
                    cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.Int32));
                    cmd.ExecuteNonQuery();
                    //updation of issue status by stockledger
                    cmd.Parameters.Clear();
                    cmd.CommandText = "usp_update_indent_issue_status_by_stockledger";
                    cmd.Parameters.Add(mc.getPObject("@IndentId", dbobject.IndentId, DbType.Int32));
                    cmd.ExecuteNonQuery();
                }
                //qty chk for indent ledger and stock ledger
                if (dbobject.IndentId != 0)
                {
                    if (ledgerBLL.isValidIssueAgainstApprovedIndentQty(cmd,dbobject.IndentId,arlitems) == false)
                    {
                        Message = "Invalid issued qty found for item(s)";
                        return;
                    }
                }
                //
                mc.setEventLog(cmd, dbTables.tbl_stock, dbobject.RecId.ToString(), "Updated " + dbobject.VType);
                trn.Commit();
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("StockBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int recid)
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
                //this procedure is with indent reversal performed
                cmd.CommandText = "usp_delete_tbl_stock";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_order, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("StockBLL", "deleteObject", ex.Message);
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
        internal StockMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            StockMdl dbobject = new StockMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_stock";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject = createObjectList(ds)[0];
                }
            }
            ds = new DataSet();
            ds = getVTypeVNoByRecId(recid);
            dbobject.Ledgers = ledgerBLL.getObjectList("xp", "0", "0", "");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbobject.Ledgers = ledgerBLL.getObjectList(ds.Tables[0].Rows[0]["vtype"].ToString(), ds.Tables[0].Rows[0]["vno"].ToString(), ds.Tables[0].Rows[0]["compcode"].ToString(), ds.Tables[0].Rows[0]["finyear"].ToString());
                }
            }
            return dbobject;
        }
        //
        internal DataSet getObjectData(string dtfrom,string dtto,string vtype = "op")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_stock";
            cmd.Parameters.Add(mc.getPObject("@vtype", vtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<StockMdl> getObjectList(string dtfrom, string dtto, string vtype = "op")
        {
            DataSet ds = getObjectData(dtfrom,dtto,vtype);
            return createObjectList(ds);
        }
        //
        internal DataSet getIndentIssueData(string dtfrom, string dtto)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_stock_for_indent";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", objcoockie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", objcoockie.getFinYear(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<StockMdl> getIndentIssueList(string dtfrom, string dtto)
        {
            DataSet ds = getIndentIssueData(dtfrom, dtto);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}