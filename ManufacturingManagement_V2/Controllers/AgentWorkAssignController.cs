using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class AgentWorkAssignController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private UserBLL bllUser = new UserBLL();
        private AgentWorkAssignBLL bllObject = new AgentWorkAssignBLL();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public ActionResult Index(int agentid = 0, string agentname = "", string workid = "0", string ackstatus = "0", string compstatus = "0", string workstatus = "0", string billstatus = "0")
        {
            if (mc.getPermission(Entry.AgentWorkAssignment, permissionType.Add) == false)
            {
                return Content("<a href='#' onclick='javascript:window.close();'><h1>Permission Denied! [1356]</h1></a>");
            }
            setViewData();
            //
            ViewBag.WorkIdList = new SelectList(bllObject.getAgentWorkList(), "ObjectCode", "ObjectName", workid);
            ViewBag.AckStatusList = new SelectList(getAckStatusList(), "Value", "Text", ackstatus);
            ViewBag.CompStatusList = new SelectList(getCompletionStatusList(), "Value", "Text", compstatus);
            ViewBag.WorkStatusList = new SelectList(getWorkStatusList(), "Value", "Text", workstatus);
            ViewBag.BillStatusList = new SelectList(getBillStatusList(), "Value", "Text", billstatus);
            List<AgentWorkAssignMdl> modelObject = new List<AgentWorkAssignMdl> { };
            modelObject = bllObject.getObjectList(agentid, workid, ackstatus, compstatus, workstatus, billstatus);
            //
            ViewBag.agentid = agentid;
            ViewBag.agentname = agentname;
            return View(modelObject.ToList());
        }

        [HttpPost]
        public ActionResult FilterIndex(FormCollection form)
        {
            int agentid = 0;
            if (form["hfAgentId"].ToString().Length > 0)
            {
                agentid = Convert.ToInt32(form["hfAgentId"].ToString());
            }
            string agentname = form["txtAgentName"].ToString();
            if (agentname.Length == 0)
            {
                agentid = 0;
            }

            string workid = form["ddlWorkId"].ToString();
            string ackstatus = form["ddlAckStatus"].ToString();
            string compstatus = form["ddlCompStatus"].ToString();
            string workstatus = "0";//form["ddlWorkStatus"].ToString();
            string billstatus = form["ddlBillStatus"].ToString();
            return RedirectToAction("Index", new { agentid = agentid, agentname = agentname, workid = workid, ackstatus = ackstatus, compstatus = compstatus, workstatus = workstatus, billstatus = billstatus });
        }

        [HttpPost]
        public JsonResult UpdateAcknowledgement(int recid, string ackstatus, string ackdate)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.AgentWorkAssignment, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new AgentWorkAssignBLL();
            bllObject.updateAgentWorkAcknowledgement(recid, ackstatus, mc.getDateByString(ackdate));
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        [HttpPost]
        public JsonResult UpdateBillStatus(int recid, string billstatus)
        {
            if (objCookie.checkSessionState() == false) { return new JsonResult { Data = new { status = false, message = "Permission denied!" } }; };
            if (mc.getPermission(Entry.AgentWorkAssignment, permissionType.Edit) == false)
            {
                return new JsonResult { Data = new { status = false, message = "Permission denied!" } };
            }
            bllObject = new AgentWorkAssignBLL();
            bllObject.updateAgentBillStatus(recid, billstatus);
            return new JsonResult { Data = new { status = bllObject.Result, message = bllObject.Message } };
        }

        public List<System.Web.UI.WebControls.ListItem> getAckStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "Received", Value = "Received" },
                new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "Pending" }
            };
            return listItems;
        }
        //

        public List<System.Web.UI.WebControls.ListItem> getCompletionStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "Completed", Value = "Completed" },
                new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "Pending" }
            };
            return listItems;
        }
        //

        public List<System.Web.UI.WebControls.ListItem> getWorkStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "Completed On-time", Value = "Completed On-time" },
                new System.Web.UI.WebControls.ListItem { Text = "Delayed", Value = "Delayed" },
                new System.Web.UI.WebControls.ListItem { Text = "Delayed Long", Value = "Delayed Long" },
                new System.Web.UI.WebControls.ListItem { Text = "Incomplete", Value = "Incomplete" }
            };
            return listItems;
        }
        //

        public List<System.Web.UI.WebControls.ListItem> getBillStatusList()
        {
            var listItems = new List<System.Web.UI.WebControls.ListItem>
            {
                new System.Web.UI.WebControls.ListItem { Text = "Settled", Value = "Settled" },
                new System.Web.UI.WebControls.ListItem { Text = "Pending", Value = "Pending" }
            };
            return listItems;
        }
        //

        protected override void Dispose(bool disposing)
        {
            bllObject.Dispose();
            base.Dispose(disposing);
        }

    }
}
