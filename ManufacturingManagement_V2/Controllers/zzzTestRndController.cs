using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Collections;
using System.Data;
using ManufacturingManagement_V2.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ManufacturingManagement_V2.Controllers
{
    public class zzzTestRndController : Controller
    {
        //
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        //not to be used in application -it's just for sample programme
        public ActionResult Index()
        {
            //notes-------
            //[HttpPost, ActionName("Index")] to use same action with different method name
            //end-notes---
            return Content("<xml>This is poorly formatted xml.</xml>", "text/xml");//just to return formatted content
        }

        //not to be used in application -it's just for sample programme
        public ActionResult Index1()
        {
            //notes-------
            //[HttpPost, ActionName("Index")] to use same action with different method name
            //end-notes---
            return View();
        }

        #region api work
        /// <summary>
        /// Get API Test Method
        /// </summary>
        /// <returns></returns>
        // GET: Student
        public ActionResult IndexGetAPI()
        {
            setViewData();
            
            IEnumerable<EmployeeMdl> students = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58521/api/StudentAPI/");
                
                //HTTP GET
                var responseTask = client.GetAsync("GetStaffList");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<EmployeeMdl>>();
                    readTask.Wait();

                    students = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    students = Enumerable.Empty<EmployeeMdl>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(students);
        }

        [HttpGet]
        public ActionResult IndexPostAPI(string msg = "")
        {
            setViewData();
            EmployeeMdl empmdl = new EmployeeMdl();
            empmdl.EmpName = "ABCD";
            ViewBag.Message = msg;
            return View(empmdl);
        }

        [HttpGet]//Direct Method
        public ActionResult IndexGetDirect()
        {
            setViewData();

            IEnumerable<EInvoiceSaleMdl> invList = null;
            //working ok
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:52341/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//or application/xml

                //sample tokens passing
                //client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                //client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                //client.DefaultRequestHeaders.Add("gstin", "29AAACGIIIII1Z3");
                client.DefaultRequestHeaders.Add("authtoken", "KKKKLX7rcNYJB30g5DS2u8z");

                //HTTP GET
                var responseTask = client.GetAsync("Values");

                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<EInvoiceSaleMdl>>();
                    readTask.Wait();

                    invList = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    invList = new List<EInvoiceSaleMdl>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(invList);
        }

        /// <summary>
        /// Post API Test Method
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IndexPostAPI(EmployeeMdl student)
        {
            setViewData();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58521/api/StudentAPI/");
                client.DefaultRequestHeaders.Accept.Clear();

                //sample tokens passing
                client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                client.DefaultRequestHeaders.Add("gstin", "29AAACGIIIII1Z3");
                client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<EmployeeMdl>("SaveStaff", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("IndexPostAPI", new { msg = "OK" });
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(student);
        }

        //async task Get

        public async Task<ActionResult> IndexT1()
        {
            setViewData();

            await RunAsync();//ok
            //await RunAsync1();
            return View();
        }

        public static async Task<EmployeeMdl> RunAsync()
        {
            //EmployeeMdl exm = null;
            IEnumerable<EmployeeMdl> students = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58521/api/StudentAPI/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//or application/xml

                    //sample tokens passing
                    client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                    client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                    client.DefaultRequestHeaders.Add("gstin", "29AAACGIIIII1Z3");
                    client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                    //New code:
                    var response = await client.GetAsync("GetStaffList/?username=abcd&api_key=gdysdxmd34lk2bk3jb2kj").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var exm2 = await response.Content.ReadAsAsync<IEnumerable<EmployeeMdl>>();
                        students = exm2;
                    }
                    return students.FirstOrDefault();//note
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                return students.FirstOrDefault();//note
            }

        }

        public static async Task<EInvoiceSaleMdl> RunAsync1()
        {
            EInvoiceSaleMdl mdl = new EInvoiceSaleMdl();
            string[] x1 = { };
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58521/api/PRAG_ERP_API/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));//or application/json

                    //sample tokens passing
                    client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                    client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                    client.DefaultRequestHeaders.Add("salerecid", "30970");
                    client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                    //New code:
                    var response = await client.GetAsync("GetInvoiceResult").ConfigureAwait(false);
                    //var response = await client.GetAsync("Authenticate").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        mdl = await response.Content.ReadAsAsync<EInvoiceSaleMdl>();
                        //x1 = await response.Content.ReadAsAsync<string[]>();
                    }
                    
                    return mdl;
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                return mdl;
            }

        }

        #endregion api work

        public ActionResult CanvasChart()
        {
            //canvasjs.chart--working--ok
            setViewData();
            return View();
        }

        public ActionResult DisplayChart(string charttype = "Bar")
        {
            //chart.js--working--ok
            setViewData();

            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();
            ArrayList x1Value = new ArrayList();

            DataSet ds = new DataSet();
            SaleBLL bllObject = new SaleBLL();
            ds = bllObject.getChartData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                xValue.Add(ds.Tables[0].Rows[i]["GrowthYear"].ToString());
                yValue.Add(ds.Tables[0].Rows[i]["GrowthValue"].ToString());
                x1Value.Add(ds.Tables[0].Rows[i]["Name"].ToString());
            }

            //chartType: = column,Bar,Pie,Candlestick,Bubble,Doughnut,StackedBar100,BoxPlot
            //Pyramid,Polar,Radar,RangeBar,Funnel
            new Chart(width: 750, height: 550, theme: ChartTheme.Vanilla3D)
                    .AddTitle("Chart for Growth [Pie Chart]")
                    .AddLegend("Summary")
                    .AddSeries("Company1", chartType: charttype, xValue: xValue, yValues: yValue)
                    .AddLegend("Name")
                    .AddSeries("Company2", chartType: charttype, xValue: x1Value, yValues: yValue)//etc
                    .Write("bmp");

            return null;
        }

    }
}
