﻿using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public class MongoPersistanceClient : IPersistanceClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        private readonly IMongoDatabase _database;
        public MongoPersistanceClient(IMongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
            _database = _mongoClient.GetDatabase(_dbName);
            Configure();
        }

        public async Task<bool> CheckUniqness<T>(string entity, string fieldName, T value)
        {
            //create index here? instead of splitting it up in different files
            var collection = _database.GetCollection<T>($"{entity}_{fieldName}_index");
            try
            {
                await collection.InsertOneAsync(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Configure()
        {
            var uniqueIndexOptions = new CreateIndexOptions { Unique = true };

            ConfigureTournamentNameUniqnes(uniqueIndexOptions);
            ConfigureTournamentTeamSelection(uniqueIndexOptions);
            ConfigureTournamentVersion(uniqueIndexOptions);
        }

        public async Task SaveEventAsync<TAggregateRoot>(DomainEvent domainEvent)
            where TAggregateRoot : IAgregateRoot
            => await GetCollection<DomainEvent>($"{typeof(TAggregateRoot).Name}Stream").InsertOneAsync(domainEvent);

        public Task SaveActionAsync<T>(T action) where T : LctAction
            => GetCollection<T>($"{typeof(T).Name}").InsertOneAsync(action);

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _database.GetCollection<T>($"{streamName}");

        public async Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot
        {
            var cursorAsync = await GetCollection<DomainEvent>($"{typeof(T).Name}Stream").FindAsync(s => s.StreamId == streamId);
            return await cursorAsync.ToListAsync();
        }

        private void ConfigureTournamentNameUniqnes(CreateIndexOptions indexOptions)
        {
            var indexModel = new CreateIndexModel<TournamentName>(new BsonDocument("Value", 1), indexOptions);
            _database.GetCollection<TournamentName>("Tournament_TournamentName_index")
                .Indexes.CreateOne(indexModel);
        }

        private void ConfigureTournamentTeamSelection(CreateIndexOptions indexOptions)
        {
            var teamSelectionIndex = Builders<TeamSelectionUniqnessModel>.IndexKeys
                .Ascending(l => l.Team)
                .Ascending(l => l.TournamentId);

            var indexModel = new CreateIndexModel<TeamSelectionUniqnessModel>(
                teamSelectionIndex,
                indexOptions);

            _database.GetCollection<TeamSelectionUniqnessModel>("Tournament_SelectedTeams_index")
                .Indexes.CreateOne(indexModel);
        }

        private void ConfigureTournamentVersion(CreateIndexOptions indexOptions)
        {
            var teamSelectionIndex = Builders<TournamentVersionModel>.IndexKeys
                .Ascending(l => l.TournamentId)
                .Ascending(l => l.Version);

            var indexModel = new CreateIndexModel<TournamentVersionModel>(
                teamSelectionIndex,
                indexOptions);

            _database.GetCollection<TournamentVersionModel>("Tournament_Version_index")
                .Indexes.CreateOne(indexModel);
        }

        public async Task<List<T>> GetActionsAsync<T, TKey>(TKey aggregateId)
             where T : LctAction<TKey>
        {
            var cursorAsync = await GetCollection<T>($"{typeof(T).Name}").FindAsync(t => t.GroupKey.Equals(aggregateId));
            return await cursorAsync.ToListAsync();
        }
    }
}
