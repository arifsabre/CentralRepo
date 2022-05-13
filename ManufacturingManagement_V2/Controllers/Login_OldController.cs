using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManufacturingManagement_V2.Models;

namespace ManufacturingManagement_V2.Controllers
{
    public class Login_OldController : Controller
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
            if (loginopt == "offline-admin")
            {
                //just for admin in debug mode----------------------
                loginBLL.performLogin("1", "2", getRecentFinYear(), "123",false,"Home");
                return RedirectToAction("Index", "Home");
                //--------------------------------------------------
            }
            //release mode--------------------------------------
            //string captcha = form["txtCaptcha"].ToString();
            //if (Session["Captcha"] == null || Session["Captcha"].ToString() != captcha)
            //{
            //    //ModelState.AddModelError("Captcha", "Wrong value of sum, please try again.");
            //    ViewBag.Result = "Invalid Captcha!";
            //    return View();
            //}
            LoginMdl objlogin = new LoginMdl();
            //objlogin.UserName = form["txtUserId"].ToString();
            //objlogin.PassW = form["txtPassword"].ToString();
            objlogin.UserName = form["UserName"].ToString();
            objlogin.PassW = form["PassW"].ToString();
            loginBLL.getUserLoginInfo(objlogin);
            if (loginBLL.Result == true)
            {
                //set user info by objlogin object to use further untill cookie created
                Session["userid"] = objlogin.UserId;
                string sd = "";
                if (loginopt == "online-otp")
                {
                    string otp = mc.getOTP();
                    Session["svotp"] = otp;
                    string msg = otp + " is your PRAG ERP OTP.";
                    sd = mc.sendOTPtoEmail(msg, objlogin.EMail);
                }
                else if (loginopt == "offline")
                {
                    Session["svotp"] = "123";
                    sd = "1";
                }
                if (sd == "1")
                {
                    return RedirectToAction("PerformLogin");//otp and comp-finyear selection
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

        // GET: /otp-comp-finyear/
        public ActionResult PerformLogin()
        {
            if (Session["userid"] == null) { return RedirectToAction("LoginUser", "Login"); };
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(Session["userid"].ToString())), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(),"finyear","finyear",getRecentFinYear());
            return View();
        }

        [HttpPost]
        public ActionResult PerformLogin(FormCollection form)
        {
            string compcode = form["ddlCompany"].ToString();
            string finyear = form["ddlFinYear"].ToString();
            string otp = "123";//form["txtOTP"].ToString();//note
            //if (Session["svotp"].ToString() == otp && Session["userid"] != null)//note
            if (Session["userid"] != null)
            {
                loginBLL.performLogin(Session["userid"].ToString(), compcode, finyear, otp, false, "Home");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CompanyList = new SelectList(compBLL.getCompanyListByUser(Convert.ToInt32(Session["userid"].ToString())), "compcode", "cmpname");
            ViewBag.FinYearList = new SelectList(compBLL.getFinancialYear(), "finyear", "finyear");
            ViewBag.Result = "Invalid Attempt!";
            return View();
        }

        // GET: /company/
        public ActionResult ChangeCompany()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
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
            return RedirectToAction("Index", "Home");
        }

        // GET: /finyear/
        public ActionResult ChangeFinYear()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
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
            return RedirectToAction("Index", "Home");
        }

        // GET: /password/
        public ActionResult ChangePassword()
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            if (objCookie.checkSessionState() == false) { return RedirectToAction("LoginUser", "Login"); };
            setViewData();
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
            int luserid = Convert.ToInt32(objCookie.getUserId());
            objbll.updatePassword(oldpassw, newpassw, luserid);
            if (objbll.Result == false)
            {
                ViewBag.Result = "Invalid current password!";
                return View();
            }
            return RedirectToAction("Index", "Home");
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
        public ActionResult ForgetPassword(LoginMdl modelObject)
        {
            LoginBLL objbll = new LoginBLL();
            objbll.forgetPassword(modelObject.UserName,modelObject.Department);
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
