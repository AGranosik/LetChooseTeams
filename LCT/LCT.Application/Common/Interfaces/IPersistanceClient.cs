using MongoDB.Driver;

namespace LCT.Application.Common.Interfaces
{
    //i should not use mongo db here but use some ancapsulation?
    public interface IPersistanceClient
    {
        IMongoCollection<T> GetCollection<T>(string streamName);
        Task<bool> CheckUniqness<T>(string entity, string fieldName, T value);
        void Configure();
    }


}
