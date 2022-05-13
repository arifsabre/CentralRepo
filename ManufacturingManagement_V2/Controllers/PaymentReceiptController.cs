using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class PaymentReceiptController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private PaymentReceiptBLL bllObject = new PaymentReceiptBLL();
        
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Index(string dtfrom = "", string dtto = "", int payingauthid = 0, int pmtstatus = 2, string payingauthname = "")
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            //
            DateTime fromdate = mc.getDateBySqlGenericString(dtfrom);
            DateTime todate = mc.getDateBySqlGenericString(dtto);
            ViewBag.PmtStatusList = new SelectList(getPmtStatusList(), "Value", "Text", pmtstatus);
            List<PaymentReceiptMdl> modelObject = new List<PaymentReceiptMdl> { };
            modelObject = bllObject.getReceiptList(fromdate, todate, payingauthid, pmtstatus);
            //
            ViewBag.dtfrom = dtfrom;
            ViewBag.dtto = dtto;
            ViewBag.payingauthid = payingauthid;
            ViewBag.payingauthname = payingauthname;
            ViewBag.pmtstatus = pmtstatus;
            //
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            int pmtstatus = Convert.ToInt32(form["ddlPmtStatus"].ToString());
            int payingauthid = 0;
            if (form["hfPayingAuthId"].ToString().Length > 0)
            {
                payingauthid = Convert.ToInt32(form["hfPayingAuthId"].ToString());
            }
            string payingauthname = form["txtPayingAuthority"].ToString();
            if (payingauthname.Length == 0)
            {
                payingauthid = 0;
            }
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, payingauthid = payingauthid, pmtstatus = pmtstatus, payingauthname = payingauthname });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int salerecid = 0, string msg="")
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            PaymentReceiptMdl modelObject = new PaymentReceiptMdl();
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            modelObject = bllObject.searchReceipt(salerecid);
            if (modelObject.SaleRecId > 0)
            {
                DataSet ds = new DataSet();
                ds = bllObject.getSaleInfo(salerecid);
                ViewBag.SaleInfo = getSaleInfoHtml(ds);
            }
            ViewBag.Message = msg;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(PaymentReceiptMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            bllObject = new PaymentReceiptBLL();
            ViewBag.Message = "Permission Denied!";
            ViewData["AddEdit"] = "Update";
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.updateReceipt(modelObject);
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                return RedirectToAction("CreateUpdate", new { salerecid = modelObject.SaleRecId, msg = bllObject.Message});
            }
            if (modelObject.SaleRecId > 0)
            {
                DataSet ds = new DataSet();
                ds = bllObject.getSaleInfo(modelObject.SaleRecId);
                ViewBag.SaleInfo = getSaleInfoHtml(ds);
            }
            return View(modelObject);
        }

        private HtmlString getSaleInfoHtml(DataSet ds)
        {
            string info = "<table class='tblcontainer'>";
            info += "<tr class='firstrow'>";
            info += "<td valign='top'><b>Railway</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["RailwayName"].ToString() + "</td>";
            info += "</tr>";
            info += "<tr>";
            info += "<td valign='top'><b>Paying&nbsp;Authority</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["PayingAuthName"].ToString() + "</td>";
            info += "</tr>";
            info += "<tr class='tblrow'>";
            info += "<td valign='top'><b>Bill&nbsp;No</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            string billinfo = ds.Tables[0].Rows[0]["BillNo"].ToString();
            billinfo += ", Bill Date "+ ds.Tables[0].Rows[0]["VDate"].ToString();
            info += "<td valign='top'>" + billinfo + "</td>";
            info += "</tr>";
            info += "<tr class='tblrow'>";
            info += "<td valign='top'><b>Items</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["ItemQty"].ToString() + "</td>";
            info += "</tr>";
            info += "<tr class='tblrow'>";
            info += "<td valign='top'><b>Consignee</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["ConsigneeName"].ToString() + "</td>";
            info += "</tr>";
            info += "<tr class='tblrow'>";
            info += "<td valign='top'><b>Invoice&nbsp;Amount</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["NetAmount"].ToString() + "</td>";
            info += "</tr>";
            info += "<tr class='tblrow'>";
            info += "<td valign='top'><b>Bill&nbsp;%&nbsp;Terms</b></td>";
            info += "<td valign='top'><b>:</b></td>";
            info += "<td valign='top'>" + ds.Tables[0].Rows[0]["PaymentMode"].ToString() + "</td>";
            info += "</tr>";
            info += "</table>";
            HtmlString hstr = new HtmlString("<html><body>" + info + "</body></html>");
            return hstr;
        }

        public List<System.Web.UI.WebControls.ListItem> getPmtStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "All", Value = "2" },
                  new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "1" },
                  new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "0" }
            };
            return listItems;
        }
        //

        [HttpPost]
        public JsonResult DeleteReceipt(int salerecid)
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new PaymentReceiptBLL();
            bllObject.deleteReceipt(salerecid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
