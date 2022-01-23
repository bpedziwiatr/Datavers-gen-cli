using DataverseGen.Core.ConnectionString;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataverseGen.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow(@"AuthType=Office365;
        Username=jsmith@contoso.onmicrosoft.com; 
        Password=passcode;
        Url=https://contoso.crm.dynamics.com")]
        public void TestMethod1(string connectionString)
        {
            ConnectionStringValidator connection =
                new ConnectionStringValidator(connectionString);
            connection.Validate();
        }
    }
}