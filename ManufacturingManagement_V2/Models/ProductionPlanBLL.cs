using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class ProductionPlanBLL : DbContext
    {
        //
        //internal DbSet<ProductionPlanMdl> productions { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static ProductionPlanBLL Instance
        {
            get { return new ProductionPlanBLL(); }
        }
        //
        #region private objects
        //
        private void addCommandParameters(SqlCommand cmd, ProductionPlanMdl dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@PPMonth", dbobject.PPMonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PPYear", dbobject.PPYear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ItemId", dbobject.ItemId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@PrdQty", dbobject.PrdQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@InspQty", dbobject.InspQty, DbType.Double));
            cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
        }
        //
        private List<ProductionPlanMdl> createObjectList(DataSet ds)
        {
            List<ProductionPlanMdl> productions = new List<ProductionPlanMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductionPlanMdl objmdl = new ProductionPlanMdl();
                objmdl.PlanRecId = Convert.ToInt32(dr["PlanRecId"].ToString());
                objmdl.PPMonth = Convert.ToInt32(dr["PPMonth"].ToString());
                objmdl.PPYear = Convert.ToInt32(dr["PPYear"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());//d
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                objmdl.PrdQty = Convert.ToDouble(dr["PrdQty"].ToString());
                objmdl.InspQty = Convert.ToDouble(dr["InspQty"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();//d
                productions.Add(objmdl);
            }
            return productions;
        }
        //
        private bool checkSetValidModel(ProductionPlanMdl dbobject)
        {
            Message = "";
            return true;
        }
        //
        #endregion
        //
        #region dml objects
        //
        internal void setProductionPlan(ProductionPlanMdl dbobject)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = mc.strconn;
            try
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); };
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.Clear();
                cmd.CommandText = "usp_set_productionplan";
                addCommandParameters(cmd, dbobject);
                //
                cmd.Parameters.Add("@PlanRecId", SqlDbType.Int);
                cmd.Parameters["@PlanRecId"].Direction = ParameterDirection.Output;
                //
                cmd.ExecuteNonQuery();
                string planrecid = cmd.Parameters["@PlanRecId"].Value.ToString();
                //
                mc.setEventLog(cmd, dbTables.tbl_productionplan, planrecid, "Saved");
                //
                Result = true;
                Message = "Plan Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("ProductionPlanBLL", "setProductionPlan", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void setPlanForSubAssemblyTotal(int ppmonth, int ppyear)
        {
            Result = false;
            //if (checkSetValidModel(dbobject) == false) { return; };
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
                cmd.CommandText = "usp_set_plan_for_sub_assembly_total";
                cmd.Parameters.Add(mc.getPObject("@PPMonth", ppmonth, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@PPYear", ppyear, DbType.Int32));
                cmd.Parameters.Add(mc.getPObject("@CompCode", objCookie.getCompCode(), DbType.Int16));
                cmd.Parameters.Add(mc.getPObject("@UserId", objCookie.getUserId(), DbType.Int32));
                cmd.ExecuteNonQuery();
                //
                mc.setEventLog(cmd, dbTables.tbl_productionplan, ppmonth.ToString()+"-"+ppyear.ToString()+"-"+objCookie.getCompCode(), "Subassembly Plan");
                //
                trn.Commit();
                Result = true;
                Message = "Subassembly Plan Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("ProductionPlanBLL", "setPlanForSubAssemblyTotal", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
        //
        internal void deleteProductionPlan(int planrecid)
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
                cmd.CommandText = "usp_delete_tbl_productionplan";
                cmd.Parameters.Add(mc.getPObject("@planrecid", planrecid, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.tbl_productionplan, planrecid.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("fk_tbl_productionentry_tbl_productionplan"))
                {
                    Message = "Production entry has been entered for this item. This record can not be deleted!";
                }
                else if (ex.Message.Contains("fk_tbl_inspectionentry_tbl_productionplan"))
                {
                    Message = "Inspection entry has been entered for this item. This record can not be deleted!";
                }
                else
                {
                    Message = mc.setErrorLog("ProductionPlanBLL", "deleteProductionPlan", ex.Message);
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
        internal ProductionPlanMdl searchObject(int planrecid)
        {
            DataSet ds = new DataSet();
            ProductionPlanMdl dbobject = new ProductionPlanMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_search_tbl_productionplan";
            cmd.Parameters.Add(mc.getPObject("@planrecid", planrecid, DbType.Int32));
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
        internal DataSet getObjectData(int ppmonth, int ppyear, int itemid = 0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_tbl_productionplan";
            cmd.Parameters.Add(mc.getPObject("@ppmonth", ppmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ppyear", ppyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ProductionPlanMdl> getObjectList(int ppmonth, int ppyear, int itemid = 0)
        {
            DataSet ds = getObjectData(ppmonth, ppyear, itemid);
            return createObjectList(ds);
        }
        //
        internal DataSet getProductionPlanSearchList(int ppmonth, int ppyear, int ccode)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_searchlist_productionplan";
            cmd.Parameters.Add(mc.getPObject("ppmonth", ppmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("ppyear", ppyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("compcode", ccode, DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal List<ProductionPlanStatusMdl> getProductionPlanStatusList(int ppmonth, int ppyear)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_production_plan_status";
            cmd.Parameters.Add(mc.getPObject("ppmonth", ppmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("ppyear", ppyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("compcode", objCookie.getCompCode(), DbType.Int16));
            cmd.Parameters.Add(mc.getPObject("userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            List<ProductionPlanStatusMdl> objlist = new List<ProductionPlanStatusMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductionPlanStatusMdl objmdl = new ProductionPlanStatusMdl();
                objmdl.RecId = Convert.ToInt32(dr["RecId"].ToString());
                objmdl.PlanRecId = Convert.ToInt32(dr["PlanRecId"].ToString());
                objmdl.PPMonth = Convert.ToInt32(dr["PPMonth"].ToString());
                objmdl.PPYear = Convert.ToInt32(dr["PPYear"].ToString());
                objmdl.ItemId = Convert.ToInt32(dr["ItemId"].ToString());//d
                objmdl.ItemCode = dr["ItemCode"].ToString();//d
                objmdl.ShortName = dr["ShortName"].ToString();//d
                
                objmdl.PrdQty = Convert.ToDouble(dr["PrdQty"].ToString());
                objmdl.InspQty = Convert.ToDouble(dr["InspQty"].ToString());
                objmdl.ProducedQty = Convert.ToDouble(dr["ProducedQty"].ToString());
                objmdl.InspectedQty = Convert.ToDouble(dr["InspectedQty"].ToString());

                objmdl.PrvWipQty = Convert.ToDouble(dr["PrvWipQty"].ToString());
                objmdl.PrvPrdQty = Convert.ToDouble(dr["PrvPrdQty"].ToString());
                objmdl.PrvInspQty = Convert.ToDouble(dr["PrvInspQty"].ToString());
                objmdl.PrvProducedQty = Convert.ToDouble(dr["PrvProducedQty"].ToString());
                objmdl.PrvInspectedQty = Convert.ToDouble(dr["PrvInspectedQty"].ToString());

                objmdl.M1DPQty = Convert.ToDouble(dr["M1DPQty"].ToString());
                objmdl.M2DPQty = Convert.ToDouble(dr["M2DPQty"].ToString());
                objmdl.M3DPQty = Convert.ToDouble(dr["M3DPQty"].ToString());
                objmdl.M4DPQty = Convert.ToDouble(dr["M4DPQty"].ToString());
                objmdl.M5DPQty = Convert.ToDouble(dr["M5DPQty"].ToString());
                objmdl.M6DPQty = Convert.ToDouble(dr["M6DPQty"].ToString());
                objmdl.M7DPQty = Convert.ToDouble(dr["M7DPQty"].ToString());
                objmdl.M8DPQty = Convert.ToDouble(dr["M8DPQty"].ToString());
                objmdl.M9DPQty = Convert.ToDouble(dr["M9DPQty"].ToString());
                objmdl.M10DPQty = Convert.ToDouble(dr["M10DPQty"].ToString());
                objmdl.M11DPQty = Convert.ToDouble(dr["M11DPQty"].ToString());
                objmdl.M12DPQty = Convert.ToDouble(dr["M12DPQty"].ToString());
                objmdl.M13DPQty = Convert.ToDouble(dr["M13DPQty"].ToString());
                objmdl.M14DPQty = Convert.ToDouble(dr["M14DPQty"].ToString());
                objmdl.M15DPQty = Convert.ToDouble(dr["M15DPQty"].ToString());
                objmdl.UnitName = dr["UnitName"].ToString();
                objmdl.IsAutoPlanned = Convert.ToBoolean(dr["IsAutoPlanned"].ToString());
                //
                DateTime dtct = new DateTime(ppyear, ppmonth, 1);
                objmdl.PrvMonth = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(-1).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month1 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", ppmonth.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month2 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(1).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month3 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(2).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month4 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(3).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month5 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(4).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month6 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(5).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month7 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(6).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month8 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(7).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month9 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(8).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month10 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(9).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month11 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(10).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month12 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(11).Month.ToString(), "monthname"),3).ToUpper();
                objmdl.Month13 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(12).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month14 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(13).Month.ToString(), "monthname"), 3).ToUpper();
                objmdl.Month15 = mc.LeftNChars(mc.getNameByKey(mc.getMonths(), "monthid", dtct.AddMonths(14).Month.ToString(), "monthname"), 3).ToUpper();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        internal List<ProductionPlanDPDetailMdl> getProductionPlanDPDetail(int ppmonth, int ppyear, int itemid, int monthfor)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_production_plan_dpdetail";
            cmd.Parameters.Add(mc.getPObject("@ppmonth", ppmonth, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@ppyear", ppyear, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@monthfor", monthfor, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@compcode", objCookie.getCompCode(), DbType.Int16));
            mc.fillFromDatabase(ds, cmd);
            List<ProductionPlanDPDetailMdl> objlist = new List<ProductionPlanDPDetailMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductionPlanDPDetailMdl objmdl = new ProductionPlanDPDetailMdl();
                objmdl.PONumber = dr["PONumber"].ToString();
                objmdl.PODate = mc.getStringByDate(Convert.ToDateTime(dr["PODate"].ToString()));
                objmdl.CaseFileNo = dr["CaseFileNo"].ToString();
                objmdl.PendingQty = Convert.ToDouble(dr["PendingQty"].ToString());
                objmdl.DelvDate = mc.getStringByDate(Convert.ToDateTime(dr["DelvDate"].ToString()));
                objmdl.RailwayName = dr["RailwayName"].ToString();
                objmdl.ConsigneeName = dr["ConsigneeName"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}