using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class LoginController : Controller
    {
        clsCookie objCookie = new clsCookie();
        clsMyClass mc = new clsMyClass();
        LoginBLL loginBLL = new LoginBLL();
        CompanyBLL compBLL = new CompanyBLL();
        FinYearMdl finyrBLL = new FinYearMdl();

        private void setViewData()
        {
            ViewData["UserName"] = objCookie.getUserName();
            ViewData["CompName"] = objCookie.getCmpName();
            ViewData["FinYear"] = objCookie.getFinYear();
            ViewData["Dept"] = objCookie.getDepartment();
            ViewData["mnuOpt"] = objCookie.getMenu();
        }

        // GET: /Starter/
        public ActionResult Starter()
        {
            return View();
        }

        // GET: /Login/
        public ActionResult LoginUser()
        {
            clsCookie objCookie = new clsCookie();
            if (objCookie.checkSessionState() == true)
            {
                UserLogBLL ubll = new UserLogBLL();
                //with clearing of users-variable-tables
                ubll.setUserLogToLogout(objCookie.getUserId());
                objCookie.RemoveCookie(true);
                Session.Remove("userid");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(FormCollection form)
        {
            string loginopt = System.Configuration.ConfigurationManager.AppSettings["loginOption"].ToString();
            string captcha = form["txtCaptcha"].ToString();
            if (Session["Captcha"] == null || Session["Captcha"].ToString() != captcha)
            {
                ViewBag.Result = "Invalid Captcha!";
                return View();
            }
            LoginMdl objlogin = new LoginMdl();
            objlogin.UserName = form["UserName"].ToString();
            objlogin.PassW = form["PassW"].ToString();
            //getting values into LoginMdl
            loginBLL.getUserLoginInfo(objlogin);
            if (loginBLL.Result == true)
            {
                //set user info by objlogin object to use further untill cookie created
                Session["userid"] = objlogin.UserId;
                //getting default company for the user
                List<CompanyMdl> complist = compBLL.getCompanyListByUser(Convert.ToInt32(Session["userid"].ToString()));
                string sd = "";
                //
                Session["svloginuser"] = objlogin.FullName;
                //------------------------------------------
                string chkprofile = System.Configuration.ConfigurationManager.AppSettings["chkProfile"].ToString();
                if (chkprofile == "1")
                {
                    //checking user profile policy
                    if (objlogin.UserName.Length < 8 || objlogin.MobileNo.Length == 0 || objlogin.EMail.Length == 0)
                    {
                        return RedirectToAction("UpdateProfile", "UserAdmin");
                    }
                    //checking password policy
                    if (loginBLL.isValidForPasswordPolicy(objlogin.PassW) == false)
                    {
                        return RedirectToAction("ChangePassword", "Login");
                    }
                    //--cl--user profile & pasword policy
                }
                //--------------------------------------------------------------
                if (loginopt == "online-otp")
                {
                    string otp = mc.getOTP();
                    Session["svotp"] = otp;
                    string msg = "Dear " + objlogin.FullName + ", ";
                    msg += otp + " is OTP to complete login in to PRAG ERP System.";
                    msg += " PRAG INDUSTRIES (INDIA) PVT LIMITED";
                    //send otp on mobile as
                    mc.performAPICall_SendSMS(objlogin.MobileNo, msg);
                    //or send otp on email as
                    //mc.sendOTPtoEmail(msg, objlogin.EMail);
                    string msgAlt = "Dear " + objlogin.FullName + ",";
                    msgAlt += "<br/>You have accessed prag erp system for which One Time Password (OTP) ";
                    msgAlt += "has been generated and sent on your registered mobile number ";
                    msgAlt += "XXXXXX" + objlogin.MobileNo.Substring(6, 4) + " on " + DateTime.Now.ToString() + ".";
                    msgAlt += "<br/>In case you have not logged in to your account, please call immediately to erp admin.";
                    sd = mc.sendOTPtoEmail(msgAlt, objlogin.EMail);
                    //sd = "1";
                }
                else if (loginopt == "offline")
                {
                    loginBLL.performLogin(objlogin.UserId.ToString(), complist[0].CompCode.ToString(), getRecentFinYear(), "123", false, "Home");
                    return RedirectToAction("Index", "Home");
                }
                if (sd == "1")
                {
                    Session["svcompcode"] = complist[0].CompCode.ToString();
                    Session["svfinyear"] = getRecentFinYear();
                    Session["svlogintype"] = objlogin.LoginType.ToString();
                    return RedirectToAction("PerformLogin");//otp
                }
                else
                {
                    ViewBag.Result = "Problem in sending OTP! Please contact to administrator.";
                    return View();
                }
            }
            ViewBag.Result = "Invalid Login or Password!";
            return View(); 
        }

        // GET: /otp-comp-finyear/
        public ActionResult PerformLogin(string msg="")
        {
            if (Session["userid"] == null) { return RedirectToAction("LoginUser", "Login"); };
            ViewBag.LoginTypeOTP = Session["svlogintype"].ToString();
            ViewBag.UserOTP = Session["svloginuser"].ToString();
            ViewBag.errormessage = msg;
            return View();
        }

        [HttpPost]
        public ActionResult PerformLogin(FormCollection form)
        {
            string otp = form["OTP"].ToString();
            if (Session["userid"] != null)
            {
                if (Session["svlogintype"].ToString() != "0")
                {
                    if (Session["svotp"].ToString() == otp)
                    {
                        loginBLL.performLogin(Session["userid"].ToString(), Session["svcompcode"].ToString(), Session["svfinyear"].ToString(), otp, false, "Home");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (Session["svlogintype"].ToString() == "0")//admin login
                {
                    string PinOTP = System.Configuration.ConfigurationManager.AppSettings["PinOTP"].ToString();
                    if (otp == PinOTP || otp == Session["svotp"].ToString())
                    {
                        loginBLL.performLogin(Session["userid"].ToString(), Session["svcompcode"].ToString(), Session["svfinyear"].ToString(), otp, false, "Home");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("PerformLogin",new { msg= "Invalid OTP entered or invalid attempt!" });
        }

        private string getRecentFinYear()
        {
            string finyr = "";
            int yr = DateTime.Now.Year;
            int mt = DateTime.Now.Month;
            if (mt >= 4 && mt <= 12)//apr-dec
            {
                finyr = yr.ToString().Substring(2, 2) + "-" + (yr + 1).ToString().Substring(2, 2);
            }
            else if (mt >= 1 && mt <= 3)//jan-march
            {
                finyr = (yr - 1).ToString().Substring(2, 2) + "-" + yr.ToString().Substring(2, 2);
            }
            return finyr;
        }

        // GET: /company/
        public ActionResult ChangeCompany(string msg = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Result = msg;
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(objCookie.getUserId())), "compcode", "cmpname", objCookie.getCompCode());
            return View();
        }

        [HttpPost]
        public ActionResult ChangeCompany(FormCollection form)
        {
            string compcode = form["ddlCompany"].ToString();
            if (objCookie.getCompCode() != compcode)
            {
                loginBLL.changeCoockieForCompany(compcode);
            }
            return RedirectToAction("ChangeCompany", new { msg = "Company Changed" });
        }

        // GET: /finyear/
        public ActionResult ChangeFinYear(string msg = "")
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            ViewBag.Result = msg;
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear",objCookie.getFinYear());
            return View();
        }

        [HttpPost]
        public ActionResult ChangeFinYear(FormCollection form)
        {
            string finyear = form["ddlFinYear"].ToString();
            if (objCookie.getFinYear() != finyear)
            {
                loginBLL.changeCoockieForFinancialYear(finyear);
            }
            return RedirectToAction("ChangeFinYear", new{ msg = "Financial Year Changed" });
        }

        // GET: /password/
        public ActionResult ChangePassword()
        {
            //allowed only to logged-in user or redirected 
            //from login to update profile
            if (objCookie.checkSessionState() == false && Session["userid"] == null)
            {
                return RedirectToAction("LoginUser", "Login");
            }
            if (objCookie.checkSessionState() == true)
            {
                setViewData();
            }
            else if (Session["userid"] != null)
            {
                ViewData["UserName"] = Session["svloginuser"].ToString();
                ViewData["CompName"] = "PRAG GROUP";
                ViewData["FinYear"] = DateTime.Now.Year.ToString();
                ViewData["Dept"] = "Home";
                ViewData["mnuOpt"] = "x";
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            //allowed only to logged-in user or redirected 
            //from login to update profile
            if (objCookie.checkSessionState() == false && Session["userid"] == null)
            {
                return RedirectToAction("LoginUser", "Login");
            }
            int luserid = 0;
            if (objCookie.checkSessionState() == true)
            {
                luserid = Convert.ToInt32(objCookie.getUserId());
                setViewData();
            }
            else if (Session["userid"] != null)
            {
                luserid = Convert.ToInt32(Session["userid"].ToString());
                ViewData["UserName"] = Session["svloginuser"].ToString();
                ViewData["CompName"] = "PRAG GROUP";
                ViewData["FinYear"] = DateTime.Now.Year.ToString();
                ViewData["Dept"] = "Home";
                ViewData["mnuOpt"] = "x";
            }
            string oldpassw = form["txtOldPassW"].ToString();
            string newpassw = form["txtNewPassW"].ToString();
            string confpassw = form["txtConfPassW"].ToString();
            if (oldpassw.Length == 0)
            {
                ViewBag.Result = "Current password not entered!";
                return View();
            }
            if (newpassw.Length == 0)
            {
                ViewBag.Result = "New password not entered!";
                return View();
            }
            if (confpassw.Length == 0)
            {
                ViewBag.Result = "Password not confirmed!";
                return View();
            }
            if (confpassw != newpassw)
            {
                ViewBag.Result = "Password confirmation failed!";
                return View();
            }
            LoginBLL objbll=new LoginBLL();
            objbll.updatePassword(oldpassw, newpassw, luserid);
            ViewBag.Result = objbll.Message;
            if (objbll.Result == true)
            {
                return RedirectToAction("UpdateConfirmation", "UserAdmin");
            }
            return View();
        }

        //
        #region forget password
        // GET:
        public ActionResult ForgetPassword()
        {
            return View();
        }
        //
        [HttpPost]
        public ActionResult ForgetPassword(LoginMdl modelObject, FormCollection form)
        {
            string captcha = form["txtCaptcha"].ToString();
            if (Session["Captcha"] == null || Session["Captcha"].ToString() != captcha)
            {
                ViewBag.Message = "Invalid Captcha!";
                return View();
            }
            LoginBLL objbll = new LoginBLL();
            objbll.forgetPassword(modelObject.UserName, modelObject.Department);
            ViewBag.Message = objbll.Message;    
            return View(modelObject);
        }
        //
        #endregion

        //
        #region captcha section
        //
        public ActionResult CaptchaImage1(string prefix, bool noisy = true) 
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            //old logic----------------------------------------------
            //generate new question
            //int a = rand.Next(10, 99); 
            //int b = rand.Next(0, 9); 
            //var captcha = string.Format("{0} + {1} = ?", a, b);
            //store answer 
            //Session["Captcha" + prefix] = a + b;
            //-----------------------------------------------------
            //new logic
            var captcha = mc.getCaptcha();
            Session["Captcha"] = captcha;
            //image stream 
            FileContentResult img = null;
            using (var mem = new System.IO.MemoryStream())
            using (var bmp = new System.Drawing.Bitmap(130, 30))
            using (var gfx = System.Drawing.Graphics.FromImage((System.Drawing.Image)bmp))
            {
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.FillRectangle(System.Drawing.Brushes.White, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));
                //add noise 
                //noisy = false;
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new System.Drawing.Pen(System.Drawing.Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = System.Drawing.Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));
                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);
                        float fx = x - r;
                        float fy = y - r;
                        gfx.DrawEllipse(pen, fx, fy, r, r);
                    }
                }
                //add question 
                gfx.DrawString(captcha, new System.Drawing.Font("Verdana", 15), System.Drawing.Brushes.DarkBlue, 2, 3);
                //gfx.DrawString(captcha, new Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel.Font("Tahoma", 15), System.Drawing.Brushes.Gray, 2, 3); 
                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }
            return img;
        }
        //
        public ActionResult CaptchaImage2()
        {
            var captcha = mc.getCaptcha();
            Session["Captcha"] = captcha;
            Response.Clear();
            int height = 30;
            int width = 100;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
            System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(10, 5, 0, 0);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.DrawString(Session["Captcha"].ToString(), new System.Drawing.Font("Verdana", 15, System.Drawing.FontStyle.Italic), System.Drawing.Brushes.DarkBlue, rectf);
            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red), 1, 1, width - 2, height - 2);
            g.Flush();
            Response.ContentType = "image/jpeg";
            bmp.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //g.Dispose();
            //bmp.Dispose();
            var mem = new System.IO.MemoryStream();
            bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
            FileContentResult img = null;
            img = this.File(mem.GetBuffer(), "image/Jpeg");
            return img;
        }
        //
        #endregion
        //

    }
}
