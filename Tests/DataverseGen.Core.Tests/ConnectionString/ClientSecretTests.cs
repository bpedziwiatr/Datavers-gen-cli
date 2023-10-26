using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class ClientSecretTests
{
	[TestMethod]
	[DataRow(@"AuthType=ClientSecret;
  url=https://contosotest.crm.dynamics.com;
  ClientId={AppId};
  ClientSecret={ClientSecret}")]
	[DataRow(
        "AuthType=ClientSecret;url=https://conrosodev.crm4.dynamics.com/main.aspx?appid=256c6c51-9c0a-46d4-8fde-3d9bc0b4af9f;ClientId=6702e612-b6a9-4eed-bfab-967d5565df69;ClientSecret=vz3246578*&*&!;")]
	public void ClientSecret_ConnectionString_Success(string connectionString)
	{
		new ConnectionStringValidator(connectionString)
		   .Validate();
	}
}