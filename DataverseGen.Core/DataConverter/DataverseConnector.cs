using DataverseGen.Core.ConnectionString;
using Microsoft.PowerPlatform.Dataverse.Client;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Core.DataConverter;

public class DataverseConnector
{
	private readonly string _connectionString;
	private readonly bool _isConnectionStringValidatorEnabled;

	public DataverseConnector(
		string connectionString,
		bool isConnectionStringValidatorEnabled)
	{
		_connectionString = connectionString;
		_isConnectionStringValidatorEnabled = isConnectionStringValidatorEnabled;
		WriteConnectorInfo();
		ValidateConnectionStringIfEnabled();
	}

	public ServiceClient OrganizationService { get; private set; }

	public void Connect()
	{
		WriteInfo(@"Waiting for connection...");
		OrganizationService = new ServiceClient(_connectionString);
		WaitForConnection();

		CheckIfThereWasErrorOnConnecting();
	}

	private void CheckIfThereWasErrorOnConnecting()
	{
		if (string.IsNullOrWhiteSpace(OrganizationService.LastError))
		{
			return;
		}

		string exceptionMessage =
			$"Connection did not connect with {_connectionString}. LastCrmError: {OrganizationService.LastError} | {OrganizationService.LastException}";

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

		while (!OrganizationService.IsReady)
		{
			WriteInfo($@"Waiting for connection... {waitForConnection}ms");

			if (OrganizationService.LastException != null)
			{
				WriteError(OrganizationService.LastException.Message);

				throw OrganizationService.LastException;
			}

			Thread.Sleep(waitForConnection);
		}
	}

	private void WriteConnectorInfo()
	{
		WriteInfo($"Validate ConnectionsString {_isConnectionStringValidatorEnabled}");
	}
}