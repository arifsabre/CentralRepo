using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class ProductionReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        //
        // GET: /AttendanceReport/

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now;
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region daily production report



        [HttpPost]
        public ActionResult DailyProductionReport(rptOptionMdl rptOption)
        {
            //[100113]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Production_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Production_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            //if (rptOption.EmpName == null) { rptOption.EmpName = ""; };
            if (rptOption.FgItemCode == null) { rptOption.FgItemId = 0; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ProductionReport/getDailyProductionReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&itemid=" + rptOption.FgItemId + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&monthyear=" + rptOption.EmpName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getDailyProductionReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, itemid=rptOption.FgItemId, monthyear=rptOption.EmpName });
        }

        [HttpGet]
        public FileResult getDailyProductionReport(DateTime dtfrom, DateTime dtto, int compcode, int itemid=0, string monthyear="")
        {
            //[100113]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "DailyProductionRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            if (monthyear.Length > 0)
            {
                txtrpthead.Text = "For Month: " + monthyear;
            }
            else
            {
                txtrpthead.Text = "Date From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            }
            //dbp parameters   --usp_get_tbl_productionentry
            rptDoc.SetParameterValue("@itemid", itemid);
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@monthyear", monthyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region production plan status report

        public ActionResult ProductionPlanStatusIndex()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.AttMonth = DateTime.Now.Month;
            rptOption.AttYear = DateTime.Now.Year;
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult ProductionPlanStatusReport(rptOptionMdl rptOption)
        {
            //[100111]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Production_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Production_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ProductionReport/getProductionPlanStatusReport";
                string reportpms = "ppmonth=" + rptOption.AttMonth + "";
                reportpms += "&ppyear=" + rptOption.AttYear + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getProductionPlanStatusReport", new { ppmonth = rptOption.AttMonth, ppyear = rptOption.AttYear, compcode=rptOption.CompCode });
        }

        [HttpGet]
        public ActionResult DisplayPlanStatusReport(int ppmonth, int ppyear)
        {
            //[100111]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Production_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Production_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ProductionReport/getProductionPlanStatusReport";
                string reportpms = "ppmonth=" + ppmonth + "";
                reportpms += "&ppyear=" + ppyear + "";
                reportpms += "&compcode=" + objCookie.getCompCode() + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getProductionPlanStatusReport", new { ppmonth = ppmonth, ppyear = ppyear, compcode = objCookie.getCompCode() });
        }

        [HttpGet]
        public FileResult getProductionPlanStatusReport(int ppmonth, int ppyear, int compcode)
        {
            //[100111]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ProductionPlanStatusRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtPrvMonth = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtPrvMonth"];
            CrystalDecisions.CrystalReports.Engine.TextObject txtMonth1 = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtMonth1"];
            string cmonth = mc.getNameByKey(mc.getMonths(), "monthid", ppmonth.ToString(), "monthname");
            DateTime dtct = new DateTime(ppyear, ppmonth, 1);
            DateTime dtprv = dtct.AddMonths(-1);
            txtrpthead.Text = "For Month: " + cmonth + "-" + ppyear.ToString();
            txtMonth1.Text = cmonth + "-" + ppyear.ToString();
            txtPrvMonth.Text = mc.getNameByKey(mc.getMonths(), "monthid", dtprv.Month.ToString(), "monthname")+"-"+dtprv.Year.ToString();
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtUserIdName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtUserIdName"];
            int userid = Convert.ToInt32(objCookie.getUserId());
            txtUserIdName.Text = "User Id/Name: " + userid.ToString() + "/" + objCookie.getUserName();
            //dbp parameters   --usp_get_production_plan_status
            rptDoc.SetParameterValue("@ppmonth", ppmonth);
            rptDoc.SetParameterValue("@ppyear", ppyear);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@userid", userid);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region production target status report

        public ActionResult ProductionTargetStatusIndex()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.DateFrom = DateTime.Now.AddMonths(-1);
            rptOption.DateTo = DateTime.Now;
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult ProductionTargetStatusReport(rptOptionMdl rptOption)
        {
            //[100112]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Production_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Production_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ProductionReport/getProductionTargetStatusReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getProductionTargetStatusReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode });
        }

        [HttpGet]
        public FileResult getProductionTargetStatusReport(DateTime dtfrom, DateTime dtto, int compcode)
        {
            //[100112]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "ProductionTargetStatusRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            string mth1 = mc.getNameByKey(mc.getMonths(), "monthid", dtfrom.Month.ToString(), "monthname");
            if (dtfrom.Month == dtto.Month)
            {
                txtrpthead.Text = "For " + mth1 + "-" + dtfrom.Year.ToString();
            }
            else 
            {
                string mth2 = mc.getNameByKey(mc.getMonths(), "monthid", dtto.Month.ToString(), "monthname");
                txtrpthead.Text = "From " + mth1 + "-" + dtfrom.Year.ToString() + " To " + mth2 + "-" + dtto.Year.ToString();
            }
            CrystalDecisions.CrystalReports.Engine.TextObject txtUserIdName = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtUserIdName"];
            int userid = Convert.ToInt32(objCookie.getUserId());
            txtUserIdName.Text = "User Id/Name: " + userid.ToString() + "/" + objCookie.getUserName();
            //dbp parameters   --usp_get_production_target_status
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@userid", userid);
            //additional parameters --defined in crpt with @name/static
            //rptDoc.SetParameterValue("@KK", "My Parameter");
            //
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //rd.RecordSelectionFormula = "";
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //add these lines to download
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //return File(stream, "application/pdf", "ReportName.pdf");
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();
                //rptDocSub.Close();
            }
            return File(stream, "application/pdf");
        }

        #endregion

        #region production plan material requirement report
        [HttpGet]
        public ActionResult DisplayProductionPlanMaterialList(int ppmonth, int ppyear, int format)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Production_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Production_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ProductionReport/GetReportHTML";
                string reportpms = "ppmonth=" + ppmonth + "";
                reportpms += "&ppyear=" + ppyear + "";
                reportpms += "&format=" + format + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { ppmonth = ppmonth, ppyear = ppyear, format = format });
        }

        //get
        public ActionResult GetReportHTML(int ppmonth, int ppyear, int format=1, int ccode=0)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            setViewData();

            ProductionReportBLL rptBLL = new ProductionReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetProductionPlanReportHtml(ppmonth, ppyear, ccode, format);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            sbHeader.Append("<div style='font-size:10pt;color:red;'>* Under Trial</div>");
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address
            sbHeader.Append("<div style='font-size:10pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpFooter1"].ToString());
            sbHeader.Append("</div>");
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            if (format == 1)
            {
                sbHeader.Append("<b><u>Detailed Production Plan Report</u></b>");
            }
            else
            {
                sbHeader.Append("<b><u>Material Requirement List for Production</u></b>");
            }
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            if (format == 1)
            {
                sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString()+" -Finished Item Group");
            }
            else 
            {
                sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString() + " -Raw Material Group");
            }
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            string lev = "";
            string fgitem = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            //report content -1
            if (format == 1)
            {
                ArrayList arlitem = new ArrayList();
                for (int i = 0; i < ds.Tables["tbl2"].Rows.Count; i++)
                {
                    if (!arlitem.Contains(ds.Tables["tbl2"].Rows[i]["itemid"].ToString()))
                    {
                        arlitem.Add(ds.Tables["tbl2"].Rows[i]["itemid"].ToString());
                    }
                }
                
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th style='width:15px;'>SlNo</th>");
                sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
                sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Prd.&nbsp;Qty</th>");
                sb.Append("<th style='width:15px;'>Item&nbsp;Unit</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");

                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td>" + (i + 1).ToString() + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["itemcode"].ToString() + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["shortname"].ToString() + "</td>");
                    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["prdqty"].ToString())) + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["unitname"].ToString() + "</td>");
                    sb.Append("</tr>");

                    if (arlitem.Contains(ds.Tables["tbl1"].Rows[i]["itemid"].ToString()))
                    {
                        int sno = 1;
                        lev = "";
                        fgitem = "";
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>&nbsp;</td>");
                        sb.Append("<td colspan='4'>");
                        sb.Append("<table class='tblcontainer' style='width:100%;'>");
                        sb.Append("<thead>");
                        sb.Append("<tr>");
                        sb.Append("<th style='width:15px;'>SlNo</th>");
                        sb.Append("<th style='width:15px;'>Level</th>");
                        sb.Append("<th style='width:15px;'>FG/SA&nbsp;Item</th>");
                        sb.Append("<th style='width:15px;'>SA/RM&nbsp;Item</th>");
                        sb.Append("<th style='width:15px;text-align:right;'>RM&nbsp;Qty</th>");
                        sb.Append("<th style='width:15px;text-align:right;'>Usage</th>");
                        sb.Append("<th style='width:15px;text-align:right;'>Prod.&nbsp;Usage</th>");
                        sb.Append("<th style='width:15px;'>FG&nbsp;Unit</th>");
                        sb.Append("<th style='width:15px;'>RM&nbsp;Unit</th>");
                        sb.Append("<th style='width:15px;text-align:right;'>Waste&nbsp;Qty</th>");
                        sb.Append("</tr>");
                        sb.Append("</thead>");
                        for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
                        {
                            if (ds.Tables["tbl2"].Rows[j]["itemid"].ToString() == ds.Tables["tbl1"].Rows[i]["itemid"].ToString())
                            {
                                sb.Append("<tr class='tblrow'>");
                                sb.Append("<td>" + sno.ToString() + "</td>");
                                if (lev != ds.Tables["tbl2"].Rows[j]["Lev"].ToString())
                                {
                                    lev = ds.Tables["tbl2"].Rows[j]["Lev"].ToString();
                                    sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["Lev"].ToString() + "</td>");
                                }
                                else
                                {
                                    sb.Append("<td>&nbsp;</td>");//note
                                }
                                string str = ds.Tables["tbl2"].Rows[j]["fgitemcode"].ToString() + "<br/>" + ds.Tables["tbl2"].Rows[j]["fgitemname"].ToString();
                                if (fgitem != str)
                                {
                                    fgitem = str;
                                    sb.Append("<td>" + str + "</td>");
                                }
                                else
                                {
                                    sb.Append("<td>&nbsp;</td>");//note
                                }
                                str = ds.Tables["tbl2"].Rows[j]["rmitemcode"].ToString() + "<br/>" + ds.Tables["tbl2"].Rows[j]["rmitemname"].ToString();
                                sb.Append("<td>" + str + "</td>");
                                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["rmqty"].ToString())) + "</td>");
                                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["usage"].ToString())) + "</td>");
                                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["prdusage"].ToString())) + "</td>");
                                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["fgunitname"].ToString() + "</td>");
                                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["rmunitname"].ToString() + "</td>");
                                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["wasteqty"].ToString())) + "</td>");
                                sb.Append("</tr>");
                                sno += 1;
                            }
                        }
                        sb.Append("</table>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }

                }
            }
            else if (format == 2)
            {
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th style='width:15px;'>SlNo</th>");
                sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
                sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Prd.&nbsp;Usage</th>");
                sb.Append("<th style='width:15px;'>Unit</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Stock</th>");
                sb.Append("<th style='width:15px;text-align:right;'>MSB</th>");
                sb.Append("<th style='width:15px;text-align:right;'>Rqd.&nbsp;Qty</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td>" + (i + 1).ToString() + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["itemcode"].ToString() + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["shortname"].ToString() + "</td>");
                    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["prdusage"].ToString())) + "</td>");
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["unitname"].ToString() + "</td>");
                    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["stock"].ToString())) + "</td>");
                    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["msbqty"].ToString())) + "</td>");
                    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["RequiredQty"].ToString())) + "</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        private void PerformDataGrouping(DataSet dsRm)
        {
            //data grouping
            DataTable dtr = new DataTable();
            dtr.Columns.Add("ItemId", typeof(System.Int32));
            dtr.Columns.Add("ItemCode", typeof(System.String));
            dtr.Columns.Add("ShortName", typeof(System.String));
            dtr.Columns.Add("PrdUsage", typeof(System.Decimal));
            dtr.Columns.Add("UnitName", typeof(System.String));
            dtr.Columns.Add("WasteQty", typeof(System.Decimal));
            dtr.Columns.Add("WIPQty", typeof(System.Decimal));
            dtr.Columns.Add("Stock", typeof(System.Decimal));
            dtr.Columns.Add("MSBQty", typeof(System.Decimal));
            dtr.Columns.Add("RequiredQty", typeof(System.Decimal));
            DataRow drx = dtr.NewRow();
            int x = 0;
            for (int i = 0; i < dsRm.Tables[0].Rows.Count; i++)
            {
                x = getItemIndex(dtr, dsRm.Tables[0].Rows[i]["itemid"].ToString());
                if (x == -1)
                {
                    drx = dtr.NewRow();
                    drx["ItemId"] = dsRm.Tables[0].Rows[i]["ItemId"].ToString();
                    drx["ItemCode"] = dsRm.Tables[0].Rows[i]["ItemCode"].ToString();
                    drx["ShortName"] = dsRm.Tables[0].Rows[i]["ShortName"].ToString();
                    drx["PrdUsage"] = dsRm.Tables[0].Rows[i]["PrdUsage"].ToString();
                    drx["UnitName"] = dsRm.Tables[0].Rows[i]["UnitName"].ToString();
                    drx["WasteQty"] = dsRm.Tables[0].Rows[i]["WasteQty"].ToString();
                    dtr.Rows.Add(drx);
                }
                else
                {
                    dtr.Rows[x]["prdusage"] = Convert.ToDouble(dtr.Rows[x]["prdusage"]) + Convert.ToDouble(dsRm.Tables[0].Rows[i]["prdusage"].ToString());
                    dtr.Rows[x]["wasteqty"] = Convert.ToDouble(dtr.Rows[x]["wasteqty"]) + Convert.ToDouble(dsRm.Tables[0].Rows[i]["wasteqty"].ToString());
                }
            }
        }

        private int getItemIndex(DataTable dt, string itemid)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["itemid"].ToString() == itemid)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

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

    }
}
