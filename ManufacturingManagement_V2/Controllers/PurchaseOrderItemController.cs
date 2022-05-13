using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class PurchaseOrderItemController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private POrderDetailBLL bllObject = new POrderDetailBLL();
        private ItemBLL itemBLL = new ItemBLL();
        private CompanyBLL compBLL = new CompanyBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult CreateUpdate(int porderid = 0, int itemrecid=0, string msg = "")
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
            POrderDetailMdl modelObject = new POrderDetailMdl();
            modelObject = bllObject.searchPurchaseOrderItem(itemrecid);
            if (modelObject.ItemRecId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            else
            {
                modelObject.POrderId = porderid;
                modelObject.DelvDate = DateTime.Now;
                modelObject.CommDate = DateTime.Now;
                modelObject.DlvStatus = "i";//note
                modelObject.DlvStatusName = "In Progress";//note
                modelObject.ItemRecId = 0;
                modelObject.ItemId = 0;
                modelObject.ItemCode = "";
                modelObject.ShortName = "";
                modelObject.ItemName = "";
                modelObject.ConsigneeId = 0;
                modelObject.ConsigneeName = "";
                modelObject.OrdQty = 0;
                modelObject.DspQty = 0;
                modelObject.Rate = 0;
                modelObject.Amount = 0;
                modelObject.Discount = 0;
                modelObject.ExciseDutyPer = 0;
                modelObject.ExciseDutyAmount = 0;
                modelObject.VATPer = 0;
                modelObject.VATAmount = 0;
                modelObject.SATPer = 0;
                modelObject.SATAmount = 0;
                modelObject.CSTPer = 0;
                modelObject.CSTAmount = 0;
                modelObject.EntryTaxPer = 0;
                modelObject.EntryTaxAmount = 0;
                modelObject.FreightRate = 0;
                modelObject.FreightAmount = 0;
                modelObject.NetAmount = 0;
                modelObject.BillingUnit = "";
                modelObject.BillPO = "";
                modelObject.InspClause = "";
                modelObject.TransitDepot = "";
                modelObject.ModeOfDisp = "";
                modelObject.DelvTerms = "";
                modelObject.IsVerified = false;
                modelObject.Remarks = "";
                modelObject.WayBill = "";
                modelObject.OCE = "";
                modelObject.VerbalCommitment = "";
                modelObject.DelayReason = "";
                modelObject.ActualDP = modelObject.DelvDate;
                modelObject.IsNonGst = false;
                modelObject.IsInclGst = false;
                modelObject.UnitRate = 0;
                modelObject.UnitName = "";
                modelObject.ItemSlNo = 1;
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
                ds = bllObject.getPOrderDetailData(porderid);
                modelObject.POItemsHtml = GetPOItemsHtml(ds);
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult CreateUpdate(POrderDetailMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.ItemRecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.InsertPurchaseOrderItem(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.UpdatePurchaseOrderItem(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("CreateUpdate", new {porderid = modelObject.POrderId, itemrecid = 0, msg = bllObject.Message });
            }
            DataSet dspo = new DataSet();
            PurchaseOrderBLL poBll = new PurchaseOrderBLL();
            dspo = poBll.getPurchaseOrderInfo(modelObject.POrderId);
            modelObject.TenderId = Convert.ToInt32(dspo.Tables[0].Rows[0]["tenderid"].ToString());
            modelObject.RailwayId = Convert.ToInt32(dspo.Tables[0].Rows[0]["railwayid"].ToString());
            modelObject.POInfo = getPOInfoString(dspo);
            DataSet ds = new DataSet();
            ds = bllObject.getPOrderDetailData(modelObject.POrderId);
            modelObject.POItemsHtml = GetPOItemsHtml(ds);
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

        private string GetPOItemsHtml(DataSet ds)
        {
            string dtstr = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='etItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>PO&nbsp;Item<br/>No</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Edit</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Delete</th>");
            sb.Append("<th style='width:15px;'>Group</th>");
            sb.Append("<th style='width:15px;'>Item<br/>Code</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Ord.<br/>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Disp.<br/>Qty</th>");
            sb.Append("<th style='width:15px;'>Billing<br/>Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Discount/<br/>Unit</th>");
            sb.Append("<th style='width:auto;'>Consignee</th>");
            sb.Append("<th style='width:15px;'>Comm.Date<br/>Delv.Date</th>");
            sb.Append("<th style='width:15px;text-align:right;'>SGST%<br/>CGST%</th>");
            sb.Append("<th style='width:15px;text-align:right;'>IGST%</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Freight<br/>Amount</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Net<br/>Amount</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' href='/PurchaseOrderItem/CreateUpdate?porderid=" + ds.Tables[0].Rows[i]["porderid"].ToString() + "&itemrecid=" + ds.Tables[0].Rows[i]["itemrecid"].ToString() + "'> Edit </a></td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='deletePOItem(" + ds.Tables[0].Rows[i]["itemrecid"].ToString() + ")' href='#'> Delete </a></td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["groupname"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["itemcode"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["ordqty"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["dspqty"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["billingunit"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["rate"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["amount"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["discount"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["consigneename"].ToString() + "</td>");
                dtstr = mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["CommDate"].ToString()));
                dtstr += "<br/>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[i]["DelvDate"].ToString()));
                sb.Append("<td>" + dtstr + "</td>");
                dtstr = ds.Tables[0].Rows[i]["vatper"].ToString();
                dtstr += "<br/>" + ds.Tables[0].Rows[i]["satper"].ToString();
                sb.Append("<td style='text-align:right;'>" + dtstr + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["cstper"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["freightamount"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + ds.Tables[0].Rows[i]["NetAmount"].ToString() + "</td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        [HttpPost]
        public JsonResult DeletePOItem(int itemrecid)
        {
            bllObject = new POrderDetailBLL();
            bllObject.DeletePOItem(itemrecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
