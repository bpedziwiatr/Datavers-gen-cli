using System;
using System.Collections.Generic;

namespace DataverseGen.Core.ConnectionString.Validators
{
    public class CertificateBaseConnectionStringValidator : BaseConnectionStringValidator,
        IConnectionStringValidator
    {
        protected CertificateBaseConnectionStringValidator(
            IDictionary<string, string> connectionStringTokens) : base(connectionStringTokens) { }

        public bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}