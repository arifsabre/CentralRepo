using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
namespace ManufacturingManagement_V2.Controllers
{
    public class C_FittingController : Controller
    {
        
        // GET: C_Fitting
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UserBLL bllUser = new UserBLL();
        CompanyBLL compBLL = new CompanyBLL();
        private C_Fitting_ItemBLL bllObject = new C_Fitting_ItemBLL();
        private LoginBLL loginBLL = new LoginBLL();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }
        public ActionResult MessageBox(string msg = "")
        {
            ViewBag.Message = msg;
            //Response.AddHeader("Refresh", "5");
            return View();
        }
        private void setViewObject(string saledate = "", string invoicedate = "")
        {
            int compcode = Convert.ToInt32(objCookie.getCompCode());
            string finyear = objCookie.getFinYear();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.ItemList = new SelectList(bllObject.getItemList(compcode), "ItemId", "ItemName");
            //ViewBag.InvoiceList = new SelectList(bllObject.getInvoiceList(compcode, finyear), "SaleRecId","invoiceno");
            ViewBag.Consigneelist = new SelectList(bllObject.getConsigneeList(),"consigneeId","consigneename");
            ViewBag.DtFrom = saledate; 
            ViewBag.DtTo = invoicedate; 
            //ViewBag.depcode = depcode;
        }
        public ActionResult Index()
        {
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            //{
            //    return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [9050]</h1></a>");
            //}
            setViewData();
            setViewObject();
            C_Fitting_ItemMDI modelObject = new C_Fitting_ItemMDI();
            modelObject.Item_List = bllObject.GetAll_FittingItem_Details();
            return View(modelObject);
        }
        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_Fitting_ItemMDI modelObject = new C_Fitting_ItemMDI();
            modelObject.solddate = DateTime.Now;
            modelObject.invoicedate = DateTime.Now;
            modelObject.fittingdate = DateTime.Now;
            modelObject.invoiceno = " ";
            modelObject.coachno = " ";
            modelObject.rakenoorderagency = " ";
            modelObject.rakenorailway = " ";
            modelObject.rakenouserrailway = " ";
            modelObject.shedname = " ";
           if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }
        [HttpPost]
        public ActionResult CreateUpdate(C_Fitting_ItemMDI modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";
          
            if (modelObject.srno == 0)//add
            {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
                return PartialView("_AddRecord");
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                //return RedirectToAction("MessageBox", "C_Fitting", new { msg = bllObject.Message });
                return PartialView("_AddRecord");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }
        // GET: /CreateUpdate
        public ActionResult CreateUpdate1(int id = 0)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            setViewObject();
            ViewData["AddEdit"] = "Save";
            C_Fitting_ItemMDI modelObject = new C_Fitting_ItemMDI();
            modelObject.solddate = DateTime.Now;
            modelObject.invoicedate = DateTime.Now;
            modelObject.fittingdate = DateTime.Now;
            modelObject.invoiceno = " ";
            //modelObject.fittingdate = DateTime.Now;
            //modelObject.validity = DateTime.Now;
            modelObject.coachno = " ";
            modelObject.rakenoorderagency = " ";
            modelObject.rakenorailway = " ";
            modelObject.rakenouserrailway = " ";
            modelObject.shedname = " ";
            if (id != 0)
            {
                modelObject = bllObject.searchObject(id);
                if (modelObject == null)
                {
                    return HttpNotFound();
                }
                ViewData["AddEdit"] = "Update";
            }
            return View(modelObject);
        }
        [HttpPost]
        public ActionResult CreateUpdate1(C_Fitting_ItemMDI modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            //if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
            //{
            //    return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            //}
            setViewData();
            setViewObject();
            ViewBag.Message = "Permission Denied!";

            if (modelObject.srno == 0)//add
            {
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Add) == false)
                {
                    return View();
                }
                bllObject.insertObject(modelObject);
            }
            else
            {
                ViewData["AddEdit"] = "Update";
                if (mc.getPermission(Entry.CRM_Entry, permissionType.Edit) == false)
                {
                    return View();
                }
                bllObject.updateObject(modelObject);
            }
            if (bllObject.Result == true)
            {
                //return RedirectToAction("MessageBox", "C_Fitting", new { msg = bllObject.Message });
                //return RedirectToAction("Index");
                return PartialView("_AddRecord");
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        [HttpPost]
        public JsonResult Delete(int srno)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.CRM_Entry, permissionType.Delete) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new C_Fitting_ItemBLL();
            bllObject.deleteObject(srno);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }
        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult ChangeCompany()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            return View();
        }
        [HttpPost]
        public ActionResult ChangeCompany(FormCollection form)
        {
            string compcode = form["ddlCompany"].ToString();
            if (objCookie.getCompCode() != compcode)
            {
                loginBLL.changeCoockieForCompany(compcode);
            }
            return RedirectToAction("Index");
        }



       

    }
}