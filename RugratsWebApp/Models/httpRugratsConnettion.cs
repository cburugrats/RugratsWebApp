using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RugratsWebApp.Models
{
    static public class HttpRugartsConnettion
    {
        static public async System.Threading.Tasks.Task<HttpResponseMessage> PostMessageAsync(StringContent content, string apiRoute)
        {
            // TODO: Add insert logic here
            // Create a HttpClient
            var client = new HttpClient();
            return await client.PostAsync("https://rugratswebapi.azurewebsites.net/api/" + apiRoute, content);
        } 

    }
}