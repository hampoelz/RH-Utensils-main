using System;
using System.Net;

namespace Main.Wpf.Functions
{
    public static class InternetChecker
    {
        public static bool Check(string url = "https://google.com/")
        {
            var urlCheck = new Uri(url);
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead(urlCheck))
                {
                    return true;
                }
            }
            catch
            {
                LogFile.WriteLog("No Internet connection!");
                return false;
            }
        }
    }
}