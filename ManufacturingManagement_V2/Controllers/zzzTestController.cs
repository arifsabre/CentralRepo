using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class zzzTestController : Controller
    {
        private EmployeeBLL bllObject = new EmployeeBLL();
        //
        // GET: /zzzTest/

        // GET: Home
        [HttpGet]//working to be revised for database
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]//with index
        public JsonResult Index(string Prefix)
        {
            List<EmployeeMdl> ObjList = new List<EmployeeMdl> { };
            ObjList = bllObject.getObjectList("s", 5);
            //Searching records from list using LINQ query  
            //var EmpDet = (from N in ObjList
              //            where N.EmpId.ToLower().StartsWith(Prefix) || N.EmpName.ToLower().StartsWith(Prefix.ToLower())
                //          select new { N.EmpName, N.EmpId });
            //var EmpDet = ObjList;
            //var EmpDet = ObjList.Where(s => s.EmpName.Contains(Prefix.ToLower())).Select(w => w).ToList();
            var EmpDet = ObjList.Where(s => s.EmpName.Contains(Prefix.ToLower()));

            //return Json(EmpDet, JsonRequestBehavior.AllowGet);
            return Json(EmpDet, JsonRequestBehavior.AllowGet);
        }

        //working
        public ActionResult Index2()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]//with index2
        public JsonResult AutoComplete2(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            //method 1 preparation of result by IList
            //string txt = "abc";
            //int vl = 9;
            //IList<SelectListItem> List = new List<SelectListItem>();
            //List.Add(new SelectListItem { Text = "test1", Value = "0" });
            //List.Add(new SelectListItem { Text = "test2", Value = "1" });
            //List.Add(new SelectListItem { Text = "test3", Value = "2" });
            //List.Add(new SelectListItem { Text = "test4", Value = "3" });
            //List.Add(new SelectListItem { Text = "Sanjay Kumar", Value = "4" });
            //List.Add(new SelectListItem { Text = "Sanjay Mishra", Value = "5" });
            //List.Add(new SelectListItem { Text = "Win 7", Value = "6" });
            //List.Add(new SelectListItem { Text = "Win 8", Value = "7" });
            //List.Add(new SelectListItem { Text = txt, Value = vl.ToString() });
            //foreach (var item in List)
            //{
            //    result.Add(new KeyValuePair<string, string>(item.Value.ToString(), item.Text));
            //}
            //end-method 1
            //method 2 
            //preparing list by database
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = bllObject.getObjectData("s",5);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["empid"].ToString(), ds.Tables[0].Rows[i]["empname"].ToString()));
            }
            //end-method 2
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]//with index2
        public JsonResult GetDetail(string id,ProductionEntryMdl pmdl)
        {
            zzzTestModel model = new zzzTestModel();
            EmployeeMdl objmdl = new EmployeeMdl();
            objmdl = bllObject.searchObject(id);
            model.id = id;
            model.name = objmdl.EmpName;
            model.mobile = objmdl.ContactNo;
            return Json(model);
        }

        //not- working
         public ActionResult Index5()
         {
             return View();
         }

        [HttpPost]//with index5
        public JsonResult getData(string term)
         {
             List<string> msp = new List<string>();
             msp.Add("Office");
             msp.Add(".NET");
             msp.Add("Visual Studio");
             msp.Add("sql server");
             msp.Add("Windows7");
             msp.Add("Window8");
             msp.Add("Sanjay Srivastava");
             msp.Add("Sanjay Mishra");
             msp.Add("Alok Bhargava");
             msp.Add("Alok Johari");
             // Select the tags that match the query, and get the 
             // number or tags specified by the limit.
             List<string> getValues = msp.Where(item => item.ToLower().StartsWith(term.ToLower())).ToList();
             // Return the result set as JSON
             return Json(getValues, JsonRequestBehavior.AllowGet);
         }

    }
}
