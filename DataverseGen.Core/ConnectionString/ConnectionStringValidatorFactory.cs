using DataverseGen.Core.ConnectionString.Validators;
using System;
using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString
{
    internal static class ConnectionStringValidatorFactory
    {
        private static IConnectionStringValidator CreateValidator(
            ConnectionType connectionType,
            IDictionary<string, string> connectionStringTokens)
        {
            switch (connectionType)
            {
                case ConnectionType.AD:
                case ConnectionType.IFD:
                    return new AdConnectionStringValidator(connectionStringTokens);

                case ConnectionType.OAuth:
                    return new OAuthConnectionStringValidator(connectionStringTokens);

                case ConnectionType.Certificate:
                    return new CertificateBaseConnectionStringValidator(connectionStringTokens);

                case ConnectionType.ClientSecret:
                    return new ClientSecretConnectionStringValidator(connectionStringTokens);

                case ConnectionType.Office365:
                    return new O365ConnectionStringValidator(connectionStringTokens);

                default:
                    throw new NotImplementedException();
            }
        }

        internal static IConnectionStringValidator CreateValidator(
            IDictionary<string, string> connectionStringTokens)
        {
            string tokenName =
                ConnectionStringConst.GetToken(connectionStringTokens, ConnectionStringConst.Auth);
            string authValue = connectionStringTokens[tokenName];
            ConnectionType connectionType = ConnectionStringConst.Map(authValue);
            return CreateValidator(connectionType, connectionStringTokens);
        }
    }
}