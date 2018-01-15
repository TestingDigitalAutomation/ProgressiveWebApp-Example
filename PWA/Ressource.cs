using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PWA
{
    public class Ressource
    {
        public string TestType;
        public long LoadStart;
        public long LoadEventEnd;
        public string Title;
        public MatchCollection Body;
        public MatchCollection Scripts;
        public MatchCollection Meta;
        public MatchCollection Link;
        public MatchCollection Img;
        public HttpWebResponse ServerResponse;
        public string Manifest;
        public string Url; // Checks s'il sont unique + access directe + cannonicale. 
        public MatchCollection allTagProp;
        public string socialMetaData;
        public string Canonical;

    }
}
