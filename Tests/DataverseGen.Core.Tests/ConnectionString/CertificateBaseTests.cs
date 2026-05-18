using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class CertificateBaseTests
{
	[TestMethod]
	[DataRow(@"AuthType=Certificate;
  url=https://contosotest.crm.dynamics.com;
  thumbprint={CertThumbPrintId};
  ClientId={AppId};")]
	public void CertificateBase_ConnectionString_Success(string connectionString)
	{
		new ConnectionStringValidator(connectionString)
		   .Validate();
	}

	[TestMethod]
	public void Certificate_MissingThumbprint_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Certificate;url=https://contoso.crm.dynamics.com;ClientId=abc;").Validate());
	}

	[TestMethod]
	public void Certificate_MissingClientId_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Certificate;url=https://contoso.crm.dynamics.com;thumbprint=ABC123;").Validate());
	}
}