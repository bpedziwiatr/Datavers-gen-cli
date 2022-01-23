using DataverseGen.Core.ConnectionString.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseGen.Core.ConnectionString
{
    internal class ConnectionStringValidatorFactory
    {
        public static IConnectionStringValidator CreateValidator(
            ConnectionType connectionType,
            IDictionary<string, string> connectionStringTokens)
        {
            switch (connectionType)
            {
                case ConnectionType.AD:
                    throw new NotImplementedException();
                    break;
                case ConnectionType.IFD:
                    throw new NotImplementedException();
                    break;
                case ConnectionType.OAuth:
                    throw new NotImplementedException();
                    break;
                case ConnectionType.Certificate:
                    throw new NotImplementedException();
                    break;
                case ConnectionType.ClientSecret:
                    throw new NotImplementedException();
                    break;
                case ConnectionType.Office365:
                    return new O365ConnectionStringValidator(connectionStringTokens);
                default :
                    throw new NotImplementedException();
            }
        }

        internal static IConnectionStringValidator CreateValidator(
            IDictionary<string, string> connectionStringTokens)
        {
            string tokenName = ConnectionStringConst.GetToken(connectionStringTokens,ConnectionStringConst.Auth);
            string authValue = connectionStringTokens[tokenName];
            ConnectionType connectionType = ConnectionStringConst.Map(authValue);
            return CreateValidator(connectionType,connectionStringTokens);

        }
    }
}