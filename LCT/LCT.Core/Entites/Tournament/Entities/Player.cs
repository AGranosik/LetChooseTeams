using LCT.Core.Entites.Tournament.ValueObjects;

namespace LCT.Core.Entites.Tournament.Entities
{
    public class Player : Entity
    {
        public Player(Guid id, FullName name): base(id){
        
        }
        public FullName Name { get; private set; }
    }
}
