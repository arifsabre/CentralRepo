using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AmcWorkController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AmcWorkBLL bllObject = new AmcWorkBLL();
        private ItemBLL itemBLL = new ItemBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(string dtfrom = "", string dtto = "", bool filterbydt = true, int itemid = 0, string itemcode = "")
        {
            ViewBag.FilterByDT = filterbydt;
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.ItemCode = itemcode;
            ViewBag.ItemId = itemid;
        }

        public ActionResult Index(string dtfrom = "", string dtto = "", bool filterbydt = true, int itemid = 0, string itemcode = "")
        {
            if (mc.getPermission(Entry.AMC_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto, filterbydt, itemid, itemcode);
            List<AmcWorkMdl> modelObject = new List<AmcWorkMdl> { };
            modelObject = bllObject.getObjectList(mc.getDateByString(dtfrom),mc.getDateByString(dtto),filterbydt,itemid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            bool filterbydt = false;
            if (form["chkFilterByDT"] != null)
            {
                filterbydt = true;
            }
            string itemid = form["hfItemId"].ToString();
            string itemcode = form["txtItemCode"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto, filterbydt = filterbydt, itemid = itemid, itemcode = itemcode });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (mc.getPermission(Entry.AMC_Entry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.ConsigneeList = new SelectList(bllObject.getPOConsigneeList(0), "Unit", "UnitName");
            //
            AmcWorkMdl modelObject = new AmcWorkMdl();
            ViewBag.VDate = mc.getStringByDate(objCookie.getDispToDate());
            modelObject = bllObject.searchObject(id);
            if (modelObject.AmcId > 0)
            {
                ViewData["AddEdit"] = "Update";
                ViewBag.ConsigneeList = new SelectList(bllObject.getPOConsigneeList(modelObject.POrderId), "Unit", "UnitName");
            }
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public JsonResult CreateUpdate(AmcWorkMdl modelObject)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Session expired! Login again." } };
            }
            bllObject = new AmcWorkBLL();
            if (modelObject.AmcId == 0)//add mode
            {
                if (mc.getPermission(Entry.AMC_Entry, permissionType.Add) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Save" } };
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.AMC_Entry, permissionType.Edit) == false)
                {
                    return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
                }
                bllObject.updateObject(modelObject);
            }
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message, AddEdit = "Save" } };
        }

        [HttpPost]
        public JsonResult deleteAmcWork(int amcid)
        {
            if (objCookie.checkSessionState() == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            bllObject.deleteObject(amcid);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]//used with AutoCompletePurchaseNumbers
        public JsonResult GetConsigneeListForPO(int porderid)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = bllObject.getPOConsigneeData(porderid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["consigneeid"].ToString(), ds.Tables[0].Rows[i]["consigneename"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        #region upload/download section

        //get
        public ActionResult UploadFile(int amcid = 0, string challanno = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            ViewBag.amcid = amcid;
            ViewBag.challanno = challanno;
            ViewBag.Message = "";
            if (Session["upamcid"] != null)
            {
                ViewBag.amcid = Session["upamcid"].ToString();
                Session.Remove("upamcid");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase docfile, int amcid, string challanno = "")
        {
            if (mc.getPermission(Entry.AMC_Entry, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.AMC_Entry) + "]";
                return Content(msg);
            }
            setViewData();

            if (amcid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }
            
            ViewBag.amcid = amcid;
            Session["upamcid"] = amcid;
            Session["updmsg"] = "Error in File Upload!";
            ViewBag.challnno = challanno;

            if (docfile != null)
            {
                System.IO.Stream str = docfile.InputStream;
                System.IO.BinaryReader Br = new System.IO.BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                AmcWorkMdl modelObject = new AmcWorkMdl();
                modelObject.FlName = docfile.FileName;
                modelObject.FileContent = FileDet;
                modelObject.AmcId = amcid;
                bllObject.uploadAmcDocumentFile(modelObject);
                Session["updmsg"] = bllObject.Message;
            }
            else 
            {
                Session["updmsg"] = "No file selected to upload!";
            }
            return RedirectToAction("UploadFile", new { amcid = amcid, challanno = challanno });
        }

        [HttpGet]
        public FileContentResult ShowDocument(int amcid = 0)
        {
            if (mc.getPermission(Entry.AMC_Entry, permissionType.Edit) == false)
            {
                return null;
            }
            setViewData();
            AmcWorkBLL bll = new AmcWorkBLL();
            return File(bll.getAmcDocumentFile(amcid), bll.Message);
        }

        #endregion //upload/download section

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
