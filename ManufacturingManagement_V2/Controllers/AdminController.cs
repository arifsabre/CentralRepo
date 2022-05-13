
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AdminController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private EmployeeBLL empBLL = new EmployeeBLL();
        
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        //get
        public ActionResult Index()
        {
            setViewData();
            return View();
        }

        public ActionResult ChkIndex()
        {
            setViewData();
            ViewBag.RptDate = mc.getDateTimeString(DateTime.Now);
            return View();
        }

        public ActionResult ChkIndex1()
        {
            setViewData();
            ViewBag.RptDate = mc.getDateTimeString(DateTime.Now);
            return View();
        }

        #region execute action

        [HttpGet]
        public ActionResult ExecuteOneAction(int opt=0)
        {
            setViewData();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            
            string taskname = "No current action defined to perform";
            string msg = "Set opt value in URL and press enter";

            if (opt == 1)
            {
                taskname = "Document Status Info";
                msg = UpdateDocumentUploadStatus();
            }
            else if (opt == 2)
            {
                taskname = "Photograph to directory updation";
                msg = SendEmpPhotoToDirectory();
            }

            ViewBag.TaskName = taskname;
            ViewBag.Message = msg;
            return View();
        }

        private string UpdateDocumentUploadStatus()
        {
            List<EmployeeMdl> modelObject = new List<EmployeeMdl> { };
            modelObject = empBLL.getObjectList("", 0);
            for (int i = 0; i < modelObject.Count; i++)
            {
                var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/EmpDocs/" + modelObject[i].NewEmpId.ToString() + "/"));
                List<string> items = new List<string>();
                empBLL.updateEmployeeDocument(modelObject[i].NewEmpId, false);
                if (dir.Exists)
                {
                    System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                    if (fileNames.Length > 0)
                    {
                        empBLL.updateEmployeeDocument(modelObject[i].NewEmpId, true);
                    }
                }
            }
            return "Document Status Updated Successfully!";
        }

        private string SendEmpPhotoToDirectory()
        {
            List<EmployeeMdl> modelObject = new List<EmployeeMdl> { };
            modelObject = empBLL.getObjectList("", 0).ToList();//.Where(s => s.NewEmpId.Equals(217)).ToList();
            for (int i = 0; i < modelObject.Count; i++)
            {
                string photopath = Server.MapPath("~/App_Data/EmpDocs/Photo/" + modelObject[i].EmpId + ".jpg");
                if (System.IO.File.Exists(photopath))
                {
                    string empdirpath = Server.MapPath("~/App_Data/EmpDocs/" + modelObject[i].NewEmpId + "/");
                    System.IO.DirectoryInfo empdirinfo = new System.IO.DirectoryInfo(empdirpath);
                    if (!empdirinfo.Exists)
                    {
                        empdirinfo.Create();
                    }
                    string newpath = System.IO.Path.Combine(empdirpath, System.IO.Path.GetFileName(modelObject[i].EmpId + ".jpg"));
                    if (System.IO.File.Exists(newpath) == false)
                    {
                        System.IO.File.Copy(photopath, newpath);
                    }
                }
            }
            return "Photo(s) Transfered to Directories Successfully!";
        }

        #endregion

        #region getting file name(s) where objects are defined/used

        [HttpPost]
        public ActionResult SearchDirectory(FormCollection form)
        {
            string dirname = form["txtDirName"].ToString();
            string objname = form["txtObjName"].ToString();
            return RedirectToAction("getObjectFile", new { dirname = dirname, objname = objname });
        }
        public ActionResult getObjectFile(string dirname = "App_Data/dbProcedures", string objname = "")
        {
            setViewData();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            //-----------------------------------------------------------------
            ViewBag.dirname = dirname;
            ViewBag.objname = objname;
            dirname = "~/" + dirname + "/";
            //-----------------------------------------------------------------
            //var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/dbProcedures/"));
            var dir = new System.IO.DirectoryInfo(Server.MapPath(dirname));
            List<string> items = new List<string>();
            string text = "";
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string fn = fileNames[i].Name;
                    //string newpath = Server.MapPath("~/App_Data/dbProcedures/" + fn);
                    string newpath = Server.MapPath(dirname + fn);
                    if (System.IO.File.Exists(newpath))
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(newpath))
                        {
                            string fl = sr.ReadToEnd().ToLower();
                            if (fl.Contains(objname.ToLower()))
                            {
                                text = text + fn + "<br/><br/>";
                            }
                        }
                    }
                }
            }
            ViewBag.Result = "<html><body>" + text + "</body></html>";
            return View();
        }

        [HttpPost]
        public ActionResult SearchSingleDirectory(FormCollection form)
        {
            string dirname = form["txtDirName"].ToString();
            return RedirectToAction("createSingleFile", new { dirname = dirname });
        }
        public ActionResult createSingleFile(string dirname = "")
        {
            setViewData();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            //-----------------------------------------------------------------
            ViewBag.dirname = dirname;
            if (dirname.Length == 0) 
            {
                return View();
            }
            //
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/"+dirname+"/"));
            List<string> items = new List<string>();
            string text = "";
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string fn = fileNames[i].Name;
                    string newpath = Server.MapPath("~/App_Data/"+dirname+"/" + fn);
                    if (System.IO.File.Exists(newpath))
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(newpath))
                        {
                            text = text + sr.ReadToEnd().Replace("\r\n","<br/>");
                            text = text + "<br/><br/>";
                        }
                    }
                }
            }
            ViewBag.Result = "<html><body>" + text + "</body></html>";
            return View();
        }

        [HttpPost]
        public ActionResult SearchForDBPList(FormCollection form)
        {
            string dirname = form["txtDirName"].ToString();
            return RedirectToAction("CreateDBPList", new { filename = dirname });
        }
        public ActionResult CreateDBPList(string filename = "")
        {
            setViewData();
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            //-----------------------------------------------------------------
            ViewBag.dirname = filename;
            if (filename.Length == 0)
            {
                return View();
            }
            //
            string filepath = Server.MapPath("~/" + filename);
            string textFile = "";
            string txtLine = "";
            string dbpCmd = "";
            ArrayList arlPrc = new ArrayList();
            if (System.IO.File.Exists(filepath))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    //text = text + sr.ReadToEnd().Replace("\r\n", "<br/>");
                    textFile = sr.ReadToEnd().Replace("\r\n", "#");
                    string[] arl = textFile.Split('#');
                    for (int i = 0; i < arl.Length; i++)
                    {
                        if (arl[i].Contains("cmd.CommandText"))// + || arl[i].Contains("con.Execute(") || arl[i].Contains("SqlCommand("))
                        {
                            if (!(arlPrc.Contains(arl[i])))
                            {
                                arlPrc.Add(arl[i]);
                            }
                        }
                    }
                    for (int i = 0; i < arlPrc.Count; i++)
                    {
                        dbpCmd = arlPrc[i].ToString().Trim().Replace("=", "").Replace(";", "");
                        txtLine = txtLine + "sp_helptext " + dbpCmd.Split('"')[1];
                        txtLine = txtLine + "<br/>GO<br/><br/>";
                    }
                    txtLine = txtLine + "<br/><br/>";
                }
            }
            ViewBag.Result = "<html><body>" + txtLine + "</body></html>";
            return View();
        }

        #endregion

        #region file upload/download

        //get
        public ActionResult UploadFile()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Message = "";
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            if (file != null && file.ContentLength > 0)
                try
                {
                    string dirpath = Server.MapPath("~/App_Data/FileTrf/");
                    string path = System.IO.Path.Combine(dirpath, System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    Session["updmsg"] = ViewBag.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                return View();
            }
            return RedirectToAction("UploadFile");
        }

        //get
        public ActionResult Downloads()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/FileTrf/"));
            List<string> items = new List<string>();
            if (dir.Exists)
            {
                System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
                foreach (var file in fileNames)
                {
                    items.Add(file.Name);
                }
            }
            return View(items);
        }

        //get
        public FileResult Download(string fileName)
        {
            if (objCookie.getUserId() != "1")
            {
                string err = Server.MapPath("~/App_Data/EmpDocs/");
                fileName = "Permission.png";
                return File(err + "/blank/Permission.png", mc.getMimeType(fileName));
            }
            string path = Server.MapPath("~/App_Data/FileTrf/");
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            if (ext != "pdf")
            {
                return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Octet,fileName);
            }
            return File(path + fileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        //get
        public ActionResult Delete(string fileName)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            if (objCookie.getUserId() != "1") { return RedirectToAction("LoginUser", "Login"); };
            string path = Server.MapPath("~/App_Data/FileTrf/");
            System.IO.File.Delete(path + fileName);
            return RedirectToAction("Downloads");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
