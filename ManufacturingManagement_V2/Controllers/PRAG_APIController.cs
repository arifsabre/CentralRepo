using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class PRAG_APIController : ApiController
    {
        // GET: api/PRAG_API
        public IEnumerable<EInvoiceSaleMdl> Get()
        {
            //tokens redaing
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            string salerecid = "0";
            if (Request.Headers.Contains("salerecid"))
            {
                salerecid = Request.Headers.GetValues("salerecid").First();
            }
            //and so on ...
            
            //
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            List<EInvoiceSaleMdl> listInv = new List<EInvoiceSaleMdl> { };
            EInvoiceSaleMdl slMdl = new EInvoiceSaleMdl();
            slMdl = slBll.getEInvoiceSaleObject(Convert.ToInt32(salerecid));
            listInv.Add(slMdl);
            
            return listInv;

        }

        // GET: api/PRAG_API/5
        public EInvoiceJSonMdl Get(int id)
        {
            //tokens redaing
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            string salerecid = "0";
            if (Request.Headers.Contains("salerecid"))
            {
                salerecid = Request.Headers.GetValues("salerecid").First();
            }
            //and so on ...

            //
            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            EInvoiceJSonMdl slMdl = new EInvoiceJSonMdl();
            slMdl = slBll.GetEInvoiceJSon(Convert.ToInt32(salerecid));

            return slMdl;
        }

        // POST: api/PRAG_API
        public void Post(EInvoiceJSonMdl einv)
        {
            //tokens redaing
            string token = "";
            if (Request.Headers.Contains("authtoken"))
            {
                token = Request.Headers.GetValues("authtoken").First();
            }
            //and so on ...
            //

            //actions to be done into db...
            //return response

        }

        // PUT: api/PRAG_API/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PRAG_API/5
        public void Delete(int id)
        {
        }

        //
    }
}
