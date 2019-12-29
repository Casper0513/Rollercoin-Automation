using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Mining
{
    [JsonObject]
    public class CurrencyPower
    {
        [JsonProperty(PropertyName = "currency")]
        public string CurrencyType;
        [JsonProperty(PropertyName = "percent")]
        public double Percentage;
        [JsonProperty(PropertyName = "_id")]
        public string Id;

        public CurrencyPower(string serialized)
        {
            CurrencyPower currencyPower = JsonConvert.DeserializeObject<CurrencyPower>(serialized);
            CurrencyType = currencyPower.CurrencyType;
            Percentage = currencyPower.Percentage;
            Id = currencyPower.Id;
        }

        public CurrencyPower(string type, double percentage, string id)
        {
            CurrencyType = type;
            Percentage = percentage;
            Id = id;
        }
    }

    public class PowerDistribution
    {
        public CurrencyPower Bitcoin;
        public CurrencyPower Dogecoin;
        public CurrencyPower Ethereum;

        public PowerDistribution(CurrencyPower bitcoin, CurrencyPower dogecoin, CurrencyPower ethereum)
        {
            Bitcoin = bitcoin;
            Dogecoin = dogecoin;
            Ethereum = ethereum;
        }
    }
}
