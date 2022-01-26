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
        [DataRow("AuthType=ClientSecret;url=https://rwrgbsmdrsdev.crm4.dynamics.com/main.aspx?appid=bcedab73-941b-ec11-b6e6-000d3adbbffa;ClientId=27aa45a2-7b10-15f3-b123-53b53de124e1;ClientSecret=vz3246578*&*&!;")]

        public void ClientSecret_ConnectionString_Success(string connectionString)
        {
            new ConnectionStringValidator(connectionString)
                .Validate();
        }
    }
}