namespace Sav.Infrastructure.Entities
{
    public abstract class BaseEntity
    {
        public DateTime Inserted { get; set; }

        public DateTime Updated { get; set; }

        public abstract object[] GetKeys();
    }
}
