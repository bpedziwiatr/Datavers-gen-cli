using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseGen.Core.ConnectionString.Validators
{
    internal class O365ConnectionStringValidator :BaseConnectionStringValidator, IConnectionStringValidator
    {
        

        public O365ConnectionStringValidator(IDictionary<string, string> connectionStringTokens)
            : base(connectionStringTokens)
        {
        }

        public bool Validate()
        {
            CheckIfPasswordIsPresent();
            CheckIfUserNameIsPresnet();
            CheckIfUrlIsPresentAndValid();

            return true;
        }

        private void CheckIfUrlIsPresentAndValid()
        {
            var tokenFound = ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,ConnectionStringConst.Url);
            //todo validate Uri
        }

        private void CheckIfUserNameIsPresnet()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,ConnectionStringConst.UserName);
        }

        private void CheckIfPasswordIsPresent()
        {
            ValidatorTokenHelper.CheckIfTokenIsPresentWithValue(_connectionStringTokens,ConnectionStringConst.Password);
        }
    }
}
