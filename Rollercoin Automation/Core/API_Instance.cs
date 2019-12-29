using Newtonsoft.Json.Linq;
using Rollercoin.API.Mining;
using Rollercoin.API.Responses;
using Rollercoin.API.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rollercoin.API.Core
{
    public class API_Instance
    {
        public API_WebClient API_WebClient;
        public bool UserAuthenticated;
        public CredentialModel UserCredentials;
        public API_Instance()
        {
            API_WebClient = new API_WebClient();
            UserAuthenticated = false;
            UserCredentials = null;
        }
        public bool GetSignInCookies()
        {
            HttpWebResponse response = API_WebClient.MakeRequest(new Uri("https://rollercoin.com/sign-in"), true);
            if (response.StatusCode == HttpStatusCode.OK) return true;
            return false;
        }
        public LoginResult Login(CredentialModel credentials)
        {
            GetSignInCookies();

            HttpWebRequest request = API_WebClient.GetRequest(new Uri("https://rollercoin.com/api/login")) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["x-csrf-token"] = API_WebClient.Cookies.Cookies["csrf"].Value;
            request.Headers["cookie"] = $"{API_WebClient.Cookies.Cookies["__cfduid"].Serialize()}; {API_WebClient.Cookies.Cookies["fbsid"].Serialize()}";

            JObject form_obj = new JObject();
            form_obj.Add("mail", credentials.Email);
            form_obj.Add("password", credentials.Password);
            form_obj.Add("keepSigned", true);
            form_obj.Add("isLoading", false);

            byte[] formData_bytes = Encoding.ASCII.GetBytes(form_obj.ToString());
            using (var stream = request.GetRequestStream())
                stream.Write(formData_bytes, 0, formData_bytes.Length);
            HttpWebResponse response;
            try
            {
                response = API_WebClient.GetResponse(request) as HttpWebResponse;
            }
            catch(WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            LoginResult respObject = new LoginResult(new StreamReader(response.GetResponseStream()).ReadToEnd());
            if(respObject.Success)
            {
                UserCredentials = credentials;
                UserAuthenticated = true;
                return respObject;
            }

            return respObject;
        }
        public UserBannedResult CheckUserBanned()
        {
            if (!UserAuthenticated) return new UserBannedResult(false, false, "access_denied");

            HttpWebRequest request = API_WebClient.GetRequest(new Uri("https://rollercoin.com/api/check-user-banned")) as HttpWebRequest;
            request.Method = "GET";
            request.Headers["x-csrf-token"] = API_WebClient.Cookies.Cookies["csrf"].Value;
            request.Headers["cookie"] = $"{API_WebClient.Cookies.Cookies["__cfduid"].Serialize()}; {API_WebClient.Cookies.Cookies["fbsid"].Serialize()}";

            HttpWebResponse response;
            try
            {
                response = API_WebClient.GetResponse(request) as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            UserBannedResult respObject = new UserBannedResult(new StreamReader(response.GetResponseStream()).ReadToEnd());
            return respObject;
        }
        public FetchUserSettingsResult FetchUserSettings()
        {
            if (!UserAuthenticated) return new FetchUserSettingsResult(false, null);

            HttpWebRequest request = API_WebClient.GetRequest(new Uri("https://rollercoin.com/api/mining/user-settings")) as HttpWebRequest;
            request.Method = "GET";
            request.Headers["x-csrf-token"] = API_WebClient.Cookies.Cookies["csrf"].Value;
            request.Headers["cookie"] = $"{API_WebClient.Cookies.Cookies["__cfduid"].Serialize()}; {API_WebClient.Cookies.Cookies["fbsid"].Serialize()}";

            HttpWebResponse response;
            try
            {
                response = API_WebClient.GetResponse(request) as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            FetchUserSettingsResult respObject = new FetchUserSettingsResult(new StreamReader(response.GetResponseStream()).ReadToEnd());
            return respObject;
        }
        public FetchUserProfileResult FetchUserProfileData(string userCode)
        {
            HttpWebRequest request = API_WebClient.GetRequest(new Uri($"https://rollercoin.com/api/profile/user-profile-data/{userCode}")) as HttpWebRequest;
            request.Method = "GET";

            HttpWebResponse response;
            try
            {
                response = API_WebClient.GetResponse(request) as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            FetchUserProfileResult respObject = new FetchUserProfileResult(new StreamReader(response.GetResponseStream()).ReadToEnd());
            return respObject;
        }
        public UpdateUserSettingsResult UpdateUserSettings(PowerDistribution powerDistribution)
        {
            if (!UserAuthenticated) return new UpdateUserSettingsResult(false, "access_denied");
            HttpWebRequest request = API_WebClient.GetRequest(new Uri("https://rollercoin.com/api/mining/update-settings")) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["x-csrf-token"] = API_WebClient.Cookies.Cookies["csrf"].Value;
            request.Headers["cookie"] = $"{API_WebClient.Cookies.Cookies["__cfduid"].Serialize()}; {API_WebClient.Cookies.Cookies["fbsid"].Serialize()}";

            JObject form_obj = new JObject();
            JArray array = new JArray();
            array.Add(JToken.FromObject(powerDistribution.Bitcoin));
            array.Add(JToken.FromObject(powerDistribution.Dogecoin));
            array.Add(JToken.FromObject(powerDistribution.Ethereum));
            form_obj.Add("settings", array);

            byte[] formData_bytes = Encoding.ASCII.GetBytes(form_obj.ToString());
            using (var stream = request.GetRequestStream())
                stream.Write(formData_bytes, 0, formData_bytes.Length);
            HttpWebResponse response;
            try
            {
                response = API_WebClient.GetResponse(request) as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            UpdateUserSettingsResult respObject = new UpdateUserSettingsResult(new StreamReader(response.GetResponseStream()).ReadToEnd());
            return respObject;
        }
    }
}
