using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VanilaBakery.VNPayCofig
{
    public class VNPayConfig
    {
        public static string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public static string vnp_Returnurl = "http://localhost:5315/GioHang/VNPAYRETURN";
        public static string vnp_TmnCode = "YNC8NDJZ";
        public static string vnp_HashSecret = "FURSQEORGNCDMRURMEZAIOGWKKUZKFTW";
        public static string querydr = "http://sandbox.vnpayment.vn/merchant_webapi/merchant.html";
    }
  
}