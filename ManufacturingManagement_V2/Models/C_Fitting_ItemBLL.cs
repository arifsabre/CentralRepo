using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace ManufacturingManagement_V2.Models
{
    public class C_Fitting_ItemBLL : DbContext
    {
       //
        //internal DbSet<CorrespondenceMdl> dbconObj { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        public static C_Fitting_ItemMDI Instance
        {
            get { return new C_Fitting_ItemMDI(); }
        }
      
        private void addCommandParameters(SqlCommand cmd, C_Fitting_ItemMDI dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@pragno", dbobject.pragno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@invoiceno", dbobject.invoiceno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicedate", dbobject.invoicedate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@solddate", dbobject.solddate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@consigneeId", dbobject.consigneeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@coachno", dbobject.coachno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenoorderagency", dbobject.rakenoorderagency.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenorailway", dbobject.rakenorailway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenouserrailway", dbobject.rakenouserrailway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@shedname", dbobject.shedname.Trim(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@validity", dbobject.validity, DbType.DateTime));
           cmd.Parameters.Add(mc.getPObject("@fittingdate", dbobject.fittingdate, DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@createdby", objCookie.getUserName(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@updatedby", objCookie.getUserName(), DbType.String));
            //cmd.Parameters.Add(mc.getPObject("@updatedon", dbobject.updatedon, DbType.DateTime));
        }

        private void addCommandParameters1(SqlCommand cmd, C_Fitting_ItemMDI dbobject)
        {
            cmd.Parameters.Add(mc.getPObject("@pragno", dbobject.pragno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", dbobject.compcode, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@itemid", dbobject.itemid, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@invoiceno", dbobject.invoiceno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@invoicedate", dbobject.invoicedate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@solddate", dbobject.solddate.ToShortDateString(), DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@consigneeId", dbobject.consigneeId, DbType.Int32));
            cmd.Parameters.Add(mc.getPObject("@coachno", dbobject.coachno, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenoorderagency", dbobject.rakenoorderagency.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenorailway", dbobject.rakenorailway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@rakenouserrailway", dbobject.rakenouserrailway.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@shedname", dbobject.shedname.Trim(), DbType.String));
            cmd.Parameters.Add(mc.getPObject("@validity", dbobject.validity, DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@fittingdate", dbobject.fittingdate, DbType.DateTime));
            cmd.Parameters.Add(mc.getPObject("@updatedby", objCookie.getUserName(), DbType.String));
          

        }

        private List<C_Fitting_ItemMDI> createObjectList(DataSet ds)
        {
            List<C_Fitting_ItemMDI> listObj = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.srno = Convert.ToInt32(dr["srno"].ToString());
                objmdl.pragno = dr["pragno"].ToString();//d
                objmdl.compcode = Convert.ToInt32(dr["compcode"].ToString());
                objmdl.cmpname = dr["cmpname"].ToString();//d
                objmdl.itemid = Convert.ToInt32(dr["itemid"].ToString());
                objmdl.itemname = dr["itemname"].ToString();
                objmdl.consigneeId = Convert.ToInt32(dr["consigneeId"].ToString());
                objmdl.consigneename = dr["consigneename"].ToString();//d
                objmdl.solddate = Convert.ToDateTime(dr["solddate"].ToString());
                objmdl.invoicedate = Convert.ToDateTime(dr["invoicedate"].ToString());
                objmdl.invoiceno = dr["invoiceno"].ToString();//d
                objmdl.coachno = dr["coachno"].ToString();//d
                objmdl.rakenoorderagency = dr["rakenoorderagency"].ToString();
                objmdl.rakenorailway = dr["rakenorailway"].ToString();
                objmdl.rakenouserrailway = dr["rakenouserrailway"].ToString();
                objmdl.shedname = dr["shedname"].ToString();
                objmdl.fittingdate = Convert.ToDateTime(dr["fittingdate"].ToString());
                objmdl.validity = Convert.ToDateTime(dr["validity"].ToString());
                objmdl.createdby = dr["createdby"].ToString();
                objmdl.createdon = Convert.ToDateTime(dr["createdon"].ToString());
                objmdl.updatedby = objCookie.getUserName();
                objmdl.updatedon = Convert.ToDateTime(dr["updatedon"].ToString());
                objmdl.userid = Convert.ToInt32(dr["userid"].ToString());
                listObj.Add(objmdl);
            }
            return listObj;
        }

        
        public List<C_Fitting_ItemMDI> GetAll_FittingItem_Details()
        {
            List<C_Fitting_ItemMDI> tasklist = new List<C_Fitting_ItemMDI>();

            using (SqlConnection conn = new SqlConnection(Appsetting.ConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("c_Item_Fiting_GetAll", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                   // cmd.Parameters.AddWithValue("@userid", objCookie.getUserId());
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                       
                        C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                        objmdl.srno = Convert.ToInt32(dr["srno"].ToString());
                        objmdl.pragno = dr["pragno"].ToString();//d
                        objmdl.compcode = Convert.ToInt32(dr["compcode"].ToString());
                        objmdl.cmpname = dr["cmpname"].ToString();//d
                        objmdl.itemid = Convert.ToInt32(dr["itemid"].ToString());
                        objmdl.itemname = dr["itemname"].ToString();
                        objmdl.solddate = Convert.ToDateTime(dr["solddate"].ToString());
                        objmdl.invoicedate = Convert.ToDateTime(dr["invoicedate"].ToString());
                        objmdl.invoiceno = dr["invoiceno"].ToString();//d
                        objmdl.consigneeId = Convert.ToInt32(dr["consigneeId"].ToString());
                        objmdl.consigneename = dr["consigneename"].ToString();//d
                        objmdl.coachno = dr["coachno"].ToString();//d
                        objmdl.rakenoorderagency = dr["rakenoorderagency"].ToString();
                        objmdl.rakenorailway = dr["rakenorailway"].ToString();
                        objmdl.rakenouserrailway = dr["rakenouserrailway"].ToString();
                        objmdl.shedname = dr["shedname"].ToString();
                        objmdl.fittingdate = Convert.ToDateTime(dr["fittingdate"].ToString());
                        objmdl.validity = Convert.ToDateTime(dr["validity"].ToString());
                        objmdl.createdby = dr["createdby"].ToString();
                        objmdl.createdon = Convert.ToDateTime(dr["createdon"].ToString());
                        objmdl.updatedby = dr["updatedby"].ToString();//d
                        objmdl.updatedon = Convert.ToDateTime(dr["updatedon"].ToString());
                        objmdl.userid = Convert.ToInt32(dr["userid"].ToString());
                        tasklist.Add(objmdl);
                    }
                }
            }
            return tasklist;
        }
        private bool checkSetValidModel(C_Fitting_ItemMDI dbobject)
        {
            //if (mc.isValidDate(dbobject.fittingdate) == false)
            //{
            //    Message = "Invalid date!";
            //    return false;
            //}
            //if (mc.isValidDate1(dbobject.validity) == true)
            //{
            //    Message = "Ok!";
            //    return false;
            //}

            //if (dbobject.pragno == null)
            //{
            //    dbobject.pragno = "";
            //}
            //if (dbobject.cmpname == null)
            //{
            //    dbobject.cmpname = "";
            //}
            //if (dbobject.itemname == null)
            //{
            //    dbobject.itemname = "";
            //}
            //if (dbobject.invoiceno == null)
            //{
            //    dbobject.invoiceno = "";
            //}
            //if (dbobject.shedname == null)
            //{
            //    dbobject.shedname = "";
            //}
            DateTime dtm1 = new DateTime(1, 1, 0001);
            DateTime dtm2 = new DateTime(1, 1, 0001);
            if(dbobject.fittingdate == dtm1 || dbobject.fittingdate == dtm1)
            {
                Message = "Ok!";
                return false;
            }
            if (dbobject.validity == dtm2 || dbobject.validity == dtm2)
            {
                Message = "Ok!";
                return false;
            }

            dbobject.fittingdate = mc.setToValidOptionalDate(dbobject.fittingdate);
            dbobject.validity = mc.setToValidOptionalDate(dbobject.validity);
            return true;
        }
        internal void insertObject(C_Fitting_ItemMDI dbobject)
        {

            Result = false;
           // if (checkSetValidModel(dbobject) == false) { return; };
            //if (checkSetValidModel(dbobject) == true) { return; };
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
                cmd.CommandText = "c_Item_Fiting_Insert";
                addCommandParameters(cmd, dbobject);
                cmd.ExecuteNonQuery();
                dbobject.srno = Convert.ToInt32(mc.getRecentIdentityValue(cmd, dbTables.c_ItemFitting, "srno"));
                mc.setEventLog(cmd, dbTables.c_ItemFitting, dbobject.srno.ToString(), "Inserted");
                trn.Commit();
                Result = true;
                Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                trn.Rollback();
                Message = mc.setErrorLog("C_Fitting_ItemBLL", "insertObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }
   
        internal void updateObject(C_Fitting_ItemMDI dbobject)
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
                cmd.CommandText = "c_Item_Fiting_Update";
                addCommandParameters1(cmd, dbobject);
                cmd.Parameters.Add(mc.getPObject("@srno", dbobject.srno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_ItemFitting, dbobject.srno.ToString(), "Updated");
                Result = true;
                Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_Fitting_ItemBLL", "updateObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }

        internal C_Fitting_ItemMDI searchObject(int srno)
        {
            DataSet ds = new DataSet();
            C_Fitting_ItemMDI dbobject = new C_Fitting_ItemMDI();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "c_Item_Fiting_Get_BySRNo";
            cmd.Parameters.Add(mc.getPObject("@srno", srno, DbType.Int32));
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
        internal List<C_Fitting_ItemMDI> getConsigneeList()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "c_Item_Fiting_GetConsigneeList";
            mc.fillFromDatabase(ds, cmd);
            List<C_Fitting_ItemMDI> depmdl = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.consigneeId = Convert.ToInt32(dr["consigneeId"].ToString());
                objmdl.consigneename = dr["consigneename"].ToString();
                depmdl.Add(objmdl);
            }
            return depmdl;
        }
        internal List<C_Fitting_ItemMDI> getItemList(int compcode=0)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
            cmd.CommandText = "c_Item_Fiting_GetItemList";
            mc.fillFromDatabase(ds, cmd);
            List<C_Fitting_ItemMDI> depmdl = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.itemid = Convert.ToInt32(dr["ItemId"].ToString());
                objmdl.itemname = dr["ItemName"].ToString();
                depmdl.Add(objmdl);
            }
            return depmdl;
        }
        internal List<C_Fitting_ItemMDI> getInvoiceList(int compcode = 0,string finyear="")
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@compcode", objCookie.getCompCode());
            cmd.Parameters.AddWithValue("@finyear", objCookie.getFinYear());
            cmd.CommandText = "[C_Fitting_GetInvoicelist]";
            mc.fillFromDatabase(ds, cmd);
            List<C_Fitting_ItemMDI> depmdl = new List<C_Fitting_ItemMDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                C_Fitting_ItemMDI objmdl = new C_Fitting_ItemMDI();
                objmdl.SaleRecId = Convert.ToInt32(dr["invoiceno"].ToString());
                objmdl.invoiceno = dr["SaleRecId"].ToString();
                depmdl.Add(objmdl); 
            }
            return depmdl;
        }
        internal void deleteObject(int srno)
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
                cmd.CommandText = "C_FittingItem_Delete";
                cmd.Parameters.Add(mc.getPObject("@srno", srno, DbType.Int32));
                cmd.ExecuteNonQuery();
                mc.setEventLog(cmd, dbTables.c_ItemFitting, srno.ToString(), "Deleted");
                Result = true;
                Message = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                Message = mc.setErrorLog("C_Fitting_ItemBLL", "deleteObject", ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); };
            }
        }



    }
}

