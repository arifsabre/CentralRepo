using System.Collections.Generic;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Dapper;

namespace ManufacturingManagement_V2.Controllers
{
    public class B_GetInvoiceDetailController : Controller
    {
        //
        // GET: /B_GetInvoiceDetail/
        private SqlConnection con;
        private string constr;
        clsMyClass mc = new clsMyClass();
        private void DbConnection()
        {
            constr = mc.strconn;
            con = new SqlConnection(constr);
        }

        // GET: /B_Upload_Invoice_ITCEll/
        AAA_ITAssestCell_BLL Lists = new AAA_ITAssestCell_BLL();

        clsCookie objCookie = new clsCookie();
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
            return View();
        }
        [HttpGet]
        public FileResult DownLoadFile(int id)
        {


            List<AAA_ITAssestCell_MDI> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();

            return File(FileById.FileContent, "application/pdf", FileById.FileName);

        }



        [HttpGet]
        public PartialViewResult FileDetails()
        {
            List<AAA_ITAssestCell_MDI> DetList = GetFileList();

            return PartialView("FileDetails", DetList);


        }
        private List<AAA_ITAssestCell_MDI> GetFileList()
        {
            List<AAA_ITAssestCell_MDI> DetList = new List<AAA_ITAssestCell_MDI>();

            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<AAA_ITAssestCell_MDI>(con, "AAA_GetInvoiceDetails_ITCell", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }
        //AAA_GetInvoiceDetails_ITCell
        //ZZZZ_GetFileDetails
    }
}
