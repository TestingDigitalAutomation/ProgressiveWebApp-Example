using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PWA
{
    public class Fixture
    {
         

        //https://app.ft.com/metatags.json?v=3
        //https://www.datememe.com/static/js/manifest.json
        //https://babe.news/public/js/manifest.json
        //https://podle.audio/v7/static/manifest.json
        //https://m.lequipe.fr/manifest.json
        //http://www.free.fr/freebox/manifest.json

        public AndroidDriver<AndroidElement> _driver;
        public List<List<Ressource>> _PagesToTest;
        public string udid = "FA79M1A05188";
        public List<string> UrlsToTest = new List<string>
            {   // Add list of urls to check!
                // always set the first index of the home page
               "https://testingdigital.com/fr/contact",
               "https://app.ft.com/content/4c78c052-cd12-11e7-b781-794ce08b24dc?sectionid=home",
               "https://app.ft.com/content/40446aa4-e4e9-11e7-97e2-916d4fbac0da?sectionid=home",
               "https://app.ft.com/content/456875fc-f0e2-11e7-b220-857e26d1aca4?sectionid=home"
            };

        [SetUp]
        public void SetupTest()
        {
            #region Capabilitées DRIVER
            DesiredCapabilities _cap = new DesiredCapabilities();
            _cap.SetCapability(MobileCapabilityType.PlatformName, MobilePlatform.Android);
            _cap.SetCapability(MobileCapabilityType.BrowserName, MobileBrowserType.Chrome);
            _cap.SetCapability(MobileCapabilityType.NewCommandTimeout, 18000);
            _cap.SetCapability(MobileCapabilityType.Udid, udid);
            _cap.SetCapability(MobileCapabilityType.DeviceName, "Pixel 2");
            _cap.SetCapability(MobileCapabilityType.PlatformVersion, "8.1");
            _cap.SetCapability(MobileCapabilityType.NoReset, "true");
            //3G/3G Add capabilies to specify connexion type.
            _cap.SetCapability(CapabilityType.SupportsWebStorage, true);
            _cap.SetCapability(CapabilityType.SupportsApplicationCache, true);
            _cap.SetCapability(AndroidMobileCapabilityType.UnicodeKeyboard, true);
            _cap.SetCapability(AndroidMobileCapabilityType.ResetKeyboard, true);
            
            #endregion Capabilitées DRIVER
            // TestCase RECUPERRATION
            _driver = new AndroidDriver<AndroidElement>(new Uri("http://" + GetLocalIPAddress() + ":" + 4777 + "/wd/hub"), _cap);
            _PagesToTest = PagesRessource();
        }

        [TearDown]
        public void TearDown()
        {
            // Turn Off Wifi.
            AndroidSwitchWifi(udid);
            _driver.Quit();

        }

        #region PROVIDERS
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void AndroidSwitchWifi(string deviceUDID)
        {
            // --------------Try to record--------------
            Process AcceesWifi = new Process();
            Process TurnOff = new Process();
            Process TurnOffB = new Process();
            ProcessStartInfo startInfoAccess = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"adb.exe",
                Arguments = " -s " + deviceUDID + " shell am start -a android.intent.action.MAIN -n com.android.settings/.wifi.WifiSettings"
            };
            AcceesWifi.StartInfo.UseShellExecute = false;
            AcceesWifi.StartInfo = startInfoAccess;
            AcceesWifi.Start();
            Thread.Sleep(2000);
            ProcessStartInfo startInfoPreDisable = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"adb.exe",
                // for some devices use 20 insted of 19!
                Arguments = " -s " + deviceUDID + " shell input keyevent 19" 
            };
            TurnOff.StartInfo.UseShellExecute = false;
            TurnOff.StartInfo = startInfoPreDisable;
            TurnOff.Start();
            Thread.Sleep(2000);
            ProcessStartInfo startInfoDisable = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"adb.exe",
                Arguments = " -s " + deviceUDID + "  shell input keyevent 23"
            };
            TurnOffB.StartInfo.UseShellExecute = false;
            TurnOffB.StartInfo = startInfoDisable;
            TurnOffB.Start();
            Thread.Sleep(2000);
            ProcessStartInfo startInfoReturn = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = @"adb.exe",
                Arguments = " -s " + deviceUDID + "  shell input keyevent 4"
            };
            TurnOffB.StartInfo.UseShellExecute = false;
            TurnOffB.StartInfo = startInfoReturn;
            TurnOffB.Start();
            Thread.Sleep(2000);
        }

        public HttpWebResponse HttpResponse(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            response.Close();
            readStream.Close();
            return response;
        }

        public MatchCollection RegX(string Input, string Regex)
        {
            var regex = new Regex(Regex);
            return regex.Matches(Input);
        }

        public List<Ressource> SetupData(string mode, List<string> listUrl)
        {
            List<Ressource> allMetrics = new List<Ressource>();
            foreach (var url in listUrl)
            {
                _driver.Navigate().GoToUrl(url); // Set Session test destination.
                
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                #region Get online Ressource of the progressive Web App
                Ressource _Ressources = new Ressource
                { 
                    TestType = mode, //online|offline
                    LoadStart = (long)js.ExecuteScript("return window.performance.timing.navigationStart"), //load start.
                    LoadEventEnd = (long)js.ExecuteScript("return window.performance.timing.loadEventEnd"), //load end.
                    Url = _driver.Url, // current Url.
                    Title = _driver.Title, // title
                    Body = RegX(_driver.PageSource.ToString(), @"<body(.*)>(.*?\s)+(.*)<\/body>"), // getting all the body.
                    Scripts = RegX(_driver.PageSource.ToString(), @"<script(.*)>(.*?\s)+(.*)<\/script>"), // getting scripts.
                    Meta = RegX(_driver.PageSource.ToString(), @"<meta (.*?)\/>"), //  meta tags.
                    Link = RegX(_driver.PageSource.ToString(), @"<link (.*?)\/>"), // link tags.
                    Img = RegX(_driver.PageSource.ToString(), @"<img (.*?)\/>"), // img tags.
                    allTagProp = RegX(_driver.PageSource.ToString(), "(\\S+)=[\"']?((?:.(?![\"']?\\s+(?:\\S+)=|[>\"']))+.)[\"']?"),
                    ServerResponse = HttpResponse(_driver.Url), // http object with all informations of server response.
                };
                allMetrics.Add(_Ressources);
                #endregion
                // to quick check Some information of running online/offline instance.
                PrintMetrics(_Ressources, mode);
                // Testing redirect HTTP -> HTTPS
                /*
                if (_Ressources.TestType.ToLower().Equals("online"))
                {
                    var xx = UrlsToTest.First().Replace("https", "http");
                    _driver.Navigate().GoToUrl(xx);
                    string yy = _driver.Url;
                    if (yy.Contains("https"))
                    {
                        Assert.IsTrue(true, "Redirect Https Set");
                    }
                    else Assert.IsTrue(false, "Redirect Http to https not Set");
                }*/
            }
            return allMetrics;
        }

        // lets print something ! :D
        public void PrintMetrics(Ressource dataMetric, String mode)
        {
            Console.WriteLine("----=========---- " + mode + " MODE ----=========----\n");
            Console.WriteLine("Page : " + dataMetric.Url);
            Console.WriteLine("Original String : " + dataMetric.ServerResponse.ResponseUri.OriginalString);
            Console.WriteLine("Load time : " + (dataMetric.LoadEventEnd - dataMetric.LoadStart + "milliseconds"));
            Console.WriteLine("Server Status Code : " + dataMetric.ServerResponse.StatusCode);
            Console.WriteLine("Authority : " + dataMetric.ServerResponse.ResponseUri.Authority);
            Console.WriteLine("Idn Host : " + dataMetric.ServerResponse.ResponseUri.IdnHost);
            Console.WriteLine("Host name Type : " + dataMetric.ServerResponse.ResponseUri.HostNameType);
            Console.WriteLine("Scheme : " + dataMetric.ServerResponse.ResponseUri.Scheme);
            Console.WriteLine("Port : " + dataMetric.ServerResponse.ResponseUri.Port + "\n");
        }

        public List<List<Ressource>> PagesRessource()
        {
            List<List<Ressource>> _ElementsToTest = new List<List<Ressource>>();
            // getting App data online mode
            _ElementsToTest.Add(SetupData("ONLINE", UrlsToTest));
            Thread.Sleep(15000);
            // Turn Off Wifi.
            AndroidSwitchWifi(udid);
            // getting App data offline mode
            _ElementsToTest.Add(SetupData("OFFLINE", UrlsToTest));
            return _ElementsToTest;
        }
        #endregion


    }

}
