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
    public class InvoiceControlReportController : Controller
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
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            ViewBag.RptList = new SelectList(mc.getInvCtrlRptList(), "Value", "Text");
            ViewBag.StatusList = new SelectList(mc.getInvCtrlStatusList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getDispToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        #region invoice unloading report

        [HttpPost]
        public ActionResult DisplayUnloadingReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "InvoiceControlReport/GetReportHTML";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&rptfor=" + rptOption.RptFor + "";
                reportpms += "&rptopt=" + rptOption.RptOpt + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, potype = rptOption.POType, rptfor = rptOption.RptFor, rptopt = rptOption.RptOpt });
        }

        //get
        public ActionResult GetReportHTML(string potype, DateTime dtfrom, DateTime dtto, string rptfor, string rptopt, int compcode)
        {
            //from dbProcedures/InvoiceControlRPT_SP.sql
            setViewData();
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }

            //dataset from dbProcedures/InvoiceControlRPT_SP.sql
            InvoiceControlBLL rptBLL = new InvoiceControlBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.InvoiceControlReportHtml(potype,dtfrom,dtto,rptfor,rptopt,compcode);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            sbHeader.Append("<div style='text-align:center;'>");//div main
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["CmpName"].ToString());
            sbHeader.Append("</div>");
            //cmp address-n/a
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>"+ ds.Tables["tbl"].Rows[0]["rptname"].ToString() + "</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            ArrayList arlcmp = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (arlcmp.Contains(ds.Tables["tbl1"].Rows[i]["cmpname"].ToString()) == false)
                {
                    arlcmp.Add(ds.Tables["tbl1"].Rows[i]["cmpname"].ToString());
                }
            }

            //report content
            int cnt = 0;
            int cntr = 0;
            string rptstatus = "";
            double rptamount = 0;
            double cmpamount = 0;
            double totalamount = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>Railway</th>");
            sb.Append("<th style='width:135px;'>Case&nbsp;File</th>");
            sb.Append("<th style='width:15px;'>Invoice&nbsp;No</th>");
            sb.Append("<th style='width:15px;'>Invoice&nbsp;Date</th>");
            sb.Append("<th style='width:auto;'>Paying&nbsp;Authority</th>");
            sb.Append("<th style='width:auto;'>Consignee</th>");
            sb.Append("<th style='width:15px;text-align:right;'>"+ ds.Tables["tbl"].Rows[0]["amountlbl"].ToString() + "</th>");
            sb.Append("<th style='width:15px;'>Status</th>");
            sb.Append("<th style='width:15px;'>" + ds.Tables["tbl"].Rows[0]["datelbl"].ToString() + "</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int x = 0; x < arlcmp.Count; x++)
            {
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td style='font-size:11pt;text-align:center;' colspan='10'><b><u>" + arlcmp[x].ToString() + "</u></b></td>");
                sb.Append("</tr>");
                cmpamount = 0;
                cnt = 1;
                for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
                {
                    if (arlcmp[x].ToString() == ds.Tables["tbl1"].Rows[i]["cmpname"].ToString())
                    {
                        sb.Append("<tr class='tblrow'>");
                        sb.Append("<td>" + cnt.ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["RailwayName"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["CaseFileNo"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["billno"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["vdate"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["PayingAuthName"].ToString() + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["ConsigneeName"].ToString() + "</td>");

                        rptamount = Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["rptamount"].ToString());
                        sb.Append("<td align='right'>" + mc.getINRCFormat(rptamount) + "</td>");
                        cmpamount += rptamount;
                        totalamount += rptamount;

                        rptstatus = "Pending";
                        if (ds.Tables["tbl1"].Rows[i]["rptstatus"].ToString().ToLower() == "true")
                        {
                            rptstatus = "Completed";
                        }
                        sb.Append("<td>" + rptstatus + "</td>");
                        sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["rptdate"].ToString() + "</td>");
                        sb.Append("</tr>");

                        if (rptfor.ToLower() == "un")
                        {
                            sb.Append("<tr class='tblrow'>");//tr-sub
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td colspan='9'>Item(s)<br/>");//td-sub
                            //sub-rpt
                            sb.Append("<table class='tblcontainer' style='width:100%;'>");
                            sb.Append("<thead>");
                            sb.Append("<tr>");
                            sb.Append("<th style='width:15px;'>SlNo</th>");
                            sb.Append("<th style='width:15px;'>Item&nbsp;Code</th>");
                            sb.Append("<th style='width:auto;'>Short&nbsp;Name</th>");
                            sb.Append("<th style='width:15px;'>DP</th>");
                            sb.Append("</tr>");
                            sb.Append("</thead>");
                            cntr = 1;
                            for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
                            {
                                if (ds.Tables["tbl1"].Rows[i]["salerecid"].ToString() == ds.Tables["tbl2"].Rows[j]["salerecid"].ToString())
                                {
                                    sb.Append("<tr class='tblrow'>");
                                    sb.Append("<td>" + cntr.ToString() + "</td>");
                                    sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["itemcode"].ToString() + "</td>");
                                    sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["ShortName"].ToString() + "</td>");
                                    sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["DelvDate"].ToString() + "</td>");
                                    sb.Append("</tr>");
                                    cntr += 1;
                                }
                            }
                            sb.Append("</table>");//cl-subrpt
                            sb.Append("</td>");//cl-td-sub
                            sb.Append("</tr>");//cl-tr-sub
                        }
                        else
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td colspan='9'>Item(s): " + ds.Tables["tbl1"].Rows[i]["items"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }

                        if (ds.Tables["tbl1"].Rows[i]["RCInfo"].ToString().Length > 0)
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td colspan='9'>Remarks: " + ds.Tables["tbl1"].Rows[i]["RCInfo"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }

                        if (ds.Tables["tbl1"].Rows[i]["trpdetail"].ToString().Length > 0)
                        {
                            sb.Append("<tr class='tblrow'>");
                            sb.Append("<td>&nbsp;</td>");
                            sb.Append("<td colspan='9'>Transport Detail: " + ds.Tables["tbl1"].Rows[i]["trpdetail"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                        cnt += 1;
                    }
                }
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>&nbsp;</td>");
                sb.Append("<td colspan='9' align='right'><b>Total for " + arlcmp[x].ToString() + ": " + mc.getINRCFormat(cmpamount) + "</b></td>");
                sb.Append("</tr>");
            }
            //
            sb.Append("<tr class='tblrow'>");
            sb.Append("<td colspan='10' align='right'><b>Grand Total Amount: " + mc.getINRCFormat(totalamount) + "</b></td>");
            sb.Append("</tr>");
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        #endregion

        #region invoice control report

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            //[100159]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Sales_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Sales_Report, permissionType.Edit);
            if (downloadper == false) { rptOption.ReportFormat = "pdf"; };
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "InvoiceControlReport/getInvControlReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&potype=" + rptOption.POType + "";
                reportpms += "&rptfor=" + rptOption.RptFor + "";
                reportpms += "&rptopt=" + rptOption.RptOpt + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getInvControlReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, potype = rptOption.POType, rptfor = rptOption.RptFor, rptopt = rptOption.RptOpt });
        }

        [HttpGet]
        public FileResult getInvControlReport(DateTime dtfrom, DateTime dtto, int compcode, string potype, string rptfor, string rptopt)
        {
            //[100159]
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //CrystalDecisions.CrystalReports.Engine.ReportDocument rptDocSub = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "InvoiceControlRpt.rpt"));//TestXCrystalReport1
            //rptDocSub.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "OrderFooter.rpt"));
            setLoginInfo(rptDoc);
            //setLoginInfo(rptDocSub);
            CrystalDecisions.CrystalReports.Engine.TextObject txtrpthead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtrpthead"];
            txtrpthead.Text = "Invoice Date";
            string statusname = "";
            if (rptopt == "a")
            {
                statusname = "All";
            }
            else if (rptopt == "p")
            {
                statusname = "Pending";
            }
            else if (rptopt == "c")
            {
                statusname = "Completed";
            }
            if(rptopt=="c")
            {
                if (rptfor == "un")
                {
                    txtrpthead.Text = "Unloading Date";
                }
                else if (rptfor == "rc")
                {
                    txtrpthead.Text = "Receipted Challan Receiving Date";
                }
                else if (rptfor == "rn")
                {
                    txtrpthead.Text = "R Note Receiving Date";
                }
                else if (rptfor == "bsp1")
                {
                    txtrpthead.Text = "Part 1 Bill Submission Date";
                }
                else if (rptfor == "bsp2")
                {
                    txtrpthead.Text = "Part 2 Bill Submission Date";
                }
            }
            txtrpthead.Text += " From " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            txtrpthead.Text += ", Status: "+statusname;
            //dbp parameters
            rptDoc.SetParameterValue("@dtfrom", dtfrom);
            rptDoc.SetParameterValue("@dtto", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@potype", potype);
            rptDoc.SetParameterValue("@rptfor", rptfor);
            rptDoc.SetParameterValue("@rptopt", rptopt);
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
