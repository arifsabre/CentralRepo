using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
using System.IO;


namespace ManufacturingManagement_V2.Controllers
{
    public class B_Item_Master_ItCellController : Controller
    {
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
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(Lists.get_Item_Type_ITCell(), "Item_Type_Id", "Item_Type", objCookie.getCompCode());
            ModelState.Clear();
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            model.Item_List = Lists.Get_MasterItem_List_ItCell();
            //model.Item_Name = "NA";
            model.StockMaster = 0;
            return View(model);
        }


        public JsonResult Edit_Item_ItemMasterITCell_Record(int? id)
        {
            var hl = Lists.Get_MasterItem_List_ItCell().Find(x => x.Item_Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit_ItemMaster_ITCell(AAA_ITAssestCell_MDI objModel)
        {
            try
            {
                int result = Lists.Update_ItemMaster_ItCell(objModel);
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
        public ActionResult Add_ItemMaster_ItCell(AAA_ITAssestCell_MDI empmodel)
        {
            try
            {
                int result = Lists.Insert_ItemMaster_ItCell(empmodel);
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
        public ActionResult DeleteItemName(int id)
        {
            try
            {
                int result = Lists.DeleteItemName(id);
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
