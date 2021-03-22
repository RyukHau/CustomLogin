using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomLogin.Areas.Account.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CustomLogin.Areas.Account.Controllers
{
    // claim area
    [Area("Account")]
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
        // get appsetting
        private readonly IConfiguration config;

        public LoginController(IConfiguration config)
        {
            this.config = config;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel login)
        {
            // check model
            if (ModelState.IsValid)
            {

                // check account
                if (((login.LoginAccount == "test") && (login.LoginPassword == "test")) == false)
                {
                    ViewBag.errMsg = "Login Fail";

                    return View();
                }
                else
                {
                    // get login account
                    Claim[] claims = new[] { new Claim("Account", login.LoginAccount) };

                    // scheme
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    // Login
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);


                    // get over setting value
                    double loginExpireMinute = this.config.GetValue<double>("LoginExpireMinute");

                    // IsPersistent = false，web will close and logout
                    // if web over setting will logout
                    /* ExpiresUtc = DateTime.Now.AddMinutes(loginExpireMinute) */
                     await HttpContext.SignInAsync(principal, new AuthenticationProperties() { IsPersistent = false, });

                    return Redirect("/Home/Index");
                }
            }
            else
            {
                ViewData["errMsg"] = "Please Input";

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            //logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Privacy");
        }
    }
}
