using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class NitListController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private NitListBLL bllObject = new NitListBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (mc.getPermission(Entry.NIT_List_Upload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            NitListMdl modelObject = new NitListMdl();
            List<NitListMdl> objlist = new List<NitListMdl> { };
            modelObject.TenderList = objlist;
            return View(modelObject);
        }

        [HttpPost]
        public ActionResult Index(NitListMdl modelObject)
        {
            if (mc.getPermission(Entry.NIT_List_Upload, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //
            bllObject.prepareNITList(modelObject);
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        #region NIt List Procesing

        [HttpGet]
        public ActionResult NitProcessingIndex(string uploadingdt = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (uploadingdt.Length == 0) { uploadingdt = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            DateTime upldt = mc.getDateBySqlGenericString(uploadingdt);
            //
            List<NitListMdl> modelObject = new List<NitListMdl> { };
            modelObject = bllObject.getToProcessNITList(upldt);
            ViewBag.uploadingdt = uploadingdt;
            return View(modelObject.ToList());
        }

        [HttpGet]
        public ActionResult NitProcessingIndex_1x(string uploadingdt = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (uploadingdt.Length == 0) { uploadingdt = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            DateTime upldt = mc.getDateBySqlGenericString(uploadingdt);
            //
            List<NitListMdl> modelObject = new List<NitListMdl> { };
            modelObject = bllObject.getToProcessNITList(upldt);
            ViewBag.uploadingdt = uploadingdt;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterNitProcessIndex(FormCollection form)
        {
            string uploadingdt = form["txtUploadingDt"].ToString();
            return RedirectToAction("NitProcessingIndex", new { uploadingdt = uploadingdt });
        }

        [HttpPost]
        public JsonResult updateNitListForProcessing(NitListMdl modelObject)
        {
            if (mc.getPermission(Entry.NIT_List_Processing, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new NitListBLL();
            bllObject.updateNITListForProcessing(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        #endregion

        #region NIT View List/Process Updation

        [HttpGet]
        public ActionResult NitViewIndex(string dtfrom = "", string dtto = "", string filterby = "upl", string filteropt = "prctocs", int ccode = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            DateTime datefrom = mc.getDateBySqlGenericString(dtfrom);
            DateTime dateto = mc.getDateBySqlGenericString(dtto);
            ViewBag.CompanyList = new SelectList(CompanyBLL.Instance.getObjectList(), "compcode", "cmpname", ccode);
            ViewBag.FilterByList = new SelectList(getFilterByList(), "Value", "Text", filterby);
            ViewBag.FilterOptList = new SelectList(getFilterOptionList(), "Value", "Text", filteropt);
            List<NitListMdl> modelObject = new List<NitListMdl> { };
            modelObject = bllObject.getNITList(datefrom,dateto,filterby,filteropt,ccode);
            ViewBag.dtfrom = dtfrom;
            ViewBag.dtto = dtto;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterNitViewIndex(FormCollection form)
        {
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            string filterby = form["ddlFilterBy"].ToString();
            string filteropt = form["ddlFilterOpt"].ToString();
            int ccode = 0;
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            return RedirectToAction("NitViewIndex", new { dtfrom = dtfrom, dtto = dtto, filterby = filterby, filteropt = filteropt, ccode = ccode });
        }

        [HttpPost]
        public JsonResult UpdateNitProcessStatus(NitListMdl modelObject)
        {
            if (mc.getPermission(Entry.NIT_Process_Updation, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission Denied!", AddEdit = "Update" } };
            }
            bllObject = new NitListBLL();
            bllObject.updateProcessingStatus(modelObject);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        public List<System.Web.UI.WebControls.ListItem> getFilterByList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Uploading Date", Value = "upl" },
                  new System.Web.UI.WebControls.ListItem { Text = "Closing Date", Value = "clg" }
            };
            return listItems;
        }

        public List<System.Web.UI.WebControls.ListItem> getFilterOptionList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                  new System.Web.UI.WebControls.ListItem { Text = "Marked Proceeded", Value = "prctocs" },
                  new System.Web.UI.WebControls.ListItem { Text = "Alert Generated", Value = "alert" },
                  new System.Web.UI.WebControls.ListItem { Text = "Not Our Item", Value = "noi" },
                  new System.Web.UI.WebControls.ListItem { Text = "Un-Processed", Value = "unprc" },
                  new System.Web.UI.WebControls.ListItem { Text = "Un-Attended", Value = "unattd" },
                  new System.Web.UI.WebControls.ListItem { Text = "All Records", Value = "all" },
            };
            return listItems;
        }

        [HttpPost]
        public JsonResult getDeptRailwayExclListGeneric()
        {
            string list = "Generic Divisions - Checked as 'Contains':\r\n";
            bllObject = new NitListBLL();
            DataSet ds = bllObject.getDeptRailwaysExclusionListForNITGeneric();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                list += ds.Tables[0].Rows[i]["deptrailway"].ToString() + "\r\n";
            }
            return new JsonResult { Data = new { list = list}};
        }

        [HttpPost]
        public JsonResult getDeptRailwaysExclusionListForNIT()
        {
            string list = "Specific Divisions - Checked as 'Equals':\r\n";
            bllObject = new NitListBLL();
            DataSet ds = bllObject.getDeptRailwaysExclusionListForNIT();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                list += ds.Tables[0].Rows[i]["deptrailway"].ToString() + "\r\n";
            }
            return new JsonResult { Data = new { list = list } };
        }

        [HttpPost]
        public JsonResult getTenderTitlesExclusionListForNIT()
        {
            string list = "Tender Title - Checked by 'Value found as Word'\r\n";
            bllObject = new NitListBLL();
            DataSet ds = bllObject.getTenderTitlesExclusionListForNIT();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                list += ds.Tables[0].Rows[i]["tendertitle"].ToString() + "\r\n";
            }
            return new JsonResult { Data = new { list = list } };
        }

        #endregion

        #region NIT List Report

        [HttpPost]
        public ActionResult DisplayNitReport(FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            string dtfrom = form["txtDtFrom"].ToString();
            string dtto = form["txtDtTo"].ToString();
            //
            if (dtfrom.Length == 0) { dtfrom = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            if (dtto.Length == 0) { dtto = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            DateTime datefrom = mc.getDateBySqlGenericString(dtfrom);
            DateTime dateto = mc.getDateBySqlGenericString(dtto);
            //
            string filterby = form["ddlFilterBy"].ToString();
            string filteropt = form["ddlFilterOpt"].ToString();
            int ccode = 0;
            if (form["ddlCompany"].ToString().Length > 0)
            {
                ccode = Convert.ToInt32(form["ddlCompany"].ToString());
            }
            //
            bool viewper = mc.getPermission(Entry.NIT_List_Processing, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.NIT_List_Processing, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "NitList/getNitListReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(datefrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(dateto) + "";
                reportpms += "&filterby=" + filterby + "";
                reportpms += "&filteropt=" + filteropt + "";
                reportpms += "&ccode=" + ccode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getNitListReport", new { dtfrom = datefrom, dtto = dateto, filterby = filterby, filteropt = filteropt, ccode = ccode });
        }

        [HttpPost]
        public ActionResult DisplayNitReportR1(FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //
            string uploadingdt = form["txtUploadingDt"].ToString();
            if (uploadingdt.Length == 0) { uploadingdt = mc.getStringByDate(DateTime.Now.AddDays(-1)); };
            DateTime datefrom = mc.getDateBySqlGenericString(uploadingdt);
            DateTime dateto = datefrom;
            //
            string filterby = "upl";
            string filteropt = "all";
            int ccode = 0;
            //
            bool viewper = mc.getPermission(Entry.NIT_List_Processing, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.NIT_List_Processing, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "NitList/getNitListReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(datefrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(dateto) + "";
                reportpms += "&filterby=" + filterby + "";
                reportpms += "&filteropt=" + filteropt + "";
                reportpms += "&ccode=" + ccode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getNitListReport", new { dtfrom = datefrom, dtto = dateto, filterby = filterby, filteropt = filteropt, ccode = ccode });
        }

        [HttpGet]
        public FileResult getNitListReport(DateTime dtfrom, DateTime dtto, string filterby, string filteropt, int ccode)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/TenderReport/"), "NitListReportV2.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtCmpName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtCmpName"];
            txtCmpName.Text = "PRAG GROUP";
            if(ccode !=0)
            {
                CompanyMdl cmpMdl = new CompanyMdl();
                CompanyBLL cmpBLL = new CompanyBLL();
                cmpMdl = cmpBLL.searchObject(ccode);
                txtCmpName.Text = cmpMdl.CmpName;
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            //report filters
            string RptFilters = "";
            if (filterby.ToLower() == "upl")
            {
                RptFilters = "Uploading Date";
            }
            else if (filterby.ToLower() == "clg")
            {
                RptFilters = "Closing Date";
            }
            if (dtfrom == dtto)
            {
                RptFilters += ": " + mc.getStringByDate(dtfrom);
            }
            else
            {
                RptFilters += " From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            }

            NitListBLL objBll = new NitListBLL();
            List<NitListMdl> modelObject = new List<NitListMdl> { };
            modelObject = objBll.getNITList(dtfrom, dtto, filterby, filteropt, ccode);

            RptFilters += "\r\nTender List - " + modelObject.Count.ToString() + " Record(s) found";
            string foption = "";
            if (filteropt.ToLower() == "prctocs")
            {
                foption = " (Marked Proceeded)";
            }
            else if (filteropt.ToLower() == "alert")
            {
                foption = " (Alert Generated)";
            }
            else if (filteropt.ToLower() == "noi")
            {
                foption = " (Not Our Item)";
            }
            else if (filteropt.ToLower() == "unprc")
            {
                foption = " (Un-Processed)";
            }
            else if (filteropt.ToLower() == "unattd")
            {
                foption = " (Un-Attended)";
            }
            RptFilters += foption;
            txtRptHead.Text = RptFilters;

            //dbp --usp_get_tbl_nitlist_v2
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@filterby", filterby);
            rptDoc.SetParameterValue("@filteropt", filteropt);
            rptDoc.SetParameterValue("@compcode", ccode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
            }
            return File(stream, "application/pdf");
        }

        private void setLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
        {
            DataTable lginfo = mc.getCrptLoginInfo();
            CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
            CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();
            CrystalDecisions.Shared.ConnectionInfo crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
            crConnectionInfo.ServerName = lginfo.Rows[0]["svrname"].ToString();
            crConnectionInfo.DatabaseName = lginfo.Rows[0]["dbname"].ToString();
            crConnectionInfo.UserID = lginfo.Rows[0]["userid"].ToString();
            crConnectionInfo.Password = lginfo.Rows[0]["passw"].ToString();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables = rptDoc.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
