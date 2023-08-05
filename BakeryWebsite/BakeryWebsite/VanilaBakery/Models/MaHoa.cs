using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace VanilaBakery.Models
{
    public class MaHoa
    {
        public static string EncryptMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bHash = System.Text.Encoding.UTF8.GetBytes(str);
            bHash = md5.ComputeHash(bHash);

            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {

                sbHash.Append(b.ToString("x").ToLower());

            }
            return sbHash.ToString();

        }
    }
}