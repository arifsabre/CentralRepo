﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class VoucherEntryGeneralPaymentController : Controller
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
            modelObject = bllObject.getVoucherDataList("pt", mc.getDateByString(dtfrom), mc.getDateByString(dtto));
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
            List<VoucherInfoMdl> objlist = new List<VoucherInfoMdl> { };
            modelObject.Info = objlist;
            if (id != "0")
            {
                modelObject = bllObject.SearchVoucher("pt", id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                //remove cr part for cashAccount
		        var st = modelObject.Info.Find(c => c.DrCr == "c");
                modelObject.Info.Remove(st);
                ViewData["AddEdit"] = "Update";
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
            //
            //set cashAccount cr with sum(amt) from dr part
            double cramt = 0;
            for(int i=0; i < modelObject.Info.Count; i++)
            {
                cramt += modelObject.Info[i].DrAmount;
            }
            if (cramt == 0)
            {
                return new JsonResult { Data = new { status = false, message = "Invalid entry!", AddEdit = "Save" } };
            }
            VoucherInfoMdl vimdl = new VoucherInfoMdl();
            vimdl.AcCode = 50;//cash in hand account
            vimdl.CrAmount = cramt;
            vimdl.DrCr = "c";
            vimdl.Narration = "Cash Payment";
            modelObject.Info.Add(vimdl);
            //
            if (modelObject.VNo == "0")//add mode
            {
                if (mc.getPermission(Entry.Voucher_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.InsertVoucher("pt", modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.Voucher_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.UpdateVoucher("pt", modelObject.VNo, modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        public ActionResult Delete(string vno = "0")
        {
            if (mc.getPermission(Entry.Voucher_Entry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteVoucher("pt", vno);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
