using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class TenderController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private TenderBLL bllObject = new TenderBLL();
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
        }

        [HttpGet]
        public ActionResult Index(string dtfrom = "", string dtto = "", int groupid = 0, int itemid = 0, int railwayid = 0, string tenderstatus = "0", int tenderid = 0, bool isretender = false, string railway = "", string group = "", string item = "", string tenderno ="", bool options = false)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject();
            //
            ViewBag.StatusList = new SelectList(bllObject.getTenderStatusList(), "ObjectCode", "ObjectName", tenderstatus);
            List<TenderMdl> modelObject = new List<TenderMdl> { };
            DateTime fromdate = mc.getDateBySqlGenericString(dtfrom);
            DateTime todate = mc.getDateBySqlGenericString(dtto);
            bool filterbydt = true;//note
            if (options == true)
            {
                modelObject = bllObject.getObjectList(fromdate, todate, filterbydt, groupid, itemid, railwayid, tenderstatus, tenderid, isretender);
            }
            else
            {
                modelObject = bllObject.getObjectList(fromdate, todate, filterbydt, 0, 0, 0, tenderstatus, 0, false);
            }
            ViewBag.lgtype = objCookie.getLoginType();
            //
            ViewBag.dtfrom = dtfrom;
            ViewBag.dtto = dtto;
            ViewBag.railwayid = railwayid;
            ViewBag.railway = railway;
            ViewBag.groupid = groupid;
            ViewBag.group = group;
            ViewBag.itemid = itemid;
            ViewBag.item = item;
            ViewBag.Options = options;
            ViewBag.isReTender = isretender;
            ViewBag.tenderNo = tenderno;
            ViewBag.tenderId = tenderid;
            ViewBag.allowDelete = 0;
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == true)
            {
                ViewBag.allowDelete = 1;
            }
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string tenderstatus = form["ddlStatus"].ToString();
            string railway = form["txtRailway"].ToString();
            string group = form["txtGroup"].ToString();
            string item = form["txtItem"].ToString();
            string tenderno = form["txtTenderNo"].ToString();

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

            int tenderid = 0;
            if (form["hfTenderId"].ToString().Length > 0)
            {
                tenderid = Convert.ToInt32(form["hfTenderId"].ToString());
            }
            if (tenderno.Length == 0)
            {
                tenderid = 0;
            }

            bool isretender = false;
            if (form["chkIsReTender"] != null)
            {
                isretender = true;
            }

            bool options = false;
            if (form["chkOptions"] != null)
            {
                options = true;
            }
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, groupid = groupid, itemid = itemid, railwayid = railwayid, tenderstatus = tenderstatus, tenderid = tenderid, isretender = isretender, railway = railway, group = group, item = item, tenderno = tenderno, options = options });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            ViewBag.StatusList = new SelectList(bllObject.getTenderStatusList(), "ObjectCode", "ObjectName");
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            //
            TenderMdl modelObject = new TenderMdl();
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            modelObject = bllObject.searchTender(id);
            ViewBag.OpnTime = DateTime.Now.ToShortTimeString();
            modelObject.LoaInfo = "";
            if (modelObject.TenderId > 0)
            {
                modelObject.LoaInfo = bllObject.getLoaDetail(modelObject.TenderId, modelObject.LoaNumber, modelObject.LoaDateStr, modelObject.AalCo);
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(TenderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new TenderBLL();
            if (modelObject.TenderId == 0)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertTender(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateTender(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        [HttpGet]
        public ActionResult GoToEdit(int tenderid)
        {
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/TenderEntry.aspx?tenderid=" + tenderid + "" });
        }
        
        [HttpGet]
        public ActionResult GoToETenderStep(int step, int tenderid)
        {
            //not in use
            if (step == 1)
            {
                return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/ETenderEntry.aspx?tenderid=" + tenderid + "" });
            }
            else if (step == 2)
            {
                return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/ETenderItemEntry.aspx?tenderid=" + tenderid + "" });
            }
            //otherwise
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/ETenderConsigneeEntry.aspx?tenderid=" + tenderid + "" });
        }

        [HttpGet]
        public ActionResult DisplayTenderFile(int tenderid)
        {
            return RedirectToAction("DisplayErpV1File", "Home", new { url = "../Report/DisplayControlledDocument.aspx?strvalue=" + tenderid + ".pdf?TenderFile?TenderFile?0?0" });
        }

        [HttpGet]
        public ActionResult UploadTenderFile(int tenderid, string tenderno)
        {
            return RedirectToAction("DisplayErpV1", "Home", new { url = "../Entry/UploadDocTenderFile.aspx?tenderid=" + tenderid + "*tenderno=" + tenderno + "" });
        }

        [HttpGet]
        public ActionResult TenderChecksheetReport(int tenderid)
        {
            int entryid = Convert.ToInt32(Entry.Tender_Report);
            return RedirectToAction("DisplayErpV1Report", "Home",
            new
            {
                reporturl = "../RptDetail/TenderChecksheetRptDetail.aspx",
                reportpms = "tenderid=" + tenderid, entryid = entryid,
                rptname = "Tender Checksheet"
            });
        }

        [HttpGet]
        public ActionResult ETFormatReport(int tenderid)
        {
            int entryid = Convert.ToInt32(Entry.Tender_Report);
            return RedirectToAction("DisplayErpV1Report", "Home",
            new
            {
                reporturl = "../RptDetail/ETenderRptDetail.aspx",
                reportpms = "tenderid=" + tenderid, entryid = entryid,
                rptname = "E-Tender Format"
            });
        }

        [HttpPost]
        public JsonResult DeleteTenderItem(int tenderid, int itemid)
        {
            if (mc.getPermission(Entry.Tender_Item, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            if (tenderid == 0)
            {
                return new JsonResult { Data = new { status = true, message = "OK" } };//note
            }
            bllObject = new TenderBLL();
            bllObject.deleteTenderDetailRecord(tenderid,itemid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult Delete(int tenderid)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new TenderBLL();
            bllObject.deleteTender(tenderid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult getTenderDetailJSon(int tenderid)
        {
            TenderMdl tenderMdl = new TenderMdl();
            tenderMdl = bllObject.searchTender(tenderid);
            return new JsonResult 
            { Data = new 
                { 
                    quotationno = tenderMdl.QuotationNo, 
                    quotationdate = tenderMdl.QuotationDate,
                    railwayid = tenderMdl.RailwayId,
                    railwayname = tenderMdl.RailwayName,
                    tcfileno = tenderMdl.TCFileNo
            } 
            };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
