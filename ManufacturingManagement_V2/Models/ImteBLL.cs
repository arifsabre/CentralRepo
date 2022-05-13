using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ImteBLL : DbContext
    {
        //
        //internal DbSet<ImteMdl> Cities { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static ImteBLL Instance
        {
            get { return new ImteBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ImteMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@IdNo", dbobject.IdNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ImteTypeId", dbobject.ImteTypeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ImteRange", dbobject.ImteRange.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Location", dbobject.Location.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PurchaseYear", dbobject.PurchaseYear.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@LeastCount", dbobject.LeastCount.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@IsInUse", dbobject.IsInUse, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.String));
        }
        //
        private List<ImteMdl> createObjectList(DataSet ds)
        {
            List<ImteMdl> objlist = new List<ImteMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ImteMdl objmdl = new ImteMdl();
                objmdl.ImteId = Convert.ToInt32(dr["ImteId"].ToString());
                objmdl.IdNo = dr["IdNo"].ToString();
                objmdl.ImteTypeId = Convert.ToInt32(dr["ImteTypeId"].ToString());
                if (dr.Table.Columns.Contains("ImteTypeName"))
                {
                    objmdl.ImteTypeName = dr["ImteTypeName"].ToString();//d
                }
                objmdl.ImteRange = dr["ImteRange"].ToString();
                objmdl.Location = dr["Location"].ToString();
                objmdl.PurchaseYear = dr["PurchaseYear"].ToString();
                objmdl.LeastCount = dr["LeastCount"].ToString();
                objmdl.IsInUse = Convert.ToBoolean(dr["IsInUse"].ToString());
                objmdl.InUseStatus = "Deleted";
                if (objmdl.IsInUse == true)
                {
                    objmdl.InUseStatus = "In Use";
                }
                
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string idno)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_imte";
            cmd.Parameters.Add(mc.getPObject("@idno", idno.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.String));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(ImteMdl dbobject)
        {
            if (dbobject.IdNo.Length == 0)
            {
                Message = "Id No not entered!";
                return false;
            }
            if (dbobject.ImteRange == null)
            {
                dbobject.ImteRange = "";
            }
            if (dbobject.Location == null)
            {
                dbobject.Location = "";
            }
            if (dbobject.PurchaseYear == null)
            {
                dbobject.PurchaseYear = "";
            }
            if (dbobject.LeastCount == null)
            {
                dbobject.LeastCount = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(ImteMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.IdNo) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_imte";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string imteid = mc.getRecentIdentityValue(cmd, dbTables.tbl_imte, "imteid");
                mc.setEventLog(cmd, dbTables.tbl_imte, imteid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ImteBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(ImteMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_imte";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@ImteId", dbobject.ImteId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_imte, dbobject.ImteId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_imte") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("ImteBLL", "updateObject", ex.Message);
                }
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteObject(int imteid)
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
                cmd.CommandText = "usp_delete_tbl_imte";
                cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_imte, imteid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                if (ex.Message.Contains("fk_tbl_calibration_tbl_imte") == true)
                {
                    Message = "This record has been used in calibration entry, so it cannot be deleted!";
                }
                else
                {
                    Message = mc.setErrorLog("ImteBLL", "deleteObject", ex.Message);
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
        internal ImteMdl searchObject(int imteid)
        {
            DataSet ds = new DataSet();
            ImteMdl dbobject = new ImteMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_imte";
            cmd.Parameters.Add(mc.getPObject("@imteid", imteid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
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
        internal DataSet getObjectData(int imtetypeid, int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_imte";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ImteMdl> getObjectList(int imtetypeid, int ccode=0)
        {
            DataSet ds = getObjectData(imtetypeid, ccode);
            return createObjectList(ds);
        }
        //
        internal DataSet getImteLocationData(int imtetypeid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_imte_location";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ImteMdl> getImteLocationList(int imtetypeid)
        {
            DataSet ds = getImteLocationData(imtetypeid);
            List<ImteMdl> objlist = new List<ImteMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ImteMdl objmdl = new ImteMdl();
                objmdl.Location = dr["Location"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal List<ImteMdl> getImteSearchList(int imtetypeid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_imte_search_list";
            cmd.Parameters.Add(mc.getPObject("@imtetypeid", imtetypeid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            List<ImteMdl> objlist = new List<ImteMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ImteMdl objmdl = new ImteMdl();
                objmdl.Location = dr["Location"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}