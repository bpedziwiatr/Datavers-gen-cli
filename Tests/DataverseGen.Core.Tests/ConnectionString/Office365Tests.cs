using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class Office365Tests
{
	[TestMethod]
	[DataRow(@"AuthType=Office365;
        Username=jsmith@contoso.onmicrosoft.com;
        Password=passcode;
        Url=https://contoso.crm.dynamics.com")]
	[DataRow(
		@"AuthType=Office365;Username=jsmith@contoso.onmicrosoft.com; Password=passc$!&ode;^;Url=https://192.168.13.1")]
	public void Office365_ConnectionString_Success(string connectionString)
	{
		new ConnectionStringValidator(connectionString)
		   .Validate();
	}

	[TestMethod]
	public void Office365_MissingUrl_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Office365;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;").Validate());
	}

	[TestMethod]
	public void Office365_MissingUsername_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Office365;Password=passcode;Url=https://contoso.crm.dynamics.com;").Validate());
	}

	[TestMethod]
	public void Office365_NoPasswordAndNoIntegratedSecurity_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Office365;Username=jsmith@contoso.onmicrosoft.com;Url=https://contoso.crm.dynamics.com;").Validate());
	}

	[TestMethod]
	public void Office365_IntegratedSecurityFalseWithoutPassword_Throws()
	{
		Assert.ThrowsException<Exception>(() =>
			new ConnectionStringValidator(
				"AuthType=Office365;Username=jsmith@contoso.onmicrosoft.com;Integrated Security=false;Url=https://contoso.crm.dynamics.com;").Validate());
	}
}