using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataverseGen.Core.ConnectionString
{
    internal class ConnectionStringValidator
    {
        private static Regex ConnectionStringRegex = new Regex(@"(?<key>[^=;,]+)=(?<val>[^;,]+(,\d+)?)");
        private readonly string _connectionString;
        public ConnectionStringValidator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDictionary<string, string> ParseConnectionStrings()
        {
            if (!ConnectionStringRegex.IsMatch(_connectionString))
            {
                throw new ArgumentException("ConnectionString not valid no match found for tokens!", _connectionString);
            }
            return ConnectionStringRegex.Matches(_connectionString)
                 .Cast<Match>()
                 .ToDictionary(k => k.Name, v => v.Value);
        }

        public bool TryValidate()
        {
            try
            {
                return InternalValidate();
            }
            catch (Exception ex)
            {
                ColorConsole.WriteError(ex.ToString());
                return false;
            }
        }

        public void Validate()
        {
            InternalValidate();
        }
        private bool InternalValidate()
        {
            IDictionary<string,string> connectionStrinTokens = ParseConnectionStrings();
            IConnectionStringValidator validator= ConnectionStringValidatorFactory
                .CreateValidator(connectionStrinTokens);
            return validator.Validate();
        }
    }
}