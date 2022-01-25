using System;
using System.Collections.Generic;
using System.Text;

namespace DataverseGen.Core.ConnectionString.Validators
{
    public class BaseConnectionStringValidator
    {
        protected readonly IDictionary<string, string> ConnectionStringTokens;

        protected BaseConnectionStringValidator(IDictionary<string, string> connectionStringTokens)
        {
            ConnectionStringTokens = connectionStringTokens;
        }
        protected void CheckIfPasswordOrIntegratedSecurityIsPresent()
        {
            bool isPasswordTokenFound = ValidatorTokenHelper.TryCheckIfTokenIsPresentWithValue(
                ConnectionStringTokens,
                ConnectionStringConst.Password,
                out KeyValuePair<string, string> _);
            if (isPasswordTokenFound)
            {
                return;
            }
            bool isIntegratedSecurityToken = ValidatorTokenHelper.TryCheckIfTokenIsPresentWithValue(
                ConnectionStringTokens,
                ConnectionStringConst.IntegratedSecurity,
                out KeyValuePair<string, string> integratedSecurityToken);
            if (!isIntegratedSecurityToken)
            {
                throw new Exception($"IntegratedSecurityToken or Password token aren't present");
            }
            if (integratedSecurityToken.Value == "false")
            {
                throw new Exception(
                    $"Integrated Security Token present but missing password token , integrated token value {integratedSecurityToken.Value}");
            }
        }
        protected void CheckIfPasswordIsPresent()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(ConnectionStringTokens,
                ConnectionStringConst.Password);
        }

        protected KeyValuePair<string, string> CheckIfTokenIsPresentWithValue(
            HashSet<string> searchTokens)
        {
            return ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(ConnectionStringTokens,
                searchTokens);
        }

        protected void CheckIfUrlIsPresentAndValid()
        {
            KeyValuePair<string, string> tokenFound =
                CheckIfTokenIsPresentWithValue(
                    ConnectionStringConst.Url);
            if (!UrlValidator.ValidUrl(tokenFound.Value))
            {
                throw new Exception($"Url not valid {tokenFound.Value} | key: {tokenFound.Key}");
            }
        }

        protected void CheckIfUserNameIsPresent()
        {
            CheckIfTokenIsPresentWithValue(
                ConnectionStringConst.UserName);
        }
    }
}