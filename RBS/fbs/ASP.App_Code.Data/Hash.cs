using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;


namespace ASP.App_Code.Data
{
    public class Hash
    {
        public static string SHA1(string src) => Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(src))).Replace("+", "-").Replace("/", "-").Replace("=", ".");

        public static string ForPage(Page page) => Hash.SHA1(page.Request.PhysicalPath);
    }
}