using Newtonsoft.Json;
using RugratsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RugratsWebApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (String.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                FormsAuthentication.SignOut();
                return View();
            }
            return Redirect("/Home");
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Index(LoginModel collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create post body object
                    // Serialize C# object to Json Object
                    var serializedProduct = JsonConvert.SerializeObject(collection);
                    // Json object to System.Net.Http content type
                    StringContent content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    HttpResponseMessage result = await HttpRugartsConnettion.PostMessageAsync(content, "login"); 
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            FormsAuthentication.SetAuthCookie(collection.TcIdentityKey, true);
                            return RedirectToAction("Index", "Home");
                        }
                        else if (response == "0")
                        {
                            //Bilinmeyen Hata
                            ViewBag.LoginResponse = "You entered incorrect password or TC";
                            return View("Index");
                        }
                        else
                        {
                            ViewBag.LoginResponse = "Unknown error occurred";
                            return View("Index");
                        }
                    }
                    else
                    {
                        ViewBag.LoginResponse = "Service is not response";
                        return View("Index");
                    }

                }
                catch
                {
                    return View();
                }

            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Login");
        }
    }
}