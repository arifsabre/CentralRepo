using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Reports
{
    public partial class DisplayIFrameRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (clsPermission.Instance.getPermission(Entry.Marketing_HOD_Dashboard, permissionType.Add) == false)
            //{
            //    Response.Redirect("../login.aspx");
            //}
            if (Page.IsPostBack == false)
            {
                //objProgramme.IncludeJS(this, 1);
            }
            if (Session["arliframeopt"] != null)
            {
                //ArrayList arl = new ArrayList();
                //arl = (ArrayList)Session["arliframeopt"];
                //lblRptName.Text = arl[0].ToString();
                //frmRpt.Attributes.Add("src", arl[1].ToString());
                //hfOptionURL.Value = arl[2].ToString();
                //hfReturnURL.Value = arl[3].ToString();
                //Session.Remove("arliframeopt");
                //setIFrameHeight();
                //lblUser.Text = "[" + objCoockie.getUserName() + "]";
            }
            //string returl = Request.QueryString["returl"] != null ? Request.QueryString["returl"].ToString() : "";
            //hfReturnURL.Value = returl;
            //string opturl = Request.QueryString["opturl"] != null ? Request.QueryString["opturl"].ToString() : "";
            //hfOptionURL.Value = opturl;
            //string user = Request.QueryString["user"] != null ? Request.QueryString["user"].ToString() : "";
            //lblUser.Text = user;
            //string rptname = Request.QueryString["rptname"] != null ? Request.QueryString["rptname"].ToString() : "";
            //lblRptName.Text = rptname;
            //string rpturl = Request.QueryString["rpturl"] != null ? Request.QueryString["rpturl"].ToString() : "";
            ////lblRptName.Text = rptname;
            ////frmRpt.Attributes.Add("src", "http://192.168.0.218:85/RptDetail/DownloadDocumentFile.aspx?filename=.pdf&filepath=CaseFile&No=123");
            frmRpt.Attributes.Add("src", "../SalaryReport/displayDefaultReport");
            setIFrameHeight(); 
        }

        private void setIFrameHeight()
        {
            HttpContext hc = HttpContext.Current;
            int x = hc.Request.Browser.ScreenPixelsWidth;
            int y = hc.Request.Browser.ScreenPixelsHeight + 110;
            frmRpt.Attributes.Add("style", "width:100%;height:" + y.ToString() + "px;");
        }

        protected void lbOption_Click(object sender, EventArgs e)
        {
            if (hfOptionURL.Value.Length > 0)
            {
                Response.Redirect(hfOptionURL.Value);
            }
            else
            {
                Response.Redirect("../Home/Index");
            }
        }

        protected void lblRetuen_Click(object sender, EventArgs e)
        {
            if (hfReturnURL.Value.Length > 0)
            {
                Response.Redirect(hfReturnURL.Value);
            }
            else
            {
                Response.Redirect("../Home/Index");
            }
        }

        protected void lblLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Login/LoginUser");
        }
    }
}