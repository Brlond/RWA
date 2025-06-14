using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Security
{
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128/8);
            string b64salt = Convert.ToBase64String(salt);
            return b64salt;
        }

        public static string GetHash(string password,string b64salt)
        {
            byte[] salt = Convert.FromBase64String(b64salt);

            byte[] hash = KeyDerivation.Pbkdf2(
                    password:password,
                    salt:salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount:100000,
                    numBytesRequested: 256/8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }
    }
}
