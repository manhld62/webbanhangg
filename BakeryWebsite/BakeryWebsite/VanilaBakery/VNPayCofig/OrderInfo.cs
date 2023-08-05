using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanilaBakery.VNPayCofig
{
    public class OrderInfo
    {
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string OrderDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public long PaymentTranId { get; set; }
        public string BankCode { get; set; }
        public string PayStatus { get; set; }
    }
}