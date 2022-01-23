using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataverseGen.Core.ConnectionString
{
    public class ConnectionStringValidator
    {
        private static Regex ConnectionStringRegex = new Regex(@"(?<key>[^=;,]+)=(?<val>[^;,]+(,\d+)?)");
        private readonly string _connectionString;
        public ConnectionStringValidator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<KeyValuePair<string,string>> ParseConnectionStrings()
        {
            if (!ConnectionStringRegex.IsMatch(_connectionString))
            {
                throw new ArgumentException("ConnectionString not valid no match found for tokens!", _connectionString);
            }
            var names = ConnectionStringRegex.GetGroupNames();
            foreach(Match match in ConnectionStringRegex.Matches(_connectionString))
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
            IDictionary<string,string> connectionStrinTokens = ParseConnectionStrings()
                .ToDictionary(k=>k.Key,v=>v.Value);
            IConnectionStringValidator validator= ConnectionStringValidatorFactory
                .CreateValidator(connectionStrinTokens);
            return validator.Validate();
        }
    }
}