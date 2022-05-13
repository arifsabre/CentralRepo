using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManufacturingManagement_V2.Reports
{
    public partial class GetFromErpV1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //objProgramme.IncludeJS(this, 1);
            }
            //ReturnToErpV1(), in reverse of this method, is located in HomeController
            string uid = Request.QueryString["uid"] != null ? Request.QueryString["uid"].ToString() : "";
            string ccode = Request.QueryString["ccode"] != null ? Request.QueryString["ccode"].ToString() : "";
            string fy = Request.QueryString["fy"] != null ? Request.QueryString["fy"].ToString() : "";
            string pa = Request.QueryString["pa"] != null ? Request.QueryString["pa"].ToString() : "";
            ManufacturingManagement_V2.Models.LoginBLL loginBLL = new ManufacturingManagement_V2.Models.LoginBLL();
            loginBLL.performLogin(uid, ccode, fy, "123",true, "Marketing");
            Response.Redirect(pa.Replace('*','&'));// =page/action as "../Home/Index"
        }

    }
}