using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rollercoin.API.Web
{
    public class API_WebClient : WebClient
    {
        public CookieCollection Cookies = new CookieCollection();
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            if (webRequest is HttpWebRequest)
            {
                (webRequest as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";
                //(webRequest as HttpWebRequest).CookieContainer = this.Cookies;
                (webRequest as HttpWebRequest).AllowAutoRedirect = false;
            }
            return webRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse webResponse = base.GetWebResponse(request);
            if (webResponse is HttpWebResponse)
            {
                foreach (string cookieHeader in webResponse.Headers.GetValues("set-cookie"))
                    AddCookieFromString(cookieHeader);
            }
            return webResponse;
        }

        public WebRequest GetRequest(Uri address)
        {
            return this.GetWebRequest(address);
        }

        public WebResponse GetResponse(WebRequest request)
        {
            return this.GetWebResponse(request);
        }

        public HttpWebResponse MakeRequest(Uri address, bool followRedirects)
        {
            HttpWebRequest request = this.GetWebRequest(address) as HttpWebRequest;
            HttpWebResponse httpWebResponse = this.GetWebResponse(request) as HttpWebResponse;
            Console.WriteLine(httpWebResponse.StatusCode);
            if (httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently && followRedirects)
            {
                Uri uri = new Uri("https://" + address.Host + httpWebResponse.Headers.Get("Location"));
                Console.WriteLine(uri);
                httpWebResponse.Close();
                httpWebResponse.Dispose();
                return this.MakeRequest(uri, true);
            }
            return httpWebResponse;
        }

        public string DownloadString(Uri address, bool followRedirects)
        {
            HttpWebRequest request = this.GetWebRequest(address) as HttpWebRequest;
            HttpWebResponse httpWebResponse = this.GetWebResponse(request) as HttpWebResponse;
            Console.WriteLine(httpWebResponse.StatusCode);
            if (httpWebResponse.StatusCode == HttpStatusCode.MovedPermanently && followRedirects)
            {
                Uri uri = new Uri("https://" + address.Host + httpWebResponse.Headers.Get("Location"));
                Console.WriteLine(uri);
                httpWebResponse.Close();
                httpWebResponse.Dispose();
                return this.DownloadString(uri, true);
            }
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader;
                if (httpWebResponse.CharacterSet == null)
                {
                    streamReader = new StreamReader(responseStream);
                }
                else
                {
                    streamReader = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet));
                }
                string result = streamReader.ReadToEnd();
                responseStream.Close();
                streamReader.Close();
                httpWebResponse.Close();
                httpWebResponse.Dispose();
                return result;
            }
            httpWebResponse.Close();
            httpWebResponse.Dispose();
            return "";
        }

        public HtmlDocument DownloadDocument(Uri address, bool followRedirects)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(this.DownloadString(address, followRedirects));
            return htmlDocument;
        }

        public void AddCookieFromString(string cookieString)
        {
            string cookie = cookieString.Split(';')[0];
            string[] cookieData = cookie.Split('=');
            Cookies.Add(new Cookie(cookieData[0].Trim(), HttpUtility.UrlDecode(cookieData[1].Trim())));
        }

        public string SerializeCookies()
        {
            List<string> serializedCookies = new List<string>();
            foreach (Cookie c in Cookies.GetValuesEnumerable())
                serializedCookies.Add(c.Serialize());
            string cookieString = string.Join("; ", serializedCookies);
            return cookieString;
        }
    }

    public class CookieCollection
    {
        public Dictionary<string, Cookie> Cookies;

        public CookieCollection()
        {
            Cookies = new Dictionary<string, Cookie>();
        }

        public void Add(Cookie cookie)
        {
            if (Cookies.ContainsKey(cookie.Name))
            {
                Cookies[cookie.Name] = cookie;
                return;
            }

            Cookies.Add(cookie.Name, cookie);
        }

        public bool Remove(string name)
        {
            if (!Cookies.ContainsKey(name)) return false;
            Cookies.Remove(name);
            return true;
        }

        public bool TryGetCookie(string name, out Cookie cookie)
        {
            return Cookies.TryGetValue(name, out cookie);
        }

        public IEnumerable<Cookie> GetValuesEnumerable()
        {
            List<Cookie> cookies = new List<Cookie>();
            foreach (KeyValuePair<string, Cookie> ckvp in Cookies)
                cookies.Add(ckvp.Value);
            return cookies;
        }
    }

    public class Cookie
    {
        public string Name;
        public string Value;

        public Cookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Serialize()
        {
            return $"{Name}={Value}";
        }
    }
}
