using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class CorrespondenceBLL : DbContext
    {
        //
        //internal DbSet<CorrespondenceMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static CorrespondenceBLL Instance
        {
            get { return new CorrespondenceBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, CorrespondenceMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.CompCode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", dbobject.FinYear, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DepCode", dbobject.DepCode, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DocumentId", dbobject.DocumentId, DbType.Int32));
            //by procedure cmd.Parameters.Add(mc.getPObject("@Series", dbobject.Series, DbType.String));
            //by procedure cmd.Parameters.Add(mc.getPObject("@CorpNo", dbobject.CorpNo, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@LetterDT", dbobject.LetterDT.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@PartyName", dbobject.PartyName.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CorpAddress", dbobject.CorpAddress.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@ContactPerson", dbobject.ContactPerson.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CorpSubject", dbobject.CorpSubject.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@CorpReference", dbobject.CorpReference.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@Keywords", dbobject.Keywords.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@DocumentLink", dbobject.DocumentLink.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
        }
        //
        private List<CorrespondenceMdl> createObjectList(DataSet ds)
        {
            List<CorrespondenceMdl> listObj = new List<CorrespondenceMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CorrespondenceMdl objmdl = new CorrespondenceMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.CompCode = Convert.ToInt32(dr["CompCode"].ToString());
                objmdl.CmpName = dr["CmpName"].ToString();//d
                objmdl.FinYear = dr["FinYear"].ToString();
                objmdl.DepCode = dr["DepCode"].ToString();
                objmdl.Department = dr["Department"].ToString();//d
                objmdl.DocumentId = Convert.ToInt32(dr["DocumentId"].ToString());
                objmdl.DocumentName = dr["DocumentName"].ToString();//d
                objmdl.Series = dr["Series"].ToString();
                objmdl.CorpNo = Convert.ToInt32(dr["CorpNo"].ToString());
                objmdl.LetterDT = Convert.ToDateTime(dr["LetterDT"].ToString());
                objmdl.PartyName = dr["PartyName"].ToString();
                objmdl.CorpAddress = dr["CorpAddress"].ToString();
                objmdl.ContactPerson = dr["ContactPerson"].ToString();
                objmdl.CorpSubject = dr["CorpSubject"].ToString();
                objmdl.CorpReference = dr["CorpReference"].ToString();
                objmdl.Keywords = dr["Keywords"].ToString();
                objmdl.DocumentLink = dr["DocumentLink"].ToString();
                objmdl.DocumentURL = dr["DocumentURL"].ToString();
                objmdl.LetterNo = dr["LetterNo"].ToString();//d
                objmdl.UserId = Convert.ToInt32(dr["UserId"].ToString());
                objmdl.UserName = dr["UserName"].ToString();
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        private bool checkSetValidModel(CorrespondenceMdl dbobject)
        {
            if (dbobject.Series == null)
            {
                dbobject.Series = "d";
            }
            if (mc.isValidDate(dbobject.LetterDT) == false)
            {
                Message = "Invalid date entered";
                return false;
            }
            if (dbobject.PartyName == null)
            {
                dbobject.PartyName = "";
            }
            if (dbobject.CorpAddress == null)
            {
                dbobject.CorpAddress = "";
            }
            if (dbobject.ContactPerson == null)
            {
                dbobject.ContactPerson = "";
            }
            if (dbobject.CorpSubject == null)
            {
                dbobject.CorpSubject = "";
            }
            if (dbobject.CorpReference == null)
            {
                dbobject.CorpReference = "";
            }
            if (dbobject.Keywords == null)
            {
                dbobject.Keywords = "";
            }
            if (dbobject.DocumentLink == null)
            {
                dbobject.DocumentLink = "";
            }
            return true;
        }
        //
        internal bool isValidToModifyLetter(int recid)
        {
            int lgt = objCookie.getLoginType();
            if (lgt == 0 || lgt == 3) // admin/director
            {
                return true;
            }
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isvalid_to_modify_letter";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            return Convert.ToBoolean(mc.getFromDatabase(cmd));
        }
        //
        internal string getCorrespondenceSeries(int recid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_series";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void insertObject(CorrespondenceMdl dbobject)
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
                cmd.Transaction = trn;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_insert_tbl_correspondence";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.RecId = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.tbl_correspondence, "recid"));
                mc.setEventLog(cmd, dbTables.tbl_correspondence, dbobject.RecId.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Letter Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CorrespondenceBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void updateObject(CorrespondenceMdl dbobject)
        {
            Result = false;
            if (getCorrespondenceSeries(dbobject.RecId).ToLower() != "d")//not draft
            {
                if (objCookie.getLoginType() != 0)//not admin
                {
                    Message = "Letters of Main Series cannot be updated!";
                    return;
                }
            }
            if (isValidToModifyLetter(dbobject.RecId) == false)
            {
                Message = "Invalid attempt!";
                return;
            }
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
                cmd.CommandText = "usp_update_tbl_correspondence";
                addCommandParameters(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@recid", dbobject.RecId, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_correspondence, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Letter Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("CorrespondenceBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void ConvertDraftCorrespondenceToMain(int recid)
        {
            Result = false;
            if (isValidToModifyLetter(recid) == false)
            {
                Message = "Invalid attempt!";
                return;
            }
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
                cmd.CommandText = "usp_convert_draft_correspondence_to_main";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_correspondence, recid.ToString(), "draft correspondence to main");
                trn.Commit();
                Result = true;
                Message = "Draft Converted to Main Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CorrespondenceBLL", "ConvertDraftCorrespondenceToMain", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteCorrespondence(int recid)
        {
            Result = false;
            if (getCorrespondenceSeries(recid).ToLower() != "d")//not draft
            {
                if (objCookie.getLoginType() != 0)//not admin
                {
                    Message = "Letters of Main Series cannot be deleted!";
                    return;
                }
            }
            if (isValidToModifyLetter(recid) == false)
            {
                Message = "Invalid attempt!";
                return;
            }
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
                cmd.CommandText = "usp_delete_tbl_correspondence";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_correspondence, recid.ToString(), "Deleted");
                trn.Commit();
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("CorrespondenceBLL", "deleteCorrespondence", ex.Message);
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
        internal CorrespondenceMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            CorrespondenceMdl dbobject = new CorrespondenceMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_correspondence";
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
        internal string getLetterNoById(int recid)
        {
            DataSet ds = new DataSet();
            CorrespondenceMdl dbobject = new CorrespondenceMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_letterno_by_id";
            cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
            return mc.getFromDatabase(cmd);
        }
        //
        internal DataSet getObjectData(int ccode, string finyr, bool filterbydt, DateTime dtfrom, DateTime dtto, string series, string depcode, int documentid, string searchfield, string searchtext)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_correspondence";
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("@finyear", finyr, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@filterbydt", filterbydt, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtfrom", dtfrom.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@dtto", dtto.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@series", series, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@depcode", depcode.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@documentid", documentid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@SearchField", searchfield.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@SearchText", searchtext.Trim(), DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<CorrespondenceMdl> getObjectList(int ccode, string finyr, bool filterbydt, DateTime dtfrom, DateTime dtto, string series, string depcode, int documentid, string searchfield, string searchtext)
        {
            DataSet ds = getObjectData(ccode, finyr, filterbydt, dtfrom, dtto, series, depcode, documentid, searchfield, searchtext);
            return createObjectList(ds);
        }
        //
        internal DataSet getLetterNoSearchList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_letterno_search_list";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondencePartyList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_party_list";
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceValuesForParty(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_values_for_party";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceAddressList(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_address_list";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceContactPersonList(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_contacperson_list";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceSubjectList(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_subject_list";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceReferenceList(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_reference_list";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getCorrespondenceKeywordsList(string partyname)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_correspondence_keywords_list";
            cmd.Parameters.Add(mc.getPObject("@partyname", partyname, DbType.String));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getPONumbers(int compcode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_ponumbers";//dbp from v1
            cmd.Parameters.Add(mc.getPObject("@compcode", compcode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        #endregion
        //
    }
}