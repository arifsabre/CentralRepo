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
   
    public class Add_In_Stock_ITCELLLisController : Controller
    {
        //
        // GET: /Add_In_Stock_ITCELLLis/
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
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
           setViewData();
           int luserid = Convert.ToInt32(objCookie.getUserId());
            LibFileUploadMDI model1 = new LibFileUploadMDI();
            LibFileUploadBLL Listss = new LibFileUploadBLL();
            LibBLL List1 = new LibBLL();
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            model.AAA_Item_Type = Lists.PopulateDropDown("SELECT Item_Type_Id, Item_Type FROM AAA_Item_Type", "Item_Type", "Item_Type_Id");
            ViewBag.LocationList = new SelectList(Lists.get_Item_Location_ITCell(), "compcode", "cmpname");
            ViewBag.SupplierName = new SelectList(Lists.get_Supplier_Name_ItCell(), "Supplier_Id", "Supplier_Name");
            //ViewBag.EMPName = new SelectList(Lists.get_EMPName_To_Issue(), "NewEmpId", "EmpName", objCookie.getCompCode());
            ModelState.Clear();
            model.Serial_No = "NA";
            model.Invoice_No = "NA";
            model.Comment = "NA";
            model.Scrap_Item = "NO";
            model.Item_Name = "NA";
            model.Plus = 1;
            model.Minus = 0;
            model.Item_List = Lists.Get_Item_List();
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
        public ActionResult Index(int countryId, int stateId)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI
            {
                AAA_Item_Type = Lists.PopulateDropDown("SELECT Item_Type_Id, Item_Type FROM AAA_Item_Type", "Item_Type", "Item_Type_Id"),
                AAA_ItemMaster_ITCell = Lists. PopulateDropDown("SELECT Item_Id, Item_Name FROM AAA_ItemMaster_ITCell WHERE Item_Type_Id = " + countryId, "Item_Name", "Item_Id"),
                };
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit_AddinstockItem_Record(int? id)
        {

            var hl = Lists.Get_Item_List().Find(x => x.Tran_Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Edit_AddinstockItem(AAA_ITAssestCell_MDI objModel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.Update_Item_NameMaster_Credit_ITCELL(objModel);
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
        public ActionResult Addinstock_Name(AAA_ITAssestCell_MDI empmodel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
               int result = Lists.Insert_Item_CreditName_ITCell(empmodel);
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
       
        public ActionResult Delete_StockItemName(int id)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.DeleteStockItem(id);
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
