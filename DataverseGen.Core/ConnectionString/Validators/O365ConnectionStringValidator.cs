using System;
using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString.Validators
{
    internal class O365ConnectionStringValidator : BaseConnectionStringValidator,
        IConnectionStringValidator
    {
        public O365ConnectionStringValidator(IDictionary<string, string> connectionStringTokens)
            : base(connectionStringTokens) { }

        public bool Validate()
        {
            CheckIfPasswordIsPresent();
            CheckIfUserNameIsPresent();
            CheckIfUrlIsPresentAndValid();

            return true;
        }

        private void CheckIfPasswordIsPresent()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,
                ConnectionStringConst.Password);
        }

        private void CheckIfUrlIsPresentAndValid()
        {
            KeyValuePair<string, string> tokenFound =
                ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,
                    ConnectionStringConst.Url);
            if (UrlValidator.ValidUrl(tokenFound.Value))
            {
                throw new Exception($"Url not valid {tokenFound.Value} | key: {tokenFound.Key}");
            }
        }

        private void CheckIfUserNameIsPresent()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,
                ConnectionStringConst.UserName);
        }
    }
}