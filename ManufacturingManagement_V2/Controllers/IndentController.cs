using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class IndentController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private IndentBLL bllObject = new IndentBLL();
        private IndentLedgerBLL indentLgrBLL = new IndentLedgerBLL();
        private ItemBLL itemBLL = new ItemBLL();
        private UserBLL userBLL = new UserBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }

        private void setViewObject(string dtfrom = "", string dtto = "")
        {
            //if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            //if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            //ViewBag.DtFrom = dtfrom;
            //ViewBag.DtTo = dtto;
        }

        public ActionResult Index()
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndents();
            return View(modelObject.ToList());
        }

        public ActionResult AllIndentIndex()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getAllIndents();
            return View(modelObject.ToList());
        }

        #region indent creation updation

        // GET: /CreateUpdate
        public ActionResult CreateUpdate()
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.ItemTypeList = new SelectList(mc.getItemTypeList(), "Value", "Text");
            ViewBag.HODUserList = new SelectList(userBLL.getUsersByPermission(2155, Convert.ToInt32(objCookie.getCompCode())), "userid", "fullname");
            //
            IndentMdl modelObject = new IndentMdl();
            ViewBag.VDate = mc.getStringByDate(DateTime.Now);
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(IndentMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new IndentBLL();
            if (mc.getPermission(Entry.IndentEntry, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject.insertIndent(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // GET: /UpdateIndent
        public ActionResult UpdateIndent(int id = 0)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.HODUserList = new SelectList(userBLL.getUsersByPermission(2155, Convert.ToInt32(objCookie.getCompCode())), "userid", "fullname");
            IndentMdl modelObject = new IndentMdl();
            ViewBag.VDate = mc.getStringByDate(DateTime.Now);
            modelObject = bllObject.searchIndent(id);
            if(modelObject.IndentId != 0)
            {
                ViewBag.StrIndentNo = modelObject.StrIndentNo;
            }
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult UpdateIndent(IndentMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.HODUserList = new SelectList(userBLL.getUsersByPermission(2155, Convert.ToInt32(objCookie.getCompCode())), "userid", "fullname",modelObject.HODUserId);
            //if (ModelState.IsValid) { }
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.updateIndent(modelObject);
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            ViewBag.StrIndentNo = modelObject.StrIndentNo;
            return View(modelObject);
        }

        #endregion

        #region indent ledger updation

        // GET:
        public ActionResult LedgerItemUpdation(int recid = 0)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.HODUserList = new SelectList(userBLL.getUsersByPermission(2155, Convert.ToInt32(objCookie.getCompCode())), "userid", "fullname");
            IndentLedgerMdl modelObject = new IndentLedgerMdl();
            ViewBag.VDate = mc.getStringByDate(DateTime.Now);
            modelObject = indentLgrBLL.searchIndentLedger(recid);
            if (modelObject.IndentId != 0)
            {
                ViewBag.StrIndentNo = modelObject.StrIndentNo;
                ViewBag.UnitList = new SelectList(itemBLL.getTransactionUnitList(modelObject.ItemId), "unit", "unitname", modelObject.UnitId);
            }
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult UpdateLedgerItem(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentLedgerByIndentor(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult DeleteLedgerItem(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.deleteIndentItem(modelObject.RecId);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult AddLedgerItem(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.addIndentItem(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        #region indent approval by HOD

        [HttpGet]
        public ActionResult IndentApprovalHOD()
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndentsToApproveByHOD();
            return View(modelObject.ToList());
        }

        [HttpGet]
        public ActionResult IndentsRepliedByHOD()
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndentsRepliedByHOD();
            return View(modelObject.ToList());
        }

        //get
        public ActionResult ApproveIndentByHOD(int recid)
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            //note 2165 = admin approval users
            IndentLedgerMdl modelObject = new IndentLedgerMdl();
            modelObject = indentLgrBLL.searchIndentLedger(recid);
            ViewBag.AdminUserList = new SelectList(userBLL.getUsersByPermission(2165, modelObject.CompCode), "userid", "fullname");
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult ApproveIndentByHOD(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentApprovalHOD(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        // POST: /CancelIndent
        [HttpPost]
        public JsonResult CancelIndent(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentCancelHOD(modelObject.RecId);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult ReplyIndent(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentReplyByHOD(modelObject.RecId, modelObject.HODReply);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult HODRemarksForIndentor(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalHOD, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentForHODRemarks(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        #region indent approval by admin

        [HttpGet]
        public ActionResult IndentApprovalAdmin()
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndentsToApproveByAdmin();
            return View(modelObject.ToList());
        }

        [HttpGet]
        public ActionResult IndentsQueriedByAdmin()
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndentsQueriedByAdmin();
            return View(modelObject.ToList());
        }

        //get
        public ActionResult ApproveIndentByAdmin(int recid)
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            IndentLedgerMdl modelObject = new IndentLedgerMdl();
            modelObject = indentLgrBLL.searchIndentLedger(recid);
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult ApproveIndentByAdmin(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentApprovalAdmin(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult RejectIndent(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentRejectionAdmin(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult QueryIndent(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentApprovalAdmin, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentQueryByAdmin(modelObject.RecId, modelObject.AdminQuery);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        #region indent execution

        [HttpGet]
        public ActionResult IndentExecution()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentLedgerMdl> modelObject = new List<IndentLedgerMdl> { };
            modelObject = bllObject.getIndentsToExecute();
            return View(modelObject.ToList());
        }

        //get
        public ActionResult ExecuteIndent(int recid)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            IndentLedgerMdl modelObject = new IndentLedgerMdl();
            modelObject = indentLgrBLL.searchIndentLedger(recid);
            modelObject.PurchaseMode = "i";
            modelObject.PRequiredQty = modelObject.AppQty;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult ExecuteIndent(IndentLedgerMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentExecution(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, stkrecid = modelObject.StkRecId, vno = modelObject.VNo } };
        }

        #endregion

        #region indent purchsae slip generation

        [HttpGet]
        public ActionResult IndentPurchaseSlipGeneration()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            IndentPurchaseSlipMdl modelObject = new IndentPurchaseSlipMdl();
            modelObject = bllObject.getIndentsToGeneratePurchaseSlip();
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult generateIndentPurchaseSlip(IndentPurchaseSlipMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentForSlipNoNdVendor(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, slipno = modelObject.SlipNo } };
        }

        [HttpGet]
        public ActionResult IndentPurchaseSlipView()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentPurchaseSlipMdl> modelObject = new List<IndentPurchaseSlipMdl> { };
            modelObject = bllObject.getIndentPurchaseSlipToPrint();
            return View(modelObject.ToList());
        }

        #endregion

        #region indent purchase order generation

        [HttpGet]
        public ActionResult IndentPurchaseOrderGeneration()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            IndentPurchaseSlipMdl modelObject = new IndentPurchaseSlipMdl();
            modelObject = bllObject.getIndentsToGeneratePurchaseOrder();
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult generatePurchaseOrder(IndentPurchaseSlipMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updateIndentLedgerForPurchaseOrder(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, slipno = modelObject.SlipNo } };
        }

        #endregion

        #region pending indents to issue

        [HttpGet]
        public ActionResult PendingIndentIssue()
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            List<IndentMdl> modelObject = new List<IndentMdl> { };
            modelObject = bllObject.getIndentsPendingToIssue();
            return View(modelObject.ToList());
        }

        //get
        public ActionResult IssuePendingIndent(int indentid)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            IndentMdl modelObject = new IndentMdl();
            modelObject = bllObject.searchIndent(indentid);
            for (int i = 0; i < modelObject.Ledgers.Count; i++)
            {
                modelObject.Ledgers[i].IssuedQty = modelObject.Ledgers[i].IssueBalance;
            }
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult IssuePendingIndent(IndentMdl modelObject)
        {
            if (mc.getPermission(Entry.IndentExecution, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new IndentBLL();
            bllObject.updatePendingIndentIssueToStock(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, stkrecid = modelObject.StkRecId } };
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            IndentMdl modelObject = bllObject.searchIndent(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.IndentEntry, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteIndent(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
