using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Data;


namespace ClinicalTrials.Tests.MockModels
{
    public class MockClinicalTrialsContext: Mock<DbContext>
    {
        public MockClinicalTrialsContext()
        {
            Setup(x => x.Database).Returns(new MockDatabaseFacade(Object).Object);
        }
    }

    public class MockDatabaseFacade: Mock<DatabaseFacade>
    {
        public MockDatabaseFacade(DbContext context): base(context)
        {
            Setup(x => x.BeginTransaction()).Returns(new MockTransaction().Object);
        }
    }

    public class MockTransaction: Mock<IDbContextTransaction>
    {

    }
}
