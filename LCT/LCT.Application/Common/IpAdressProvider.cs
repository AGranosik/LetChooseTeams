using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Application.Common
{
    public static class IpAdressProvider
    {
        public static string GetHostAdress()
            => Dns.GetHostByName(String.Empty).AddressList.ElementAt(3).ToString();
    }
}
