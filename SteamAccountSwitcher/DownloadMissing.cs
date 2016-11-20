using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher
{
    class DownloadMissing
    {
        static readonly WebClient _client = new WebClient();

        public static bool CheckIfMissing()
        {
            return !File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\main.ahk");
            
        }
        public static void Download()
        {
            _client.DownloadFile("https://raw.githubusercontent.com/judge2020/Battle.net-Switcher/gh-pages/bin/latest/main.ahk", AppDomain.CurrentDomain.BaseDirectory + @"\main.ahk");
        }
    }
}
