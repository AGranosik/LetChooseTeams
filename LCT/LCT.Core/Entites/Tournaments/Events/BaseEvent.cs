namespace LCT.Core.Entites.Tournaments.Events
{
    public abstract class BaseEvent
    {
        public string Type => this.GetType().ToString();
        public Guid Id = Guid.NewGuid(); //nanoseconds after request...
        public DateTime TimeStamp = DateTime.UtcNow;
    }
}
