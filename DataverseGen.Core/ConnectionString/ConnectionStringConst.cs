using System;
using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString
{
    internal class ConnectionStringConst
    {
        public static HashSet<string> Auth = new HashSet<string>() { "AuthenticationType", "AuthType" };

        public static string GetToken(IDictionary<string, string> tokens, HashSet<string> tokenNames)
        {
            foreach (string tokenName in tokenNames)
            {
                if (tokens.ContainsKey(tokenName))
                {
                    return tokenName;
                }
            }
            throw new Exception($"token not found {string.Join(", ", tokenNames)} in {string.Join(", ", tokens.Keys)}");
        }

        public static ConnectionType Map(string authType)
        {
            switch (authType)
            {
                case "OAuth":
                    return ConnectionType.OAuth;

                case "AD":
                    return ConnectionType.AD;

                case "IFD":
                    return ConnectionType.IFD;

                case "Certificate":
                    return ConnectionType.Certificate;

                case "ClientSecret":
                    return ConnectionType.ClientSecret;

                case "Office365":
                    return ConnectionType.Office365;

                default:
                    throw new NotImplementedException($"not implemented authType:{authType}");
            }
        }
    }
}