using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class JobworkReceiptController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private JobworkReceiptBLL bllObject = new JobworkReceiptBLL();
        private JobworkIssueBLL bllJWIssue = new JobworkIssueBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", string opt = "0", string chno = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.JobworkStatusList = new SelectList(mc.getJobworkStatusList(), "Value", "Text", opt);
            ViewBag.challanno = chno;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", string opt = "0", string chno = "")
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto, opt, chno);
            List<JobworkReceiptMdl> modelObject = new List<JobworkReceiptMdl> { };
            modelObject = bllObject.getListPendingJobworksToReceive(dtfrom, dtto, opt, 0, chno).ToList();
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string opt = form["ddlStatus"].ToString();
            string chno = form["txtChallanNo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, opt = opt, chno = chno });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int dispid = 0)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            JobworkReceiptMdl modelObject = new JobworkReceiptMdl();
            modelObject.RecDate = DateTime.Now;
            modelObject.InvDate = DateTime.Now;
            modelObject.DispId = dispid;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                modelObject.RemainingQty = bllObject.getJobworkBalanceQty(dispid);
                ViewData["AddEdit"] = "Update";
            }
            else
            {
                if (id == 0 && dispid != 0)
                {
                    AddIssueDetail(dispid, modelObject);
                }
            }
            return View(modelObject);
        }

        private void AddIssueDetail(int dispid, JobworkReceiptMdl modelObject)
        {
            JobworkIssueMdl mdlissue = new JobworkIssueMdl();
            mdlissue = bllJWIssue.searchObject(dispid);
            if (mdlissue.DispId != 0)
            {
                modelObject.ChallanNo = mdlissue.ChallanNo;
                modelObject.ChallanDateStr = mdlissue.ChallanDateStr;
                modelObject.VendorName = mdlissue.VendorName;
                modelObject.RMItemCode = mdlissue.RMItemCode;
                modelObject.RMShortName = mdlissue.RMShortName;
                modelObject.IssuedQty = mdlissue.IssuedQty;
                modelObject.RMUnitName = mdlissue.RMUnitName;
                modelObject.ProcessDesc = mdlissue.ProcessDesc;
                modelObject.RemainingQty = bllObject.getJobworkBalanceQty(dispid);
            }
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(JobworkReceiptMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            modelObject.WasteQty = 0;//note: not in use
            ViewBag.Message = "Permission Denied!";
            if (modelObject.RecId == 0)//add mode
            {
                if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.JobworkEntry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            ViewBag.Message = bllObject.Message;
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            AddIssueDetail(modelObject.DispId, modelObject);
            return View(modelObject);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            JobworkReceiptMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            if (bllObject.Result == false)
            {
                return Content(bllObject.Message);
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
