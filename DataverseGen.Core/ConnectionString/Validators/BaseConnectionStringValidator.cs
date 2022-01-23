using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseGen.Core.ConnectionString.Validators
{
    internal class BaseConnectionStringValidator
    {
        protected readonly IDictionary<string, string> _connectionStringTokens;

        public BaseConnectionStringValidator(IDictionary<string,string> connectionStringTokens)
        {
            _connectionStringTokens = connectionStringTokens;
        }
    }
}
