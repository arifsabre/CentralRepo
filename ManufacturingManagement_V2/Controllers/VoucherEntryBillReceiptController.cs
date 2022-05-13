using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class VoucherEntryBillReceiptController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private VoucherBLL bllObject = new VoucherBLL();

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
            ViewData["AddEdit"] = "Save";
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DrCrList = new SelectList(mc.getDrCrList(), "Value", "Text");
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            //
            List<VoucherMdl> modelObject = new List<VoucherMdl> { };
            modelObject = bllObject.getVoucherDataList("bpr", mc.getDateByString(dtfrom), mc.getDateByString(dtto));
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto });
        }

        [HttpGet]
        public ActionResult SearchBillNo(int salerecid)
        {
            return RedirectToAction("CreateUpdate", new { id = "0", salerecid = salerecid });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(string id = "0", int salerecid = 0)
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Receipt_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            ViewBag.RecHeadList = new SelectList(bllObject.getAccountHeadsToReceivePayment(), "accode", "acdesc");
            VoucherMdl modelObject = new VoucherMdl();
            modelObject.VDate = mc.getStringByDate(DateTime.Now);
            modelObject.VNo = "0";
            List<VoucherInfoMdl> lstVoucherInfo = new List<VoucherInfoMdl> { };
            modelObject.Info = lstVoucherInfo;
            List<BillOsMdl> lstBillOsInfo = new List<BillOsMdl> { };
            modelObject.BillOsInfo = lstBillOsInfo;
            if (id != "0")
            {
                modelObject = bllObject.SearchVoucher("bpr", id);
                //note: for excess amount [acct head = 67]
                for (int i = 0; i < modelObject.Info.Count; i++)
                {
                    if (modelObject.Info[i].AcCode == 67)
                    {
                        modelObject.Info[i].DrCr = "d";
                        modelObject.Info[i].DrAmount = -(modelObject.Info[i].CrAmount);
                        modelObject.Info[i].CrAmount = 0;
                    }
                }
                //
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            //
            if (salerecid > 0)
            {
                DataSet ds = new DataSet();
                ds = bllObject.getBillInfoBySaleRecId(salerecid);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    string msg = "<a href='#' onclick='javascript:window.close();'";
                    msg += "<h1>Bill-outstanding not found for the selected Invoice!</h1></a>";
                    return Content(msg);
                }
                modelObject.BillAcCode = Convert.ToInt32(ds.Tables[0].Rows[0]["AcCode"].ToString());
                modelObject.PayingAuthId = Convert.ToInt32(ds.Tables[0].Rows[0]["PayingAuthId"].ToString());
                BillOsMdl mdl = new BillOsMdl();
                mdl.BillNo = ds.Tables[0].Rows[0]["BillNo"].ToString();
                mdl.CrAmount = Convert.ToDouble(ds.Tables[0].Rows[0]["Balance"].ToString());
                lstBillOsInfo.Add(mdl);
                modelObject.BillOsInfo = lstBillOsInfo;
                modelObject.setBillInfo = true;
            }
            //
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(VoucherMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new VoucherBLL();
            //note: for excess amount [acct head = 67]
            for (int i = 0; i < modelObject.Info.Count; i++)
            {
                if (modelObject.Info[i].AcCode == 67)
                {
                    modelObject.Info[i].DrCr = "c";
                    modelObject.Info[i].CrAmount = Math.Abs(modelObject.Info[i].DrAmount);
                    modelObject.Info[i].DrAmount = 0;
                }
            }
            //
            if (modelObject.VNo == "0")//add mode
            {
                if (mc.getPermission(Entry.Receipt_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.InsertVoucher("bpr", modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.Receipt_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.UpdateVoucher("bpr", modelObject.VNo, modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        public ActionResult Delete(string vno = "0")
        {
            if (mc.getPermission(Entry.Receipt_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteVoucher("bpr", vno);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
