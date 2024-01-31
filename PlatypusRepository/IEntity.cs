namespace PlatypusRepository
{
    public interface IEntity<IDType>
    {
        IDType GetID();
    }
}
