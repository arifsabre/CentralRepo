using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_FirstInLastOut_CasualandGuardController : Controller
    {
        //
        // GET: /B_FirstInLastOut_CasualandGuard/


        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        CompanyBLL compBLL = new CompanyBLL();
        EmployeeBLL empBLL = new EmployeeBLL();
        Security_BLL securitybll = new Security_BLL();
        private LoginBLL loginBLL = new LoginBLL();
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
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListOther(), "compcode", "cmpname", objCookie.getCompCode());
            ViewBag.EmpList = new SelectList(securitybll.Security_Get_AllEmpList(), "ECode", "EmpName");
            //ViewBag.GradeList = new SelectList(mc.getGradeList(), "Value", "Text");
            rptOptionMdl rptOption = new rptOptionMdl();
            rptOption.DateTo = DateTime.Now;
            rptOption.DateFrom = DateTime.Now;
            return View(rptOption);
        }
        private void setLoginInfo(CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc)
        {
            DataTable lginfo = mc.getCrptLoginInfo();
            CrystalDecisions.Shared.TableLogOnInfos crtableLogoninfos = new CrystalDecisions.Shared.TableLogOnInfos();
            CrystalDecisions.Shared.TableLogOnInfo crtableLogoninfo = new CrystalDecisions.Shared.TableLogOnInfo();
            CrystalDecisions.Shared.ConnectionInfo crConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
            crConnectionInfo.ServerName = lginfo.Rows[0]["svrname"].ToString();
            crConnectionInfo.DatabaseName = lginfo.Rows[0]["dbname"].ToString();
            crConnectionInfo.UserID = lginfo.Rows[0]["userid"].ToString();
            crConnectionInfo.Password = lginfo.Rows[0]["passw"].ToString();
            CrystalDecisions.CrystalReports.Engine.Tables CrTables = rptDoc.Database.Tables;
            foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
            {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }
        }
        [HttpPost]
        public ActionResult B_Offline_EmpMachine(rptOptionMdl rptOption)
        {
            //[100138]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_Offline_EmpMachine/Get_B_Offline_Emp_FileMachine";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "compcode=" + rptOption.CompCode + "";

                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_Offline_Emp_FileMachine", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode });
        }
        public ActionResult Get_B_Offline_Emp_FileMachine(DateTime dtfrom, DateTime dtto, int compcode)
        {
            //[100138]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_FirstInLastOut_WithMachine_SecurityGuard.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //dbp --ZZZ_USP_GET_FirstIn_Lastout_OR_GuardWithMachineNo
            rptDoc.SetParameterValue("@from", dtfrom);
            rptDoc.SetParameterValue("@to", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            //
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();

            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            // return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }
        [HttpGet]
        public ActionResult Security_Index_Save()
        {
            //int compcode = Convert.ToInt32(objCookie.getCompCode());
            //int userid = Convert.ToInt32(objCookie.getUserId());
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            Security_BLL model = new Security_BLL();
           ViewBag.CompanyList = new SelectList(model.Security_Get_All_CompanyOR(),"compcode","cmpname");
          
            //model.EmpName = " ";
            model.Item_List = model.Security_Get_All_EmpList();
            return View(model);
        }
          [HttpPost]
        public ActionResult Security_Add(Security_BLL empmodel)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {

                int  result = securitybll.Security_Insert(empmodel);
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

                return RedirectToAction("GetEmpId");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit_Security_Record(int? id)
        {
            Security_BLL model = new Security_BLL();
            var hl = model.Security_Get_All_EmpList().Find(x => x.ECode.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult Security_CompanyUpdate(int? id)
        {
            Security_BLL model = new Security_BLL();
            var hl = model.Security_Get_All_EmpList().Find(x => x.ECode.Equals(id));
            return Json(hl, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Edit_Security_Company(Security_BLL objModel)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                Security_BLL model = new Security_BLL();
                int result = model.Security_Update_Company(objModel);
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

                return RedirectToAction("Security_Index_Save");
            }
            catch
            {
                throw;
            }
        }

       
        [HttpPost]
        public ActionResult Edit_Security(Security_BLL objModel)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Edit) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                Security_BLL model = new Security_BLL();
                int result = model.Security_Update(objModel);
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

                return RedirectToAction("Security_Index_Save");
            }
            catch
            {
                throw;
            }
        }
        public ActionResult Delete_Security(int id)
        {
            if (mc.getPermission(Entry.Biometric_Data, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            try
            {
                Security_BLL model = new Security_BLL();
                int result = model.Security_Delete(id);
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
                return RedirectToAction("Security_Index_Save");
            }
            catch
            {
                throw;
            }
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

        public ActionResult GetEmpId()
        {
            Security_BLL model = new Security_BLL();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            //List<ComplaintMDI> modelObject = new List<ComplaintMDI> { };
            model.Item_List = model.Security_GetEmpId();
            // modelObject = Lists.CoplaintGet_AllCoplaint();
            return View(model);
        }


        public ActionResult B_Offline_EmpMachineByName(rptOptionMdl rptOption)
        {
            //[100138]
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            bool viewper = mc.getPermission(Entry.Biometric_Data, permissionType.Add);
            if (viewper == false)//no permission
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bool downloadper = mc.getPermission(Entry.Biometric_Data, permissionType.Edit);
            Session["xsid"] = objCookie.getUserId();
            if (viewper == true && downloadper == false)//view only
            {
                string reporturl = "B_Offline_EmpMachineByName/Get_B_Offline_Emp_FileMachineByName";
                string reportpms = "dtfrom=" + mc.getStringByDateForReport(rptOption.DateFrom) + "";
                reportpms += "dtto=" + mc.getStringByDateForReport(rptOption.DateTo) + "";
                reportpms += "compcode=" + rptOption.CompCode + "";
                reportpms += "Ecode=" + rptOption.ECode + "";


                return RedirectToAction("IndexIFrameRpt", "Home", new { reporturl = reporturl, reportpms = reportpms });
            }
            //full access with download (no escalation)
            return RedirectToAction("Get_B_Offline_Emp_FileMachineByName", new { dtfrom = rptOption.DateFrom, dtto = rptOption.DateTo, compcode = rptOption.CompCode, ECode = rptOption.ECode });
        }
        public ActionResult Get_B_Offline_Emp_FileMachineByName(DateTime dtfrom, DateTime dtto, int compcode,int Ecode)
        {
            //[100138]
            if (mc.isValidToDisplayReport() == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            rptDoc.Load(System.IO.Path.Combine(Server.MapPath("~/Reports"), "B_FirstInLastOut_WithMachine_SecurityGuardName.rpt"));//TestXCrystalReport1
            setLoginInfo(rptDoc);
            //dbp --ZZZ_USP_GET_FirstIn_Lastout_OR_GuardWithMachineNo
            rptDoc.SetParameterValue("@from", dtfrom);
            rptDoc.SetParameterValue("@to", dtto);
            rptDoc.SetParameterValue("@compcode", compcode);
            rptDoc.SetParameterValue("@Ecode", Ecode);
            //
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            System.IO.Stream stream = null;
            try
            {
                stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
            finally
            {
                rptDoc.Close();

            }
            //add these lines to download
            //stream.Seek(0, System.IO.SeekOrigin.Begin);
            // return File(stream, "application/pdf", "ReportName.pdf");
            return File(stream, "application/pdf");
        }

    }
   }

