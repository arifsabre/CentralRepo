using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ETenderController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ETenderBLL bllObject = new ETenderBLL();
        
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
            ViewData["AddEdit"] = "Save";
            ViewBag.TDocCostList = new SelectList(getTDocCostList(), "Value", "Text");
            ViewBag.EmdCostList = new SelectList(getTDocCostList(), "Value", "Text");
            ViewBag.TdcmpExmpList = new SelectList(getTdcmpExmpList(), "Value", "Text");
            ViewBag.EmdcExmpList = new SelectList(getEmdcExmpList(), "Value", "Text");
            ViewBag.DispModeList = new SelectList(getDispModeList(), "Value", "Text");
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
            //
            ETenderMdl modelObject = new ETenderMdl();
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            modelObject = bllObject.searchETender(id);
            if (modelObject.TenderId > 0)
            {
                ViewData["AddEdit"] = "Update";
                modelObject.toAdd = false;//note
            }
            else //for add mode only
            {
                TenderBLL tenderBLL = new TenderBLL();
                TenderMdl tenderMdl = new TenderMdl();
                tenderMdl = tenderBLL.searchTender(id);
                modelObject.toAdd = true;//note
                modelObject.TenderId = id;//note
                modelObject.TC1ValidFrom = 90;
                modelObject.TC2ValidFrom = 90;
                modelObject.TC1PmtTerms = tenderMdl.TC1PmtTerms;
                modelObject.TC1ModeOfDisp = tenderMdl.ModeOfDisp;
                modelObject.TC1Insp = tenderMdl.InspAuthority;
                modelObject.TC1DelvPeriod = tenderMdl.DelvSchedule;
                modelObject.DelvSchedule = "Starting after ... weeks of receipt of Purchase Order and thereafter full quantity of Tender to be supplied within ... months.";
                modelObject.TC2PmtTerms = "... percent payment against Inspection Certificate and Receipted Challan of Consignee and balance ... percent payment against Receipt Note.";
            }
            modelObject.TenderInfo = TenderBLL.Instance.getTenderInfoString(id);
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ETenderMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            //
            bllObject = new ETenderBLL();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.toAdd == true)//add mode
            {
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertETender(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Tender_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateETender(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }
        public List<System.Web.UI.WebControls.ListItem> getTDocCostList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "n" },
                  new System.Web.UI.WebControls.ListItem { Text = "Manual", Value = "m" },
                  new System.Web.UI.WebControls.ListItem { Text = "Exempted", Value = "e" },
                  new System.Web.UI.WebControls.ListItem { Text = "Online", Value = "o" }
            };
            return listItems;
        }
        public List<System.Web.UI.WebControls.ListItem> getTdcmpExmpList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "N/A" },
                  new System.Web.UI.WebControls.ListItem { Text = "NSIC", Value = "NSIC" },
                  new System.Web.UI.WebControls.ListItem { Text = "DIC", Value = "DIC" }
            };
            return listItems;
        }
        public List<System.Web.UI.WebControls.ListItem> getEmdcExmpList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "N/A", Value = "N/A" },
                  new System.Web.UI.WebControls.ListItem { Text = "RDSO Approval", Value = "RDSO Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "DLW Approval", Value = "DLW Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "CLW Approval", Value = "CLW Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "BLW Approval", Value = "BLW Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "RCF", Value = "RCF" },
                  new System.Web.UI.WebControls.ListItem { Text = "MCF Approval", Value = "MCF Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "ICF Approval", Value = "ICF Approval" },
                  new System.Web.UI.WebControls.ListItem { Text = "DIC", Value = "DIC" },
                  new System.Web.UI.WebControls.ListItem { Text = "UAM", Value = "UAM" }
            };
            return listItems;
        }
        public List<System.Web.UI.WebControls.ListItem> getDispModeList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Rail/Road", Value = "Rail/Road" },
                  new System.Web.UI.WebControls.ListItem { Text = "Road", Value = "Road" },
                  new System.Web.UI.WebControls.ListItem { Text = "Rail", Value = "Rail" }
            };
            return listItems;
        }
        //

        [HttpPost]
        public JsonResult Delete(int tenderid)
        {
            if (mc.getPermission(Entry.Tender_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new ETenderBLL();
            bllObject.deleteETender(tenderid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
