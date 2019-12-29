using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rollercoin.API.Core;
using Rollercoin.API.Mining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Responses
{
    public struct LoginResult
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success;
        [JsonProperty(PropertyName = "msg")]
        public LoginErrorType ErrorType;

        public enum LoginErrorType
        {
            None,
            Wrong_Mail,
            Wrong_Password
        }

        public LoginResult(string serialized)
        {
            LoginResult resp = JsonConvert.DeserializeObject<LoginResult>(serialized);
            Success = resp.Success;
            ErrorType = resp.ErrorType;
        }

        public LoginResult(bool success, LoginErrorType errorType)
        {
            Success = success;
            ErrorType = errorType;
        }
    }

    public struct FetchUserSettingsResult
    {
        public bool Success;
        public PowerDistribution PowerDistribution;

        public FetchUserSettingsResult(string serialized)
        {
            JToken token = JToken.Parse(serialized);
            Success = (bool)token["success"];
            PowerDistribution = new PowerDistribution(null, null, null);
            foreach(dynamic currencyPower in token["data"])
            {
                CurrencyPower power = new CurrencyPower("FREE", 0, "");
                power.Percentage = (double)currencyPower["percent"];
                power.Id = (string)currencyPower["_id"];
                switch ((string)currencyPower["currency"])
                {
                    case "SAT":
                        power.CurrencyType = "SAT";
                        PowerDistribution.Bitcoin = power;
                        break;
                    case "DOGE_SMALL":
                        power.CurrencyType = "DOGE_SMALL";
                        PowerDistribution.Dogecoin = power;
                        break;
                    case "ETH_SMALL":
                        power.CurrencyType = "ETH_SMALL";
                        PowerDistribution.Ethereum = power;
                        break;
                }
            }
        }

        public FetchUserSettingsResult(bool success, PowerDistribution powerDistribution)
        {
            Success = success;
            PowerDistribution = powerDistribution;
        }
    }

    public struct FetchUserProfileResult
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success;
        [JsonProperty(PropertyName = "data")]
        public Profile UserProfile;

        public FetchUserProfileResult(string serialized)
        {
            FetchUserProfileResult resp = JsonConvert.DeserializeObject<FetchUserProfileResult>(serialized);
            Success = resp.Success;
            UserProfile = resp.UserProfile;
        }

        public FetchUserProfileResult(bool success, Profile userProfile)
        {
            Success = success;
            UserProfile = userProfile;
        }
    }

    public struct UpdateUserSettingsResult
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success;
        [JsonProperty(PropertyName = "error")]
        public string ErrorMessage;

        public UpdateUserSettingsResult(string serialized)
        {
            UpdateUserSettingsResult resp = JsonConvert.DeserializeObject<UpdateUserSettingsResult>(serialized);
            Success = resp.Success;
            ErrorMessage = resp.ErrorMessage;
        }

        public UpdateUserSettingsResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }

    public struct UserBannedResult
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success;
        [JsonProperty(PropertyName = "banned")]
        public bool Banned;
        [JsonProperty(PropertyName = "error")]
        public string ErrorMessage;

        public UserBannedResult(string serialized)
        {
            UserBannedResult resp = JsonConvert.DeserializeObject<UserBannedResult>(serialized);
            Success = resp.Success;
            Banned = resp.Banned;
            ErrorMessage = resp.ErrorMessage;
        }

        public UserBannedResult(bool success, bool banned, string errorMessage)
        {
            Success = success;
            Banned = banned;
            ErrorMessage = errorMessage;
        }
    }
}
