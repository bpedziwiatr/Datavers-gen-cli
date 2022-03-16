using System;
using System.Collections.Generic;
using System.Linq;

namespace DataverseGen.Core.ConnectionString.Validators
{
    internal static class ValidatorTokenHelper
    {
        public static KeyValuePair<string, string> CheckIfTokenIsPresentWithValue(
            IDictionary<string, string> tokens,
            HashSet<string> searchTokens)
        {
            string foundToken = ConnectionStringConst.GetToken(tokens, searchTokens);
            return CheckIfTokenIsPresentWithValue(tokens, foundToken);
        }

        public static KeyValuePair<string, string> CheckIfTokenIsPresentWithValue(
            IDictionary<string, string> tokens,
            string searchToken)
        {
            string tokenName = CheckIfTokenKeyPresent(tokens, searchToken);
            CheckIfTokenValuePresent(tokens, searchToken);
            return tokens.Single(p => p.Key == tokenName);
        }

        public static bool TryCheckIfTokenIsPresentWithValue(
            IDictionary<string, string> tokens,
            HashSet<string> searchTokens,
            out KeyValuePair<string, string> foundToken)
        {
            foundToken = new KeyValuePair<string, string>();
            try
            {
                foundToken = CheckIfTokenIsPresentWithValue(tokens, searchTokens);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryCheckIfTokenIsPresentWithValue(
            IDictionary<string, string> tokens,
            string searchToken,
            out KeyValuePair<string, string> foundToken)
        {
            foundToken = new KeyValuePair<string, string>();
            try
            {
                foundToken = CheckIfTokenIsPresentWithValue(tokens, searchToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string CheckIfTokenKeyPresent(IDictionary<string, string> tokens,
                                                     string searchToken)
        {
            return !tokens.ContainsKey(searchToken)
                ? throw new ArgumentNullException(
                    $"Missing token:{searchToken} Token from connectionstring")
                : searchToken;
        }

        private static void CheckIfTokenValuePresent(IDictionary<string, string> tokens,
                                                     string searchToken)
        {
            string value = tokens[searchToken];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(
                    $"Token {searchToken}, value: {value} IsNullOrWhiteSpace");
            }
        }
    }
}