using Newtonsoft.Json;
using RugratsWebApp.Models;
using RugratsWebApp.Models.Login;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RugratsWebApp.Controllers
{
    [_SessionController]
    public class AccountController : Controller
	{
        // GET: Account
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> CreateAsync()
		{
            try
            {
                // Create a HttpClient
                using (var client = new HttpClient())
                {

                    // Create post body object
                    // Serialize C# object to Json Object
                    tcIdentityKeyModel userName = new tcIdentityKeyModel { TcIdentityKey = long.Parse(User.Identity.Name) };
                    var serializedProduct = JsonConvert.SerializeObject(userName);
                    // Json object to System.Net.Http content type
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    //HttpResponseMessage result = await client.PostAsync("https://rugratswebapi.azurewebsites.net/api/account/openAnAccount", content);
                    HttpResponseMessage result = await HttpRugartsConnettion.PostMessageAsync(content, "account/openAnAccount");
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "0")
                        {
                            return RedirectToAction("LogOff", "Login");
                        }
                        else if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Succes";
                            return RedirectToAction("List", "Account");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred";
                            return RedirectToAction("List", "Account");
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Unknown error occurred";
                        return RedirectToAction("List", "Account");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Service is not responding.";
                return RedirectToAction("List", "Account");
            }
		}
        [HttpGet]
        public ActionResult List()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            List<AccountModel> accounts = new List<AccountModel>();
            using (var client = new HttpClient())
            {
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/account/" + User.Identity.Name)
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
		public async System.Threading.Tasks.Task<ActionResult> Delete(string AccountNo)
		{
            AccountModel dModel = new AccountModel() { accountNo = AccountNo, };
            try
            {
                // TODO: Add insert logic here
                // Create a HttpClient
                using (var client = new HttpClient())
                {

                    // Create post body object
                    // Serialize C# object to Json Object
                    var serializedProduct = JsonConvert.SerializeObject(dModel);
                    // Json object to System.Net.Http content type
                    var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
                    // Post Request to the URI
                    HttpResponseMessage result = await client.PostAsync("https://rugratswebapi.azurewebsites.net/api/account/closeAccount", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Succes";
                            return RedirectToAction("List", "Account");
                        }
                        else if (response == "0")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "No such account found.";
                            return RedirectToAction("List", "Account");
                        }
                        else if (response == "2")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Could not close because there is balance on account.";
                            return RedirectToAction("List", "Account");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown Error.";
                            return RedirectToAction("List", "Account");
                        }

                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("List", "Account");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown Error";
                return RedirectToAction("List", "Account");
            }
        }
        [HttpGet]
        public ActionResult ShortCuts()
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
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/account/" + User.Identity.Name)
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
        [HttpGet]
        public ActionResult MoneyWithdraw(string AccountNo)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            AccountModel account = new AccountModel();
            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/account/getaccountbyNo/" + AccountNo)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      account = JsonConvert.DeserializeObject<AccountModel>(jsonString.Result);

                  });
                task.Wait();
            }
            return View(account);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> MoneyWithdrawAsync(FormCollection collection)
        {
            CultureInfo cultures = new CultureInfo("en-US");
            withDrawMoneyModel tMoney = new withDrawMoneyModel
            {
                accountNo = collection["accountNo"].ToString(),
                Balance = Convert.ToDecimal(collection["Balance"].ToString(), cultures)
            };
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
                    HttpResponseMessage result = await client.PostAsync("https://rugratswebapi.azurewebsites.net/api/account/withDrawMoney", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "0")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "No such account found.";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                        else if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Succes";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                        else if (response == "2")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "There is not enough money in the account";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "No such account found.";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("ShortCuts", "Account");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown Error.";
                return RedirectToAction("ShortCuts", "Account");
            }
        }
        [HttpGet]
        public ActionResult DepositMoney(string AccountNo)
        {

            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            AccountModel account = new AccountModel();
            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/account/getaccountbyNo/" + AccountNo)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      account = JsonConvert.DeserializeObject<AccountModel>(jsonString.Result);

                  });
                task.Wait();
            }
            return View(account);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> DepositMoney(FormCollection collection)
        {
            CultureInfo cultures = new CultureInfo("en-US");
            withDrawMoneyModel tMoney = new withDrawMoneyModel
            {
                accountNo = collection["accountNo"].ToString(),
                Balance = Convert.ToDecimal(collection["Balance"].ToString(), cultures)
            };
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
                    HttpResponseMessage result = await client.PostAsync("https://rugratswebapi.azurewebsites.net/api/account/toDepositMoney", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Succes";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                        else if (response == "0")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "No such account found.";
                            return RedirectToAction("ShortCuts", "Account");
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown Error.";
                            return RedirectToAction("ShortCuts", "Account");
                        }

                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("ShortCuts", "Account");
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown Error";
                return RedirectToAction("ShortCuts", "Account");
            }
        }
        [HttpGet]
        public ActionResult AccountInfo(string AccountNo)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Login");
            }
            List<MoneyTransferModel> tModel = new List<MoneyTransferModel>();
            using (var client = new HttpClient())
            {
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/MoneyTransfers/getTransferList/" + AccountNo)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      tModel = JsonConvert.DeserializeObject<List<MoneyTransferModel>>(jsonString.Result);

                  });
                task.Wait();
            }
            AccountModel account = new AccountModel();
            using (var client = new HttpClient())
            {
                var task = client.GetAsync("https://rugratswebapi.azurewebsites.net/api/account/getaccountbyNo/" + AccountNo)
                  .ContinueWith((taskwithresponse) =>
                  {
                      var response = taskwithresponse.Result;
                      var jsonString = response.Content.ReadAsStringAsync();
                      jsonString.Wait();
                      account = JsonConvert.DeserializeObject<AccountModel>(jsonString.Result);

                  });
                task.Wait();
            }
            ViewBag.AccountNo = account.accountNo;
            ViewBag.balance = Convert.ToDouble(String.Format("{0:0.00}", account.balance)); 
            ViewBag.blockageAmount = Convert.ToDouble(String.Format("{0:0.00}", account.blockageAmount));
            ViewBag.netBalance = Convert.ToDouble(String.Format("{0:0.00}", account.netBalance));
            ViewBag.openingDate = account.openingDate;
            return View(tModel);
        }
    }

}