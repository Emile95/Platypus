namespace Core.Abstract
{
    public interface IGetterByGuid<EntityType>
        where EntityType : class
    {
        EntityType Get(string guid);
    }
}
