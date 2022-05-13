using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class OrderController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private OrderBLL bllObject = new OrderBLL();
        private OrderTypeBLL orderTypeBLL = new OrderTypeBLL();
        private VendorBLL vendorBLL = new VendorBLL();
        private VendorAddressBLL vAddressBLL = new VendorAddressBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index(int vendorid=0, string vendorname = "")
        {
            if (mc.getPermission(Entry.Stores_PO_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.vendorid = vendorid;
            ViewBag.vendorname = vendorname;
            List<OrderMdl> modelObject = new List<OrderMdl> { };
            modelObject = bllObject.getObjectList(vendorid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int vendorid = 0;
            if (form["hfVendorId"].ToString() != null)
            {
                vendorid = Convert.ToInt32(form["hfVendorId"].ToString());
            }
            string vendorname = form["txtVendorName"].ToString();
            if (vendorname.Length == 0) { vendorid = 0; };
            return RedirectToAction("Index", new { vendorid = vendorid, vendorname = vendorname});
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0,int slipno = 0)
        {
            if (mc.getPermission(Entry.OrderEntry_Store, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            ViewData["AddEdit"] = "Save";
            //note
            //ViewBag.ItemList = new SelectList(storeItemBLL.getObjectList(), "itemid", "itemname");
            //ViewBag.VendorList = new SelectList(vendorBLL.getObjectList(), "vendorid", "vendorname");
            //end-note
            ViewBag.OrderTypeList = new SelectList(orderTypeBLL.getObjectList(), "OrderTypeId", "OrderTypeName");
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.CurrencyList = new SelectList(mc.getCurrencyList(), "Value", "Text");
            ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(0), "VendorAddId", "VAddress");
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            //
            OrderMdl modelObject = new OrderMdl();
            modelObject = bllObject.searchObject(id);
            if (modelObject.OrderId > 0)
            {
                ViewData["AddEdit"] = "Update";
                ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(modelObject.VendorId), "VendorAddId", "VAddress",modelObject.VendorAddId);
            }
            //note json to class retrieval
            //IndentPurchaseSlipMdl sm = Newtonsoft.Json.JsonConvert.DeserializeObject<IndentPurchaseSlipMdl>(lgr.ToString());
            if (slipno > 0)
            {
                modelObject = new OrderMdl();
                modelObject = bllObject.getOrderBySlipNo(slipno);
                //from slip
                //modelObject.VendorId = sm.VendorId;
                //modelObject.VendorName = sm.VendorName;
                //set others
                modelObject.OrderNo = 0;
                modelObject.OrderDate = mc.getStringByDate(DateTime.Now);
                modelObject.DelvSchedule = "";
                modelObject.DelvDate = mc.getStringByDate(DateTime.Now);
                modelObject.OrderTypeId = 1;
                modelObject.OrderTypeName = "";
                modelObject.SpecialInst = "";
                modelObject.RefDetail = "";
                modelObject.ItemCategory = "";
                modelObject.RevisionNo = "";
                modelObject.Packing = "";
                modelObject.Excise = "";
                modelObject.SaleTax = "";
                modelObject.TrpMode = "";
                modelObject.Freight = "";
                modelObject.Insurance = "";
                modelObject.DelvPlace = "";
                modelObject.Inspection = "";
                modelObject.PaymentTerms = "";
                modelObject.TDS = "";
                modelObject.NetAmount = 0;
                modelObject.VendorAddId = 1;
                modelObject.OrderAmount = "";
                modelObject.FinYear = objCookie.getFinYear();
                modelObject.Notes = "";
                modelObject.Currency = "Rs.";
                modelObject.IndentIds = "";
                modelObject.IndentNo = "";
                System.Collections.ArrayList arlindentid = new System.Collections.ArrayList();
                for (int i = 0; i < modelObject.Ledgers.Count; i++)
                {
                    //from slip
                    modelObject.Ledgers[i].SlNo = i + 1;
                    if (arlindentid.Contains(modelObject.Ledgers[i].IndentId) == false)
                    {
                        arlindentid.Add(modelObject.Ledgers[i].IndentId);
                    }
                }
                ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(modelObject.VendorId), "VendorAddId", "VAddress", modelObject.VendorAddId);
                string indentids = "";
                for (int i = 0; i < arlindentid.Count; i++)
                {
                    indentids += arlindentid[i].ToString() + ", ";
                }
                modelObject.IndentIds = indentids.Substring(0, indentids.Length - 2);
            }
            //
            modelObject.SaveAsNew = false;
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(OrderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) 
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new OrderBLL();
            if (modelObject.OrderId == 0)//add mode
            {
                if (mc.getPermission(Entry.OrderEntry_Store, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.OrderEntry_Store, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                if (modelObject.SaveAsNew == true)
                {
                    bllObject.insertObject(modelObject);
                }
                else
                {
                    bllObject.updateObject(modelObject);
                }
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save", orderno = modelObject.OrderNo } };
        }

        #region order cancellation

        // GET:
        public ActionResult OrderCancellation(int orderid)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            OrderMdl modelObject = new OrderMdl();
            modelObject = bllObject.searchObject(orderid);
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult CancelOrder(OrderMdl modelObject)
        {
            if (mc.getPermission(Entry.Stores_PO_Report, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            bllObject.updateOrderToCancelled(modelObject.OrderId, modelObject.CancelledOn, modelObject.Reason);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Stores_PO_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            OrderMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.OrderEntry_Store, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
