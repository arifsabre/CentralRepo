using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class LOAUpdationController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UpdateLOABLL bllObject = new UpdateLOABLL();

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
            ViewBag.AalCoList = new SelectList(mc.getAalCoList(), "Value", "Text");
            ViewBag.tenderid = tenderid;
            ViewBag.tenderno = tenderno;
        }

        // GET: /
        public ActionResult Index(int tenderid = 0, string tenderno = "")
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject(tenderid, tenderno);
            List<UpdateLOAMdl> modelObject = new List<UpdateLOAMdl> { };
            modelObject = bllObject.getObjectList(tenderid);
            TenderBLL tenderBll = new TenderBLL();
            if (modelObject.Count > 0)
            {
                modelObject[0].CalcLoaAmount = tenderBll.getCalculatedLoaAmount(tenderid);
                ViewBag.tenderno = modelObject[0].TenderNo;
                ViewBag.aalco = modelObject[0].AalCo;
            }
            else
            {
                //to get tender info only
                BankGuaranteeBLL bgbll = new BankGuaranteeBLL();
                BankGuaranteeMdl bgmdl = new BankGuaranteeMdl();
                bgmdl = bgbll.getTenderDetail(tenderid);
                //
                UpdateLOAMdl objmdl = new UpdateLOAMdl();
                objmdl.RecId = 0;
                objmdl.TenderId = tenderid;
                objmdl.TenderNo = bgmdl.TenderNo;
                ViewBag.tenderno = bgmdl.TenderNo;
                ViewBag.aalco = "n";
                objmdl.ItemCode = "Step-3 Not Completed";
                objmdl.ShortName = "";
                objmdl.ConsigneeName = "";
                objmdl.OfferedQty = 0;
                objmdl.BasicRate = 0;
                objmdl.SaleTaxPer = 0;
                objmdl.LoaQty = 0;
                objmdl.LoaRate = 0;
                objmdl.LoaAmt = 0;
                objmdl.SdBgAmount = bgmdl.SdBgAmount;
                objmdl.LoaNumber = "";
                objmdl.LoaDate = DateTime.Now;
                objmdl.LoaDateStr = mc.getStringByDateDDMMYYYY(DateTime.Now);
                objmdl.CalcLoaAmount = 0;
                objmdl.DelvSchedule = bgmdl.DelvSchedule;
                objmdl.LoaDelvSchedule = bgmdl.LoaDelvSchedule;
                modelObject.Add(objmdl);
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

        [HttpPost]
        public JsonResult updateLoaQtyRate(double qty, double rate, int recid, int tenderid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            setViewData();
            setViewObject();
            UpdateLOAMdl modelObject = new UpdateLOAMdl();
            modelObject.RecId = recid;
            modelObject.LoaQty = qty;
            modelObject.LoaRate = rate;
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject.updateLoaQtyRate(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult updateLoaInformation(string aalco, string loanumber, string loadate, double loaamount, int tenderid, string loadelvscd, string tcfileno)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            setViewData();
            setViewObject();
            UpdateLOAMdl modelObject = new UpdateLOAMdl();
            modelObject.TenderId = tenderid;
            modelObject.AalCo = aalco;
            modelObject.LoaNumber = loanumber;
            modelObject.SdBgAmount = loaamount;
            modelObject.LoaDate = mc.getDateByStringDDMMYYYY(loadate);
            modelObject.LoaDelvSchedule = loadelvscd;
            modelObject.TCFileNo = tcfileno;
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject.updateLoaInformation(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
