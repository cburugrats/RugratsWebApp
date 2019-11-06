using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RugratsWebApp.Models
{
    public class LoginModel
    {
        public string TcIdentityKey { get; set; }
        [DataType(DataType.Password)]
        public string userPassword { get; set; }
    }
}