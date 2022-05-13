using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class BomController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private BomBLL bllObject = new BomBLL();
        private ItemBLL itemBLL = new ItemBLL();
        
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int fgitemid=0, string fgitemcode="")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.fgitemid = fgitemid;
            ViewBag.fgitemcode = fgitemcode;
        }

        [HttpGet]
        public ActionResult IndexNew()
        {
            if (mc.getPermission(Entry.BOM_Definition, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9030]</h1></a>");
            }
            setViewData();
            ViewBag.baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
            //setViewObject(fgitemid, fgitemcode);
            bllObject = new BomBLL();
            BomMdl modelObject = new BomMdl();
            modelObject.BomList = bllObject.getObjectList().ToList();
            return View(modelObject);
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, int fgitemid = 0, string fgitemcode = "", string msg = "")
        {
            if (mc.getPermission(Entry.BOM_Definition, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            setViewData();
            ViewData["AddEdit"] = "Save";
            setViewObject();
            BomMdl modelObject = new BomMdl();
            modelObject.FgItemId = fgitemid;
            modelObject.FgItemCode = fgitemcode;
            ViewBag.fgitemid = fgitemid;
            ViewBag.fgunitname = "[Unit]";
            ViewBag.rmunitname = "[Unit]";
            ViewBag.Message = msg;
            modelObject.RevDate = DateTime.Now;
            ViewBag.Status = "0";
            //
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
                ViewBag.fgitemid = modelObject.FgItemId;
                ViewBag.fgunitname = modelObject.FgUnitName;
                ViewBag.rmunitname = modelObject.RmUnitName;
            }
            modelObject.RevisionUpdate = false;//note
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(BomMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewData["AddEdit"] = "Save";
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                ViewBag.Status = "0";
                if (modelObject.RecId == 0)//add mode
                {
                    if (mc.getPermission(Entry.BOM_Definition, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.BOM_Definition, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                ViewBag.Message = bllObject.Message;
                if (bllObject.Result == true)
                {
                    if (modelObject.RecId == 0)//add mode
                    {
                        return RedirectToAction("CreateUpdate", 
                            new { id = modelObject.RecId, fgitemid = modelObject.FgItemId, 
                                fgitemcode = modelObject.FgItemCode, msg = bllObject.Message});
                    }
                    else//edit mode
                    {
                        ViewBag.Status = "1";
                    }
                }
            }
            return View(modelObject);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteItemsDefinedInBOM(string term, int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            BomBLL objBll = new BomBLL();
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = objBll.getItemsDefinedInBOM(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ItemId"].ToString(), ds.Tables[0].Rows[i]["ItemCode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteOtherThanFinishedItemsForBOM(string term, int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            BomBLL objBll = new BomBLL();
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = objBll.getOtherThanFinishedItemsForBOM(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ItemId"].ToString(), ds.Tables[0].Rows[i]["ItemCode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLatestBomRevisionDetail(int itemid)
        {
            BomBLL bll = new BomBLL();
            DataSet ds = bll.getLatestBomRevisionDetail(itemid);
            return new JsonResult { Data = new { revno = ds.Tables[0].Rows[0]["revno"].ToString(), revdate = ds.Tables[0].Rows[0]["revdate"].ToString() } };
        }

        [HttpPost]
        public JsonResult Delete(int recid)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.BOM_Definition, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new BomBLL();
            bllObject.deleteObject(recid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}