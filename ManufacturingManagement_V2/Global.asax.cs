﻿using ManufacturingManagement_V2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ManufacturingManagement_V2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            //Database.SetInitializer<ErpConnection>(new DropCreateDatabaseIfModelChanges<ErpConnection>());
            Database.SetInitializer<ErpConnection>(null);

        }

        //protected void Application_BeginRequest()
        //{
        //    CultureInfo cInf = new CultureInfo("en-IN", false);
        //    // NOTE: change the culture name en-ZA to whatever culture suits your needs

        //    cInf.DateTimeFormat.DateSeparator = "/";
        //    cInf.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        //    cInf.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss tt";

        //    System.Threading.Thread.CurrentThread.CurrentCulture = cInf;
        //    System.Threading.Thread.CurrentThread.CurrentUICulture = cInf;
        //}

    }
}