using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class Network_StatusController : Controller
    {
        // GET: Network_Status
        BioLive bll = new BioLive();
        readonly int PP_Unit_2_Reader_IN = 1;

        readonly int Reader_l_PP2_Out = 14;

        readonly int PP_Unit_1_Reader_IN = 3;
        readonly int PP_Unit_1_Reader_OUT = 8;

        readonly int PI_Unit_1_Reader_IN = 4;
        readonly int PI_Unit_1_Reader_OUT = 10;

        readonly int Prag_Rubber_Unit_1_Reader_IN = 5;
        readonly int Prag_Rubber_Unit_1_Reader_OUT = 2;


        readonly int PI_Unit_2_Reader_IN = 6;
        readonly int PI_Unit_2_Reader_OUT = 9;

        readonly int Head_Office_Reader_IN_OUT = 7;

        readonly int erpserver = 16;
        readonly int bioserver = 17;
        readonly int gateway = 19;
        readonly int sms1 = 1;
        readonly string StatusRead = "Connected";
        readonly string mycon = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
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
        public ActionResult Index()
        {
            Reader_l_PP2_IN();
            Reader_l_PP2_Outt();
    

            Reader_3_PP1_IN();
        
            Reader_8_PP1_OUT();
     

            Reader_4_PI_IN();
        
            Reader_10_PI_OUT();
      

            Reader_5_Rubber_IN();
        
            Reader_2_Rubber_OUT();
    

            Reader_7_HO_IN_OUT();


            Reader_6_PI2_IN();
      
            Reader_9_PI2_OUT();
        

            Gateway();
        

            ERP_Server();
     

            Bio_Server();

            setViewData();

            B_INTEGRATIONMDI model = new B_INTEGRATIONMDI();
            model.Item_List = bll.GetAllNetworkStatus();
            return View(model);
        }
        private void Reader_l_PP2_IN()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.30", 20000);
                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PP_Unit_2_Reader_IN.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    string DeviceId = PP_Unit_2_Reader_IN.ToString();
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PP_Unit_2_Reader_IN + "  " + DateTime.Now.ToString());
                }
                else
                {
                    string s = "TimedOut";
                    string t = "2";
                    string add = "192.168.0.30";
                    string DeviceId = PP_Unit_2_Reader_IN.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId);
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                   // WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
            //Console.ReadKey();
        }

        private void Reader_l_PP2_Outt()
        {

            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.181", 20000);
                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + Reader_l_PP2_Out.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = Reader_l_PP2_Out.ToString();
                    string status1 = StatusRead;
                    int smsstatus = sms1;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + Reader_l_PP2_Out + "  " + DateTime.Now.ToString());
                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.181";
                    string DeviceId = Reader_l_PP2_Out.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    //string Connect = "NotConnected";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId);
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }

            }
            catch
            {

                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }
        private void Reader_3_PP1_IN()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.26", 20000);
                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PP_Unit_1_Reader_IN.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PP_Unit_1_Reader_IN.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PP_Unit_1_Reader_IN + "  " + DateTime.Now.ToString());
                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.26";
                    string DeviceId = PP_Unit_1_Reader_IN.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                // Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }
        private void Reader_8_PP1_OUT()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.27", 20000);
                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PP_Unit_1_Reader_OUT.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PP_Unit_1_Reader_OUT.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PP_Unit_1_Reader_OUT + "  " + DateTime.Now.ToString());
                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.27";
                    string DeviceId = PP_Unit_1_Reader_OUT.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                // Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }

        private void Reader_4_PI_IN()
        {

            try
            {

                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.169", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PI_Unit_1_Reader_IN.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PI_Unit_1_Reader_IN.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PI_Unit_1_Reader_IN + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.169";
                    string DeviceId = PI_Unit_1_Reader_IN.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }
        private void Reader_10_PI_OUT()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.164", 20000);
                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PI_Unit_1_Reader_OUT.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PI_Unit_1_Reader_OUT.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PI_Unit_1_Reader_OUT + "  " + DateTime.Now.ToString());

                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.164";
                    string DeviceId = PI_Unit_1_Reader_OUT.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }

        private void Reader_5_Rubber_IN()
        {

            try
            {

                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.28", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + Prag_Rubber_Unit_1_Reader_IN.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = Prag_Rubber_Unit_1_Reader_IN.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + Prag_Rubber_Unit_1_Reader_IN + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.28";
                    string DeviceId = Prag_Rubber_Unit_1_Reader_IN.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }
        private void Reader_2_Rubber_OUT()
        {

            try
            {

                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.25", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + Prag_Rubber_Unit_1_Reader_OUT.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = Prag_Rubber_Unit_1_Reader_OUT.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + Prag_Rubber_Unit_1_Reader_OUT + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.25";
                    string DeviceId = Prag_Rubber_Unit_1_Reader_OUT.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }

            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }

            //Console.ReadKey();
        }
        private void Reader_7_HO_IN_OUT()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.29", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + Head_Office_Reader_IN_OUT.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = Head_Office_Reader_IN_OUT.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + Head_Office_Reader_IN_OUT + "  " + DateTime.Now.ToString());

                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.29";
                    string DeviceId = Head_Office_Reader_IN_OUT.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }
      
        private void Reader_6_PI2_IN()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.163", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PI_Unit_2_Reader_IN.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PI_Unit_2_Reader_IN.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PI_Unit_2_Reader_IN + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.163";
                    string DeviceId = PI_Unit_2_Reader_IN.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                // Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }
        private void Reader_9_PI2_OUT()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.162", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + PI_Unit_2_Reader_OUT.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = PI_Unit_2_Reader_OUT.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + PI_Unit_2_Reader_OUT + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.162";
                    string DeviceId = PI_Unit_2_Reader_OUT.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                // Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }
        private void ERP_Server()
        {

            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("172.16.0.253", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + erpserver.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = erpserver.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + erpserver + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "172.16.0.253";
                    string DeviceId = erpserver.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId);
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }
        private void Bio_Server()
        {

            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.167", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + bioserver.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = bioserver.ToString();
                    int smsstatus = sms1;
                    string status1 = StatusRead;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + bioserver + "  " + DateTime.Now.ToString());
                }
                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.167";
                    string DeviceId = bioserver.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId);
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }

        private void Gateway()
        {

            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.0.1", 20000);

                if (reply.Address != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " " + "Time : " + reply.RoundtripTime.ToString() + " " + "Address : " + reply.Address + " " + "DeviceId:" + gateway.ToString());
                    string s = reply.Status.ToString();
                    string t = reply.RoundtripTime.ToString();
                    string add = reply.Address.ToString();
                    string DeviceId = gateway.ToString();
                    string status1 = StatusRead;
                    int smsstatus = sms1;
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + reply.Status + "  " + reply.RoundtripTime.ToString() + "  " + reply.Address + "  " + gateway + "  " + DateTime.Now.ToString());

                }

                else
                {
                    string s = "TimedOut";
                    string t = "0";
                    string add = "192.168.0.1";
                    string DeviceId = gateway.ToString();
                    int smsstatus = 0;
                    string status1 = "NotConnect";
                    Console.WriteLine("Status :  " + s + " " + "Time : " + " " + t + " " + "Address : " + add + " " + "DeviceId:" + DeviceId + "");
                    ReaderStatus_SaveLog(s, t, add, DeviceId, smsstatus, status1);
                    WriteToFile("\n " + s + "  " + t + "  " + add + "  " + DeviceId + "  " + DateTime.Now.ToString());
                }
            }
            catch
            {
                //Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
        }

        private void ReaderStatus_SaveLog(string s, string t, string add, string DeviceId, int smsstatus, string status1)
        {

            using (SqlConnection con = new SqlConnection(mycon))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[ZZZZ_Insert_Reader_PingLog]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Clear();
                try
                {
                    //con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DeviceStatus", s));
                    cmd.Parameters.Add(new SqlParameter("@ReaderReplyTime", t));
                    cmd.Parameters.Add(new SqlParameter("@DeviceIP", add));
                    cmd.Parameters.Add(new SqlParameter("@DeviceLocation", DeviceId));
                    cmd.Parameters.Add(new SqlParameter("@SmsStatus", smsstatus));
                    cmd.Parameters.Add(new SqlParameter("@status1", status1));


                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        //Console.WriteLine("Records Inserted Successfully.");
                    }

                }
                catch (SqlException)
                {
                    //Console.WriteLine("Error Generated. Details: " + e.ToString());
                }
                finally
                {
                    con.Close();
                    // Console.ReadKey();
                }
            }
        }

        private void WriteToFile(string text)
        {
            string path = "D:\\SMSLOG\\NetworkStatusLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }


    }
}