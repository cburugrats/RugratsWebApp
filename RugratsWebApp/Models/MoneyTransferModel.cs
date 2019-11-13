using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RugratsWebApp.Models
{
    public class MoneyTransferModel
    {
        public string receiverAccountNo { get; set; }
        public string senderAccountNo { get; set; }
        public decimal amount { get; set; }
        public DateTime realizationTime { get; set; }
        public string statement { get; set; }
        public string transferType { get; set; }
		public DateTime? createdDate { get; set; }

    }
}