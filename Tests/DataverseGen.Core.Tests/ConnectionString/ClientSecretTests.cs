using DataverseGen.Core.ConnectionString;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataverseGen.Core.Tests.ConnectionString
{
    [TestClass]
    public class ClientSecretTests
    {
        [TestMethod]
        [DataRow(@"AuthType=ClientSecret;
  url=https://contosotest.crm.dynamics.com;
  ClientId={AppId};
  ClientSecret={ClientSecret}")]
        [DataRow("AuthType=ClientSecret;url=https://contosotest.crm4.dynamics.com/main.aspx?appid=cbeoar26-931b-ec11-b6e6-020d3adbbfda;ClientId=6702e612-b6a9-4eed-bfab-967d5565df69;ClientSecret=vz3246578*&*&!;")]

        public void ClientSecret_ConnectionString_Success(string connectionString)
        {
            new ConnectionStringValidator(connectionString)
                .Validate();
        }
    }
}