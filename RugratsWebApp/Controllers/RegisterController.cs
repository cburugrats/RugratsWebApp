using Newtonsoft.Json;
using RugratsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RugratsWebApp.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        // GET: Register
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
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(FormCollection collection)
        {
            
            try
            {
                Regex digitsOnly = new Regex(@"^[0-9]{5}$");
                bool a = digitsOnly.IsMatch(collection["phoneNumber"].ToString());
                RegisterModel nRegister = new RegisterModel();
                nRegister.TcIdentityKey = Int64.Parse(collection["TcIdentityKey"].ToString());
                nRegister.userName = collection["TcIdentityKey"].ToString();
                nRegister.surname = collection["surname"].ToString();
                nRegister.firstname = collection["firstname"].ToString();
                nRegister.phoneNumber = Int64.Parse(digitsOnly.Replace(collection["phoneNumber"].ToString(), ""));
                nRegister.userPassword = collection["userPassword"].ToString();
                nRegister.eMail = collection["eMail"].ToString();
                nRegister.dateOfBirth = Convert.ToDateTime(collection["dateOfBirth"].ToString());
                // TODO: Add insert logic here
                // Create a HttpClient
                using (var client = new HttpClient())
                {

                    // Create post body object
                    // Serialize C# object to Json Object
                    var serializedProduct = JsonConvert.SerializeObject(nRegister);
                    // Json object to System.Net.Http content type
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    HttpResponseMessage result = await HttpRugartsConnettion.PostMessageAsync(content, "register");
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response =="2")
                        {
                            //Aynı Mail Mevcut
                            ViewBag.RegisterResponse = "Sign up with the same mail. Please try a different email address.";
                            return View("Index");
                        }
                        else if (response=="3")
                        {
                            //Aynı TC Mevcut
                            ViewBag.RegisterResponse = "Registered with the same TC. please try a different TC.";
                            return View("Index");
                        }
                        else if (response=="0")
                        {
                            //Bilinmeyen Hata
                            ViewBag.RegisterResponse = "Unknown error occurred";
                            return View("Index");
                        }
                    }
                    else
                    {
                        ViewBag.RegisterResponse = "Services is not response";
                        return View("Index");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.RegisterResponse = "You Have Entered Missing Information";
                return View("Index");
            }
        }
    }
}