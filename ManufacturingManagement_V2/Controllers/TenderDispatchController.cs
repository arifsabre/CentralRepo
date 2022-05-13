
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class TenderDispatchController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private TenderDispatchBLL bllObject = new TenderDispatchBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int tenderid = 0, string tenderno = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.tenderid = tenderid;
            ViewBag.tenderno = tenderno;
        }

        private HtmlString getTenderInfo(int tenderid)
        {
            DataSet ds = new DataSet();
            TenderBLL tenderBll = new TenderBLL();
            ds = tenderBll.getTenderInformation(tenderid);
            string info = "<table class='indexTable'>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Tender No</b></td>";
            info += "<td class='indexColumn' valign='top'>"+ds.Tables[0].Rows[0]["TenderNo"].ToString()+"</td>";
            info += "<td class='indexColumn' valign='top'><b>Tender Type</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["TenderType"].ToString() + "</td>";
            info += "</tr>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Railway</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["RailwayName"].ToString() + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Delv Schedule</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["DelvSchedule"].ToString() + "</td>";
            info += "</tr>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Opening Date</b></td>";
            info += "<td class='indexColumn' valign='top'>" + mc.getStringByDate(Convert.ToDateTime(ds.Tables[0].Rows[0]["OpeningDate"].ToString())) + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Time</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["OpeningTime"].ToString() + "</td>";
            info += "</tr>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Quotation No</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["QuotationNo"].ToString() + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Quotation Date</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["QuotationDate"].ToString() + "</td>";
            info += "</tr>";
            
            info += "</table>";
            setViewObject(tenderid, ds.Tables[0].Rows[0]["TenderNo"].ToString() + " " + ds.Tables[0].Rows[0]["TenderType"].ToString());
            HtmlString hstr = new HtmlString("<html><body>" + info + "</body></html>");
            return hstr;
        }

        private HtmlString getItemInfo(int itemid,int tenderid)
        {
            DataSet ds = new DataSet();
            TenderBLL tenderBll = new TenderBLL();
            ds = tenderBll.getTenderItemInfo(itemid,tenderid);
            string info = "<table class='indexTable'>";

            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Item Group</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["GroupName"].ToString() + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Item Unit</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["UnitName"].ToString() + "</td>";
            info += "</tr>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Item Code</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["ItemCode"].ToString() + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Short Name</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["ShortName"].ToString() + "</td>";
            info += "</tr>";

            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Item Description</b></td>";
            info += "<td colspan='3' class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["ItemName"].ToString() + "</td>";
            info += "</tr>";
            
            info += "<tr>";
            info += "<td class='indexColumn' valign='top'><b>Tender Qty</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["Qty"].ToString() + "</td>";
            info += "<td class='indexColumn' valign='top'><b>Expected Qty</b></td>";
            info += "<td class='indexColumn' valign='top'>" + ds.Tables[0].Rows[0]["ourqty"].ToString() + "</td>";
            info += "</tr>";
            
            info += "</table>";
            HtmlString hstr = new HtmlString("<html><body>" + info + "</body></html>");
            return hstr;
        }

        private string getItemDetailInfo(int itemid, int tenderid)
        {
            List<TenderDispatchMdl> modelList = new List<TenderDispatchMdl> { };
            modelList = bllObject.getObjectList(itemid, tenderid);
            string info = "<table class='indexTable'>";
            info += "<tr>";
            info += "<td class='indexColumn' style='width:25px;' valign='top'><b>SlNo</b></td>";
            info += "<td class='indexColumn' style='width:80px;' valign='top'><b>Delv Date</b></td>";
            info += "<td class='indexColumn' style='width:80px;' valign='top'><b>Qty</b></td>";
            info += "<td class='indexColumn' style='width:auto;' valign='top'><b>Remarks</b></td>";
            info += "<td class='indexColumn' style='width:60px;' valign='top'><b>Edit</b></td>";
            info += "<td class='indexColumn' style='width:60px;' valign='top'><b>Remove</b></td>";
            info += "</tr>";
            for (int i = 0; i < modelList.Count; i++)
            {
                info += "<tr>";
                info += "<td class='indexColumn' valign='top'>" + (i + 1).ToString() + "</td>";
                info += "<td class='indexColumn' valign='top'>" + mc.getStringByDate(modelList[i].DelvDate) + "</td>";
                info += "<td class='indexColumn' valign='top'>" + modelList[i].Qty + "</td>";
                info += "<td class='indexColumn' valign='top'>" + modelList[i].Remarks + "</td>";
                info += "<td class='indexColumn' valign='top'><a style='color:blue;' href='/TenderDispatch/CreateUpdate?id=" + modelList[i].RecId + "'>Edit</a></td>";
                info += "<td class='indexColumn' valign='top'><a style='color:blue;' href='/TenderDispatch/DeleteEntry?recid=" + modelList[i].RecId + "&tenderid=" + tenderid + "&itemid=" + itemid + "'>Remove</a></td>";
                info += "</tr>";
            }
            info += "</table>";

            //HtmlString hstr = new HtmlString("<html><body>" + info + "</body></html>");
            //return hstr;
            return "<html><body>" + info + "</body></html>";
        }

        // GET: /
        public ActionResult Index(int tenderid = 0, string tenderno="")
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.TenderInfo = "";//to get and set
            setViewObject(tenderid,tenderno);
            List<TenderItemsMdl> modelObject = new List<TenderItemsMdl> { };
            modelObject = bllObject.getTenderItemsList(tenderid);
            if (modelObject.Count > 0)
            {
                //ViewBag.TenderInfo = "<html><body>" + getTenderInfo(tenderid) + "</body></html>";
                ViewBag.TenderInfo = getTenderInfo(tenderid);
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string tenderid = form["hfTenderId"].ToString();
            string tenderno = form["txtTenderNo"].ToString();
            return RedirectToAction("Index", new { tenderid = tenderid, tenderno = tenderno });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int tenderid = 0, int itemid = 0)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            TenderDispatchMdl modelObject = new TenderDispatchMdl();
            modelObject.DelvDate = DateTime.Now;
            bool editmode = false;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                editmode = true;
                tenderid = modelObject.TenderId;
                itemid = modelObject.ItemId;
            }
            setViewObject(tenderid);
            ViewBag.TenderInfo = getTenderInfo(tenderid);
            ViewBag.ItemInfo = getItemInfo(itemid,tenderid);
            modelObject.DetailInfo = getItemDetailInfo(itemid, tenderid);
            if (editmode) { ViewData["AddEdit"] = "Update"; };
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(TenderDispatchMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("CreateUpdate", new {id = 0, tenderid = modelObject.TenderId, itemid = modelObject.ItemId });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpGet]
        public ActionResult DeleteEntry(int recid, int tenderid, int itemid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject = new TenderDispatchBLL();
            bllObject.deleteObject(recid);
            return RedirectToAction("CreateUpdate", new { id = 0, tenderid = tenderid, itemid = itemid});
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
