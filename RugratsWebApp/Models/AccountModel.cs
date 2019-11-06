using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RugratsWebApp.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public int customerId { get; set; }
        public string accountNo { get; set; }
        public decimal balance { get; set; }
        public decimal blockageAmount { get; set; }
        public decimal netBalance { get; set; }
        public DateTime? openingDate { get; set; }
        public DateTime? lastTransactionDate { get; set; }
        public bool status { get; set; }
    }
}