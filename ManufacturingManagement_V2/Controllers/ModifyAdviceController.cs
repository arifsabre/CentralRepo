using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ModifyAdviceController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ModifyAdviceBLL bllObject = new ModifyAdviceBLL();
        
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult CreateUpdate(int porderid = 0, int recslno=0, string msg = "")
        {
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
            {
                msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Purchase_Order_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            ViewData["AddEdit"] = "Save";
            ModifyAdviceMdl modelObject = new ModifyAdviceMdl();
            modelObject = bllObject.searchModifyAdvice(porderid, recslno);
            if (modelObject.RecSlNo > 0)
            {
                ViewData["AddEdit"] = "Update";
                modelObject.editRecSlNo = modelObject.RecSlNo;//note
            }
            else
            {
                modelObject.POrderId = porderid;
                modelObject.editRecSlNo = 0;//note
                modelObject.RecSlNo = 1;
                modelObject.ModifyAdvNo = "";
                modelObject.MAFor = "";
            }
            if(porderid > 0)
            {
                DataSet dspo = new DataSet();
                PurchaseOrderBLL poBll = new PurchaseOrderBLL();
                dspo = poBll.getPurchaseOrderInfo(porderid);
                modelObject.TenderId = Convert.ToInt32(dspo.Tables[0].Rows[0]["tenderid"].ToString());
                modelObject.RailwayId = Convert.ToInt32(dspo.Tables[0].Rows[0]["railwayid"].ToString());
                modelObject.POInfo = getPOInfoString(dspo);
                DataSet ds = new DataSet();
                ds = bllObject.getModifyAdviceData(porderid);
                modelObject.MADetailHtml = GetMADetailHtml(ds);
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult CreateUpdate(ModifyAdviceMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.editRecSlNo == 0)//add mode
            {
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.InsertModifyAdvice(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.UpdateModifyAdvice(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("CreateUpdate", new {porderid = modelObject.POrderId, recslno = 0, msg = bllObject.Message });
            }
            DataSet dspo = new DataSet();
            PurchaseOrderBLL poBll = new PurchaseOrderBLL();
            dspo = poBll.getPurchaseOrderInfo(modelObject.POrderId);
            modelObject.TenderId = Convert.ToInt32(dspo.Tables[0].Rows[0]["tenderid"].ToString());
            modelObject.RailwayId = Convert.ToInt32(dspo.Tables[0].Rows[0]["railwayid"].ToString());
            modelObject.POInfo = getPOInfoString(dspo);
            DataSet ds = new DataSet();
            ds = bllObject.getModifyAdviceData(modelObject.POrderId);
            modelObject.MADetailHtml = GetMADetailHtml(ds);
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        internal string getPOInfoString(DataSet ds)
        {
            string info = "";
            if (ds.Tables[0].Rows[0]["PONumber"].ToString().Length != 0)
            {
                info = "PO NO: " + ds.Tables[0].Rows[0]["PONumber"].ToString();
                info += " Date: " + ds.Tables[0].Rows[0]["PODateStr"].ToString();
                info += ", Railway: " + ds.Tables[0].Rows[0]["RailwayName"].ToString();
            }
            return info;
        }

        private string GetMADetailHtml(DataSet ds)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='etItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>MA&nbsp;SlNo</th>");
            sb.Append("<th style='width:250px;'>MA&nbsp;Number</th>");
            sb.Append("<th style='width:auto;'>MA&nbsp;For</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Edit</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Delete</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["recslno"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["ModifyAdvNo"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["MAFor"].ToString() + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' href='/ModifyAdvice/CreateUpdate?porderid=" + ds.Tables[0].Rows[i]["porderid"].ToString() + "&recslno=" + ds.Tables[0].Rows[i]["recslno"].ToString() + "'> Edit </a></td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='deleteMARecord(" + ds.Tables[0].Rows[i]["porderid"].ToString() + "," + ds.Tables[0].Rows[i]["recslno"].ToString() + ")' href='#'> Delete </a></td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        [HttpPost]
        public JsonResult DeleteMARecord(int porderid, int recslno)
        {
            bllObject = new ModifyAdviceBLL();
            bllObject.DeleteModifyAdvice(porderid, recslno);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
