using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;

namespace ManufacturingManagement_V2.Controllers
{
    public class MaterialRequirementReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();

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
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.ReportDate = DateTime.Now;
            rptOption.Amount = 1;
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region material requirement report

        [HttpPost]
        public ActionResult MaterialRequirementReport(rptOptionMdl rptOption)
        {
            //[100106]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (rptOption.FgItemId == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Finished Item not selected!</h1></a>");
            }
            setViewData();
            bool viewper = mc.getPermission(Entry.Import_List, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Import_List, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "MaterialRequirementReport/MaterialRequirementReportHTML";
                string reportpms = "itemid=" + rptOption.FgItemId + "";
                //reportpms += "&itemcode=" + rptOption.FgItemCode + "";
                reportpms += "&prdqty=" + rptOption.Amount + "";
                //reportpms += "&compcode=" + rptOption.CompCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            //return RedirectToAction("getMaterialRequirementReport", new { itemid = rptOption.FgItemId, itemcode = rptOption.FgItemCode, rqty= rptOption.Amount, compcode = rptOption.CompCode });
            return RedirectToAction("MaterialRequirementReportHTML", new { itemid = rptOption.FgItemId, prdqty = rptOption.Amount });
        }

        [HttpGet]
        public ActionResult DisplayBomReport(int fgitemid, double prdqty=1)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (fgitemid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Finished Item not selected!</h1></a>");
            }
            setViewData();
            bool viewper = mc.getPermission(Entry.Import_List, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Import_List, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "MaterialRequirementReport/MaterialRequirementReportHTML";
                string reportpms = "fgitemid=" + fgitemid + "";
                reportpms += "&prdqty=" + prdqty + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("MaterialRequirementReportHTML", new { itemid = fgitemid, prdqty = prdqty });
        }

        //get
        public ActionResult MaterialRequirementReportHTML(int itemid, double prdqty)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            if (itemid == 0)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Purchase number not selected!</h1></a>");
            }
            setViewData();

            BomBLL rptBLL = new BomBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.getMaterialRequirementReportHtml(itemid, prdqty);

            ReportModelObject.ReportHeader = ds.Tables[0].Rows[0]["rptheader"].ToString();

            //report content
            string lev = "";
            string fgitem = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Level</th>");
            sb.Append("<th style='width:auto;'>FG/SA&nbsp;Item</th>");
            sb.Append("<th style='width:auto;'>SA/RM&nbsp;Item</th>");
            sb.Append("<th style='width:15px;text-align:right;'>RM&nbsp;Qty</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Usage</th>");
            sb.Append("<th style='width:15px;text-align:right;'>Prd.&nbsp;Usage</th>");
            sb.Append("<th style='width:15px;'>FG&nbsp;Unit</th>");
            sb.Append("<th style='width:15px;'>RM&nbsp;Unit</th>");
            sb.Append("<th style='width:15px;text-align:right;'>W.&nbsp;Qty</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                if (lev != ds.Tables["tbl1"].Rows[i]["Lev"].ToString())
                {
                    lev = ds.Tables["tbl1"].Rows[i]["Lev"].ToString();
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["Lev"].ToString() + "</td>");
                }
                else
                {
                    sb.Append("<td>&nbsp;</td>");//note
                }
                if (fgitem != ds.Tables["tbl1"].Rows[i]["FGItem"].ToString())
                {
                    fgitem = ds.Tables["tbl1"].Rows[i]["FGItem"].ToString();
                    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["FGItem"].ToString() + "</td>");
                }
                else
                {
                    sb.Append("<td>&nbsp;</td>");//note
                }
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["RMItem"].ToString() + "</td>");
                sb.Append("<td align='right'>" + ds.Tables["tbl1"].Rows[i]["RMQty"].ToString() + "</td>");
                sb.Append("<td align='right'>" + ds.Tables["tbl1"].Rows[i]["Usage"].ToString() + "</td>");
                sb.Append("<td align='right'>" + ds.Tables["tbl1"].Rows[i]["PrdUsage"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["FGUnitName"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["RMUnitName"].ToString() + "</td>");
                sb.Append("<td align='right'>" + ds.Tables["tbl1"].Rows[i]["WasteQty"].ToString() + "</td>");
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        [HttpGet]
        public ActionResult getMaterialRequirementReport(int itemid, string itemcode, int rqty, int compcode)
        {
            //[100106]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "MaterialRequirementList.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{usp_get_lic_worksheet_report;.age} <= 58";
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "For- " + itemcode;
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for --usp_get_material_requirement_list
            rptDoc.SetParameterValue("@FgItemId", itemid);
            rptDoc.SetParameterValue("@RequiredQty", rqty);
            rptDoc.SetParameterValue("@CompCode", compcode);
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
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

        #endregion

        #region material requirement summary

        [HttpGet]
        public ActionResult MaterialRequirementSummary()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBLL.getObjectList(), "compcode", "cmpname");
            rptOption.DateFrom = DateTime.Now;
            rptOption.DateTo = DateTime.Now.AddMonths(1);
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult MaterialRequirementSummaryReport(rptOptionMdl rptOption)
        {
            //[100107]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Import_List, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Import_List, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "MaterialRequirementReport/getMaterialRequirementSummary";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&ccode=" + rptOption.CompCode + "";
                reportpms += "&fgitemid=" + rptOption.FgItemId + "";
                reportpms += "&rmitemid=" + rptOption.RmItemId + "";
                reportpms += "&fpname=" + rptOption.FgItemCode + "";
                reportpms += "&rmname=" + rptOption.RmItemCode + "";
                reportpms += "&filterbydt=" + rptOption.FilterByDT + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getMaterialRequirementSummary", new { ccode = rptOption.CompCode, dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, fgitemid = rptOption.FgItemId, rmitemid = rptOption.RmItemId, fpname=rptOption.FgItemCode, rmname=rptOption.RmItemCode, filterbydt=rptOption.FilterByDT });
        }

        [HttpGet]
        public ActionResult getMaterialRequirementSummary(int ccode, DateTime dtfrom, DateTime dtto, int fgitemid=0, int rmitemid=0, string fpname="", string rmname="", bool filterbydt=false )
        {
            //[100107]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "MaterialRequirementSummaryRpt.rpt"));
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            //rptDoc.RecordSelectionFormula = "{usp_get_lic_worksheet_report;.age} <= 58";
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Required for all pending supplies as on date: "+mc.getStringByDate(DateTime.Now);
            if (rmname.Length == 0) { rmitemid=0;};
            if (fpname.Length == 0) { fgitemid = 0; };
            if (filterbydt == true)
            {
                txtrpthead.Text = "Required for Pending Supplies From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            }
            if (fpname.Length > 0)
            {
                txtrpthead.Text += "\r\nFinished Product : " + fpname;
            }
            if (rmname.Length > 0)
            {
                txtrpthead.Text += "\r\nRaw Material : " + rmname;
            }
            //rptDoc.Subreports[0].SetDataSource(ds);
            //dbp parameters for --usp_get_material_requirement_summary
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@filterbydt", filterbydt);
            rptDoc.SetParameterValue("@fgitemid", fgitemid);
            rptDoc.SetParameterValue("@rmitemid", rmitemid);
            rptDoc.SetParameterValue("@compcode", ccode);
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
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            //return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
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
