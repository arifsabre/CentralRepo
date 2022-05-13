using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Collections;
using System.Data;
using ManufacturingManagement_V2.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace ManufacturingManagement_V2.Controllers
{
    public class PRAG_API_TESTController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        public JsonResult IndexT1()
        {
            setViewData();

            return IndexGetAPI();
        }

        public JsonResult IndexGetAPI()
        {
            setViewData();

            //IEnumerable<EInvoiceSaleMdl> invlist = null;
            EInvoiceJSonMdl einv = new EInvoiceJSonMdl();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58521/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//or application/xml

                //sample tokens passing
                client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                client.DefaultRequestHeaders.Add("gstin", "29AAACGIIIII1Z3");
                client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                //HTTP GET
                var responseTask = client.GetAsync("PRAG_API/30970");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<EInvoiceJSonMdl>();
                    readTask.Wait();

                    einv = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here
                    //return message
                }
            }

            return Json(einv, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IndexPostAPI()
        {
            setViewData();

            EInvoiceSaleBLL slBll = new EInvoiceSaleBLL();
            EInvoiceJSonMdl einv = new EInvoiceJSonMdl();
            einv = slBll.GetEInvoiceJSon(Convert.ToInt32(30970));

            //check or convert json to object from- 
            //k-https://json2csharp.com/json-to-csharp

            //tests
            byte[] appk = generateAppKey();//ok
            //byte[] appk1 = EncryptText("dadadadadaad","toencrypt");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:58521/api/");
                client.DefaultRequestHeaders.Accept.Clear();

                //sample tokens passing
                client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                client.DefaultRequestHeaders.Add("gstin", "29AAACGIIIII1Z3");
                client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<EInvoiceJSonMdl>("PRAG_API", einv);
                //or var postTask = client.PostAsXmlAsync<EmployeeMdl>("PRAG_API", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //redirect to success url...
                }
            }

            //redirect to error url...
            return View();
        }

        #region sample codes
        /// <summary>
        /// Generation of appKey
        /// </summary>
        /// <returns></returns>
        public static byte[] generateAppKey()
        {
            Aes KEYGEN = Aes.Create();
            byte[] secretKey = KEYGEN.Key;
            return secretKey;
        }

        /// <summary>
        /// Password Encryption
        /// </summary>
        /// <param name="password"></param>
        /// <param name="Publickey"></param>
        /// <returns></returns>
        public static string EncryptAsymmetric(string password, string Publickey)
        {
            byte[] keyBytes = Convert.FromBase64String(Publickey);
            //AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            //RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters();
            //rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            //rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);
            byte[] plaintext = Encoding.UTF8.GetBytes(password);
            byte[] ciphertext = rsa.Encrypt(plaintext, false);
            string cipherresult = Convert.ToBase64String(ciphertext);
            return cipherresult;
        }

        // Create a method to encrypt a text using a RSA algorithm public key   
        public static byte[] EncryptText(string publicKey, string text)
        {
            // Convert the text to an array of bytes   
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(text);

            // Create a byte array to store the encrypted data in it   
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Set the rsa pulic key   
                rsa.FromXmlString(publicKey);

                // Encrypt the data and store it in the encyptedData Array   
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
            // Save the encypted data array into a file   
            // File.WriteAllBytes(fileName, encryptedData);

            return encryptedData;
        }

        // Method to decrypt the data using a RSA algorithm private key   
        public static string DecryptData(string privateKey, byte[] dataToDecrypt)
        {
            // read the encrypted bytes from the file   
            //byte[] dataToDecrypt = File.ReadAllBytes(fileName);

            // Create an array to store the decrypted data in it   
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Set the private key of the algorithm   
                rsa.FromXmlString(privateKey);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }

            // Get the string value from the decryptedData byte array   
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            return byteConverter.GetString(decryptedData);
        }


        /// <summary>
        /// App Key Encryption
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string Encrypt(byte[] appKey, string publicKey)
        {
            byte[] keyBytes = Convert.FromBase64String(publicKey);
            //AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            //RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            //RSAParameters rsaParameters = new RSAParameters();
            //rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            //rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //rsa.ImportParameters(rsaParameters);
            byte[] plaintext = appKey;
            byte[] ciphertext = rsa.Encrypt(plaintext, false);
            string cipherresult = Convert.ToBase64String(ciphertext);
            return cipherresult;
        }

        /// <summary>
        /// The following C#.Net code snippet can be used for decrypting the encrypted sek using the appkey.
        /// Here the encryptedSek is the one that is received in response to the authentication.
        /// </summary>
        /// <param name="encryptedSek"></param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public static byte[] DecryptBySymmetricKey(string encryptedSek, byte[] appkey)
        {
            //Decrypting SEK
            try
            {
                byte[] dataToDecrypt = Convert.FromBase64String(encryptedSek);
                var keyBytes = appkey;
                AesManaged tdes = new AesManaged();
                tdes.KeySize = 256;
                tdes.BlockSize = 128;
                tdes.Key = keyBytes;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decrypt__1 = tdes.CreateDecryptor();
                byte[] deCipher = decrypt__1.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                tdes.Clear();
                string EK_result = Convert.ToBase64String(deCipher);
                //return EK_result;
                return deCipher;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The following C#.Net code snippet can be used for encrypting the data using the symmetric key.
        /// The decrypted sek need to be passed here
        /// (It is got by decrypting the obtained SEK after successful authentication)
        /// </summary>
        /// <param name="jsondata"></param>
        /// <param name="sek"></param>
        /// <returns></returns>
        public static string EncryptBySymmetricKey(string jsondata, string sek)
        {
            //Encrypting SEK
            try
            {
                byte[] dataToEncrypt = Convert.FromBase64String(jsondata);
                var keyBytes = Convert.FromBase64String(sek);
                AesManaged tdes = new AesManaged();
                tdes.KeySize = 256;
                tdes.BlockSize = 128;
                tdes.Key = keyBytes;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                //pICryptoTransform encrypt__1 = tdes.CreateEncryptor();
                //byte[] deCipher = encrypt__1.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                tdes.Clear();
                //string EK_result = Convert.ToBase64String(deCipher);
                return "";//EK_result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Decoding the Signed eInvoice
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string Decode(string token)
        {
            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];
            //byte[] crypto = Base64UrlDecode(parts[2]);
            //var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            //var headerData = JObject.Parse(headerJson);
            //var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            //var payloadData = JObject.Parse(payloadJson);
            //return headerData.ToString() + payloadData.ToString();
            return "";
        }

        /// <summary>
        /// Note: the ‘ProdPubKey.cer’ mentioned here is the Key provided for 
        /// the verification of the signed content.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool ValidateToken(string token)
        {
            //var handler = new JsonWebTokenHandler();
            //string path = HttpContext.Current.Server.MapPath("~") + "\\EncDesc\\ProdPubKey.cer";
            //X509Certificate2 signingPublicCert = new X509Certificate2(path);
            //Microsoft.IdentityModel.Tokens.X509SecurityKey publickey = new Microsoft.IdentityModel.Tokens.X509SecurityKey(signingPublicCert);
            //TokenValidationResult result = handler.ValidateToken(token,
            //new TokenValidationParameters
            //{
            //    ValidIssuer = "NIC",
            //    ValidateAudience = false,
            //    IssuerSigningKey = publickey,
            //    ValidateLifetime = false
            //});
            //bool isValid = result.IsValid;
            //SecurityToken securityToken = handler.ReadToken(token);
            //return isValid;
            return true;
        }


        #endregion

        //async task Get/Post
        //to be revised and checked
        public async Task<ActionResult> IndexTaskSync()
        {
            setViewData();

            await RunAsyncGet();//ok
            //await RunAsyncPost();// to be defined
            return View();
        }

        //to be revised and checked
        public static async Task<EInvoiceSaleMdl> RunAsyncGet()
        {
            EInvoiceSaleMdl mdl = new EInvoiceSaleMdl();
            string[] x1 = { };
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:58521/api/PRAG_ERP_API/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));//or application/json

                    //sample tokens passing
                    client.DefaultRequestHeaders.Add("client-id", "TESTCLIENTID");
                    client.DefaultRequestHeaders.Add("client-secret", "CLIENTSECRET");
                    client.DefaultRequestHeaders.Add("salerecid", "30970");
                    client.DefaultRequestHeaders.Add("authtoken", "0aAjBKdo7rcNYJB30g5DS2u8z");

                    //New code:
                    var response = await client.GetAsync("GetInvoiceResult").ConfigureAwait(false);
                    //var response = await client.GetAsync("Authenticate").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        mdl = await response.Content.ReadAsAsync<EInvoiceSaleMdl>();
                        //x1 = await response.Content.ReadAsAsync<string[]>();
                    }

                    return mdl;
                }
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                return mdl;
            }

        }

        //
    }
}