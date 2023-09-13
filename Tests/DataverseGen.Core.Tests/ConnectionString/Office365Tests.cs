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
}