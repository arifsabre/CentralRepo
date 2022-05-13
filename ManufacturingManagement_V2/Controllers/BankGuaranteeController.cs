using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class BankGuaranteeController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private BankGuaranteeBLL bllObject = new BankGuaranteeBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", int statusid = 0)
        {
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.BGStatusList = new SelectList(bllObject.getBGStatusList(), "GroupId", "GroupName", statusid);
            ViewBag.BGTypeList = new SelectList(mc.getBGTypeList(), "Value", "Text");
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.StatusId = statusid;
        }

        // GET: /
        public ActionResult Index(string dtfrom = "", string dtto = "", int statusid = 0)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Update_BG) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            setViewObject(dtfrom, dtto, statusid);
            List<BankGuaranteeMdl> modelObject = new List<BankGuaranteeMdl> { };
            modelObject = bllObject.getObjectList(statusid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            //string dtfrom = form["txtDtFrom"].ToString();
            //string dtto = form["txtDtTo"].ToString();
            string statusid = form["ddlBGStatus"].ToString();
            return RedirectToAction("Index", new { statusid = statusid });
        }

        public PartialViewResult BGLinks()
        {
            return PartialView();
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Update_BG) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            BankGuaranteeMdl modelObject = new BankGuaranteeMdl();
            modelObject.BGDate = DateTime.Now;
            modelObject.Validity = DateTime.Now;
            modelObject.ExtValidity = DateTime.Now;
            modelObject.LoaDetail = "";
            modelObject.editMode = false;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);//search existing
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                if (modelObject.TenderId == 0)//get tender detail to enter
                {
                    modelObject = bllObject.getTenderDetail(id);
                }
                else
                {
                    ViewData["AddEdit"] = "Update";
                    modelObject.editTenderId = id;
                    modelObject.editMode = true;
                }
            }
            modelObject.sendToHistory = false;//note
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(BankGuaranteeMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.editMode == false)//add mode
            {
                if (mc.getPermission(Entry.Update_BG, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.Update_BG, permissionType.Edit) == false)
                {
                    return View();
                }
                if (modelObject.TenderId == modelObject.editTenderId)
                {
                    bllObject.updateObject(modelObject);
                }
                else
                {
                    bllObject.Message = "Tender No is Not Editable!";
                }
            }
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        #region bg history

        // GET: /
        public ActionResult BGHistory(int tenderid)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Add) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<span style='font-size:18pt;'>Permission Denied!</span></a>";
                msg += "<br/><br/>[" + Convert.ToInt32(Entry.Update_BG) + "]";
                return Content("<html><body style='font-family:verdana;'>" + msg + "</body></html>");
            }
            setViewData();
            List<BankGuaranteeMdl> modelObject = new List<BankGuaranteeMdl> { };
            modelObject = bllObject.getBGHistoryList(tenderid);
            return View(modelObject.ToList());
        }

        // GET: /CreateUpdate
        public ActionResult EditBGHistory(int tenderid, string bgnumber)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Update_BG) + "]";
                return Content(msg);
            }
            setViewData();
            setViewObject();
            BankGuaranteeMdl modelObject = new BankGuaranteeMdl();
            if (!(tenderid == 0 || bgnumber.Length == 0))
            {
                modelObject = bllObject.searchBGHistory(tenderid,bgnumber);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult EditBGHistory(BankGuaranteeMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (mc.getPermission(Entry.Update_BG, permissionType.Edit) == false)
            {
                return View();
            }
            bllObject.updateBGHistory(modelObject);
            if (bllObject.Result == true)
            {
                return RedirectToAction("BGHistory", new {tenderid = modelObject.TenderId});
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult DeleteBGHistory(int tenderid, string bgnumber, bool act=false)
        {
            if (act == false)
            {
                return new JsonResult { Data = new { status = false, message = "" } };
            }
            if (mc.getPermission(Entry.Update_BG, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new BankGuaranteeBLL();
            bllObject.deleteBGHistory(tenderid,bgnumber);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            BankGuaranteeMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Update_BG, permissionType.Delete) == false)
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
