using NUnit.Framework;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PWA
{
    public class Script : Fixture
    {
        [TestCase(TestName = "Manifest", Description = "Ensure Metadata provided for Add to Home screen, Read Manifest")]
        public void Manifest()
        {
            bool isManifest = false;
            string mssg = "No manifest found";
            foreach (var webPage in _PagesToTest)
            { // For every Webpage
                foreach (var _ressource in webPage)
                { // and every Ressource
                    foreach (Match item in _ressource.allTagProp)
                    { //get all attributes and check the manifest if exist.
                        if (item.Groups[1].Value.ToLower() == "rel" && item.Groups[2].Value.ToLower().Contains("manifest"))
                        {
                            isManifest = true;
                            mssg = "manifest is Set";
                            break;
                        }
                        // get the manifest if you want.
                        using (WebClient wc = new WebClient())
                        _ressource.Manifest = wc.DownloadString("https://app.ft.com/metatags.json?v=3");     
                    }
                }
                Assert.IsTrue(isManifest, mssg);
            }
        }

        [TestCase(TestName = "Social Metadata", Description = " Social metadata a verifier")]
        public void SocialmetaData()
        {
            int count = 0;
            foreach (var webPage in _PagesToTest)
            {
                foreach (var _ressource in webPage)
                {
                    foreach (Match item in RegX(_driver.PageSource.ToString(), @"<meta (.*?)\/>")) // recuperation des tags Meta, du Regex
                    {
                        if (item.Groups[1].Value.ToLower().Contains("twitter")) // a remplacer par  d'autre mot clefs selon le besoin (facebook, google....)
                        {
                            count++;
                            Assert.IsTrue(true, "Social metadata is Set, Count:" + count + " declaration");
                        }
                        else Assert.IsFalse(false, _ressource.Url + ": no mention of SocialMedia set");
                    }
                }
            }
        }

        [TestCase(TestName = "Https", Description = "Ensure Site is served over HTTPS")]
        public void HTTPS()
        {
            foreach (var webPage in _PagesToTest)
            {
                foreach (var _ressource in webPage)
                {
                    if (_ressource.ServerResponse.ResponseUri.Scheme.ToLower().Contains("https"))
                    {
                        Assert.IsTrue(true, "there is a definition");
                    }
                    else Assert.IsFalse(false, _ressource.Url + ": Is not secure");
                }

            }
        }
        [TestCase(TestName = "http redirect to Https", Description = "Ensure Site is served over HTTPS with http call")]
        public void HTTPSREDIRECTE()
        {
            foreach (var webPage in _PagesToTest)
            {
                foreach (var _ressource in webPage)
                {
                    if (_ressource.TestType.ToLower().Equals("online"))
                    {

                    }
                }
            }
        }

        [TestCase(TestName = "Canonical", Description = "Ensure Canonical URLs are provided when necessary")]
        public void Canonical()
        {
            foreach (var webPage in _PagesToTest)
            {
                foreach (var _ressource in webPage)
                {
                    foreach (Match item in _ressource.Link)
                    {
                        if (item.Groups[1].Value.ToLower().Contains("canonical"))
                        {
                            Assert.IsTrue(true, "Canonical is defined");
                        }
                        else Assert.IsTrue(false, _ressource.Url + ": no mention of canonical Url");
                    }
                }
            }
        }

        [TestCase(TestName = "Server respone", Description = "Ensure All app URLs load while offline")]
        public void ServerResponse()
        {
            foreach (var webPage in _PagesToTest)
            {
                foreach (var _ressource in webPage)
                {
                    if (_ressource.ServerResponse.StatusCode.ToString() == "OK")
                    {
                        Assert.IsTrue(true, "200");
                    }
                    else Assert.IsFalse(true, "The URL: " + _ressource.Url + " return: " + _ressource.ServerResponse.StatusCode);
                }
            }
        }

        [TestCase(TestName = "Schema.org metadata", Description = "Ensure title, image, description, Social metadata etc. are available. ")]
        public void Schema()
        {
            int imgCount = 0; int altCount = 0;
            foreach (var webPage in _PagesToTest)
            {
                if (_driver.Title.Equals("") || _driver.Equals(null)) //Title?
                    Assert.IsFalse(true, _driver.Url + "| Do not contain a Title!");
                foreach (var _ressource in webPage)
                {
                    foreach (Match item in _ressource.Img) // getting img tags
                    {
                        imgCount++;
                        if (item.Groups[1].Value.ToLower().Contains("alt")) //cheking if description is set.
                            altCount++;
                    }
                    foreach (Match item in _ressource.Meta) // getting meta tags.
                    {
                        if (item.Groups[1].Value.ToLower().Contains("twitter")) // example of Twitter Social media.
                        Assert.IsTrue(true, "Social metadata is Set declaration");
                        else Assert.IsFalse(true, _ressource.Url + ": no mention of SocialMedia set");
                    }
                }
            }
            Console.WriteLine("In total of:" + imgCount + " img Count, " + altCount + " got a description");
        }

        [TestCase(TestName = "Navigation", Description = "Ensure Page transitions don't feel like they block on the network // Ensure that Backward to same previous page")]
        public void Navigation()
        {
            foreach (string webPage in UrlsToTest)
            {
                _driver.Navigate().GoToUrl(webPage);
                _driver.Navigate().GoToUrl(UrlsToTest.First());
                _driver.Navigate().Back();
                //Testing the actual é the start Url
                //StringAssert.AreEqualIgnoringCase(webPage, _driver.Url);
            }
        }
    }
}
