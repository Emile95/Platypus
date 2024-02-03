using PlatypusFramework.Configuration.Application;
using System.Reflection;

namespace Core.Abstract
{
    public interface IApplicationAttributeMethodResolver<AttributeType>
        where AttributeType : Attribute
    {
        void Resolve(PlatypusApplicationBase application, AttributeType attribute, MethodInfo method);
    }
}
