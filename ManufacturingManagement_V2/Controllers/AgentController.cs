using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AgentController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private AgentBLL bllObject = new AgentBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        private void setViewObject(int svptypeid = 0, int cityid = 0, string cityname = "")
        {
            ViewData["AddEdit"] = "Save";
            ViewBag.SVPTypeList = new SelectList(bllObject.getServicePartnerTypeList(), "SVPTypeId", "SVPTypeName", svptypeid);
            ViewBag.cityid = cityid;
            ViewBag.cityname = cityname;
        }

        public ActionResult Index(int svptypeid = 0, int cityid = 0, string cityname = "")
        {
            if (mc.getPermission(Entry.Agent_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            if (cityname.Length == 0) { cityid = 0; };
            setViewObject(svptypeid,cityid,cityname);
            List<AgentMdl> modelObject = new List<AgentMdl> { };
            modelObject = bllObject.getObjectList(svptypeid, cityid);
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int svptypeid = 0;
            if (form["ddlSVPType"].ToString().Length > 0)
            {
                svptypeid = Convert.ToInt32(form["ddlSVPType"].ToString());
            }
            string cityid = form["hfCityId"].ToString();
            string cityname = form["txtCityName"].ToString();
            return RedirectToAction("Index", new { svptypeid = svptypeid, cityid = cityid, cityname= cityname });
        }

        // GET: /CreateUpdate
        public ActionResult CreateUpdate(int id = 0, string iname = "")
        {
            if (mc.getPermission(Entry.Agent_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            setViewObject();
            AgentMdl modelObject = new AgentMdl();
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

        // POST: /CreateUpdate
        [HttpPost]
        public ActionResult CreateUpdate(AgentMdl modelObject)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            setViewObject();
            if (ModelState.IsValid)
            {
                ViewBag.Message = "Permission Denied!";
                if (modelObject.AgentId == 0)//add mode
                {
                    if (mc.getPermission(Entry.Agent_Master, permissionType.Add) == false)
                    {
                        return View();
                    }
                    bllObject.insertObject(modelObject);
                }
                else//edit mode
                {
                    ViewData["AddEdit"] = "Update";
                    if (mc.getPermission(Entry.Agent_Master, permissionType.Edit) == false)
                    {
                        return View();
                    }
                    bllObject.updateObject(modelObject);
                }
                if (bllObject.Result == true)
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Message = bllObject.Message;
            return View(modelObject);
        }

        #region upload/download section

        //get
        public ActionResult UploadFile(int agentid = 0, string agentname = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();

            ViewBag.agentid = agentid;
            ViewBag.agentname = agentname;
            ViewBag.Message = "";
            if (Session["upagentid"] != null)
            {
                ViewBag.rfqid = Session["upagentid"].ToString();
                Session.Remove("upagentid");
            }
            if (Session["updmsg"] != null)
            {
                ViewBag.Message = Session["updmsg"].ToString();
                Session.Remove("updmsg");
            }
            return View();
        }

        // POST: /UploadFile
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase docfile, int agentid, string agentname = "")
        {
            if (mc.getPermission(Entry.Agent_Master, permissionType.Edit) == false)
            {
                string msg = "<a href='#' onclick='javascript:window.history.back();'";
                msg += "<h1>Permission Denied!</h1></a>";
                msg += "<br/>[" + Convert.ToInt32(Entry.Agent_Master) + "]";
                return Content(msg);
            }
            setViewData();

            if (agentid == 0)
            {
                ViewBag.Message = "Invalid attempt!";
                return View();
            }

            ViewBag.agentid = agentid;
            Session["upagentid"] = agentid;
            Session["updmsg"] = "Error in File Upload!";
            ViewBag.agentname = agentname;

            if (docfile != null)
            {
                System.IO.Stream str = docfile.InputStream;
                System.IO.BinaryReader Br = new System.IO.BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                AgentMdl modelObject = new AgentMdl();
                modelObject.FlName = docfile.FileName;
                modelObject.FileContent = FileDet;
                modelObject.AgentId = agentid;
                bllObject.uploadAgentFile(modelObject);
                Session["updmsg"] = bllObject.Message;
            }
            else
            {
                Session["updmsg"] = "No file selected to upload!";
            }
            return RedirectToAction("UploadFile", new { agentid = agentid, agentname = agentname });
        }

        [HttpGet]
        public ActionResult ShowDocument(int agentid = 0)
        {
            //note: ActionResult instead of FileContentResult
            string st ="";
            if (mc.getPermission(Entry.Agent_Master, permissionType.Edit) == false)
            {
                return null;
            }
            setViewData();
            AgentBLL bll = new AgentBLL();
            try
            {
                return File(bll.getAgentFile(agentid), bll.Message);
            }
            catch (Exception ex)
            {
                st = ex.Message;
                st = "File Not Uploaded!";//note
            }
            return Content("<a href='#' onclick='javascript:window.close();'><h1>"+st+"</h1></a>");
        }

        #endregion //upload/download section

        //get
        public ActionResult Details(int id = 0)
        {
            if (mc.getPermission(Entry.Agent_Master, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            setViewData();
            AgentMdl modelObject = bllObject.searchObject(id);
            if (modelObject == null)
            {
                return HttpNotFound();
            }
            return View(modelObject);
        }

        [HttpPost, ActionName("Details")]//performs deletion on posting
        public ActionResult Delete(int id = 0)
        {
            if (mc.getPermission(Entry.Agent_Master, permissionType.Delete) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied!</h1></a>");
            }
            bllObject.deleteObject(id);
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteAgent(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            System.Data.DataSet ds = new System.Data.DataSet();
            ds = bllObject.getObjectData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["agentid"].ToString(), ds.Tables[0].Rows[i]["agentname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
