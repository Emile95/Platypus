namespace PlatypusRepository
{
    public interface IRepositoryRemoveOperator<IDType>
    {
        void Remove(IDType id);
    }
}
