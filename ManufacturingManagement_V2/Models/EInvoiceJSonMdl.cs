using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class EInvoiceJSonMdl
    {
        //[Key]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        //public int SaleRecId { get; set; }

        public string TaxSch { get; set; }
        public string Version { get; set; }
        public string Irn { get; set; }
        public tranDtls TranDtls { get; set; }
        public docDtls DocDtls { get; set; }
        public expDtls ExpDtls { get; set; }
        public sellerDtls SellerDtls { get; set; }
        public buyerDtls BuyerDtls { get; set; }
        public dispDtls DispDtls { get; set; }
        public shipDtls ShipDtls { get; set; }
        public valDtls ValDtls { get; set; }
        public refDtls RefDtls { get; set; }
        public payDtls PayDtls { get; set; }
        public List<itemList> ItemList { get; set; }
    }

    public class tranDtls
    {
        public string Catg { get; set; }
        public string RegRev { get; set; }
        public string Typ { get; set; }
        public string EcmTrnSel { get; set; }
        public string EcmTrn { get; set; }
        public string EcmGstin { get; set; }
    }

    public class docDtls
    {
        public string Typ { get; set; }
        public string No { get; set; }
        public string Dt { get; set; }
        public string OrgInvNo { get; set; }
    }

    public class expDtls
    {
        public string Expcat { get; set; }
        public string WthPay { get; set; }
        public string ShipBNo { get; set; }
        public string ShipBDt { get; set; }
        public string Port { get; set; }
        public string InvForCur { get; set; }
        public string ForCur { get; set; }
        public string CntCode { get; set; }
    }

    public class sellerDtls
    {
        public string Gstin { get; set; }
        public string TrdNm { get; set; }
        public string Bno { get; set; }
        public string Bnm { get; set; }
        public string Flno { get; set; }
        public string Loc { get; set; }
        public string Dst { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class buyerDtls
    {
        public string Gstin { get; set; }
        public string TrdNm { get; set; }
        public string Bno { get; set; }
        public string Bnm { get; set; }
        public string Flno { get; set; }
        public string Loc { get; set; }
        public string Dst { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class dispDtls
    {
        public string Gstin { get; set; }
        public string TrdNm { get; set; }
        public string Bno { get; set; }
        public string Bnm { get; set; }
        public string Flno { get; set; }
        public string Loc { get; set; }
        public string Dst { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class shipDtls
    {
        public string Gstin { get; set; }
        public string TrdNm { get; set; }
        public string Bno { get; set; }
        public string Bnm { get; set; }
        public string Flno { get; set; }
        public string Loc { get; set; }
        public string Dst { get; set; }
        public string Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class valDtls
    {
        public string AssVal { get; set; }
        public string CgstVal { get; set; }
        public string SgstVal { get; set; }
        public string IgstVal { get; set; }
        public string CesVal { get; set; }
        public string StCesVal { get; set; }
        public string CesNonAdVal { get; set; }
        public string Disc { get; set; }
        public string TotInvVal { get; set; }
    }

    public class refDtls
    {
        public string InvRmk { get; set; }
        public string InvStDt { get; set; }
        public string InvEndDt { get; set; }
        public string PrecInvNo { get; set; }
        public string PrecInvDt { get; set; }
        public string RecAdvRef { get; set; }
        public string TendRef { get; set; }
        public string ContrRef { get; set; }
        public string ExtRef { get; set; }
        public string ProjRef { get; set; }
        public string PORef { get; set; }
    }
    public class payDtls
    {
        public string Nam { get; set; }
        public string Mode { get; set; }
        public string FinInsBr { get; set; }
        public string PayTerm { get; set; }
        public string PayInstr { get; set; }
        public string CrTrn { get; set; }
        public string DirDr { get; set; }
        public string CrDay { get; set; }
        public string BalAmt { get; set; }
        public string PayDueDt { get; set; }
        public string AcctDet { get; set; }
    }
    public class itemList
    {
        public string Item { get; set; }
        public string PrdNm { get; set; }
        public string PrdDesc { get; set; }
        public string HsnCd { get; set; }
        public string BarCde { get; set; }
        public string Qty { get; set; }
        public string FreeQty { get; set; }
        public string Unit { get; set; }
        public string UnitPrice { get; set; }
        public string TotAmt { get; set; }
        public string CgstRt { get; set; }
        public string SgstRt { get; set; }
        public string IgstRt { get; set; }
        public string CesRt { get; set; }
        public string CesNonAdVal { get; set; }
        public string StateCes { get; set; }
        public string TotItemVal { get; set; }
        public string Discount { get; set; }
        public string OthChrg { get; set; }
        public string AssAmt { get; set; }
    }

}