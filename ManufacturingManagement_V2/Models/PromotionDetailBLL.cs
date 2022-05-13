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
    public class PromotionDetailBLL : DbContext
    {
        //
        internal DbSet<PromotionDetailMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, PromotionDetailMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@PromotionDate", dbobject.PromotionDate.ToString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@CategoryId", dbobject.CategoryId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@DesigId", dbobject.DesigId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@RecId", dbobject.RecId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@NewEmpId", dbobject.NewEmpId, DbType.Int32));
        }
        //
        private List<PromotionDetailMdl> createObjectList(DataSet ds)
        {
            List<PromotionDetailMdl> listObj = new List<PromotionDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                PromotionDetailMdl objmdl = new PromotionDetailMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.NewEmpId = Convert.ToInt32(dr["NewEmpId"].ToString());
                objmdl.EmpId = dr["EmpId"].ToString();//d
                objmdl.EmpName = dr["EmpName"].ToString();//d
                objmdl.PromotionDate = Convert.ToDateTime(dr["PromotionDate"].ToString());
                objmdl.CategoryId = Convert.ToInt32(dr["CategoryId"].ToString());
                objmdl.DesigId = Convert.ToInt32(dr["DesigId"].ToString());
                objmdl.CategoryName = mc.getNameByKey(mc.getEmpCategory(), "categoryid", dr["CategoryId"].ToString(), "categoryname");
                objmdl.Designation = mc.getNameByKey(mc.getDesignation(), "desigid", dr["DesigId"].ToString(), "designation");
                listObj.Add(objmdl);
            }
            return listObj;
        }
        //
        internal bool isFoundPromotionDetail(int newempid, DateTime promotiondate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_isfound_promotiondetail";
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@promotiondate", promotiondate.ToShortDateString(), DbType.DateTime));
            if (Convert.ToBoolean(mc.getFromDatabase(cmd)) == true)
            {
                Message = "Duplicate entry for the same date is not allowed!";
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        private bool checkSetValidModel(PromotionDetailMdl dbobject)
        {
            if (dbobject.NewEmpId == 0)
            {
                Message = "Employee not selected!";
                return false;
            }
            if (mc.isValidDate(dbobject.PromotionDate) == false)
            {
                Message = "Invalid promotion date!";
                return false;
            }
            if (dbobject.CategoryId == 0)
            {
                Message = "Invalid category selected!";
                return false;
            }
            EmployeeBLL empBLL = new EmployeeBLL();
            if (empBLL.getEmployeeGradeNewEmpId(dbobject.NewEmpId).ToLower() == "d" && mc.getPermission(Models.Entry.DirectorsInfo, permissionType.Add) == false)
            {
                Message = "Unauthorised access!";
                return false;
            }
            if (isFoundPromotionDetail(dbobject.NewEmpId, dbobject.PromotionDate) == true) { return false; };
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void updatePromotionDetail(PromotionDetailMdl dbobject)
        {
            Result = false;
            if (checkSetValidModel(dbobject) == false) { return; };
            if (mc.isValidDateToSetRecord(dbobject.PromotionDate) == false)
            {
                Message = "[Date check failed] Old record modification is not allowed!";
                return;
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_update_promotiondetail";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_promotiondetail, dbobject.RecId.ToString(), "Updated");
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PromotionDetailBLL", "updatePromotionDetail", ex.Message);
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
                cmd.CommandText = "usp_delete_tbl_promotiondetail";
                cmd.Parameters.Add(mc.getPObject("@recid", recid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_promotiondetail, recid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("PromotionDetailBLL", "deleteObject", ex.Message);
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
        internal PromotionDetailMdl searchObject(int recid)
        {
            DataSet ds = new DataSet();
            PromotionDetailMdl dbobject = new PromotionDetailMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_promotiondetail";
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
        internal DataSet getObjectData(string dtfrom, string dtto, int newempid)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_promotiondetail";
            cmd.Parameters.Add(mc.getPObject("@dtfrom", mc.getStringByDateToStore(dtfrom), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@dtto", mc.getStringByDateToStore(dtto), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@newempid", newempid, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<PromotionDetailMdl> getObjectList(string dtfrom, string dtto, int newempid)
        {
            DataSet ds = getObjectData(dtfrom, dtto, newempid);
            return createObjectList(ds);
        }
        //
        #endregion
        //
    }
}