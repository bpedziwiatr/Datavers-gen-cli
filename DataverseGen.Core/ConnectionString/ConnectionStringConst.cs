﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DataverseGen.Core.ConnectionString
{
    public static class ConnectionStringConst
    {
        public const string Password = "Password";

        public static readonly HashSet<string> Auth = new HashSet<string>
        {
            "AuthenticationType",
            "AuthType"
        };

        public static readonly HashSet<string> ClientId = new HashSet<string>
        {
            "ClientId",
            "clientid",
            "AppId",
            "appid",
            "ApplicationId",
            "applicationid"
        };

        public static readonly HashSet<string> ClientSecret = new HashSet<string>
        {
            "ClientSecret",
            "clientsecret",
            "Secret",
            "secret"
        };

        public static readonly HashSet<string> IntegratedSecurity = new HashSet<string>
        {
            "Integrated Security",
            "integrated security"
        };

        public static readonly HashSet<string> RedirectUri = new HashSet<string>
        {
            "RedirectUri",
            "redirecturi",
            "ReplyUrl",
            "replyurl"
        };

        public static readonly HashSet<string> TokenCacheStorePath = new HashSet<string>
        {
            "TokenCacheStorePath",
            "Tokencachestorepath"
        };

        public static readonly HashSet<string> Url = new HashSet<string>
        {
            "ServiceUri",
            "serviceuri",
            "Service Uri",
            "service uri",
            "Url",
            "url",
            "Server",
            "server"
        };

        public static readonly HashSet<string> UserName = new HashSet<string>
        {
            "UserName",
            "Username",
            "username",
            "User Name",
            "UserId",
            "userid",
            "User Id"
        };

        internal static readonly HashSet<string> Thumbprint = new HashSet<string>
        {
            "Thumbprint",
            "thumbprint",
            "CertThumbprint",
            "certthumbprint"
        };

        public static string GetToken(IDictionary<string, string> tokens,
                                      HashSet<string> tokenNames)
        {
            foreach (string tokenName in tokenNames.Where(
                         tokens.ContainsKey))
            {
                return tokenName;
            }

            throw new Exception(
                $"token not found {string.Join(", ", tokenNames)} in {string.Join(", ", tokens.Keys)}");
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