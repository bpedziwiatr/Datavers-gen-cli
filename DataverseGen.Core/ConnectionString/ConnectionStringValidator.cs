using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataverseGen.Core.ConnectionString
{
    public class ConnectionStringValidator
    {
        private static readonly Regex ConnectionStringRegex = new Regex(@"(?<key>[^=;,]+)=(?<val>[^;,]+(,\d+)?)");
        private readonly string _connectionString;

        public ConnectionStringValidator(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IEnumerable<KeyValuePair<string, string>> ParseConnectionStrings()
        {
            if (!ConnectionStringRegex.IsMatch(_connectionString))
            {
                throw new ArgumentException(@"ConnectionString not valid no match found for tokens!", _connectionString);
            }

            foreach (Match match in ConnectionStringRegex.Matches(_connectionString))
            {
                yield return new KeyValuePair<string, string>(match.Groups["key"]?.Value.Trim(), match.Groups["val"].Value.Trim());
            }
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
            IDictionary<string, string> connectionStringTokens = ParseConnectionStrings()
                .ToDictionary(k => k.Key, v => v.Value);
            IConnectionStringValidator validator = ConnectionStringValidatorFactory
                .CreateValidator(connectionStringTokens);
            return validator.Validate();
        }
    }
}