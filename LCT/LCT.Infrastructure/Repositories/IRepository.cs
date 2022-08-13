﻿using LCT.Core.Entites;

namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<T>
        where T : Aggregate
    {
        Task<T> Load(Guid Id);
        Task Save(T model);
    }
}
