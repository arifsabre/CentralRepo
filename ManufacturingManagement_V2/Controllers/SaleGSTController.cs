using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class SaleGSTController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private SaleBLL bllObject = new SaleBLL();
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

        private void setViewObject(string dtfrom = "", string dtto = "")
        {
            var cmplist = compBLL.getObjectList();
            cmplist.Remove(cmplist.Find(s => s.CompCode.Equals(Convert.ToInt32(objCookie.getCompCode()))));
            ViewBag.CompanyList = new SelectList(cmplist, "compcode", "cmpname");
        }

        [HttpGet]
        public ActionResult GenerateInvoice()
        {
            setViewData();
            return View();
        }

        [HttpPost]
        public ActionResult GenerateInvoice(SaleMdl modelObject)
        {
            string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            baseurl += "SaleGST/GenerateInvoice";
            if (modelObject.POrderId == 0)
            {
                return Content("<a href='" + baseurl + "'><h1>Invalid Purchase Order!</h1></a>");
            }
            PurchaseOrderBLL poBll = new PurchaseOrderBLL();
            PurchaseOrderMdl poMdl = new PurchaseOrderMdl();
            poMdl = poBll.searchPurchaseOrder(modelObject.POrderId);
            if (poMdl.IsPOVerified == false)
            {
                return Content("<a href='" + baseurl + "'><h1>Purchase order is not verified!</h1></a>");
            }
            return RedirectToAction("CreateUpdate", new { id = 0, porderid = modelObject.POrderId });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int porderid=0)
        {
            if (mc.getPermission(Entry.Sales_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.CashCreditList = new SelectList(mc.getCashCreditList(), "Value", "Text");
            ViewBag.SupplyTypeList = new SelectList(mc.getSupplyTypeList(), "Value", "Text");
            ViewBag.DocumentTypeList = new SelectList(mc.getDocumentTypeList(), "Value", "Text");
            ViewBag.InvoiceModeList = new SelectList(mc.getPOSaleInvoiceModeList(), "Value", "Text");
            ViewBag.OtherChgAcctList = new SelectList(bllObject.getOtherChargesAccountList(), "groupid", "groupname");
            //
            PartyBLL partyBll = new PartyBLL();
            ERP_V1_ReportBLL erpV1RptBll = new ERP_V1_ReportBLL();
            //
            CompanyAddressBLL compaddr = new CompanyAddressBLL();
            List<CompanyAddressMdl> cmdl = compaddr.getObjectList(Convert.ToInt32(objCookie.getCompCode()));
            CompanyAddressMdl compaddrmdl = new CompanyAddressMdl();
            compaddrmdl.RecId = 0;
            compaddrmdl.CAddress = "---Select---";
            cmdl.Add(compaddrmdl);
            ViewBag.CompAddress = new SelectList(cmdl, "recid", "caddress");
            //admin-options
            ViewBag.AllowEdit = "1";
            ViewBag.AllowDisableGST = "0";
            if (objCookie.getLoginType() == 0)//is admin
            {
                ViewBag.AllowDisableGST = "1";
            }
            //
            SaleMdl modelObject = new SaleMdl();
            modelObject = bllObject.searchObject(id);
            //
            if (modelObject.SaleRecId > 0)
            {
                ViewData["AddEdit"] = "Update";
                if (modelObject.InvoiceMode.ToLower() == "slr")
                {
                    //must not be sale return
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Attempt!</h1></a>");
                }
                if (!(modelObject.InvSeries.ToLower() == "d" || modelObject.InvSeries.ToLower() == "p"))
                {
                    if (modelObject.VType.ToLower() != "gi")
                    {
                        //must not be other than po-sale
                        return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid Attempt!</h1></a>");
                    }
                }
                if (modelObject.InvoiceMode.ToLower() == "exp")
                {
                    if (modelObject.Remarks.ToLower().Contains("on payment of igst") == true)
                    {
                        modelObject.ExportOpt1 = true;
                    }
                    else if (modelObject.Remarks.ToLower().Contains("under bond without payment") == true)
                    {
                        modelObject.ExportOpt2 = true;
                    }
                }
                ViewBag.PartyList = new SelectList(partyBll.getPartySearchList(modelObject.RailwayId), "accode", "acdesc");
                ViewBag.PayingAuthList = new SelectList(erpV1RptBll.getPayingAuthorityList(modelObject.RailwayId), "accode", "acdesc");
                ViewBag.ConsigneeList = new SelectList(erpV1RptBll.getConsigneeList(modelObject.RailwayId), "accode", "acdesc");
                modelObject.ActionMode = "update";
                //admin-options
                if (objCookie.getLoginType() != 0 && modelObject.InvSeries.ToLower() == "m")//is not admin
                {
                    DateTime cdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    if (mc.getDateByString(modelObject.VDate).AddDays(1) < cdate)
                    {
                        ViewBag.AllowEdit = "0";
                    }
                }
                //
                if (modelObject.POrderId > 0)
                {
                    PurchaseOrderBLL poBll = new PurchaseOrderBLL();
                    PurchaseOrderMdl poMdl = new PurchaseOrderMdl();
                    poMdl = poBll.searchPurchaseOrder(modelObject.POrderId);
                    ViewBag.POInfo = "Dt." + poMdl.PODate + " " + poMdl.POTypeName + " " + poMdl.RailwayName;
                }
            }
            else
            {
                modelObject.VDate = mc.getStringByDate(DateTime.Now);
                if (porderid != 0)
                {
                    PurchaseOrderBLL poBll = new PurchaseOrderBLL();
                    PurchaseOrderMdl poMdl = new PurchaseOrderMdl();
                    poMdl = poBll.searchPurchaseOrder(porderid);
                    if (poMdl.POrderId > 0)
                    {
                        modelObject.PayingAuthName = poMdl.PayingAuthName;
                        modelObject.ConsigneeName = poMdl.Ledgers[0].ConsigneeName;
                        modelObject.Party = poMdl.AcDesc;
                        modelObject.VNo = 0;
                        modelObject.ChInv = "i";
                        modelObject.InvSeries = "D";
                        modelObject.SaleRecId = 0;
                        modelObject.SubTotal = 0;
                        modelObject.VATAmount = 0;
                        modelObject.SATAmount = 0;
                        modelObject.CSTAmount = 0;
                        modelObject.FreightAmount = 0;
                        modelObject.AdjAmount = 0;
                        modelObject.OtherCharges = 0;
                        modelObject.Discount = 0;
                        modelObject.TaxableAmount = 0;
                        modelObject.NetAmount = 0;
                        modelObject.GRNo = "";
                        modelObject.GRDate = modelObject.VDate;
                        modelObject.TrpMode = "N/A";
                        modelObject.TrpDetail = "";
                        modelObject.POrderId = porderid;
                        modelObject.PONumber = poMdl.PONumber;
                        modelObject.MANo = poBll.getModifyAdvNoInformation(porderid);
                        modelObject.ElctRefNo = "";
                        modelObject.ElctRefDate = modelObject.VDate;
                        modelObject.RevChgTaxAmount = 0;
                        modelObject.PackingDetail = "";
                        modelObject.InvNote = "";
                        modelObject.Remarks = "";
                        modelObject.InvReferenceNo = "";
                        modelObject.PrecDocNo = "";
                        modelObject.PrecDocDate = modelObject.VDate;
                        modelObject.SupTypeCode = "N/A";
                        modelObject.SupAddressId = 0;
                        modelObject.OtherChgAcCode = 39;
                        modelObject.OtherChgPer = 0.075;
                        ViewBag.PartyList = new SelectList(partyBll.getPartySearchList(poMdl.RailwayId), "accode", "acdesc");
                        modelObject.AcCode = poMdl.AcCode;
                        ViewBag.PayingAuthList = new SelectList(erpV1RptBll.getPayingAuthorityList(poMdl.RailwayId), "accode", "acdesc");
                        modelObject.PayingAuthId = poMdl.PayingAuthId;
                        ViewBag.ConsigneeList = new SelectList(erpV1RptBll.getConsigneeList(poMdl.RailwayId), "accode", "acdesc");
                        modelObject.ConsigneeId = poMdl.Ledgers[0].ConsigneeId;
                        //
                        List<SaleLedgerMdl> podetail = new List<SaleLedgerMdl>();
                        POrderDetailBLL poDetailBll = new POrderDetailBLL();
                        podetail = poDetailBll.getItemLedgerListForSale(porderid);
                        modelObject.Ledgers = podetail;
                        //
                        ViewBag.POInfo = "Dt." + poMdl.PODate + " " + poMdl.POTypeName + " " + poMdl.RailwayName;
                    }
                }
                modelObject.ActionMode = "addnew";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(SaleMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            
            bllObject = new SaleBLL();
            //defaults in context of sale-entry
            modelObject.RetSaleRecId = 0;//note

            //
            if (modelObject.ActionMode == "draft" || modelObject.ActionMode == "proforma")
            {
                modelObject.InvSeries = modelObject.ActionMode.Substring(0, 1);
                if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
                }
                bllObject.generateDraftProformaInvoice(modelObject);
            }
            else if (modelObject.ActionMode == "intposaleinv")
            {
                if (mc.getPermission(Entry.Internal_PO_Sale_Invoice, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
                }
                modelObject.InvSeries = "m";//note -assigned in bll method
                PurchaseOrderBLL poBll = new PurchaseOrderBLL();
                PurchaseOrderMdl poMdl = new PurchaseOrderMdl();
                poMdl = poBll.searchPurchaseOrder(modelObject.POrderId);
                if (poMdl.POType.ToLower() != "i")//re-check if not internal
                {
                    return new JsonResult { Data = new { status = false, message = "Only Internal PO's are allowed for direct sale entry!" } };
                }
                bllObject.generateMainInvoice(modelObject);
            }
            else if (modelObject.ActionMode == "convert")
            {
                if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
                }
                //note: based on modelObject.InvSeries
                bllObject.convertDraftProformaToMainInvoice(modelObject.SaleRecId, modelObject);
            }
            else if (modelObject.ActionMode == "update")
            {
                if (modelObject.InvSeries.ToLower() == "d" || modelObject.InvSeries.ToLower() == "p")
                {
                    if (mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == false)
                    {
                        return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
                    }
                }
                else if (modelObject.InvSeries.ToLower() == "m")
                {
                    if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Edit) == false)
                    {
                        return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
                    }
                }
                bllObject.updateSaleEntry(modelObject);
            }

            //
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult UpdateMANumber(int salerecid, string mano)
        {
            bool pass = false;
            if (mc.getPermission(Entry.Sale_Invoice_Main, permissionType.Edit) == true || mc.getPermission(Entry.Draft_Proforma_Invoice, permissionType.Edit) == true)
            {
                pass = true;
            }
            if (pass == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new SaleBLL();
            bllObject.updateMANo(salerecid, mano);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult getConsigneeDetail(int consigneeid)
        {
            DataSet ds = new DataSet();
            ds = bllObject.GetConsigneeDetail(consigneeid);
            return new JsonResult
            {
                Data = new
                {
                    Address2 = ds.Tables[0].Rows[0]["address2"].ToString(),
                    Address3 = ds.Tables[0].Rows[0]["address3"].ToString(),
                    Address4 = ds.Tables[0].Rows[0]["address4"].ToString(),
                    StateCode = ds.Tables[0].Rows[0]["statecode"].ToString(),
                    StateName = ds.Tables[0].Rows[0]["statename"].ToString(),
                    GSTinNo = ds.Tables[0].Rows[0]["GSTinNo"].ToString()
                }
            };
        }

        [HttpPost]
        public JsonResult getMANumberInfo(int porderid)
        {
            PurchaseOrderBLL bll = new PurchaseOrderBLL();
            string mainfo = bll.getModifyAdvNoInformation(porderid);
            return new JsonResult {Data = new { mainfo = mainfo } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
