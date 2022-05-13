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
    public class TreeviewController : Controller
    {
        private readonly clsCookie objCookie = new clsCookie();
        private readonly clsMyClass mc = new clsMyClass();
        private readonly CompanyBLL compBLL = new CompanyBLL();
        private readonly EmployeeBLL empBLL = new EmployeeBLL();

        private void SetViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
        }
        //
        // GET: /Treeview/

        AAA_SiteMenu_BLL lists = new AAA_SiteMenu_BLL();
        public ActionResult Index()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();

            //List<SiteMenu> all = new List<SiteMenu>();
            //using (ErpConnection dc = new ErpConnection())
            //{
            //    all = dc.SiteMenu.OrderBy(a => a.ParentMenuID).ToList();
            //}
            //return View(all);
            return View();
        }

        public ActionResult Simple()
        {
            //List<SiteMenu> all = new List<SiteMenu>();
            //using (ErpConnection dc = new ErpConnection())
            //{
            //    all = dc.SiteMenu.OrderBy(a => a.ParentMenuID).ToList();
            //}
            //return View(all);
            return View();
        }
        public PartialViewResult _Index()
        {

            return PartialView();
        }
        public ActionResult _Simple()
        {
            return PartialView();
        }
        public ActionResult ParentList()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            SetViewData();
            //ViewBag.CompanyList = new SelectList(Lists.get_Item_Type_ITCell(), "Item_Type_Id", "Item_Type", objCookie.getCompCode());
            ModelState.Clear();
            AAA_SiteMenu model = new AAA_SiteMenu
            {
                Item_List = lists.Get_ParentMenu_List()
            };
            return View(model);
        }


        public JsonResult Edit_ParentMenu_Record(int? id)
        {

            var hl = lists.Get_ParentMenu_List().Find(x => x.ParentMenuID.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_ParentMenu(AAA_SiteMenu objModel)
        {
            try
            {
                int result = lists.Update_ParentMenu(objModel);
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

                return RedirectToAction("ParentList");
            }
            catch
            {
                throw;
            }
        }



        [HttpPost]
        public ActionResult Add_ParentMenu(AAA_SiteMenu objModel)
        {
            try
            {
                int result = lists.Insert_ParentMenu(objModel);
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

                return RedirectToAction("ParentList");
            }
            catch
            {
                throw;
            }
        }












    }
}
