using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Core.Entites
{
    public class Entity
    {
        public Entity(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
