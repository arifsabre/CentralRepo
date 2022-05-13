using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_Scrap_ItemController : Controller
    {
        //
        // GET: /B_ITAssestCell_Item_Name/

        //
        // GET: /B_ITAssestCell/
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
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();
        public ActionResult Index()
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            //ViewBag.CompanyList = new SelectList(Lists.get_Item_Type_ITCell(), "Item_Type_Id", "Item_Type", objCookie.getCompCode());
            //ViewBag.LocationList = new SelectList(Lists.get_Item_Location_ITCell(), "compcode", "cmpname");
            //ViewBag.SupplierName = new SelectList(Lists.get_Supplier_Name_ItCell(), "Supplier_Id", "Supplier_Name");
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            ModelState.Clear();
            model.Scrap1 = 0;
            model.Scrap_Date = DateTime.Now;
            model.Item_List = Lists.Get_ScrapItem_List();

            return View(model);
           

        }

        public JsonResult Edit_ScrapItem_Record(int? id)
        {

            var hl = Lists.Get_ScrapItem_List().Find(x => x.Tran_Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_ScrapItem(AAA_ITAssestCell_MDI objModel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.Update_ScrapItem_ITCELL(objModel);
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

     



    }
}
