using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class AdTests
{
	[TestMethod]
	[DataRow(@"AuthType=AD;Url=https://contoso:8080/Test;")]
	[DataRow(@"AuthType=AD;Url=http://contoso:8080/Test;")]
	[DataRow(@"AuthType=IFD;Url=https://contoso.crm.dynamics.com;")]
	public void AD_ConnectionString_Success(string connectionString)
	{
		new ConnectionStringValidator(connectionString)
		   .Validate();
	}

	[TestMethod]
	public void AD_MissingUrl_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator("AuthType=AD;").Validate());
	}

	[TestMethod]
	public void AD_InvalidUrlScheme_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator("AuthType=AD;Url=ftp://contoso:8080/Test;").Validate());
	}

	[TestMethod]
	public void AD_MalformedUrl_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator("AuthType=AD;Url=not-a-url;").Validate());
	}

	[TestMethod]
	public void AD_WhitespaceUrlValue_Throws()
	{
		// Tests ValidatorTokenHelper.CheckIfTokenValuePresent — empty-after-trim path
		Assert.ThrowsException<ArgumentNullException>(() =>
			new ConnectionStringValidator("AuthType=AD;Url= ;").Validate());
	}
}