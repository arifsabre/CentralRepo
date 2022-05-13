using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class PurchaseOrderController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private PurchaseOrderBLL bllObject = new PurchaseOrderBLL();
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

        private void setViewObject()
        {
            var cmplist = compBLL.getObjectList();
            cmplist.Remove(cmplist.Find(s => s.CompCode.Equals(Convert.ToInt32(objCookie.getCompCode()))));
            ViewBag.CompanyList = new SelectList(cmplist, "compcode", "cmpname");
            //
            ViewBag.POTypeListE = new SelectList(getPOTypeList(), "Value", "Text");
            ViewBag.OrderStatusListE = new SelectList(getOrderStatusList(), "Value", "Text");
            ViewBag.MAReasonListE = new SelectList(mc.getMAReasonList(), "Value", "Text");
            ViewBag.EntryTypeListE = new SelectList(getEntryTypeList(), "Value", "Text");
            //
            CompanyAddressBLL compaddr = new CompanyAddressBLL();
            List<CompanyAddressMdl> cmdl = compaddr.getObjectList(Convert.ToInt32(objCookie.getCompCode()));
            ViewBag.CompAddressList = new SelectList(cmdl, "RecId", "CAddress");
            //
            ViewBag.IsAdmin = 0;
            if (objCookie.getLoginType() == 0)
            {
                ViewBag.IsAdmin = 1;
            }
            //
        }

        [HttpGet]
        public ActionResult Index(string potype = "t", int railwayid = 0, int groupid = 0, int itemid = 0, string orderstatus = "x", int marequired = 2, string mareason = "0", int isverified = 2, bool options = false, string railway = "", string group = "", string item = "")
        {
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text", potype);
            ViewBag.OrderStatusList = new SelectList(mc.getOrderStatusList(), "Value", "Text", orderstatus);
            ViewBag.MaReqList = new SelectList(mc.getYesNoList(), "Value", "Text", marequired);
            ViewBag.MAReasonList = new SelectList(mc.getMAReasonList(), "Value", "Text", mareason);
            ViewBag.VerifiedList = new SelectList(mc.getYesNoList(), "Value", "Text", isverified);
            List<PurchaseOrderMdl> modelObject = new List<PurchaseOrderMdl> { };
            if (options == true)
            {
                modelObject = bllObject.getObjectListV2(potype, railwayid, groupid, itemid, orderstatus, marequired, mareason, isverified);
            }
            else
            {
                modelObject = bllObject.getObjectListV2(potype, 0, 0, 0, orderstatus, marequired, "0", isverified);
            }
            ViewBag.lgtype = objCookie.getLoginType();
            //
            ViewBag.railwayid = railwayid;
            ViewBag.railway = railway;
            ViewBag.groupid = groupid;
            ViewBag.group = group;
            ViewBag.itemid = itemid;
            ViewBag.item = item;
            ViewBag.Options = options;
            ViewBag.lgtype = objCookie.getLoginType();//0=admin
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string potype = form["ddlPOType"].ToString();
            string orderstatus = form["ddlOrderStatus"].ToString();
            string mareason = form["ddlMAReason"].ToString();
            string railway = form["txtRailway"].ToString();
            string group = form["txtGroup"].ToString();
            string item = form["txtItem"].ToString();

            int marequired = Convert.ToInt32(form["ddlMARequired"].ToString());
            int isverified = Convert.ToInt32(form["ddlIsVerified"].ToString());

            int railwayid = 0;
            if (form["hfRailwayId"].ToString().Length > 0)
            {
                railwayid = Convert.ToInt32(form["hfRailwayId"].ToString());
            }
            if (railway.Length == 0)
            {
                railwayid = 0;
            }

            int groupid = 0;
            if (form["hfGroupId"].ToString().Length > 0)
            {
                groupid = Convert.ToInt32(form["hfGroupId"].ToString());
            }
            if (group.Length == 0)
            {
                groupid = 0;
            }

            int itemid = 0;
            if (form["hfItemId"].ToString().Length > 0)
            {
                itemid = Convert.ToInt32(form["hfItemId"].ToString());
            }
            if (item.Length == 0)
            {
                itemid = 0;
            }

            bool options = false;
            if (form["chkOptions"] != null)
            {
                options = true;
            }
            return RedirectToAction("Index", new { potype = potype, railwayid = railwayid, groupid = groupid, itemid = itemid, orderstatus = orderstatus, marequired = marequired, mareason = mareason, isverified = isverified, options = options, railway = railway, group = group, item = item });
        }
        internal string getPOInfoString(PurchaseOrderMdl modelObject)
        {
            string info = "";
            if (modelObject.POrderId != 0)
            {
                info = "PO NO: " + modelObject.PONumber;
                info += " Date: " + mc.getStringByDate(modelObject.PODate);
                info += ", Railway: " + modelObject.RailwayName;
            }
            return info;
        }

        [HttpGet]
        public ActionResult GeneratePO(int porderid = 0, string msg = "")
        {
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
            {
                msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Purchase_Order_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            PurchaseOrderMdl modelObject = new PurchaseOrderMdl();
            modelObject = bllObject.searchPurchaseOrder(porderid);
            if (modelObject.POrderId > 0)
            {
                ViewData["AddEdit"] = "Update";
                modelObject.HdnOrderStatus = modelObject.OrderStatus;//note
            }
            else
            {
                modelObject.PODate = DateTime.Now;
                modelObject.ClosureDate = new DateTime(1900, 1, 1);
                modelObject.HdnOrderStatus = "i";//note
            }
            modelObject.POInfo = getPOInfoString(modelObject);
            ViewBag.Message = msg;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult GeneratePO(PurchaseOrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.POrderId == 0)//add mode
            {
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.GeneratePurchaseOrder(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.UpdatePurchaseOrder(modelObject);
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("GeneratePO", new { porderid = modelObject.POrderId, msg = bllObject.Message });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult UpdateCaseClosure(PurchaseOrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.updatePurchaseOrderForCaseClosure(modelObject.POrderId, modelObject.IsCaseClosed, modelObject.ClosureDate);
            if (bllObject.Result == true)
            {
                return RedirectToAction("GeneratePO", new { porderid = modelObject.POrderId, msg = bllObject.Message });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult ResetOrderStatus(PurchaseOrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (objCookie.getLoginType() != 0)//not admin
            {
                return View();
            }
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.resetPOfromHoldCancelStatus(modelObject.POrderId);
            if (bllObject.Result == true)
            {
                return RedirectToAction("GeneratePO", new { porderid = modelObject.POrderId, msg = bllObject.Message });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult VerifyPurchaseOrder(PurchaseOrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.Verify_PO, permissionType.Add) == false)
            {
                return View();
            }
            bllObject.verifyPurchaseOrder(modelObject.POrderId);
            if (bllObject.Result == true)
            {
                return RedirectToAction("GeneratePO", new { porderid = modelObject.POrderId, msg = bllObject.Message });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpGet]
        public ActionResult POChecksheet(int porderid = 0, string msg = "")
        {
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Add) == false)
            {
                msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Purchase_Order_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            PurchaseOrderMdl modelObject = new PurchaseOrderMdl();
            modelObject = bllObject.searchPurchaseOrder(porderid);
            if (modelObject.POrderId > 0)
            {
                ViewData["AddEdit"] = "Update";
            }
            else
            {
                msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Purchase_Order_Entry) + "]";
                return Content(msg);
            }
            modelObject.POInfo = getPOInfoString(modelObject);
            ViewBag.Message = msg;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult POChecksheet(PurchaseOrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.UpdatePOChecksheet(modelObject);
            if (bllObject.Result == true)
            {
                return RedirectToAction("POChecksheet", new { porderid = modelObject.POrderId, msg = bllObject.Message });
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        public List<System.Web.UI.WebControls.ListItem> getPOTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Railway", Value = "t" },
                  new System.Web.UI.WebControls.ListItem { Text = "Private", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Internal", Value = "i" },
                  new System.Web.UI.WebControls.ListItem { Text = "Warranty", Value = "w" }
            };
            return listItems;
        }

        public List<System.Web.UI.WebControls.ListItem> getOrderStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "In Progress", Value = "i" },
                  new System.Web.UI.WebControls.ListItem { Text = "Partially Executed", Value = "p" },
                  new System.Web.UI.WebControls.ListItem { Text = "Executed", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Cancelled", Value = "c" },
                  new System.Web.UI.WebControls.ListItem { Text = "On-Hold", Value = "o" },
                  new System.Web.UI.WebControls.ListItem { Text = "Executed By admin", Value = "a" }
            };
            return listItems;
        }

        public List<System.Web.UI.WebControls.ListItem> getEntryTypeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "PO", Value = "po" },
                  new System.Web.UI.WebControls.ListItem { Text = "AAL", Value = "aal" }
            };
            return listItems;
        }

        [HttpGet]
        public ActionResult GoToEdit(int porderid)
        {
            //until po entry is working in v2
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/PurchaseOrderEntry.aspx?porderid=" + porderid + "" });
        }

        [HttpGet]
        public ActionResult GenerateInvoiceV1(string ponumber)
        {
            //until new links are not used for sale entry
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/SaleEntryGST.aspx?ponumber=" + ponumber + "" });
        }

        [HttpGet]
        public ActionResult DisplayTenderFile(int tenderid)
        {
            return RedirectToAction("DisplayErpV1File", "Home", new { url = "../Report/DisplayControlledDocument.aspx?strvalue=" + tenderid + ".pdf?TenderFile?TenderFile?0?0" });
        }

        [HttpGet]
        public ActionResult PODispatchReport(string ponumber)
        {
            int entryid = Convert.ToInt32(Entry.Marketing_PO_Report);
            return RedirectToAction("DisplayErpV1Report", "Home",
            new
            {
                reporturl = "../RptDetail/OrderExecutionRptDetail.aspx",
                reportpms = "ponumber=" + ponumber,
                entryid = entryid,
                rptname = "Order Execution"
            });
        }

        [HttpGet]
        public ActionResult UploadCaseFile(int porderid, string casefileno)
        {
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/UploadDocCaseFile.aspx?porderid=" + porderid + "*casefileno=" + casefileno + "" });
        }

        [HttpPost]
        public JsonResult UpdateCorrectionRequirementInfo(int porderid, bool crequired, string mareason, string remarks)
        {
            if (mc.getPermission(Entry.Purchase_Order_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new PurchaseOrderBLL();
            bllObject.updateCorrectionRequiredRemarks(porderid, crequired, mareason, remarks);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int porderid)
        {
            if (objCookie.getLoginType() != 0)//not admin
            {
                return new JsonResult { Data = new { status = false, message = "Admin Access Required!", AddEdit = "Update" } };
            }
            if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new PurchaseOrderBLL();
            bllObject.deletePurchaseOrder(porderid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult getLprLqrDetail(int itemid, int ccode = 0)
        {
            bllObject = new PurchaseOrderBLL();
            string info = bllObject.getLprLqrDetailHtml(itemid, ccode);
            return new JsonResult { Data = new { lprlqr = info } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
