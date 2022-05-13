using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
//using TaxProEInvoice.API;
using ManufacturingManagement_V2.Models;
using QRCoder;

namespace ManufacturingManagement_V2.Controllers
{
    public class QRCodeController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        //eInvoiceSession eInvSession = new eInvoiceSession(true, true);
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        // GET: Home
        public ActionResult Index()
        {
            //TxnRespWithObj<eInvoiceSession> txnRespWithObj = eInvoiceAPI.GetAuthTokenAsync(eInvSession);
            //if (txnRespWithObj.IsSuccess)
            //{
                //DisplayLoginDetail();
                //rtbResponce.Text = JsonConvert.SerializeObject(rtbResponce.Text);
            //}
            //txtResponceHdr.Text = "Auth Api Responce";
            //rtbResponce.Text = txnRespWithObj.TxnOutcome;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string qrcode)
        {
            //working fine
            using (MemoryStream ms = new MemoryStream())
            {
                QRCoder.QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(qrcode, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
