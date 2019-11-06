using Newtonsoft.Json;
using RugratsWebApp.Models;
using RugratsWebApp.Models.Login;
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
                    // TODO: Add insert logic here
                    // Create a HttpClient
                    using (var client = new HttpClient())
                    {

                        // Create post body object
                        // Serialize C# object to Json Object
                        var serializedProduct = JsonConvert.SerializeObject(collection);
                        // Json object to System.Net.Http content type
                        var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                        // Post Request to the URI
                        HttpResponseMessage result = await client.PostAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/login", content);
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
                    }
                }
                catch
                {
                    return View();
                }

                //Aşağıdaki if komutu gönderilen mail ve şifre doğrultusunda kullanıcı kontrolu yapar. Eğer kullanıcı var ise login olur.
                if (collection.TcIdentityKey == "1" && collection.userPassword == "1")
                {
                    FormsAuthentication.SetAuthCookie(collection.TcIdentityKey, true);
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", "EMail veya şifre hatalı!");
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