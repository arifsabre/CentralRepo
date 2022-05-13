using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class JobworkIssueController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private JobworkIssueBLL bllObject = new JobworkIssueBLL();
        private VendorAddressBLL vAddressBLL = new VendorAddressBLL();
        private ItemBLL itemBLL = new ItemBLL();

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
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            ViewBag.DtFrom = dtfrom;
            ViewBag.DtTo = dtto;
            ViewBag.UnitList = new SelectList(itemBLL.getItemUnitList(), "unit", "unitname");
            ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(0), "VendorAddId", "VAddress");
        }

        [HttpGet]
        public ActionResult Index(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<JobworkIssueMdl> modelObject = new List<JobworkIssueMdl> { };
            modelObject = bllObject.getObjectList(dtfrom, dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("Index", new { dtfrom = dtfrom, dtto = dtto });
        }

        [HttpPost]
        public ActionResult SearchChallan(FormCollection form)
        {
            string challanno = form["txtChallanNo"].ToString();
            if (challanno.Length == 0)
            {
                string baseurl = System.Configuration.ConfigurationManager.AppSettings["baseurl"].ToString();
                baseurl += "JobworkIssue/CreateUpdate";
                return Content("<a href='" + baseurl + "'><h1>Invalid Challan!</h1></a>");
            }
            return RedirectToAction("CreateUpdate", new { id = 0, challanno = challanno });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string msg = "", string challanno = "", int vendorid = 0, int vaddid = 0, string vname = "", string processname="")
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            JobworkIssueMdl modelObject = new JobworkIssueMdl();
            modelObject.ChallanDate = DateTime.Now;
            DataSet ds = new DataSet();
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                if (modelObject.EntryType.ToLower() == "list")
                {
                    return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid entry type!</h1></a>");
                }
                ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(modelObject.VendorId), "VendorAddId", "VAddress", modelObject.VendorAddId);
                ds = bllObject.GetChallanItemData(modelObject.ChallanNo);
            }
            else  
            {
                ds = bllObject.GetChallanItemData(challanno);
                modelObject.DispId = 0;//so for
                modelObject.EntryType = "store";//note
                modelObject.ChallanNo = challanno;
                modelObject.VendorId = vendorid;
                modelObject.VendorName = vname;
                //
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (vendorid == 0) 
                        { 
                            modelObject.VendorId = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorId"].ToString());
                            vendorid = modelObject.VendorId;
                        }
                        if (vname.Length == 0) { modelObject.VendorName = ds.Tables[0].Rows[0]["VendorName"].ToString(); };
                        modelObject.ChallanDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ChallanDate"].ToString());
                        modelObject.VendorAddId = Convert.ToInt32(ds.Tables[0].Rows[0]["VendorAddId"].ToString());
                        modelObject.TrpMode = ds.Tables[0].Rows[0]["TrpMode"].ToString();
                        modelObject.TrpDetail = ds.Tables[0].Rows[0]["TrpDetail"].ToString();
                        modelObject.InvNote = ds.Tables[0].Rows[0]["InvNote"].ToString();
                        //note
                        modelObject.EntryType = ds.Tables[0].Rows[0]["EntryType"].ToString();
                    }
                }
                //
                modelObject.RMItemId = 0;
                modelObject.RMItemCode = "";
                modelObject.IssuedQty = 0;
                modelObject.ProcessDesc = processname;
                modelObject.ApproxValue = 0;
                ViewBag.VAddressList = new SelectList(vAddressBLL.getAddressListForVendor(vendorid), "VendorAddId", "VAddress", vaddid);
            }
            modelObject.ChallanItems = GetChallnItemHtml(ds);
            if (msg.Length > 0) { ViewBag.Message = msg; };
            return View(modelObject);
        }

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(JobworkIssueMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (modelObject.EntryType.ToLower() == "list")
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Invalid entry type!</h1></a>");
            }
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
            if (modelObject.DispId == 0)//add mode
            {
                modelObject.EntryType = "store";//note
                if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else//edit mode
            {
                if (mc.getPermission(Entry.JobworkEntry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            return RedirectToAction("CreateUpdate", new { id = 0, msg = bllObject.Message, challanno = modelObject.ChallanNo, vendorid = modelObject.VendorId, vaddid = modelObject.VendorAddId, vname = modelObject.VendorName, processname = modelObject.ProcessDesc });
        }

        private string GetChallnItemHtml(DataSet ds)
        {
            double amtvalue = 0;
            double apvalue = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tablecontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
            sb.Append("<th style='width:15px;'>Short&nbsp;Name</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Issued&nbsp;Qty</th>");
            sb.Append("<th style='width:100px;'>Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Rate</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Approx&nbsp;Value</th>");
            sb.Append("<th style='width:50px;'>Process</th>");
            sb.Append("<th style='width:50px;'>Links</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["rmitemcode"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["rmshortname"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["issuedqty"].ToString())) + "</td>");
                sb.Append("<td>" + ds.Tables[0].Rows[i]["rmunitname"].ToString() + "</td>");
                sb.Append("<td style='text-align:right;'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["rate"].ToString())) + "</td>");
                apvalue = Convert.ToDouble(ds.Tables[0].Rows[i]["approxvalue"].ToString());
                sb.Append("<td style='text-align:right;'>" + mc.getINRCFormat(apvalue) + "</td>");
                amtvalue += apvalue;
                sb.Append("<td>" + ds.Tables[0].Rows[i]["processdesc"].ToString() + "</td>");
                sb.Append("<td><a style='color:blue;' href='/JobworkIssue/CreateUpdate?id=" + ds.Tables[0].Rows[i]["dispid"].ToString() + "'> Edit </a><br/><a style='color:blue;' href='#' onclick='javascript:deleteRecord(" + ds.Tables[0].Rows[i]["dispid"].ToString() + ");'> Delete </a></td>");
            }
            sb.Append("</table>");
            sb.Append("<div><b>Total Amount: " + mc.getINRCFormat(amtvalue) + "</b></div>");
            return sb.ToString();
        }

        #region jobwork issue by wiplist

        [HttpGet]
        public ActionResult IndexWipToJobwork(string dtfrom = "", string dtto = "")
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(objCookie.getFromDate()); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(objCookie.getDispToDate()); };
            setViewObject(dtfrom, dtto);
            List<JobworkIssueMdl> modelObject = new List<JobworkIssueMdl> { };
            modelObject = bllObject.getPendingWipToJobworkList(dtfrom, dtto);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndexWipToJobwork(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            return RedirectToAction("IndexWipToJobwork", new { dtfrom = dtfrom, dtto = dtto });
        }

        [HttpPost]
        public JsonResult GenerateJobworkChallanByWip(int wipdispid, string jwchallanno, double qty)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!" } };
            }
            bllObject = new JobworkIssueBLL();
            bllObject.GenerateJobworkIssueByWIP(wipdispid, jwchallanno, qty);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        #region jobwork cancellation

        [HttpGet]
        public ActionResult CancelJobwork(int dispid, string msg="")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            JobworkIssueMdl modelObject = new JobworkIssueMdl();
            modelObject = bllObject.searchObject(dispid);
            ViewBag.Message = msg;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult CancelJobwork(JobworkIssueMdl modelObject)
        {
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Edit) == false)
            {
                ViewBag.Message = "Permission Denied!";
                return View();
            }
            setViewData();
            bllObject = new JobworkIssueBLL();
            bllObject.updateJobworkToCancelled(modelObject.DispId, modelObject.CancelledOn, modelObject.Reason);
            if (bllObject.Result == true)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = bllObject.Message;
            return RedirectToAction("CancelJobwork", new { dispid=modelObject.DispId, msg=bllObject.Message });
        }

        #endregion

        [HttpPost]
        public JsonResult Delete(JobworkIssueMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.JobworkEntry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new JobworkIssueBLL();
            bllObject.deleteObject(modelObject.DispId);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
