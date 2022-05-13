using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
using System.Configuration;
using System.Data.SqlClient;
namespace ManufacturingManagement_V2.Controllers
{
    public class B_Issue_Item_ITCELLListController : Controller
    {

        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();

        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();

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
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            model.AAA_Item_Type = Lists.PopulateDropDown("SELECT Item_Type_Id, Item_Type FROM AAA_Item_Type", "Item_Type", "Item_Type_Id");
            ViewBag.EMPName = new SelectList(Lists.get_EMPName_To_Issue(), "NewEmpId", "EmpName", objCookie.getCompCode());
            ModelState.Clear();
            model.Serial_No = "NA";
            model.Comment = "NA";
            model.Minus = 0;
            model.Issue_Qty = 1;
            model.Item_List = Lists.Get_Issue_Item_List();
            return View(model);
        }
        [HttpPost]
        public JsonResult AjaxMethod(string type, int value)
        {
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            switch (type)
            {
                case "Item_Type_Id":
                    model.AAA_ItemMaster_ITCell = Lists.PopulateDropDown("SELECT Item_Id, Item_Name FROM AAA_ItemMaster_ITCell WHERE Item_Type_Id = " + value, "Item_Name", "Item_Id");
                    break;
               
               
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult Index(int countryId,int itemid)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            model.AAA_Item_Type = Lists.PopulateDropDown("SELECT Item_Type_Id, Item_Type FROM AAA_Item_Type", "Item_Type", "Item_Type_Id");
            model.AAA_ItemMaster_ITCell = Lists.PopulateDropDown("SELECT Item_Id, Item_Name FROM AAA_ItemMaster_ITCell WHERE Item_Type_Id = " + countryId, "Item_Name", "Item_Id");

            //model.AAA_Item_StockITCell = PopulateDropDown("SELECT Item_Id, Stock FROM AAA_Item_StockITCell  WHERE Item_Id = " + itemid, "Stock", "Item_Id");
            
            //model.AAA_Item_StockITCell = PopulateDropDown("SELECT Item_Id, Serial_No FROM AAA_Item_StockITCell  WHERE Item_Id = " + stateId, "Serial_No", "Item_Id");
            return View(model);
        }
  [HttpGet]
        public ActionResult Edit_IssueItem_Record(int? id)
        {

            var hl = Lists.Get_Issue_Item_List().Find(x => x.Tran_Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_IssueItem_Name(AAA_ITAssestCell_MDI objModel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.Update_Isssue_Item_Name_ITCELL1(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Updated Successfully";

                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Add_IssueItem_Name(AAA_ITAssestCell_MDI empmodel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {


                int result = Lists.Insert_Isssue_Item_Name_ITCELL(empmodel);
                if (result == 1)
                {
                    ViewBag.Message = "Record Added Successfully";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }
        //private static List<SelectListItem> PopulateDropDown(string query, string textColumn, string valueColumn)
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();
        //    string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(query))
        //        {
        //            cmd.Connection = con;
        //            con.Open();
        //            using (SqlDataReader sdr = cmd.ExecuteReader())
        //            {
        //                while (sdr.Read())
        //                {
        //                    items.Add(new SelectListItem
        //                    {
        //                        Text = sdr[textColumn].ToString(),
        //                        Value = sdr[valueColumn].ToString()
        //                    });
        //                }
        //            }
        //            con.Close();
        //        }
        //    }

        //    return items;
        //}



        public ActionResult Delete_IssueItem(int id)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.DeleteIssueItem(id);
                if (result == 1)
                {
                    ViewBag.Message = "Item Deleted Successfully";
                    // ModelState.Clear();
                }
                else
                {
                    ViewBag.Message = "Unsucessfull";
                    //ModelState.Clear();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw;
            }
        }


    }
}
