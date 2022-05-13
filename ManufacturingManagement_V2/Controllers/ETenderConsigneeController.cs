using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ETenderConsigneeController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ETenderConsigneeBLL bllObject = new ETenderConsigneeBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject()
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.SaletaxTypeList = new SelectList(getSaletaxTypeList(), "Value", "Text");
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int tenderid = 0, int itemrecid = 0, int recid = 0, string msg = "", bool error = false)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //
            ETenderConsigneeMdl modelObject = new ETenderConsigneeMdl();
            modelObject = bllObject.searchETenderConsignee(tenderid, itemrecid, recid);
            if (modelObject.RecId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            //display
            modelObject.TenderInfo = "";
            modelObject.RailwayId = 0;
            if (tenderid != 0)
            {
                TenderBLL bLL = new TenderBLL();
                System.Data.DataSet ds = new System.Data.DataSet();
                ds = bLL.getTenderInformation(tenderid);
                modelObject.TenderInfo = bLL.getTenderInfoString(ds);
                modelObject.RailwayId = Convert.ToInt32(ds.Tables[0].Rows[0]["railwayid"].ToString());
            }
            modelObject.ETendetItemsHtml = GetETenderItemsHtml(modelObject);
            modelObject.ETenderConsigneeHtml = GetETenderConsigneeHtml(modelObject);
            //cl-display
            if (error == true)
            {
                if (Session["recid"] != null)
                {
                    modelObject.RecId = Convert.ToInt32(Session["recid"].ToString());
                    Session.Remove("recid");
                }
                if (Session["consigneeid"] != null)
                {
                    modelObject.ConsigneeId = Convert.ToInt32(Session["consigneeid"].ToString());
                    Session.Remove("consigneeid");
                }
                if (Session["consigneename"] != null)
                {
                    modelObject.ConsigneeName = Session["consigneename"].ToString();
                    Session.Remove("consigneename");
                }
                if (Session["tenderqty"] != null)
                {
                    modelObject.TenderQty = Convert.ToDouble(Session["tenderqty"].ToString());
                    Session.Remove("tenderqty");
                }
                if (Session["offeredqty"] != null)
                {
                    modelObject.OfferedQty = Convert.ToDouble(Session["offeredqty"].ToString());
                    Session.Remove("offeredqty");
                }
                if (Session["saletaxtype"] != null)
                {
                    modelObject.SaleTaxType = Session["saletaxtype"].ToString();
                    Session.Remove("saletaxtype");
                }
                if (Session["saletaxper"] != null)
                {
                    modelObject.SaleTaxPer = Convert.ToDouble(Session["saletaxper"].ToString());
                    Session.Remove("saletaxper");
                }
                if (Session["freight"] != null)
                {
                    modelObject.Freight = Convert.ToDouble(Session["freight"].ToString());
                    Session.Remove("freight");
                }
                if (Session["basicrate"] != null)
                {
                    modelObject.BasicRate = Convert.ToDouble(Session["basicrate"].ToString());
                    Session.Remove("basicrate");
                }
                if (Session["unitrate"] != null)
                {
                    modelObject.UnitRate = Convert.ToDouble(Session["unitrate"].ToString());
                    Session.Remove("unitrate");
                }
                if (Session["totalamount"] != null)
                {
                    modelObject.TotalAmount = Convert.ToDouble(Session["totalamount"].ToString());
                    Session.Remove("totalamount");
                }
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ETenderConsigneeMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //
            bllObject = new ETenderConsigneeBLL();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertETenderConsignee(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateETenderConsignee(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == false)
            {
                Session["recid"] = modelObject.RecId;
                Session["consigneeid"] = modelObject.ConsigneeId;
                Session["consigneename"] = modelObject.ConsigneeName;
                Session["tenderqty"] = modelObject.TenderQty;
                Session["offeredqty"] = modelObject.OfferedQty;
                Session["saletaxtype"] = modelObject.SaleTaxType;
                Session["saletaxper"] = modelObject.SaleTaxPer;
                Session["freight"] = modelObject.Freight;
                Session["basicrate"] = modelObject.BasicRate;
                Session["unitrate"] = modelObject.UnitRate;
                Session["totalamount"] = modelObject.TotalAmount;
                return RedirectToAction("CreateUpdate", new { tenderid = modelObject.TenderId, itemrecid = modelObject.ItemRecId, recid = modelObject.RecId, msg = bllObject.Message, error = true });
            }
            return RedirectToAction("CreateUpdate", new { tenderid = modelObject.TenderId, itemrecid = modelObject.ItemRecId, msg= bllObject.Message });
        }

        private string GetETenderItemsHtml(ETenderConsigneeMdl modelObject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='tdItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;RecId</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
            sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
            sb.Append("<th style='width:15px;'>Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>BasicRate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>EDMaxPerApplicable</th>");
            sb.Append("<th style='width:15px;'>EDType</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Select</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < modelObject.ETenderItems.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ItemRecId + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ItemCode + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ShortName + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].UnitOfMeasurement + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderItems[i].FullTDQty + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderItems[i].BasicRatePerUnit + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderItems[i].EDMaxPerApplicable + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].EDType + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='selectItem(" + (i + 1).ToString() + "," + modelObject.ETenderItems[i].ItemRecId + ")' href='#'> Select </a></td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private string GetETenderConsigneeHtml(ETenderConsigneeMdl modelObject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='etItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Consignee</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TenderQty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>OfferedQty</th>");
            sb.Append("<th style='width:15px;'>Saletax&nbsp;Type</th>");
            sb.Append("<th style='width:15px;text-align:right;'>SaleTax%</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Freight</th>");
            sb.Append("<th style='width:15px;text-align:right;'>BasicRate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>UnitRate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TotalAmount</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Edit</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Delete</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < modelObject.ETenderConsigneeItems.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + modelObject.ETenderConsigneeItems[i].ConsigneeName + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].TenderQty + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].OfferedQty + "</td>");
                sb.Append("<td>" + modelObject.ETenderConsigneeItems[i].SaleTaxType + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].SaleTaxPer + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].Freight + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].BasicRate + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].UnitRate + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderConsigneeItems[i].TotalAmount + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' href='/ETenderConsignee/CreateUpdate?tenderid=" + modelObject.TenderId + "&itemrecid=" + modelObject.ItemRecId + "&recid=" + modelObject.ETenderConsigneeItems[i].RecId + "'> Edit </a></td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='deleteETenderConsignee(" + modelObject.ETenderConsigneeItems[i].RecId + ")' href='#'> Delete </a></td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        public List<System.Web.UI.WebControls.ListItem> getSaletaxTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "LST Inclusive", Value = "LST Inclusive" },
                  new System.Web.UI.WebControls.ListItem { Text = "CST Extra", Value = "CST Extra" },
                  new System.Web.UI.WebControls.ListItem { Text = "VAT Inclusive", Value = "VAT Inclusive" },
                  new System.Web.UI.WebControls.ListItem { Text = "NIL", Value = "NIL" },
                  new System.Web.UI.WebControls.ListItem { Text = "LST Extra", Value = "LST Extra" },
                  new System.Web.UI.WebControls.ListItem { Text = "CST Inclusive", Value = "CST Inclusive" },
                  new System.Web.UI.WebControls.ListItem { Text = "VAT Extra", Value = "VAT Extra" },
                  new System.Web.UI.WebControls.ListItem { Text = "GST Inclusive", Value = "GST Inclusive" },
                  new System.Web.UI.WebControls.ListItem { Text = "GST Extra", Value = "GST Extra" },
                  new System.Web.UI.WebControls.ListItem { Text = "GST NIL", Value = "GST NIL" }
            };
            return listItems;
        }

        [HttpPost]
        public JsonResult getETenderItemDesc(int itemrecid)
        {
            ETenderItemBLL etItemBll = new ETenderItemBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = etItemBll.getETenderItemInfo(itemrecid);
            return new JsonResult 
            { 
                Data = new 
                { 
                    fulltdqty = ds.Tables[0].Rows[0]["fulltdqty"].ToString(), 
                    uom = ds.Tables[0].Rows[0]["UnitOfMeasurement"].ToString(),
                    basicrate = ds.Tables[0].Rows[0]["BasicRatePerUnit"].ToString(),
                    edtype = ds.Tables[0].Rows[0]["EDType"].ToString(),
                    edper = ds.Tables[0].Rows[0]["EDMaxPerApplicable"].ToString(),
                    itemcode = ds.Tables[0].Rows[0]["ItemCode"].ToString(),
                    shortname = ds.Tables[0].Rows[0]["ShortName"].ToString()
                } 
            };
        }

        public JsonResult getETenderConsigneeTableHtml(int itemrecid)
        {
            bllObject = new ETenderConsigneeBLL();
            List<ETenderConsigneeMdl> objmdl = new List<ETenderConsigneeMdl>();
            objmdl = bllObject.getObjectList(itemrecid);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='etItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Consignee</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TenderQty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>OfferedQty</th>");
            sb.Append("<th style='width:15px;'>Saletax&nbsp;Type</th>");
            sb.Append("<th style='width:15px;text-align:right;'>SaleTax%</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Freight</th>");
            sb.Append("<th style='width:15px;text-align:right;'>BasicRate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>UnitRate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>TotalAmount</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Edit</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Delete</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < objmdl.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + objmdl[i].ConsigneeName + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].TenderQty + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].OfferedQty + "</td>");
                sb.Append("<td>" + objmdl[i].SaleTaxType + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].SaleTaxPer + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].Freight + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].BasicRate + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].UnitRate + "</td>");
                sb.Append("<td style='text-align:right;'>" + objmdl[i].TotalAmount + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' href='/ETenderConsignee/CreateUpdate?tenderid=" + objmdl[i].TenderId + "&itemrecid=" + objmdl[i].ItemRecId + "&recid=" + objmdl[i].RecId + "'> Edit </a></td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='deleteETenderConsignee(" + objmdl[i].RecId + ")' href='#'> Delete </a></td>");
            }
            sb.Append("</table>");
            return new JsonResult { Data = new { result = sb.ToString()} };
        }

        [HttpPost]
        public JsonResult DeleteETenderConsignee(int recid)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new ETenderConsigneeBLL();
            bllObject.deleteETenderConsignee(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
