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
    public class ItemMasterReportController : Controller
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

            var itypelist = mc.getItemTypeList();
            //var st = itypelist.Find(c => c.Value == "0");//all
            //itypelist.Remove(st);
            ViewBag.ItemTypeList = new SelectList(itypelist, "Value", "Text");

            rptOption.CompCode = Convert.ToInt32(objCookie.getCompCode());
            return View(rptOption);
        }

        [HttpPost]
        public ActionResult DisplayReport(rptOptionMdl rptOption)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            bool viewper = mc.getPermission(Entry.Item_Master_Report, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Item_Master_Report, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (rptOption.HsnCode == null) { rptOption.HsnCode = ""; };
            if (rptOption.GroupName == null) { rptOption.GroupId = 0; };
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "ItemMasterReport/GetReportHTML";
                string reportpms = "itemtype=" + rptOption.ItemType + "";
                reportpms += "&groupid=" + rptOption.GroupId + "";
                reportpms += "&status=" + rptOption.Status + "";
                reportpms += "&ccode=" + rptOption.CompCode + "";
                reportpms += "&hsncode=" + rptOption.HsnCode + "";
                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("GetReportHTML", new { itemtype = rptOption.ItemType, groupid = rptOption.GroupId, status=rptOption.Status, hsncode=rptOption.HsnCode, ccode=rptOption.CompCode });
        }

        //get
        public ActionResult GetReportHTML(string itemtype, int groupid, string status, int ccode, string hsncode="")
        {
            //from dbProcedures/ItemMasterRPT_SP.sql
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            ItemBLL rptBLL = new ItemBLL();
            ReportMdl ReportModelObject = new ReportMdl();
            DataSet ds = new DataSet();
            ds = rptBLL.GetItemReportHtml(itemtype, groupid, status, hsncode, ccode);
            //
            ReportModelObject.ReportHeader = ds.Tables["tbl"].Rows[0]["rptheader"].ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table class='tblcontainer' style='width:100%;'>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style='width:15px;'>SlNo</th>");
            sb.Append("<th style='width:15px;'>ItemCode</th>");
            sb.Append("<th style='width:15px;'>ShortName</th>");
            sb.Append("<th style='width:15px;'>Group</th>");
            sb.Append("<th style='width:15px;text-align:center;'>PurchaseRate</th>");
            sb.Append("<th style='width:15px;text-align:center;'>SaleRate</th>");
            sb.Append("<th style='width:15px;'>UnitName</th>");
            sb.Append("<th style='width:15px;'>Status</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            for (int i = 0; i < ds.Tables["tbl1"].Rows.Count; i++)
            {
                //tr-1/3
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td>" + (i + 1).ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["itemcode"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["shortname"].ToString() + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["groupname"].ToString() + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["purchaserate"].ToString())) + "</td>");
                sb.Append("<td align='right'>" + mc.getINRCFormat(Convert.ToDouble(ds.Tables["tbl1"].Rows[i]["salerate"].ToString())) + "</td>");
                sb.Append("<td>" + ds.Tables["tbl1"].Rows[i]["unitname"].ToString() + "</td>");
                if (Convert.ToBoolean(ds.Tables["tbl1"].Rows[i]["isactive"].ToString()) == true)
                {
                    sb.Append("<td>In Use</td>");
                }
                else
                {
                    sb.Append("<td>Deleted</td>");
                }
                sb.Append("</tr>");
                //tr-2/3
                sb.Append("<tr class='tblrow'>");
                sb.Append("<td></td>");
                sb.Append("<td colspan='7'>Item Name: " + ds.Tables["tbl1"].Rows[i]["itemname"].ToString() + "</td>");
                sb.Append("</tr>");
                //tr-3/3
                if(ds.Tables["tbl1"].Rows[i]["Specification"].ToString().Length>0)
                {
                    sb.Append("<tr class='tblrow'>");
                    sb.Append("<td></td>");
                    sb.Append("<td colspan='7'>Specification: " + ds.Tables["tbl1"].Rows[i]["Specification"].ToString() + "</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table><br/>");
            ReportModelObject.ReportContent = sb.ToString();
            //
            ReportModelObject.RunDate = mc.getDateTimeString(DateTime.Now);
            ReportModelObject.ReportUser = "[" + objCookie.getUserId() + "]/" + objCookie.getUserName();
            return View(ReportModelObject);
        }

    }
}
