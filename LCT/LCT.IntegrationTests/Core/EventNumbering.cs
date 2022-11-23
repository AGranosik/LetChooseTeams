using System.Linq;
using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using NUnit.Framework;

namespace LCT.IntegrationTests.Core
{
    [TestFixture]
    public class EventNumbering
    {
        private Tournament _tournament; // tested on Tournament which uses Aggreate base cclass

        [SetUp]
        public void SetUp()
            => _tournament = Tournament.Create("test", 3);


        [Test]
        public void EventNumber_TwoEventsAtOnce_Set()
        {
            var changes = _tournament.GetChanges();
            changes.Should().NotBeEmpty();
            changes.Should().HaveCount(2);

            var uniqueNumbers = changes.Select(c => c.EventNumber).Distinct().ToList();
            uniqueNumbers.Should().NotBeEmpty();
            uniqueNumbers.Should().HaveCount(2);

            uniqueNumbers.Any(e => e.Value == 0).Should().BeTrue();
            uniqueNumbers.Any(e => e.Value == 1).Should().BeTrue();
        }

        [Test]
        public void EventNumber_Single_Set()
        {
            _tournament.SetName("fiu fiuf");
            _tournament.AddPlayer("test", "test");
            _tournament.AddPlayer("test2", "test2");

            _tournament.SelectTeam("test", "test", TournamentTeamNames.Teams.First());

            var changes = _tournament.GetChanges();
            changes.Should().NotBeEmpty();
            changes.Should().HaveCount(6);

            var uniqueNumbers = changes.Select(c => c.EventNumber).Distinct().ToList();
            uniqueNumbers.Should().NotBeEmpty();
            uniqueNumbers.Should().HaveCount(6);

            uniqueNumbers.Min().Should().Be(0);
            uniqueNumbers.Max().Should().Be(5);
        }
    }
}
