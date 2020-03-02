using RugratsWebApp.Models.Login;
using RugratsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace RugratsWebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            UpdateUserModel uUser = new UpdateUserModel();
            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/User/getUpadateUser/" + User.Identity.Name)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      uUser = JsonConvert.DeserializeObject<UpdateUserModel>(jsonString.Result);

                  });
                task.Wait();
            }
            return View(uUser);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(UpdateUserModel collection)
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
                    HttpResponseMessage result = await client.PutAsync("https://rugratswebapi.azurewebsites.net/api/User/updateUser", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Money Transfer Successful";
                            return RedirectToAction("Index", "Profile");
                        }
                        else if (response == "2")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Error saving!";
                            return RedirectToAction("Index", "Profile");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred.";
                            return RedirectToAction("Index", "Profile");
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("Index", "Profile");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown error occurred";
                return RedirectToAction("Index", "Profile");
            }
            

        }
    }
}