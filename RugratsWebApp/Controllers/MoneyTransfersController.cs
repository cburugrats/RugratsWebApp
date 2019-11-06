﻿using Newtonsoft.Json;
using RugratsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RugratsWebApp.Controllers
{
    public class MoneyTransfersController : Controller
    {
        // GET: MoneyTransfers
        public ActionResult SenderToOwnAccounts()
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
        public ActionResult ToOwnAccounts(string AccountNo, string NetBalance)
        {
            ViewBag.AccountNo = AccountNo;
            ViewBag.netBlance = NetBalance;
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
        public async System.Threading.Tasks.Task<ActionResult> ToOwnAccountsAsync(FormCollection collection)
        {
            CultureInfo cultures = new CultureInfo("en-US");
            MoneyTransferModel tMoney = new MoneyTransferModel();
            tMoney.senderAccountNo = collection["SenderAccountNo"].ToString();
            tMoney.receiverAccountNo = collection["ReceiverAccountNo"].ToString();
            tMoney.amount = Convert.ToDecimal(collection["Amount"].ToString(), cultures);
            tMoney.statement = collection["description"].ToString();
            tMoney.realizationTime = DateTime.Now;
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
                    HttpResponseMessage result = await client.PostAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/MoneyTransfers/virman", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                        if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Money Transfer Successful";
                            return RedirectToAction("ToOwnAccounts", "MoneyTransfers",tMoney);
                        }
                        else if (response == "4")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "There's not enough money in the account.";
                            return RedirectToAction("ToOwnAccounts", "MoneyTransfers", tMoney);
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred.";
                            return RedirectToAction("ToOwnAccounts", "MoneyTransfers", tMoney);
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("ToOwnAccounts", "MoneyTransfers", tMoney);
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown error occurred";
                return RedirectToAction("ToOwnAccounts", "MoneyTransfers", tMoney);
            }
        }
        public ActionResult SenderToOtherAccounts()
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
        public ActionResult ToOtherAccounts(string AccountNo,string NetBalance)
        {
            ViewBag.AccountNo = AccountNo;
            ViewBag.netBlance = NetBalance;
            return View(); 
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ToOtherAccountsAsync(FormCollection collection)
        {
            CultureInfo cultures = new CultureInfo("en-US");
            MoneyTransferModel tMoney = new MoneyTransferModel();
            tMoney.senderAccountNo = collection["SenderAccountNo"].ToString();
            tMoney.receiverAccountNo = collection["ReceiverAccountNo"].ToString();
            tMoney.amount = Convert.ToDecimal(collection["Amount"].ToString(), cultures);
            tMoney.statement = collection["description"].ToString();
            tMoney.realizationTime = DateTime.Now;
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
                    HttpResponseMessage result = await client.PostAsync("https://bankappcorewebapirugrats.azurewebsites.net/api/MoneyTransfers/havale", content);
                    // Check for result
                    if (result.IsSuccessStatusCode)
                    {
                        result.EnsureSuccessStatusCode();
                        string response = await result.Content.ReadAsStringAsync();
                         if (response == "1")
                        {
                            TempData["status"] = 1;
                            TempData["StatusDescription"] = "Money Transfer Successful";
                            return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
                        }
                        else if (response=="4")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "There's not enough money in the account.";
                            return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
                        }
                        else if(response=="3")
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Recipient account not found.";
                            return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
                        }
                        else
                        {
                            TempData["status"] = 0;
                            TempData["StatusDescription"] = "Unknown error occurred.";
                            return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
                        }
                    }
                    else
                    {
                        TempData["status"] = 0;
                        TempData["StatusDescription"] = "Service is not responding.";
                        return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
                    }
                }
            }
            catch
            {
                TempData["status"] = 0;
                TempData["StatusDescription"] = "Unknown error occurred";
                return RedirectToAction("SenderToOtherAccounts", "MoneyTransfers", tMoney);
            }
        }

    }
}