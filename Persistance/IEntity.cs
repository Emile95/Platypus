namespace Persistance
{
    public interface IEntity<IDType>
    {
        IDType GetID();
    }
}
