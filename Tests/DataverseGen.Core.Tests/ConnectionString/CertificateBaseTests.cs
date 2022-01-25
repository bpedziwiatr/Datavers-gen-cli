using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataverseGen.Core.ConnectionString;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataverseGen.Core.Tests.ConnectionString
{
    [TestClass]
    public class CertificateBaseTests
    {
        [TestMethod]
        [DataRow(@"AuthType=Certificate;
  url=https://contosotest.crm.dynamics.com;
  thumbprint={CertThumbPrintId};
  ClientId={AppId};")]
        public void CertificateBase_Success(string connectionString)
        {
            new ConnectionStringValidator(connectionString)
                .Validate();
        } 
    }
}
