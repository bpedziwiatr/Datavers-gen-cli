using System;
using System.Collections.Generic;
using System.Linq;

namespace DataverseGen.Core.ConnectionString.Validators
{
    internal class ValidatorTokenHelper
    {
        public static KeyValuePair<string, string> CheckIfTokenIsPresentWithValue(IDictionary<string, string> tokens, HashSet<string> searchTokens)
        {
            string foundToken = ConnectionStringConst.GetToken(tokens, searchTokens);
            return CheckIfTokenIsPresentWithValue(tokens, foundToken);
        }

        public static KeyValuePair<string, string> CheckIfTokenIsPresentWithValue(IDictionary<string, string> tokens, string searchToken)
        {
            string tokenName = CheckIfTokenKeyPresnet(tokens, searchToken);
            CheckIfTokenValuePresnet(tokens, searchToken);
            return tokens.Single(p => p.Key == tokenName);
        }

        private static string CheckIfTokenKeyPresnet(IDictionary<string, string> tokens, string searchToken)
        {
            if (!tokens.ContainsKey(searchToken))
            {
                throw new ArgumentNullException($"Missing token:{searchToken} Token from connectionstring");
            }
            return searchToken;
        }

        private static void CheckIfTokenValuePresnet(IDictionary<string, string> tokens, string searchToken)
        {
            string value = tokens[searchToken];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException($"Token {searchToken}, value: {value} IsNullOrWhiteSpace");
            }
        }
    }
}