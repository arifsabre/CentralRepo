using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Reports
{
    public partial class zzzCHK_WebForm : System.Web.UI.Page
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //objProgramme.IncludeJS(this, 1);
                Label1.Text= objCookie.getMenu();
            }
            //ReturnToErpV1(), in reverse of this method, is located in HomeController
            Label1.Text = objCookie.getMenu();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = "OK AGAIN";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Label1.Text = "OK";
        }
    }
}