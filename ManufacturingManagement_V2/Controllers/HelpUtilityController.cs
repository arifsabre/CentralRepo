using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class HelpUtilityController : Controller
    {
        clsCookie objCookie = new clsCookie();
        private EmployeeBLL empBllObject = new EmployeeBLL();
        private ItemBLL itemBllObject = new ItemBLL();
        private UserBLL userBLL = new UserBLL();
        private ItemGroupBLL itemGroupBllObject = new ItemGroupBLL();
        private VendorBLL vendorBllObject = new VendorBLL();
        private VendorAddressBLL vAddrBllObject = new VendorAddressBLL();
        private CorrespondenceBLL corpBLL = new CorrespondenceBLL();
        private CompanyBLL compBLL = new CompanyBLL();
        private UserPermissionBLL uPerBLL = new UserPermissionBLL();
        private ProductionPlanBLL prdPlanBLL = new ProductionPlanBLL();
        private StatisticalSummaryBLL statBLL = new StatisticalSummaryBLL();
        private TenderDispatchBLL tenderDispBLL = new TenderDispatchBLL();
        private BankGuaranteeBLL bgBLL = new BankGuaranteeBLL();
        private AccountBLL acctBLL = new AccountBLL();
        private ERP_V1_ReportBLL erpV1RptBLL = new ERP_V1_ReportBLL();
        private VoucherBLL voucherBLL = new VoucherBLL();
        private OrderLedgerBLL orderLgrBLL = new OrderLedgerBLL();
        private InvoiceControlBLL invCtrlBLL = new InvoiceControlBLL();
        private MasterDocumentNameBLL mdocListBLL = new MasterDocumentNameBLL();
        private IMTETypeBLL imteTypeBLL = new IMTETypeBLL();
        private ImteBLL imteBLL = new ImteBLL();
        private TenderBLL tenderBll = new TenderBLL();
        //
        // GET: /HelpUtility/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteItemGroup(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemGroupBllObject.getObjectData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["groupid"].ToString(), ds.Tables[0].Rows[i]["groupname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteFinishedItemList(string term, int ccode=0, int groupid=0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemBllObject.getFinishedItemsList(groupid,ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["itemid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteStockItemList(string term, int groupid = 0, string itemtype = "0", int ccode = 0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemBllObject.getStockItemsList(itemtype, groupid, ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["itemid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteAllItemListGroup(string term, int ccode = 0, int groupid = 0)
        {
            if (ccode == 0)
            {
                ccode = Convert.ToInt32(objCookie.getCompCode());
            }
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemBllObject.getAllItemsListGroup(groupid, ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["itemid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteVendorPOItemList(string term, int vendorid = 0, int purchaseid = 0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemBllObject.getVendorPOItemsList(vendorid, purchaseid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["recid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteVendorPOItemList
        public JsonResult getVendorPOItemDetail(int recid)
        {
            OrderLedgerMdl stimdl = new OrderLedgerMdl();
            stimdl = orderLgrBLL.searchOrderLedger(recid);
            return new JsonResult { Data = new { itemid = stimdl.ItemId, purchaserate = stimdl.Rate, unit = stimdl.UnitId, itemname = stimdl.ItemDesc, unitname = stimdl.UnitName } };
        }

        [AcceptVerbs(HttpVerbs.Post)]//with select event of autocomplete
        public JsonResult GetDetail(string id)
        {
            zzzTestModel model = new zzzTestModel();
            EmployeeMdl objmdl = new EmployeeMdl();
            //objmdl = bllObject.searchObject(id);
            model.id = id;
            model.name = objmdl.EmpName;
            model.mobile = objmdl.ContactNo;
            return Json(model);
        }

        [HttpPost]//used with AutoCompleteItems
        public JsonResult getItemDetail(int itemid)
        {
            ItemMdl stimdl = new ItemMdl();
            stimdl = itemBllObject.searchObject(itemid);
            return new JsonResult { Data = new { salerate = stimdl.SaleRate, purchaserate = stimdl.PurchaseRate, unit = stimdl.Unit,itemname = stimdl.ItemName, unitname = stimdl.UnitName, itemcode = stimdl.ItemCode, hsncode = stimdl.HSNCode, shortname = stimdl.ShortName } };
        }

        [HttpPost]//used in AutoCompleteItems for indent & order
        public JsonResult getItemDetailByCode(string itemcode)
        {
            ItemMdl stimdl = new ItemMdl();
            stimdl = itemBllObject.getObjectByItemCode(itemcode);
            return new JsonResult { Data = new { itemid = stimdl.ItemId, salerate = stimdl.SaleRate, purchaserate = stimdl.PurchaseRate, unit = stimdl.Unit, itemname = stimdl.ItemName } };
        }

        [HttpPost]//used with AutoCompleteItem
        public JsonResult getTransactionUnitList(int itemid)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = itemBllObject.getTransactionUnitData(itemid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["unitid"].ToString(), ds.Tables[0].Rows[i]["unitname"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with correspondence company change
        public JsonResult getMasterDocumentNameList(int ccode)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = mdocListBLL.getObjectData(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["documentid"].ToString(), ds.Tables[0].Rows[i]["documentname"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used in imte master index
        public JsonResult getImteLocationList(int imtetypeid)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = imteBLL.getImteLocationData(imtetypeid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["location"].ToString(), ds.Tables[0].Rows[i]["location"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used in calibration history report index
        public JsonResult getImteTypeList(int ccode)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = imteTypeBLL.getObjectData(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["imtetypeid"].ToString(), ds.Tables[0].Rows[i]["imtetypename"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used in calibration index
        public JsonResult getImteList(int imtetypeid, int ccode=0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = imteBLL.getObjectData(imtetypeid, ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["imteid"].ToString(), ds.Tables[0].Rows[i]["idno"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteVendor(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = vendorBllObject.getObjectData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["vendorid"].ToString(), ds.Tables[0].Rows[i]["vendorname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteVendor
        public JsonResult getVendorDetail(int vendorid)
        {
            VendorMdl objmdl = new VendorMdl();
            objmdl = vendorBllObject.searchObject(vendorid);
            return new JsonResult { Data = new { vendorname = objmdl.VendorName } };
        }

        [HttpPost]//used with AutoCompleteVendor
        public JsonResult getAddressListForVendor(int vendorid)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = vAddrBllObject.getAddressDataForVendor(vendorid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["VendorAddId"].ToString(), ds.Tables[0].Rows[i]["VAddress"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteEmployee(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = empBllObject.getEmployeeSearchList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["empid"].ToString(), ds.Tables[0].Rows[i]["empname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteEmployeeNew(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = empBllObject.getEmployeeSearchListNew();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["newempid"].ToString(), ds.Tables[0].Rows[i]["empname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteEmployeeInactive(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = empBllObject.getEmployeeSearchListInactive();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["newempid"].ToString(), ds.Tables[0].Rows[i]["empname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteStaffList(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = empBllObject.getStaffList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["newempid"].ToString(), ds.Tables[0].Rows[i]["empname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }


        //[AcceptVerbs(HttpVerbs.Get)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteUsersList(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = userBLL.getUsersList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["userid"].ToString(), ds.Tables[0].Rows[i]["fullname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteUsersList
        public JsonResult getCompaniesByUser(int userid)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = compBLL.getCompanyDataByUser(userid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["compcode"].ToString(), ds.Tables[0].Rows[i]["cmpname"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteHODUsersList(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = userBLL.getHODUserList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["userid"].ToString(), ds.Tables[0].Rows[i]["fullname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteUsersListByPermission(string term,string opt)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            int entryid = 0;
            if (opt == "hod")
            {
                //compcode by logged in company
                entryid = 2155;
            }
            else if (opt == "admin")
            {
                //company by indent compcode
                entryid = 2165;
            }
            ds = userBLL.getUsersByPermissionData(entryid,Convert.ToInt32(objCookie.getCompCode()));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["userid"].ToString(), ds.Tables[0].Rows[i]["fullname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with dropDown EntryGroup in userpermission report
        public JsonResult getEntryListForGroup(int groupid=0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = uPerBLL.getEntryDetailData(groupid);
            resultall.Add(new KeyValuePair<string, string>("0", "ALL"));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["EntryId"].ToString(), ds.Tables[0].Rows[i]["EntryName"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with dropDown SectionList in statistical report
        public JsonResult getSegmentList(string sccode = "")
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = statBLL.getSegmentListData(sccode);
            resultall.Add(new KeyValuePair<string, string>("0", "ALL"));
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["segmentcode"].ToString(), ds.Tables[0].Rows[i]["segmentname"].ToString()));
            }
            //var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            //return Json(resultmain, JsonRequestBehavior.AllowGet);
            return Json(resultall, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteProductionPlanList(string term, string monthyear="", int ccode=0)
        {
            var resultx = new List<KeyValuePair<string, string>>();
            var resultall = new List<KeyValuePair<string, string>>();
            try
            {
                if (ccode == 0)
                {
                    ccode = Convert.ToInt32(objCookie.getCompCode());
                }
                DataSet ds = new DataSet();
                if (monthyear.Length > 0)
                {
                    int mth = Convert.ToInt32(monthyear.Split('-')[0]);
                    int yr = Convert.ToInt32(monthyear.Split('-')[1]);
                    ds = prdPlanBLL.getProductionPlanSearchList(mth, yr, ccode);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["planrecid"].ToString(), ds.Tables[0].Rows[i]["pdesc"].ToString()));
                    }
                }
                else
                {
                    ItemBLL objBll = new ItemBLL();
                    ds = objBll.getFinishedAndAssembledItems(ccode);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["itemid"].ToString(), ds.Tables[0].Rows[i]["itemcode"].ToString()));
                    }
                }
                var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    resultmain.Add(new KeyValuePair<string, string>("0", "No records found!"));
                }
                return Json(resultmain, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                resultx.Add(new KeyValuePair<string, string>("0", "Invalid Month/Year!"));
            }
            return Json(resultx, JsonRequestBehavior.AllowGet);
        }

        #region correspondence utilities

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteLetterNo(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getLetterNoSearchList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["recid"].ToString(), ds.Tables[0].Rows[i]["letterno"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceParty(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondencePartyList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["partyname"].ToString(), ds.Tables[0].Rows[i]["partyname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteCorrespondenceParty
        public JsonResult getCorrespondenceValuesForParty(string partyname)
        {
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceValuesForParty(partyname);
            string add = "";
            string cper = "";
            string sbj = "";
            string cref = "";
            string keywrd = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    add = ds.Tables[0].Rows[0]["CorpAddress"].ToString();
                    cper = ds.Tables[0].Rows[0]["ContactPerson"].ToString();
                    sbj = ds.Tables[0].Rows[0]["CorpSubject"].ToString();
                    cref = ds.Tables[0].Rows[0]["CorpReference"].ToString();
                    keywrd = ds.Tables[0].Rows[0]["Keywords"].ToString();
                }
            }
            return new JsonResult { Data = new { corpaddress = add, contactperson = cper, corpsubject = sbj, corpreference = cref, keywords = keywrd } };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceAddress(string term,string partyname)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceAddressList(partyname);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["CorpAddress"].ToString(), ds.Tables[0].Rows[i]["CorpAddress"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceContactPerson(string term,string partyname)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceContactPersonList(partyname);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ContactPerson"].ToString(), ds.Tables[0].Rows[i]["ContactPerson"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceSubject(string term,string partyname)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceSubjectList(partyname);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["CorpSubject"].ToString(), ds.Tables[0].Rows[i]["CorpSubject"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceReference(string term,string partyname)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceReferenceList(partyname);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["CorpReference"].ToString(), ds.Tables[0].Rows[i]["CorpReference"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCorrespondenceKeywords(string term, string partyname)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getCorrespondenceKeywordsList(partyname);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["Keywords"].ToString(), ds.Tables[0].Rows[i]["Keywords"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteCaseFileNos(string term, int compcode)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = corpBLL.getPONumbers(compcode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["porderid"].ToString(), ds.Tables[0].Rows[i]["ponumber"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteBillHeads(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = voucherBLL.getBillHeadsData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["headid"].ToString(), ds.Tables[0].Rows[i]["headname"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteTenderNo(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = tenderBll.getTenderListData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["tenderid"].ToString(), ds.Tables[0].Rows[i]["tenderno"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteTenderNo
        public JsonResult getPODetail(int tenderid)
        {
            BankGuaranteeMdl bgmdl = new BankGuaranteeMdl();
            bgmdl = bgBLL.getTenderDetail(tenderid);
            return new JsonResult { Data = new { loadetail = bgmdl.LoaDetail, podetail = bgmdl.PODetail, railwayname = bgmdl.RailwayName} };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompletePurchaseNumbers(string term, int ccode=0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = bgBLL.getPurchaseNumbers(ccode);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["porderid"].ToString(), ds.Tables[0].Rows[i]["PONumber"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteAccountWithGroup(string term,string rectype="", int ccode=0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            if (rectype == "a")
            {
                ds = acctBLL.getAccountWithGroupData(recType.Account, ccode);
            }
            else if (rectype == "g")
            {
                ds = acctBLL.getAccountWithGroupData(recType.Group, ccode);
            }
            else if (rectype == "b")
            {
                ds = acctBLL.getAccountListByGroup(fAccount.Bank, cashType.Bank,"0");
            }
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["accode"].ToString(), ds.Tables[0].Rows[i]["acdesc"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteRailway(string term)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = erpV1RptBLL.getRailwayData();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["RailwayId"].ToString(), ds.Tables[0].Rows[i]["RailwayName"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteConsignee(string term, int railwayid =0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = erpV1RptBLL.GetConsigneeData(railwayid);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["ConsigneeId"].ToString(), ds.Tables[0].Rows[i]["ConsigneeName"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        #region payment receipt

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompletePayingAuthority(string term, int railwayid=0)
        {
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = erpV1RptBLL.getPayingAuthorityData(railwayid,"0");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                resultall.Add(new KeyValuePair<string, string>(ds.Tables[0].Rows[i]["PayingAuthId"].ToString(), ds.Tables[0].Rows[i]["PayingAuthName"].ToString()));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompletePayingAuthority
        public JsonResult getPayingAuthorityInfo(int payingauthid)
        {
            if (payingauthid == 0)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Address2 = "", Address3 = "", Address4 = "", StateName = "", StateCode = "", GSTIN = ""
                    }
                };
            }
            clsMyClass mc = new clsMyClass();
            voucherBLL = new VoucherBLL();
            DataSet ds = new DataSet();
            ds = voucherBLL.getPayingAuthorityDetail(payingauthid);
            return new JsonResult
            {
                Data = new
                {
                    Address2 = ds.Tables[0].Rows[0]["Address2"].ToString(),
                    Address3 = ds.Tables[0].Rows[0]["Address3"].ToString(),
                    Address4 = ds.Tables[0].Rows[0]["Address4"].ToString(),
                    StateName = ds.Tables[0].Rows[0]["StateName"].ToString(),
                    StateCode = ds.Tables[0].Rows[0]["StateCode"].ToString(),
                    GSTIN = ds.Tables[0].Rows[0]["GSTinNo"].ToString(),
                }
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompleteSaleBillNumbers(string term, int payingauthid)
        {
            clsMyClass mc = new clsMyClass();
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = voucherBLL.getBillsToReceiptData(payingauthid);
            string billnostr = "";
            string billno = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                billno = ds.Tables[0].Rows[i]["billno"].ToString();
                billnostr = ds.Tables[0].Rows[i]["billno"].ToString() 
                + " Rs." + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["balance"].ToString()));
                resultall.Add(new KeyValuePair<string, string>(billno, billnostr));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//used with AutoCompleteSaleBillNumbers
        public JsonResult getBillInformationByBillNo(string billno)
        {
            voucherBLL = new VoucherBLL();
            int salerecid = voucherBLL.getSaleRecIdByBillNo(billno);
            return getBillInformationBySaleRecId(salerecid);
        }

        [HttpPost]
        public JsonResult getBillInformationBySaleRecId(int salerecid)
        {
            clsMyClass mc = new clsMyClass();
            voucherBLL = new VoucherBLL();
            DataSet ds = new DataSet();
            ds = voucherBLL.getBillInfoBySaleRecId(salerecid);
            return new JsonResult
            {
                Data = new
                {
                    PayingAuthId = ds.Tables[0].Rows[0]["PayingAuthId"].ToString(),
                    PayingAuthName = ds.Tables[0].Rows[0]["PayingAuthName"].ToString(),
                    ConsigneeName = ds.Tables[0].Rows[0]["ConsigneeName"].ToString(),
                    RCInfo = ds.Tables[0].Rows[0]["RCInfo"].ToString(),
                    AcCode = ds.Tables[0].Rows[0]["AcCode"].ToString(),
                    Balance = ds.Tables[0].Rows[0]["Balance"].ToString(),
                    BillNo = ds.Tables[0].Rows[0]["BillNo"].ToString(),
                    RlyShortName = ds.Tables[0].Rows[0]["RlyShortName"].ToString(),
                    BillPODesc = ds.Tables[0].Rows[0]["BillPODesc"].ToString(),
                    PaymentMode = ds.Tables[0].Rows[0]["PaymentMode"].ToString(),
                    InvAmount = ds.Tables[0].Rows[0]["InvAmount"].ToString(),
                    InvDate = mc.getStringByDateDDMMYYYY(Convert.ToDateTime(ds.Tables[0].Rows[0]["InvDate"].ToString())),
                    PONumber = ds.Tables[0].Rows[0]["PONumber"].ToString()
                }
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AutoCompletePurchaseBillNumbers(string term, int vendorid)
        {
            clsMyClass mc = new clsMyClass();
            var resultall = new List<KeyValuePair<string, string>>();
            DataSet ds = new DataSet();
            ds = voucherBLL.getBillsForPaymentData(vendorid);
            string billno = "";
            string billid = "";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                billno = "Bill-" + ds.Tables[0].Rows[i]["billno"].ToString() + " Rs." + mc.getINRCFormat(Convert.ToDouble(ds.Tables[0].Rows[i]["balance"].ToString()));
                billid = ds.Tables[0].Rows[i]["billno"].ToString() + "#" //0
                         + ds.Tables[0].Rows[i]["accode"].ToString() + "#" //1
                         + ds.Tables[0].Rows[i]["balance"].ToString(); //2
                resultall.Add(new KeyValuePair<string, string>(billid, billno));
            }
            var resultmain = resultall.Where(s => s.Value.ToLower().Contains(term.ToLower())).Select(w => w).ToList();
            return Json(resultmain, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]//used with fin-year dropdown
        public JsonResult getDateRangeByFinYear(int ccode, string finyr)
        {
            clsMyClass mc = new clsMyClass();
            compBLL = new CompanyBLL();
            FinYearMdl fymdl = new FinYearMdl();
            fymdl = compBLL.getDateRangeByFinancialYear(ccode,finyr);
            return new JsonResult { Data = new { fromdt = mc.getStringByDateForJavaScript(fymdl.FromDate), todt = mc.getStringByDateForJavaScript(fymdl.ToDate) } };
        }

    }
}
