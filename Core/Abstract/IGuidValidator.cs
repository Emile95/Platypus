namespace Core.Abstract
{
    public interface IGuidValidator<EntityTytpe>
        where EntityTytpe : class
    {
        bool Validate(string guid);
    }
}
