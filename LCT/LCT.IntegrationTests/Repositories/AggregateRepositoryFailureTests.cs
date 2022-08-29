using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Repositories
{
    [TestFixture]
    public class AggregateRepositoryFailureTests : Testing<Tournament>
    {
        public AggregateRepositoryFailureTests()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();

            //SwapSingleton<Iper>
        }
    }
}
