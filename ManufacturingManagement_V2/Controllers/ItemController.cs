using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class ItemController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private ItemBLL bllObject = new ItemBLL();
        private ItemGroupBLL iGroupBll = new ItemGroupBLL();
        private UserBLL ubll = new UserBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int groupid = 0, string itemtype = "fi")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.UnitList = new SelectList(bllObject.getItemUnitList(), "unit", "unitname");
            ViewBag.ItemTypeList = new SelectList(mc.getItemTypeList(), "Value", "Text", itemtype);
            ViewBag.ItemGroupList = new SelectList(iGroupBll.getObjectList(), "groupid", "groupname", groupid);
        }

        public ActionResult Index(int groupid = 0, string itemtype = "fi")
        {
            if (mc.getPermission(Entry.Item_Master_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.ItemTypeList = new SelectList(mc.getItemTypeList(), "Value", "Text", itemtype);
            List<ItemMdl> modelObject = new List<ItemMdl> { };
            modelObject = bllObject.getObjectList(groupid, itemtype);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int groupid = 0;
            if (form["ddlGroup"].ToString().Length > 0)
            {
                groupid = Convert.ToInt32(form["ddlGroup"].ToString());
            }
            string itemtype = form["ddlItemType"].ToString();
            return RedirectToAction("Index", new { groupid = groupid, itemtype = itemtype });
        }

        [HttpPost]
        public ActionResult SearchItem(FormCollection form)
        {
            string itemcode = form["txtItemCode"].ToString();
            int itemid = 0;
            if (form["hfSearchItemId"].ToString().Length > 0)
            {
                itemid = Convert.ToInt32(form["hfSearchItemId"].ToString());
            }
            if (itemid == 0 || itemcode.Length == 0)
            {
                //return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid ItemCode entered!</h1></a>");
                string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
                baseurl += "Item/CreateUpdate";
                return Content("<a href='"+baseurl+"'><h1>Invalid ItemCode entered!</h1></a>");
            }
            return RedirectToAction("CreateUpdate", new { id = itemid, iname = itemcode });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string iname = "")
        {
            if (mc.getPermission(Entry.Item_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ItemMdl modelObject = new ItemMdl();
            modelObject.ItemCode = iname;
            modelObject.updateUnit = false;
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Modifyinfo = ubll.udfGetLatestModifyInfo(dbTables.tbl_item,id.ToString());
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(ItemMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.ItemId == 0)//add mode
                {
                    if (mc.getPermission(Entry.Item_Master, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.Item_Master, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    if (modelObject.updateUnit == true)
                    {
                        bllObject.updateItemUnit(modelObject.Unit, modelObject.ItemId);
                    }
                    else
                    {
                        bllObject.updateObject(modelObject);
                    }
                }
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteAllItems(string term, int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            ItemBLL objBll = new ItemBLL();
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = objBll.getAllItemList(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ItemId"].ToString(), ds.Tables[0].Rows[i]["ItemCode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteFinishedAndAssembledItems(string term, int ccode = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            ItemBLL objBll = new ItemBLL();
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = objBll.getFinishedAndAssembledItems(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ItemId"].ToString(), ds.Tables[0].Rows[i]["ItemCode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Item_Master_Report, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            ItemMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAdmin = "0";
            if (objCookie.getLoginType() == 0)
            {
                ViewBag.IsAdmin = "1";
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Item_Master, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompletePOSaleItemList(string term, int groupid = 0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = bllObject.getPOSaleItemsList(groupid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["itemid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
