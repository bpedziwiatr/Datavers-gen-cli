using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString.Validators
{
    public class O365ConnectionStringValidator : BaseConnectionStringValidator,
        IConnectionStringValidator
    {
        public O365ConnectionStringValidator(IDictionary<string, string> connectionStringTokens)
            : base(connectionStringTokens) { }

        public bool Validate()
        {
            CheckIfPasswordOrIntegratedSecurityIsPresent();
            CheckIfUserNameIsPresent();
            CheckIfUrlIsPresentAndValid();

            return true;
        }
    }
}