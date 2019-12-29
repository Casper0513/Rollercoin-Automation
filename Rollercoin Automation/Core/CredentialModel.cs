using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollercoin.API.Core
{
    public class CredentialModel
    {
        public string Email;
        public string Username;
        public string Password;

        public CredentialModel(string email, string username, string password)
        {
            Email = email;
            Username = username;
            Password = password;
        }
    }
}
