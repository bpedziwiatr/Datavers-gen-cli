using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class ConnectionStringConstTests
{
	[TestMethod]
	[DataRow("OAuth", ConnectionType.OAuth)]
	[DataRow("AD", ConnectionType.AD)]
	[DataRow("IFD", ConnectionType.IFD)]
	[DataRow("Certificate", ConnectionType.Certificate)]
	[DataRow("ClientSecret", ConnectionType.ClientSecret)]
	[DataRow("Office365", ConnectionType.Office365)]
	public void Map_ValidAuthType_ReturnsExpectedConnectionType(string authType, ConnectionType expected)
	{
		ConnectionType result = ConnectionStringConst.Map(authType);

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow("Basic")]
	[DataRow("oauth")]
	[DataRow("OAUTH")]
	[DataRow("")]
	public void Map_UnknownAuthType_ThrowsNotImplementedException(string authType)
	{
		Assert.ThrowsException<NotImplementedException>(() => ConnectionStringConst.Map(authType));
	}

	[TestMethod]
	public void GetToken_PrimaryKeyPresent_ReturnsKey()
	{
		var tokens = new Dictionary<string, string> { { "AuthType", "AD" } };

		string result = ConnectionStringConst.GetToken(tokens, ConnectionStringConst.Auth);

		Assert.AreEqual("AuthType", result);
	}

	[TestMethod]
	public void GetToken_AlternativeKeyPresent_ReturnsAlternativeKey()
	{
		var tokens = new Dictionary<string, string> { { "AuthenticationType", "AD" } };

		string result = ConnectionStringConst.GetToken(tokens, ConnectionStringConst.Auth);

		Assert.AreEqual("AuthenticationType", result);
	}

	[TestMethod]
	public void GetToken_TokenNotPresent_ThrowsException()
	{
		var tokens = new Dictionary<string, string> { { "Url", "https://contoso.crm.dynamics.com" } };

		Assert.ThrowsException<Exception>(() =>
			ConnectionStringConst.GetToken(tokens, ConnectionStringConst.Auth));
	}

	[TestMethod]
	public void GetToken_EmptyTokens_ThrowsException()
	{
		var tokens = new Dictionary<string, string>();

		Assert.ThrowsException<Exception>(() =>
			ConnectionStringConst.GetToken(tokens, ConnectionStringConst.Auth));
	}
}
