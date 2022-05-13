
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using ManufacturingManagement_V2.Models;
using System.Data;
using System.Text;
using System.Collections;

namespace ManufacturingManagement_V2.Controllers
{
    public class WarrantyRedressalReportController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBll = new CompanyBLL();

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
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            rptOptionMdl rptOption = new rptOptionMdl();
            ViewBag.CompanyList = new SelectList(compBll.getObjectList(), "compcode", "cmpname");
            ViewBag.POTypeList = new SelectList(mc.getPOTypeRptList(), "Value", "Text");
            ViewBag.OrderStatusList = new SelectList(mc.getOrderStatusList(), "Value", "Text");
            ViewBag.ReportFormatList = new SelectList(mc.getReportFormatList(), "Value", "Text");
            rptOption.DateFrom = objCookie.getFromDate();
            rptOption.DateTo = objCookie.getToDate();
            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Marketing_PO_Report, permissionType.Edit);
            if (rptOption.GroupName == null) { rptOption.GroupId = 0; }
            if (rptOption.ItemCode == null) { rptOption.ItemId = 0; }
            if (rptOption.RailwayName == null) { rptOption.RailwayId = 0; }
            if (rptOption.EmpName == null) { rptOption.NewEmpId = 0; }
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "WarrantyRedressalReport/getReport";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "&dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&itemid=" + rptOption.ItemId + "";
                reportpms += "&railwayid=" + rptOption.RailwayId + "";
                reportpms += "&orderstatus=" + rptOption.OrderStatus + "";
                reportpms += "&compcode=" + rptOption.CompCode + "";
                reportpms += "&groupname=" + rptOption.GroupName + "";
                reportpms += "&itemcode=" + rptOption.ItemCode + "";
                reportpms += "&railwayname=" + rptOption.RailwayName + "";
                reportpms += "&consigneeid=" + rptOption.NewEmpId + "";
                reportpms += "&consigneename=" + rptOption.EmpName + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("getReport", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, groupid = rptOption.GroupId, itemid = rptOption.ItemId, railwayid = rptOption.RailwayId, orderstatus = rptOption.OrderStatus, compcode = rptOption.CompCode, groupname = rptOption.GroupName, itemcode = rptOption.ItemCode, railwayname = rptOption.RailwayName, consigneeid = rptOption.NewEmpId, consigneename = rptOption.EmpName });
        }

        //get
        public FileResult getReport(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string orderstatus, int compcode, string groupname, string itemcode, string railwayname, int consigneeid, string consigneename)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return File(Server.MapPath("~/App_Data/EmpDocs/") + "/blank/Permission.png", "png");
            }
            //
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports/MarketingPORPT/"), "WarrantyRedressalRpt.rpt"));
            setLoginInfo(rptDoc);
            //
            CrystalDecisions.CrystalReports.Engine.TextObject txtRptHead = (CrystalDecisions.CrystalReports.Engine.TextObject)rptDoc.ReportDefinition.Sections["Section1"].ReportObjects["txtRptHead"];
            txtRptHead.Text = "PO Date From : " + mc.getStringByDate(dtfrom) + " To " + mc.getStringByDate(dtto);
            if (orderstatus != "0")
            {
                string osname = mc.getNameByKey(mc.getOrderStatus(), "status", orderstatus, "statusname");
                txtRptHead.Text += ", Order Status: " + osname;
            }
            if (railwayid > 0)
            {
                txtRptHead.Text += ", Railway : " + railwayname;
            }
            if (consigneeid > 0)
            {
                txtRptHead.Text += ", Consignee : " + consigneename;
            }
            if (groupid > 0)
            {
                txtRptHead.Text += ", Group : " + groupname;
            }
            if (itemid > 0)
            {
                txtRptHead.Text += ", Item : " + itemcode;
            }
            //dbp
            rptDoc.SetParameterValue("@podtfrom", dtfrom);
            rptDoc.SetParameterValue("@podtto", dtto);
            rptDoc.SetParameterValue("@GroupId", groupid);
            rptDoc.SetParameterValue("@ItemId", itemid);
            rptDoc.SetParameterValue("@railwayid", railwayid);
            rptDoc.SetParameterValue("@consigneeid", consigneeid);
            rptDoc.SetParameterValue("@orderstatus", orderstatus);
            rptDoc.SetParameterValue("@compcode", compcode);
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

        //get
        public ActionResult GetReportHTML(DateTime dtfrom, DateTime dtto, int groupid, int itemid, int railwayid, string orderstatus, int compcode, bool addinv, int consigneeid)
        {
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            ERP_V1_ReportBLL rptBLL = new ERP_V1_ReportBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            //ds = rptBLL.GetWarrantyRedressalReportHtml(dtfrom,dtto,groupid,itemid,railwayid,orderstatus,compcode,consigneeid,addinv);

            //report header
            System.Text.StringBuilder sbHeader = new System.Text.StringBuilder();
            //company
            sbHeader.Append("<div style='font-size:12pt;'>");
            sbHeader.Append("PRAG GROUP");
            sbHeader.Append("</div>");
            //cmp address
            //repoprt name
            sbHeader.Append("<div style='font-size:11pt;background-color:lightgray;'>");
            sbHeader.Append("<b><u>Warranty Redressal Report</u></b>");
            sbHeader.Append("</div>");
            //report filters
            sbHeader.Append("<div style='font-size:10pt;text-align:left;'><br/>");
            sbHeader.Append(ds.Tables["tbl"].Rows[0]["RptFilters"].ToString());
            sbHeader.Append("</div>");
            sbHeader.Append("</div>");//div main
            ReportModelObject.ReportHeader = sbHeader.ToString();

            //groups
            ArrayList arlCompany = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlCompany.Contains(ds.Tables["tbl1"].Rows[i]["compcode"].ToString()))
                {
                    arlCompany.Add(ds.Tables["tbl1"].Rows[i]["compcode"].ToString());
                }
            }
            ArrayList arlRailway = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlRailway.Contains(ds.Tables["tbl1"].Rows[i]["railwayid"].ToString()))
                {
                    arlRailway.Add(ds.Tables["tbl1"].Rows[i]["railwayid"].ToString());
                }
            }
            ArrayList arlConsg = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlConsg.Contains(ds.Tables["tbl1"].Rows[i]["consigneeid"].ToString()))
                {
                    arlConsg.Add(ds.Tables["tbl1"].Rows[i]["consigneeid"].ToString());
                }
            }
            ArrayList arlGroup = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlGroup.Contains(ds.Tables["tbl1"].Rows[i]["groupid"].ToString()))
                {
                    arlGroup.Add(ds.Tables["tbl1"].Rows[i]["groupid"].ToString());
                }
            }
            ArrayList arlItem = new ArrayList();
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                if (!arlItem.Contains(ds.Tables["tbl1"].Rows[i]["itemid"].ToString()))
                {
                    arlItem.Add(ds.Tables["tbl1"].Rows[i]["itemid"].ToString());
                }
            }

            //processing
            System.Text.StringBuilder sbGrp = new System.Text.StringBuilder();
            string rptgrp = "";
            for (int i = 0; i < arlCompany.Count; i++)
            {
                //company-group
                rptgrp = arlCompany[i].ToString();
                sbGrp.Append("<div style='font-size:12pt;'>");
                sbGrp.Append(GetNameByIdFromDS("compcode","cmpname", arlCompany[i].ToString(),ds));
                
                //railway-group
                for (int j = 0; j < arlRailway.Count; j++)
                {
                    sbGrp.Append("<div style='font-size:12pt;'>");
                    sbGrp.Append(GetNameByIdFromDS("railwayid", "railwayname", arlRailway[j].ToString(), ds));

                    //consignee-group
                    for (int k = 0; k < arlConsg.Count; k++)
                    {
                        sbGrp.Append("<div style='font-size:12pt;'>");
                        sbGrp.Append(GetNameByIdFromDS("consigneeid", "consigneename", arlConsg[k].ToString(), ds));

                        //item-group
                        for (int l = 0; l < arlGroup.Count; l++)
                        {
                            sbGrp.Append("<div style='font-size:12pt;'>");
                            sbGrp.Append(GetNameByIdFromDS("groupid", "groupname", arlGroup[l].ToString(), ds));

                            //item
                            for (int m = 0; m < arlItem.Count; m++)
                            {
                                sbGrp.Append("<div style='font-size:12pt;'>");
                                sbGrp.Append(GetNameByIdFromDS("itemid", "item", arlItem[m].ToString(), ds));

                                sbGrp.Append("Item report here");//space to display report

                                sbGrp.Append("</div>");//item
                            }
                            //

                            sbGrp.Append("</div>");//item-group
                        }
                        //

                        sbGrp.Append("</div>");//consignee-group
                    }
                    //

                    sbGrp.Append("</div>");//railway-group
                }
                //

                sbGrp.Append("</div>");//company-group
            }

            //report content
            //int detslno = 0;
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.Append("<table class='tblcontainer' style='width:100%;'>");
            //sb.Append("<thead>");
            //sb.Append("<tr>");
            //sb.Append("<th style='width:15px;'>SlNo</th>");
            //sb.Append("<th style='width:15px;'>Company</th>");
            //sb.Append("<th style='width:15px;'>Railway</th>");
            //sb.Append("<th style='width:auto;'>Group</th>");
            //sb.Append("<th style='width:auto;'>Item</th>");
            //sb.Append("<th style='width:auto;'>PO&nbsp;No&nbsp;&&nbsp;Date</th>");
            //sb.Append("<th style='width:15px;text-align:right;'>Order&nbsp;Qty</th>");
            //sb.Append("<th style='width:15px;text-align:right;'>Supplied&nbsp;Qty</th>");
            //sb.Append("<th style='width:15px;text-align:right;'>Balance&nbsp;Qty</th>");
            //sb.Append("<th style='width:15px;text-align:right;'>Unit</th>");
            //sb.Append("</tr>");
            //sb.Append("</thead>");
            //for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            //{
            //    //tr-1/2
            //    sb.Append("<tr class='tblrow'>");
            //    sb.Append("<td>" + (i + 1).ToString() + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["cmpname"].ToString() + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["rlyconsg"].ToString().Trim() + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["groupname"].ToString() + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["item"].ToString() + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["ponumdate"].ToString() + "</td>");
            //    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["ordqty"].ToString())) + "</td>");
            //    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["dspqty"].ToString())) + "</td>");
            //    sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["remqty"].ToString())) + "</td>");
            //    sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["unitname"].ToString() + "</td>");
            //    sb.Append("</tr>");
            //    //tr-2/2
            //    if (addinv == true)
            //    {
            //        sb.Append("<tr class='tblrow'>");
            //        sb.Append("<td></td>");//slno
            //        sb.Append("<td colspan='9'>");
            //        sb.Append("<table class='tblcontainer' style='width:50%;' align='right'>");
            //        sb.Append("<thead>");
            //        sb.Append("<tr>");
            //        sb.Append("<th style='width:15px;'>SlNo</th>");
            //        sb.Append("<th style='width:15px;'>InvoiceNO</th>");
            //        sb.Append("<th style='width:15px;'>Invoice&nbsp;Date</th>");
            //        sb.Append("<th style='width:15px;text-align:right;'>Qty</th>");
            //        sb.Append("<th style='width:15px;'>Dispatch&nbsp;Memo</th>");
            //        sb.Append("<th style='width:15px;'>Insp.&nbsp;Certificate</th>");
            //        sb.Append("<th style='width:15px;'>Remarks</th>");
            //        sb.Append("</tr>");
            //        sb.Append("</thead>");
            //        detslno = 1;
            //        for (int j = 0; j < ds.Tables["tbl2"].Rows.Count; j++)
            //        {
            //            if (ds.Tables["tbl2"].Rows[j]["link"].ToString() == ds.Tables["tbl1"].Rows[i]["link"].ToString())
            //            {
            //                sb.Append("<tr class='tblrow'>");
            //                sb.Append("<td>" + detslno.ToString() + "</td>");
            //                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["InvNo"].ToString() + "</td>");
            //                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["VDate"].ToString() + "</td>");
            //                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl2"].Rows[j]["qty"].ToString())) + "</td>");
            //                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["dispatchmemo"].ToString().Trim() + "</td>");
            //                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["inspcertificate"].ToString().Trim() + "</td>");
            //                sb.Append("<td>" + ds.Tables["tbl2"].Rows[j]["rcinfo"].ToString() + "</td>");
            //                sb.Append("</tr>");
            //                detslno += 1;
            //            }
            //        }
            //        sb.Append("</table>");
            //        sb.Append("</td>");
            //        sb.Append("</tr>");
            //    }
            //}
            //sb.Append("</table><br/>");
            //ReportModelObject.ReportContent = sb.ToString();

            ReportModelObject.ReportContent = sbGrp.ToString();

            //report footer
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

        private string GetNameByIdFromDS(string idcol, string namecol, string idvalstr, DataSet ds)
        {
            string namestr = "";
            for (int x = 0; x < ds.Tables["tbl1"].Rows.Count; x++)
            {
                if (ds.Tables["tbl1"].Rows[x][idcol].ToString() == idvalstr)
                {
                    namestr= ds.Tables["tbl1"].Rows[x][namecol].ToString();
                    break;
                }
            }
            return namestr;
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

    }
}
