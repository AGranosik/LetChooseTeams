﻿using Newtonsoft.Json;

namespace LCT.Domain.Common.BaseTypes
{
    public abstract class BaseId<TKey, TType> : ValueType<TKey>
        where TKey : ValueType<TKey>
    {
        protected BaseId() { }
        protected BaseId(TType id)
        {
            Value = id;
        }
        [JsonProperty]
        public TType Value { get; private set; }
        public override bool Equals(object obj)
        {
            var key = obj as BaseId<TKey, TType>;
            if(key == null)
                return false;

            return Value.Equals(key.Value);
        }
    }
}
