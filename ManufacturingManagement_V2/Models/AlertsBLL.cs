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
    public class AlertsBLL : DbContext
    {
        //
        //internal DbSet<WorkListMdl> Advances { get; set; }
        //
        internal string Message { get; set; }
        internal bool Result { get; set; }
        //
        clsMyClass mc = new clsMyClass();
        clsCookie objCookie = new clsCookie();
        //
        internal AlertsMdl getAlertListForUser()
        {
            DataSet ds = new DataSet();
            AlertsMdl dbobject = new AlertsMdl();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_alerts_for_user";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                   dbobject.AlertDetail = "";
                }
            }
            //setBirthDayAnniversaryAlert(dbobject);
            //getting values from ContractorIndent/IndexlApprovalDashboard 
            //if (mc.getPermission(ManufacturingManagement_V2.Models.Entry.Civil, permissionType.Add) == true)
            //{
            //    int x = 0;
            //    DataRow dr = ds.Tables[0].NewRow();
            //    //HODApproval
            //    ContractorIndentMDI modelObject = new ContractorIndentMDI { };
            //    ContractorIndentMDI List = new ContractorIndentMDI();
            //    modelObject.Item_List = List.IndentHODApprovalList(Convert.ToInt32(objCookie.getCompCode()));
            //    //ViewBag.HODApproval = modelObject.Item_List.Count.ToString();
            //    x = modelObject.Item_List.Count;
            //    //if (x > 0)
            //    //{
            //        dr = ds.Tables[0].NewRow();
            //        dr["alertfor"] = "HOD Approval";
            //        dr["result"] = "Pending: " + x.ToString();
            //        dr["srcurl"] = "<html><body><a style='color:blue;' href='../Home/Index?deptt=Maintenance&url=IndexHODApproval'><u>View</u></a></body></html>";
            //        ds.Tables[0].Rows.Add(dr);
            //    //}
            //    //FinalApproval
            //    ContractorIndentMDI modelObject1 = new ContractorIndentMDI { };
            //    ContractorIndentMDI List1 = new ContractorIndentMDI();
            //    modelObject1.Item_List = List1.IndentFinalApprovalList();
            //    //ViewBag.FinalApproval = modelObject1.Item_List.Count.ToString();
            //    x = modelObject1.Item_List.Count;
            //    //if (x != null)
            //    //{
            //        dr = ds.Tables[0].NewRow();
            //        dr["alertfor"] = "Final Approval";
            //        dr["result"] = "Pending: " + x.ToString();
            //        dr["srcurl"] = "<html><body><a style='color:blue;' href='../Home/Index?deptt=Maintenance&url=IndexFinalApproval'><u>View</u></a></body></html>";
            //        ds.Tables[0].Rows.Add(dr);
            //    //}
            //    //BothApproved
            //    ContractorIndentMDI modelObject2 = new ContractorIndentMDI { };
            //    ContractorIndentMDI List2 = new ContractorIndentMDI();
            //    modelObject2.Item_List = List2.IndentFinalandHODApprovalList();
            //    //ViewBag.FinalHODApproval = modelObject2.Item_List.Count.ToString();
            //    x = modelObject2.Item_List.Count;
            //    //if (x > 0)
            //    //{
            //        dr = ds.Tables[0].NewRow();
            //        dr["alertfor"] = "Monitor Task";
            //        dr["result"] = "Total: " + x.ToString();
            //        dr["srcurl"] = "<html><body><a style='color:blue;' href='../Home/Index?deptt=Maintenance&url=IndexHODFinalApproval'><u>View</u></a></body></html>";
            //        ds.Tables[0].Rows.Add(dr);
            //    //}
            //}
            //
            dbobject.AlertList = createAlertList(ds);
            return dbobject;
        }
        //
        internal DataSet getWorkListAlert()
        {
            //[100160]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_worklist_alert";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //
        internal DataSet getTenderAlert(bool addlink,int ccode)
        {
            //--[Copied-100027]
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_tender_alert_report";
            cmd.Parameters.Add(mc.getPObject("@addlink", addlink, DbType.String));
            cmd.Parameters.Add(mc.getPObject("@compcode", ccode, DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            return ds;
        }
        //

        #region private objects
        //
        private void setBirthDayAnniversaryAlert(AlertsMdl dbobject)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "usp_get_birthday_anniv_alert_for_user";
            cmd.Parameters.Add(mc.getPObject("@userid", objCookie.getUserId(), DbType.Int32));
            mc.fillFromDatabase(ds, cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToBoolean(ds.Tables[0].Rows[0]["DispAlert"].ToString()) == true)
                    {
                        dbobject.isBirthdayAlert = true;
                        dbobject.BirthdayAnnivMsg = ds.Tables[0].Rows[0]["DispMessage"].ToString();
                    }
                }
            }
        }
        //
        private List<AlertListInfoMdl> createAlertList(DataSet ds)
        {
            List<AlertListInfoMdl> objlist = new List<AlertListInfoMdl> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                AlertListInfoMdl objmdl = new AlertListInfoMdl();
                objmdl.alertfor = dr["alertfor"].ToString();
                objmdl.result = dr["result"].ToString();
                objmdl.srcurl = dr["srcurl"].ToString();
                objlist.Add(objmdl);
            }
            return objlist;
        }
        //
        #endregion
        //
    }
}