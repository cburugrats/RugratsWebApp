using Newtonsoft.Json;
using RugratsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RugratsWebApp.Controllers
{
    public class HgsController : Controller
    {
        // GET: Hgs
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            List<AccountModel> accounts = new List<AccountModel>();
            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                var task = client.GetAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/hgs/" + User.Identity.Name)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      //accounts = JsonConvert.DeserializeObject<List<AccountModel>>(jsonString.Result);

                  });
                task.Wait();
            }
            return View();
        }
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            List<AccountModel> accounts = new List<AccountModel>();
            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                var task = client.GetAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/account/" + User.Identity.Name)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      accounts = JsonConvert.DeserializeObject<List<AccountModel>>(jsonString.Result);

                  });
                task.Wait();
            }
            return View(accounts);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> CreateAsync(FormCollection collection)
        {
            HgsModel nHgs = new HgsModel();
            nHgs.accountNo = collection["ReceiverAccountNo"].ToString();
            nHgs.balance =Convert.ToDecimal(collection["Amount"].ToString());
            try
            {
                // TODO: Add insert logic here
                // Create a HttpClient
                using (var client = new HttpClient())
                {
                    // Create post body object
                    // Serialize C# object to Json Object
                    var serializedProduct = JsonConvert.SerializeObject(nHgs);
                    // Json object to System.Net.Http content type
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    HttpResponseMessage result = await client.PostAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/hgs/", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (int.Parse(response) > 999)
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "New HGS No = " + response;
                            return RedirectToAction("Index", "Hgs");
                        }
                        else if (response == "5")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "There's not enough money in the account.";
                            return RedirectToAction("Index", "Hgs");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred.";
                            return RedirectToAction("Index", "Hgs");
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("Index", "Hgs");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown error occurred";
                return RedirectToAction("Index", "Hgs");
            }
        }
        public ActionResult Payment(string hgsNo)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            List<AccountModel> accounts = new List<AccountModel>();
            HgsUserModel hgsUser = new HgsUserModel();
            using (var client = new HttpClient())
            {
                var task = client.GetAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/account/" + User.Identity.Name)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      accounts = JsonConvert.DeserializeObject<List<AccountModel>>(jsonString.Result);

                  });
                task.Wait();
            }
            using (var client = new HttpClient())
            {
                var task = client.GetAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/hgs/" + hgsNo)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      hgsUser = JsonConvert.DeserializeObject<HgsUserModel>(jsonString.Result);

                  });
                task.Wait();
            }

            ViewBag.hgsNo = hgsUser.HgsNo;
            ViewBag.hgsBalance = hgsUser.balance;
            return View(accounts);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> PaymentAsync(FormCollection collection)
        {
            HgsModel tMoney = new HgsModel();
            tMoney.HgsNo = int.Parse(collection["hgsNo"].ToString());
            tMoney.accountNo = collection["ReceiverAccountNo"].ToString();
            tMoney.balance = Convert.ToDecimal(collection["Amount"].ToString());
            try
            {
                // TODO: Add insert logic here
                // Create a HttpClient
                using (var client = new HttpClient())
                {
                    // Create post body object
                    // Serialize C# object to Json Object
                    var serializedProduct = JsonConvert.SerializeObject(tMoney);
                    // Json object to System.Net.Http content type
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    HttpResponseMessage result = await client.PutAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/hgs/toDepositMoney", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Money Transfer Successful";
                            return RedirectToAction("Index", "Hgs");
                        }
                        else if (response == "4")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "There's not enough money in the account.";
                            return RedirectToAction("Index", "Hgs");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred.";
                            return RedirectToAction("Index", "Hgs");
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("Index", "Hgs");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown error occurred";
                return RedirectToAction("Index", "Hgs");
            }
        }
    }
}