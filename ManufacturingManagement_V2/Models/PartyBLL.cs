using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class PartyBLL : DbContext
    {
        //
        //internal DbSet<PartyMdl> Vendors { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static PartyBLL Instance
        {
            get { return new PartyBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, PartyMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@RailwayId",dbobject.RailwayId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@AcCode", mc.getForSqlIntString(dbobject.AcCode.ToString()), DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@AcDesc", dbobject.AcDesc.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ContPer", dbobject.ContPer.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address1", dbobject.Address1.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address2", dbobject.Address2.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address3", dbobject.Address3.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Address4", dbobject.Address4.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateName", dbobject.StateName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@StateCode", dbobject.StateCode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@TinNo", dbobject.TinNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@GSTinNo", dbobject.GSTinNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@PhoneOff", dbobject.PhoneOff.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@FaxNo", dbobject.FaxNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@MobileNo", dbobject.MobileNo.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Email", dbobject.Email.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
        }
        //
        private List<PartyMdl> createObjectList(DataSet ds)
        {
            List<PartyMdl> objlist = new List<PartyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PartyMdl objmdl = new PartyMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.RailwayId = Convert.ToInt32(dr["RailwayId"].ToString());
                objmdl.AcCode = Convert.ToInt32(dr["AcCode"].ToString());
                objmdl.AcDesc = dr["AcDesc"].ToString();
                objmdl.ContPer = dr["ContPer"].ToString();
                objmdl.Address1 = dr["Address1"].ToString();
                objmdl.Address2 = dr["Address2"].ToString();
                objmdl.Address3 = dr["Address3"].ToString();
                objmdl.Address4 = dr["Address4"].ToString();
                objmdl.StateName = dr["StateName"].ToString();
                objmdl.StateCode = dr["StateCode"].ToString();
                objmdl.TinNo = dr["TinNo"].ToString();
                objmdl.GSTinNo = dr["GSTinNo"].ToString();
                objmdl.PhoneOff = dr["PhoneOff"].ToString();
                objmdl.FaxNo = dr["FaxNo"].ToString();
                objmdl.MobileNo = dr["MobileNo"].ToString();
                objmdl.Email = dr["Email"].ToString();
                objmdl.RailwayName = dr["RailwayName"].ToString();//d
                objmdl.RlyShortName = dr["RlyShortName"].ToString();//d
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        private bool isAlreadyFound(string acdesc)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_party";
            cmd.Parameters.Add(mc.getPObject("@acdesc", acdesc, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
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
        private bool checkSetValidModel(PartyMdl dbobject)
        {
            if (dbobject.ContPer == null)
            {
                dbobject.ContPer = "";
            }
            if (dbobject.Address2 == null)
            {
                dbobject.Address2 = "";
            }
            if (dbobject.Address3 == null)
            {
                dbobject.Address3 = "";
            }
            if (dbobject.Address4 == null)
            {
                dbobject.Address4 = "";
            }
            if (dbobject.StateName == null)
            {
                dbobject.StateName = "";
            }
            if (dbobject.StateCode == null)
            {
                dbobject.StateCode = "";
            }
            if (dbobject.TinNo == null)
            {
                dbobject.TinNo = "";
            }
            if (dbobject.GSTinNo == null)
            {
                dbobject.GSTinNo = "";
            }
            if (dbobject.PhoneOff == null)
            {
                dbobject.PhoneOff = "";
            }
            if (dbobject.FaxNo == null)
            {
                dbobject.FaxNo = "";
            }
            if (dbobject.MobileNo == null)
            {
                dbobject.MobileNo = "";
            }
            if (dbobject.Email == null)
            {
                dbobject.Email = "";
            }
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(PartyMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (isAlreadyFound(dbobject.AcDesc) == true) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_party";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                string recid = mc.getRecentIdentityValue(cmd, dbTables.tbl_party, "recid");
                mc.setEventLog(cmd, dbTables.tbl_party, recid, "Inserted");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PartyBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(PartyMdl dbobject)
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
                cmd.CommandText = "usp_update_tbl_party";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_party, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("uk_tbl_party") == true)
                {
                    Message = "Duplicate entry not allowed!";
                }
                else
                {
                    Message = mc.setErrorLog("PartyBLL", "updateObject", ex.Message);
                }
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
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_delete_tbl_party";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_party, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PartyBLL", "deleteObject", ex.Message);
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
        internal PartyMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            PartyMdl dbobject = new PartyMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_party";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
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
        internal PartyMdl searchObjectByAcCode(int accode)
        {
            DataSet ds = new DataSet();
            PartyMdl dbobject = new PartyMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_party_by_accode";
            cmd.Parameters.Add(mc.getPObject("@accode", accode, DbType.Int64));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
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
        internal DataSet getObjectData(int railwayid=0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_party";
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            if (railwayid > 0)
            {
                cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            }
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PartyMdl> getObjectList()
        {
            DataSet ds = getObjectData();
            return createObjectList(ds);
        }
        //
        internal DataSet getPartySearchListData(int railwayid=0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_party_search_list";
            cmd.Parameters.Add(mc.getPObject("@railwayid", railwayid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PartyMdl> getPartySearchList(int railwayid = 0)
        {
            DataSet ds = new DataSet();
            ds = getPartySearchListData(railwayid);
            List<PartyMdl> objlist = new List<PartyMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PartyMdl objmdl = new PartyMdl();
                objmdl.AcCode = Convert.ToInt32(dr["AcCode"].ToString());
                objmdl.AcDesc = dr["AcDesc"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal DataSet getPartyDataByRailwayType(string rtype = "0")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_party_records";
            cmd.Parameters.Add(mc.getPObject("@rtype", rtype, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}