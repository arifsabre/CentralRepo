using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class VoucherEntryBillPaymentController : Controller
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
            if (mc.getPermission(Entry.Voucher_Entry, permissionType.Add) == false)
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
            modelObject = bllObject.getVoucherDataList("vpt", mc.getDateByString(dtfrom), mc.getDateByString(dtto));
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(string id = "0")
        {
            if (mc.getPermission(Entry.Voucher_Entry, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Voucher_Entry) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            VoucherMdl modelObject = new VoucherMdl();
            modelObject.VDate = mc.getStringByDate(DateTime.Now);
            modelObject.VNo = "0";
            List<VoucherInfoMdl> lstVoucherInfo = new List<VoucherInfoMdl> { };
            modelObject.Info = lstVoucherInfo;
            List<BillOsMdl> lstBillOsInfo = new List<BillOsMdl> { };
            modelObject.BillOsInfo = lstBillOsInfo;
            if (id != "0")
            {
                modelObject = bllObject.SearchVoucher("vpt", id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                modelObject.editMode = true;
            }
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
            if (modelObject.VNo == "0")//add mode
            {
                if (mc.getPermission(Entry.Voucher_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.InsertVoucher("vpt", modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.Voucher_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.UpdateVoucher("vpt", modelObject.VNo, modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        public ActionResult Delete(string vno = "0")
        {
            if (mc.getPermission(Entry.Voucher_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteVoucher("vpt", vno);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
