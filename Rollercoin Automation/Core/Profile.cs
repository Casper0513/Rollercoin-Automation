using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Core
{
    public class Profile
    {
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "gender")]
        public UserGender Gender;
        [JsonProperty(PropertyName = "registration")]
        public DateTime Registration;
        [JsonProperty(PropertyName = "id")]
        public string UserId;

        public Profile(string name, UserGender gender, DateTime registration, string userId)
        {
            Name = name;
            Gender = gender;
            Registration = registration;
            UserId = userId;
        }
    }

    public enum UserGender
    {
        Male,
        Female
    }
}
