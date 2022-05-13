using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_Update_OrgChartController : Controller
    {
        //
        // GET: /B_Send_SMS/

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

        AAA_SendSMS_BLL Lists = new AAA_SendSMS_BLL();

        public ActionResult Index()
        {

            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Reportingto = new SelectList(Lists.Get_ReportingToEmployee(), "NewEmpId", "Empname", objCookie.getCompCode());
            ModelState.Clear();
            EmployeeMdl model = new EmployeeMdl
            {
                GetempList = Lists.Get_ReportingToUpdate()
            };
            return View(model);
        }

        public JsonResult EditReportingTo_Record(int? id)
        {

            var hl = Lists.Get_ReportingToUpdate().Find(x => x.NewEmpId.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult EditReportingTo(EmployeeMdl objModel)
        {
            try
            {
                int result = Lists.UpdateReportingTo(objModel);
                if (result == 1)
                {
                    ViewBag.Message = "Updated Successfully";

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
        