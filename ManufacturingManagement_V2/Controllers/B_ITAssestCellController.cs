using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
using System.IO;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_ITAssestCellController : Controller
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
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();

            ViewBag.CompanyList = new SelectList(Lists.get_Item_Type_ITCell(), "Item_Type_Id", "Item_Type", objCookie.getCompCode());
            ModelState.Clear();
            AAA_ITAssestCell_MDI model = new AAA_ITAssestCell_MDI();
            model.Item_List = Lists.get_Item_Type_ITCell();
            return View(model);
        }


        public JsonResult Edit_Item_Type_Record(int? id)
        {

            var hl = Lists.get_Item_Type_ITCell().Find(x => x.Item_Type_Id.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }




        [HttpPost]
        public ActionResult Edit_Item_Type(AAA_ITAssestCell_MDI objModel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.Update_ItemType(objModel);
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
        public ActionResult Add_ItemType(AAA_ITAssestCell_MDI objModel)
        {
            if (mc.getPermission(Entry.ItAssest, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                int result = Lists.Insert_ItemType(objModel);
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
   public ActionResult RemoveEmployee(int Id)
   {
            if (mc.getPermission(Entry.ItAssest, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
       {

           int result = Lists.Delete_Item_Type(Id);
           if (result == 1)
           {
               ViewBag.Message = "Record Deleted Successfully";
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
        public ActionResult DeleteType(int id)
        {
            try
            {
                int result = Lists.DeleteItemType(id);
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
