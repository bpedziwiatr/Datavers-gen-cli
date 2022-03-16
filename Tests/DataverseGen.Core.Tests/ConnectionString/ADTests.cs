using DataverseGen.Core.ConnectionString;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataverseGen.Core.Tests.ConnectionString
{
    [TestClass]
    public class AdTests
    {
        [TestMethod]
        [DataRow(@"AuthType=AD;Url=https://contoso:8080/Test;")]
        public void AD_ConnectionString_Success(string connectionString)
        {
            new ConnectionStringValidator(connectionString)
                .Validate();
        }
    }
}