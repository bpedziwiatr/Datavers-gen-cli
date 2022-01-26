using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString.Validators
{
    public class OAuthConnectionStringValidator : BaseConnectionStringValidator,
        IConnectionStringValidator
    {
        public OAuthConnectionStringValidator(IDictionary<string, string> connectionStringTokens) :
            base(connectionStringTokens) { }

        public bool Validate()
        {
            CheckIfPasswordOrIntegratedSecurityIsPresent();
            CheckIfUserNameIsPresent();
            CheckIfUrlIsPresentAndValid();
            CheckIfAppIdIsPresent();
            CheckIfRedirectUriIsPresent();
            CheckIfTokenCacheStorePathIsPresent();
            return true;
        }

        private void CheckIfAppIdIsPresent()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(ConnectionStringTokens,
                ConnectionStringConst.ClientId);
        }

        private void CheckIfRedirectUriIsPresent()
        {
            CheckIfTokenIsPresentWithValue(ConnectionStringConst.RedirectUri);
        }

        private void CheckIfTokenCacheStorePathIsPresent()
        {
            CheckIfTokenIsPresentWithValue(ConnectionStringConst.TokenCacheStorePath);
        }
    }
}