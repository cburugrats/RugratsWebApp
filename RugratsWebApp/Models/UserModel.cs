using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RugratsWebApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public long TcIdentityKey { get; set; }
        public int customerId { get; set; }
        [DataType(DataType.Password)]
        public string userPassword { get; set; }
    }
}