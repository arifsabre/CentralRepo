using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ETenderItemController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ETenderItemBLL bllObject = new ETenderItemBLL();

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
            ViewBag.EDtypeList = new SelectList(getEDTypeList(), "Value", "Text");
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int tenderid = 0, int itemrecid = 0, string msg="")
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //
            ETenderItemMdl modelObject = new ETenderItemMdl();
            modelObject = bllObject.searchETenderItem(tenderid, itemrecid);
            if (modelObject.ItemRecId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            modelObject.TenderInfo = TenderBLL.Instance.getTenderInfoString(tenderid);
            modelObject.TendetDetailItemsHtml = GetTenderDetailHtml(modelObject);
            modelObject.ETenderItemsHtml = GetETenderItemsHtml(modelObject);
            ViewBag.Message = msg;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ETenderItemMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //
            bllObject = new ETenderItemBLL();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.ItemRecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertETenderItem(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateETenderItem(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            return RedirectToAction("CreateUpdate", new { tenderid = modelObject.TenderId, msg= bllObject.Message });
        }

        private string GetTenderDetailHtml(ETenderItemMdl modelObject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='tdItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Id</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
            sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
            sb.Append("<th style='width:15px;'>Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Rate</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Select</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < modelObject.TendetDetailItems.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + modelObject.TendetDetailItems[i].ItemId + "</td>");
                sb.Append("<td>" + modelObject.TendetDetailItems[i].ItemCode + "</td>");
                sb.Append("<td>" + modelObject.TendetDetailItems[i].ShortName + "</td>");
                sb.Append("<td>" + modelObject.TendetDetailItems[i].UnitName + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.TendetDetailItems[i].Qty + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.TendetDetailItems[i].Rate + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='selectItem(" + modelObject.TendetDetailItems[i].ItemId + ")' href='#'> Select </a></td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private string GetETenderItemsHtml(ETenderItemMdl modelObject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id='etItem' class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Id</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
            sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
            sb.Append("<th style='width:15px;'>Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>BasicRate</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Edit</th>");
            sb.Append("<th style='width:15px;text-align:center;'>Delete</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < modelObject.ETenderItems.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ItemId + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ItemCode + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].ShortName + "</td>");
                sb.Append("<td>" + modelObject.ETenderItems[i].UnitOfMeasurement + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderItems[i].FullTDQty + "</td>");
                sb.Append("<td style='text-align:right;'>" + modelObject.ETenderItems[i].BasicRatePerUnit + "</td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' href='/ETenderItem/CreateUpdate?itemrecid=" + modelObject.ETenderItems[i].ItemRecId + "'> Edit </a></td>");
                sb.Append("<td style='text-align:center;'><a style='color:blue;' onclick='deleteETenderItem(" + modelObject.ETenderItems[i].ItemRecId + ")' href='#'> Delete </a></td>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        public List<System.Web.UI.WebControls.ListItem> getEDTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "GST", Value = "GST" },
                  new System.Web.UI.WebControls.ListItem { Text = "ED Inclusive", Value = "ED Inclusive" },
                  new System.Web.UI.WebControls.ListItem { Text = "Not Applicable", Value = "Not Applicable" },
                  new System.Web.UI.WebControls.ListItem { Text = "NIL", Value = "NIL" },
                  new System.Web.UI.WebControls.ListItem { Text = "Maximum Applicable", Value = "Maximum Applicable" }
            };
            return listItems;
        }

        [HttpPost]
        public JsonResult getTenderDetailItemDesc(int itemid, int tenderid)
        {
            TenderBLL _tenderBll = new TenderBLL();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = _tenderBll.getTenderItemInfo(itemid, tenderid);
            return new JsonResult { Data = new { tdrecid = ds.Tables[0].Rows[0]["recid"].ToString(), itemname = ds.Tables[0].Rows[0]["itemname"].ToString() } };
        }

        [HttpPost]
        public JsonResult DeleteETenderItem(int itemrecid)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new ETenderItemBLL();
            bllObject.deleteETenderItem(itemrecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
