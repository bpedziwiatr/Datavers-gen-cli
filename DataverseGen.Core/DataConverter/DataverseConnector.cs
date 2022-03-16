using DataverseGen.Core.ConnectionString;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Threading;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Core.DataConverter
{
    public class DataverseConnector
    {
        private readonly string _connectionString;
        private readonly bool _isConnectionStringValidatorEnabled;
        private CrmServiceClient _crmServiceClient;
        public DataverseConnector(
            string connectionString,
            bool isConnectionStringValidatorEnabled)
        {
            _connectionString = connectionString;
            _isConnectionStringValidatorEnabled = isConnectionStringValidatorEnabled;
            WriteConnectorInfo();
            ValidateConnectionStringIfEnabled();
        }

        public IOrganizationService OrganizationService => _crmServiceClient;
        public void Connect()
        {
            WriteInfo(@"Waiting for connection...");
            _crmServiceClient = new CrmServiceClient(_connectionString);
            WaitForConnection();

            CheckIfThereWasErrorOnConnecting();
        }

        private void CheckIfThereWasErrorOnConnecting()
        {
            if (string.IsNullOrWhiteSpace(_crmServiceClient.LastCrmError))
            {
                return;
            }
            string exceptionMessage =
                $"Connection did not connect with {_connectionString}. LastCrmError: {_crmServiceClient.LastCrmError} | {_crmServiceClient.LastCrmException}";
            WriteError(exceptionMessage);
            throw new Exception(exceptionMessage);
        }

        private void ValidateConnectionStringIfEnabled()
        {
            if (_isConnectionStringValidatorEnabled)
            {
                new ConnectionStringValidator(_connectionString).Validate();
            }
        }

        private void WaitForConnection()
        {
            const int waitForConnection = 1000;
            while (!_crmServiceClient.IsReady)
            {
                WriteInfo($@"Waiting for connection... {waitForConnection}ms");
                Thread.Sleep(waitForConnection);
            }
        }

        private void WriteConnectorInfo()
        {
            WriteInfo($"Validate ConnectionsString {_isConnectionStringValidatorEnabled}");
        }
    }
}